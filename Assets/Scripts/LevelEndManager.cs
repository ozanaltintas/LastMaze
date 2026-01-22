using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelEndManager : MonoBehaviour
{
    [Header("UI Ayarları")]
    public GameObject bravoEkrani;
    public float beklemeSuresi = 3f;

    private void OnTriggerEnter(Collider other)
    {
        // Çarpan şeyin etiketi Player mı?
        if (other.CompareTag("Player"))
        {
            Debug.Log("1. Adım: Oyuncu bitiş çizgisine girdi."); // KONSOLU KONTROL ET

            // Sahnedeki GameTimer scriptini bulmaya çalış
            GameTimer timerScript = FindObjectOfType<GameTimer>();

            if (timerScript != null)
            {
                Debug.Log("2. Adım: Timer scripti bulundu, durdurma komutu gönderiliyor."); // KONSOLU KONTROL ET
                timerScript.Durdur();
            }
            else
            {
                Debug.LogError("HATA: Sahnede GameTimer scripti bulunamadı!"); // EĞER BU ÇIKARSA SORUN BURADA
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