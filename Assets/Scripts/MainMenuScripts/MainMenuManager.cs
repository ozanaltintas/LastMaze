using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Ses Ayarları")]
    public AudioSource audioSource; // Managers objesindeki Audio Source
    public AudioClip clickSound;    // Tık sesi dosyası

    // Oyunu Başlat Butonuna bağlayacağın fonksiyon
    public void OyunaBasla()
    {
        // Önce sesi çal
        PlayClickSound();

        // Sonra (örneğin 0.2 saniye sonra) sahneyi değiştir ki ses duyulsun
        // Invoke("SahneDegistir", 0.2f); 
        // VEYA direkt geçiş (Ses biraz kesilebilir ama basittir):
        SceneManager.LoadScene(1); 
        // (Not: Sahne numaran 1 ise 1 yaz, build settings'e bak)
    }

    // Harici olarak sadece ses çalmak istersen (Başka butonlar için)
    public void PlayClickSound()
    {
        // 1. Hafızadan ses ayarını oku (Varsayılan 1 = Açık)
        bool isSoundOn = PlayerPrefs.GetInt("SoundSetting", 1) == 1;

        // 2. Ses açıksa ve dosyalar tamamsa çal
        if (isSoundOn && audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}