using UnityEngine;

namespace VSLike.Data
{
    /// <summary>
    /// Orbital weapon specific data (Blade)
    /// </summary>
    [CreateAssetMenu(fileName = "Orbital Data", menuName = "VSLike/Weapon Specific/Orbital")]
    public class OrbitalWeaponData : ScriptableObject, IWeaponSpecificData
    {
        [Header("Orbital Behavior")]
        [Tooltip("Orbit radius around player")]
        public float orbitRadius = 3f;
        
        [Tooltip("Rotation speed in degrees/sec")]
        public float rotationSpeed = 360f;
        
        [Tooltip("Hit cooldown per enemy (prevent rapid multi-hit)")]
        public float hitCooldown = 0.5f;
        
        [Header("Level-Specific Bonuses (5 levels)")]
        [Tooltip("Number of blades per level")]
        public int[] bladeCountPerLevel = new int[5] { 1, 1, 2, 2, 3 };
        
        [Tooltip("Orbit radius multiplier per level")]
        public float[] radiusMultiplierPerLevel = new float[5] { 1f, 1f, 1.2f, 1.5f, 1.5f };
        
        [Tooltip("Rotation speed multiplier per level")]
        public float[] speedMultiplierPerLevel = new float[5] { 1f, 1.2f, 1.5f, 1.5f, 2f };
        
        [Header("Visual")]
        [Tooltip("Blade prefab reference")]
        public GameObject bladePrefab;
    }
}
