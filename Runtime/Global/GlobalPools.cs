using System.Collections.Generic;
using Dythervin.Core.Utils;
using UnityEngine;

namespace Dythervin.ObjectPool.Component.Global
{
    internal static class GlobalPools<T>
        where T : UnityEngine.Component
    {
        internal static readonly Dictionary<int, WeakReferenceT<IComponentPool<T>>> Pools = new Dictionary<int, WeakReferenceT<IComponentPool<T>>>();
    }

    public static partial class GlobalPools
    {
        private static readonly Dictionary<Object, Object> PoolObjToPrefab = new Dictionary<Object, Object>(1024);
        public static bool TryGetPool<T>(int prefabId, out IComponentPool<T> pool)
            where T : UnityEngine.Component
        {
            if (GlobalPools<T>.Pools.TryGetValue(prefabId, out var weakPtr) && weakPtr.TryGetTarget(out var poolBase))
            {
                pool = poolBase;
                return true;
            }

            pool = null;
            return false;
        }

        public static bool TryGetPool<T>(T prefab, out IComponentPool<T> pool)
            where T : UnityEngine.Component
        {
            return TryGetPool(prefab.GetInstanceID(), out pool);
        }

        internal static IComponentPool<T> GetPool<T>(this PrefabPooled<T> prefabPooled)
            where T : UnityEngine.Component
        {
            IComponentPool<T> pool;
            if (GlobalPools<T>.Pools.TryGetValue(prefabPooled.instanceId, out var weakPTr))
            {
                if (!weakPTr.TryGetTarget(out pool))
                {
                    pool = CreatePool(prefabPooled);
                    weakPTr.Target = pool;
                }
            }
            else
            {
                pool = CreatePool(prefabPooled);
                GlobalPools<T>.Pools[prefabPooled.instanceId] = new WeakReferenceT<IComponentPool<T>>(pool);
            }

            return pool;
        }

        private static ComponentPool<T> CreatePool<T>(PrefabPooled<T> prefabPooled)
            where T : UnityEngine.Component
        {
            var pool = new GlobalComponentPool<T>(prefabPooled.Prefab, collectionCheckDefault: false);
            pool.OnInstantiated += (componentPool, component) => PoolObjToPrefab.Add(component, componentPool.Prefab);
            pool.OnDestroyed += (componentPool, component) => PoolObjToPrefab.Remove(component);
            return pool;
        }
    }
}