using System;
using UnityEngine;

namespace PVZEngine.Level
{
    public partial class LevelEngine
    {
        public void BeginLevel(string transition)
        {
            OnBeginLevel?.Invoke(transition);
        }
        public void ShowMoney()
        {
            OnShowMoney?.Invoke();
        }
        public void ShowDialog(string title, string content, string[] options, Action<int> onConfirm = null)
        {
            OnShowDialog?.Invoke(title, content, options, onConfirm);
        }
        public void Stop()
        {
            OnStop?.Invoke();
        }
        public void ShakeScreen(float startAmplitude, float endAmplitude, int time)
        {
            OnShakeScreen?.Invoke(startAmplitude, endAmplitude, time);
        }
        public void PlaySound(NamespaceID id, Vector3 position, float pitch = 1)
        {
            OnPlaySoundPosition?.Invoke(id, position, pitch);
        }
        public void PlaySound(NamespaceID id, float pitch = 1)
        {
            OnPlaySound?.Invoke(id, pitch);
        }
        public event Action<string> OnBeginLevel;
        public event Action OnShowMoney;
        public event Action<string, string, string[], Action<int>> OnShowDialog;
        public event Action OnStop;
        public event Action<float, float, int> OnShakeScreen;
        public event Action<NamespaceID, Vector3, float> OnPlaySoundPosition;
        public event Action<NamespaceID, float> OnPlaySound;
    }
}
