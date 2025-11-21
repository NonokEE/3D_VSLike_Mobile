using UnityEngine;
using System.Collections.Generic;

namespace VSLike.Data
{
    /// <summary>
    /// Main enemy data ScriptableObject<br/>
    /// References behavior-specific data for type-specific AI
    /// </summary>
    [CreateAssetMenu(fileName = "New Enemy", menuName = "VSLike/Enemy Data")]
    public class EnemyDataSO : ScriptableObject
    {
        [Header("Basic Info")]
        public EnemyType enemyType;

        [Tooltip("Display name for debugging")]
        public string enemyName;

        [Header("Common Stats")]
        public CommonEnemyStats commonStats;

        [Header("Wave Progression")]
        [Tooltip("Scaling for each wave (extensible list)")]
        public List<WaveScaling> waveScaling = new List<WaveScaling>(4);

        [Header("Behavior-Specific Data")]
        [Tooltip("Reference to behavior-specific ScriptableObject (ChargerBehaviorData, ChaserBehaviorData, etc.)")]
        public ScriptableObject behaviorData;

        /// <summary>
        /// Calculate HP at given wave
        /// </summary>
        public float GetHP(int wave)
        {
            int index = Mathf.Clamp(wave - 1, 0, waveScaling.Count - 1);
            return commonStats.baseHP * waveScaling[index].hpMultiplier;
        }

        /// <summary>
        /// Calculate damage at given wave
        /// </summary>
        public float GetDamage(int wave)
        {
            int index = Mathf.Clamp(wave - 1, 0, waveScaling.Count - 1);
            return commonStats.baseDamage * waveScaling[index].damageMultiplier;
        }

        /// <summary>
        /// Calculate speed at given wave
        /// </summary>
        public float GetSpeed(int wave)
        {
            int index = Mathf.Clamp(wave - 1, 0, waveScaling.Count - 1);
            return commonStats.baseSpeed * waveScaling[index].speedMultiplier;
        }

        /// <summary>
        /// Get maximum wave count configured
        /// </summary>
        public int GetMaxWave()
        {
            return waveScaling.Count;
        }
    }
    
    public enum EnemyType
    {
        Charger,
        Chaser,
        Kiter
    }

    /// <summary>
    /// Wave scaling configuration
    /// Supports dynamic wave count expansion
    /// </summary>
    [System.Serializable]
    public class WaveScaling
    {
        [Tooltip("HP multiplier for this wave")]
        public float hpMultiplier = 1.0f;
        
        [Tooltip("Damage multiplier for this wave")]
        public float damageMultiplier = 1.0f;
        
        [Tooltip("Speed multiplier for this wave")]
        public float speedMultiplier = 1.0f;
    }

    /// <summary>
    /// Common enemy stats shared by all enemy types
    /// </summary>
    [System.Serializable]
    public class CommonEnemyStats
    {
        [Header("Base Stats (Wave 1)")]
        [Tooltip("Base HP at Wave 1")]
        public float baseHP = 30f;
        
        [Tooltip("Base contact damage")]
        public float baseDamage = 10f;
        
        [Tooltip("Base movement speed")]
        public float baseSpeed = 3f;
        
        [Tooltip("Experience reward on death")]
        public int experienceReward = 10;
        
        [Tooltip("Dies on contact with player")]
        public bool diesOnContact = true;
    }
}
