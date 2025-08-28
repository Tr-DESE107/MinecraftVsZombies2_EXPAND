using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Armors;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Grids;
using PVZEngine.Level;
using PVZEngine.Level.Collisions;
using PVZEngine.Models;
using PVZEngine.Modifiers;
using Tools;
using UnityEngine;

namespace PVZEngine.Entities
{
    public sealed class Entity : IBuffTarget, IAuraSource, IModifierContainer, IPropertyModifyTarget
    {
        #region 公有方法

        public Entity(LevelEngine level, long id, EntityReferenceChain spawnerReference, EntityDefinition definition, int seed) : this(level, definition.Type, id, spawnerReference)
        {
            Definition = definition;
            ModelID = definition.GetModelID();
            InitSeed = seed;
            RNG = new RandomGenerator(seed);
            DropRNG = new RandomGenerator(RNG.Next());
            CreateAuraEffects();
            UpdateModifierCaches();
        }
        private Entity(LevelEngine level, int type, long id, EntityReferenceChain spawnerReference)
        {
            Level = level;
            Type = type;
            TypeCollisionFlag = EntityCollisionHelper.GetTypeMask(type);
            ID = id;
            SpawnerReference = spawnerReference;
            InitBuffList();
            Cache = new EntityCache();
            properties = new PropertyBlock(this);
        }
        public void Init(Entity spawner)
        {
            OnInit(spawner);
            Definition.Init(this);
            auras.PostAdd();
            Cache.UpdateAll(this);
            var param = new EntityCallbackParams()
            {
                entity = this
            };
            Level.Triggers.RunCallbackFiltered(LevelCallbacks.POST_ENTITY_INIT, param, Type);
            PostInit?.Invoke();
        }
        public void Update()
        {
            try
            {
                OnUpdate();
                Definition.Update(this);
                var armors = armorDict.Values.ToArray();
                foreach (var armor in armors)
                {
                    armor.Update();
                }
                auras.Update();
                buffs.Update();
                var param = new EntityCallbackParams()
                {
                    entity = this
                };
                Level.Triggers.RunCallbackFiltered(LevelCallbacks.POST_ENTITY_UPDATE, param, Type);
            }
            catch (Exception ex)
            {
                Debug.LogError($"更新实体时出现错误：{ex}");
            }
            time++;
        }
        public void SetParent(Entity parent)
        {
            var oldParent = Parent;
            if (oldParent != null)
            {
                oldParent.children.RemoveAll(r => r.ID == ID);
            }
            Parent = parent;
            if (parent != null)
            {
                parent.children.Add(this);
            }
        }
        public Entity[] GetChildren()
        {
            return children.ToArray();
        }
        public bool Exists()
        {
            return !Removed;
        }
        public void Remove()
        {
            if (!Removed)
            {
                Removed = true;
                Level.RemoveEntity(this);
                foreach (var pair in takenConveyorSeeds)
                {
                    Level.PutSeedToConveyorPool(pair.Key, pair.Value);
                }
                takenConveyorSeeds.Clear();
                auras.PostRemove();
                Definition.PostRemove(this);
                var param = new EntityCallbackParams()
                {
                    entity = this
                };
                Level.Triggers.RunCallbackFiltered(LevelCallbacks.POST_ENTITY_REMOVE, param, Type);
            }
        }
        public bool IsEntityOf(NamespaceID id)
        {
            return Definition.GetID() == id;
        }

        #region 伤害

        public void Die(Entity source = null, BodyDamageResult damage = null)
        {
            Die(new DamageEffectList(), new EntityReferenceChain(source), damage);
        }
        public void Die(DamageEffectList effects, Entity source = null, BodyDamageResult damage = null)
        {
            Die(effects, new EntityReferenceChain(source), damage);
        }
        public void Die(DamageEffectList effects, EntityReferenceChain source, BodyDamageResult damage = null)
        {
            Die(new DeathInfo(this, effects, source, damage));
        }
        public void Die(DeathInfo info)
        {
            if (IsDead)
                return;
            info = info ?? new DeathInfo(this, new DamageEffectList(), new EntityReferenceChain(null), null);
            IsDead = true;
            Definition.PostDeath(this, info);
            var param = new LevelCallbacks.PostEntityDeathParams()
            {
                entity = this,
                deathInfo = info
            };
            Level.Triggers.RunCallbackFiltered(LevelCallbacks.POST_ENTITY_DEATH, param, Type);
        }
        public void Revive()
        {
            if (!IsDead)
                return;
            IsDead = false;
            var param = new EntityCallbackParams()
            {
                entity = this,
            };
            Level.Triggers.RunCallbackFiltered(LevelCallbacks.POST_ENTITY_REVIVE, param, Type);
        }
        #endregion

