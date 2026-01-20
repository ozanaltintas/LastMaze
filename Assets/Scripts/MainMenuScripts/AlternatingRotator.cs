using UnityEngine;

public class AlternatingRotator : MonoBehaviour
{
    [Header("Dönüş Ayarları")]
    [Tooltip("Saniyedeki dönüş hızı (Derece cinsinden)")]
    public float speed = 90f;

    [Tooltip("Hangi eksende dönecek? (2D için genelde Z: -1 yapılır)")]
    public Vector3 rotationAxis = new Vector3(0, 0, -1);

    [Header("Zamanlama")]
    [Tooltip("Kaç saniyede bir yön değiştirsin?")]
    public float changeInterval = 2f; // Örneğin her 2 saniyede bir yön değişir

    private float timer;

    void Update()
    {
        // 1. Nesneyi döndür
        transform.Rotate(rotationAxis * speed * Time.deltaTime);

        // 2. Zamanı say
        timer += Time.deltaTime;

        // 3. Süre dolduysa yönü tersine çevir
        if (timer >= changeInterval)
        {
            speed = -speed; // Hızı eksi ile çarparak (100 -> -100) yönü tersleriz
            timer = 0f;     // Sayacı sıfırla ki tekrar saymaya başlasın
        }
    }
}