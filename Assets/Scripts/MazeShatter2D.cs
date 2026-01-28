using UnityEngine;

public class MazeShatter2D : MonoBehaviour
{
    [Header("Temel Ayarlar")]
    public ParticleSystem explosionPrefab; 

    [Header("Ses Ayarları (Patlama)")]
    public AudioSource audioSource;
    public AudioClip pitSesi;
    [Range(0f, 1f)] public float sesSiddeti = 1f;

    [Header("Kontrol Ayarları (DÜZELTİLDİ)")]
    [Tooltip("Labirenti döndüren objeyi (GameObject) buraya sürükle.")]
    public GameObject labirentObjesi; // Script yerine direkt Objenin kendisini alalım, daha garanti.
    
    [Tooltip("Labirentin dönmesini sağlayan scriptin tam adı (Örn: MazeRotator)")]
    public string scriptAdi = "MazeRotator"; // Scriptin adını buraya yazacağız.

    [Header("Miktar ve Sıklık")]
    [Range(1, 20)] public int noktaAtlamaSikligi = 2; 
    public int parcaSayisi = 8; 

    [Header("Boyut ve Konum")]
    public float parcaBoyutu = 0.4f;
    public Vector2 pozisyonKaydirma = Vector2.zero;

    private SpriteRenderer mazeRenderer;
    private PolygonCollider2D mazePoly;

    void Start()
    {
        mazeRenderer = GetComponent<SpriteRenderer>();
        mazePoly = GetComponent<PolygonCollider2D>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    public void Shatter()
    {
        if (mazeRenderer.enabled == false) return;

        // 1. GÖRÜNTÜYÜ KAPAT
        mazeRenderer.enabled = false;
        if (mazePoly != null) mazePoly.enabled = false;

        // 2. HAREKETİ VE SESİ KAPAT (GÜNCELLENDİ) 🛑
        if (labirentObjesi != null)
        {
            // A) Scripti bul ve kapat (İsmi neyse onu bulur)
            MonoBehaviour hareketScripti = labirentObjesi.GetComponent(scriptAdi) as MonoBehaviour;
            if (hareketScripti != null)
            {
                hareketScripti.enabled = false;
            }

            // B) O objede çalan dönme sesini (AudioSource) bul ve SUSTUR!
            AudioSource donmeSesi = labirentObjesi.GetComponent<AudioSource>();
            if (donmeSesi != null)
            {
                donmeSesi.Stop(); // Sesi anında kes
                donmeSesi.loop = false; // Tekrar etmesini engelle
            }
        }

        // 3. PATLAMA SESİNİ ÇAL
        if (audioSource != null && pitSesi != null)
        {
            audioSource.PlayOneShot(pitSesi, sesSiddeti);
        }

        // 4. PATLAMA EFEKTİ
        if (explosionPrefab != null)
        {
            ParticleSystem ps = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            var main = ps.main;
            main.startColor = Color.black;
            main.maxParticles = 10000; 

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
                var emitParams = new ParticleSystem.EmitParams();
                emitParams.position = transform.position + (Vector3)pozisyonKaydirma;
                emitParams.startSize = parcaBoyutu;
                emitParams.startColor = Color.black;
                ps.Emit(emitParams, 1000); 
            }

            Destroy(ps.gameObject, 3f);
        }
    }
}