using UnityEngine;
using TMPro; // TextMeshPro kütüphanesi
using UnityEngine.SceneManagement; // Sahne bilgisini okumak için

public class LevelDisplay : MonoBehaviour
{
    [Header("Ayarlar")]
    public TextMeshProUGUI levelText;  // Text bileşeni
    public string onEk = "LEVEL ";     // Sayının önüne ne yazsın?
    
    [Tooltip("Eğer Build Settings'de Main Menu 0. sıradaysa ve Level 1 1. sıradaysa burayı 0 yap. Eğer Level 1 2. sıradaysa burayı -1 yap.")]
    public int levelOffset = 0; 

    void Start()
    {
        // 1. Text bileşenini bul (Eğer atamayı unuttuysan)
        if (levelText == null)
            levelText = GetComponent<TextMeshProUGUI>();

        // 2. Şu anki Sahne Numarasını (Build Index) al
        int sahneNo = SceneManager.GetActiveScene().buildIndex;

        // 3. Offset hesaplaması (Menü vs. varsa ayarlamak için)
        int gosterilecekLevel = sahneNo + levelOffset;

        // 4. Ekrana Yazdır
        if (levelText != null)
        {
            levelText.text = onEk + gosterilecekLevel.ToString();
        }
    }
}