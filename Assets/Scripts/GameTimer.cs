using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections; 

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

    [Header("Gerilim Sesleri")]
    public AudioSource audioSource; 
    public AudioClip countdownBeep; 
    public AudioClip timeUpSound;   

    private int lastSecond; 

    void Start()
    {
        lastSecond = Mathf.CeilToInt(levelSuresi);

        if (sureYazisi != null)
        {
            sureYazisi.gameObject.SetActive(true);
            sureYazisi.text = lastSecond.ToString();
            sureYazisi.color = Color.black; 
        }
    }

    void Update()
    {
        if (sureYazisi != null && !oyunBasladi)
        {
            sureYazisi.text = Mathf.CeilToInt(levelSuresi).ToString();
        }

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
            int currentSecond = Mathf.CeilToInt(levelSuresi);

            if (currentSecond != lastSecond)
            {
                if (currentSecond <= 3 && currentSecond > 0)
                {
                    PlayBeep();
                    if (sureYazisi != null) sureYazisi.color = Color.red; 
                }
                else if (currentSecond <= 0)
                {
                    PlayTimeUp();
                }
                
                lastSecond = currentSecond;
            }

            if (sureYazisi != null)
            {
                sureYazisi.text = Mathf.Max(0, currentSecond).ToString();
            }

            if (levelSuresi <= 0)
            {
                levelSuresi = 0;
                OyunuKaybet();
            }
        }
    }

    // --- ZAMAN KUMBARASI VE ANİMASYON TETİKLEYİCİSİ ---
    public void SüreyiKumbarayaEkle()
    {
        if (levelSuresi > 0)
        {
            int kalanSaniye = Mathf.FloorToInt(levelSuresi);
            int mevcutKumbara = PlayerPrefs.GetInt("TimeBank", 0);
            
            // 1. Veriyi Kaydet (Toplam Kumbara)
            PlayerPrefs.SetInt("TimeBank", mevcutKumbara + kalanSaniye);
            
            // 2. Ana menü animasyonu için bunu da kaydediyoruz
            PlayerPrefs.SetInt("SonKazanilan", kalanSaniye); 
            PlayerPrefs.Save();
            
            // 3. Oyun İçi Görsel Animasyonu Başlat (DOTween)
            if (InGameTimeBank.Instance != null)
            {
                InGameTimeBank.Instance.AnimasyonuBaslat(kalanSaniye);
            }
        }
    }
    
    // --- ZAMAN EKLEME (MARKET VEYA ÇARK İÇİN) ---
    public void ZamanEkle(float saniye)
    {
        // Zaman işliyor olsa da olmasa da eklenebilsin (Lose ekranındayken de lazım)
        levelSuresi += saniye;
        
        if (sureYazisi != null)
        {
            sureYazisi.text = Mathf.CeilToInt(levelSuresi).ToString();
            StartCoroutine(YaziEfekti());
        }
    }

    // --- ÇARKIFELEK SONRASI OYUNU DEVAM ETTİR ---
    public void OyunuDevamEttir()
    {
        // 1. Lose ekranını kapat
        if (loseEkrani != null) loseEkrani.SetActive(false);

        // 2. Zamanı tekrar akıtmaya başla
        zamanIsliyor = true;
        oyunBasladi = true; 

        // 3. Yazı rengini düzelt (Kırmızı kalmış olabilir)
        if (sureYazisi != null) sureYazisi.color = Color.black;

        // 4. Müzik veya TimeScale durduysa burada açabilirsin
        Time.timeScale = 1; 
        
        Debug.Log("Oyun Devam Ediyor!");
    }

    IEnumerator YaziEfekti()
    {
        if (sureYazisi != null)
        {
            sureYazisi.color = Color.green; 
            yield return new WaitForSeconds(0.5f);
            // Eğer süre kritikse kırmızı yap, değilse siyah
            if (levelSuresi <= 3) sureYazisi.color = Color.red;
            else sureYazisi.color = Color.black; 
        }
    }

    void PlayBeep()
    {
        if (audioSource != null && countdownBeep != null)
            audioSource.PlayOneShot(countdownBeep);
    }

    void PlayTimeUp()
    {
        if (audioSource != null && timeUpSound != null)
            audioSource.PlayOneShot(timeUpSound);
    }

    void OyunuKaybet()
    {
        zamanIsliyor = false;
        if (loseEkrani != null) loseEkrani.SetActive(true);
        // İstersen burada Time.timeScale = 0 yapabilirsin
    }

    public void Durdur()
    {
        zamanIsliyor = false;
        SüreyiKumbarayaEkle();
        // Kazandığında süre yazısını kapatmak istersen burayı aç:
        // if (sureYazisi != null) sureYazisi.gameObject.SetActive(false);
    }

    public void BolumuYenidenBaslat()
    {
        StartCoroutine(RestartGecikmeli());
    }

    IEnumerator RestartGecikmeli()
    {
        Time.timeScale = 1; 
        yield return new WaitForSecondsRealtime(0.4f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}