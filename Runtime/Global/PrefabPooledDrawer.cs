#if UNITY_EDITOR
using Dythervin.Core.Editor.Drawers;
using UnityEditor;

namespace Dythervin.ObjectPool.Component.Global
{
    [CustomPropertyDrawer(typeof(PrefabPooled<>))]
    public class PrefabPooledDrawer : SimpleGenericDrawer { }
}
#endif