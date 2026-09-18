using UnityEngine;

namespace BeaconPatch.GameObjectPoolSystem
{
    /// <summary>
    /// An interface used by the GameObjectPool to execute logic on the GameObject’s MonoBehaviour components whenever
    /// taken from or returned to the GameObjectPool.
    /// </summary>
    public interface IPoolableObject
    {
        #region Abstract methods
        /// <summary>
        /// Called utility method when gameObject is activated (taken from a GameObjectPool).
        /// </summary>
        public abstract void OnExtractedFromPool();
        
        /// <summary>
        /// Called utility method when gameObject is deactivated (returning to a GameObjectPool).
        /// </summary>
        public abstract void OnReturnedToPool();
        #endregion Abstract methods
    }
}
