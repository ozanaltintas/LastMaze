using UnityEngine;

public class MazeAudioController : MonoBehaviour
{
    [Header("Bileşenler")]
    public AudioSource mazeAudioSource; // MP3'ü çalacak kaynak

    [Header("Ses Seviyesi (Volume)")]
    [Tooltip("Labirent dönerken ses en fazla ne kadar çıksın? (0.0 ile 1.0 arası)")]
    [Range(0f, 1f)] // Inspector'da kaydırma çubuğu çıkarır
    public float maxVolume = 0.5f; // Varsayılan olarak %50 yaptık

    [Header("Ses Hızı (Pitch)")]
    [Tooltip("Labirent dururken veya çok yavaşken ses hızı (Kalın ses)")]
    public float minPitch = 0.8f;

    [Tooltip("Labirent maksimum hızda dönerken ses hızı (İnce ses)")]
    public float maxPitch = 1.4f;

    [Header("Geçiş Yumuşaklığı")]
    [Tooltip("Sesin açılıp kapanma yumuşaklığı")]
    public float smoothSpeed = 5f;

    private float _targetVolume;
    private float _targetPitch;

    void Start()
    {
        // Başlangıçta sesin loop olduğundan ve çaldığından emin olalım
        if (mazeAudioSource != null)
        {
            mazeAudioSource.loop = true;
            mazeAudioSource.volume = 0f; // Başta sessiz başlasın
            mazeAudioSource.Play(); // Arka planda çalmaya başlasın
        }
    }

    void Update()
    {
        // 1. Girdiyi al (Mutlak değer, çünkü sağa da sola da dönse ses çıkmalı)
        float inputMagnitude = 0f;
        
        // UIWheelController'a güvenli erişim
        if (UIWheelController.Instance != null)
        {
            inputMagnitude = Mathf.Abs(UIWheelController.Instance.HorizontalInput);
        }

        // 2. Hedef Pitch ve Volume değerlerini hesapla
        if (inputMagnitude > 0.05f) // Çok küçük hareketlerde (Deadzone) ses gelmesin
        {
            // Pitch: Input gücüne göre incelip kalınlaşsın
            _targetPitch = Mathf.Lerp(minPitch, maxPitch, inputMagnitude);
            
            // Volume: Artık 1f değil, senin belirlediğin maxVolume değerine çıkacak
            _targetVolume = maxVolume; 
        }
        else
        {
            // Duruyorsa pitch en alta, ses tamamen kapalıya
            _targetPitch = minPitch;
            _targetVolume = 0f; 
        }

        // 3. Değerleri yumuşak bir şekilde uygula (Lerp)
        if (mazeAudioSource != null)
        {
            mazeAudioSource.pitch = Mathf.Lerp(mazeAudioSource.pitch, _targetPitch, Time.deltaTime * smoothSpeed);
            mazeAudioSource.volume = Mathf.Lerp(mazeAudioSource.volume, _targetVolume, Time.deltaTime * smoothSpeed);
        }
    }
}