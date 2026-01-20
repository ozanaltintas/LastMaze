using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    // Diğer scriptlerin (WinZone) buna ulaşması için Singleton yapısı
    public static GameUIManager Instance;

    [Header("UI Panelleri")]
    [Tooltip("Kazandığında açılacak olan Panel objesi")]
    public GameObject winScreenPanel;

    private void Awake()
    {
        // Singleton Kurulumu
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Bu fonksiyon çağrıldığında Bravo ekranını açar
    public void ShowWinScreen()
    {
        if (winScreenPanel != null)
        {
            winScreenPanel.SetActive(true);
            
            // İstersen burada oyunu durdurabilirsin (Time.timeScale = 0)
            // Time.timeScale = 0f; 
        }
    }
}