using UnityEngine;

namespace BeaconPatch.GameObjectPoolSystem
{
    /// <summary>
    /// A GameObject instance container in charge of pre-instantiating a number of them based of prefab “model” and to
    /// distribute (take from, or activate) or receive (return to, or deactivate) them as needed until it can’t do so.
    /// </summary>
    public class GameObjectPool : MonoBehaviour
    {
        #region References
        private GameObject _prefab = null;
        #endregion References
        
        #region Const
        public const float RESTING_HEIGHT = 1000.0f;
        #endregion Const
        
        
        #region Initialization
        /// <summary>
        /// Initialize the GameObjectPool, instantiating a quantity of a prefab (or any GameObject) that serves as a
        /// model. Initialization can only be done once, failing if already initialized. It must also be done manually,
        /// usually taken care of by the GameObjectPoolMaster.
        /// </summary>
        /// <param name="prefab">The GameObject model, a prefab, to instantiate.</param>
        /// <param name="quantity">The quantity of the model to instantiate.</param>
        public void InitializePool(GameObject prefab, uint quantity)
        {
            if (_prefab != null) return;

            _prefab = prefab;
            
            Transform defaultTransform = transform;
            Vector3 defaultPosition = defaultTransform.position;
            defaultTransform.position = defaultPosition;
            
            for (uint i = 0; i < quantity; i++)
            {
                GameObject newInstance = Instantiate(_prefab, defaultTransform);
                newInstance.name = _prefab.name + "_" + i;
                
                foreach (IPoolableObject poolableObject in newInstance.gameObject.GetComponents<IPoolableObject>())
                {
                    poolableObject.OnReturnedToPool();
                }
                
                newInstance.gameObject.SetActive(false);
                
                newInstance.transform.position += Vector3.down * RESTING_HEIGHT;
            }

            UpdatePoolName();
        }
        #endregion Initialization
        
        #region Update
        private void UpdatePoolName()
        {
            uint activeCount = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (!transform.GetChild(i).gameObject.activeSelf)
                {
                    activeCount++;
                }
            }
            
            transform.name = "GOP " + _prefab.name + " (" + activeCount + "/" + transform.childCount + ")";
        }
        #endregion Update
        
        #region Request & Return
        /// <summary>
        /// Activate, and return the available GameObject from this GameObjectPool or null it none was found.
        /// </summary>
        /// <returns>Return an activated GameObject if one could be retrieved from the GameObjectPool</returns>
        public GameObject RequestObject()
        {
            GameObject found = null;
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject candidate = transform.GetChild(i).gameObject;
                if (!candidate.activeSelf)
                {
                    candidate.SetActive(true);
                    UpdatePoolName();
                    found = candidate;
                    foreach (IPoolableObject component in found.GetComponents<IPoolableObject>())
                    {
                        component.OnExtractedFromPool();
                    }
                    break;
                }
            }
            
            return found;
        }
        
        
        /// <summary>
        /// Try returning a GameObject to this GameObjectPool, failing if it isn't contained within.
        /// </summary>
        /// <param name="poolableObject">The GameObject to return to this GameObjectPool.</param>
        /// <returns>Returns true if the GameObject was returned to the GameObjectPool.</returns>
        public bool ReturnObject(GameObject poolableObject)
        {
            if (!IsOriginatingFromPool(poolableObject)) return false;
            
            foreach (IPoolableObject component in poolableObject.GetComponents<IPoolableObject>())
            {
                component.OnReturnedToPool();
            }
            poolableObject.SetActive(false);
            UpdatePoolName();
            return true;
        }


        /// <summary>
        /// Return all GameObject from GameObjectPool back to it.
        /// </summary>
        public void ReturnAllObjects()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.gameObject.activeInHierarchy)
                {
                    foreach (IPoolableObject component in child.GetComponents<IPoolableObject>())
                    {
                        component.OnReturnedToPool();
                    }
                }
                child.gameObject.SetActive(false);
            }
            
            UpdatePoolName();
        }
        #endregion Request & Return
        
        #region Getters
        /// <summary>
        /// Get the model GameObject that serves as the base for this GameObjectPool.
        /// </summary>
        /// <returns>The model for this GameObjectPool.</returns>
        public GameObject GetPrefab() { return _prefab; }
        
        
        /// <summary>
        /// Get the number of available GameObject in this GameObjectPool that could still be activated before running out.
        /// </summary>
        /// <returns>The currently amount of GameObject available for retrieval in this GameObjectPool.</returns>
        public int GetAvailableObjectCount()
        {
            int availableCount = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (!transform.GetChild(i).gameObject.activeInHierarchy) availableCount++;
            }
            return availableCount;
        }
        
        
        private bool IsOriginatingFromPool(GameObject poolableObject)
        {
            bool isOriginatingFromPool = false;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (poolableObject == transform.GetChild(i).gameObject)
                {
                    isOriginatingFromPool = true;
                    break;
                }
            }

            return isOriginatingFromPool;
        }
        #endregion Getters
    }
}
