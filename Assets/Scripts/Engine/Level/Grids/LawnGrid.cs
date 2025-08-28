using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Models;
using PVZEngine.Modifiers;
using UnityEngine;

namespace PVZEngine.Grids
{
    public class LawnGrid : IPropertyModifyTarget, IBuffTarget
    {
        #region �����¼�
        public LawnGrid(LevelEngine level, GridDefinition definition, int lane, int column)
        {
            Level = level;
            Lane = lane;
            Column = column;
            Definition = definition;
            properties = new PropertyBlock(this);
            InitBuffList();
        }
        public void Update()
        {
            try
            {
                buffs.Update();
            }
            catch (Exception ex)
            {
                Debug.LogError($"���µظ�ʱ���ִ���{ex}");
            }
        }
        public int GetIndex()
        {
            return Level.GetGridIndex(Column, Lane);
        }
        public Vector3 GetEntityPosition()
        {
            var x = Level.GetEntityColumnX(Column);
            var z = Level.GetEntityLaneZ(Lane);
            var y = Level.GetGroundY(x, z);
            return new Vector3(x, y, z);
        }
        public float GetGroundY()
        {
            var x = Level.GetEntityColumnX(Column);
            var z = Level.GetEntityLaneZ(Lane);
            return Level.GetGroundY(x, z);
        }
        #region ���ռ��ʵ��
        private HashSet<Entity> GetOrCreateLayerEntityHashSet(NamespaceID layer)
        {
            if (!layerEntities.TryGetValue(layer, out var hashSet))
            {
                hashSet = new HashSet<Entity>();
                layerEntities.Add(layer, hashSet);
            }
            return hashSet;
        }
        private void AddReversedLayerEntity(NamespaceID layer, Entity entity)
        {
            if (reverseLayerEntities.TryGetValue(entity, out var layerHashSet))
            {
                layerHashSet.Add(layer);
            }
            else
            {
                layerHashSet = new HashSet<NamespaceID>() { layer };
                reverseLayerEntities.Add(entity, layerHashSet);
            }
        }
        public void AddLayerEntity(NamespaceID layer, Entity entity)
        {
            var hashSet = GetOrCreateLayerEntityHashSet(layer);
            hashSet.Add(entity);
            AddReversedLayerEntity(layer, entity);
        }
        #endregion

        #region �Ƴ�ռ��ʵ��
        public void RemoveLayerEntity(NamespaceID layer, Entity entity)
        {
            if (layerEntities.TryGetValue(layer, out var hashSet))
            {
                hashSet.Remove(entity);
                if (hashSet.Count <= 0)
                {
                    layerEntities.Remove(layer);
                }
            }
            if (reverseLayerEntities.TryGetValue(entity, out var reverseHashSet))
            {
                reverseHashSet.Remove(layer);
                if (reverseHashSet.Count <= 0)
                {
                    reverseLayerEntities.Remove(entity);
                }
            }
        }
        public void RemoveGridEntity(Entity entity)
        {
            if (reverseLayerEntities.TryGetValue(entity, out var reversedHashSet))
            {
                foreach (var layer in reversedHashSet)
                {
                    if (layerEntities.TryGetValue(layer, out var hashSet))
                    {
                        hashSet.Remove(entity);
                        if (hashSet.Count <= 0)
                        {
                            layerEntities.Remove(layer);
                        }
                    }
                }
                reversedHashSet.Clear();
                reverseLayerEntities.Remove(entity);
            }
        }
        #endregion

        #region ��ȡռ��ʵ��
        public Entity GetLayerEntity(NamespaceID layer)
        {
            if (layerEntities.TryGetValue(layer, out var hashSet))
            {
                return hashSet.FirstOrDefault();
            }
            return null;
        }
        public Entity[] GetLayerEntities(NamespaceID layer)
        {
            if (layerEntities.TryGetValue(layer, out var hashSet))
            {
                return hashSet.ToArray();
            }
            return null;
        }
        public void GetLayerEntities(NamespaceID layer, List<Entity> results)
        {
            if (layerEntities.TryGetValue(layer, out var hashSet))
            {
                results.AddRange(hashSet);
            }
        }
        public bool IsEntityOnLayer(Entity entity, NamespaceID layer)
        {
            if (layerEntities.TryGetValue(layer, out var hashSet))
            {
                return hashSet.Contains(entity);
            }
            return false;
        }
        public bool HasEntity(Entity entity)
        {
            return reverseLayerEntities.ContainsKey(entity);
        }
        public bool IsEmpty()
        {
            return layerEntities.Count == 0;
        }
        public Entity[] GetEntities()
        {
            return reverseLayerEntities.Keys.ToArray();
        }
        public NamespaceID[] GetLayers()
        {
            return layerEntities.Keys.ToArray();
        }
        #endregion

        #region ��ȡʵ��ռ�ݲ�
        public NamespaceID[] GetEntityLayers(Entity entity)
        {
            if (reverseLayerEntities.TryGetValue(entity, out var reverseHashSet))
            {
                return reverseHashSet.ToArray();
            }
            return Array.Empty<NamespaceID>();
        }
        public void GetEntityLayersNonAlloc(Entity entity, List<NamespaceID> results)
        {
            if (reverseLayerEntities.TryGetValue(entity, out var reverseHashSet))
            {
                results.AddRange(reverseHashSet);
            }
        }
        #endregion

