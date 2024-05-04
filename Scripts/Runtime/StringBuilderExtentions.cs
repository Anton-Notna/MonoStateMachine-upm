using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MonoStateMachine
{
    public static class StringBuilderExtentions
    {
        public static void FillByHierarchy(this StringBuilder stringBuilder, Transform transform)
        {
            List<Transform> hierarchy = new List<Transform>()
            {
                transform,
            };

            while (hierarchy[hierarchy.Count - 1].parent != null)
                hierarchy.Add(hierarchy[hierarchy.Count - 1].parent);

            for (int i = hierarchy.Count - 1; i >= 0; i--)
            {
                stringBuilder.Append(hierarchy[i].name);
                stringBuilder.Append('/');
            }

        }
    }
}