using UnityEngine;
using TMPro;
using System.Collections;
// DOTween varsa kullanabilirsin, yoksa Coroutine iş görür. 
// Şimdilik Coroutine (saf C#) ile yapıyoruz ki hata almayasın.

public class TimeBankManager : MonoBehaviour
{
    public static TimeBankManager Instance;

    [Header("UI Bileşeni")]
    public TextMeshProUGUI kumbaraText; 

    [Header("Animasyon Ayarı")]
    public float animasyonSuresi = 1.0f; // Sayılar ne kadar sürede artsın?

    private int hedefDeger;
    private int baslangicDegeri;
    private int guncelGosterilenDeger;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 1. Verileri Oku
        hedefDeger = PlayerPrefs.GetInt("TimeBank", 0);
        int sonKazanilan = PlayerPrefs.GetInt("SonKazanilan", 0);

        // 2. Başlangıç Değerini Hesapla
        baslangicDegeri = hedefDeger - sonKazanilan;
        if (baslangicDegeri < 0) baslangicDegeri = 0;
        
        guncelGosterilenDeger = baslangicDegeri;

        // 3. Ekrana önce başlangıç değerini yaz
        UpdateUI();

        // 4. Eğer yeni bir kazanç varsa animasyonu başlat
        if (sonKazanilan > 0)
        {
            StartCoroutine(SayacAnimasyonu(baslangicDegeri, hedefDeger));
            
            // Animasyon oynadı, artık sıfırla
            PlayerPrefs.SetInt("SonKazanilan", 0);
            PlayerPrefs.Save();
        }
        else
        {
            // Kazanç yoksa direkt hedefi yaz
            guncelGosterilenDeger = hedefDeger;
            UpdateUI();
        }
    }

    // --- İŞTE ÇARKIFELEK SCRIPTININ ARADIĞI FONKSİYON ---
    public void SayaciGuncelle()
    {
        // Yeni hafızayı oku (Çarkıfelek parayı kestiği için değer değişti)
        int yeniHedef = PlayerPrefs.GetInt("TimeBank", 0);
        
        // Şu an ekranda yazan değerden yeni değere doğru animasyon başlat
        StopAllCoroutines();
        StartCoroutine(SayacAnimasyonu(guncelGosterilenDeger, yeniHedef));
    }

    IEnumerator SayacAnimasyonu(int baslangic, int bitis)
    {
        float gecenSure = 0;
        
        while (gecenSure < animasyonSuresi)
        {
            gecenSure += Time.deltaTime;
            float t = gecenSure / animasyonSuresi;
            
            // Animasyon eğrisi (Yumuşak geçiş)
            t = Mathf.SmoothStep(0, 1, t);

            guncelGosterilenDeger = (int)Mathf.Lerp(baslangic, bitis, t);
            UpdateUI();
            
            yield return null;
        }

        // Garanti olsun diye döngü bitince net değeri yaz
        guncelGosterilenDeger = bitis;
        hedefDeger = bitis; // Hedef değerimizi de güncelleyelim
        UpdateUI();
    }

    void UpdateUI()
    {
        if (kumbaraText != null)
            kumbaraText.text = guncelGosterilenDeger.ToString();
    }
}