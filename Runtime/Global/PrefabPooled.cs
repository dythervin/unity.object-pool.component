using System;
using Dythervin.Callbacks;
using Dythervin.Core;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;
#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Dythervin.ObjectPool.Component.Global
{
    [Serializable]
    public sealed class PrefabPooled<T> : ISerializationCallbackReceiver, IPlayModeListener
        where T : UnityEngine.Component
    {
        [SerializeField] private T value;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [ShowInInspector] [ReadOnly]
#endif
        internal int instanceId;

        public IComponentPool<T> Pool { get; private set; }

        public T Prefab => value;

        bool IPlayModeListener.MainThreadOnly => true;

        void IPlayModeListener.OnEnterPlayMode()
        {
            Init();
        }

        [Preserve]
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            this.PlayModeSubscribe();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        public PrefabPooled([NotNull] T prefab)
        {
            value = prefab;
            Init();
        }

        public T GetSafe(Transform parent)
        {
#if UNITY_EDITOR
            if (!ApplicationExt.IsPlaying)
                return (T)PrefabUtility.InstantiatePrefab(Prefab, parent);
#endif
            return Pool.Get(parent);
        }

        private void Init()
        {
            instanceId = value.GetInstanceID();
            Pool = this.GetPool();
        }
    }
}