using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections; 
using DG.Tweening; // DOTween kütüphanesi eklendi

public class GameTimer : MonoBehaviour
{
    [Header("Ayarlar")]
    public float levelSuresi = 60f;
    
    [Header("Durum Kontrolü")]
    public bool oyunBasladi = false; 
    public bool zamanIsliyor = true;

    [Header("UI Bağlantıları")]
    public TextMeshProUGUI sureYazisi;      // Ana sayaç
    public GameObject loseEkrani;

    [Header("Visual Juice (YENİ)")]
    public TextMeshProUGUI floatingText;    // Uçacak olan "+10" yazısı
    public float ucusSuresi = 0.8f;         // Animasyon hızı

    [Header("Gerilim Sesleri")]
    public AudioSource audioSource; 
    public AudioClip countdownBeep; 
    public AudioClip timeUpSound;   

    private int lastSecond; 
    private RigidbodyType2D savedBodyType;

    void Awake()
    {
        // Uçan yazıyı başlangıçta gizle ve küçült
        if (floatingText != null)
        {
            floatingText.gameObject.SetActive(false);
            floatingText.transform.localScale = Vector3.zero;
        }
    }

    void Start()
    {
        Time.timeScale = 1f; 
        
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
        if (!oyunBasladi)
        {
            if (sureYazisi != null) sureYazisi.text = Mathf.CeilToInt(levelSuresi).ToString();

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

            // Eğer o an animasyon yoksa yazıyı güncelle (Çakışmayı önlemek için)
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

    // --- ZAMAN KUMBARASI ---
    public void SüreyiKumbarayaEkle()
    {
        if (levelSuresi > 0)
        {
            int kalanSaniye = Mathf.FloorToInt(levelSuresi);
            int mevcutKumbara = PlayerPrefs.GetInt("TimeBank", 0);
            
            PlayerPrefs.SetInt("TimeBank", mevcutKumbara + kalanSaniye);
            PlayerPrefs.SetInt("SonKazanilan", kalanSaniye); 
            PlayerPrefs.Save();
            
            if (InGameTimeBank.Instance != null)
            {
                InGameTimeBank.Instance.AnimasyonuBaslat(kalanSaniye);
            }
        }
    }
    
    // --- ZAMAN EKLEME VE ANİMASYON ---
    public void ZamanEkle(float saniye)
    {
        // 1. Mantıksal olarak süreyi ekle
        levelSuresi += saniye;

        // 2. Görsel Animasyonu Başlat (Senin paylaştığın InGameTimeBank mantığı)
        if (floatingText != null && sureYazisi != null)
        {
            AnimasyonuOynat((int)saniye);
        }
        else
        {
            // Eğer referanslar yoksa sadece eski usul güncelle
            if (sureYazisi != null) sureYazisi.text = Mathf.CeilToInt(levelSuresi).ToString();
        }
    }

    void AnimasyonuOynat(int miktar)
    {
        // Hazırlık
        floatingText.gameObject.SetActive(true);
        floatingText.text = "+" + miktar;
        
        // Başlangıç pozisyonunu (ekranın ortası veya belirlenen yer) resetle
        floatingText.rectTransform.anchoredPosition = Vector2.zero; 
        floatingText.transform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();

        // A) POP UP: Büyüyerek açıl (InGameTimeBank stili)
        seq.Append(floatingText.transform.DOScale(Vector3.one * 1.5f, 0.4f).SetEase(Ease.OutBack));

        // B) UÇUŞ: Hedefe (Sayaca) doğru uç ve küçül
        // Not: sureYazisi'nin dünya pozisyonuna gitmesi lazım
        seq.Append(floatingText.transform.DOMove(sureYazisi.transform.position, ucusSuresi).SetEase(Ease.InBack));
        
        // Havada süzülürken hafif küçülsün (0.4 boyutuna)
        seq.Join(floatingText.transform.DOScale(Vector3.one * 0.5f, ucusSuresi).SetEase(Ease.InQuad));

        // C) VARIŞ: Hedefe ulaştığında
        seq.OnComplete(() => {
            floatingText.gameObject.SetActive(false); // Uçan yazıyı gizle

            // HEDEF TEPKİSİ: Ana sayaç zıplasın ve yeşil yansın
            if (sureYazisi != null)
            {
                sureYazisi.transform.DOPunchScale(Vector3.one * 0.4f, 0.3f, 10, 1);
                StartCoroutine(YaziEfekti());
            }
        });
    }

    // --- OYUNU DEVAM ETTİR ---
    public void OyunuDevamEttir()
    {
        if (loseEkrani != null) loseEkrani.SetActive(false);

        zamanIsliyor = true;
        oyunBasladi = true; 
        
        if (sureYazisi != null) sureYazisi.color = Color.black;

        TopuCoz();

        WinZone winZone = FindFirstObjectByType<WinZone>();
        if (winZone != null)
        {
             Collider2D col2D = winZone.GetComponent<Collider2D>();
             if (col2D != null) col2D.enabled = true;
             
             Collider col3D = winZone.GetComponent<Collider>();
             if (col3D != null) col3D.enabled = true;
        }
        
        Debug.Log("Oyun Devam Ediyor + Juice Efekti Eklendi.");
    }

    IEnumerator YaziEfekti()
    {
        if (sureYazisi != null)
        {
            Color originalColor = sureYazisi.color;
            sureYazisi.color = Color.green; // Parlak yeşil
            
            yield return new WaitForSeconds(0.5f);
            
            // Kritik seviyeye göre rengi geri döndür
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
        TopuDondur();

        WinZone winZone = FindFirstObjectByType<WinZone>(); 
        if (winZone != null)
        {
            Collider2D col2D = winZone.GetComponent<Collider2D>();
            if (col2D != null) col2D.enabled = false;
            
            Collider col3D = winZone.GetComponent<Collider>();
            if (col3D != null) col3D.enabled = false;
        }

        if (loseEkrani != null) loseEkrani.SetActive(true);
    }

    // --- FİZİK KONTROLÜ ---
    void TopuDondur()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; 
                rb.angularVelocity = 0f;
                savedBodyType = rb.bodyType; 
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }
    }

    void TopuCoz()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic; 
                rb.constraints = RigidbodyConstraints2D.None; 
                rb.WakeUp();
            }
        }
    }

    public void Durdur()
    {
        zamanIsliyor = false;
        SüreyiKumbarayaEkle();
    }

    public void BolumuYenidenBaslat()
    {
        StartCoroutine(RestartGecikmeli());
    }

    IEnumerator RestartGecikmeli()
    {
        Time.timeScale = 1f; 
        yield return new WaitForSecondsRealtime(0.4f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}