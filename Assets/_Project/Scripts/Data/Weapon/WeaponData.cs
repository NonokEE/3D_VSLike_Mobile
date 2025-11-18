using UnityEngine;
using System.Collections.Generic;

namespace VSLike.Data
{
    /// <summary>
    /// Main weapon data ScriptableObject
    /// References weapon-specific data for type-specific behavior
    /// </summary>
    [CreateAssetMenu(fileName = "New Weapon", menuName = "VSLike/Weapon Data")]
    public class WeaponDataSO : ScriptableObject
    {
        [Header("Basic Info")]
        public WeaponType weaponType;

        [Tooltip("Display name for UI")]
        public string weaponName;

        [Tooltip("Weapon icon for UI")]
        public Sprite weaponIcon;

        [Header("Common Stats")]
        public CommonWeaponStats commonStats;

        [Header("Level Progression")]
        [Tooltip("Upgrades for each level (index 0 = Lv1, index 1 = Lv2, etc.)")]
        public List<LevelUpgrade> levelUpgrades = new List<LevelUpgrade>(5);

        [Header("Weapon-Specific Data")]
        [Tooltip("Reference to weapon-specific ScriptableObject (ProjectileWeaponData, ExplosiveWeaponData, etc.)")]
        public ScriptableObject weaponSpecificData;

        /// <summary>
        /// Calculate damage at given level
        /// </summary>
        public float GetDamage(int level)
        {
            int index = Mathf.Clamp(level - 1, 0, levelUpgrades.Count - 1);
            return commonStats.baseDamage * levelUpgrades[index].damageMultiplier;
        }

        /// <summary>
        /// Calculate cooldown at given level
        /// </summary>
        public float GetCooldown(int level)
        {
            int index = Mathf.Clamp(level - 1, 0, levelUpgrades.Count - 1);
            return commonStats.baseCooldown * levelUpgrades[index].cooldownMultiplier;
        }

        /// <summary>
        /// Get base speed (weapon-specific data may apply multipliers)
        /// </summary>
        public float GetSpeed(int level)
        {
            return commonStats.baseSpeed;
        }

        /// <summary>
        /// Get base range (weapon-specific data may apply multipliers)
        /// </summary>
        public float GetRange(int level)
        {
            return commonStats.baseRange;
        }

        /// <summary>
        /// Get base piercing count (weapon-specific data may add bonuses)
        /// </summary>
        public int GetPiercing(int level)
        {
            return commonStats.basePiercing;
        }

        /// <summary>
        /// Get base projectile/effect count (weapon-specific data may add bonuses)
        /// </summary>
        public int GetCount(int level)
        {
            return commonStats.baseCount;
        }

        /// <summary>
        /// Get maximum level available for this weapon
        /// </summary>
        public int GetMaxLevel()
        {
            return levelUpgrades.Count;
        }
    }

    public enum WeaponType
    {
        Bullet,
        Missile,
        Flamethrower,
        Laser,
        Blade
    }

    /// <summary>
    /// Level upgrade configuration for weapons
    /// Defines damage/cooldown scaling and special effects per level
    /// </summary>
    [System.Serializable]
    public class LevelUpgrade
    {
        [Tooltip("Damage multiplier at this level")]
        public float damageMultiplier = 1.0f;
        
        [Tooltip("Cooldown multiplier at this level")]
        public float cooldownMultiplier = 1.0f;
        
        [Tooltip("Description of special effects at this level")]
        [TextArea(2, 4)]
        public string specialEffectDescription;
    }

    /// <summary>
    /// Base weapon stats shared by all weapon types
    /// </summary>
    [System.Serializable]
    public class CommonWeaponStats
    {
        [Header("Base Stats (Level 1)")]
        [Tooltip("Base damage at Level 1")]
        public float baseDamage = 10f;
        
        [Tooltip("Base cooldown in seconds")]
        public float baseCooldown = 1f;
        
        [Tooltip("Base speed (projectile/effect)")]
        public float baseSpeed = 20f;
        
        [Tooltip("Base range/radius")]
        public float baseRange = 10f;
        
        [Tooltip("Base piercing count (0 = no pierce)")]
        public int basePiercing = 0;
        
        [Tooltip("Base projectile/effect count")]
        public int baseCount = 1;
    }

}
