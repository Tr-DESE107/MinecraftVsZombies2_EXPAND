﻿using System;
using MVZ2.Vanilla;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Level.Components;
using PVZEngine;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Level.Components
{
    public class ArtifactComponent : MVZ2Component, IArtifactComponent
    {
        public ArtifactComponent(LevelEngine level, LevelController controller) : base(level, componentID, controller)
        {
            artifacts = new ArtifactList(level, 3);
            artifacts.OnArtifactHighlighted += OnArtifactHighlighted;
        }
        public override void OnStart()
        {
            base.OnStart();
            UpdateUIArtifacts();
        }
        public void SetSlotCount(int count)
        {
            artifacts.SetSlotCount(count);
            var uiPreset = Controller.GetUIPreset();
            uiPreset.SetArtifactCount(count);
        }
        public int GetSlotCount()
        {
            return artifacts.GetSlotCount();
        }
        public void ReplaceArtifacts(ArtifactDefinition[] artifactDef)
        {
            artifacts.ReplaceArtifacts(artifactDef);
        }
        public Artifact[] GetArtifacts()
        {
            return artifacts.GetAllArtifacts();
        }
        public bool HasArtifact(ArtifactDefinition artifactDef)
        {
            return artifacts.HasArtifact(artifactDef);
        }

        public int GetArtifactIndex(ArtifactDefinition artifactDef)
        {
            return artifacts.GetArtifactIndex(artifactDef);
        }
        public bool HasArtifact(NamespaceID artifactID)
        {
            return artifacts.HasArtifact(artifactID);
        }

        public int GetArtifactIndex(NamespaceID artifactID)
        {
            return artifacts.GetArtifactIndex(artifactID);
        }
        public Artifact GetArtifactAt(int index)
        {
            return artifacts.GetArtifactAt(index);
        }



        public override void Update()
        {
            base.Update();
            artifacts.Update();
        }
        public override void UpdateFrame(float deltaTime, float simulationSpeed)
        {
            base.UpdateFrame(deltaTime, simulationSpeed);
            UpdateUIArtifacts();
        }
        public override ISerializableLevelComponent ToSerializable()
        {
            return new SerializableArtifactComponent()
            {
                artifacts = artifacts.ToSerializable()
            };
        }
        public override void LoadSerializable(ISerializableLevelComponent seri)
        {
            if (seri is not SerializableArtifactComponent comp)
                return;
            artifacts = ArtifactList.FromSerializable(comp.artifacts, Level);
            artifacts.OnArtifactHighlighted += OnArtifactHighlighted;
            var uiPreset = Controller.GetUIPreset();
            uiPreset.SetArtifactCount(artifacts.GetSlotCount());
            UpdateUIArtifacts();
        }
        private void UpdateUIArtifacts()
        {
            var uiPreset = Controller.GetUIPreset();
            var count = artifacts.GetSlotCount();
            for (int i = 0; i < count; i++)
            {
                var artifact = artifacts.GetArtifactAt(i);
                Sprite icon = null;
                string numberText = string.Empty;
                bool grayscale = false;
                bool glowing = false;
                if (artifact != null)
                {
                    icon = Main.GetFinalSprite(artifact.Definition.GetSpriteReference());
                    var number = artifact.GetNumber();
                    numberText = number < 0 ? string.Empty : number.ToString();
                    grayscale = artifact.IsInactive();
                    glowing = artifact.GetGlowing();
                }
                uiPreset.SetArtifactIcon(i, icon);
                uiPreset.SetArtifactNumber(i, numberText);
                uiPreset.SetArtifactGrayscale(i, grayscale);
                uiPreset.SetArtifactGlowing(i, glowing);
            }
        }
        private void OnArtifactHighlighted(int index)
        {
            var uiPreset = Controller.GetUIPreset();
            uiPreset.HighlightArtifact(index);
        }
        private ArtifactList artifacts;
        public static readonly NamespaceID componentID = new NamespaceID(VanillaMod.spaceName, "artifact");
    }
    [Serializable]
    public class SerializableArtifactComponent : ISerializableLevelComponent
    {
        public SerializableArtifactList artifacts;
    }
}
