#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System;
using System.Collections.Generic;
using Dythervin.Callbacks;
using Dythervin.Core;
using Dythervin.Core.Extensions;
using Dythervin.Core.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dythervin.ObjectPool.Component
{
    [Serializable]
    public class ComponentPool<T> : ObjectPoolBase<T>, IComponentPool<T>, IPlayModeListener, ISerializationCallbackReceiver
        where T : UnityEngine.Component
    {
        private readonly Dictionary<ICallbacks, T> _objectsMap = new Dictionary<ICallbacks, T>();

        private static readonly Action<ICallbacks> DestroyedFunc = callbacks =>
        {
            if (!ApplicationExt.IsQuitting)
            {
                throw new Exception($"Pooled object can be destroyed only by pool ({((Object)callbacks).name})");
            }
        };

        private readonly Action<ICallbacks> _returnToPoolFunc;

#if ODIN_INSPECTOR
        [HideInEditorMode]
        [ShowInInspector]
        [ReadOnly]
#endif
        private int _instanced;

        private int _instantiated;

#if ODIN_INSPECTOR
        [HideInEditorMode]
        [ShowInInspector]
        [ReadOnly]
#endif
        private int _pooled;

        private IComponentPool<T> _componentPoolImplementation;

        [field: SerializeField] public T Prefab { get; private set; }
        [field: SerializeField] public Transform Parent { get; private set; }
        [field: SerializeField] public bool OnlyPoolCanDestroyObj { get; private set; }
        public int PrefabId { get; private set; }


        public T Get(Transform parent, bool setActive = true)
        {
            T obj = Get(setActive);
            obj.transform.SetParent(parent);
            return obj;
        }

        public T Get(bool setActive = true)
        {
            T obj;
            while (Stack.TryPop(out obj))
            {
                if (!OnlyPoolCanDestroyObj && !obj)
                    continue;

                if (setActive)
                    obj.gameObject.SetActive(true);

                PoolHelper.Instance.Remove(obj.transform);

                OnGot(obj);
                _instanced++;
                return obj;
            }

            obj = CreateObj(setActive);
            OnGot(obj);
            _instanced++;
            return obj;
        }

        public T Get(in Vector3 position, in Quaternion rotation, bool setActive = true)
        {
            T obj = Get(setActive);
            obj.transform.localRotation = rotation;
            obj.transform.localPosition = position;
            return obj;
        }

        public T Get(Transform parent, in Vector3 position, in Quaternion rotation, Space space = Space.World, bool setActive = true)
        {
            T obj = Get(parent, setActive);
            obj.transform.SetParent(parent, true);
            if (space == Space.World)
            {
                obj.transform.SetPositionAndRotation(position, rotation);
            }
            else
            {
                obj.transform.localRotation = rotation;
                obj.transform.localPosition = position;
            }

            return obj;
        }


        public T Get(in Vector3 position, bool setActive = true)
        {
            T obj = Get(setActive);
            obj.transform.localPosition = position;
            return obj;
        }

        public T Get(Transform parent, in Vector3 position, Space space = Space.World, bool setActive = true)
        {
            T obj = Get(parent, setActive);
            obj.transform.SetParent(parent);
            if (space == Space.World)
                obj.transform.position = position;
            else
                obj.transform.localPosition = position;
            return obj;
        }

        public ComponentPool(T prefab, Action<T> onGet = null, Action<T> onRelease = null, Action<T> actionOnDestroy = null, bool onlyPoolCanDestroyObj = true,
            bool collectionCheckDefault = DefaultCollectionCheck, int defaultCapacity = DefaultCapacity, int maxSize = DefaultMaxSize)
            : base(collectionCheckDefault, onGet, onRelease, actionOnDestroy, defaultCapacity, maxSize)
        {
            OnlyPoolCanDestroyObj = onlyPoolCanDestroyObj;
            Prefab = prefab;
            PrefabId = prefab.GetInstanceID();
            OnDestroy += Destroy;
            _returnToPoolFunc = ReturnToPool;
            this.PlayModeSubscribe();
        }

        protected ComponentPool()
        {
            _returnToPoolFunc = ReturnToPool;
        }

        ~ComponentPool()
        {
            if (!Application.isPlaying || ApplicationExt.IsQuitting)
                return;
            Clear();
        }

        public void ReturnToPool(ICallbacks callbacks)
        {
            if (ApplicationExt.IsQuitting)
                return;

            T obj = _objectsMap[callbacks];
            if (obj.gameObject.activeSelf)
                obj.gameObject.SetActive(false);

            //obj.transform.SetParent(Parent);
            PoolHelper.Instance.Add(obj.transform, Parent);

            Stack.Push(obj);
            _instanced--;
            _pooled++;
        }

        protected T CreateObj(bool setActive)
        {
            if (setActive)
                return GetNew();

            Prefab.gameObject.SetActive(false);
            T obj = GetNew();
            Prefab.gameObject.SetActive(true);
            return obj;
        }

        protected override T CreateNew()
        {
            Assertions.AssertPlayMode();
            T obj = Object.Instantiate(Prefab, Parent);
#if UNITY_EDITOR
            obj.name = obj.name.Replace("(Clone)", $" - {_instantiated.ToString()}");
#endif
            _instantiated++;
            AddToPool(obj);
            return obj;
        }

        public void AddToPool(T obj)
        {
            ICallbacks pooledObject = obj.GetCallbacks();
            pooledObject.OnDisabled += _returnToPoolFunc;
            if (OnlyPoolCanDestroyObj)
                pooledObject.OnDestroyed += DestroyedFunc;
            _objectsMap.Add(pooledObject, obj);
        }

        protected override void OnReleased(T element)
        {
            base.OnReleased(element);
            GameObject gameObj = element.gameObject;
            if (gameObj.activeSelf)
                gameObj.SetActive(false);
        }

        private void Destroy(T obj)
        {
            ICallbacks callbacks = obj.GetCallbacks();
            callbacks.OnDisabled -= _returnToPoolFunc;
            if (OnlyPoolCanDestroyObj)
                callbacks.OnDestroyed -= DestroyedFunc;
            _objectsMap.Remove(callbacks);
            Object.Destroy(obj);
        }

        bool IPlayModeListener.MainThreadOnly => true;

        void IPlayModeListener.OnEnterPlayMode()
        {
            OnEnterPlayMode();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            this.PlayModeSubscribe();
        }

        protected virtual void OnEnterPlayMode()
        {
            if (Parent)
                return;

            PrefabId = Prefab.GetInstanceID();
            Parent = new GameObject($"{typeof(T).Name} pool")
            {
                isStatic = true,
                transform = { parent = PersistentRoot.Get("ObjectPools").transform },
                hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor
            }.transform;
        }
    }
}