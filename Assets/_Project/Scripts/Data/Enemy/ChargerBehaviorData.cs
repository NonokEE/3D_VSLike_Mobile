using UnityEngine;

namespace VSLike.Data
{
    /// <summary>
    /// Charger behavior specific data (straight-line aggressive)
    /// </summary>
    [CreateAssetMenu(fileName = "Charger Behavior", menuName = "VSLike/Enemy Behavior/Charger")]
    public class ChargerBehaviorData : ScriptableObject, IEnemyBehaviorData
    {
        [Header("Attack Behavior")]
        [Tooltip("Dies immediately on player contact (suicide attack)")]
        public bool suicidalAttack = true;
        
        [Tooltip("Knockback force applied to player on hit")]
        public float knockbackForce = 5f;
        
        [Header("Movement")]
        [Tooltip("Direct movement without pathfinding")]
        public bool useDirectMovement = true;
    }
}
