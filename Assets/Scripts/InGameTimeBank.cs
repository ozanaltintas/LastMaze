using UnityEngine;
using TMPro;
using DG.Tweening;

public class InGameTimeBank : MonoBehaviour
{
    public static InGameTimeBank Instance;

    [Header("UI Elemanları")]
    public TextMeshProUGUI bankaText;   
    public TextMeshProUGUI efektText;   
    public RectTransform hedefNokta;    

    [Header("Ayarlar")]
    public float beklemeSuresi = 0.5f;  
    public float ucusSuresi = 0.8f;     // Hızı biraz daha yavaşlattık (0.7 -> 0.8)

    private int mevcutBakiye;

    void Awake()
    {
        if (Instance == null) Instance = this;
        
        if (efektText != null) 
        {
            efektText.gameObject.SetActive(false);
            efektText.transform.localScale = Vector3.zero;
        }
    }

    void Start()
    {
        mevcutBakiye = PlayerPrefs.GetInt("TimeBank", 0);
        UpdateBankaText(mevcutBakiye);
    }

    public void AnimasyonuBaslat(int kazanilanMiktar)
    {
        if (efektText == null || bankaText == null) return;

        // Hazırlık
        efektText.gameObject.SetActive(true);
        efektText.text = "+" + kazanilanMiktar;
        efektText.rectTransform.anchoredPosition = Vector2.zero; 
        efektText.transform.localScale = Vector3.zero; 

        Sequence seq = DOTween.Sequence();

        // A) POP UP: Ortada büyüyerek açıl
        seq.Append(efektText.transform.DOScale(Vector3.one * 1.5f, 0.4f).SetEase(Ease.OutBack));
        
        // B) BEKLE
        seq.AppendInterval(beklemeSuresi);

        // C) UÇUŞ VE KÜÇÜLME (ÖNEMLİ DEĞİŞİKLİK BURADA)
        
        // Hareket: Yine hafif gerilerek fırlasın (InBack)
        seq.Append(efektText.transform.DOMove(hedefNokta.position, ucusSuresi).SetEase(Ease.InBack));
        
        // Küçülme: Artık "Vector3.zero" değil, "0.4f" boyutuna küçülecek (Tam kaybolmasın)
        // Ease.InQuad: Başta YAVAŞ küçül, sonda HIZLI küçül. (Böylece havada büyük kalır)
        seq.Join(efektText.transform.DOScale(Vector3.one * 0.4f, ucusSuresi).SetEase(Ease.InQuad));

        // D) VARIŞ
        seq.OnComplete(() => {
            efektText.gameObject.SetActive(false); 
            SayaciArttir(kazanilanMiktar); 
        });
    }

    void SayaciArttir(int miktar)
    {
        int eskiDeger = mevcutBakiye;
        mevcutBakiye += miktar;

        // Sayaç pıt pıt artsın
        DOVirtual.Float(eskiDeger, mevcutBakiye, 0.5f, (v) => {
            bankaText.text = Mathf.FloorToInt(v).ToString();
        }).OnComplete(() => {
            UpdateBankaText(mevcutBakiye);
            
            // Köşedeki sayaç bi "Zıplasın"
            bankaText.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, 10, 1);
        });
    }

    void UpdateBankaText(int deger)
    {
        if (bankaText != null) bankaText.text = deger.ToString();
    }
}