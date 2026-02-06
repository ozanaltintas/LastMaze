using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System; 

[RequireComponent(typeof(CanvasGroup))] 
public class TimeShopButton : MonoBehaviour
{
    [Header("Satış Ayarları")]
    [Tooltip("Bu butona basınca kaç saniye alınacak?")]
    public int saniyeMiktari = 10;

    [Header("UI Bağlantıları")]
    public Button butonComponent;
    public TextMeshProUGUI fiyatYazisi; 

    private GameTimer gameTimer;
    private CanvasGroup canvasGroup;
    
    // Satın alındı mı kontrolü
    private bool buButonSatinAlindi = false;

    public static event Action<TimeShopButton> OnHerhangiBiriSatinAlindi;

    void Start()
    {
        gameTimer = FindFirstObjectByType<GameTimer>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (butonComponent == null)
            butonComponent = GetComponent<Button>();

        if (butonComponent != null)
        {
            butonComponent.onClick.AddListener(SatinAl);
        }

        if (fiyatYazisi != null)
        {
            fiyatYazisi.text = "+" + saniyeMiktari + " SN";
        }

        OnHerhangiBiriSatinAlindi += Kilitle;
    }

    void OnDestroy()
    {
        OnHerhangiBiriSatinAlindi -= Kilitle;
    }

    void Update()
    {
        // 1. EĞER BU BUTON ZATEN SATIN ALINDI YA DA BAŞKASI ALINDI DİYE KİLİTLENDİYSE
        // HİÇBİR ŞEY YAPMA (Kilitli kalsın)
        if (butonComponent != null && !butonComponent.interactable && buButonSatinAlindi) 
        {
             // Eğer bu buton özel olarak satın alındıysa alpha 1 kalsın (görünsün ama basılmasın)
             // Ya da senin tercihin: silik kalsın. Aşağıdaki mantıkla devam edelim.
             return;
        }
        
        // Eğer herhangi bir alım yapıldıysa (Event ile kilitlendiyse) update çalışmasın
        if (butonComponent != null && !butonComponent.interactable && canvasGroup.alpha <= 0.3f)
            return;

        // 2. BAKİYE KONTROLÜ (Henüz kimse bir şey almadıysa)
        int mevcutBakiye = PlayerPrefs.GetInt("TimeBank", 0);
        
        if (butonComponent != null)
        {
            bool paraYeterli = mevcutBakiye >= saniyeMiktari;
            
            // Paran yetiyorsa buton aktif, yetmiyorsa pasif
            butonComponent.interactable = paraYeterli;
            
            // Paran yetiyorsa parlak, yetmiyorsa yarı saydam (0.5f)
            if (canvasGroup != null)
            {
                canvasGroup.alpha = paraYeterli ? 1f : 0.5f; 
            }
        }
    }

    void SatinAl()
    {
        if (butonComponent != null && !butonComponent.interactable) return;

        int mevcutBakiye = PlayerPrefs.GetInt("TimeBank", 0);

        if (mevcutBakiye >= saniyeMiktari)
        {
            buButonSatinAlindi = true;

            // 1. Bankadan düş
            mevcutBakiye -= saniyeMiktari;
            PlayerPrefs.SetInt("TimeBank", mevcutBakiye);
            PlayerPrefs.Save();

            // 2. Animasyonu Tetikle
            if (InGameTimeBank.Instance != null)
            {
                InGameTimeBank.Instance.HarcamaYap(saniyeMiktari);
            }

            // 3. Oyuna süreyi ekle ve devam ettir
            if (gameTimer != null)
            {
                gameTimer.ZamanEkle(saniyeMiktari);
                gameTimer.OyunuDevamEttir();
            }

            Debug.Log(saniyeMiktari + " saniye alındı.");

            // 4. Herkese haber ver (Dükkanı kapat)
            OnHerhangiBiriSatinAlindi?.Invoke(this);
        }
    }

    void Kilitle(TimeShopButton satinAlanButon)
    {
        // Butonu tıklanamaz yap
        if (butonComponent != null)
        {
            butonComponent.interactable = false; 
        }

        // Görünümü silikleştir (Dükkan kapandı hissi)
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.3f; 
        }
    }
}