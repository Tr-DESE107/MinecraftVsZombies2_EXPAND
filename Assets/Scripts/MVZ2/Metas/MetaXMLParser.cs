﻿using System.Xml;
using MVZ2.IO;
using MVZ2.Modding;
using PVZEngine.Level.Collisions;
using UnityEngine;

namespace MVZ2.Metas
{
    public static class MetaXMLParser
    {
        public static void LoadMetaList(this ModResource resource, string metaPath, XmlDocument document, string defaultNsp)
        {
            switch (metaPath)
            {
                case "talkcharacters":
                    resource.TalkCharacterMetaList = TalkCharacterMetaList.FromXmlNode(document["characters"], defaultNsp);
                    break;
                case "sounds":
                    resource.SoundMetaList = SoundMetaList.FromXmlNode(document["sounds"], defaultNsp);
                    break;
                case "models":
                    resource.ModelMetaList = ModelMetaList.FromXmlNode(document["models"], defaultNsp);
                    break;
                case "fragments":
                    resource.FragmentMetaList = FragmentMetaList.FromXmlNode(document["fragments"]);
                    break;
                case "difficulties":
                    resource.DifficultyMetaList = DifficultyMetaList.FromXmlNode(document["difficulties"], defaultNsp);
                    break;
                case "armors":
                    resource.ArmorMetaList = ArmorMetaList.FromXmlNode(document["armors"], defaultNsp);
                    break;
                case "entities":
                    resource.EntityMetaList = EntityMetaList.FromXmlNode(document["entities"], defaultNsp);
                    break;
                case "shapes":
                    resource.ShapeMetaList = ShapeMetaList.FromXmlNode(document["shapes"], defaultNsp);
                    break;
                case "artifacts":
                    resource.ArtifactMetaList = ArtifactMetaList.FromXmlNode(document["artifacts"], defaultNsp);
                    break;
                case "almanac":
                    resource.AlmanacMetaList = AlmanacMetaList.FromXmlNode(document["almanac"], defaultNsp);
                    break;
                case "notes":
                    resource.NoteMetaList = NoteMetaList.FromXmlNode(document["notes"], defaultNsp);
                    break;
                case "stages":
                    resource.StageMetaList = StageMetaList.FromXmlNode(document["stages"], defaultNsp);
                    break;
                case "maps":
                    resource.MapMetaList = MapMetaList.FromXmlNode(document["maps"], defaultNsp);
                    break;
                case "areas":
                    resource.AreaMetaList = AreaMetaList.FromXmlNode(document["areas"], defaultNsp);
                    break;
                case "stats":
                    resource.StatMetaList = StatMetaList.FromXmlNode(document["stats"], defaultNsp);
                    break;
                case "achievements":
                    resource.AchievementMetaList = AchievementMetaList.FromXmlNode(document["achievements"], defaultNsp);
                    break;
                case "store":
                    resource.StoreMetaList = StoreMetaList.FromXmlNode(document["store"], defaultNsp);
                    break;
                case "archive":
                    resource.ArchiveMetaList = ArchiveMetaList.FromXmlNode(document["archive"], defaultNsp);
                    break;
                case "mainmenuviews":
                    resource.MainmenuViewMetaList = MainmenuViewMetaList.FromXmlNode(document["views"], defaultNsp);
                    break;
                case "musics":
                    resource.MusicMetaList = MusicMetaList.FromXmlNode(document["musics"], defaultNsp);
                    break;
                case "progressbars":
                    resource.ProgressBarMetaList = ProgressBarMetaList.FromXmlNode(document["bars"], defaultNsp);
                    break;
                case "blueprints":
                    resource.BlueprintMetaList = BlueprintMetaList.FromXmlNode(document["blueprints"], defaultNsp);
                    break;
                case "spawns":
                    resource.SpawnMetaList = SpawnMetaList.FromXmlNode(document["spawns"], defaultNsp);
                    break;
                case "chaptertransitions":
                    resource.ChapterTransitionMetaList = ChapterTransitionMetaList.FromXmlNode(document["transitions"], defaultNsp);
                    break;
                case "grids":
                    resource.GridMetaList = GridMetaList.FromXmlNode(document["grids"], defaultNsp);
                    break;
                case "credits":
                    resource.CreditsMetaList = CreditMetaList.FromXmlNode(document["credits"], defaultNsp);
                    break;
                case "arcade":
                    resource.ArcadeMetaList = ArcadeMetaList.FromXmlNode(document["arcade"], defaultNsp);
                    break;
            }
        }
        public static ColliderConstructor LoadColliderConstructor(this XmlNode node)
        {
            var name = node.GetAttribute("name");
            var sizeNode = node["size"];
            var size = Vector3.zero;
            if (sizeNode != null)
            {
                size = new Vector3(
                    sizeNode.GetAttributeFloat("x") ?? 0,
                    sizeNode.GetAttributeFloat("y") ?? 0,
                    sizeNode.GetAttributeFloat("z") ?? 0);
            }
            var offsetNode = node["offset"];
            var offset = Vector3.zero;
            if (offsetNode != null)
            {
                offset = new Vector3(
                    offsetNode.GetAttributeFloat("x") ?? 0,
                    offsetNode.GetAttributeFloat("y") ?? 0,
                    offsetNode.GetAttributeFloat("z") ?? 0);
            }
            var pivotNode = node["pivot"];
            var pivot = Vector3.one * 0.5f;
            if (pivotNode != null)
            {
                pivot = new Vector3(
                    pivotNode.GetAttributeFloat("x") ?? 0.5f,
                    pivotNode.GetAttributeFloat("y") ?? 0.5f,
                    pivotNode.GetAttributeFloat("z") ?? 0.5f);
            }
            return new ColliderConstructor()
            {
                name = name,
                size = size,
                offset = offset,
                pivot = pivot,
            };
        }
    }
}
