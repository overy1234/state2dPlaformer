using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FemaleSamurai
{
	public class FSAnimationController : MonoBehaviour
	{
		#region FIELD

		Animator myAnim;
		FSMovement FSMv;

		private const string speed = "Speed";
		private const string idleRunTrans = "IdleRunTrans";
		private const string RunIdleTrans = "RunIdleTrans";
		private const string yVelocity = "YVelocity";
		private const string grounded = "Grounded";
		private const string wallSliding = "WallSliding";
		private const string wallJump = "WallJump";
		private const string hurt = "Hurt";
		private const string death = "Death";
		private const string parryIdleName = "ParryIdle";
		private const string parryHitName = "ParryHit";


		[HideInInspector] public bool parryIdle;
		[HideInInspector] public bool parryHit;
        #endregion

        void Start()
		{
			FSMv = GetComponent<FSMovement>();
			myAnim = GetComponent<Animator>();
		}


		void Update()
		{
			#region IDLE&RUN

			myAnim.SetFloat(speed, Mathf.Abs(FSMv.move.x));

			// Set Idle Run Transition Animation
			if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("IdleRunTransition"))
			{
				myAnim.SetBool(idleRunTrans, true);

				if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
					myAnim.SetBool(idleRunTrans, false);
			}

			// Set Run Idle Transition Animation
			if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("RunIdleTransition"))
			{
				myAnim.SetBool(RunIdleTrans, true);

				if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
					myAnim.SetBool(RunIdleTrans, false);
			}

			#endregion

			#region JUMP

			myAnim.SetFloat(yVelocity, GetComponent<Rigidbody2D>().linearVelocity.y);
			myAnim.SetBool(grounded, FSMv.grounded);

			#endregion

			#region WALLSLIDING

			myAnim.SetBool(wallSliding, FSMv.wallSliding);
			myAnim.SetBool(wallJump, FSMv.wallJump);

			#endregion

			#region DEATH & HURT

			//Set Hurt Animation
			if ((!parryIdle && !parryHit) && Input.GetKeyDown(KeyCode.H) && FSMv.grounded)
			{
				GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0f, 0f);
				myAnim.SetBool(hurt, true);
			}

			if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Hurt"))
			{
				if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
					myAnim.SetBool(hurt, false);
			}

			//Set Death Animation
			if ((!parryIdle && !parryHit) && Input.GetKeyDown(KeyCode.X) && FSMv.grounded)
				myAnim.SetBool(death, true);


			if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Death"))
			{
				if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
				{
					myAnim.SetBool(death, false);
				}
			}

			#endregion

			#region PARRY


			//Set parry idle animation
			if (Input.GetKeyDown(KeyCode.P))
            {
				GetComponent<Rigidbody2D>().linearVelocity = new Vector3(0, 0, 0);
				parryIdle = !parryIdle;
			}

			myAnim.SetBool(parryIdleName, parryIdle);

			//Set parry hit animation
			if(parryIdle && Input.GetKeyDown(KeyCode.H))
            {
				parryIdle = false;
				parryHit = true;
			}
				
            if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("ParryHit"))
            {
				if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
					parryIdle = true;
					parryHit = false;
				}
					
            }

			myAnim.SetBool(parryHitName, parryHit);
			#endregion
		}
	}
}
