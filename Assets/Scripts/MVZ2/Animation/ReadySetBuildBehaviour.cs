using MVZ2.Level.UI;
using UnityEngine;

namespace MVZ2.Animation
{
    public class ReadySetBuildBehaviour : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var preset = animator.GetComponent<LevelUIPreset>();
            if (!preset)
                return;
            preset.CallStartGame();
        }
    }
}
