using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    [Header("Ayarlar")]
    public float levelSuresi = 60f; // Başlangıç süresi
    
    [Header("Durum Kontrolü")]
    public bool oyunBasladi = false; 
    public bool zamanIsliyor = true;

    [Header("UI Bağlantıları")]
    public TextMeshProUGUI sureYazisi; // Timer Text nesnesini buraya sürükle
    public GameObject loseEkrani;      // Lose Panelini buraya sürükle

    // Start: Oyun açıldığı ilk milisaniye çalışır
    void Start()
    {
        // Süre yazısını en başta göster (Örn: "60")
        if (sureYazisi != null)
        {
            sureYazisi.text = Mathf.CeilToInt(levelSuresi).ToString();
            sureYazisi.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        // 1. Oyun henüz başlamadıysa bekle
        if (!oyunBasladi)
        {
            // Ekrana veya herhangi bir UI elemanına dokunuldu mu?
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            {
                oyunBasladi = true; // Ve start ver!
            }
            return; // Dokunulmadıysa aşağı inme, bekle
        }

        // 2. Oyun başladı, geri sayım başlasın
        if (zamanIsliyor)
        {
            levelSuresi -= Time.deltaTime;
            
            if (sureYazisi != null)
            {
                sureYazisi.text = Mathf.CeilToInt(levelSuresi).ToString();
            }

            if (levelSuresi <= 0)
            {
                levelSuresi = 0;
                if (sureYazisi != null) sureYazisi.text = "0";
                OyunuKaybet();
            }
        }
    }

    void OyunuKaybet()
    {
        zamanIsliyor = false;
        
        // Kaybedince süreyi gizle
        if (sureYazisi != null) sureYazisi.gameObject.SetActive(false);

        if (loseEkrani != null)
        {
            loseEkrani.SetActive(true);
        }
    }

    // WinZone scripti bu fonksiyonu çağıracak
    public void Durdur()
    {
        zamanIsliyor = false;
        
        // Kazanınca süreyi gizle
        if (sureYazisi != null) sureYazisi.gameObject.SetActive(false);
    }

    public void BolumuYenidenBaslat()
    {
        Time.timeScale = 1; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}