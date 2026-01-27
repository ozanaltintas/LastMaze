using UnityEngine;
using TMPro;
using System.Collections;

public class MainMenuLevelCounter : MonoBehaviour
{
    [Header("UI Bağlantıları")]
    public RectTransform container;   
    public TextMeshProUGUI oldText;   
    public TextMeshProUGUI newText;   
    public float slideDuration = 0.8f; 

    [Header("Yazı Ayarı")]
    public string onEk = "LEVEL "; // Buraya "BÖLÜM " veya "STAGE " de yazabilirsin

    [Header("Ses")]
    public AudioSource audioSource;
    public AudioClip flipSound; 

    private float textHeight; 

    void Start()
    {
        int realLevel = PlayerPrefs.GetInt("ReachedLevel", 1);
        int displayLevel = PlayerPrefs.GetInt("MenuDisplayLevel", 1);

        textHeight = container.rect.height;

        if (realLevel > displayLevel)
        {
            // DEĞİŞİKLİK BURADA: Başına onEk ekliyoruz
            oldText.text = onEk + displayLevel.ToString();
            newText.text = onEk + realLevel.ToString();

            StartCoroutine(AnimateLevelChange(realLevel));
        }
        else
        {
            // DEĞİŞİKLİK BURADA
            oldText.text = onEk + realLevel.ToString();
            newText.text = ""; 
            container.anchoredPosition = Vector2.zero; 
        }
    }

    IEnumerator AnimateLevelChange(int targetLevel)
    {
        yield return new WaitForSeconds(0.5f);

        if (audioSource != null && flipSound != null) audioSource.PlayOneShot(flipSound);

        float timer = 0f;
        Vector2 startPos = Vector2.zero;
        Vector2 targetPos = new Vector2(0, -textHeight); 

        while (timer < slideDuration)
        {
            timer += Time.deltaTime;
            float t = timer / slideDuration;
            t = t * t * (3f - 2f * t); 

            container.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        container.anchoredPosition = Vector2.zero; 
        
        // DEĞİŞİKLİK BURADA
        oldText.text = onEk + targetLevel.ToString();     
        newText.text = ""; 

        PlayerPrefs.SetInt("MenuDisplayLevel", targetLevel);
        PlayerPrefs.Save();
    }
    
    // Test Butonları (Sağ Tık menüsü için)
    [ContextMenu("Test - Level Sıfırla")]
    public void ResetProgress()
    {
        PlayerPrefs.SetInt("ReachedLevel", 1);
        PlayerPrefs.SetInt("MenuDisplayLevel", 1);
        Debug.Log("Sıfırlandı.");
    }
    
    [ContextMenu("Test - Level Atlat")]
    public void CheatLevelUp()
    {
        int current = PlayerPrefs.GetInt("ReachedLevel", 1);
        PlayerPrefs.SetInt("ReachedLevel", current + 1);
        Debug.Log("Level atlandı! Tekrar oyna.");
    }
}