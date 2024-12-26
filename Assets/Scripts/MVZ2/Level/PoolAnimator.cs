using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVZ2.Models;
using UnityEngine;

namespace MVZ2.Level
{
    [ExecuteAlways]
    public class PoolAnimator : MonoBehaviour
    {
        private void Update()
        {
            if (!poolElement)
                return;
            poolElement.SetFloat("_WarpTime", warpTime);
            poolElement.SetFloat("_CausticTime", causticTime);
            if (animator)
            {
                animator.SetFloat("WarpSpeed", warpSpeed);
            }
        }
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private RendererElement poolElement;
        [Range(0, 1)]
        [SerializeField]
        private float warpTime;
        [Range(1, 3)]
        [SerializeField]
        private float warpSpeed;
        [Range(0, 1)]
        [SerializeField]
        private float causticTime;
    }
}
