using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SamuraiVagabond
{
	public class AnimationController : MonoBehaviour
	{
        #region FIELD
        private Animator myAnim;
		private MovementController samuraiMv;

		private const string speed = "Speed";
		private const string dash = "Dash";
		private const string Xvelocity = "Xvelocity";
		private const string idleRunTrans = "IdleRunTrans";
		private const string runIdleTrans = "RunIdleTrans";
		private const string grounded = "Grounded";
		private const string yVelocity = "Yvelocity";
		private const string wallSlide = "WallSlide";
		private const string walljump = "WallJump";
		private const string hurt = "Hurt";
		private const string hurtEnd = "HurtEnd";
		private const string death = "Death";
		private const string deathEnd = "DeathEnd";
		private const string ladderUpNDown = "LadderUp&Down";
		private const string parryIdleName = "ParryIdle";
		private const string parryHitName = "ParryHit";

		[HideInInspector] public bool Hurt;
		[HideInInspector] public bool Death;

		[HideInInspector] public bool parryIdle;
		[HideInInspector] public bool parryHit;
        #endregion

        #region MONOBEHAVIOUR
        void Awake()
		{
			myAnim = GetComponent<Animator>();
			samuraiMv = GetComponent<MovementController>();
		}


		void Update()
		{

			#region IDLE&RUN

			//Set Idle and Run Animation
			myAnim.SetFloat(speed, Mathf.Abs(samuraiMv.move.x));

            if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("IdleRunTrans"))
            {
				myAnim.SetBool(idleRunTrans, true);
				if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
					myAnim.SetBool(idleRunTrans, false);
            }

			if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("RunIdleTrans"))
			{
				myAnim.SetBool(runIdleTrans, true);
				if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
					myAnim.SetBool(runIdleTrans, false);
			}

			#endregion

			#region DASH

			myAnim.SetBool(dash, samuraiMv.dashing);
			myAnim.SetFloat(Xvelocity, Mathf.Abs(GetComponent<Rigidbody2D>().linearVelocity.x));

			#endregion

			#region JUMP

			myAnim.SetBool(grounded, samuraiMv.grounded);
			myAnim.SetFloat(yVelocity, GetComponent<Rigidbody2D>().linearVelocity.y);

			#endregion

			#region WALLSLIDE&JUMP

			myAnim.SetBool(wallSlide, samuraiMv.wallSliding);
			myAnim.SetBool(walljump, samuraiMv.wallJumping);

			#endregion

			#region HURT&DEATH

			//Set hurt animation
			if (Input.GetKeyDown(KeyCode.H) && !parryHit && !parryIdle)
            {
				myAnim.SetTrigger(hurt);
				Hurt = true;
			}
				
            if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Hurt") && samuraiMv.grounded)
            {
				if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
					Hurt = false;
					myAnim.SetTrigger(hurtEnd);
				}
					
            }

            //Set death animation
            if (Input.GetKeyDown(KeyCode.X) && !parryHit && !parryIdle)
            {
				Death = true;
				myAnim.SetTrigger(death);
            }

            if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            {
				if(myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= .8f)
                {
					Death = false;
					myAnim.SetTrigger(deathEnd);
                }
            }

			#endregion

			#region LADDERUP&DOWN

			myAnim.SetBool(ladderUpNDown, samuraiMv.doingLadder);

            #endregion

            #region Parry

			//Set parry idle animation
            if (Input.GetKeyDown(KeyCode.P) && !parryHit && samuraiMv.grounded)
            {
				GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, 0);
				parryIdle = !parryIdle;
            }

			myAnim.SetBool(parryIdleName, parryIdle);

			//Set parry hit animation
			if(Input.GetKeyDown(KeyCode.H) && parryIdle)
            {
				parryHit = true;
            }

            if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("ParryHit"))
            {
				if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
					parryHit = false;
            }

			myAnim.SetBool(parryHitName, parryHit);
            #endregion 
        }
        #endregion
    }
}