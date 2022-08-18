using Dythervin.Core;
using UnityEngine;

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
            return TryGetPool(original, out var pool) ? pool.Get(position, rotation, parent) : Object.Instantiate(original, position, rotation, parent);
        }

        public static T InstantiatePooled<T>(T original, Transform parent)
            where T : UnityEngine.Component
        {
            return TryGetPool(original, out var pool) ? pool.Get(parent) : Object.Instantiate(original, parent, false);
        }

        public static void ReleaseOrDestroy<T>(T obj)
            where T : UnityEngine.Component
        {
            if (GlobalPools<T>.PoolObjToPrefab.ContainsKey(obj.GetInstanceID()))
            {
                obj.gameObject.SetActive(false);
            }
            else
            {
#if UNITY_EDITOR
                if (!ApplicationExt.IsPlaying)
                {
                    Object.DestroyImmediate(obj.gameObject);
                    return;
                }
#endif
                Object.Destroy(obj.gameObject);
            }
        }

        public static void DisabledAndDestroy(GameObject obj)
        {
            obj.SetActive(false);
            DelayedDestroyer.Instance.Destroy(obj, 3);
        }
    }
}