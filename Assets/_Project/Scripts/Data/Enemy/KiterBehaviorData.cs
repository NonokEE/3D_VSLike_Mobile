using UnityEngine;

namespace VSLike.Data
{
    /// <summary>
    /// Kiter behavior specific data (ranged + distance maintenance)
    /// </summary>
    [CreateAssetMenu(fileName = "Kiter Behavior", menuName = "VSLike/Enemy Behavior/Kiter")]
    public class KiterBehaviorData : ScriptableObject, IEnemyBehaviorData
    {
        [Header("Range Settings")]
        [Tooltip("Maximum attack range")]
        public float attackRange = 15f;
        
        [Tooltip("Preferred maintain distance from player")]
        public float maintainDistance = 10f;
        
        [Tooltip("Minimum safe distance (retreat threshold)")]
        public float minSafeDistance = 8f;
        
        [Header("Attack Settings")]
        [Tooltip("Projectile speed")]
        public float projectileSpeed = 15f;
        
        [Tooltip("Projectile damage (uses enemy base damage if 0)")]
        public float projectileDamage = 0f;
        
        [Tooltip("Attack cooldown in seconds")]
        public float attackCooldown = 2f;
        
        [Tooltip("Projectile lifetime in seconds")]
        public float projectileLifetime = 2f;
        
        [Header("Behavior Settings")]
        [Tooltip("State evaluation interval (seconds)")]
        public float stateUpdateInterval = 0.5f;
        
        [Tooltip("Movement speed multiplier when retreating")]
        public float retreatSpeedMultiplier = 1.2f;
        
        [Tooltip("Target prediction (lead shots)")]
        public bool predictPlayerMovement = false;
        
        [Header("Visual")]
        [Tooltip("Kiter projectile prefab")]
        public GameObject projectilePrefab;
    }
}
