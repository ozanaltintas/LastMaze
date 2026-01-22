using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    [Header("Panel Ayarları")]
    public GameObject settingsPanel;

    [Header("Night Mode Ayarı")]
    public GameObject nightModeOverlay;

    [Header("Ses Efektleri (YENİ)")]
    public AudioSource audioSource; // Canvas'taki Audio Source buraya
    public AudioClip clickSound;    // Bilgisayarındaki ses dosyası buraya

    [Header("Ses Butonu Ayarları")]
    public Image soundButtonImage;
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    [Header("Titreşim Butonu Ayarları")]
    public Image vibrationButtonImage;
    public Sprite vibrationOnSprite;
    public Sprite vibrationOffSprite;

    // Durum değişkenleri
    private bool isSoundOn = true;
    private bool isVibrationOn = true;
    private bool isNightMode = false;

    void Start()
    {
        // Ayarları yükle
        isSoundOn = PlayerPrefs.GetInt("SoundSetting", 1) == 1;
        isVibrationOn = PlayerPrefs.GetInt("VibrationSetting", 1) == 1;
        isNightMode = PlayerPrefs.GetInt("NightModeSetting", 0) == 1;

        // Ayarları uygula
        UpdateSoundState();
        UpdateVibrationState();
        UpdateNightMode();

        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    // --- SES ÇALMA FONKSİYONU ---
    // Bu fonksiyonu dışarıdan (Play butonu vb.) çağırabilirsin
    public void PlayClickSound()
    {
        // Sadece ses ayarı AÇIKSA ve dosya varsa çal
        if (isSoundOn && audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    // --- BUTON FONKSİYONLARI ---

    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;
        PlayerPrefs.SetInt("SoundSetting", isSoundOn ? 1 : 0);
        PlayerPrefs.Save();
        
        UpdateSoundState();

        // Sesi açtıysa duyması için çalsın, kapattıysa çalmasın
        if (isSoundOn) PlayClickSound();
    }

    public void ToggleVibration()
    {
        PlayClickSound(); // Tık sesi
        isVibrationOn = !isVibrationOn;
        PlayerPrefs.SetInt("VibrationSetting", isVibrationOn ? 1 : 0);
        PlayerPrefs.Save();
        UpdateVibrationState();
    }

    public void ToggleNightMode()
    {
        PlayClickSound(); // Tık sesi
        isNightMode = !isNightMode;
        PlayerPrefs.SetInt("NightModeSetting", isNightMode ? 1 : 0);
        PlayerPrefs.Save();
        UpdateNightMode();
    }

    public void OpenSettings()
    {
        PlayClickSound(); // Tık sesi
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        PlayClickSound(); // Tık sesi
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void GoHome()
    {
        PlayClickSound(); // Tık sesi
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    // --- GÜNCELLEME FONKSİYONLARI ---
    void UpdateSoundState()
    {
        AudioListener.volume = isSoundOn ? 1 : 0;
        if (soundButtonImage != null)
            soundButtonImage.sprite = isSoundOn ? soundOnSprite : soundOffSprite;
    }

    void UpdateVibrationState()
    {
        if (vibrationButtonImage != null)
            vibrationButtonImage.sprite = isVibrationOn ? vibrationOnSprite : vibrationOffSprite;
    }

    void UpdateNightMode()
    {
        if (nightModeOverlay != null)
            nightModeOverlay.SetActive(isNightMode);
    }
}