using UnityEngine;
using System.Collections;

public class RestartButtonController : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float rotationDuration = 0.4f; // 0.6'dan 0.4'e düşürüldü
    [SerializeField] private AnimationCurve smoothCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private RectTransform rectTransform;
    private bool isRotating = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void TriggerRestart()
    {
        if (isRotating) return;

        StartCoroutine(AnimateRestart());

        GameTimer timer = FindObjectOfType<GameTimer>();
        if (timer != null)
        {
            timer.BolumuYenidenBaslat();
        }
    }

    IEnumerator AnimateRestart()
    {
        isRotating = true;
        float elapsedTime = 0;
        Quaternion startRot = rectTransform.localRotation;
        float targetAngle = 360f;

        while (elapsedTime < rotationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / rotationDuration;
            float curveValue = smoothCurve.Evaluate(t);

            rectTransform.localRotation = startRot * Quaternion.Euler(0, 0, curveValue * targetAngle);
            yield return null;
        }

        rectTransform.localRotation = startRot * Quaternion.Euler(0, 0, targetAngle);
        isRotating = false;
    }
}