        #region 阵营
        public int GetFaction()
        {
            return Cache.Faction;
        }
        public bool IsFriendly(Entity entity)
        {
            if (entity == null)
                return false;
            return IsFriendly(entity.GetFaction());
        }
        public bool IsFriendly(int faction)
        {
            return EngineEntityExt.IsFriendly(GetFaction(), faction);
        }
        public bool IsHostile(Entity entity)
        {
            if (entity == null)
                return false;
            return IsHostile(entity.GetFaction());
        }
        public bool IsHostile(int faction)
        {
            return EngineEntityExt.IsHostile(GetFaction(), faction);
        }
        #endregion 魅惑

        #region 增益属性
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
        private void GetModifierItems(IPropertyKey name, List<ModifierContainerItem> results)
        {
            if (!modifierCaches.TryGetValue(name, out var list))
                return;
            results.AddRange(list);
        }
        private void UpdateAllBuffedProperties(bool triggersEvaluation)
        {
            properties.UpdateAllModifiedProperties(triggersEvaluation);
        }
        private void UpdateModifiedProperty(IPropertyKey name)
        {
            properties.UpdateModifiedProperty(name);
        }
        bool IPropertyModifyTarget.GetFallbackProperty(IPropertyKey name, out object value)
        {
            if (Definition == null)
            {
                value = default;
                return false;
            }
            if (Definition.TryGetPropertyObject(name, out var defProp))
            {
                value = defProp;
                return true;
            }

            var behaviourCount = Definition.GetBehaviourCount();
            for (int i = 0; i < behaviourCount; i++)
            {
                var behaviour = Definition.GetBehaviourAt(i);
                if (behaviour.TryGetPropertyObject(name, out var behProp))
                {
                    value = behProp;
                    return true;
                }
            }
            value = default;
            return false;
        }
        IEnumerable<IPropertyKey> IPropertyModifyTarget.GetModifiedProperties()
        {
            var entityPropertyNames = modifierCaches.Keys;
            var buffPropertyNames = buffs.GetModifierPropertyNames();
            return entityPropertyNames.Union(buffPropertyNames);
        }
        PropertyModifier[] IPropertyModifyTarget.GetModifiersUsingProperty(IPropertyKey name)
        {
            return Definition.GetModifiers().Where(m => name.Equals(m.UsingContainerPropertyName)).ToArray();
        }
        void IPropertyModifyTarget.GetModifierItems(IPropertyKey name, List<ModifierContainerItem> results)
        {
            GetModifierItems(name, results);
            buffs.GetModifierItems(name, results);
        }
        void IPropertyModifyTarget.UpdateModifiedProperty(IPropertyKey name, object beforeValue, object afterValue, bool triggersEvaluation)
        {
            if (triggersEvaluation)
            {
                if (name == ((PropertyKey<float>)EngineEntityProps.MAX_HEALTH))
                {
                    var before = beforeValue.ToGeneric<float>();
                    var after = afterValue.ToGeneric<float>();
                    Health = Mathf.Min(after, Health * (after / before));
                }
            }
            Cache.UpdateProperty(this, name, beforeValue, afterValue);
            PostPropertyChanged?.Invoke(name, beforeValue, afterValue);
        }
        #endregion

        #region 增益
        public BuffReference GetBuffReference(Buff buff) => new BuffReferenceEntity(ID, buff.ID);
        #endregion

        #region 光环
        public AuraEffect GetAuraEffect<T>() where T : AuraEffectDefinition
        {
            return auras.Get<T>();
        }
        public AuraEffect[] GetAuraEffects()
        {
            return auras.GetAll();
        }
        #endregion

        #region 物理

