using UnityEngine;

namespace VSLike.Data
{
    /// <summary>
    /// Beam weapon specific data (Laser)
    /// </summary>
    [CreateAssetMenu(fileName = "Beam Data", menuName = "VSLike/Weapon Specific/Beam")]
    public class BeamWeaponData : ScriptableObject, IWeaponSpecificData
    {
        [Header("Beam Behavior")]
        [Tooltip("Beam width")]
        public float beamWidth = 1f;
        
        [Tooltip("Beam visual duration (instant damage)")]
        public float beamDuration = 0.1f;
        
        [Tooltip("Base pierce count")]
        public int basePierceCount = 5;
        
        [Header("Level-Specific Bonuses (5 levels)")]
        [Tooltip("Pierce count per level (-1 = infinite pierce)")]
        public int[] pierceCountPerLevel = new int[5] { 5, 7, 10, -1, -1 };
        
        [Tooltip("Beam width multiplier per level")]
        public float[] widthMultiplierPerLevel = new float[5] { 1f, 1f, 1.2f, 1.5f, 2f };
        
        [Header("Visual")]
        [Tooltip("Laser beam LineRenderer prefab")]
        public GameObject laserBeamPrefab;
    }
}
