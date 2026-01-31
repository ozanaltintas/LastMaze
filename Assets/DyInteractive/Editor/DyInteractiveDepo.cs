using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

[InitializeOnLoad]
public class DyInteractiveDepo : EditorWindow
{
    private string searchQuery = "";
    private string baseUrl = "https://depo.shapemaster.net/";
    private string myApiKey = "1122Safak33";
    
    private List<AssetData> allAssets = new List<AssetData>();
    private Dictionary<string, bool> categoryFoldout = new Dictionary<string, bool>();
    private Dictionary<string, Texture2D> thumbCache = new Dictionary<string, Texture2D>();
    private Vector2 scrollPos;
    private AudioSource previewSource;

    private string pendingFilePath = "";
    private string pendingTags = "";
    private bool isWaitingForTags = false;

    static DyInteractiveDepo()
    {
        EditorApplication.delayCall += () => {
            if (!HasOpenInstances<DyInteractiveDepo>()) ShowWindow();
        };
    }

    [MenuItem("Dy Interactive/Asset Deposu")]
    public static void ShowWindow()
    {
        System.Type sceneViewType = typeof(SceneView);
        System.Type gameViewType = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        DyInteractiveDepo window = GetWindow<DyInteractiveDepo>("Dy Interactive", sceneViewType, gameViewType);
        window.minSize = new Vector2(400, 600);
        window.FetchAssets();
    }

    private void OnGUI()
    {
        if (isWaitingForTags) { DrawTagEntryArea(); return; }

        DrawUploadArea();
        GUILayout.Space(10);
        DrawSearchBar();
        GUILayout.Space(5);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        var groupedAssets = allAssets.GroupBy(a => string.IsNullOrEmpty(a.category_name) ? "Genel" : a.category_name);

        foreach (var group in groupedAssets)
        {
            string catName = group.Key;
            if (!categoryFoldout.ContainsKey(catName)) categoryFoldout[catName] = true;
            categoryFoldout[catName] = EditorGUILayout.Foldout(categoryFoldout[catName], $"{catName.ToUpper()} ({group.Count()})", true, EditorStyles.foldoutHeader);
            
            if (categoryFoldout[catName])
            {
                EditorGUI.indentLevel++;
                foreach (var asset in group) DrawAssetCard(asset);
                EditorGUI.indentLevel--;
            }
        }
        EditorGUILayout.EndScrollView();
    }

    private void UploadAsset(string localPath, string tags)
    {
        byte[] fileData = File.ReadAllBytes(localPath);
        string ext = Path.GetExtension(localPath).ToLower().Replace(".", "");
        
        // 3D MODEL & PREFAB ÖNİZLEME OLUŞTURMA
        byte[] thumbData = null;
        if (ext == "fbx" || ext == "obj" || ext == "prefab") 
        {
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(localPath);
            if (obj != null) 
            {
                Texture2D preview = AssetPreview.GetAssetPreview(obj);
                int wait = 0;
                while (preview == null && wait < 50) { System.Threading.Thread.Sleep(20); preview = AssetPreview.GetAssetPreview(obj); wait++; }
                if (preview != null) thumbData = preview.EncodeToJPG();
            }
        }

        // --- GELİŞMİŞ KATEGORİ SİSTEMİ ---
        string catId = "1"; // Varsayılan (Genel/Diğer)
        if (ext == "fbx" || ext == "obj") catId = "1"; // 3D Model
        else if (ext == "wav" || ext == "mp3" || ext == "ogg") catId = "2"; // Ses
        else if (ext == "png" || ext == "jpg" || ext == "tga" || ext == "psd") catId = "3"; // Görsel
        else if (ext == "unitypackage" || ext == "prefab") catId = "4"; // Paket/Prefab
        else if (ext == "cs") catId = "5"; // Script (Yeni kategori eklemelisin)
        else if (ext == "shader" || ext == "mat") catId = "6"; // Shader/Material

        WWWForm form = new WWWForm();
        form.AddField("key", myApiKey);
        form.AddField("name", Path.GetFileNameWithoutExtension(localPath));
        form.AddField("cat_id", catId);
        form.AddField("tags", tags);
        form.AddBinaryData("file", fileData, Path.GetFileName(localPath));
        if (thumbData != null) form.AddBinaryData("thumb", thumbData, "thumb.jpg", "image/jpeg");

        UnityWebRequest www = UnityWebRequest.Post(baseUrl + "api/upload_api.php", form);
        var op = www.SendWebRequest();

        while (!op.isDone) EditorUtility.DisplayProgressBar("Dy Interactive", "Yükleniyor...", www.uploadProgress);
        EditorUtility.ClearProgressBar();

        if (www.result == UnityWebRequest.Result.Success && www.downloadHandler.text.Contains("SUCCESS"))
        {
            Debug.Log("<color=cyan><b>[Dy Interactive]</b></color> Yükleme Başarılı!");
            searchQuery = "";
            FetchAssets();
        }
        else Debug.LogError("Hata: " + www.downloadHandler.text);
    }

