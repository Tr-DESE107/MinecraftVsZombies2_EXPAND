using System.Collections.Generic;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Grids;
using MVZ2Logic.Modding;
using PVZEngine;
using PVZEngine.Base;

namespace MVZ2.Games
{
    public partial class Game
    {
        public T GetDefinition<T>(string type, NamespaceID id) where T : Definition
        {
            return definitionGroup.GetDefinition<T>(type, id);
        }
        public T[] GetDefinitions<T>(string type) where T : Definition
        {
            return definitionGroup.GetDefinitions<T>(type);
        }
        public Definition[] GetDefinitions()
        {
            return definitionGroup.GetDefinitions();
        }
        public int GetGridLayerPriority(NamespaceID layer)
        {
            if (layer == VanillaGridLayers.protector)
            {
                return 100;
            }
            else if (layer == VanillaGridLayers.carrier)
            {
                return -100;
            }
            else
            {
                return 0;
            }
        }
        public string GetGridErrorMessage(NamespaceID error)
        {
            if (error == null)
                return null;
            if (gridErrorMessages.TryGetValue(error, out var message))
            {
                return message;
            }
            return null;
        }
        public void AddMod(IModLogic mod)
        {
            foreach (var def in mod.GetDefinitions())
            {
                definitionGroup.Add(def);
            }
        }
        private DefinitionGroup definitionGroup = new DefinitionGroup();
        private static readonly Dictionary<NamespaceID, string> gridErrorMessages = new Dictionary<NamespaceID, string>()
        {
            { VanillaGridStatus.needLilypad, VanillaStrings.ADVICE_PLACE_LILYPAD_FIRST },
            { VanillaGridStatus.notOnWater, VanillaStrings.ADVICE_CANNOT_PLACE_ON_WATER },
            { VanillaGridStatus.notOnLand, VanillaStrings.ADVICE_CANNOT_PLACE_ON_LAND },
            { VanillaGridStatus.notOnPlane, VanillaStrings.ADVICE_CANNOT_PLACE_ON_PLANE },
            { VanillaGridStatus.notOnStatues, VanillaStrings.ADVICE_CANNOT_PLACE_ON_STATUES },
            { VanillaGridStatus.onlyCanSleep, VanillaStrings.ADVICE_ONLY_PLACE_ON_CAN_SLEEP },
            { VanillaGridStatus.onlyUpgrade, VanillaStrings.ADVICE_ONLY_UPGRADE },
        };
    }
}