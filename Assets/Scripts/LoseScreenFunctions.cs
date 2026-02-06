using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoseScreenFunctions : MonoBehaviour
{
    // RESTART TUŞUNA BAĞLA
    public void RestartLevel()
    {
        // Donmuş zamanı çöz
        Time.timeScale = 1f;
        
        // Bölümü yeniden yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // MAIN MENU TUŞUNA BAĞLA
    public void GoToMainMenu()
    {
        // Donmuş zamanı çöz
        Time.timeScale = 1f;

        // Ana menüye (Index 0) dön
        SceneManager.LoadScene(0);
    }
}