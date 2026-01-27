using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WinZone : MonoBehaviour
{
    [Header("UI ve Efektler")]
    public GameObject bravoEkrani;
    public ParticleSystem confettiParticle; 
    
    [Header("Zamanlama Ayarları")]
    [Tooltip("Top girdikten kaç saniye sonra Win ekranı açılsın?")]
    public float panelAcilmaGecikmesi = 1.0f; 

    [Tooltip("Win ekranı açıldıktan kaç saniye sonra diğer bölüme geçilsin?")]
    public float sonrakiLevelBeklemeSuresi = 3.0f;

    [Header("Ses Ayarları")]
    public AudioSource audioSource; 
    public AudioClip winSound;     

    private bool isWon = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Eğer zaten kazanıldıysa tekrar çalışma
        if (isWon) return;

        if (other.CompareTag("Player"))
        {
            isWon = true;
            Debug.Log("Oyun Kazanıldı!");

            // 1. Timer'ı Durdur
            GameTimer timerScript = FindFirstObjectByType<GameTimer>();
            if (timerScript != null)
            {
                timerScript.Durdur();
            }

            // 2. Konfetiyi Patlat
            if (confettiParticle != null)
            {
                confettiParticle.Play();
            }

            // 3. LABİRENTİ PATLAT (Shatter Efekti)
            MazeShatter2D mazeShatter = FindFirstObjectByType<MazeShatter2D>();
            if (mazeShatter != null)
            {
                mazeShatter.Shatter();
            }

            // 4. TOPU PATLAT (Ball Shatter)
            // Çarpan objenin (Topun) üzerindeki scripti al ve patlat
            BallShatter ballShatter = other.GetComponent<BallShatter>();
            if (ballShatter != null)
            {
                ballShatter.Patlat();
            }

            // 5. Kazanma Sürecini Başlat (Bekleme, Panel, Ses vb.)
            StartCoroutine(WinSequence());
        }
    }

    IEnumerator WinSequence()
    {
        // Patlama efektleri görünsün diye panel açılmadan önce bekle
        yield return new WaitForSeconds(panelAcilmaGecikmesi);

        // --- SESİ ÇAL ---
        if (audioSource != null && winSound != null)
        {
            audioSource.PlayOneShot(winSound);
        }

        // --- PANELİ AÇ ---
        if (bravoEkrani != null)
        {
            bravoEkrani.SetActive(true);
        }

        // --- KAYDET ---
        KaydetVeIlerlet();

        // --- SONRAKİ LEVEL İÇİN BEKLE ---
        yield return new WaitForSeconds(sonrakiLevelBeklemeSuresi);
        
        // --- SAHNE YÜKLE ---
        LoadNextScene();
    }

    void KaydetVeIlerlet()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int reachedLevel = currentSceneIndex + 1;

        // Sadece yeni bir rekorsa kaydet
        if (reachedLevel > PlayerPrefs.GetInt("ReachedLevel", 1))
        {
            PlayerPrefs.SetInt("ReachedLevel", reachedLevel);
            PlayerPrefs.Save();
        }
    }

    void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Sonraki sahne varsa yükle, yoksa başa dön
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            SceneManager.LoadScene(0); 
        }
    }
}