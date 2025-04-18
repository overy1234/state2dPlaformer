using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiVagabond
{
	public class SamuraiAttackController : MonoBehaviour
	{
        #region FIELD

		private Animator myAnim;
		private Rigidbody2D myBody;
		private MovementController SamuraiMv;

		private const float rangeplayerShouldMoveInAtk03 = 0.2f;

		public bool clickedInAnimation1;
		public bool clickedInAnimation2;
		public bool clickedInAnimation3;

        #endregion

        #region MONOBEHAIOUR
        void Awake()
		{
			myAnim = GetComponent<Animator>();
			myBody = GetComponent<Rigidbody2D>();
			SamuraiMv = GetComponent<MovementController>();
		}

		void Update()
		{
			if (SamuraiMv.wallSliding || SamuraiMv.wallJumping)
				return;


			AttackCombo();
		}

		void FixedUpdate()
		{

		}
		#endregion


		#region COMBO MECHANIC

		//Combo Mechanic
		void AttackCombo()
		{

			if (Input.GetMouseButtonDown(0) && !myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack01") && !myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack02") && !myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack03"))
			{
				myBody.linearVelocity = new Vector3(0, 0, 0);
				myAnim.SetTrigger("Attack1");
			}

			//Check if attackButton is pressed while attack1 animation is playying, if so play attack2
			if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack01"))
			{
				if (Input.GetMouseButtonDown(0))
					clickedInAnimation1 = true;


				if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && clickedInAnimation1)
				{
					clickedInAnimation1 = false;
					myAnim.SetTrigger("Attack2");
				}
				else if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7 && !clickedInAnimation1)
				{
					myAnim.SetTrigger("NotAttacking");
				}

			}

			//Check if attackButton is pressed while attack2 animation is playying, if so play attack3
			if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack02"))
			{
				if (Input.GetMouseButtonDown(0))
					clickedInAnimation2 = true;


				if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && clickedInAnimation2)
				{
					clickedInAnimation2 = false;
					myAnim.SetTrigger("Attack3");
				}
				else if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !clickedInAnimation2)
				{
					myAnim.SetTrigger("NotAttacking");
				}
			}

			//Check if attackButton is pressed while attack3 animation is playying if so play attack1
			if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack03"))
			{
				if (Input.GetMouseButtonDown(0))
					clickedInAnimation3 = true;


				if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && clickedInAnimation3)
				{
					myAnim.SetTrigger("Attack1");
					clickedInAnimation3 = false;

				}
				else if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !clickedInAnimation3)
				{
					myAnim.SetTrigger("NotAttacking");
				}
			}
		}
        #endregion

        #region ELSE

		private void Atk03AnimEvent()
        {
			transform.position += new Vector3( rangeplayerShouldMoveInAtk03 * transform.localScale.x, 0, 0);
        }

        #endregion
    }
}
