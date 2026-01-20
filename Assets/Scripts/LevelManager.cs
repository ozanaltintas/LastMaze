using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yükleme işlemleri için bu kütüphane şart

public class LevelManager : MonoBehaviour
{
    // Bu fonksiyonu butona bağlayacağız
    public void RestartGame()
    {
        // 1. Önemli: Eğer oyun bitince zamanı durdurduysan (Time.timeScale = 0 yaptıysan),
        // yeniden başladığında zamanın akması için bunu tekrar 1 yapmalısın.
        Time.timeScale = 1f;

        // 2. Şu an aktif olan sahnenin ismini al
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 3. Sahneyi baştan yükle
        SceneManager.LoadScene(currentSceneName);
    }
}