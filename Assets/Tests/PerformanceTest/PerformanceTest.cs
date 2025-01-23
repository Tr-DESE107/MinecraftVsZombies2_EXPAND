using System.Collections.Generic;
using MVZ2.GameContent.Contraptions;
using MVZ2Logic.Entities;
using PVZEngine;
using PVZEngine.Base;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.Tests
{
    public class PerformanceTest : MonoBehaviour
    {
        private void Start()
        {
            var def = new MetaEntityDefinition(EntityTypes.PLANT, "mvz2", "dispenser");
            def.AddBehaviour(new Dispenser("mvz2", "dispenser"));
            var container = new TestPropertyContainer(def);
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
            for (int i = 0; i < 1000; i++)
            {
                props.GetProperty<int>("int");
            }
            //Assert.AreEqual("123", props.GetProperty<string>("string"));
            //Assert.AreEqual(vector3, props.GetProperty<Vector3>("vector3"));
            //props.GetProperty<NamespaceID>("namespaceID");
        }
        ModifiableProperties props;

        private class TestPropertyContainer : IPropertyModifyTarget
        {
            private EntityDefinition definition;
            public TestPropertyContainer(EntityDefinition definition)
            {
                this.definition = definition;
            }
            public bool GetFallbackProperty(string name, out object value)
            {
                if (definition == null)
                {
                    value = null;
                    return false;
                }
                if (definition.TryGetProperty(name, out var defProp))
                {
                    value = defProp;
                    return true;
                }

                var behaviourCount = definition.GetBehaviourCount();
                for (int i = 0; i < behaviourCount; i++)
                {
                    var behaviour = definition.GetBehaviourAt(i);
                    if (behaviour.TryGetProperty(name, out var behProp))
                    {
                        value = behProp;
                        return true;
                    }
                }
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
