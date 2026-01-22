using UnityEngine;
using UnityEngine.SceneManagement; // Sahne işlemleri gerekirse diye ekli
using System.Collections;

public class WinZone : MonoBehaviour
{
    [Header("UI Ayarları")]
    public GameObject winPanel; // Inspector'dan Win Panelini buraya sürükle

    // 2D oyun olduğu için OnTriggerEnter2D kullanıyoruz
    void OnTriggerEnter2D(Collider2D other)
    {
        // Çarpan nesne Player etiketine sahip mi?
        if (other.CompareTag("Player"))
        {
            Debug.Log("Oyun Kazanıldı! - BRAVO");

            // 1. ÖNCE SAYACI BUL VE DURDUR
            GameTimer timerScript = FindObjectOfType<GameTimer>();
            if (timerScript != null)
            {
                timerScript.Durdur(); // Sayacı durdur ve gizle
            }
            else
            {
                Debug.LogWarning("Sahnede GameTimer bulunamadı!");
            }

            // 2. SONRA WIN EKRANINI AÇ
            if (winPanel != null)
            {
                winPanel.SetActive(true);
            }
            else
            {
                Debug.LogWarning("WinZone scriptine Win Panel atanmamış!");
            }
        }
    }
}