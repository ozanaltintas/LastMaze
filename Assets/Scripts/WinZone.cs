using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WinZone : MonoBehaviour
{
    [Header("UI ve Efektler")]
    public GameObject bravoEkrani;
    public ParticleSystem confettiParticle; 
    public float beklemeSuresi = 3f;

    [Header("Ses Ayarları")]
    public AudioSource audioSource; 
    public AudioClip winSound;     

    private bool isWon = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isWon) return;

        if (other.CompareTag("Player"))
        {
            isWon = true;
            Debug.Log("Oyun Kazanıldı! - BRAVO");

            // 1. Timer'ı Durdur
            GameTimer timerScript = FindObjectOfType<GameTimer>();
            if (timerScript != null)
            {
                timerScript.Durdur();
            }

            // 2. Konfeti Patlat
            if (confettiParticle != null) confettiParticle.Play();

            // 3. İşlemleri Başlat
            StartCoroutine(WinSequence());
        }
    }

    IEnumerator WinSequence()
    {
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

        // --- İLERLEMEYİ KAYDET (BURASI EKSİKTİ!) ---
        KaydetVeIlerlet();

        // --- BEKLE ---
        yield return new WaitForSeconds(beklemeSuresi);
        
        // --- SONRAKİ BÖLÜMÜ YÜKLE ---
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Eğer sonraki sahne varsa yükle, yoksa başa dön
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            SceneManager.LoadScene(0); 
        }
    }

    void KaydetVeIlerlet()
    {
        // Şu anki sahne indexi (Örn: Scene 1desin)
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        
        // Ulaşılan yeni level (Örn: Scene 2ye geçeceksin, yani Level 2 oldun)
        int reachedLevel = currentSceneIndex + 1;

        // ÖNEMLİ KONTROL:
        // Eğer oyuncu zaten Level 5'e gelmişse ve dönüp Level 1'i tekrar oynuyorsa,
        // kaydını bozmamalıyız (Level 2 yapmamalıyız).
        // Sadece yeni bir rekora imza attıysa kaydediyoruz.
        if (reachedLevel > PlayerPrefs.GetInt("ReachedLevel", 1))
        {
            PlayerPrefs.SetInt("ReachedLevel", reachedLevel);
            PlayerPrefs.Save(); // Defteri kapat ve yaz.
            Debug.Log("YENİ LEVEL KAYDEDİLDİ: " + reachedLevel);
        }
    }
}