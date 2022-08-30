using UnityEngine;

namespace Dythervin.ObjectPool.Component
{
    public interface IComponentPool : IObjectPool { }

    public interface IComponentPool<out T> : IComponentPool, IObjectPoolOut<T>
        where T : UnityEngine.Component
    {
        T Get(Transform parent, bool setActive = true);
        T Get(bool setActive = true);
        T Get(in Vector3 position, in Quaternion rotation, bool setActive = true, Space space = Space.World);
        T Get(in Vector3 position, in Quaternion rotation, Transform parent, bool setActive = true, Space space = Space.World);
        T Get(in Vector3 position, bool setActive = true, Space space = Space.World);
        T Get(in Vector3 position, in Transform parent, bool setActive = true, Space space = Space.World);
    }
}