        #region 体积
        public Vector3 GetCenter()
        {
            var center = Position;
            var pivot = Vector3.one * 0.5f - Cache.BoundsPivot;
            center += Vector3.Scale(GetScaledSize(), pivot);
            return center;
        }
        public Vector3 GetScaledSize()
        {
            var size = Cache.Size;
            size.Scale(Cache.GetFinalScale());
            return size.Abs();
        }
        public void SetCenter(Vector3 center)
        {
            var offset = GetCenter() - Position;
            Position = center - offset;
        }
        public Bounds GetBounds()
        {
            return new Bounds(GetCenter(), GetScaledSize());
        }
        #endregion

        #region 相对高度
        public float GetGroundY()
        {
            return Level.GetGroundY(Position.x, Position.z);
        }
        public float GetRelativeY()
        {
            return Position.y - GetGroundY();
        }
        public void SetRelativeY(float value)
        {
            var pos = Position;
            pos.y = value + GetGroundY();
            Position = pos;
        }
        #endregion

        private Vector3 GetNextPosition(float simulationSpeed = 1)
        {
            Vector3 velocity = GetNextVelocity(simulationSpeed);
            velocity.Scale(Vector3.one - Cache.VelocityDampen);
            var nextPos = Position + velocity * simulationSpeed;
            return nextPos;
        }


        private Vector3 GetNextVelocity(float simulationSpeed = 1)
        {
            Vector3 velocity = Velocity;

            // Friction.
            var frictionMulti = Mathf.Pow(Mathf.Max(0, 1 - Cache.Friction), simulationSpeed);
            velocity = new Vector3(velocity.x * frictionMulti, velocity.y, velocity.z * frictionMulti);

            // Gravity.
            velocity.y -= Cache.Gravity * simulationSpeed;

            return velocity;
        }

        public void UpdatePhysics(float simulationSpeed = 1)
        {
            Vector3 nextVelocity = GetNextVelocity(simulationSpeed);
            Vector3 nextPos = GetNextPosition(simulationSpeed);

            // 地面限制。
            float groundY = Level.GetGroundY(nextPos.x, nextPos.z);
            var groundLimit = groundY + Cache.GroundLimitOffset;
            var contactingGround = nextPos.y <= groundY;
            var contactVelocity = nextVelocity;
            if (nextPos.y <= groundLimit)
            {
                nextPos.y = groundLimit;
                nextVelocity.y = Mathf.Max(nextVelocity.y, 0);
            }

            PreviousPosition = Position;
            Position = nextPos;
            Velocity = nextVelocity;

            if (contactingGround)
            {
                if (!IsOnGround)
                {
                    OnContactGround(contactVelocity);
                    IsOnGround = true;
                }
            }
            else
            {
                if (IsOnGround)
                {
                    OnLeaveGround();
                    IsOnGround = false;
                }
            }
        }
        #endregion

        #region 网格位置
        public int GetColumn()
        {
            var gridPivotOffset = Cache.GridPivotOffset;
            return Level.GetColumn(Position.x + gridPivotOffset.x);
        }
        public int GetLane()
        {
            var gridPivotOffset = Cache.GridPivotOffset;
            return Level.GetLane(Position.z + gridPivotOffset.y);
        }
        public int GetGridIndex()
        {
            return Level.GetGridIndex(GetColumn(), GetLane());
        }
        public LawnGrid GetGrid()
        {
            return Level.GetGrid(GetColumn(), GetLane());
        }
        public Vector2Int GetGridPosition()
        {
            return new Vector2Int(GetColumn(), GetLane());
        }
        #endregion

