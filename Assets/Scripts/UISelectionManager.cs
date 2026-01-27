using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISelectionManager : MonoBehaviour
{
    [SerializeField] private RectTransform haloEffect;
    [SerializeField] private MenuController menuController;
    [SerializeField] private float smoothSpeed = 20f;
    [SerializeField] private Vector2 padding = new Vector2(20, 20);

    void Update()
    {
        // Menü kapalıysa halo görünmez
        if (menuController != null && !menuController.IsMenuOpen())
        {
            haloEffect.gameObject.SetActive(false);
            return;
        }

        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected != null && currentSelected.GetComponent<Selectable>())
        {
            RectTransform targetRect = currentSelected.GetComponent<RectTransform>();
            haloEffect.gameObject.SetActive(true);
            
            // Yumuşak Hareket ve Boyutlanma
            haloEffect.position = Vector3.Lerp(haloEffect.position, targetRect.position, Time.deltaTime * smoothSpeed);
            
            Vector2 targetSize = targetRect.sizeDelta + padding;
            haloEffect.sizeDelta = Vector2.Lerp(haloEffect.sizeDelta, targetSize, Time.deltaTime * smoothSpeed);
        }
        else
        {
            haloEffect.gameObject.SetActive(false);
        }
    }
}