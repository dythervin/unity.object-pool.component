using System;
using System.Collections.Generic;
using Dythervin.Collections;
using Dythervin.Core.Utils;
using Dythervin.Updater;
using Object = UnityEngine.Object;

namespace Dythervin.ObjectPool.Component
{
    public class DelayedDestroyer : Singleton<DelayedDestroyer>, IUpdatable
    {
        private readonly LockableCollection<HashSet<Object>, Object> _values = new LockableHashSet<Object>();
        private readonly Dictionary<int, int> _frames = new Dictionary<int, int>();

        public void Destroy(Object obj, int frames = 1)
        {
            if (frames <= 0)
                throw new ArgumentOutOfRangeException($"{nameof(frames)} must be more than 0");

            _values.Add(obj);
            _frames.Add(obj.GetInstanceID(), frames);
            this.SetUpdater(true);
        }

        void IUpdatable.OnUpdate()
        {
            _values.Lock();
            foreach (Object obj in _values.values)
            {
                int key = obj.GetInstanceID();
                int frame = _frames[key] - 1;
                if (frame <= 0)
                {
                    Object.Destroy(obj);
                    _values.Remove(obj);
                    _frames.Remove(key);
                }

                else
                {
                    _frames[key] = frame;
                }
            }
            
            _values.Unlock();
            if (_values.Count == 0)
                this.SetUpdater(false);
        }
    }
}