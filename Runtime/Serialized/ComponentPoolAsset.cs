using UnityEngine;

namespace Dythervin.ObjectPool.Component.Serialized
{
    public abstract class ComponentPoolAsset<T> : ScriptableObject
        where T : UnityEngine.Component
    {
        [SerializeField]
        private ComponentPoolSerialized<T> pool;

        public IComponentPool<T> Pool => pool;

        public event System.Action<T> OnSpawned
        {
            add => pool.OnCreated += value;
            remove => pool.OnCreated -= value;
        }

        protected virtual void OnDisable()
        {
#if UNITY_EDITOR
            if (pool.Parent)
#endif
                Destroy(pool.Parent.gameObject);
        }
    }
}