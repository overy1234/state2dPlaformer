using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiVagabond
{
	public class AudioController : MonoBehaviour
	{
        #region FIELD

        MovementController samuraiMV;
		AudioManager myAudio;
		Animator myAnim;

		public AudioSource Run;

		private bool dashCanPlay;
		private bool jumpCanPlay;
		private bool jumpLandingCanPlay;
		private bool wallJumpCanPlay;
		private bool BAtk1;
		private bool BAtk2;
		private bool BAtk3;
		private bool deathCanPlay;

		#endregion

		void Awake()
		{ 
			samuraiMV = GetComponent<MovementController>();
			myAudio = FindObjectOfType<AudioManager>();
			myAnim = GetComponent<Animator>();
		}

		void Update()
		{
			#region RUN&IDLE
			//Set run sound FX
			if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Run") && !samuraiMV.dashing)
				Run.UnPause();
			else
				Run.Pause();
			#endregion

			#region DASH
			//Set Dash sound FX
			if (!samuraiMV.dashing)
				dashCanPlay = true;

			if (samuraiMV.dashing && dashCanPlay)
            {
				myAudio.play("Dash");
				dashCanPlay = false;
			}
			#endregion

			#region JUMP

			//Set Jump Ascending sound FX
			if (!samuraiMV.jumping)
				jumpCanPlay = true;

			if (samuraiMV.jumping && jumpCanPlay && !samuraiMV.doingLadder)
			{
				myAudio.play("JumpAscending");
				jumpCanPlay = false;
			}

			//Set Jump Landing sound FX
			if (samuraiMV.wallSliding)
				jumpLandingCanPlay = false;

			if (!samuraiMV.grounded)
				jumpLandingCanPlay = true;

			if(samuraiMV.grounded && jumpLandingCanPlay)
            {
				myAudio.play("JumpLanding");
				jumpLandingCanPlay = false;
            }
			#endregion

			#region WALLSLIDE&WALLJUMP

			if (!samuraiMV.wallJumping)
				wallJumpCanPlay = true;

            if (samuraiMV.wallJumping && wallJumpCanPlay)
            {
				myAudio.play("WallJump");
				wallJumpCanPlay = false;
            }

			#endregion

			#region BASICATTACK

			//Set basic attack 1 sound FX
			if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack01"))
				BAtk1 = true;

            if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack01") && BAtk1)
            {
				myAudio.play("BasicAttack1");
				BAtk1 = false;
            }

			//Set basic attack 2 sound FX
			if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack02"))
				BAtk2 = true;

			if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack02") && BAtk2)
			{
				myAudio.play("BasicAttack2");
				BAtk2 = false;
			}

			//Set basic attack 3 sound FX
			if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack03"))
				BAtk3 = true;

			if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack03") && BAtk3)
			{
				myAudio.play("BasicAttack3");
				BAtk3 = false;
			}

			#endregion

			#region 

			if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("Death"))
				deathCanPlay = true;

            if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Death") && deathCanPlay)
            {
				myAudio.play("Death");
				deathCanPlay = false;
            }

            #endregion
        }
    }
}
