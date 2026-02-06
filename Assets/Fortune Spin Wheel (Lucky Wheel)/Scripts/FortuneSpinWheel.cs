using System.Collections;
using UnityEngine;
using UnityEngine.UI; 
using TMPro; 
using DG.Tweening; 

namespace FortuneSpinWheel
{
    public class FortuneSpinWheel : MonoBehaviour
    {
        [Header("--- SORUN ÇÖZÜCÜ ---")]
        [Tooltip("Bunu açarsan, Play modundayken çarkı elinle çevirdiğinde Konsola anlık ödülü yazar.")]
        public bool debugMode = false;

        [Tooltip("Eğer ödüller rastgele/karışık geliyorsa bu tiki değiştir (Aç/Kapa).")]
        public bool reverseDataOrder = true; // YÖN SORUNUNU BU ÇÖZER

        [Tooltip("Sıralama doğru ama hep 1-2 dilim yanına düşüyorsa bunu değiştir.")]
        public int indexOffset = 0; 

        [Header("UI Ana Bağlantılar")]
        public GameObject wheelVisuals;  
        public Button riskButton;        
        public Button spinButton;
        public TextMeshProUGUI riskCostText; 

        [Header("Sonuç Paneli")]
        public GameObject rewardPanel;          
        public Image rewardPanelImage;          
        public Text rewardPanelText; 

        [Header("Ses Efektleri")]
        public AudioSource audioSource;         
        public AudioClip spinSound;             
        public AudioClip rewardPopSound;
        
        [Header("Ses Pitch Ayarları")]
        [Range(0.1f, 3f)] public float minPitch = 0.5f; 
        [Range(0.1f, 3f)] public float maxPitch = 1.2f; 

        [Header("Dönüş Süresi ve Hız Ayarları")]
        public float minStartSpeed = 15f; 
        public float maxStartSpeed = 30f;
        [Range(0.1f, 2.0f)] public float stoppingPower = 0.5f; 

        [Header("Ödül Verileri")]
        public RewardData[] m_RewardData; 
        public Image m_CircleBase;       
        public Image[] m_RewardPictures; 
        public Text[] m_RewardCounts; 

        [Header("Ayarlar")]
        public int spinCost = 10;
        
        // İç Değişkenler
        private bool m_IsSpinning = false;
        private float m_SpinSpeed = 0;
        private float m_MaxSpinSpeed = 0; 
        private float m_Rotation = 0;
        private int m_RewardNumber = -1;
        private bool m_HasUsedRisk = false; 

        void Start()
        {
            m_HasUsedRisk = false;
            
            // Görselleri yerleştir
            SetupWheelVisuals();
            
            if (wheelVisuals != null) wheelVisuals.SetActive(false);
            if (rewardPanel != null) rewardPanel.SetActive(false);

            if (riskButton != null)
            {
                riskButton.onClick.RemoveAllListeners();
                riskButton.onClick.AddListener(RiskButonunaBasildi);
            }

            if (spinButton != null)
            {
                spinButton.onClick.RemoveAllListeners();
                spinButton.onClick.AddListener(StartSpin);
            }

            UpdateRiskButtonState();
        }

        void OnEnable()
        {
            UpdateRiskButtonState();
        }

        void SetupWheelVisuals()
        {
            for (int i = 0; i < m_RewardData.Length; i++)
            {
                if (i < m_RewardPictures.Length && m_RewardPictures[i] != null)
                    m_RewardPictures[i].sprite = m_RewardData[i].m_Icon;

                if (i < m_RewardCounts.Length && m_RewardCounts[i] != null && m_RewardData[i].m_Count > 0)
                    m_RewardCounts[i].text = "+" + m_RewardData[i].m_Count;
            }
        }

        void UpdateRiskButtonState()
        {
            if (riskButton == null) return;
            if (m_HasUsedRisk)
            {
                riskButton.gameObject.SetActive(false);
                return;
            }
            riskButton.gameObject.SetActive(true);
            int bakiye = PlayerPrefs.GetInt("TimeBank", 0);
            riskButton.interactable = (bakiye >= spinCost);
            if (riskCostText) riskCostText.text = "RISK (" + spinCost + " Seconds)";
        }

        void RiskButonunaBasildi()
        {
            if (m_HasUsedRisk) return;
            int mevcutBakiye = PlayerPrefs.GetInt("TimeBank", 0);

            if (mevcutBakiye >= spinCost)
            {
                mevcutBakiye -= spinCost;
                PlayerPrefs.SetInt("TimeBank", mevcutBakiye);
                PlayerPrefs.Save();
                if (TimeBankManager.Instance != null) TimeBankManager.Instance.SayaciGuncelle();
                riskButton.gameObject.SetActive(false);

                if (wheelVisuals != null)
                {
                    wheelVisuals.SetActive(true);
                    wheelVisuals.transform.localScale = Vector3.zero;
                    wheelVisuals.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(true); 
                }
                if (spinButton != null) spinButton.interactable = true;
            }
        }

