using System.Collections.Generic;
using Dythervin.Core.Utils;
using Dythervin.Updaters;
using UnityEngine;

namespace Dythervin.ObjectPool.Component
{
    public class PoolHelper : Singleton<PoolHelper>, IUpdatable
    {
        private readonly Dictionary<Transform, (Transform parent, Vector3 scale)> _objectToParents =
            new Dictionary<Transform, (Transform parent, Vector3 scale)>();

        public void Add(Transform component, Transform parent, in Vector3 defaultScale)
        {
            _objectToParents.Add(component, (parent, defaultScale));
            this.SetUpdater(true);
        }

        public void Remove(Transform component)
        {
            _objectToParents.Remove(component);
        }

        void IUpdatable.OnUpdate()
        {
            foreach (var pair in _objectToParents)
            {
                pair.Key.SetParent(pair.Value.parent);
                pair.Key.localScale = pair.Value.scale;
            }

            _objectToParents.Clear();
            this.SetUpdater(false);
        }
    }
}