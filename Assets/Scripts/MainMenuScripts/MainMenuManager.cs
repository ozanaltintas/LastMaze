using UnityEngine;
using UnityEngine.SceneManagement; // Sahne geçişleri için şart

public class MainMenuManager : MonoBehaviour
{
    [Header("Ayarlar")]
    [Tooltip("Play butonuna basınca açılacak sahnenin adı")]
    public string gameSceneName = "GameScene";

    // Play Butonu buna bağlanacak
    public void PlayGame()
    {
        // Hedef sahneyi yükle
        SceneManager.LoadScene(gameSceneName);
    }

    // (Opsiyonel) Çıkış Butonu buna bağlanacak
    public void QuitGame()
    {
        Debug.Log("Oyundan Çıkıldı!"); // Editörde çıkış çalışmaz, konsola yazar
        Application.Quit();
    }
}