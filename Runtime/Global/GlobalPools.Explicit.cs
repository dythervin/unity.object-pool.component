using Dythervin.Core;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

namespace Dythervin.ObjectPool.Component.Global
{
    public static partial class GlobalPools
    {
        public static T InstantiatePooled<T>(T original)
            where T : UnityEngine.Component
        {
            return TryGetPool(original, out var pool) ? pool.Get() : Object.Instantiate(original);
        }

        public static T InstantiatePooled<T>(T original, in Vector3 position, in Quaternion rotation)
            where T : UnityEngine.Component
        {
            return TryGetPool(original, out var pool) ? pool.Get(position, rotation) : Object.Instantiate(original, position, rotation);
        }

        public static T InstantiatePooled<T>(T original, Vector3 position, Quaternion rotation, Transform parent)
            where T : UnityEngine.Component
        {
            return TryGetPool(original, out var pool) ? pool.Get(parent, position, rotation) : Object.Instantiate(original, position, rotation, parent);
        }

        public static T InstantiatePooled<T>(T original, Transform parent)
            where T : UnityEngine.Component
        {
            return TryGetPool(original, out var pool) ? pool.Get(parent) : Object.Instantiate(original, parent, false);
        }

        public static void ReleaseOrDestroy([NotNull] GameObject obj)
        {
            Assert.IsNotNull(obj);
            if (PoolObjToPrefab.ContainsKey(obj.GetInstanceID()))
            {
                obj.SetActive(false);
            }
            else
            {
#if UNITY_EDITOR
                if (!ApplicationExt.IsPlaying)
                {
                    Object.DestroyImmediate(obj);
                    return;
                }
#endif
                Object.Destroy(obj);
            }
        }
        public static void ReleaseOrDestroy<T>([NotNull] T obj)
            where T : UnityEngine.Component
        {
            ReleaseOrDestroy(obj.gameObject);
        }
    }
}