        #region ����
        public BuffReference GetBuffReference(Buff buff)
        {
            return new BuffReferenceLawnGrid(GetIndex(), buff.ID);
        }
        private void InitBuffList()
        {
            buffs.OnPropertyChanged += OnBuffPropertyChangedCallback;
            buffs.OnBuffAdded += OnBuffAddedCallback;
            buffs.OnBuffRemoved += OnBuffRemovedCallback;
        }
        private void OnBuffPropertyChangedCallback(IPropertyKey name)
        {
            properties.UpdateModifiedProperty(name);
        }
        private void OnBuffAddedCallback(Buff buff)
        {
        }
        private void OnBuffRemovedCallback(Buff buff)
        {
        }
        #endregion
        public SerializableGrid Serialize()
        {
            return new SerializableGrid()
            {
                lane = Lane,
                column = Column,
                definitionID = Definition.GetID(),
                layerEntityLists = layerEntities.ToDictionary(p => p.Key.ToString(), p => p.Value.Select(e => e.ID).ToArray()),
                properties = properties.ToSerializable(),
                buffs = buffs.ToSerializable()
            };
        }
        public static LawnGrid Deserialize(SerializableGrid seri, LevelEngine level)
        {
            var definition = level.Content.GetGridDefinition(seri.definitionID);
            var grid = new LawnGrid(level, definition, seri.lane, seri.column);
            return grid;
        }
        public void LoadFromSerializable(SerializableGrid seri, LevelEngine level)
        {
            properties = PropertyBlock.FromSerializable(seri.properties, this);
            buffs = BuffList.FromSerializable(seri.buffs, level, this);
            InitBuffList();
            layerEntities.Clear();
            reverseLayerEntities.Clear();
            if (seri.layerEntityLists != null)
            {
                foreach (var pair in seri.layerEntityLists)
                {
                    var layer = NamespaceID.ParseStrict(pair.Key);
                    var entityHashSet = new HashSet<Entity>();
                    foreach (var entityID in pair.Value)
                    {
                        var entity = level.FindEntityByID(entityID);
                        if (entity == null)
                            continue;

                        entityHashSet.Add(entity);
                        AddReversedLayerEntity(layer, entity);
                    }
                    layerEntities.Add(layer, entityHashSet);
                }
            }
            else if (seri.layerEntities != null)
            {
                foreach (var pair in seri.layerEntities)
                {
                    var layer = NamespaceID.ParseStrict(pair.Key);
                    var entity = level.FindEntityByID(pair.Value);
                    if (entity == null)
                        continue;

                    var layerHashSet = new HashSet<NamespaceID>() { layer };
                    var entityHashSet = new HashSet<Entity>() { entity };
                    layerEntities.Add(layer, entityHashSet);
                    reverseLayerEntities.Add(entity, layerHashSet);
                }
            }
            LoadAuras(seri);
        }
        public void LoadAuras(SerializableGrid seri)
        {
            buffs.LoadAuras(seri.buffs, Level);
        }
        #endregion ����

        #region ��������
        public T GetProperty<T>(PropertyKey<T> name, bool ignoreBuffs = false)
        {
            return properties.GetProperty<T>(name, ignoreBuffs);
        }
        public void SetProperty<T>(PropertyKey<T> name, T value)
        {
            properties.SetProperty(name, value);
        }
        public void SetPropertyObject(IPropertyKey name, object value)
        {
            properties.SetPropertyObject(name, value);
        }
        #endregion

        #region IBuffTargetʵ��
        IModelInterface IBuffTarget.GetInsertedModel(NamespaceID key) => null;
        LevelEngine IBuffTarget.GetLevel() => Level;
        Entity IBuffTarget.GetEntity() => null;
        bool IBuffTarget.Exists() => true;
        #endregion

        #region IPropertyModifyTargetʵ��
        bool IPropertyModifyTarget.GetFallbackProperty(IPropertyKey name, out object value)
        {
            if (Definition == null)
            {
                value = default;
                return false;
            }
            return Definition.TryGetPropertyObject(name, out value);
        }
        IEnumerable<IPropertyKey> IPropertyModifyTarget.GetModifiedProperties()
        {
            return buffs.GetModifierPropertyNames();
        }
        PropertyModifier[] IPropertyModifyTarget.GetModifiersUsingProperty(IPropertyKey name)
        {
            return null;
        }
        void IPropertyModifyTarget.GetModifierItems(IPropertyKey name, List<ModifierContainerItem> results)
        {
            buffs.GetModifierItems(name, results);
        }
        void IPropertyModifyTarget.UpdateModifiedProperty(IPropertyKey name, object beforeValue, object afterValue, bool triggersEvaluation)
        {
        }
        #endregion

        #region ����
        public LevelEngine Level { get; private set; }
        public int Lane { get; set; }
        public int Column { get; set; }
        public GridDefinition Definition { get; set; }
        BuffList IBuffTarget.Buffs => buffs;
        private BuffList buffs = new BuffList();
        private Dictionary<NamespaceID, HashSet<Entity>> layerEntities = new Dictionary<NamespaceID, HashSet<Entity>>();
        private Dictionary<Entity, HashSet<NamespaceID>> reverseLayerEntities = new Dictionary<Entity, HashSet<NamespaceID>>();
        private PropertyBlock properties;
        #endregion ����
    }
}