        #region 占据网格
        public LawnGrid[] GetTakenGrids()
        {
            return takenGrids.ToArray();
        }
        public void GetTakenGridsNonAlloc(List<LawnGrid> results)
        {
            results.AddRange(takenGrids);
        }
        public NamespaceID[] GetTakingGridLayers(LawnGrid grid)
        {
            return grid.GetEntityLayers(this);
        }
        public void GetTakingGridLayersNonAlloc(LawnGrid grid, List<NamespaceID> results)
        {
            grid.GetEntityLayersNonAlloc(this, results);
        }
        public bool IsTakingGridLayer(LawnGrid grid, NamespaceID layer)
        {
            return grid.IsEntityOnLayer(this, layer);
        }
        public void TakeGrid(LawnGrid grid, NamespaceID layer)
        {
            grid.AddLayerEntity(layer, this);
            if (!takenGrids.Contains(grid))
            {
                takenGrids.Add(grid);
            }
        }
        public void ReleaseGrid(LawnGrid grid, NamespaceID layer)
        {
            grid.RemoveLayerEntity(layer, this);
            if (!grid.HasEntity(this))
            {
                takenGrids.Remove(grid);
            }
        }
        public void ClearTakenGrids()
        {
            foreach (var grid in takenGrids)
            {
                grid.RemoveGridEntity(this);
            }
            takenGrids.Clear();
        }
        #endregion

        #region 碰撞
        public void UpdateCollision()
        {
            UpdateCollisionDetection();
            UpdateCollisionPosition();
            UpdateCollisionSize();
        }
        public void UpdateCollisionDetection()
        {
            Level.UpdateEntityCollisionDetection(this);
        }
        public void UpdateCollisionPosition()
        {
            Level.UpdateEntityCollisionPosition(this);
        }
        public void UpdateCollisionSize()
        {
            Level.UpdateEntityCollisionSize(this);
        }
        public IEntityCollider CreateCollider(ColliderConstructor info)
        {
            return Level.AddEntityCollider(this, info);
        }
        public bool RemoveCollider(string name)
        {
            return Level.RemoveEntityCollider(this, name);
        }
        public IEntityCollider GetCollider(string name)
        {
            return Level.GetEntityCollider(this, name);
        }
        public void GetCurrentCollisions(List<EntityCollision> collisions)
        {
            Level.GetEntityCurrentCollisions(this, collisions);
        }
        public bool PreCollision(EntityCollision collision)
        {
            var result = new CallbackResult(true);
            Definition.PreCollision(collision, result);
            if (!result.IsBreakRequested)
            {
                var param = new LevelCallbacks.PreEntityCollisionParams()
                {
                    collision = collision,
                };
                Level.Triggers.RunCallbackWithResult(LevelCallbacks.PRE_ENTITY_COLLISION, param, result);
            }
            return result.GetValue<bool>();
        }
        public void PostCollision(EntityCollision collision, int state)
        {
            Definition.PostCollision(collision, state);
            var param = new LevelCallbacks.PostEntityCollisionParams()
            {
                collision = collision,
                state = state
            };
            Level.Triggers.RunCallback(LevelCallbacks.POST_ENTITY_COLLISION, param);
        }
        #endregion

