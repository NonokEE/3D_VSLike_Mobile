using UnityEngine;
using UnityEngine.InputSystem;
using VSLike.Core;

namespace VSLike.Player
{
    /// <summary>
    /// Player movement and health management<br/>
    /// Handles joystick input and collision with enemies<br/>
    /// </summary>
    /// <remarks>
    /// Input System: Requires PlayerInput component with OnMove action<br/>
    /// Mobile optimized: Fixed speed, no acceleration<br/>
    /// </remarks>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f; // Fixed speed (기획서 스펙)
        
        [Header("Health")]
        [SerializeField] private float maxHealth = 100f;
        private float currentHealth;
        
        [Header("References")]
        private Rigidbody rb;
        private Vector2 moveInput;
        
        // Events
        public event System.Action<float, float> OnHealthChanged; // (currentHealth, maxHealth)
        public event System.Action OnDeath;
        
        // Properties
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsAlive => currentHealth > 0;
        public Vector3 Position => transform.position;
        public Vector3 Forward => transform.forward;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            currentHealth = maxHealth;
            
            // Rigidbody settings
            rb.freezeRotation = true; // Prevent tipping over
            rb.linearDamping = 0f; // No drag for responsive movement
            rb.angularDamping = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // Prevent tunneling
        }
        
        private void Start()
        {
            // Initialize health UI
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
        
        private void FixedUpdate()
        {
            if (!IsAlive) return;
            
            HandleMovement();
            ClampPosition();
        }
        
        /// <summary>
        /// Handle player movement using Rigidbody.linearVelocity<br/>
        /// Mobile optimized: Fixed speed, instant direction change<br/>
        /// </summary>
        private void HandleMovement()
        {
            // Convert 2D input to 3D movement (XZ plane)
            Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y);
            movement = movement.normalized * moveSpeed;
            
            // Apply velocity
            rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
            
            // Rotate player to face movement direction
            if (movement.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movement);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, 
                    targetRotation, 
                    Time.fixedDeltaTime * 10f
                );
            }
        }
        
        /// <summary>
        /// Called by Input System (Unity Input System)<br/>
        /// Receives joystick/WASD input as Vector2<br/>
        /// </summary>
        /// <param name="value">Input value from InputAction</param>
        public void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }
        
        /// <summary>
        /// Take damage from enemies/projectiles<br/>
        /// Called by enemy collision or projectile hit<br/>
        /// </summary>
        /// <param name="damage">Damage amount</param>
        public void TakeDamage(float damage)
        {
            if (!IsAlive) return;
            
            currentHealth = Mathf.Max(0, currentHealth - damage);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            
            Debug.Log($"[PlayerController] HP: {currentHealth}/{maxHealth} (-{damage})");
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        /// <summary>
        /// Heal player (for future power-ups)<br/>
        /// </summary>
        /// <param name="amount">Heal amount</param>
        public void Heal(float amount)
        {
            if (!IsAlive) return;
            
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            
            Debug.Log($"[PlayerController] HP: {currentHealth}/{maxHealth} (+{amount})");
        }
        
        /// <summary>
        /// Handle player death<br/>
        /// Triggers GameManager defeat and stops movement<br/>
        /// </summary>
        private void Die()
        {
            OnDeath?.Invoke();
            GameManager.Instance.Defeat();
            
            // Stop movement
            rb.linearVelocity = Vector3.zero;
            moveInput = Vector2.zero;
            
            Debug.Log("[PlayerController] Player died");
        }
        
        /// <summary>
        /// Collision with enemies (contact damage)<br/>
        /// Enemy damage handling will be implemented in Phase3<br/>
        /// </summary>
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                // Enemy damage will be retrieved from EnemyBase component in Phase3
                Debug.Log($"[PlayerController] Collided with enemy: {collision.gameObject.name}");
                
                // Temporary: Take 10 damage for testing
                // TakeDamage(10f);
            }
        }
        
        /// <summary>
        /// Stay in map boundaries (80x80 units)<br/>
        /// Called each FixedUpdate after movement<br/>
        /// </summary>
        private void ClampPosition()
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, -40f, 40f);
            pos.z = Mathf.Clamp(pos.z, -40f, 40f);
            transform.position = pos;
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// Validation in Inspector<br/>
        /// </summary>
        private void OnValidate()
        {
            if (moveSpeed <= 0) moveSpeed = 5f;
            if (maxHealth <= 0) maxHealth = 100f;
        }
        
        /// <summary>
        /// Debug display in Game View<br/>
        /// Shows HP and speed for testing<br/>
        /// </summary>
        private void OnGUI()
        {
            if (!Application.isPlaying) return;
            
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.normal.textColor = Color.white;
            
            GUI.Label(new Rect(10, 10, 300, 25), $"HP: {currentHealth:F0}/{maxHealth:F0}", style);
            
            if (rb != null)
            {
                GUI.Label(new Rect(10, 35, 300, 25), $"Speed: {rb.linearVelocity.magnitude:F2} u/s", style);
            }
            
            GUI.Label(new Rect(10, 60, 300, 25), $"Input: ({moveInput.x:F2}, {moveInput.y:F2})", style);
        }
#endif
    }
}
