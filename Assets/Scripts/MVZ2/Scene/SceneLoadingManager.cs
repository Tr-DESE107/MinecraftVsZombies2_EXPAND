﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace MVZ2.Scenes
{
    public class SceneLoadingManager : MonoBehaviour
    {
        public async Task<SceneInstance> LoadSceneAsync(string name, LoadSceneMode mode)
        {
            var op = Addressables.LoadSceneAsync(name, mode);
            var scene = await op.Task;
            sceneCaches.Add(scene);
            return scene;
        }
        public Task UnloadSceneAsync(string name)
        {
            var scene = GetSceneInstance(name);
            if (scene.Scene.IsValid())
            {
                return UnloadSceneAsync(scene);
            }
            return Task.CompletedTask;
        }
        public async Task UnloadSceneAsync(SceneInstance scene)
        {
            var op = Addressables.UnloadSceneAsync(scene);
            await op.Task;
            sceneCaches.Remove(scene);
        }
        public bool IsSceneLoaded(string name)
        {
            var instance = GetSceneInstance(name);
            return instance.Scene.IsValid();
        }
        public SceneInstance GetSceneInstance(string name)
        {
            return sceneCaches.FirstOrDefault(s => s.Scene.name == name);
        }
        List<SceneInstance> sceneCaches = new List<SceneInstance>();
    }
}
