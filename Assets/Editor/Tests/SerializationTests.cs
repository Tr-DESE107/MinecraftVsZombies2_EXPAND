using System.Collections.Generic;
using MVZ2Logic;
using NUnit.Framework;
using PVZEngine;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using Tools;
using UnityEngine;

namespace MVZ2.EditorTests
{
    public class SerializationTests
    {
        [Test]
        public static void RNGTest()
        {
            SerializeHelper.init("mvz2");

            var rng = new RandomGenerator(1234);

            var json = rng.ToBson();
            var seri = SerializeHelper.FromBson<RandomGenerator>(json);
            var json2 = seri.ToBson();

            Debug.Log(json);
            Debug.Log(json2);
            Assert.AreEqual(json, json2);

            rng.Next();
            rng.Next();
            rng.Next();
            var json3 = rng.ToBson();
            seri.Next();
            seri.Next();
            seri.Next();
            var json4 = seri.ToBson();
            Debug.Log(json3);
            Debug.Log(json4);
            Assert.AreEqual(json3, json4);
        }
        [Test]
        public static void PropertyBlockTest()
        {
            SerializeHelper.init("mvz2");

            var modifyTarget = new TestModifyTarget();
            var propertyBlock = new PropertyBlock(modifyTarget);

            propertyBlock.SetProperty("int", 58);
            propertyBlock.SetProperty("float", 58f);
            propertyBlock.SetProperty("long", 58);
            propertyBlock.SetProperty("string", "58");
            propertyBlock.SetProperty("vector3", new Vector3(1,2,3));
            propertyBlock.SetProperty("namespaceID", new NamespaceID("mvz2", "value"));
            propertyBlock.SetProperty("rng", new RandomGenerator(1234));
            propertyBlock.SetProperty("shake", new ShakeInt(30, 0, 30));

            propertyBlock.SetField("basic", "int", 58);
            propertyBlock.SetField("basic", "float", 58f);
            propertyBlock.SetField("basic", "long", 58);
            propertyBlock.SetField("basic", "string", "58");
            propertyBlock.SetField("unity", "vector3", new Vector3(1, 2, 3));
            propertyBlock.SetField("class", "namespaceID", new NamespaceID("mvz2", "value"));
            propertyBlock.SetField("class", "rng", new RandomGenerator(1234));
            propertyBlock.SetField("class", "shake", new ShakeInt(30, 0, 30));

            var seri = propertyBlock.ToSerializable();
            var beforeBson = seri.ToBson();

            Debug.Log(beforeBson);

            var afterBlock = SerializeHelper.FromBson<SerializablePropertyBlock>(beforeBson);
            var afterBson = afterBlock.ToBson();
            Debug.Log(afterBson);

            Assert.AreEqual(beforeBson, afterBson);
        }
        private class TestModifyTarget : IPropertyModifyTarget
        {
            public bool GetFallbackProperty(string name, out object value)
            {
                value = null;
                return false;
            }

            public void GetModifierItems(string name, List<ModifierContainerItem> results)
            {
            }

            public void UpdateModifiedProperty(string name, object value)
            {
            }

            public PropertyModifier[] GetModifiersUsingProperty(string name)
            {
                return null;
            }

            public IEnumerable<string> GetModifiedProperties()
            {
                yield break;
            }
        }
    }
}
