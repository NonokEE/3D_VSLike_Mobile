using UnityEngine;
using System;
using System.Collections;

namespace VSLike.Core
{
    /// <summary>
    /// Manages game time and wave progression (Singleton)<br/>
    /// Handles 10-minute countdown and wave transitions<br/>
    /// </summary>
    public class TimeManager : MonoBehaviour
    {
        private static TimeManager _instance;
        public static TimeManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("TimeManager");
                    _instance = go.AddComponent<TimeManager>();
                }
                return _instance;
            }
        }

        [Header("Time Configuration")]
        [SerializeField] private float totalGameTime = 600f; // 10 minutes
        [SerializeField] private float currentTime = 0f;
        [SerializeField] private bool isRunning = false;

        [Header("Wave Configuration")]
        [SerializeField] private int currentWave = 1;
        [SerializeField] private WaveConfig[] waveConfigs;

        // Events
        public event Action<int> OnWaveChanged; // int: new wave number
        public event Action<float> OnTimeUpdate; // float: remaining time
        public event Action OnTimeExpired; // Game complete (victory)

        // Properties
        public float CurrentTime => currentTime;
        public float RemainingTime => Mathf.Max(0f, totalGameTime - currentTime);
        public int CurrentWave => currentWave;
        public WaveConfig CurrentWaveConfig => GetWaveConfig(currentWave);
        public bool IsRunning => isRunning;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            // Don't destroy if not child of GameManager
            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }

            InitializeWaveConfigs();
        }

        /// <summary>
        /// Initialize wave configurations based on design document<br/>
        /// Wave 1: 0~120s (Charger only)<br/>
        /// Wave 2: 120~300s (Charger 70%, Chaser 30%)<br/>
        /// Wave 3: 300~480s (Charger 50%, Chaser 30%, Kiter 20%)<br/>
        /// Wave 4: 480~600s (Balanced)<br/>
        /// </summary>
        private void InitializeWaveConfigs()
        {
            waveConfigs = new WaveConfig[]
            {
                new WaveConfig(1, 0f, 120f, 1.0f, 0f, 0f),
                new WaveConfig(2, 120f, 300f, 0.7f, 0.3f, 0f),
                new WaveConfig(3, 300f, 480f, 0.5f, 0.3f, 0.2f),
                new WaveConfig(4, 480f, 600f, 0.33f, 0.33f, 0.34f)
            };

            Debug.Log("[TimeManager] Wave configurations initialized");
        }

        /// <summary>
        /// Start the game timer
        /// </summary>
        public void StartTimer()
        {
            if (!isRunning)
            {
                isRunning = true;
                currentTime = 0f;
                currentWave = 1;

                StartCoroutine(TimerCoroutine());

                OnWaveChanged?.Invoke(currentWave);
                Debug.Log("[TimeManager] Timer started");
            }
        }

        /// <summary>
        /// Stop the game timer
        /// </summary>
        public void StopTimer()
        {
            isRunning = false;
            StopAllCoroutines();
            Debug.Log("[TimeManager] Timer stopped");
        }

        /// <summary>
        /// Reset the timer
        /// </summary>
        public void ResetTimer()
        {
            StopTimer();
            currentTime = 0f;
            currentWave = 1;
            Debug.Log("[TimeManager] Timer reset");
        }

        /// <summary>
        /// Main timer coroutine<br/>
        /// Updates every second to reduce overhead<br/>
        /// </summary>
        private IEnumerator TimerCoroutine()
        {
            while (isRunning && currentTime < totalGameTime)
            {
                // Update only when game is playing
                if (GameManager.Instance.IsPlaying)
                {
                    currentTime += 1f;

                    // Update GameManager statistics
                    GameManager.Instance.UpdateSurvivalTime(currentTime);

                    // Check for wave transitions
                    CheckWaveTransition();

                    // Notify listeners (UI)
                    OnTimeUpdate?.Invoke(RemainingTime);
                }

                // Update every second (reduces overhead vs. Update())
                yield return new WaitForSeconds(1f);
            }

            // Time expired - Victory!
            if (currentTime >= totalGameTime)
            {
                OnTimeExpired?.Invoke();
                GameManager.Instance.Victory();
                Debug.Log("[TimeManager] Time expired - Victory!");
            }
        }

        /// <summary>
        /// Check if wave should transition
        /// </summary>
        private void CheckWaveTransition()
        {
            int newWave = GetCurrentWaveNumber(currentTime);

            if (newWave != currentWave)
            {
                currentWave = newWave;
                OnWaveChanged?.Invoke(currentWave);
                Debug.Log($"[TimeManager] Wave changed to {currentWave}");
            }
        }

        /// <summary>
        /// Get current wave number based on elapsed time
        /// </summary>
        private int GetCurrentWaveNumber(float time)
        {
            for (int i = waveConfigs.Length - 1; i >= 0; i--)
            {
                if (time >= waveConfigs[i].startTime)
                {
                    return waveConfigs[i].waveNumber;
                }
            }
            return 1;
        }

        /// <summary>
        /// Get wave configuration by wave number
        /// </summary>
        public WaveConfig GetWaveConfig(int waveNumber)
        {
            foreach (WaveConfig config in waveConfigs)
            {
                if (config.waveNumber == waveNumber)
                    return config;
            }
            return waveConfigs[0];
        }

        /// <summary>
        /// Get formatted time string (MM:SS)
        /// </summary>
        public string GetFormattedTime()
        {
            float remaining = RemainingTime;
            int minutes = Mathf.FloorToInt(remaining / 60f);
            int seconds = Mathf.FloorToInt(remaining % 60f);
            return $"{minutes:00}:{seconds:00}";
        }

        /// <summary>
        /// Get formatted elapsed time string (MM:SS)
        /// </summary>
        public string GetFormattedElapsedTime()
        {
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            return $"{minutes:00}:{seconds:00}";
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                StopTimer();
                _instance = null;
            }
        }

#if UNITY_EDITOR
        // Debug info in inspector
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                name = $"TimeManager [Wave {currentWave}] {GetFormattedTime()}";
            }
        }
#endif
    }
    
    /// <summary>
    /// Wave configuration data
    /// </summary>
    [System.Serializable]
    public class WaveConfig
    {
        public int waveNumber;
        public float startTime;  // In seconds
        public float endTime;    // In seconds
        
        [Header("Enemy Distribution")]
        [Range(0f, 1f)] public float chargerPercentage;
        [Range(0f, 1f)] public float chaserPercentage;
        [Range(0f, 1f)] public float kiterPercentage;
        
        public WaveConfig(int wave, float start, float end, float charger, float chaser, float kiter)
        {
            waveNumber = wave;
            startTime = start;
            endTime = end;
            chargerPercentage = charger;
            chaserPercentage = chaser;
            kiterPercentage = kiter;
        }
    }
}
