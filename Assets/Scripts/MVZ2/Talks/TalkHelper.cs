using System;
using System.Threading.Tasks;
using MVZ2.Talk;
using PVZEngine;

namespace MVZ2.Talks
{
    public static class TalkHelper
    {
        public static void SimpleStartTalk(this TalkController controller, NamespaceID groupId, int section, float delay = 0, Action onSkipped = null, Action onStarted = null, Action onEnd = null)
        {
            if (!controller.CanStartTalk(groupId, section))
            {
                onEnd?.Invoke();
                return;
            }
            if (controller.WillSkipTalk(groupId, section))
            {
                controller.SkipTalk(groupId, section, () =>
                {
                    onSkipped?.Invoke();
                    onEnd?.Invoke();
                });
            }
            else
            {
                onStarted?.Invoke();
                controller.StartTalk(groupId, section, delay, onEnd);
            }
        }
        public static async Task<TalkResult> SimpleStartTalkAsync(this TalkController controller, NamespaceID groupId, int section, float delay = 0, Action onStarted = null)
        {
            if (!controller.CanStartTalk(groupId, section))
                return TalkResult.NotStarted;

            if (controller.WillSkipTalk(groupId, section))
            {
                await controller.SkipTalkAsync(groupId, section);
                return TalkResult.Skipped;
            }

            onStarted?.Invoke();
            await controller.StartTalkAsync(groupId, section, delay);
            return TalkResult.Started;
        }
    }
    public enum TalkResult
    {
        NotStarted,
        Skipped,
        Started
    }
}
