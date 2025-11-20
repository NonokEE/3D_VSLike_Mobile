namespace VSLike.Core
{
    /// <summary>
    /// Interface for objects that can be pooled
    /// Implement this on MonoBehaviour components that need object pooling
    /// </summary>
    public interface IPoolable
    {
        /// <summary>
        /// Called when object is spawned from pool
        /// Use this instead of Awake/Start for pooled objects
        /// </summary>
        void OnSpawnFromPool();
        
        /// <summary>
        /// Called when object is returned to pool
        /// Clean up state, stop coroutines, reset transforms, etc.
        /// </summary>
        void OnReturnToPool();
    }
}
