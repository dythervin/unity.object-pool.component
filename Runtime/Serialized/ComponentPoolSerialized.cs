using UnityEngine;

namespace Dythervin.ObjectPool.Component.Serialized
{
    [System.Serializable]
    public class ComponentPoolSerialized<T> : ComponentPool<T>
        where T : UnityEngine.Component
    {
        [SerializeField] private int capacity = 10;
        private ComponentPoolSerialized() : base() { }

        protected override void OnEnterPlayMode()
        {
            base.OnEnterPlayMode();
            MaxSize = maxSize;
            SetStack(capacity);
        }
    }
}