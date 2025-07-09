﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MVZ2.IO;
using MVZ2Logic;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Metas
{
    public class TalkCharacterMetaList
    {
        public List<TalkCharacterMeta> metas = new List<TalkCharacterMeta>();
        public static TalkCharacterMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var list = new TalkCharacterMetaList();
            var metaChildNodes = node.ChildNodes;
            for (int i = 0; i < metaChildNodes.Count; i++)
            {
                var child = metaChildNodes[i];
                list.metas.Add(TalkCharacterMeta.FromXmlNode(child, defaultNsp));
            }
            return list;
        }
    }

    public class TalkCharacterMeta
    {
        public string id;
        public string name;
        public NamespaceID unlockCondition;
        public List<TalkCharacterVariant> variants = new List<TalkCharacterVariant>();

        public TalkCharacterVariant GetVariant(NamespaceID id)
        {
            return variants.FirstOrDefault(v => v.id == id);
        }
        public static TalkCharacterMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var meta = new TalkCharacterMeta();
            meta.id = node.GetAttribute("id");
            meta.name = node.GetAttribute("name");
            meta.unlockCondition = node.GetAttributeNamespaceID("unlock", defaultNsp);

            var variantTemplates = new List<TalkCharacterVariantTemplate>();
            var variantChildNodes = node.ChildNodes;
            for (int i = 0; i < variantChildNodes.Count; i++)
            {
                var child = variantChildNodes[i];
                if (child.Name == "variant")
                {
                    variantTemplates.Add(TalkCharacterVariantTemplate.FromXmlNode(child, defaultNsp));
                }
            }
            meta.variants.AddRange(variantTemplates.Select(t => t.ToVariant(variantTemplates)));
            return meta;
        }
    }

    public class TalkCharacterVariantTemplate
    {
        public NamespaceID id;
        public NamespaceID parent;
        public NamespaceID unlock;
        public int? width;
        public int? height;
        public float? pivotX;
        public float? pivotY;
        public float? extendLeft;
        public float? extendRight;
        public List<TalkCharacterLayer> layers = new List<TalkCharacterLayer>();
        public static TalkCharacterVariantTemplate FromXmlNode(XmlNode node, string defaultNsp)
        {
            var variant = new TalkCharacterVariantTemplate();
            variant.id = node.GetAttributeNamespaceID("id", defaultNsp);
            variant.parent = node.GetAttributeNamespaceID("parent", defaultNsp);
            variant.unlock = node.GetAttributeNamespaceID("unlock", defaultNsp);

            variant.width = node.GetAttributeInt("width");
            variant.height = node.GetAttributeInt("height");
            variant.extendLeft = node.GetAttributeFloat("extendLeft");
            variant.extendRight = node.GetAttributeFloat("extendRight");
            variant.pivotX = node.GetAttributeFloat("pivotX");
            variant.pivotY = node.GetAttributeFloat("pivotY");

            var variantChildNodes = node.ChildNodes;
            for (int i = 0; i < variantChildNodes.Count; i++)
            {
                var child = variantChildNodes[i];
                variant.layers.Add(TalkCharacterLayer.FromXmlNode(child, defaultNsp));
            }
            return variant;
        }
        public TalkCharacterVariant ToVariant(IEnumerable<TalkCharacterVariantTemplate> templates)
        {
            var result = new TalkCharacterVariant();
            var visited = new Stack<TalkCharacterVariantTemplate>();
            GetCharacterVariantProperties(templates, result, visited);
            result.unlock = unlock;
            return result;
        }
        private void GetCharacterVariantProperties(IEnumerable<TalkCharacterVariantTemplate> templatePool, TalkCharacterVariant result, Stack<TalkCharacterVariantTemplate> visited)
        {
            if (visited.Contains(this))
                throw new InvalidOperationException($"A recursion exception has been occured while loading talk character {id} from talkcharacter.xml, maybe a cycle parent reference is present.");
            visited.Push(this);
            var parentID = parent;
            if (NamespaceID.IsValid(parentID))
            {
                var parent = templatePool.FirstOrDefault(t => t.id == parentID);
                if (parent != null)
                {
                    parent.GetCharacterVariantProperties(templatePool, result, visited);
                }
            }
            visited.Pop();
            result.id = id;
            result.pivotX = pivotX ?? result.pivotX;
            result.pivotY = pivotY ?? result.pivotY;
            result.width = width ?? result.width;
            result.height = height ?? result.height;
            result.widthExtend = new Vector2(extendLeft ?? result.widthExtend.x, extendRight ?? result.widthExtend.y);
            result.layers.AddRange(layers);
        }
    }
    public class TalkCharacterVariant
    {
        public NamespaceID id;
        public NamespaceID unlock;
        public int width;
        public int height;
        public float pivotX = 0.5f;
        public float pivotY = 0.5f;
        public Vector2 widthExtend;
        public List<TalkCharacterLayer> layers = new List<TalkCharacterLayer>();
    }

    public class TalkCharacterLayer
    {
        public SpriteReference sprite;
        public int positionX;
        public int positionY;
        public static TalkCharacterLayer FromXmlNode(XmlNode node, string defaultNsp)
        {
            var layer = new TalkCharacterLayer();
            layer.sprite = node.GetAttributeSpriteReference("sprite", defaultNsp);
            layer.positionX = node.GetAttributeInt("positionX") ?? 0;
            layer.positionY = node.GetAttributeInt("positionY") ?? 0;
            return layer;
        }
    }
}
