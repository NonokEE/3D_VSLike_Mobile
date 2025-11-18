using UnityEngine;

namespace VSLike.Data
{
    /// <summary>
    /// Flame weapon specific data (Flamethrower)
    /// </summary>
    [CreateAssetMenu(fileName = "Flame Data", menuName = "VSLike/Weapon Specific/Flame")]
    public class FlameWeaponData : ScriptableObject, IWeaponSpecificData
    {
        [Header("Cone Behavior")]
        [Tooltip("Cone angle in degrees")]
        public float coneAngle = 60f;
        
        [Header("DoT Behavior")]
        [Tooltip("DoT tick damage per hit")]
        public float dotTickDamage = 5f;
        
        [Tooltip("DoT tick interval in seconds")]
        public float dotTickInterval = 0.2f;
        
        [Tooltip("Base DoT duration in seconds")]
        public float baseDotDuration = 2f;
        
        [Header("Level-Specific Bonuses (5 levels)")]
        [Tooltip("DoT duration increase per level (seconds)")]
        public float[] dotDurationBonusPerLevel = new float[5] { 0f, 0f, 1f, 1f, 2f };
        
        [Tooltip("Cone angle increase per level (degrees)")]
        public float[] coneAngleBonusPerLevel = new float[5] { 0f, 0f, 15f, 15f, 30f };
        
        [Header("Visual")]
        [Tooltip("Flame effect prefab")]
        public GameObject flameEffectPrefab;
    }
}
