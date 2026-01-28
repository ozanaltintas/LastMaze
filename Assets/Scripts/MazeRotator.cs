using UnityEngine;

public class MazeRotator : MonoBehaviour
{
    [Header("Ayarlar")]
    [Tooltip("Labirentin saniyedeki maksimum dönüş hızı (Derece/Saniye)")]
    public float maxRotationSpeed = 100f;

    [Tooltip("Hızlanma yumuşaklığı (Daha yüksek = daha çabuk tepki, Düşük = daha kaygan)")]
    public float smoothTime = 10f;

    [Header("Ses Ayarları (YENİ)")]
    public AudioSource donmeSesi; // Inspector'dan AudioSource'u buraya sürükle

    // Anlık dönüş hızını tutan değişken
    private float _currentRotationSpeed;

    void Update()
    {
        // 1. GİRDİ (INPUT):
        float rawInput = 0f;

        if (UIWheelController.Instance != null)
        {
            rawInput = UIWheelController.Instance.HorizontalInput;
        }

        // --- SES KONTROLÜ (YENİ) ---
        // Eğer girdi varsa (çark dönüyorsa) ve ses çalmıyorsa -> ÇAL
        if (Mathf.Abs(rawInput) > 0.05f) 
        {
            if (donmeSesi != null && !donmeSesi.isPlaying)
            {
                donmeSesi.loop = true;
                donmeSesi.Play();
            }
        }
        // Eğer girdi yoksa (çark durduysa) ve ses çalıyorsa -> DURDUR
        else
        {
            if (donmeSesi != null && donmeSesi.isPlaying)
            {
                donmeSesi.Stop();
            }
        }
        // ---------------------------

        // 2. HESAPLAMA (CALCULATION):
        float targetSpeed = rawInput * -maxRotationSpeed;

        // Yumuşak geçiş
        _currentRotationSpeed = Mathf.Lerp(_currentRotationSpeed, targetSpeed, Time.deltaTime * smoothTime);

        // 3. UYGULAMA (EXECUTION):
        transform.Rotate(Vector3.forward * _currentRotationSpeed * Time.deltaTime);
    }

    // --- GÜVENLİK SİGORTASI (YENİ) ---
    // MazeShatter2D scripti bu scripti "enabled = false" yapıp kapattığında burası çalışır.
    void OnDisable()
    {
        // 1. Dönüş hızını sıfırla (Arka planda hız kalmasın)
        _currentRotationSpeed = 0f;

        // 2. Sesi derhal sustur
        if (donmeSesi != null)
        {
            donmeSesi.Stop();
        }
    }
}