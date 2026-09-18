using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace BeaconPatch.GameObjectPoolSystem
{
    /// <summary>
    /// In charge of defining and managing GameObjectPool, act as an entry gate to MultipleGameObjectPool. It can be
    /// used to define a quantity of them and initialize them all automatically and as en entry point to request or
    /// return GameObject instance.
    /// </summary>
    public class GameObjectPoolManager : MonoBehaviour
    {
        #region Parameters
        [SerializeField] [Tooltip("Establish a list of model GameObject (prefabs) and their quantity to build" +
                                  "GameObjectPools out of.")]
        private List<PoolDefinition> poolDefinitions = new List<PoolDefinition>();
        #endregion Parameters

        #region References
        public static GameObjectPoolManager instance { get; private set; }
        #endregion
        
        
        #region MonoBehaviour
        private void Awake()
        {
            if (TryInitializeSingleton())
            {
                CreatePools();
            }
        }
        #endregion MonoBehaviour
        
        
        #region Initialization
        private bool TryInitializeSingleton()
        {
            if (instance != null)
            {
                Debug.LogError(this + "(" + gameObject + ")" + " tried to register itself as the WorldManager " +
                               "singleton, but it was already defined. Destroying and aborting initialization.");
                gameObject.name = "! " + gameObject.name;
                Destroy(this);
                return false;
            }
            
            instance = this;
            return true;
        }


        private void CreatePools()
        {
            RemoveDuplicatePools();
            
            foreach (PoolDefinition poolDefinition in poolDefinitions)
            {
                CreatePool(poolDefinition.prefab, poolDefinition.quantity);
            }
        }
        
        
        private void RemoveDuplicatePools()
        {
            List<GameObject> prefabFound = new List<GameObject>();
            List<PoolDefinition> definitions = new List<PoolDefinition>();
            
            foreach (PoolDefinition poolDefinition in poolDefinitions)
            {
                if (prefabFound.Contains(poolDefinition.prefab))
                {
                    continue;
                }
                prefabFound.Add(poolDefinition.prefab);
                definitions.Add(poolDefinition);
            }

            poolDefinitions.Clear();
            poolDefinitions.AddRange(definitions);
        }
        
        
        private void CreatePool(GameObject prefab, uint quantity)
        {
            GameObject newPoolObject = new GameObject();
            newPoolObject.transform.parent = transform;
            
            GameObjectPool newPoolComponent = newPoolObject.AddComponent<GameObjectPool>();
            newPoolComponent.InitializePool(prefab, quantity);
        }
        #endregion
        
        #region Access
        /// <summary>
        /// Return a GameObjectPool with a matching model prefab, or null if none is found.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public GameObjectPool GetPoolForGameObject(GameObject model)
        {
            GameObjectPool foundPool = null;
            foreach (GameObjectPool gameObjectPool in GetComponentsInChildren<GameObjectPool>())
            {
                if (model.name.StartsWith(gameObjectPool.GetPrefab().name))
                {
                    foundPool = gameObjectPool;
                    break;
                }
            }
            
            return foundPool;
        }
        #endregion Access

        #region Request
        /// <summary>
        /// Activate, and return the available GameObject from a corresponding GameObjectPool or null it none was found.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public GameObject RequestObject(GameObject model)
        {
            GameObjectPool pool = GetPoolForGameObject(model);
            if (pool == null || pool.GetAvailableObjectCount() == 0) return null;

            return pool.RequestObject();
        }
        
        
        /// <summary>
        /// Try returning a GameObject to its GameObjectPool, failing if it isn't contained within any.
        /// </summary>
        /// <param name="objectInstance"></param>
        public bool ReturnObject(GameObject objectInstance)
        {
            foreach (GameObjectPool gameObjectPool in GetComponentsInChildren<GameObjectPool>())
            {
                if (gameObjectPool.ReturnObject(objectInstance)) return true;
            }
            
            return false;
        }
        
        
        /// <summary>
        /// Return all GameObjects from all GameObjectPool back, effectively deactivating everything.
        /// </summary>
        public void ReturnAllObjects()
        {
            foreach (GameObjectPool gameObjectPool in GetComponentsInChildren<GameObjectPool>())
            {
                gameObjectPool.ReturnAllObjects();
            }
        }
        #endregion
    }
}
