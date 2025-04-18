using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiVagabond
{
    public class VfxDestroyyer : MonoBehaviour
    {
        public string AnimationName;
        private Animator myAnim;

        void Awake()
        {
            myAnim = GetComponent<Animator>();
        }

        void Update()
        {
            if (myAnim.GetCurrentAnimatorStateInfo(0).IsName(AnimationName))
            {
                if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                    Destroy(gameObject);
            }
        }
    }
}