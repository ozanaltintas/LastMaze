using UnityEngine;

[ExecuteInEditMode]
public class UIForwardRenderer : MonoBehaviour
{
    public int orderInLayer = 100; // Canvas'ının order'ından yüksek olmalı
    public string sortingLayerName = "UI";

    void Start()
    {
        ApplyLayer();
    }

    void OnValidate() // Inspector'dan değiştirdiğinde anında yansır
    {
        ApplyLayer();
    }

    void ApplyLayer()
    {
        var renderer = GetComponent<ParticleSystemRenderer>();
        if (renderer != null)
        {
            renderer.sortingLayerName = sortingLayerName;
            renderer.sortingOrder = orderInLayer;
        }
    }
}
