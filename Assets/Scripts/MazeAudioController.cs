using UnityEngine;

public class MazeAudioController : MonoBehaviour
{
    [Header("Bileşenler")]
    public AudioSource mazeAudioSource; 

    [Header("Otomatik Kontrol (YENİ)")]
    [Tooltip("Labirentin dönme scripti buraya otomatik gelir veya sen sürükle.")]
    public MonoBehaviour hareketScripti; // MazeRotator'ı takip edeceğiz

    [Header("Ses Seviyesi")]
    [Range(0f, 1f)] public float maxVolume = 0.5f;

    [Header("Ses Hızı (Pitch)")]
    public float minPitch = 0.8f;
    public float maxPitch = 1.4f;

    [Header("Geçiş Yumuşaklığı")]
    public float smoothSpeed = 5f;

    private float _targetVolume;
    private float _targetPitch;
    private bool _zorlaSustur = false; // Acil durum freni

    void Start()
    {
        // 1. Eğer hareket scriptini atamayı unuttuysan, aynı objede var mı diye bakar
        if (hareketScripti == null)
        {
            // "MazeRotator" isminde script arar, bulamazsa MonoBehaviour olarak alır
            hareketScripti = GetComponent("MazeRotator") as MonoBehaviour;
        }

        // 2. Ses kaynağı ayarları
        if (mazeAudioSource != null)
        {
            mazeAudioSource.loop = true;
            mazeAudioSource.volume = 0f;
            mazeAudioSource.Play();
        }
    }

    void Update()
    {
        // --- GÜVENLİK KONTROLÜ (YENİ) ---
        // Eğer hareket scripti (MazeRotator) devre dışı kaldıysa veya obje yok olduysa:
        if (_zorlaSustur || (hareketScripti != null && !hareketScripti.enabled))
        {
            SesiKes(); // Anında sustur
            return;    // Ve aşağıdakileri yapma
        }
        // --------------------------------

        float inputMagnitude = 0f;
        
        if (UIWheelController.Instance != null)
        {
            inputMagnitude = Mathf.Abs(UIWheelController.Instance.HorizontalInput);
        }

        // Hedef değerleri hesapla
        if (inputMagnitude > 0.05f) 
        {
            _targetPitch = Mathf.Lerp(minPitch, maxPitch, inputMagnitude);
            _targetVolume = maxVolume; 
        }
        else
        {
            _targetPitch = minPitch;
            _targetVolume = 0f; 
        }

        // Uygula
        if (mazeAudioSource != null)
        {
            mazeAudioSource.pitch = Mathf.Lerp(mazeAudioSource.pitch, _targetPitch, Time.deltaTime * smoothSpeed);
            mazeAudioSource.volume = Mathf.Lerp(mazeAudioSource.volume, _targetVolume, Time.deltaTime * smoothSpeed);
        }
    }

    // Script dışarıdan (WinZone veya Shatter tarafından) kapatılırsa çalışır
    void OnDisable()
    {
        SesiKes();
    }

    // Sesi anında kesen yardımcı fonksiyon
    public void SesiKes()
    {
        if (mazeAudioSource != null)
        {
            mazeAudioSource.Stop();
            mazeAudioSource.volume = 0f;
        }
        _zorlaSustur = true;
    }
}