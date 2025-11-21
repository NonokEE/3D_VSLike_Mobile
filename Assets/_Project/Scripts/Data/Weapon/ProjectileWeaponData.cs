using UnityEngine;

namespace VSLike.Data
{
    /// <summary>
    /// Projectile weapon specific data (Bullet)
    /// </summary>
    [CreateAssetMenu(fileName = "Projectile Data", menuName = "VSLike/Weapon Specific/Projectile")]
    public class ProjectileWeaponData : ScriptableObject, IWeaponSpecificData
    {
        [Header("Projectile Behavior")]
        [Tooltip("Projectile lifetime in seconds")]
        public float lifetime = 3f;
        
        [Tooltip("Projectile acceleration (0 = constant speed)")]
        public float acceleration = 0f;
        
        [Tooltip("Max speed after acceleration")]
        public float maxSpeed = 30f;
        
        [Header("Level-Specific Bonuses (5 levels)")]
        [Tooltip("Piercing increase per level")]
        public int[] piercingPerLevel = new int[5] { 0, 0, 0, 2, 1 };
        
        [Tooltip("Speed multiplier per level")]
        public float[] speedMultiplierPerLevel = new float[5] { 1f, 1f, 1.25f, 1.25f, 1.5f };
        
        [Header("Visual")]
        [Tooltip("Projectile prefab reference")]
        public GameObject projectilePrefab;
    }
}
