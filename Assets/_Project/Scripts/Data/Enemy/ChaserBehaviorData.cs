using UnityEngine;

namespace VSLike.Data
{
    /// <summary>
    /// Chaser behavior specific data (NavMesh pathfinding)
    /// </summary>
    [CreateAssetMenu(fileName = "Chaser Behavior", menuName = "VSLike/Enemy Behavior/Chaser")]
    public class ChaserBehaviorData : ScriptableObject, IEnemyBehaviorData
    {
        [Header("Pathfinding")]
        [Tooltip("NavMesh path recalculation interval (seconds)")]
        public float pathUpdateInterval = 1f;
        
        [Tooltip("Stopping distance from player")]
        public float stoppingDistance = 0.5f;
        
        [Tooltip("NavMeshAgent acceleration")]
        public float acceleration = 8f;
        
        [Tooltip("NavMeshAgent angular speed")]
        public float angularSpeed = 120f;
        
        [Header("Attack Behavior")]
        [Tooltip("Dies on player contact")]
        public bool diesOnContact = true;
        
        [Tooltip("Knockback force on hit")]
        public float knockbackForce = 3f;
    }
}
