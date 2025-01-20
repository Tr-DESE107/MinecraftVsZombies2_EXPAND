using System.Collections.Generic;
using PVZEngine;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.Tests
{
    public class PerformanceTest : MonoBehaviour
    {
        private void Start()
        {
            var container = new TestPropertyContainer();
            props = new ModifiableProperties(container);

            props.SetProperty("int", 123);
            //props.SetProperty("string", "123");
            //var vector3 = new Vector3(1, 1, 1);
            //props.SetProperty("vector3", new Vector3(1, 1, 1));
            //var namespaceID = new NamespaceID("mvz2", "test");
            //props.SetProperty("namespaceID", namespaceID);
        }
        public void Update()
        {
            props.GetProperty<int>("int");
            //Assert.AreEqual("123", props.GetProperty<string>("string"));
            //Assert.AreEqual(vector3, props.GetProperty<Vector3>("vector3"));
            //props.GetProperty<NamespaceID>("namespaceID");
        }
        ModifiableProperties props;

        private class TestPropertyContainer : IPropertyModifyTarget
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
