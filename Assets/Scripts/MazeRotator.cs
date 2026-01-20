using UnityEngine;

public class MazeRotator : MonoBehaviour
{
    [Header("Ayarlar")]
    [Tooltip("Labirentin saniyedeki maksimum dönüş hızı (Derece/Saniye)")]
    public float maxRotationSpeed = 100f;

    [Tooltip("Hızlanma yumuşaklığı (Daha yüksek = daha çabuk tepki, Düşük = daha kaygan)")]
    public float smoothTime = 10f;

    // Anlık dönüş hızını tutan değişken
    private float _currentRotationSpeed;

    void Update()
    {
        // 1. GİRDİ (INPUT):
        // Başka bir dosyadan (UIWheelController) gelen veriyi okuyoruz.
        // Eğer sahnede Controller yoksa hata vermemesi için null kontrolü yapıyoruz.
        float rawInput = 0f;

        if (UIWheelController.Instance != null)
        {
            rawInput = UIWheelController.Instance.HorizontalInput;
        }

        // 2. HESAPLAMA (CALCULATION):
        // Sağa çekince (rawInput > 0) -> Saat Yönünde (Negatif Z) dönmeli.
        // Sola çekince (rawInput < 0) -> Saat Yönü Tersine (Pozitif Z) dönmeli.
        float targetSpeed = rawInput * -maxRotationSpeed;

        // Yumuşak geçiş (Lerp) ile hız değişimini doğal hale getiriyoruz.
        _currentRotationSpeed = Mathf.Lerp(_currentRotationSpeed, targetSpeed, Time.deltaTime * smoothTime);

        // 3. UYGULAMA (EXECUTION):
        // Transform'u z ekseninde döndür.
        transform.Rotate(Vector3.forward * _currentRotationSpeed * Time.deltaTime);
    }
}