        public void StartSpin()
        {
            if (!m_IsSpinning)
            {
                if (spinButton != null) spinButton.interactable = false;
                m_SpinSpeed = Random.Range(minStartSpeed, maxStartSpeed);
                m_MaxSpinSpeed = m_SpinSpeed; 
                m_IsSpinning = true;
                m_RewardNumber = -1;
                
                if (audioSource != null && spinSound != null)
                {
                    audioSource.clip = spinSound;
                    audioSource.loop = true; 
                    audioSource.pitch = maxPitch; 
                    audioSource.Play();
                }
            }
        }

        void Update()
        {
            // --- CANLI DEBUG MODU ---
            // Play modundayken çarkı elinle döndürdüğünde çalışır
            if (debugMode && m_CircleBase != null)
            {
                // Mevcut açıyı al (Kodun kullandığı değişken değil, objenin gerçek açısı)
                float currentDebugRot = m_CircleBase.transform.localEulerAngles.z;
                int debugIndex = CalculateIndexFromAngle(currentDebugRot);
                
                // Konsola yaz (Sadece değiştiğinde yazsın diye basit bir check yapabilirsin ama şimdilik sürekli yazsın)
                RewardData debugReward = m_RewardData[debugIndex];
                Debug.Log($"<color=yellow>CANLI TEST:</color> Açı: {currentDebugRot:F0} | Index: {debugIndex} | <color=green>ÖDÜL: {debugReward.m_Count}</color>");
            }
            // ------------------------

            if (m_HasUsedRisk && riskButton != null && riskButton.gameObject.activeSelf)
            {
                riskButton.gameObject.SetActive(false);
            }

            if (m_IsSpinning)
            {
                m_Rotation += m_SpinSpeed * Time.unscaledDeltaTime * 50; 
                m_CircleBase.transform.rotation = Quaternion.Euler(0, 0, m_Rotation);

                m_SpinSpeed -= m_SpinSpeed * Time.unscaledDeltaTime * stoppingPower; 

                if (audioSource != null && audioSource.isPlaying)
                {
                    float hizOrani = m_SpinSpeed / m_MaxSpinSpeed;
                    audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, hizOrani);
                }

                if (m_SpinSpeed < 0.5f)
                {
                    m_SpinSpeed = 0;
                    m_IsSpinning = false;
                    
                    if (audioSource != null)
                    {
                        audioSource.Stop();
                        audioSource.pitch = 1f; 
                        audioSource.loop = false;
                    }
                    
                    EvaluateReward();
                }
            }
        }

        // Hesabı tek bir fonksiyona aldım ki hem Debug'da hem gerçekte aynı matematik çalışsın
        int CalculateIndexFromAngle(float angle)
        {
            angle = angle % 360;
            if (angle < 0) angle += 360;

            float sliceAngle = 360f / m_RewardData.Length;
            int index = Mathf.FloorToInt(angle / sliceAngle);

            int finalIndex;

            if (reverseDataOrder)
            {
                // Yön ters ise (Saat yönü vs Saat yönü tersi uyuşmazlığında burası çalışmalı)
                finalIndex = ((m_RewardData.Length - 1) - index + indexOffset) % m_RewardData.Length;
            }
            else
            {
                // Yön düz ise
                finalIndex = (index + indexOffset) % m_RewardData.Length;
            }

            if (finalIndex < 0) finalIndex += m_RewardData.Length;

            return finalIndex;
        }

        void EvaluateReward()
        {
            // m_Rotation değişkeni sürekli artıyor, modülo ile 0-360 arasına çekiyoruz
            m_RewardNumber = CalculateIndexFromAngle(m_Rotation);
            Debug.Log($"SONUÇ: Hesaplanan Index: {m_RewardNumber} | Ödül: {m_RewardData[m_RewardNumber].m_Count}");
            StartCoroutine(ApplyReward());
        }

        IEnumerator ApplyReward()
        {
            RewardData reward = m_RewardData[m_RewardNumber];
            
            yield return new WaitForSecondsRealtime(0.2f);

            if (rewardPanel != null)
            {
                if (rewardPanelImage != null) rewardPanelImage.sprite = reward.m_Icon;
                if (rewardPanelText != null) rewardPanelText.text = "+" + reward.m_Count.ToString();

                rewardPanel.SetActive(true);
                rewardPanel.transform.localScale = Vector3.zero;

                if (audioSource != null && rewardPopSound != null) audioSource.PlayOneShot(rewardPopSound);

                rewardPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutElastic).SetUpdate(true);
                
                yield return new WaitForSecondsRealtime(2f);

                rewardPanel.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).SetUpdate(true);
                yield return new WaitForSecondsRealtime(0.3f);
                rewardPanel.SetActive(false);
            }

            if (wheelVisuals != null)
            {
                wheelVisuals.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).SetUpdate(true);
                yield return new WaitForSecondsRealtime(0.3f); 
                wheelVisuals.SetActive(false);
            }

            m_HasUsedRisk = true;
            
            GameTimer timer = FindObjectOfType<GameTimer>();
            if (timer != null)
            {
                timer.ZamanEkle(reward.m_Count);
                timer.OyunuDevamEttir();
            }
        }
    }
}