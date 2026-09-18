using System;
using UnityEngine;

namespace BeaconPatch.GameObjectPoolSystem
{
    [Serializable]
    public struct PoolDefinition
    {
        public GameObject prefab;
        public uint quantity;
    }
}
