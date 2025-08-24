using System;
using System.Collections.Generic;
using System.Xml;
using MVZ2.IO;
using MVZ2.Models;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Metas
{
    public class ModelMeta
    {
        public string Name { get; private set; }
        public string Type { get; private set; }
        public NamespaceID Path { get; private set; }
        public bool Shot { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public float XOffset { get; private set; }
        public float YOffset { get; private set; }
        public AnimatorParameter[] AnimatorParameters { get; private set; }
        public static ModelMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var name = node.GetAttribute("name");
            var type = node.GetAttribute("type");
            var path = node.GetAttribute("path");
            var shot = node.GetAttributeBool("shot") ?? true;
            var width = node.GetAttributeInt("width") ?? 64;
            var height = node.GetAttributeInt("height") ?? 64;
            var xOffset = node.GetAttributeFloat("xOffset") ?? 0;
            var yOffset = node.GetAttributeFloat("yOffset") ?? 0;
            var animatorParameters = new List<AnimatorParameter>();
            var animatorNode = node["animator"];
            if (animatorNode != null)
            {
                var parametersNode = animatorNode["parameters"];
                if (parametersNode != null)
                {
                    for (int i = 0; i < parametersNode.ChildNodes.Count; i++)
                    {
                        var child = parametersNode.ChildNodes[i];
                        animatorParameters.Add(AnimatorParameter.FromXmlNode(child, defaultNsp));
                    }
                }
            }
            return new ModelMeta()
            {
                Name = name,
                Type = type,
                Shot = shot,
                Path = NamespaceID.Parse(path, defaultNsp),
                Width = width,
                Height = height,
                XOffset = xOffset,
                YOffset = yOffset,
                AnimatorParameters = animatorParameters.ToArray()
            };
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public class AnimatorParameter
    {
        public string Name { get; set; }
        public AnimatorControllerParameterType Type { get; set; }
        public bool BoolValue { get; set; }
        public int IntValue { get; set; }
        public float FloatValue { get; set; }
        public static AnimatorParameter FromXmlNode(XmlNode node, string defaultNsp)
        {
            var typeName = node.Name;
            var name = node.GetAttribute("name");
            switch (typeName)
            {
                case "trigger":
                    return new AnimatorParameter()
                    {
                        Name = name,
                        Type = AnimatorControllerParameterType.Trigger,
                    };
                case "bool":
                    return new AnimatorParameter()
                    {
                        Name = name,
                        Type = AnimatorControllerParameterType.Bool,
                        BoolValue = node.GetAttributeBool("value") ?? false
                    };
                case "int":
                    return new AnimatorParameter()
                    {
                        Name = name,
                        Type = AnimatorControllerParameterType.Int,
                        IntValue = node.GetAttributeInt("value") ?? 0
                    };
                case "float":
                    return new AnimatorParameter()
                    {
                        Name = name,
                        Type = AnimatorControllerParameterType.Float,
                        FloatValue = node.GetAttributeFloat("value") ?? 0
                    };
            }
            throw new InvalidOperationException($"Could not create an AnimatorParameter with type {typeName}.");
        }
        public void Apply(Model model)
        {
            switch (Type)
            {
                case AnimatorControllerParameterType.Trigger:
                    model.TriggerAnimator(Name);
                    break;
                case AnimatorControllerParameterType.Bool:
                    model.SetAnimatorBool(Name, BoolValue);
                    break;
                case AnimatorControllerParameterType.Int:
                    model.SetAnimatorInt(Name, IntValue);
                    break;
                case AnimatorControllerParameterType.Float:
                    model.SetAnimatorFloat(Name, FloatValue);
                    break;
            }
        }
    }
}
