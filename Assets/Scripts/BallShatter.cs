using UnityEngine;
using System.Collections;

public class BallShatter : MonoBehaviour
{
    [Header("Efekt Ayarları")]
    public ParticleSystem ballExplosionPrefab; // Topa özel patlama efekti
    public float patlamaGecikmesi = 0.5f;      // Düşmeye başladıktan ne kadar sonra patlasın?

    [Header("Görünüm")]
    public int parcaSayisi = 20;               // Kaç parça çıksın?
    public float parcaBoyutu = 0.3f;           // Parçalar ne kadar büyük olsun?

    private SpriteRenderer ballRenderer;
    private Rigidbody2D rb;
    private Collider2D col;

    void Start()
    {
        ballRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    // Bu fonksiyon dışarıdan çağrılacak
    public void Patlat()
    {
        StartCoroutine(PatlamaSureci());
    }

    IEnumerator PatlamaSureci()
    {
        // 1. Gecikme süresi kadar bekle (Top düşerken süzülsün)
        yield return new WaitForSeconds(patlamaGecikmesi);

        // 2. Topu Gizle ve Durdur
        if (ballRenderer != null) ballRenderer.enabled = false;
        if (col != null) col.enabled = false;
        
        // Hareketi dondur ki patlama havada asılı kalsın (İsteğe bağlı, açıp kapatabilirsin)
        if (rb != null) rb.linearVelocity = Vector2.zero; 

        // 3. Efekti Oluştur
        if (ballExplosionPrefab != null)
        {
            ParticleSystem ps = Instantiate(ballExplosionPrefab, transform.position, Quaternion.identity);
            
            // Rengi topun kendi rengi yap
            var main = ps.main;
            if (ballRenderer != null) main.startColor = ballRenderer.color;

            // Ayarları uygula ve fırlat
            var emitParams = new ParticleSystem.EmitParams();
            emitParams.startSize = parcaBoyutu;
            
            // Topun rengini garantiye al
            if (ballRenderer != null) emitParams.startColor = ballRenderer.color;

            ps.Emit(emitParams, parcaSayisi);

            // Temizlik
            Destroy(ps.gameObject, 2f);
        }
    }
}