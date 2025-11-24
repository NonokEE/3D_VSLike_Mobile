using UnityEngine;
using System;
using VSLike.Core;

namespace VSLike.Player
{
    /// <summary>
    /// Player movement and health controller<br/>
    /// Handles player input, movement, HP system, and death<br/>
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        
        [Header("Health")]
        [SerializeField] private float maxHP = 100f;
        [SerializeField] private float currentHP = 100f;
        
        [Header("References")]
        [SerializeField] private DynamicJoystick joystick;
        
        // Components
        private Rigidbody rb;
        
        // State
        private bool isAlive = true;
        private Vector2 moveInput;
        
        // Events
        public event Action<float, float> OnHPChanged; // currentHP, maxHP
        public event Action OnDeath;
        
        // Properties
        public float CurrentHP => currentHP;
        public float MaxHP => maxHP;
        public bool IsAlive => isAlive;
        public Vector3 Position => transform.position;

        private void Awake()
        {
            // Get components
            rb = GetComponent<Rigidbody>();
            
            // Configure Rigidbody for mobile performance
            rb.linearDamping = 0f;
            rb.angularDamping = 0f;
            rb.useGravity = false; // Top-down view, no gravity needed
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            rb.interpolation = RigidbodyInterpolation.None; // Mobile optimization
            
            // Find joystick if not assigned
            if (joystick == null)
            {
                joystick = FindFirstObjectByType<DynamicJoystick>();
                if (joystick == null)
                {
                    Debug.LogError("[PlayerController] DynamicJoystick not found in scene!");
                }
            }
        }

        private void Start()
        {
            // Initialize HP
            currentHP = maxHP;
            OnHPChanged?.Invoke(currentHP, maxHP);
            
            Debug.Log("[PlayerController] Initialized");
        }

        private void Update()
        {
            if (!isAlive || !GameManager.Instance.IsPlaying)
                return;
            
            // Get input from joystick
            if (joystick != null)
            {
                moveInput = joystick.GetInputDirection();
            }
        }

        private void FixedUpdate()
        {
            if (!isAlive || !GameManager.Instance.IsPlaying)
            {
                rb.linearVelocity = Vector3.zero;
                return;
            }
            
            // Apply movement
            MovePlayer();
        }

        /// <summary>
        /// Apply movement to Rigidbody<br/>
        /// Uses linearVelocity for consistent physics-based movement<br/>
        /// </summary>
        private void MovePlayer()
        {
            if (moveInput.sqrMagnitude > 0.01f)
            {
                // Convert 2D input to 3D movement (XZ plane)
                Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y);
                movement.Normalize(); // Ensure consistent speed in all directions
                
                // Apply velocity
                rb.linearVelocity = movement * moveSpeed;
            }
            else
            {
                // Stop when no input
                rb.linearVelocity = Vector3.zero;
            }
        }

        /// <summary>
        /// Take damage from enemies or projectiles
        /// </summary>
        public void TakeDamage(float damage)
        {
            if (!isAlive)
                return;
            
            currentHP -= damage;
            currentHP = Mathf.Max(currentHP, 0f);
            
            OnHPChanged?.Invoke(currentHP, maxHP);
            
            Debug.Log($"[PlayerController] Took {damage} damage. HP: {currentHP}/{maxHP}");
            
            // Check for death
            if (currentHP <= 0f)
            {
                Die();
            }
        }

        /// <summary>
        /// Heal player (for future power-ups)
        /// </summary>
        public void Heal(float amount)
        {
            if (!isAlive)
                return;
            
            currentHP += amount;
            currentHP = Mathf.Min(currentHP, maxHP);
            
            OnHPChanged?.Invoke(currentHP, maxHP);
            
            Debug.Log($"[PlayerController] Healed {amount}. HP: {currentHP}/{maxHP}");
        }

        /// <summary>
        /// Handle player death
        /// </summary>
        private void Die()
        {
            if (!isAlive)
                return;
            
            isAlive = false;
            rb.linearVelocity = Vector3.zero;
            
            OnDeath?.Invoke();
            GameManager.Instance.Defeat();
            
            Debug.Log("[PlayerController] Player died");
            
            // Visual feedback (optional)
            // gameObject.SetActive(false);
        }

        /// <summary>
        /// Reset player state (for game restart)
        /// </summary>
        public void ResetPlayer()
        {
            currentHP = maxHP;
            isAlive = true;
            rb.linearVelocity = Vector3.zero;
            transform.position = Vector3.zero;
            
            OnHPChanged?.Invoke(currentHP, maxHP);
            
            Debug.Log("[PlayerController] Player reset");
        }

        private void OnTriggerEnter(Collider other)
        {
            // Enemy collision detection (implemented in Phase 4)
            // Enemies will call TakeDamage() directly
        }

#if UNITY_EDITOR
        // Debug visualization
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;
            
            // Draw movement direction
            if (moveInput.sqrMagnitude > 0.01f)
            {
                Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, transform.position + direction * 2f);
            }
        }
#endif
    }
}
