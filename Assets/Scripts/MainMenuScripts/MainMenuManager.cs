using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Bekleme işlemi (Coroutine) için gerekli

public class MainMenuManager : MonoBehaviour
{
    [Header("Level Ayarları (YENİ)")]
    [Tooltip("Eğer kayıt yoksa kaçıncı levelden başlasın? (Genelde 1)")]
    public int baslangicLevelIndex = 1;

    [Header("Ses Ayarları")]
    public AudioSource audioSource; // Managers objesindeki Audio Source
    public AudioClip clickSound;    // Tık sesi dosyası

    // Oyunu Başlat Butonuna bağlayacağın fonksiyon
    public void OyunaBasla()
    {
        // 1. Önce sesi çal (Senin yazdığın fonksiyonu kullanıyoruz)
        PlayClickSound();

        // 2. Sahne geçişini başlat (Ses duyulsun diye Coroutine kullanıyoruz)
        StartCoroutine(AkilliSahneGecisi());
    }

    // Sahne yüklemesini yöneten yardımcı fonksiyon
    IEnumerator AkilliSahneGecisi()
    {
        // Sesin duyulması için 0.2 saniye bekle
        yield return new WaitForSeconds(0.2f);

        // --- DEĞİŞİKLİK BURADA: SADECE 1. LEVELİ DEĞİL, KAYITLI LEVELİ AÇ ---
        
        // Kaydedilen leveli hafızadan oku. Kayıt yoksa 'baslangicLevelIndex' (1) gelir.
        int kaydedilenLevel = PlayerPrefs.GetInt("ReachedLevel", baslangicLevelIndex);

        // Güvenlik: Eğer kayıtlı level Build Settings'de varsa onu aç
        if (kaydedilenLevel < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(kaydedilenLevel);
        }
        else
        {
            // Eğer abartı bir sayı geldiyse veya oyun bittiyse başa dön
            Debug.LogWarning("Kayıtlı level bulunamadı, Level 1 açılıyor.");
            SceneManager.LoadScene(baslangicLevelIndex);
        }
    }

    // Senin yazdığın ses çalma fonksiyonu (Aynen duruyor)
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