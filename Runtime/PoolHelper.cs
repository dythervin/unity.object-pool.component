using System.Collections.Generic;
using Dythervin.Core.Utils;
using Dythervin.Updater;
using UnityEngine;

namespace Dythervin.ObjectPool.Component
{
    public class PoolHelper : Singleton<PoolHelper>, IUpdatable
    {
        private readonly Dictionary<Transform, Transform> objectToParents = new Dictionary<Transform, Transform>();

        public void Add(Transform component, Transform parent)
        {
            objectToParents.Add(component, parent);
            this.SetUpdater(true);
        }

        public void Remove(Transform component)
        {
            objectToParents.Remove(component);
        }

        void IUpdatable.OnUpdate()
        {
            foreach (var pair in objectToParents)
                pair.Key.SetParent(pair.Value);

            objectToParents.Clear();
            this.SetUpdater(false);
        }
    }
}