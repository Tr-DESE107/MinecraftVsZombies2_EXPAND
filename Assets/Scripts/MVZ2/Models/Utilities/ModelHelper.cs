﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MVZ2.Models
{
    public static class ModelHelper
    {
        public static void ReplaceList<T>(this List<T> list, IEnumerable<T> targets)
        {
            list.RemoveAll(e => !targets.Contains(e));
            list.AddRange(targets.Except(list));
        }
        public static bool IsDirectChild<T>(this Component child, T group) where T : Component
        {
            return child.GetComponentInParent<T>() == group;
        }
    }
}
