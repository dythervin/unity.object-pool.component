using System;

namespace Dythervin.ObjectPool.Component.Global
{
    internal class GlobalComponentPool<T> : ComponentPool<T>
        where T : UnityEngine.Component
    {
        public event Action<GlobalComponentPool<T>, T> OnInstantiated;
        public event Action<GlobalComponentPool<T>, T> OnDestroyed;

        public GlobalComponentPool(T prefab, Action<T> onGet = null, Action<T> onRelease = null, Action<T> actionOnDestroy = null,
            bool onlyPoolCanDestroyObj = true, bool collectionCheckDefault = DefaultCollectionCheck,
            int defaultCapacity = DefaultCapacity, int maxSize = DefaultMaxSize) : base(prefab, onGet, onRelease, actionOnDestroy,
            onlyPoolCanDestroyObj, collectionCheckDefault, defaultCapacity, maxSize)
        {
            OnCreated += Created;
            OnDestroy += Destroyed;
        }

        private void Destroyed(T obj)
        {
            OnDestroyed?.Invoke(this, obj);
        }

        private void Created(T obj)
        {
            OnInstantiated?.Invoke(this, obj);
        }
    }
}