    // --- YARDIMCI METOTLAR (FetchAssets, DrawCard vb. aynı yapıda kalıyor) ---
    private void DrawUploadArea() {
        Event evt = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 65.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "YÜKLEMEK İÇİN DOSYALARI BURAYA SÜRÜKLEYİN\n(Script, Shader, Model, Ses vb.)", new GUIStyle(EditorStyles.helpBox) { alignment = TextAnchor.MiddleCenter });
        if ((evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform) && dropArea.Contains(evt.mousePosition)) {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            if (evt.type == EventType.DragPerform) {
                DragAndDrop.AcceptDrag();
                if (DragAndDrop.paths.Length > 0) { pendingFilePath = DragAndDrop.paths[0]; pendingTags = ""; isWaitingForTags = true; }
            }
        }
    }

    private void DrawTagEntryArea() {
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Yeni Varlık Bilgileri", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Dosya:", Path.GetFileName(pendingFilePath));
        pendingTags = EditorGUILayout.TextField("Etiketler:", pendingTags);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Yüklemeyi Başlat", GUILayout.Height(30))) { UploadAsset(pendingFilePath, pendingTags); isWaitingForTags = false; }
        if (GUILayout.Button("İptal", GUILayout.Height(30))) isWaitingForTags = false;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void DrawSearchBar() {
        EditorGUILayout.BeginHorizontal();
        searchQuery = EditorGUILayout.TextField(searchQuery, GUI.skin.FindStyle("SearchTextField"), GUILayout.Height(20));
        if (GUILayout.Button("Ara", GUILayout.Width(60), GUILayout.Height(20))) FetchAssets();
        if (GUILayout.Button("Tümü", GUILayout.Width(50), GUILayout.Height(20))) { searchQuery = ""; FetchAssets(); }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawAssetCard(AssetData asset) {
        EditorGUILayout.BeginHorizontal("helpbox", GUILayout.Height(95));
        Rect thumbRect = GUILayoutUtility.GetRect(85, 85);
        if (thumbCache.ContainsKey(asset.thumbnail_url)) GUI.DrawTexture(thumbRect, thumbCache[asset.thumbnail_url], ScaleMode.ScaleToFit);
        else if (!string.IsNullOrEmpty(asset.thumbnail_url)) { GUI.Box(thumbRect, "..."); DownloadThumbnail(asset.thumbnail_url); }
        else GUI.Box(thumbRect, asset.file_type.ToUpper());
        EditorGUILayout.BeginVertical();
        GUILayout.Space(5);
        GUILayout.Label(asset.asset_name, EditorStyles.boldLabel);
        GUILayout.Label($"Tip: {asset.file_type.ToUpper()} | Etiket: {asset.tags}", EditorStyles.miniLabel);
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("İndir", GUILayout.Height(28))) DownloadAndImport(asset);
        if ((asset.file_type == "wav" || asset.file_type == "mp3") && GUILayout.Button("▶", GUILayout.Width(35), GUILayout.Height(28))) PlayPreview(baseUrl + asset.file_url);
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    private void FetchAssets() {
        UnityWebRequest www = UnityWebRequest.Get($"{baseUrl}api/search_assets.php?query={UnityWebRequest.EscapeURL(searchQuery)}");
        www.SendWebRequest();
        while (!www.isDone) { }
        if (www.result == UnityWebRequest.Result.Success) {
            allAssets = JsonUtility.FromJson<AssetList>("{\"items\":" + www.downloadHandler.text + "}").items;
            Repaint();
        }
    }

    private void DownloadAndImport(AssetData asset) {
        string safeName = asset.asset_name.Replace(" ", "_") + "." + asset.file_type;
        string tempFile = Path.Combine(Application.temporaryCachePath, safeName);
        UnityWebRequest www = UnityWebRequest.Get(baseUrl + asset.file_url);
        www.SendWebRequest();
        while (!www.isDone) EditorUtility.DisplayProgressBar("İndiriliyor", asset.asset_name, www.downloadProgress);
        EditorUtility.ClearProgressBar();
        if (www.result == UnityWebRequest.Result.Success) {
            File.WriteAllBytes(tempFile, www.downloadHandler.data);
            if (asset.file_type.ToLower() == "unitypackage") AssetDatabase.ImportPackage(tempFile, true);
            else {
                string targetDir = "Assets/DyInteractive_Assets";
                if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);
                File.Copy(tempFile, Path.Combine(targetDir, safeName), true);
                AssetDatabase.Refresh();
            }
        }
    }

    private void PlayPreview(string url) {
        if (previewSource == null) { GameObject go = new GameObject("AudioPreview_Dy"); go.hideFlags = HideFlags.HideAndDontSave; previewSource = go.AddComponent<AudioSource>(); }
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.UNKNOWN);
        www.SendWebRequest();
        while (!www.isDone) { }
        if (www.result == UnityWebRequest.Result.Success) { previewSource.clip = DownloadHandlerAudioClip.GetContent(www); previewSource.Play(); }
    }

    private void DownloadThumbnail(string thumbUrl) {
        if (thumbCache.ContainsKey(thumbUrl)) return;
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(baseUrl + thumbUrl);
        var op = www.SendWebRequest();
        op.completed += (a) => { if (www.result == UnityWebRequest.Result.Success) { thumbCache[thumbUrl] = DownloadHandlerTexture.GetContent(www); Repaint(); } };
    }

    [Serializable] public class AssetData { public string asset_name, file_url, thumbnail_url, file_type, tags, category_name; }
    [Serializable] public class AssetList { public List<AssetData> items; }
}