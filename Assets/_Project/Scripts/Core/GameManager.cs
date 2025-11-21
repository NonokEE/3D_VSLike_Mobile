using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace VSLike.Core
{
    /// <summary>
    /// Central game state manager (Singleton)<br/>
    /// Manages game lifecycle, state transitions, and coordinates other managers<br/>
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.Initializing;

        [Header("Statistics")]
        [SerializeField] private int killCount = 0;
        [SerializeField] private float survivalTime = 0f;
        [SerializeField] private int playerLevel = 1;

        // Events
        public event Action<GameState> OnGameStateChanged;
        public event Action OnGameStart;
        public event Action OnGamePause;
        public event Action OnGameResume;
        public event Action<bool> OnGameOver; // bool isVictory
        public event Action OnPlayerDeath;

        // Properties
        public GameState CurrentState => currentState;
        public int KillCount => killCount;
        public float SurvivalTime => survivalTime;
        public int PlayerLevel => playerLevel;
        public bool IsPlaying => currentState == GameState.Playing;
        public bool IsPaused => currentState == GameState.Paused;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // Auto-initialize on first frame
            if (currentState == GameState.Initializing)
            {
                InitializeGame();
            }
        }

        /// <summary>
        /// Initialize game systems<br/>
        /// Call this before starting gameplay<br/>
        /// </summary>
        public void InitializeGame()
        {
            Debug.Log("[GameManager] Initializing game systems...");

            // Reset statistics
            killCount = 0;
            survivalTime = 0f;
            playerLevel = 1;

            // Initialize managers (will be implemented in later phases)
            // PoolManager already initialized via Singleton
            // TimeManager.Instance.Initialize();
            // SpawnManager.Instance.Initialize();
            // UIManager.Instance.Initialize();

            ChangeState(GameState.Playing);
            Debug.Log("[GameManager] Game initialized");
        }

        /// <summary>
        /// Start gameplay
        /// </summary>
        public void StartGame()
        {
            if (currentState != GameState.Playing)
            {
                ChangeState(GameState.Playing);
                OnGameStart?.Invoke();
                Debug.Log("[GameManager] Game started");
            }
        }

        /// <summary>
        /// Pause game (e.g., level-up UI)
        /// </summary>
        public void PauseGame()
        {
            if (currentState == GameState.Playing)
            {
                ChangeState(GameState.Paused);
                Time.timeScale = 0f;
                OnGamePause?.Invoke();
                Debug.Log("[GameManager] Game paused");
            }
        }

        /// <summary>
        /// Resume game from pause
        /// </summary>
        public void ResumeGame()
        {
            if (currentState == GameState.Paused)
            {
                ChangeState(GameState.Playing);
                Time.timeScale = 1f;
                OnGameResume?.Invoke();
                Debug.Log("[GameManager] Game resumed");
            }
        }

        /// <summary>
        /// End game with victory<br/>
        /// Called when player survives 10 minutes<br/>
        /// </summary>
        public void Victory()
        {
            if (currentState == GameState.Playing || currentState == GameState.Paused)
            {
                ChangeState(GameState.Victory);
                Time.timeScale = 0f;
                OnGameOver?.Invoke(true);
                Debug.Log("[GameManager] Victory!");
            }
        }

        /// <summary>
        /// End game with defeat<br/>
        /// Called when player HP reaches 0<br/>
        /// </summary>
        public void Defeat()
        {
            if (currentState == GameState.Playing || currentState == GameState.Paused)
            {
                ChangeState(GameState.Defeat);
                Time.timeScale = 0f;
                OnPlayerDeath?.Invoke();
                OnGameOver?.Invoke(false);
                Debug.Log("[GameManager] Defeat!");
            }
        }

        /// <summary>
        /// Restart current game
        /// </summary>
        public void RestartGame()
        {
            Debug.Log("[GameManager] Restarting game...");

            Time.timeScale = 1f;

            // Return all pooled objects
            PoolManager.Instance.ClearAllPools();

            // Reload scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        /// Return to main menu
        /// </summary>
        public void ReturnToMainMenu()
        {
            Debug.Log("[GameManager] Returning to main menu...");

            Time.timeScale = 1f;

            // Clear pools
            PoolManager.Instance.ClearAllPools();

            // Load main menu scene (index 0)
            SceneManager.LoadScene(0);
        }

        /// <summary>
        /// Increment kill count<br/>
        /// Called by enemies on death<br/>
        /// </summary>
        public void AddKill()
        {
            killCount++;
        }

        /// <summary>
        /// Update player level<br/>
        /// Called by ExperienceManager on level up<br/>
        /// </summary>
        public void SetPlayerLevel(int level)
        {
            playerLevel = level;
        }

        /// <summary>
        /// Update survival time<br/>
        /// Called by TimeManager each frame<br/>
        /// </summary>
        public void UpdateSurvivalTime(float time)
        {
            survivalTime = time;
        }

        /// <summary>
        /// Change game state and trigger events<br/>
        /// </summary>
        private void ChangeState(GameState newState)
        {
            if (currentState == newState)
                return;

            GameState previousState = currentState;
            currentState = newState;

            OnGameStateChanged?.Invoke(newState);

            Debug.Log($"[GameManager] State changed: {previousState} → {newState}");
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

#if UNITY_EDITOR
        // Debug info in inspector
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                name = $"GameManager [{currentState}]";
            }
        }
#endif
    }
    
    /// <summary>
    /// Game state enumeration
    /// </summary>
    /// <remarks>
    /// Initializing: Game systems are being set up<br/>
    /// Playing: Active gameplay<br/>
    /// Paused: Gameplay is paused (e.g., level-up UI)<br/>
    /// Victory: Player has won the game<br/>
    /// Defeat: Player has lost the game<br/>
    /// </remarks>
    public enum GameState
    {
        Initializing,
        Playing,
        Paused,
        Victory,
        Defeat
    }
}
