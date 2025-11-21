using UnityEngine;
using System;
using System.Collections.Generic;

namespace VSLike.Core
{
    /// <summary>
    /// Centralized object pool manager (Singleton)<br/>
    /// Manages multiple pools for different object types<br/>
    /// Call Initialize() at game start with all required prefabs<br/>
    /// </summary>
    public class PoolManager : MonoBehaviour
    {
        private static PoolManager _instance;
        public static PoolManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("PoolManager");
                    _instance = go.AddComponent<PoolManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
        
        // Pool storage
        private Dictionary<Type, object> pools = new Dictionary<Type, object>();
        
        // Hierarchy organization
        private Transform poolRoot;
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Create pool root for hierarchy organization
            poolRoot = new GameObject("Pools").transform;
            poolRoot.SetParent(transform);
        }
        
        /// <summary>
        /// Create and initialize a pool for a specific type
        /// Must be called before using Get/Return for that type
        /// </summary>
        /// <typeparam name="T">Component type implementing IPoolable</typeparam>
        /// <param name="prefab">Prefab to pool</param>
        /// <param name="initialSize">Number of objects to preallocate</param>
        public void CreatePool<T>(GameObject prefab, int initialSize) where T : Component, IPoolable
        {
            Type type = typeof(T);
            
            if (pools.ContainsKey(type))
            {
                Debug.LogWarning($"[PoolManager] Pool for {type.Name} already exists!");
                return;
            }
            
            // Create pool parent for hierarchy
            Transform poolParent = new GameObject($"[{type.Name}Pool]").transform;
            poolParent.SetParent(poolRoot);
            
            // Create and initialize pool
            Pool<T> pool = new Pool<T>();
            pool.Initialize(prefab, poolParent, initialSize);
            
            pools[type] = pool;
        }
        
        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public T Get<T>() where T : Component, IPoolable
        {
            Pool<T> pool = GetPool<T>();
            if (pool == null)
            {
                Debug.LogError($"[PoolManager] No pool found for {typeof(T).Name}. Call CreatePool first!");
                return null;
            }
            
            return pool.Get();
        }
        
        /// <summary>
        /// Get an object with position and rotation
        /// </summary>
        public T Get<T>(Vector3 position, Quaternion rotation) where T : Component, IPoolable
        {
            Pool<T> pool = GetPool<T>();
            if (pool == null)
            {
                Debug.LogError($"[PoolManager] No pool found for {typeof(T).Name}. Call CreatePool first!");
                return null;
            }
            
            return pool.Get(position, rotation);
        }
        
        /// <summary>
        /// Return an object to the pool
        /// </summary>
        public void Return<T>(T obj) where T : Component, IPoolable
        {
            Pool<T> pool = GetPool<T>();
            if (pool == null)
            {
                Debug.LogError($"[PoolManager] No pool found for {typeof(T).Name}");
                return;
            }
            
            pool.Return(obj);
        }
        
        /// <summary>
        /// Return all active objects of a type to the pool
        /// </summary>
        public void ReturnAll<T>() where T : Component, IPoolable
        {
            Pool<T> pool = GetPool<T>();
            if (pool != null)
            {
                pool.ReturnAll();
            }
        }
        
        /// <summary>
        /// Get pool statistics for debugging
        /// </summary>
        public PoolStats GetPoolStats<T>() where T : Component, IPoolable
        {
            Pool<T> pool = GetPool<T>();
            if (pool == null)
            {
                return default;
            }
            
            return pool.GetStats();
        }
        
        /// <summary>
        /// Clear and destroy all pools
        /// </summary>
        public void ClearAllPools()
        {
            foreach (var poolObj in pools.Values)
            {
                // Use reflection to call Clear on unknown Pool<T> type
                var clearMethod = poolObj.GetType().GetMethod("Clear");
                clearMethod?.Invoke(poolObj, null);
            }
            
            pools.Clear();
        }
        
        /// <summary>
        /// Get pool instance (internal use)
        /// </summary>
        private Pool<T> GetPool<T>() where T : Component, IPoolable
        {
            Type type = typeof(T);
            
            if (pools.TryGetValue(type, out object poolObj))
            {
                return poolObj as Pool<T>;
            }
            
            return null;
        }
        
        private void OnDestroy()
        {
            if (_instance == this)
            {
                ClearAllPools();
                _instance = null;
            }
        }
    }
}
