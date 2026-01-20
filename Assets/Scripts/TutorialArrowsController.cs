using UnityEngine;
using UnityEngine.UI; // Image kontrolü için şart

public class TutorialArrowsController : MonoBehaviour
{
    [Header("Görseller")]
    [Tooltip("Yanıp sönecek ok objeleri (Sol ve Sağ Ok)")]
    public GameObject[] arrows; 

    [Header("Animasyon Ayarları")]
    [Tooltip("Yanıp sönme hızı")]
    public float blinkSpeed = 2f;
    [Tooltip("En düşük görünürlük (0 = tam şeffaf, 1 = tam katı)")]
    public float minAlpha = 0.2f;
    [Tooltip("En yüksek görünürlük")]
    public float maxAlpha = 1.0f;

    private CanvasGroup _canvasGroup; // Toplu şeffaflık kontrolü için

    void Start()
    {
        // 1. Okların ebeveynine (bu scriptin olduğu obje) CanvasGroup ekleyelim
        // Böylece tek tek Image'larla uğraşmak yerine hepsini aynı anda soldurabiliriz.
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // 2. UIWheelController'ı dinlemeye başla
        if (UIWheelController.Instance != null)
        {
            UIWheelController.Instance.OnFirstTouch += HideArrows;
        }
    }

    void Update()
    {
        // Yanıp sönme (PingPong) efekti
        // Mathf.PingPong, 0 ile 1 arasında sürekli gidip gelen bir değer üretir.
        // Bunu minAlpha ve maxAlpha aralığına oturtuyoruz (Lerp).
        float pingPong = Mathf.PingPong(Time.time * blinkSpeed, 1f);
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, pingPong);

        _canvasGroup.alpha = alpha;
    }

    // Olay tetiklendiğinde çalışacak fonksiyon
    private void HideArrows()
    {
        // 1. Olayı dinlemeyi bırak (Hata almamak için önemli)
        if (UIWheelController.Instance != null)
        {
            UIWheelController.Instance.OnFirstTouch -= HideArrows;
        }

        // 2. Kendini yok et veya kapat
        gameObject.SetActive(false); 
        // İstersen Destroy(gameObject) de diyebilirsin.
    }

    // Script yok edilirse (Sahne değişimi vb.) event dinlemeyi kes
    void OnDestroy()
    {
        if (UIWheelController.Instance != null)
        {
            UIWheelController.Instance.OnFirstTouch -= HideArrows;
        }
    }
}