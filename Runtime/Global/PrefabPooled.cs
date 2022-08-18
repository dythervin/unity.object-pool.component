using System;
using Dythervin.Callbacks;
using Dythervin.Core;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
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

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            this.TryEnterPlayMode();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (value && !EditorUtility.IsPersistent(value))
            {
                Debug.Log($"{nameof(value)} must be prefab", value);
                value = null;
            }
#endif
        }

        public PrefabPooled([NotNull] T prefab)
        {
            value = prefab;
            Init();
        }

        public T GetEditorSafe(Transform parent)
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