        #region 护甲
        public bool IsEquippingArmor(Armor armor)
        {
            foreach (var pair in armorDict)
            {
                if (pair.Value == armor)
                    return true;
            }
            return false;
        }
        public Armor EquipArmorTo<T>(NamespaceID slot) where T : ArmorDefinition
        {
            return EquipArmorTo(slot, Level.Content.GetArmorDefinition<T>());
        }
        public Armor EquipArmorTo(NamespaceID slot, NamespaceID id)
        {
            return EquipArmorTo(slot, Level.Content.GetArmorDefinition(id));
        }
        public Armor EquipArmorTo(NamespaceID slot, ArmorDefinition definition)
        {
            if (definition == null)
                return null;
            var armor = new Armor(this, slot, definition);
            EquipArmorTo(slot, armor);
            return armor;
        }
        public void EquipArmorTo(NamespaceID slot, Armor armor)
        {
            if (armor == null)
                return;
            if (armorDict.TryGetValue(slot, out var oldShield))
            {
                if (oldShield != null)
                    oldShield.Destroy(null);
            }
            armorDict[slot] = armor;

            // 创建碰撞体
            CreateCollidersForArmor(slot, armor);

            Definition.PostEquipArmor(this, slot, armor);
            var param = new LevelCallbacks.ArmorParams()
            {
                entity = this,
                slot = slot,
                armor = armor
            };
            Level.Triggers.RunCallback(LevelCallbacks.POST_EQUIP_ARMOR, param);
            OnEquipArmor?.Invoke(slot, armor);
            armor.PostAdd();
        }
        public void RemoveArmor(NamespaceID slot)
        {
            if (!armorDict.TryGetValue(slot, out var armor))
                return;
            if (armor == null)
                return;
            armorDict.Remove(slot);

            // 移除碰撞体
            RemoveCollidersFromArmor(slot, armor);

            Definition.PostRemoveArmor(this, slot, armor);
            var param = new LevelCallbacks.ArmorParams()
            {
                entity = this,
                slot = slot,
                armor = armor
            };
            Level.Triggers.RunCallback(LevelCallbacks.POST_REMOVE_ARMOR, param);
            OnRemoveArmor?.Invoke(slot, armor);
            armor.PostRemove();
        }
        public void DestroyArmor(NamespaceID slot, ArmorDestroyInfo info)
        {
            if (!armorDict.TryGetValue(slot, out var armor))
                return;
            if (armor == null)
                return;
            Definition.PostDestroyArmor(this, slot, armor, info);
            var param = new LevelCallbacks.PostArmorDestroyParams()
            {
                entity = this,
                slot = slot,
                armor = armor,
                info = info
            };
            Level.Triggers.RunCallback(LevelCallbacks.POST_DESTROY_ARMOR, param);
        }
        public Armor GetArmorAtSlot(NamespaceID slot)
        {
            return armorDict.TryGetValue(slot, out var armor) ? armor : null;
        }
        public NamespaceID[] GetActiveArmorSlots()
        {
            return armorDict.Keys.ToArray();
        }
        public void ActivateArmorColliders(NamespaceID slot)
        {
            foreach (var collider in GetArmorColliders(slot))
            {
                collider.SetEnabled(true);
            }
        }
        public void DeactivateArmorColliders(NamespaceID slot)
        {
            foreach (var collider in GetArmorColliders(slot))
            {
                collider.SetEnabled(false);
            }
        }
        private void CreateCollidersForArmor(NamespaceID slot, Armor armor)
        {
            foreach (var cons in armor.GetColliderConstructors(this, slot))
            {
                var info = cons;
                info.name = GetArmorColliderName(slot, cons.name);
                info.armorSlot = slot;
                CreateCollider(info);
            }
        }
        private void RemoveCollidersFromArmor(NamespaceID slot, Armor armor)
        {
            foreach (var cons in armor.GetColliderConstructors(this, slot))
            {
                var name = GetArmorColliderName(slot, cons.name);
                RemoveCollider(name);
            }
        }
        private IEnumerable<IEntityCollider> GetArmorColliders(NamespaceID slot)
        {
            var armor = GetArmorAtSlot(slot);
            if (armor == null)
                yield break;
            foreach (var cons in armor.GetColliderConstructors(this, slot))
            {
                var name = GetArmorColliderName(slot, cons.name);
                yield return GetCollider(name);
            }
        }
        private static string GetArmorColliderName(NamespaceID slot, string name)
        {
            return $"{slot}/{name}";
        }
        #endregion

        #region 时间
        public long GetEntityTime()
        {
            return time;

        }
        public bool IsTimeInterval(long interval, long offset = 0)
        {
            return time % interval == offset;
        }
        #endregion
        public bool IsFacingLeft() => this.FaceLeftAtDefault() != (Cache.GetFinalScale().x < 0);

        #region 模型
        public void SetModelInterface(IModelInterface model)
        {
            modelInterface = model;
        }
        public IModelInterface GetModelInterface()
        {
            return modelInterface;
        }
        public void ChangeModel(NamespaceID id)
        {
            ModelID = id;
            OnChangeModel?.Invoke(id);
        }
        public void SetModelProperty(string name, object value)
        {
            modelInterface?.SetModelProperty(name, value);
        }
        public void TriggerModel(string name)
        {
            modelInterface?.TriggerModel(name);
        }
        public void SetShaderInt(string name, int value)
        {
            modelInterface?.SetShaderInt(name, value);
        }
        public void SetShaderFloat(string name, float value)
        {
            modelInterface?.SetShaderFloat(name, value);
        }
        public void SetShaderColor(string name, Color value)
        {
            modelInterface?.SetShaderColor(name, value);
        }
        public IModelInterface CreateChildModel(string anchorName, NamespaceID key, NamespaceID modelID)
        {
            return modelInterface?.CreateChildModel(anchorName, key, modelID);
        }
        public bool RemoveChildModel(NamespaceID key)
        {
            return modelInterface?.RemoveChildModel(key) ?? false;
        }
        public IModelInterface GetChildModel(NamespaceID key)
        {
            return modelInterface?.GetChildModel(key);
        }
        public void UpdateModel()
        {
            modelInterface?.UpdateModel();
        }
        public void TriggerAnimation(string name)
        {
            modelInterface?.TriggerAnimation(name);
        }
        public void SetAnimationBool(string name, bool value)
        {
            modelInterface?.SetAnimationBool(name, value);
        }
        public void SetAnimationInt(string name, int value)
        {
            modelInterface?.SetAnimationInt(name, value);
        }
        public void SetAnimationFloat(string name, float value)
        {
            modelInterface?.SetAnimationFloat(name, value);
        }
        #endregion

