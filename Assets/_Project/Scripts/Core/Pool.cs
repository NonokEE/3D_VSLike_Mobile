using UnityEngine;
using System.Collections.Generic;

namespace VSLike.Core
{
    /// <summary>
    /// Generic object pool for Unity GameObjects with IPoolable components
    /// Preallocates objects to avoid runtime Instantiate/Destroy (GC optimization)
    /// </summary>
    /// <typeparam name="T">Component type that implements IPoolable</typeparam>
    public class Pool<T> where T : Component, IPoolable
    {
        private GameObject prefab;
        private Transform poolParent;
        private int initialSize;
        
        private List<T> availableObjects;
        private HashSet<T> activeObjects; // For fast Contains check
        
        /// <summary>
        /// Initialize pool with prefab and size
        /// Creates all objects upfront (no runtime allocation)
        /// </summary>
        public void Initialize(GameObject prefab, Transform parent, int size)
        {
            this.prefab = prefab;
            this.poolParent = parent;
            this.initialSize = size;
            
            availableObjects = new List<T>(size);
            activeObjects = new HashSet<T>();
            
            // Preallocate all objects
            for (int i = 0; i < size; i++)
            {
                CreateNewObject();
            }
            
            Debug.Log($"[Pool] Initialized {typeof(T).Name} pool with {size} objects");
        }
        
        /// <summary>
        /// Create a new pooled object (called only during initialization)
        /// </summary>
        private T CreateNewObject()
        {
            GameObject obj = Object.Instantiate(prefab, poolParent);
            T component = obj.GetComponent<T>();
            
            if (component == null)
            {
                Debug.LogError($"[Pool] Prefab {prefab.name} does not have {typeof(T).Name} component!");
                Object.Destroy(obj);
                return null;
            }
            
            obj.SetActive(false);
            availableObjects.Add(component);
            
            return component;
        }
        
        /// <summary>
        /// Get an object from the pool
        /// Returns null if pool is exhausted (no dynamic expansion on mobile)
        /// </summary>
        public T Get()
        {
            if (availableObjects.Count == 0)
            {
                Debug.LogWarning($"[Pool] {typeof(T).Name} pool exhausted! Consider increasing pool size.");
                return null;
            }
            
            // Get last object (O(1) removal)
            int lastIndex = availableObjects.Count - 1;
            T obj = availableObjects[lastIndex];
            availableObjects.RemoveAt(lastIndex);
            
            // Activate and track
            activeObjects.Add(obj);
            obj.gameObject.SetActive(true);
            obj.OnSpawnFromPool();
            
            return obj;
        }
        
        /// <summary>
        /// Get an object with position and rotation
        /// </summary>
        public T Get(Vector3 position, Quaternion rotation)
        {
            T obj = Get();
            if (obj != null)
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;
            }
            return obj;
        }
        
        /// <summary>
        /// Return an object to the pool
        /// </summary>
        public void Return(T obj)
        {
            if (obj == null)
            {
                Debug.LogWarning("[Pool] Attempting to return null object");
                return;
            }
            
            if (!activeObjects.Contains(obj))
            {
                Debug.LogWarning($"[Pool] Attempting to return object not from this pool: {obj.name}");
                return;
            }
            
            // Deactivate and return to available list
            activeObjects.Remove(obj);
            obj.OnReturnToPool();
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(poolParent);
            availableObjects.Add(obj);
        }
        
        /// <summary>
        /// Return all active objects to pool
        /// Useful for scene transitions or game restart
        /// </summary>
        public void ReturnAll()
        {
            // Copy to temp list to avoid modification during iteration
            List<T> temp = new List<T>(activeObjects);
            foreach (T obj in temp)
            {
                Return(obj);
            }
        }
        
        /// <summary>
        /// Get current pool statistics
        /// </summary>
        public PoolStats GetStats()
        {
            return new PoolStats
            {
                totalSize = initialSize,
                activeCount = activeObjects.Count,
                availableCount = availableObjects.Count
            };
        }
        
        /// <summary>
        /// Clear and destroy all pooled objects
        /// Call on scene unload
        /// </summary>
        public void Clear()
        {
            foreach (T obj in availableObjects)
            {
                if (obj != null)
                    Object.Destroy(obj.gameObject);
            }
            
            foreach (T obj in activeObjects)
            {
                if (obj != null)
                    Object.Destroy(obj.gameObject);
            }
            
            availableObjects.Clear();
            activeObjects.Clear();
        }
    }
    
    /// <summary>
    /// Pool statistics for debugging
    /// </summary>
    public struct PoolStats
    {
        public int totalSize;
        public int activeCount;
        public int availableCount;
        
        public override string ToString()
        {
            return $"Total: {totalSize}, Active: {activeCount}, Available: {availableCount}";
        }
    }
}
