namespace VSLike.Core
{
    /// <summary>
    /// Interface for objects that can be pooled<br/>
    /// Implement this on MonoBehaviour components that need object pooling<br/>
    /// </summary>
    public interface IPoolable
    {
        /// <summary>
        /// Called when object is spawned from pool<br/>
        /// Use this instead of Awake/Start for pooled objects<br/>
        /// </summary>
        void OnSpawnFromPool();
        
        /// <summary>
        /// Called when object is returned to pool<br/>
        /// Clean up state, stop coroutines, reset transforms, etc.<br/>
        /// </summary>
        void OnReturnToPool();
    }
}
