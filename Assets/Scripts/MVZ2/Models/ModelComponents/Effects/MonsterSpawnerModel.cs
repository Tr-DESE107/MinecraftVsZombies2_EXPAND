﻿using PVZEngine;
using UnityEngine;

namespace MVZ2.Models
{
    public class MonsterSpawnerModel : ModelComponent
    {
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);
            var currentID = Model.GetProperty<NamespaceID>("EntityToSpawn");
            if (entityID != currentID)
            {
                entityID = currentID;

                var sprite = Main.ResourceManager.GetDefaultSprite();
                var def = Main.Game.GetEntityDefinition(entityID);
                if (def != null)
                {
                    var modelID = def.GetModelID();
                    sprite = Main.ResourceManager.GetModelIcon(modelID);
                }
                iconRenderer.sprite = sprite;
            }
        }
        [SerializeField]
        private SpriteRenderer iconRenderer;
        private NamespaceID entityID;
    }
}
