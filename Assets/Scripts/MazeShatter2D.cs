using UnityEngine;

public class MazeShatter2D : MonoBehaviour
{
    [Header("Temel Ayarlar")]
    public ParticleSystem explosionPrefab; 

    [Header("Ses Ayarları (YENİ)")]
    public AudioSource audioSource; // Labirentin üzerindeki Audio Source
    public AudioClip pitSesi;       // İstediğin o 'Pıt' sesi
    [Range(0f, 1f)] public float sesSiddeti = 0.3f;

    [Header("Miktar ve Sıklık Ayarları")]
    [Tooltip("Collider üzerindeki her kaçıncı noktada patlama olsun?")]
    [Range(1, 20)] public int noktaAtlamaSikligi = 1; 
    public int parcaSayisi = 1;

    [Header("Boyut ve Konum")]
    public float parcaBoyutu = 0.2f;
    public Vector2 pozisyonKaydirma = Vector2.zero;

    private SpriteRenderer mazeRenderer;
    private PolygonCollider2D mazePoly;

    void Start()
    {
        mazeRenderer = GetComponent<SpriteRenderer>();
        mazePoly = GetComponent<PolygonCollider2D>();
        
        // Eğer AudioSource atamayı unuttuysan kod otomatik bulmaya çalışsın
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    public void Shatter()
    {
        if (mazeRenderer.enabled == false) return;

        // 1. Görüntüyü Kapat
        mazeRenderer.enabled = false;
        if (mazePoly != null) mazePoly.enabled = false;

        // 2. SESİ ÇAL (Prefab çağrıldığı an burası)
        if (audioSource != null && pitSesi != null)
        {
            audioSource.PlayOneShot(pitSesi, sesSiddeti);
        }

        // 3. Prefabı Çağır (Patlama Efekti)
        if (explosionPrefab != null)
        {
            ParticleSystem ps = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            
            var main = ps.main;
            main.startColor = Color.black;

            if (mazePoly != null)
            {
                for (int i = 0; i < mazePoly.pathCount; i++)
                {
                    Vector2[] points = mazePoly.GetPath(i);
                    for (int j = 0; j < points.Length; j += noktaAtlamaSikligi)
                    {
                        Vector2 localPoint = points[j];
                        Vector3 worldPos = transform.TransformPoint(localPoint);
                        
                        worldPos.x += pozisyonKaydirma.x;
                        worldPos.y += pozisyonKaydirma.y;
                        worldPos.z = -1f; 

                        var emitParams = new ParticleSystem.EmitParams();
                        emitParams.position = worldPos;
                        emitParams.startColor = Color.black; 
                        emitParams.startSize = parcaBoyutu;

                        ps.Emit(emitParams, parcaSayisi);
                    }
                }
            }
            else
            {
                // Collider yoksa tek nokta patlat
                var emitParams = new ParticleSystem.EmitParams();
                emitParams.position = transform.position + (Vector3)pozisyonKaydirma;
                emitParams.startSize = parcaBoyutu;
                emitParams.startColor = Color.black;
                ps.Emit(emitParams, parcaSayisi * 10);
            }

            Destroy(ps.gameObject, 3f);
        }
    }
}