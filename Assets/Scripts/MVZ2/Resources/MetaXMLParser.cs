using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MVZ2Logic.Almanacs;
using MVZ2Logic.Audios;
using MVZ2Logic.Entities;
using MVZ2Logic.Level;
using MVZ2Logic.Map;
using MVZ2Logic.Modding;
using MVZ2Logic.Models;
using MVZ2Logic.Notes;
using MVZ2Logic.Talk;
using PVZEngine;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.Resources
{
    public static class MetaXMLParser
    {
        #region 图鉴
        public static AlmanacMetaEntry ToAlmanacMetaEntry(this XmlNode node, string defaultNsp)
        {
            var id = node.GetAttributeNamespaceID("id", defaultNsp);
            var lineNodes = node.ChildNodes;
            var lines = new string[lineNodes.Count];
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lineNodes[i].InnerText;
            }
            var text = string.Join("\n", lines);
            return new AlmanacMetaEntry()
            {
                id = id,
                text = text
            };
        }
        public static AlmanacMetaList ToAlmanacMetaList(this XmlNode node, string defaultNsp)
        {
            var entries = new Dictionary<string, AlmanacMetaEntry[]>();
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                var categoryNode = node.ChildNodes[i];
                var typeName = categoryNode.Name;
                var categoryEntries = new AlmanacMetaEntry[categoryNode.ChildNodes.Count];
                for (int j = 0; j < categoryEntries.Length; j++)
                {
                    categoryEntries[j] = ToAlmanacMetaEntry(categoryNode.ChildNodes[j], defaultNsp);
                    categoryEntries[j].index = j;
                }
                entries.Add(typeName, categoryEntries);
            }
            return new AlmanacMetaList()
            {
                entries = entries,
            };
        }
        #endregion

        #region 音效
        public static SoundMeta ToSoundMeta(this XmlNode node, string defaultNsp)
        {
            var name = node.GetAttribute("name");
            var priority = node.GetAttributeInt("priority") ?? 128;
            var maxCount = node.GetAttributeInt("maxCount") ?? 0;
            var samples = new AudioSample[node.ChildNodes.Count];
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = AudioSample.FromXmlNode(node.ChildNodes[i], defaultNsp);
            }
            return new SoundMeta()
            {
                name = name,
                priority = priority,
                maxCount = maxCount,
                samples = samples
            };
        }
        public static SoundMetaList ToSoundMetaList(this XmlNode node, string defaultNsp)
        {
            var sounds = new SoundMeta[node.ChildNodes.Count];
            for (int i = 0; i < sounds.Length; i++)
            {
                sounds[i] = ToSoundMeta(node.ChildNodes[i], defaultNsp);
            }
            return new SoundMetaList()
            {
                metas = sounds,
            };
        }
        #endregion

        #region 实体
        public static EntityMeta ToEntityMeta(this XmlNode node, string defaultNsp, IEnumerable<EntityMetaTemplate> templates)
        {
            var type = EntityTypes.EFFECT;
            var template = templates.FirstOrDefault(t => t.name == node.Name);
            var id = node.GetAttribute("id");
            var name = node.GetAttribute("name");
            var deathMessage = node.GetAttribute("deathMessage");
            var unlock = node.GetAttributeNamespaceID("unlock", defaultNsp);
            var tooltip = node.GetAttribute("tooltip");

            Dictionary<string, object> properties = node.ToPropertyDictionary(defaultNsp);
            if (template != null)
            {
                type = template.id;
                foreach (var prop in template.properties)
                {
                    if (properties.ContainsKey(prop.Key))
                        continue;
                    properties.Add(prop.Key, prop.Value);
                }
            }
            return new EntityMeta()
            {
                type = type,
                id = id,
                name = name,
                deathMessage = deathMessage,
                tooltip = tooltip,
                unlock = unlock,
                properties = properties,
            };
        }
        public static EntityMetaList ToEntityMetaList(this XmlNode node, string defaultNsp)
        {
            var templatesNode = node["templates"];
            var metaTemplates = LoadEntityTemplates(templatesNode, defaultNsp);

            var entriesNode = node["entries"];
            var resources = new EntityMeta[entriesNode.ChildNodes.Count];
            for (int i = 0; i < resources.Length; i++)
            {
                var meta = ToEntityMeta(entriesNode.ChildNodes[i], defaultNsp, metaTemplates);
                meta.order = i;
                resources[i] = meta;
            }
            return new EntityMetaList()
            {
                metas = resources,
            };
        }
        private static EntityMetaTemplate LoadEntityTemplate(this XmlNode node, string defaultNsp, XmlNode rootNode)
        {
            var name = node.Name;
            var id = node.GetAttributeInt("id") ?? -1;
            var properties = new Dictionary<string, object>();
            LoadTemplatePropertiesFromNode(node, defaultNsp, rootNode, properties);

            return new EntityMetaTemplate()
            {
                name = name,
                id = id,
                properties = properties
            };
        }
        private static void LoadTemplatePropertiesFromNode(this XmlNode node, string defaultNsp, XmlNode rootNode, Dictionary<string, object> target)
        {
            var parent = node.GetAttribute("parent");
            if (!string.IsNullOrEmpty(parent))
            {
                var parentNode = rootNode[parent];
                if (parentNode != null)
                {
                    LoadTemplatePropertiesFromNode(parentNode, defaultNsp, rootNode, target);
                }
            }
            var props = node.ToPropertyDictionary(defaultNsp);
            foreach (var prop in props)
            {
                target[prop.Key] = prop.Value;
            }
        }
        public static EntityMetaTemplate[] LoadEntityTemplates(this XmlNode node, string defaultNsp)
        {
            List<EntityMetaTemplate> templates = new List<EntityMetaTemplate>();
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                var templateNode = node.ChildNodes[i];
                var template = LoadEntityTemplate(templateNode, defaultNsp, node);
                templates.Add(template);
            }
            return templates.ToArray();
        }
        #endregion

        #region 碎片
        public static FragmentMeta ToFragmentMeta(this XmlNode node)
        {
            var name = node.GetAttribute("name");
            return new FragmentMeta()
            {
                name = name,
                gradient = node.ToGradient()
            };
        }
        public static FragmentMetaList ToFragmentMetaList(this XmlNode node)
        {
            var resources = new FragmentMeta[node.ChildNodes.Count];
            for (int i = 0; i < resources.Length; i++)
            {
                resources[i] = ToFragmentMeta(node.ChildNodes[i]);
            }
            return new FragmentMetaList()
            {
                metas = resources,
            };
        }
        #endregion

        #region 关卡
        public static StageMeta ToStageMeta(this XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var name = node.GetAttribute("name");
            var type = node.GetAttribute("type") ?? "normal";
            var dayNumber = node.GetAttributeInt("dayNumber") ?? 0;
            var unlock = node.GetAttributeNamespaceID("unlock", defaultNsp);

            var talkNode = node["talk"];
            var startTalk = talkNode?.GetAttributeNamespaceID("start", defaultNsp);
            var endTalk = talkNode?.GetAttributeNamespaceID("end", defaultNsp);
            var mapTalk = talkNode?.GetAttributeNamespaceID("map", defaultNsp);

            var clearNode = node["clear"];
            var clearPickupModel = clearNode?.GetAttributeNamespaceID("pickupModel", defaultNsp);
            var clearPickupBlueprint = clearNode?.GetAttributeNamespaceID("blueprint", defaultNsp);
            var endNote = clearNode?.GetAttributeNamespaceID("note", defaultNsp);

            var cameraNode = node["camera"];
            var cameraPositionStr = cameraNode?.GetAttribute("position");
            var startCameraPosition = cameraPositionDict.TryGetValue(cameraPositionStr ?? string.Empty, out var p) ? p : LevelCameraPosition.House;
            var transition = cameraNode?.GetAttribute("transition");

            var spawnNode = node["spawns"];
            var flags = spawnNode?.GetAttributeInt("flags") ?? 1;
            var firstWaveTime = spawnNode?.GetAttributeInt("firstWaveTime") ?? 540;
            var spawns = new EnemySpawnEntry[spawnNode?.ChildNodes.Count ?? 0];
            for (int i = 0; i < spawns.Length; i++)
            {
                spawns[i] = ToEnemySpawnEntry(spawnNode.ChildNodes[i], defaultNsp);
            }

            var propertiesNode = node["properties"];
            var properties = propertiesNode.ToPropertyDictionary(defaultNsp);
            return new StageMeta()
            {
                id = id,
                name = name,
                dayNumber = dayNumber,
                type = type,
                unlock = unlock,

                startTalk = startTalk,
                endTalk = endTalk,
                mapTalk = mapTalk,

                clearPickupModel = clearPickupModel,
                clearPickupBlueprint = clearPickupBlueprint,
                endNote = endNote,

                startCameraPosition = startCameraPosition,
                startTransition = transition,

                totalFlags = flags,
                firstWaveTime = firstWaveTime,
                spawns = spawns,

                properties = properties
            };
        }
        public static StageMetaList ToStageMetaList(XmlNode node, string defaultNsp)
        {
            var resources = new StageMeta[node.ChildNodes.Count];
            for (int i = 0; i < resources.Length; i++)
            {
                var meta = ToStageMeta(node.ChildNodes[i], defaultNsp);
                resources[i] = meta;
            }
            return new StageMetaList()
            {
                metas = resources,
            };
        }
        public static EnemySpawnEntry ToEnemySpawnEntry(this XmlNode node, string defaultNsp)
        {
            var spawnRef = node.GetAttributeNamespaceID("id", defaultNsp);
            var earliestFlag = node.GetAttributeInt("earliestFlag") ?? 0;
            return new EnemySpawnEntry(spawnRef, earliestFlag);
        }
        #endregion

        #region 对话人物
        public static XmlNode ToXmlNode(this TalkCharacter character, XmlDocument document)
        {
            XmlNode node = document.CreateElement("character");
            node.CreateAttribute("id", character.id?.ToString());
            node.CreateAttribute("variant", character.variant?.ToString());
            node.CreateAttribute("side", character.side);
            return node;
        }
        public static TalkCharacter ToTalkCharacter(this XmlNode node, string defaultNsp)
        {
            var id = node.GetAttributeNamespaceID("id", defaultNsp);
            var variant = node.GetAttributeNamespaceID("variant", defaultNsp);
            var side = node.GetAttribute("side");
            return new TalkCharacter()
            {
                id = id,
                variant = variant,
                side = side
            };
        }
        public static TalkCharacterLayer ToTalkCharacterLayer(XmlNode node, string defaultNsp)
        {
            var layer = new TalkCharacterLayer();
            layer.sprite = node.GetAttributeSpriteReference("sprite", defaultNsp);
            layer.positionX = node.GetAttributeInt("positionX") ?? 0;
            layer.positionY = node.GetAttributeInt("positionY") ?? 0;
            return layer;
        }
        public static TalkCharacterVariant ToTalkCharacterVariant(XmlNode node, string defaultNsp)
        {
            var variant = new TalkCharacterVariant();
            variant.id = node.GetAttributeNamespaceID("id", defaultNsp);
            variant.width = node.GetAttributeInt("width");
            variant.height = node.GetAttributeInt("height");
            variant.pivotX = node.GetAttributeFloat("pivotX");
            variant.pivotY = node.GetAttributeFloat("pivotY");
            variant.parent = node.GetAttributeNamespaceID("parent", defaultNsp);

            var variantChildNodes = node.ChildNodes;
            for (int i = 0; i < variantChildNodes.Count; i++)
            {
                var child = variantChildNodes[i];
                variant.layers.Add(ToTalkCharacterLayer(child, defaultNsp));
            }
            return variant;
        }
        public static TalkCharacterMeta ToTalkCharacterMeta(XmlNode node, string defaultNsp)
        {
            var meta = new TalkCharacterMeta();
            meta.name = node.GetAttribute("name");
            meta.unlockCondition = node.GetAttributeNamespaceID("unlock", defaultNsp);

            var variantChildNodes = node.ChildNodes;
            for (int i = 0; i < variantChildNodes.Count; i++)
            {
                var child = variantChildNodes[i];
                meta.variants.Add(ToTalkCharacterVariant(child, defaultNsp));
            }
            return meta;
        }
        public static TalkCharacterMetaList ToTalkCharacterMetaList(XmlNode node, string defaultNsp)
        {
            var list = new TalkCharacterMetaList();
            var metaChildNodes = node.ChildNodes;
            for (int i = 0; i < metaChildNodes.Count; i++)
            {
                var child = metaChildNodes[i];
                list.metas.Add(ToTalkCharacterMeta(child, defaultNsp));
            }
            return list;
        }
        #endregion

        #region 对话
        public static XmlNode ToXmlNode(this TalkSentence sentence, XmlDocument document)
        {
            XmlNode node = document.CreateElement("sentence");
            var textNode = document.CreateTextNode(sentence.text);
            node.AppendChild(textNode);
            node.CreateAttribute("speaker", sentence.speaker?.ToString());
            node.CreateAttribute("description", sentence.descriptionId?.ToString());
            node.CreateAttribute("sounds", sentence.sounds != null ? string.Join(";", sentence.sounds.Select(s => s.ToString())) : null);
            node.CreateAttribute("variant", sentence.variant);
            node.CreateAttribute("onStart", sentence.startScripts != null ? string.Join(";", sentence.startScripts.Where(s => s != null).Select(s => s.ToString())) : null);
            node.CreateAttribute("onClick", sentence.clickScripts != null ? string.Join(";", sentence.clickScripts.Where(s => s != null).Select(s => s.ToString())) : null);
            return node;
        }
        public static TalkSentence ToTalkSentence(this XmlNode node, string defaultNsp)
        {
            var speaker = node.GetAttributeNamespaceID("speaker", defaultNsp);
            var description = node.GetAttributeNamespaceID("description", defaultNsp);
            var sounds = node.GetAttribute("sounds")?.Split(';')?.Select(s => NamespaceID.Parse(s, defaultNsp)).ToList();
            var variant = node.GetAttribute("variant");
            var startScripts = TalkScript.ParseArray(node.GetAttribute("onStart"))?.ToList();
            var clickScripts = TalkScript.ParseArray(node.GetAttribute("onClick"))?.ToList();
            var text = node.InnerText;
            return new TalkSentence()
            {
                speaker = speaker,
                descriptionId = description,
                sounds = sounds,
                variant = variant,
                text = text,
                startScripts = startScripts,
                clickScripts = clickScripts,
            };
        }
        public static XmlNode ToXmlNode(this TalkSection section, XmlDocument document)
        {
            XmlNode node = document.CreateElement("section");
            node.CreateAttribute("nameID", section.nameId?.ToString());
            node.CreateAttribute("onStart", section.startScripts != null ? string.Join(";", section.startScripts.Where(s => s != null).Select(s => s.ToString())) : null);
            node.CreateAttribute("onSkip", section.skipScripts != null ? string.Join(";", section.skipScripts.Where(s => s != null).Select(s => s.ToString())) : null);

            if (section.characters != null)
            {
                var charactersNode = document.CreateElement("characters");
                foreach (var character in section.characters)
                {
                    var child = character.ToXmlNode(document);
                    charactersNode.AppendChild(child);
                }
                node.AppendChild(charactersNode);
            }

            if (section.sentences != null)
            {
                var sentencesNode = document.CreateElement("sentences");
                foreach (var sentence in section.sentences)
                {
                    var child = sentence.ToXmlNode(document);
                    sentencesNode.AppendChild(child);
                }
                node.AppendChild(sentencesNode);
            }
            return node;
        }
        public static TalkSection ToTalkSection(this XmlNode node, string defaultNsp)
        {
            var nameID = node.GetAttributeNamespaceID("nameID", defaultNsp);
            var startScripts = TalkScript.ParseArray(node.GetAttribute("onStart"))?.ToList();
            var skipScripts = TalkScript.ParseArray(node.GetAttribute("onSkip"))?.ToList();

            var charactersNode = node["characters"];
            List<TalkCharacter> characters = null;
            if (charactersNode != null)
            {
                var characterChildren = charactersNode.ChildNodes;
                characters = new List<TalkCharacter>();
                for (int i = 0; i < characterChildren.Count; i++)
                {
                    var child = characterChildren[i];
                    characters.Add(ToTalkCharacter(child, defaultNsp));
                }
            }
            var sentencesNode = node["sentences"];
            List<TalkSentence> sentences = null;
            if (sentencesNode != null)
            {
                var sentenceChildren = sentencesNode.ChildNodes;
                sentences = new List<TalkSentence>();
                for (int i = 0; i < sentenceChildren.Count; i++)
                {
                    var child = sentenceChildren[i];
                    sentences.Add(ToTalkSentence(child, defaultNsp));
                }
            }
            return new TalkSection()
            {
                nameId = nameID,
                startScripts = startScripts,
                skipScripts = skipScripts,
                characters = characters,
                sentences = sentences,
            };
        }
        public static XmlNode ToXmlNode(this TalkGroup group, XmlDocument document)
        {
            XmlNode node = document.CreateElement("group");
            node.CreateAttribute("name", group.name);
            node.CreateAttribute("requires", group.requires?.ToString());
            node.CreateAttribute("requiresNot", group.requiresNot?.ToString());
            node.CreateAttribute("background", group.archiveBackground?.ToString());
            node.CreateAttribute("music", group.music?.ToString());
            node.CreateAttribute("tags", group.tags != null ? string.Join(";", group.tags.Select(t => t.ToString())) : null);
            foreach (var section in group.sections)
            {
                var child = section.ToXmlNode(document);
                node.AppendChild(child);
            }
            return node;
        }
        public static TalkGroup ToTalkGroup(this XmlNode node, string defaultNsp)
        {
            var name = node.GetAttribute("name");
            var requires = node.GetAttributeNamespaceID("requires", defaultNsp);
            var requiresNot = node.GetAttributeNamespaceID("requiresNot", defaultNsp);
            var background = node.GetAttributeSpriteReference("background", defaultNsp);
            var music = node.GetAttributeNamespaceID("music", defaultNsp);
            var tags = node.GetAttribute("tags")?.Split(';')?.Select(t => NamespaceID.Parse(t, defaultNsp))?.ToList();

            var children = node.ChildNodes;
            var sections = new List<TalkSection>();
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                sections.Add(ToTalkSection(child, defaultNsp));
            }
            return new TalkGroup()
            {
                name = name,
                requires = requires,
                requiresNot = requiresNot,
                archiveBackground = background,
                music = music,
                tags = tags,
                sections = sections,
            };
        }

        public static XmlDocument ToXmlDocument(this TalkMeta meta)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.AppendChild(meta.ToXmlNode(xmlDoc));
            return xmlDoc;
        }
        public static XmlNode ToXmlNode(this TalkMeta meta, XmlDocument document)
        {
            XmlNode node = document.CreateElement("talks");
            foreach (var group in meta.groups)
            {
                var child = group.ToXmlNode(document);
                node.AppendChild(child);
            }
            return node;
        }
        public static TalkMeta ToTalkMeta(this XmlDocument document, string defaultNsp)
        {
            return ToTalkMeta(document["talks"], defaultNsp);
        }
        public static TalkMeta ToTalkMeta(this XmlNode node, string defaultNsp)
        {
            var meta = new TalkMeta();
            var children = node.ChildNodes;
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                meta.groups.Add(ToTalkGroup(child, defaultNsp));
            }
            return meta;
        }
        #endregion

        #region 笔记
        public static NoteMeta ToNoteMeta(this XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var sprite = node.GetAttributeSpriteReference("sprite", defaultNsp);
            var background = node.GetAttributeSpriteReference("background", defaultNsp);
            var startTalk = node.GetAttributeNamespaceID("startTalk", defaultNsp);
            var canFlip = node.GetAttributeBool("canFlip") ?? false;
            var flipSprite = node.GetAttributeSpriteReference("flipSprite", defaultNsp);
            return new NoteMeta()
            {
                id = id,
                sprite = sprite,
                background = background,
                startTalk = startTalk,
                canFlip = canFlip,
                flipSprite = flipSprite
            };
        }
        public static NoteMetaList ToNoteMetaList(this XmlNode node, string defaultNsp)
        {
            var resources = new NoteMeta[node.ChildNodes.Count];
            for (int i = 0; i < resources.Length; i++)
            {
                resources[i] = ToNoteMeta(node.ChildNodes[i], defaultNsp);
            }
            return new NoteMetaList()
            {
                metas = resources,
            };
        }
        #endregion

        #region 地图
        public static MapPreset ToMapPreset(this XmlNode node, string defaultNsp)
        {
            var id = node.GetAttributeNamespaceID("id", defaultNsp);
            var model = node.GetAttributeNamespaceID("model", defaultNsp);
            var music = node.GetAttributeNamespaceID("music", defaultNsp);
            var backgroundColor = node.GetAttributeColor("backgroundColor") ?? Color.black;
            return new MapPreset()
            {
                id = id,
                model = model,
                music = music,
                backgroundColor = backgroundColor,
            };
        }
        public static MapMeta ToMapMeta(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var endlessUnlock = node.GetAttributeNamespaceID("endlessUnlock", defaultNsp);
            var area = node.GetAttributeNamespaceID("area", defaultNsp);

            var presetsNode = node["presets"];
            var presets = new MapPreset[presetsNode?.ChildNodes.Count ?? 0];
            for (int i = 0; i < presets.Length; i++)
            {
                presets[i] = ToMapPreset(presetsNode.ChildNodes[i], defaultNsp);
            }

            var stagesNode = node["stages"];
            var stages = new NamespaceID[stagesNode?.ChildNodes.Count ?? 0];
            for (int i = 0; i < stages.Length; i++)
            {
                stages[i] = stagesNode.ChildNodes[i].GetAttributeNamespaceID("id", defaultNsp);
            }
            return new MapMeta()
            {
                id = id,
                endlessUnlock = endlessUnlock,
                area = area,
                presets = presets,
                stages = stages,
            };
        }
        public static MapMetaList ToMapMetaList(XmlNode node, string defaultNsp)
        {
            var resources = new MapMeta[node.ChildNodes.Count];
            for (int i = 0; i < resources.Length; i++)
            {
                var meta = ToMapMeta(node.ChildNodes[i], defaultNsp);
                resources[i] = meta;
            }
            return new MapMetaList()
            {
                metas = resources,
            };
        }
        #endregion

        #region 模型
        public static ModelMeta ToModelMeta(this XmlNode node, string defaultNsp)
        {
            var name = node.GetAttribute("name");
            var type = node.GetAttribute("type");
            var path = node.GetAttribute("path");
            var width = node.GetAttributeInt("width") ?? 64;
            var height = node.GetAttributeInt("height") ?? 64;
            var xOffset = node.GetAttributeFloat("xOffset") ?? 0;
            var yOffset = node.GetAttributeFloat("yOffset") ?? 0;
            return new ModelMeta()
            {
                name = name,
                type = type,
                path = NamespaceID.Parse(path, defaultNsp),
                width = width,
                height = height,
                xOffset = xOffset,
                yOffset = yOffset,
            };
        }
        public static ModelMetaList ToModelMetaList(XmlNode node, string defaultNsp)
        {
            var metas = new ModelMeta[node.ChildNodes.Count];
            for (int i = 0; i < metas.Length; i++)
            {
                metas[i] = ToModelMeta(node.ChildNodes[i], defaultNsp);
            }
            return new ModelMetaList()
            {
                metas = metas,
            };
        }
        #endregion

        #region 区域
        public static AreaMeta ToAreaMeta(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var model = node.GetAttributeNamespaceID("model", defaultNsp);
            var music = node.GetAttributeNamespaceID("music", defaultNsp);
            return new AreaMeta()
            {
                id = id,
                model = model,
                music = music,
            };
        }
        public static AreaMetaList ToAreaMetaList(XmlNode node, string defaultNsp)
        {
            var resources = new AreaMeta[node.ChildNodes.Count];
            for (int i = 0; i < resources.Length; i++)
            {
                var meta = ToAreaMeta(node.ChildNodes[i], defaultNsp);
                resources[i] = meta;
            }
            return new AreaMetaList()
            {
                metas = resources,
            };
        }
        #endregion

        #region 难度
        public static DifficultyMeta ToDifficultyMeta(XmlNode node)
        {
            var id = node.GetAttribute("id");
            var name = node.GetAttribute("name");
            return new DifficultyMeta()
            {
                id = id,
                name = name
            };
        }
        public static DifficultyMetaList ToDifficultyMetaList(XmlNode node)
        {
            var resources = new DifficultyMeta[node.ChildNodes.Count];
            for (int i = 0; i < resources.Length; i++)
            {
                resources[i] = ToDifficultyMeta(node.ChildNodes[i]);
            }
            return new DifficultyMetaList()
            {
                metas = resources,
            };
        }
        #endregion

        public static void LoadMetaList(this ModResource resource, string metaPath, XmlDocument document, string defaultNsp)
        {
            switch (metaPath)
            {
                case "talkcharacters":
                    resource.TalkCharacterMetaList = ToTalkCharacterMetaList(document["characters"], defaultNsp);
                    break;
                case "sounds":
                    resource.SoundMetaList = ToSoundMetaList(document["sounds"], defaultNsp);
                    break;
                case "models":
                    resource.ModelMetaList = ToModelMetaList(document["models"], defaultNsp);
                    break;
                case "fragments":
                    resource.FragmentMetaList = ToFragmentMetaList(document["fragments"]);
                    break;
                case "difficulties":
                    resource.DifficultyMetaList = ToDifficultyMetaList(document["difficulties"]);
                    break;
                case "entities":
                    resource.EntityMetaList = ToEntityMetaList(document["entities"], defaultNsp);
                    break;
                case "almanac":
                    resource.AlmanacMetaList = ToAlmanacMetaList(document["almanac"], defaultNsp);
                    break;
                case "notes":
                    resource.NoteMetaList = ToNoteMetaList(document["notes"], defaultNsp);
                    break;
                case "stages":
                    resource.StageMetaList = ToStageMetaList(document["stages"], defaultNsp);
                    break;
                case "maps":
                    resource.MapMetaList = ToMapMetaList(document["maps"], defaultNsp);
                    break;
                case "areas":
                    resource.AreaMetaList = ToAreaMetaList(document["areas"], defaultNsp);
                    break;
            }
        }
        public static readonly Dictionary<string, LevelCameraPosition> cameraPositionDict = new Dictionary<string, LevelCameraPosition>()
        {
            { "house", LevelCameraPosition.House },
            { "lawn", LevelCameraPosition.Lawn },
            { "choose", LevelCameraPosition.Choose },
        };
    }
    public class EntityMetaTemplate
    {
        public string name;
        public int id;
        public Dictionary<string, object> properties;
    }
}
