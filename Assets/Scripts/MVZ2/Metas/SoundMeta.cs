﻿using System.Xml;
using MVZ2.IO;
using UnityEngine;

namespace MVZ2.Metas
{
    public class SoundMeta
    {
        public string name;
        public AudioSample[] samples;
        public int priority;
        public int maxCount;
        public float loopPitchStart;
        public float loopPitchEnd;
        public float loopFadeInSpeed;
        public float loopFadeOutSpeed;

        public static SoundMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var name = node.GetAttribute("name");
            var priority = node.GetAttributeInt("priority") ?? 128;
            var maxCount = node.GetAttributeInt("maxCount") ?? 2;
            var loopPitchStart = node.GetAttributeFloat("loopPitchStart") ?? 1;
            var loopPitchEnd = node.GetAttributeFloat("loopPitchEnd") ?? 1;
            var loopFadeInSpeed = node.GetAttributeFloat("loopFadeInSpeed") ?? 1;
            var loopFadeOutSpeed = node.GetAttributeFloat("loopFadeOutSpeed") ?? 1;
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
                loopPitchStart = loopPitchStart,
                loopPitchEnd = loopPitchEnd,
                loopFadeInSpeed = loopFadeInSpeed,
                loopFadeOutSpeed = loopFadeOutSpeed,
                samples = samples,
            };
        }
        public AudioSample GetRandomSample()
        {
            return samples[Random.Range(0, samples.Length)];
        }
    }
}
