using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThemeManager : MonoBehaviour
{
    [Header("Temel Ayarlar")]
    public Camera mainCamera;       
    public Material globalMaterial; 

    [Header("UI Elemanları")]
    public TextMeshProUGUI[] textObjects; 
    public Image[] iconObjects;           

    private bool isDarkMode = false;

    void Start()
    {
        isDarkMode = PlayerPrefs.GetInt("ThemeSetting", 0) == 1;
        ApplyTheme();
    }

    public void ToggleTheme()
    {
        isDarkMode = !isDarkMode;
        PlayerPrefs.SetInt("ThemeSetting", isDarkMode ? 1 : 0);
        PlayerPrefs.Save();
        ApplyTheme();
    }

    void ApplyTheme()
    {
        if (isDarkMode)
        {
            if(mainCamera != null) mainCamera.backgroundColor = Color.black;
            if(globalMaterial != null) globalMaterial.color = Color.white;
            ChangeUIColors(Color.white);
        }
        else
        {
            if(mainCamera != null) mainCamera.backgroundColor = Color.white;
            if(globalMaterial != null) globalMaterial.color = Color.black;
            ChangeUIColors(Color.black);
        }
    }

    void ChangeUIColors(Color targetColor)
    {
        foreach (TextMeshProUGUI txt in textObjects)
        {
            if(txt != null) txt.color = targetColor;
        }
        foreach (Image img in iconObjects)
        {
            if(img != null) img.color = targetColor;
        }
    }
}