using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
    [System.Serializable]
    public class MenuButtonData
    {
        public RectTransform buttonTransform;
        public Vector2 direction; // Örn: (1,0) Sağ, (0,-1) Aşağı
    }

    [Header("Elements")]
    [SerializeField] private RectTransform settingsWheel;
    [SerializeField] private List<MenuButtonData> menuButtons;
    
    [Header("Animation Settings")]
    [SerializeField] private float unitDistance = 120f;
    [SerializeField] private float staggerDelay = 0.08f; 
    [SerializeField] private float animationDuration = 0.4f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

    private bool isOpen = false;
    private Vector2 startPos;
    private Coroutine activeAnimation;

    void Start()
    {
        startPos = settingsWheel.anchoredPosition;
        foreach (var btn in menuButtons)
        {
            btn.buttonTransform.anchoredPosition = startPos;
            btn.buttonTransform.localScale = Vector3.zero;
            btn.buttonTransform.gameObject.SetActive(false);
        }
    }

    public void ToggleMenu()
    {
        isOpen = !isOpen;

        // Açılma/Kapanma sesini çal
        if (audioSource != null)
        {
            AudioClip clip = isOpen ? openSound : closeSound;
            if (clip != null) audioSource.PlayOneShot(clip);
        }

        if (activeAnimation != null) StopCoroutine(activeAnimation);
        activeAnimation = StartCoroutine(AnimateMenu());
    }

    IEnumerator AnimateMenu()
    {
        float elapsedTime = 0;
        float totalAnimTime = animationDuration + (menuButtons.Count * staggerDelay);

        while (elapsedTime < totalAnimTime)
        {
            elapsedTime += Time.deltaTime;

            // 1. Çark Dönüşü
            float wheelT = Mathf.Clamp01(elapsedTime / animationDuration);
            float rotationAngle = isOpen ? Mathf.Lerp(0, 360, wheelT) : Mathf.Lerp(360, 0, wheelT);
            settingsWheel.localRotation = Quaternion.Euler(0, 0, -rotationAngle);

            // 2. Butonların Fırlama ve Ölçeklenme Animasyonu
            for (int i = 0; i < menuButtons.Count; i++)
            {
                float buttonDelay = i * staggerDelay;
                float buttonT = Mathf.Clamp01((elapsedTime - buttonDelay) / animationDuration);
                float easeT = EvaluateEaseOutBack(buttonT);

                Vector2 targetOffset = menuButtons[i].direction * unitDistance;
                Vector2 targetPos = startPos + targetOffset;

                if (isOpen)
                {
                    if (buttonT > 0 && !menuButtons[i].buttonTransform.gameObject.activeSelf) 
                        menuButtons[i].buttonTransform.gameObject.SetActive(true);

                    menuButtons[i].buttonTransform.anchoredPosition = Vector2.LerpUnclamped(startPos, targetPos, easeT);
                    menuButtons[i].buttonTransform.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, easeT);
                }
                else
                {
                    menuButtons[i].buttonTransform.anchoredPosition = Vector2.LerpUnclamped(targetPos, startPos, easeT);
                    menuButtons[i].buttonTransform.localScale = Vector3.LerpUnclamped(Vector3.one, Vector3.zero, easeT);
                    
                    if (buttonT >= 1f) menuButtons[i].buttonTransform.gameObject.SetActive(false);
                }
            }
            yield return null;
        }
    }

    float EvaluateEaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }

    public bool IsMenuOpen() => isOpen;
}