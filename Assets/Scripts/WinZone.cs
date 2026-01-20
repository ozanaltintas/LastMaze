using UnityEngine;

public class WinZone : MonoBehaviour
{
    [Header("Ayarlar")]
    [Tooltip("Çarpan objenin etiketi ne olmalı? (Genelde 'Player')")]
    public string targetTag = "Player";

    // Bir obje bu alanın İÇİNE girdiğinde çalışır
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Giren şey bizim Topumuz mu?
        if (other.CompareTag(targetTag))
        {
            Debug.Log("Oyun Kazanıldı! - BRAVO");
            
            // UI Manager'a haber ver
            if (GameUIManager.Instance != null)
            {
                GameUIManager.Instance.ShowWinScreen();
            }
        }
    }
}