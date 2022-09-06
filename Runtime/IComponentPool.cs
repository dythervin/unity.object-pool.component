using UnityEngine;

namespace Dythervin.ObjectPool.Component
{
    public interface IComponentPool : IObjectPool { }

    public interface IComponentPool<out T> : IComponentPool, IObjectPoolOut<T>
        where T : UnityEngine.Component
    {
        T Get(Transform parent, bool setActive = true);
        T Get(bool setActive = true);
        T Get(in Vector3 position, in Quaternion rotation, bool setActive = true);
        T Get(Transform parent, in Vector3 position, in Quaternion rotation, Space space = Space.World, bool setActive = true);
        T Get(in Vector3 position, bool setActive = true);
        T Get(Transform parent, in Vector3 position, Space space = Space.World, bool setActive = true);
    }
}