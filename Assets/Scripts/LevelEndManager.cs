using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelEndManager : MonoBehaviour
{
    [Header("UI Ayarları")]
    public GameObject bravoEkrani;
    public float beklemeSuresi = 3f;

    [Header("Efekt Ayarları")]
    [SerializeField] private ParticleSystem confettiParticle;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("1. Adım: Oyuncu bitiş çizgisine girdi.");

            GameTimer timerScript = FindObjectOfType<GameTimer>();
            if (timerScript != null)
            {
                timerScript.Durdur();
            }

            // --- KONFETİ KONTROLÜ ---
            if (confettiParticle != null)
            {
                Debug.Log("3. Adım: Konfeti sistemi bulundu, Oynatılıyor...");
                confettiParticle.Play();
            }
            else
            {
                Debug.LogError("HATA: LevelEndManager üzerinde Confetti Particle slotu boş!");
            }

            StartCoroutine(LevelBitisSekansi());
        }
    }

    IEnumerator LevelBitisSekansi()
    {
        if (bravoEkrani != null)
        {
            bravoEkrani.SetActive(true);
        }

        yield return new WaitForSeconds(beklemeSuresi);
        SonrakiBolumuYukle();
    }

    void SonrakiBolumuYukle()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0; 
        }
        SceneManager.LoadScene(nextSceneIndex);
    }
}