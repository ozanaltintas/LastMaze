using UnityEngine;

public class SwipeInputReader : MonoBehaviour
{
    // Diğer scriptlerin bu veriye ulaşması için Singleton yapısı veya statik referans
    public static SwipeInputReader Instance;

    [Header("Ayarlar")]
    [Tooltip("Maksimum hıza ulaşmak için parmağın kaç piksel kayması gerekiyor?")]
    public float dragThreshold = 200f; 

    // Bu değer -1 (tam sol) ile 1 (tam sağ) arasında değişecek
    private float _horizontalInput;

    private Vector2 _startTouchPosition;
    private Vector2 _currentTouchPosition;
    private bool _isTouching = false;

    // Dışarıdan okumak için Property
    public float HorizontalInput => _horizontalInput;

    private void Awake()
    {
        // Basit Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        _horizontalInput = 0f; // Dokunulmadığında hız 0 olsun

        #if UNITY_EDITOR
        // Editörde test etmek için Mouse kullanımı
        if (Input.GetMouseButtonDown(0))
        {
            _isTouching = true;
            _startTouchPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0) && _isTouching)
        {
            _currentTouchPosition = Input.mousePosition;
            CalculateInput();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isTouching = false;
        }
        #else
        // Mobilde Dokunmatik Kontrol
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _isTouching = true;
                _startTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                _currentTouchPosition = touch.position;
                CalculateInput();
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                _isTouching = false;
            }
        }
        #endif
    }

    private void CalculateInput()
    {
        // Başlangıç noktası ile şu anki nokta arasındaki X farkını al
        float differenceX = _currentTouchPosition.x - _startTouchPosition.x;

        // Bu farkı eşik değere bölerek -1 ile 1 arasında bir oran bul
        // Mathf.Clamp ile değerin -1 ve 1 dışına çıkmasını engelliyoruz
        _horizontalInput = Mathf.Clamp(differenceX / dragThreshold, -1f, 1f);
    }
}