using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ElasticHaloManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MenuController menuController;
    [SerializeField] private RectTransform haloTransform;
    [SerializeField] private GameObject settingsWheelObject;

    [Header("Elastic Settings")]
    [SerializeField] private float posSmoothTime = 0.05f; // Hızlandırıldı (0.1f -> 0.05f)
    [SerializeField] private float sizeSmoothTime = 0.08f; // Hızlandırıldı (0.12f -> 0.08f)
    [SerializeField] private float stretchIntensity = 0.3f;
    [SerializeField] private Vector2 padding = new Vector2(20, 20);

    private Vector3 posVelocity;
    private Vector2 sizeVelocity;
    private RectTransform targetRect;
    private Vector3 lastPosition;

    void Start()
    {
        if (haloTransform != null) haloTransform.gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        // 1. Menü kapalıysa veya referans yoksa kapat
        if (menuController == null || !menuController.IsMenuOpen())
        {
            if (haloTransform.gameObject.activeSelf) haloTransform.gameObject.SetActive(false);
            targetRect = null;
            return;
        }

        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        // 2. Çark seçiliyse halo'yu gizle (yanıp sönmeyi önler)
        if (currentSelected == settingsWheelObject)
        {
            if (haloTransform.gameObject.activeSelf) haloTransform.gameObject.SetActive(false);
            return;
        }

        // 3. Geçerli bir seçim varsa hedefi güncelle
        if (currentSelected != null)
        {
            RectTransform newTarget = currentSelected.GetComponent<RectTransform>();
            if (newTarget != null)
            {
                targetRect = newTarget;
                if (!haloTransform.gameObject.activeSelf) haloTransform.gameObject.SetActive(true);
            }
        }

        // 4. Takip Mantığı
        if (targetRect != null && haloTransform.gameObject.activeSelf)
        {
            HandleElasticMovement();
        }
    }

    void HandleElasticMovement()
    {
        // Buton hareket halindeyken (pıt pıt açılırken) bile anlık pozisyonu al
        Vector3 targetPos = targetRect.position;
        Vector2 targetSize = targetRect.sizeDelta + padding;

        // SmoothDamp: Hedefe kilitlenmeyi sağlar
        haloTransform.position = Vector3.SmoothDamp(haloTransform.position, targetPos, ref posVelocity, posSmoothTime);
        haloTransform.sizeDelta = Vector2.SmoothDamp(haloTransform.sizeDelta, targetSize, ref sizeVelocity, sizeSmoothTime);

        // Hız bazlı esneme hesaplama
        float movementSpeed = (haloTransform.position - lastPosition).magnitude / Time.deltaTime;
        Vector3 moveDir = (haloTransform.position - lastPosition).normalized;

        if (movementSpeed > 5f) // Tolerans düşürüldü, daha hassas tepki verir
        {
            float stretch = 1f + (movementSpeed * stretchIntensity * 0.0005f);
            float squash = 1f / stretch;
            float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            
            haloTransform.rotation = Quaternion.Euler(0, 0, angle);
            haloTransform.localScale = new Vector3(stretch, squash, 1f);
        }
        else
        {
            // Durduğunda hızlıca düzel
            haloTransform.localScale = Vector3.Lerp(haloTransform.localScale, Vector3.one, Time.deltaTime * 20f);
            haloTransform.rotation = Quaternion.Lerp(haloTransform.rotation, Quaternion.identity, Time.deltaTime * 20f);
        }

        lastPosition = haloTransform.position;
    }
}