        #region 模型插入
        public ModelInsertion[] GetModelInsertions()
        {
            return buffs.SelectMany(b => b.GetModelInsertions()).ToArray();
        }
        #endregion

        #region 传送带
        public void AddTakenConveyorSeed(NamespaceID id)
        {
            if (takenConveyorSeeds.ContainsKey(id))
            {
                takenConveyorSeeds[id]++;
            }
            else
            {
                takenConveyorSeeds[id] = 1;
            }
        }
        public bool RemoveTakenConveyorSeed(NamespaceID id)
        {
            if (!takenConveyorSeeds.ContainsKey(id))
            {
                return false;
            }
            takenConveyorSeeds[id]--;
            if (takenConveyorSeeds[id] <= 0)
            {
                takenConveyorSeeds.Remove(id);
            }
            return true;
        }
        #endregion

        #region 序列化
        public SerializableEntity Serialize()
        {
            var seri = new SerializableEntity();
            seri.id = ID;
            seri.time = time;
            seri.initSeed = InitSeed;
            seri.spawnerReference = SpawnerReference;
            seri.type = Type;
            seri.state = State;
            seri.rng = RNG.ToSerializable();
            seri.dropRng = DropRNG.ToSerializable();
            seri.target = Target?.ID ?? 0;

            seri.definitionID = Definition.GetID();
            seri.modelID = ModelID;
            seri.parent = Parent?.ID ?? 0;
            seri.previousPosition = PreviousPosition;
            seri.position = Position;
            seri.velocity = Velocity;
            seri.collisionMaskHostile = CollisionMaskHostile;
            seri.collisionMaskFriendly = CollisionMaskFriendly;
            seri.renderRotation = RenderRotation;
            seri.takenConveyorSeeds = takenConveyorSeeds.ToDictionary(p => p.ToString(), p => p.Value);
            seri.timeout = Timeout;

            // 护盾
            seri.armors = new Dictionary<string, SerializableArmor>();
            foreach (var pair in armorDict)
            {
                if (pair.Value == null)
                    continue;
                seri.armors.Add(pair.Key.ToString(), pair.Value.Serialize());
            }

            seri.isDead = IsDead;
            seri.health = Health;
            seri.isOnGround = IsOnGround;
            seri.properties = properties.ToSerializable();
            seri.buffs = buffs.ToSerializable();
            seri.children = children.ConvertAll(e => e?.ID ?? 0);
            seri.takenGridIndexes = new List<int>();
            foreach (var grid in takenGrids)
            {
                seri.takenGridIndexes.Add(grid.GetIndex());
            }

            seri.auras = auras.GetAll().Select(a => a.ToSerializable()).ToArray();
            return seri;
        }
        public static Entity Deserialize(SerializableEntity seri, LevelEngine level)
        {
            Entity entity = CreateDeserializingEntity(seri, level);
            entity.ApplyDeserialize(seri);
            return entity;
        }
        public void ApplyDeserialize(SerializableEntity seri)
        {
            time = seri.time;
            InitSeed = seri.initSeed;
            RNG = RandomGenerator.FromSerializable(seri.rng);
            DropRNG = RandomGenerator.FromSerializable(seri.dropRng);
            State = seri.state;
            Target = Level.FindEntityByID(seri.target);

            ModelID = seri.modelID;
            Parent = Level.FindEntityByID(seri.parent);
            PreviousPosition = seri.previousPosition;
            Position = seri.position;
            Velocity = seri.velocity;
            CollisionMaskHostile = seri.collisionMaskHostile;
            CollisionMaskFriendly = seri.collisionMaskFriendly;
            RenderRotation = seri.renderRotation;
            takenConveyorSeeds = seri.takenConveyorSeeds.ToDictionary(p => NamespaceID.ParseStrict(p.Key), p => p.Value);
            Timeout = seri.timeout;

            // 护甲
            armorDict.Clear();
            foreach (var pair in seri.armors)
            {
                if (pair.Value == null)
                    continue;
                var slot = NamespaceID.ParseStrict(pair.Key);
                var armor = Armor.Deserialize(pair.Value, this);
                armorDict.Add(slot, armor);
            }

            IsDead = seri.isDead;
            Health = seri.health;
            IsOnGround = seri.isOnGround;
            properties = PropertyBlock.FromSerializable(seri.properties, this);

            children = seri.children.ConvertAll(e => Level.FindEntityByID(e));
            if (seri.takenGridIndexes != null)
            {
                foreach (var index in seri.takenGridIndexes)
                {
                    var grid = Level.GetGrid(index);
                    if (grid == null)
                        continue;
                    takenGrids.Add(grid);
                }
            }
            else if (seri.takenGrids != null)
            {
                foreach (var info in seri.takenGrids)
                {
                    if (info == null || info.layers == null)
                        continue;
                    var grid = Level.GetGrid(info.grid);
                    if (grid == null)
                        continue;
                    takenGrids.Add(grid);
                }
            }
            LoadAuras(seri);

            UpdateModifierCaches();
            UpdateAllBuffedProperties(false);
            Cache.UpdateAll(this);
        }
        public static Entity CreateDeserializingEntity(SerializableEntity seri, LevelEngine level)
        {
            var entity = new Entity(level, seri.type, seri.id, seri.spawnerReference);
            entity.Definition = level.Content.GetEntityDefinition(seri.definitionID);

            // 先于光环加载，不然找不到Buff
            entity.buffs = BuffList.FromSerializable(seri.buffs, level, entity);
            entity.InitBuffList();
            return entity;
        }
        public void LoadAuras(SerializableEntity seri)
        {
            buffs.LoadAuras(seri.buffs, Level);

            CreateAuraEffects();
            auras.LoadFromSerializable(Level, seri.auras);

            foreach (var pair in armorDict)
            {
                var armor = pair.Value;
                if (armor == null)
                    continue;
                var seriArmor = seri.armors.Values.FirstOrDefault(a => a.slot == pair.Key);
                if (seriArmor == null)
                    continue;
                armor.LoadAuras(seriArmor);
            }
        }
        #endregion

