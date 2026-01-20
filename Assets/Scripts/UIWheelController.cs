using UnityEngine;
using UnityEngine.EventSystems; // UI etkileşimleri için
using System; // Action eventi için

public class UIWheelController : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    // Diğer scriptlerin (MazeRotator, TutorialArrows) ulaşması için Singleton
    public static UIWheelController Instance;

    // --- EVENTLER ---
    // Oyuncu ilk kez dokunduğunda çalışacak olay (Okları kapatmak için)
    public event Action OnFirstTouch;
    private bool _hasTouchedOnce = false;

    [Header("Ayarlar")]
    [Tooltip("Butonun merkezden sağa/sola gidebileceği maksimum piksel mesafesi")]
    public float maxDistance = 200f;

    [Tooltip("Bırakınca buton merkeze geri dönsün mü?")]
    public bool snapBackToCenter = true;

    [Tooltip("Titremeyi önlemek için minimum giriş değeri (0.0 ile 1.0 arası)")]
    public float deadzone = 0.05f;

    // --- ÖZEL DEĞİŞKENLER ---
    private RectTransform _rectTransform;
    private Vector2 _startPosition;
    private float _currentInput; // -1 (Sol) ile 1 (Sağ) arası değer

    // Dışarıdan okunan değer (Property)
    public float HorizontalInput => _currentInput;

    private void Awake()
    {
        // Singleton Kurulumu
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _rectTransform = GetComponent<RectTransform>();
        _startPosition = _rectTransform.anchoredPosition; // Başlangıç (Merkez) konumu kaydet
    }

    // Oyuncu butona bastığı an çalışır
    public void OnPointerDown(PointerEventData eventData)
    {
        // Eğer bu ilk dokunuşsa, olayı tetikle (Tutorial okları dinliyorsa kapanır)
        if (!_hasTouchedOnce)
        {
            _hasTouchedOnce = true;
            OnFirstTouch?.Invoke();
        }
    }

    // Oyuncu sürüklediği sürece çalışır
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        
        // Ekran dokunuşunu Canvas düzlemine çeviriyoruz (Farklı ekran boyutları için şart)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectTransform.parent as RectTransform, 
            eventData.position, 
            eventData.pressEventCamera, 
            out localPoint);

        // Yeni pozisyonu hesapla (Sadece X ekseni)
        float newX = localPoint.x;

        // Butonun belirlenen alan dışına çıkmasını engelle (Clamp)
        newX = Mathf.Clamp(newX, -maxDistance, maxDistance);

        // Butonu görsel olarak taşı
        _rectTransform.anchoredPosition = new Vector2(newX, _startPosition.y);

        // Input değerini hesapla (-1 sol, 0 merkez, 1 sağ)
        float calculatedInput = newX / maxDistance;

        // Deadzone Kontrolü: Eğer değer çok küçükse 0 kabul et
        if (Mathf.Abs(calculatedInput) < deadzone)
        {
            _currentInput = 0f;
        }
        else
        {
            _currentInput = calculatedInput;
        }
    }

    // Oyuncu parmağını çektiği an çalışır
    public void OnPointerUp(PointerEventData eventData)
    {
        if (snapBackToCenter)
        {
            // Butonu merkeze sıfırla
            _rectTransform.anchoredPosition = _startPosition;
            _currentInput = 0f;
        }
        else
        {
            // Eğer "gaz pedalı" gibi olduğu yerde kalmasını istersen burayı boş bırak
        }
    }
}