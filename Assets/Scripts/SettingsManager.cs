using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    [Header("Panel Ayarları")]
    public GameObject settingsPanel;

    [Header("Night Mode Ayarı")]
    public GameObject nightModeOverlay;

    [Header("Ses Efektleri")]
    public AudioSource audioSource; 
    public AudioClip clickSound;    

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

    // --- BACKDOOR DEĞİŞKENLERİ ---
    private int nightCheatTapCount = 0;
    private float lastNightCheatTapTime = 0f;

    private int soundCheatTapCount = 0;
    private float lastSoundCheatTapTime = 0f;

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
    public void PlayClickSound()
    {
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

        if (isSoundOn) PlayClickSound();

        // --- BACKDOOR 2: FACTORY RESET (OYUNU KOMPLE SIFIRLA) ---
        if (Time.time - lastSoundCheatTapTime < 0.5f)
        {
            soundCheatTapCount++;
        }
        else
        {
            soundCheatTapCount = 1; 
        }
        
        lastSoundCheatTapTime = Time.time;

        // 7 Kere hızlı basılırsa
        if (soundCheatTapCount >= 7)
        {
            FactoryResetBackdoor(); // İsim güncellendi
            soundCheatTapCount = 0;
        }
        // --------------------------------------------------------
    }

    public void ToggleVibration()
    {
        PlayClickSound();

        isVibrationOn = !isVibrationOn;
        PlayerPrefs.SetInt("VibrationSetting", isVibrationOn ? 1 : 0);
        PlayerPrefs.Save();
        
        UpdateVibrationState();

        Debug.Log("Titreşim Ayarı: " + (isVibrationOn ? "AÇIK" : "KAPALI"));

        if (isVibrationOn)
        {
            HapticManager.LightFeedback();
        }
    }

    public void ToggleNightMode()
    {
        PlayClickSound(); 
        HapticManager.LightFeedback(); 

        // --- BACKDOOR 1: SADECE PARAYI SIFIRLA ---
        if (Time.time - lastNightCheatTapTime < 0.5f)
        {
            nightCheatTapCount++;
        }
        else
        {
            nightCheatTapCount = 1; 
        }
        
        lastNightCheatTapTime = Time.time;

        if (nightCheatTapCount >= 7)
        {
            ResetTimeBankBackdoor();
            nightCheatTapCount = 0; 
        }
        // -----------------------------------------

        isNightMode = !isNightMode;
        PlayerPrefs.SetInt("NightModeSetting", isNightMode ? 1 : 0);
        PlayerPrefs.Save();
        UpdateNightMode();
    }

    // --- BACKDOOR FONKSİYONLARI ---
    
    void ResetTimeBankBackdoor()
    {
        // Sadece Parayı Sil
        PlayerPrefs.SetInt("TimeBank", 0);
        PlayerPrefs.SetInt("SonKazanilan", 0);
        PlayerPrefs.Save();

        Debug.LogWarning(">>> BACKDOOR: TimeBank SIFIRLANDI! <<<");

        HapticManager.LightFeedback();
        HapticManager.LightFeedback();

        if (InGameTimeBank.Instance != null)
        {
            InGameTimeBank.Instance.HarcamaYap(100000); 
        }
    }

    void FactoryResetBackdoor()
    {
        // !!! NUCLEAR OPTION !!!
        // Hafızadaki HER ŞEYİ siler (Level, Para, Ayarlar, HighScore vb.)
        PlayerPrefs.DeleteAll();
        
        // Silme işleminden sonra temel varsayılanları tekrar oluştur
        PlayerPrefs.SetInt("ReachedLevel", 1); 
        PlayerPrefs.SetInt("SoundSetting", 1);     // Ses açık başlasın
        PlayerPrefs.SetInt("VibrationSetting", 1); // Titreşim açık başlasın
        PlayerPrefs.Save();

        Debug.LogWarning(">>> FACTORY RESET: OYUN FABRİKA AYARLARINA DÖNDÜ (HER ŞEY SİLİNDİ) <<<");

        // Güçlü Titreşim (3 Kere)
        HapticManager.LightFeedback();
        HapticManager.LightFeedback();
        HapticManager.LightFeedback();

        // Her şeyi sıfırladıktan sonra Ana Menüye dön
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void OpenSettings()
    {
        PlayClickSound(); 
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        PlayClickSound(); 
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void GoHome()
    {
        PlayClickSound(); 
        HapticManager.LightFeedback(); 

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