        public override string ToString()
        {
            return $"{ID}({this.Definition.GetID()})";
        }
        #endregion

        #region 私有方法
        private void OnInit(Entity spawner)
        {
            PreviousPosition = Position;
            Health = this.GetMaxHealth();
            UpdateAllBuffedProperties(true);
            Cache.UpdateAll(this);
        }
        private void OnUpdate()
        {
            UpdatePhysics(1);
            Health = Mathf.Min(Health, this.GetMaxHealth());
        }
        private void OnContactGround(Vector3 velocity)
        {
            Definition.PostContactGround(this, velocity);
            var param = new LevelCallbacks.PostEntityContactGroundParams()
            {
                entity = this,
                velocity = velocity
            };
            Level.Triggers.RunCallbackFiltered(LevelCallbacks.POST_ENTITY_CONTACT_GROUND, param, Definition.GetID());
        }
        private void OnLeaveGround()
        {
            Definition.PostLeaveGround(this);
            var param = new EntityCallbackParams()
            {
                entity = this,
            };
            Level.Triggers.RunCallback(LevelCallbacks.POST_ENTITY_LEAVE_GROUND, param);
        }
        private void OnBuffAddedCallback(Buff buff)
        {
            foreach (var insertion in buff.GetModelInsertions())
            {
                OnModelInsertionAdded?.Invoke(insertion);
            }
        }
        private void OnBuffRemovedCallback(Buff buff)
        {
            foreach (var insertion in buff.GetModelInsertions())
            {
                OnModelInsertionRemoved?.Invoke(insertion);
            }
        }
        private void CreateAuraEffects()
        {
            var auraDefs = Definition.GetAuras();
            for (int i = 0; i < auraDefs.Length; i++)
            {
                var auraDef = auraDefs[i];
                auras.Add(Level, new AuraEffect(auraDef, i, this));
            }
        }
        private void UpdateModifierCaches()
        {
            foreach (var modifier in Definition.GetModifiers())
            {
                var propName = modifier.PropertyName;
                if (!modifierCaches.TryGetValue(propName, out var list))
                {
                    list = new List<ModifierContainerItem>();
                    modifierCaches.Add(propName, list);
                }
                list.Add(new ModifierContainerItem(this, modifier));
            }
        }
        private void InitBuffList()
        {
            buffs.OnPropertyChanged += UpdateModifiedProperty;
            buffs.OnBuffAdded += OnBuffAddedCallback;
            buffs.OnBuffRemoved += OnBuffRemovedCallback;
        }
        IModelInterface IBuffTarget.GetInsertedModel(NamespaceID key) => GetChildModel(key);
        LevelEngine IBuffTarget.GetLevel() => Level;
        Entity IBuffTarget.GetEntity() => this;
        Entity IAuraSource.GetEntity() => this;
        LevelEngine IAuraSource.GetLevel() => Level;
        bool IAuraSource.IsValid() => Exists();
        T IModifierContainer.GetProperty<T>(PropertyKey<T> name) => GetProperty<T>(name);
        #endregion

