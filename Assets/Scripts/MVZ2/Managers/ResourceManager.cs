using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public class ResourceManager : MonoBehaviour
    {
        public Model GetModel(NamespaceID id)
        {
            var resource = models.FirstOrDefault(m => m.id == id);
            if (resource == null)
                return null;
            return resource.resource;
        }
        public AudioResource GetAudioResource(NamespaceID id)
        {
            return audios.FirstOrDefault(m => m.id == id);
        }
        public void SetModelResources(ModelResource[] items)
        {
            models.Clear();
            models.AddRange(items);
        }
        public void SetAudioResources(AudioResource[] items)
        {
            audios.Clear();
            audios.AddRange(items);
        }
        public MainManager Main => main;
        public string EditorSoundDirectory => editorSoundDirectory;
        public string EditorModelDirectory => editorModelDirectory;
        [SerializeField]
        private MainManager main;
        [SerializeField]
        private string editorSoundDirectory;
        [SerializeField]
        private string editorModelDirectory;
        [SerializeField]
        private List<AudioResource> audios;
        [SerializeField]
        private List<ModelResource> models;
    }

    [Serializable]
    public class ModelResource
    {
        public NamespaceID id;
        public Model resource;
    }
}
