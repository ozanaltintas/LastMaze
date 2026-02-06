using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic; // Liste için gerekli

public class ElasticHaloManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MenuController menuController;
    [SerializeField] private RectTransform haloTransform;
    [SerializeField] private GameObject settingsWheelObject;

    [Header("Target Filtering (YENİ)")]
    [Tooltip("Halo sadece bu listedeki objelere gider. İzin verilen butonları buraya sürükleyin.")]
    public List<GameObject> allowedButtons; 

    [Header("Elastic Settings")]
    [SerializeField] private float posSmoothTime = 0.05f; 
    [SerializeField] private float sizeSmoothTime = 0.08f; 
    [SerializeField] private float stretchIntensity = 0.3f;
    [SerializeField] private Vector2 padding = new Vector2(20, 20);

    private Vector3 posVelocity;
    private Vector2 sizeVelocity;
    private RectTransform targetRect;
    private Vector3 lastPosition;

    void Start()
    {
        if (haloTransform != null) haloTransform.gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        // 1. Menü Kapalı Kontrolü
        if (menuController == null || !menuController.IsMenuOpen())
        {
            if (haloTransform != null && haloTransform.gameObject.activeSelf) 
                haloTransform.gameObject.SetActive(false);
            targetRect = null;
            return;
        }

        // 2. Anlık Seçili Objeyi Al
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
        
        // 3. FİLTRELEME: Seçili obje "İzin Verilenler" listesinde mi?
        bool isAllowed = false;
        
        if (currentSelected != null)
        {
            // Eğer liste boşsa hepsine izin ver (Hata önlemek için), doluysa sadece listedekilere
            if (allowedButtons.Count == 0 || allowedButtons.Contains(currentSelected))
            {
                isAllowed = true;
                targetRect = currentSelected.GetComponent<RectTransform>();
            }
        }

        // 4. Duruma Göre Halo'yu Aç/Kapat ve Hareket Ettir
        if (isAllowed && targetRect != null)
        {
            // Halo kapalıysa aç
            if (!haloTransform.gameObject.activeSelf) 
            {
                haloTransform.gameObject.SetActive(true);
                // İlk açılışta ışınlan (animasyon bozulmasın)
                haloTransform.position = targetRect.position;
                haloTransform.sizeDelta = targetRect.sizeDelta + padding;
                lastPosition = haloTransform.position;
            }

            MoveAndStretchHalo();
        }
        else
        {
            // İzin verilmeyen bir şey seçildiyse veya seçim yoksa Halo'yu gizle
            if (haloTransform.gameObject.activeSelf) 
                haloTransform.gameObject.SetActive(false);
        }
    }

    void MoveAndStretchHalo()
    {
        // SmoothDamp: Hedefe kilitlenmeyi sağlar
        Vector3 targetPos = targetRect.position;
        Vector2 targetSize = targetRect.sizeDelta + padding;

        haloTransform.position = Vector3.SmoothDamp(haloTransform.position, targetPos, ref posVelocity, posSmoothTime);
        haloTransform.sizeDelta = Vector2.SmoothDamp(haloTransform.sizeDelta, targetSize, ref sizeVelocity, sizeSmoothTime);

        // Hız bazlı esneme (Squash & Stretch)
        float movementSpeed = (haloTransform.position - lastPosition).magnitude / Time.deltaTime;
        Vector3 moveDir = (haloTransform.position - lastPosition).normalized;

        if (movementSpeed > 5f) 
        {
            float stretch = 1f + (movementSpeed * stretchIntensity * 0.0005f);
            float squash = 1f / stretch;
            float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            
            haloTransform.rotation = Quaternion.Euler(0, 0, angle);
            haloTransform.localScale = new Vector3(stretch, squash, 1f);
        }
        else
        {
            // Durduğunda hızlıca düzel
            haloTransform.localScale = Vector3.Lerp(haloTransform.localScale, Vector3.one, Time.deltaTime * 20f);
            haloTransform.rotation = Quaternion.Lerp(haloTransform.rotation, Quaternion.identity, Time.deltaTime * 20f);
        }

        lastPosition = haloTransform.position;
    }
}