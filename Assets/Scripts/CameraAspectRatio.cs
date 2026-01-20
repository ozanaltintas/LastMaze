using UnityEngine;

public class CameraAspectRatio : MonoBehaviour
{
    [Header("Hedef En-Boy Oranı")]
    [Tooltip("Örneğin 9:16 telefon oranı için X=9, Y=16 girin.")]
    public Vector2 targetAspect = new Vector2(9, 16);

    void Start()
    {
        UpdateCameraCrop();
    }

    // Ekran boyutu değişirse (özellikle Web'de pencere boyutu değişebilir) tekrar hesapla
    void Update()
    {
        // Performans için bunu her karede yapmak yerine sadece boyut değişince yapmak daha iyidir
        // ama WebGL'de pencere esnetildiğinde anlık tepki için Update'de kalabilir.
        UpdateCameraCrop();
    }

    void UpdateCameraCrop()
    {
        // Hedef oran (Örn: 9/16 = 0.5625)
        float targetRatio = targetAspect.x / targetAspect.y;

        // Şu anki ekran oranı
        float windowAspect = (float)Screen.width / (float)Screen.height;

        // Mevcut oran ile hedef oran arasındaki ölçek farkı
        float scaleHeight = windowAspect / targetRatio;

        Camera camera = GetComponent<Camera>();

        // Eğer ekran hedeften daha genişse (Masaüstü/Tablet durumu)
        // Yanlara siyah bar (Pillarbox) ekle
        if (scaleHeight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            camera.rect = rect;
        }
        else // Eğer ekran hedeften daha darsa (Çok uzun telefonlar)
        // Üst ve alta siyah bar (Letterbox) ekle
        {
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = camera.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
    }
}