        #region 事件
        public event Action PostInit;
        public event Action<IPropertyKey, object, object> PostPropertyChanged;
        public event Action<NamespaceID> OnChangeModel;
        public event Action<NamespaceID, Armor> OnEquipArmor;
        public event Action<NamespaceID, Armor> OnRemoveArmor;
        public event Action<ModelInsertion> OnModelInsertionAdded;
        public event Action<ModelInsertion> OnModelInsertionRemoved;
        #endregion

        #region 属性字段
        public long ID { get; }
        public int InitSeed { get; private set; }
        public RandomGenerator RNG { get; private set; }
        public RandomGenerator DropRNG { get; private set; }
        public bool Removed { get; private set; }
        public EntityDefinition Definition { get; private set; }
        public NamespaceID ModelID { get; private set; }
        public EntityReferenceChain SpawnerReference { get; private set; }
        public Entity Parent { get; private set; }
        public LevelEngine Level { get; private set; }
        public Vector3 PreviousPosition { get; private set; }
        public Vector3 Position
        {
            get => _position;
            set
            {
                bool updates = _position != value;
                _position = value;
                if (updates)
                {
                    UpdateCollisionPosition();
                }
            }
        }
        public Vector3 Velocity { get; set; }
        public Vector3 RenderRotation { get; set; } = Vector3.zero;
        #region Collision
        public int CollisionMaskHostile { get; set; }
        public int CollisionMaskFriendly { get; set; }
        private IModelInterface modelInterface;
        #endregion

        public int Timeout { get; set; } = -1;
        public bool IsDead { get; set; }
        public float Health { get; set; }
        public int Type { get; }
        public int State { get; set; }
        public Entity Target { get; set; }
        public bool IsOnGround { get; private set; } = true;
        internal int TypeCollisionFlag { get; }
        internal EntityCache Cache { get; }
        BuffList IBuffTarget.Buffs => buffs;

        private PropertyBlock properties;
        private Vector3 _position;

        #region 护盾
        private Dictionary<NamespaceID, Armor> armorDict = new Dictionary<NamespaceID, Armor>();
        #endregion

        private long time = 0;
        private BuffList buffs = new BuffList();
        private AuraEffectList auras = new AuraEffectList();
        private List<LawnGrid> takenGrids = new List<LawnGrid>();
        private List<Entity> children = new List<Entity>();
        private Dictionary<NamespaceID, int> takenConveyorSeeds = new Dictionary<NamespaceID, int>();
        private Dictionary<IPropertyKey, List<ModifierContainerItem>> modifierCaches = new Dictionary<IPropertyKey, List<ModifierContainerItem>>(new PropertyKeyComparer());
        #endregion
    }
}