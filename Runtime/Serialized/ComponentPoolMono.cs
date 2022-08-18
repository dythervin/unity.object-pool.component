using System;
using UnityEngine;

namespace Dythervin.ObjectPool.Component.Serialized
{
    [Serializable]
    public abstract class ComponentPoolMono<T> : MonoBehaviour
        where T : UnityEngine.Component
    {
        [SerializeField]
        protected ComponentPoolSerialized<T> pool;

        public IComponentPool<T> Pool => pool;

        public event Action<T> OnItemSpawned
        {
            add => pool.OnCreated += value;
            remove => pool.OnCreated -= value;
        }
    }
}