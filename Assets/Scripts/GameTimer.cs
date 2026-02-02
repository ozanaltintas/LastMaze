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

    // --- GÜNCELLENEN KISIM ---
    public void SüreyiKumbarayaEkle()
    {
        if (levelSuresi > 0)
        {
            int kalanSaniye = Mathf.FloorToInt(levelSuresi);
            int mevcutKumbara = PlayerPrefs.GetInt("TimeBank", 0);
            
            // 1. Toplamı Kaydet
            PlayerPrefs.SetInt("TimeBank", mevcutKumbara + kalanSaniye);
            
            // 2. YENİ: Bu tur ne kadar kazandığını ayrıca kaydet (Animasyon için lazım)
            PlayerPrefs.SetInt("SonKazanilan", kalanSaniye);
            
            PlayerPrefs.Save();
            
            Debug.Log("Kazanılan: " + kalanSaniye + " Toplam: " + (mevcutKumbara + kalanSaniye));
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
    }

    public void Durdur()
    {
        zamanIsliyor = false;
        SüreyiKumbarayaEkle();
        if (sureYazisi != null) sureYazisi.gameObject.SetActive(false);
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