using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FemaleSamurai
{
	public class AttackMethod : MonoBehaviour
	{
		private bool attackButtonPressed;
		private Animator myAnim;
		private Rigidbody2D myBody;


		private bool clickedInAnimation1;
		private bool clickedInAnimation2;
		private bool clickedInAnimation3;


		void Awake()
		{
			myAnim = GetComponent<Animator>();
			myBody = GetComponent<Rigidbody2D>();
		}

		void Update()
		{
			if (GetComponent<FSMovement>().wallSliding || GetComponent<FSMovement>().wallJump || myAnim.GetBool("Hurt") || myAnim.GetBool("Death"))
				return;

			//Input Handler
			attackButtonPressed = Input.GetMouseButtonDown(0);

			AttackCombo();

		}

		void FixedUpdate()
		{
			if ((myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack01") || myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack02") || myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack03")))
			{
				if (transform.localScale.x > 0)
					myBody.linearVelocity = new Vector2(0.5f, myBody.linearVelocity.y);
				else
					myBody.linearVelocity = new Vector2(-0.5f, myBody.linearVelocity.y);
			}
		}

		#region COMBO MECHANIC

		void AttackCombo()
		{

			if (attackButtonPressed && !myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack01") && !myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack02") && !myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack03"))
			{
				myAnim.SetTrigger("Attack1");
			}

			//Check if attackButton is pressed while attack1 animation is playying, if so play attack2
			if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack01"))
			{
				if (attackButtonPressed)
					clickedInAnimation1 = true;


				if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && clickedInAnimation1)
				{
					clickedInAnimation1 = false;
					myAnim.SetTrigger("Attack2");
				}
				else if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !clickedInAnimation1)
				{
					myAnim.SetTrigger("NotAttacking");
				}

			}

			//Check if attackButton is pressed while attack2 animation is playying, if so play attack3
			if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack02"))
			{
				if (attackButtonPressed)
					clickedInAnimation2 = true;


				if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && clickedInAnimation2)
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
				if (attackButtonPressed)
					clickedInAnimation3 = true;


				if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && clickedInAnimation3)
				{
					myAnim.SetTrigger("Attack1");
					clickedInAnimation3 = false;

				}
				else if (myAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !clickedInAnimation3)
				{
					myAnim.SetTrigger("NotAttacking");
				}
			}
		}
        #endregion

        
    }
}

