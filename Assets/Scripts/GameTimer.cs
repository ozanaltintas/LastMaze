using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections; // IEnumerator için gerekli

public class GameTimer : MonoBehaviour
{
    [Header("Ayarlar")]
    public float levelSuresi = 60f;
    
    [Header("Durum Kontrolü")]
    public bool oyunBasladi = false; 
    public bool zamanIsliyor = true;

    [Header("UI Bağlantıları")]
    public TextMeshProUGUI sureYazisi;
    public GameObject loseEkrani;

    void Start()
    {
        if (sureYazisi != null)
        {
            sureYazisi.text = Mathf.CeilToInt(levelSuresi).ToString();
            sureYazisi.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (!oyunBasladi)
        {
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            {
                oyunBasladi = true;
            }
            return;
        }

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
        if (sureYazisi != null) sureYazisi.gameObject.SetActive(false);
        if (loseEkrani != null) loseEkrani.SetActive(true);
    }

    public void Durdur()
    {
        zamanIsliyor = false;
        if (sureYazisi != null) sureYazisi.gameObject.SetActive(false);
    }

    // --- DEĞİŞİKLİK BURADA BAŞLIYOR ---

    public void BolumuYenidenBaslat()
    {
        // Doğrudan yüklemek yerine Coroutine başlatıyoruz
        StartCoroutine(RestartGecikmeli());
    }

    IEnumerator RestartGecikmeli()
    {
        // Zamanı durdurmuyoruz ki animasyon (Time.deltaTime) çalışmaya devam etsin
        Time.timeScale = 1; 
        
        // Önceki adımda yazdığımız dönüş animasyonunun süresi 0.6f civarıydı.
        // Animasyonun bitmesi için 0.7 saniye bekliyoruz.
        yield return new WaitForSecondsRealtime(0.4f);

        // Ve şimdi sahneyi yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}