using UnityEngine;
using TMPro;
using DG.Tweening; // DOTween kütüphanesi

public class MainMenuFlipCounter : MonoBehaviour
{
    [Header("UI Ayarları")]
    public TextMeshProUGUI mainText;
    public string onEk = "LEVEL ";
    public float hiz = 0.3f; // Takla hızı

    [Header("Ses Ayarları (YENİ)")]
    public AudioSource audioSource;
    public AudioClip flipSound; // Mekanik 'Click' veya 'Flap' sesi

    void Start()
    {
        // Kaydedilmiş verileri al
        int realLevel = PlayerPrefs.GetInt("ReachedLevel", 1);
        int displayLevel = PlayerPrefs.GetInt("MenuDisplayLevel", 1);

        // Başlangıçta eski leveli göster
        mainText.text = onEk + displayLevel;

        // Eğer yeni bir level kazanılmışsa animasyon yap
        if (realLevel > displayLevel)
        {
            // --- DOTween ZİNCİRİ ---
            Sequence mySequence = DOTween.Sequence();
            
            // 1. Biraz bekle (Göz alışsın)
            mySequence.AppendInterval(0.5f);
            
            // 2. KAPANIŞ: X ekseninde 90 derece dön (Arkaya yat)
            mySequence.Append(mainText.rectTransform.DOLocalRotate(new Vector3(90, 0, 0), hiz).SetEase(Ease.InBack));
            
            // 3. DEĞİŞİM ANI: Tam kapandığında yapılacaklar
            mySequence.AppendCallback(() => {
                
                // A) Metni güncelle
                mainText.text = onEk + realLevel;
                
                // B) Açıyı hazırla (Arkadan gelmesi için -90 yap)
                mainText.rectTransform.localEulerAngles = new Vector3(-90, 0, 0);

                // C) SESİ ÇAL! (Tam bu anda çalar)
                if (audioSource != null && flipSound != null)
                {
                    // Pitch (ton) ile hafif oynayarak daha doğal duyulmasını sağlarız
                    audioSource.pitch = Random.Range(0.9f, 1.1f); 
                    audioSource.PlayOneShot(flipSound);
                }
            });
            
            // 4. AÇILIŞ: X ekseninde 0 dereceye dön (Yerine otur)
            // OutBack efekti ile hafifçe yaylanarak oturur
            mySequence.Append(mainText.rectTransform.DOLocalRotate(Vector3.zero, hiz).SetEase(Ease.OutBack));

            // 5. KAYIT: İşlem bitince hafızayı güncelle
            mySequence.OnComplete(() => {
                PlayerPrefs.SetInt("MenuDisplayLevel", realLevel);
                PlayerPrefs.Save();
            });
        }
    }

    // Sağ tık menüsüyle test et
    [ContextMenu("Sıfırla")]
    public void Sifirla() { PlayerPrefs.DeleteAll(); Debug.Log("Sıfırlandı!"); }
    
    [ContextMenu("Test - Level Atlat")]
    public void LevelAtlat() { 
        PlayerPrefs.SetInt("ReachedLevel", PlayerPrefs.GetInt("ReachedLevel", 1) + 1); 
        Debug.Log("Level atlandı, Play'e basınca göreceksin."); 
    }
}