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
    public float ucusSuresi = 0.8f;

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

        efektText.gameObject.SetActive(true);
        efektText.text = "+" + kazanilanMiktar;
        efektText.rectTransform.anchoredPosition = Vector2.zero; 
        efektText.transform.localScale = Vector3.zero; 

        Sequence seq = DOTween.Sequence();
        seq.Append(efektText.transform.DOScale(Vector3.one * 1.5f, 0.4f).SetEase(Ease.OutBack));
        seq.AppendInterval(beklemeSuresi);
        seq.Append(efektText.transform.DOMove(hedefNokta.position, ucusSuresi).SetEase(Ease.InBack));
        seq.Join(efektText.transform.DOScale(Vector3.one * 0.4f, ucusSuresi).SetEase(Ease.InQuad));
        seq.OnComplete(() => {
            efektText.gameObject.SetActive(false); 
            SayaciArttir(kazanilanMiktar); 
        });
    }

    // --- KAZANMA EFEKTİ (ARTMA) ---
    void SayaciArttir(int miktar)
    {
        int eskiDeger = mevcutBakiye;
        mevcutBakiye += miktar;

        DOVirtual.Float(eskiDeger, mevcutBakiye, 0.5f, (v) => {
            if (bankaText != null) bankaText.text = Mathf.FloorToInt(v).ToString();
        }).OnComplete(() => {
            UpdateBankaText(mevcutBakiye);
            if (bankaText != null) bankaText.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, 10, 1);
        });
    }

    // --- HARCAMA EFEKTİ (AZALMA - YENİ) ---
    public void HarcamaYap(int miktar)
    {
        int eskiDeger = mevcutBakiye;
        mevcutBakiye -= miktar;
        if (mevcutBakiye < 0) mevcutBakiye = 0; // Negatife düşmesin

        // Eski değerden yeni değere geriye doğru say
        DOVirtual.Float(eskiDeger, mevcutBakiye, 0.5f, (v) => {
            if (bankaText != null) bankaText.text = Mathf.FloorToInt(v).ToString();
        }).OnComplete(() => {
            UpdateBankaText(mevcutBakiye);
            // Azalınca hafifçe titreyip kırmızılaşabilir (Juice)
            if (bankaText != null) 
            {
                bankaText.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 10, 1);
                // İstersen burada anlık kırmızı yapıp düzeltebilirsin
            }
        });
    }

    void UpdateBankaText(int deger)
    {
        if (bankaText != null) bankaText.text = deger.ToString();
    }
}