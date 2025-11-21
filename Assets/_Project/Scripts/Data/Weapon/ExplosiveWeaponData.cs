using UnityEngine;

namespace VSLike.Data
{
    /// <summary>
    /// Explosive weapon specific data (Missile)
    /// </summary>
    [CreateAssetMenu(fileName = "Explosive Data", menuName = "VSLike/Weapon Specific/Explosive")]
    public class ExplosiveWeaponData : ScriptableObject, IWeaponSpecificData
    {
        [Header("Explosion Behavior")]
        [Tooltip("Base explosion radius")]
        public float baseExplosionRadius = 5f;
        
        [Tooltip("Explosion damage (separate from direct hit)")]
        public float explosionDamage = 10f;
        
        [Tooltip("Projectile acceleration duration")]
        public float accelerationDuration = 1f;
        
        [Tooltip("Initial speed before acceleration")]
        public float initialSpeed = 10f;
        
        [Tooltip("Final speed after acceleration")]
        public float finalSpeed = 30f;
        
        [Header("Level-Specific Bonuses (5 levels)")]
        [Tooltip("Explosion radius multiplier per level")]
        public float[] radiusMultiplierPerLevel = new float[5] { 1f, 1f, 1.25f, 1.25f, 1.5f };
        
        [Tooltip("Additional chain explosions at certain levels")]
        public int[] chainExplosionsPerLevel = new int[5] { 0, 0, 0, 0, 1 };
        
        [Header("Visual")]
        [Tooltip("Missile prefab reference")]
        public GameObject missilePrefab;
        
        [Tooltip("Explosion effect prefab")]
        public GameObject explosionEffectPrefab;
    }
}
