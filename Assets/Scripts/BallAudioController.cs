using UnityEngine;

[RequireComponent(typeof(AudioSource))] // Bu scripti attığın objede AudioSource yoksa otomatik ekler
public class BallAudioController : MonoBehaviour
{
    [Header("Ses Dosyası")]
    public AudioClip hitSound; // TIK sesi buraya

    [Header("Hassasiyet Ayarları")]
    [Tooltip("Sesin çalması için gereken minimum çarpma hızı.")]
    public float minCollisionSpeed = 1.0f; 

    [Tooltip("Maksimum ses seviyesine ulaşmak için gereken çarpma hızı.")]
    public float maxCollisionSpeed = 10.0f;

    [Header("Çeşitlilik (Doğallık)")]
    [Tooltip("Her çarpışmada ses perdesi hafif değişsin mi? (Daha doğal duyulur)")]
    public bool randomizePitch = true;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Çarpışmanın şiddetini (hız farkını) al
        float impactSpeed = collision.relativeVelocity.magnitude;

        // 2. Eğer çarpışma çok yavaşsa (sadece yuvarlanıyorsa) ses çalma
        if (impactSpeed < minCollisionSpeed) return;

        // 3. Hıza göre ses seviyesi hesapla (0 ile 1 arasında)
        // InverseLerp: impactSpeed değeri min ile max arasındaysa 0-1 arası oran verir.
        float volume = Mathf.InverseLerp(minCollisionSpeed, maxCollisionSpeed, impactSpeed);

        // 4. Perde (Pitch) rastgeleliği (Opsiyonel ama tavsiye edilir)
        if (randomizePitch)
        {
            _audioSource.pitch = Random.Range(0.85f, 1.15f);
        }
        else
        {
            _audioSource.pitch = 1f;
        }

        // 5. Sesi Çal
        // PlayOneShot kullanıyoruz çünkü top peş peşe 3 kez çarpabilir, 
        // Play() deseydik önceki sesi keserdi. PlayOneShot üst üste bindirir.
        _audioSource.PlayOneShot(hitSound, volume);
    }
}