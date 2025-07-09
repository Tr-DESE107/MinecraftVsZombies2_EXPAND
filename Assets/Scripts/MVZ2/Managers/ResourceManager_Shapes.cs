﻿using System;
using System.Linq;
using MVZ2.Metas;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        public ShapeMetaList GetShapeMetaList(string nsp)
        {
            var modResource = main.ResourceManager.GetModResource(nsp);
            if (modResource == null)
                return null;
            return modResource.ShapeMetaList;
        }
        public ShapeMeta GetShapeMeta(NamespaceID id)
        {
            if (!NamespaceID.IsValid(id))
                return null;
            var list = GetShapeMetaList(id.SpaceName);
            if (list == null)
                return null;
            return list.metas.FirstOrDefault(m => m.ID == id.Path);
        }
        public ShapeMeta[] GetModShapeMetas(string nsp)
        {
            var metaList = GetShapeMetaList(nsp);
            if (metaList == null)
                return Array.Empty<ShapeMeta>();
            return metaList.metas.ToArray();
        }
    }
}
