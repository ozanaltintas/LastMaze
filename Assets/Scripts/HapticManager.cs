using UnityEngine;

public static class HapticManager
{
    public static void LightFeedback()
    {
        // Önce SettingsManager'dan titreşim açık mı kontrol et (Varsa)
        if (PlayerPrefs.GetInt("VibrationSetting", 1) == 0) return;

        #if UNITY_ANDROID && !UNITY_EDITOR
            // Android için "Light" titreşim (Kısa pıt)
            AndroidVibrate(50); // 50 milisaniye
        #elif UNITY_IOS && !UNITY_EDITOR
            // iOS için Taptic Engine (Light Impact)
            UnityEngine.iOS.Device.Feedback.Haptic(UnityEngine.iOS.HapticFeedback.Light);
        #else
            // Editörde test amaçlı log
            // Debug.Log("Haptic: Pıt!");
        #endif
    }

    #if UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaObject vibrator;
    private static void AndroidVibrate(long milliseconds)
    {
        if (vibrator == null)
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
            }
        }
        
        if (vibrator != null)
        {
            // Vibrate(long milliseconds)
            vibrator.Call("vibrate", milliseconds);
        }
    }
    #endif
}