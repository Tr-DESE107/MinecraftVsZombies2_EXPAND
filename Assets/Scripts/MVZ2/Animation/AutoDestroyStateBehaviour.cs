using UnityEngine;

namespace MVZ2.Animation
{
    public class AutoDestroyStateBehaviour : StateMachineBehaviour
    {
        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Destroy(animator.gameObject);
        }
    }
}
