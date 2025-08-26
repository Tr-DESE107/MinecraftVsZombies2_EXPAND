﻿using System;
using System.Linq;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2Logic.Artifacts
{
    public class ArtifactList
    {
        public ArtifactList(LevelEngine level, int count) : this(level)
        {
            artifacts = new Artifact[count];
        }
        private ArtifactList(LevelEngine level)
        {
            Level = level;
        }
        #region 制品操作
        public void SetSlotCount(int count)
        {
            var newArray = new Artifact[count];
            if (artifacts != null)
            {
                for (int i = 0; i < Math.Min(count, artifacts.Length); i++)
                {
                    newArray[i] = artifacts[i];
                }
            }
            artifacts = newArray;
        }
        public int GetSlotCount()
        {
            return artifacts.Length;
        }
        public void ReplaceArtifacts(ArtifactDefinition[] definitions)
        {
            if (definitions == null)
            {
                // 设置为空
                // 移除列表中的所有制品。
                for (int i = 0; i < artifacts.Length; i++)
                {
                    SetArtifact(i, null);
                }
            }
            else
            {
                // 设置不为空。
                var oldArtifacts = artifacts.ToList();
                for (int i = 0; i < artifacts.Length; i++)
                {
                    if (i < 0 || i >= definitions.Length)
                    {
                        artifacts[i] = null;
                        continue;
                    }
                    var definition = definitions[i];
                    if (definition == null)
                    {
                        artifacts[i] = null;
                        continue;
                    }
                    // 在当前制品列表中寻找匹配定义的制品。
                    // 若找到了则直接搬过来。
                    // 若没找到则新建。
                    var oldArtifact = oldArtifacts.FirstOrDefault(a => a?.Definition == definition);
                    if (oldArtifact != null)
                    {
                        artifacts[i] = oldArtifact;
                        oldArtifacts.Remove(oldArtifact);
                    }
                    else
                    {
                        var newArtifact = new Artifact(Level, definition);
                        artifacts[i] = newArtifact;
                        newArtifact.PostAdd();
                        newArtifact.OnHighlighted += OnItemHighlightedCallback;
                    }
                }
                // 之前制品列表中仍残留的制品被去除。
                foreach (var oldArtifact in oldArtifacts)
                {
                    if (oldArtifact == null)
                        continue;
                    oldArtifact.PostRemove();
                    oldArtifact.OnHighlighted -= OnItemHighlightedCallback;
                }
            }
        }
        public void ReplaceArtifact(int slot, ArtifactDefinition definition)
        {
            var newArtifact = new Artifact(Level, definition);
            SetArtifact(slot, newArtifact);
        }
        public void SetArtifact(int slot, Artifact newArtifact)
        {
            if (slot < 0 || slot >= artifacts.Length)
                return;
            var oldArtifact = artifacts[slot];
            if (oldArtifact != null)
            {
                oldArtifact.PostRemove();
                oldArtifact.OnHighlighted -= OnItemHighlightedCallback;
            }
            artifacts[slot] = newArtifact;
            if (newArtifact != null)
            {
                newArtifact.PostAdd();
                newArtifact.OnHighlighted += OnItemHighlightedCallback;
            }
        }
        public bool HasArtifact<T>() where T : ArtifactDefinition
        {
            return artifacts.Any(b => b != null && b.Definition is T);
        }
        public bool HasArtifact(ArtifactDefinition artifactDef)
        {
            return artifacts.Any(b => b != null && b.Definition == artifactDef);
        }
        public bool HasArtifact(NamespaceID id)
        {
            return artifacts.Any(b => b != null && b.Definition?.GetID() == id);
        }
        public bool HasArtifact(Artifact artifact)
        {
            return artifacts.Contains(artifact);
        }
        public Artifact[] GetArtifacts<T>() where T : ArtifactDefinition
        {
            return artifacts.Where(b => b != null && b.Definition is T).ToArray();
        }
        public Artifact[] GetArtifacts(ArtifactDefinition buffDef)
        {
            return artifacts.Where(b => b != null && b.Definition == buffDef).ToArray();
        }
        public int GetArtifactIndex(Artifact artifact)
        {
            return Array.IndexOf(artifacts, artifact);
        }
        public int GetArtifactIndex(ArtifactDefinition def)
        {
            return Array.FindIndex(artifacts, a => a != null && a.Definition == def);
        }
        public int GetArtifactIndex(NamespaceID id)
        {
            return Array.FindIndex(artifacts, a => a != null && a.Definition?.GetID() == id);
        }
        public Artifact GetArtifactAt(int index)
        {
            if (index < 0 || index >= artifacts.Length)
                return null;
            return artifacts[index];
        }
        public Artifact[] GetAllArtifacts()
        {
            return artifacts.ToArray();
        }
        #endregion

        #region 序列化
        public SerializableArtifactList ToSerializable()
        {
            return new SerializableArtifactList()
            {
                artifacts = artifacts == null ? Array.Empty<SerializableArtifact>() : artifacts.Select(b => b == null ? null : b.Serialize()).ToArray()
            };
        }
        public static ArtifactList FromSerializable(SerializableArtifactList serializable, LevelEngine level)
        {
            var artifactList = new ArtifactList(level, serializable.artifacts.Length);
            for (int i = 0; i < artifactList.artifacts.Length; i++)
            {
                var seri = serializable.artifacts[i];
                if (seri == null)
                    continue;
                var artifact = Artifact.Deserialize(seri, level);
                artifact.OnHighlighted += artifactList.OnItemHighlightedCallback;
                artifactList.artifacts[i] = artifact;
            }
            return artifactList;
        }
        #endregion

        public void Update()
        {
            foreach (var artifact in artifacts)
            {
                if (artifact == null)
                    continue;
                artifact.Update();
            }
        }
        private void OnItemHighlightedCallback(Artifact artifact)
        {
            OnArtifactHighlighted?.Invoke(GetArtifactIndex(artifact));
        }

        public event Action<int> OnArtifactHighlighted;
        public LevelEngine Level { get; }
        private Artifact[] artifacts;
    }
}
