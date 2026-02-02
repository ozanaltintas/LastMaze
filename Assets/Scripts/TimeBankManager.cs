using UnityEngine;
using TMPro;
using System.Collections;

public class TimeBankManager : MonoBehaviour
{
    public static TimeBankManager Instance;

    [Header("UI Bileşeni")]
    public TextMeshProUGUI kumbaraText; 

    [Header("Animasyon Ayarı")]
    public float animasyonSuresi = 1.5f; // Sayılar ne kadar sürede artsın?

    private int hedefDeger;
    private int baslangicDegeri;

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

        // 2. Başlangıç Değerini Hesapla (Toplam - SonKazanılan)
        baslangicDegeri = hedefDeger - sonKazanilan;

        // Eksiye düşme kontrolü (olur da veri silinirse)
        if (baslangicDegeri < 0) baslangicDegeri = 0;

        // 3. Ekrana önce başlangıç değerini yaz (Örn: 100)
        if (kumbaraText != null)
            kumbaraText.text = baslangicDegeri.ToString();

        // 4. Eğer yeni bir kazanç varsa animasyonu başlat
        if (sonKazanilan > 0)
        {
            StartCoroutine(SayacAnimasyonu());
            
            // Animasyon bir kere oynadıktan sonra "SonKazanilan"ı sıfırla.
            // Böylece oyunu kapatıp açarsan tekrar animasyon oynamaz.
            PlayerPrefs.SetInt("SonKazanilan", 0);
            PlayerPrefs.Save();
        }
        else
        {
            // Kazanç yoksa direkt hedefi yaz
             if (kumbaraText != null)
                kumbaraText.text = hedefDeger.ToString();
        }
    }

    IEnumerator SayacAnimasyonu()
    {
        // Sahne yüklenir yüklenmez başlamasın, yarım saniye nefes alsın (göz görsün)
        yield return new WaitForSeconds(0.5f);

        float gecenSure = 0;
        
        while (gecenSure < animasyonSuresi)
        {
            gecenSure += Time.deltaTime;
            float t = gecenSure / animasyonSuresi;
            
            // Animasyon eğrisi (yavaşlayarak bitmesi için SmoothStep)
            t = Mathf.SmoothStep(0, 1, t);

            int anlikDeger = (int)Mathf.Lerp(baslangicDegeri, hedefDeger, t);
            
            if (kumbaraText != null)
                kumbaraText.text = anlikDeger.ToString();
            
            yield return null;
        }

        // Garanti olsun diye döngü bitince net değeri yaz
        if (kumbaraText != null)
            kumbaraText.text = hedefDeger.ToString();
    }
}