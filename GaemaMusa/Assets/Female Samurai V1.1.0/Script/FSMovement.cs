using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FemaleSamurai
{
	public class FSMovement : MonoBehaviour
	{
		#region FIELD

		[SerializeField] private FSMovementData data;
		[SerializeField] private float lastOnGroundTime;
		[SerializeField] private Transform groundCheckPoint;
		[SerializeField] private Transform WallCheck;
		[SerializeField] private Vector2 groundCheckSize = new Vector2(0.50f, 0.07f);
		[SerializeField] private LayerMask groundLayer;
		[SerializeField] private LayerMask wallLayer;

		[HideInInspector] public Vector2 move;

		private Rigidbody2D myBody;
		private Animator myAnim;

		//Jump
		[HideInInspector] public bool grounded;
		[HideInInspector] public bool isJumping;
		private bool jumpButtonPressed;

		//Wall Sliding and Wall Jump
		[HideInInspector] public bool wallJump;
		[HideInInspector] public bool wallSliding;
		private bool wall;
		private float jumpTime;
		private bool DKey;
		private bool AKey;

		#endregion

		#region MONO BEHAVIOUR
		void Awake()
		{
			myBody = GetComponent<Rigidbody2D>();
			myAnim = GetComponent<Animator>();

		}
		void Update()
		{
			if (wallJump || myAnim.GetCurrentAnimatorStateInfo(0).IsName("Hurt") || myAnim.GetCurrentAnimatorStateInfo(0).IsName("Death"))
				return;

			//See if D or Right button pressed or released 
			if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
				DKey = true;
			else if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
				DKey = false;

			//See if A or Left button presed or released
			if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
				AKey = true;
			else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
				AKey = false;


			//Input Handler
			move.x = Input.GetAxisRaw("Horizontal");
			jumpButtonPressed = Input.GetButtonDown("Jump");


			lastOnGroundTime -= Time.deltaTime;
			//Debug.Log(grounded);		

			if ((move.x != 0) && !wallJump && !wallSliding)
				CheckDirectionToFace(move.x > 0);

			jump();


			if (wallSliding && jumpButtonPressed)
				StartCoroutine(wallJumpMechanic());
		}

		void FixedUpdate()
		{
			if (wallJump || myAnim.GetCurrentAnimatorStateInfo(0).IsName("Death") || myAnim.GetCurrentAnimatorStateInfo(0).IsName("Hurt"))
				return;

			if (!GetComponent<FSAnimationController>().parryIdle && !GetComponent<FSAnimationController>().parryHit && !(wallSliding && (transform.localScale.x > 0 && DKey) && !(wallSliding && transform.localScale.x < 0 && AKey)))
				run(1);


			//checks if set box overlaps with ground
			if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer))
			{
				lastOnGroundTime = 0.1f;
				grounded = true;
			}
			else
				grounded = false;


			WallSlidngMechanic();
		}

		#endregion

		#region RUN
		private void run(float lerpAmount)
		{
			float targetSpeed = move.x * data.runMaxSpeed;

			float accelRate;

			targetSpeed = Mathf.Lerp(myBody.linearVelocity.x, targetSpeed, lerpAmount);

			//Calculate Acceleration and Decceleration
			if (lastOnGroundTime > 0)
				accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.runAccelAmount : data.runDeccelAmount;
			else
				accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.runAccelAmount * data.accelInAir : data.runDeccelAmount * data.deccelInAir;

			if (data.doConserveMomentum && Mathf.Abs(myBody.linearVelocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(myBody.linearVelocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && lastOnGroundTime < 0)
				accelRate = 0;

			float speedDif = targetSpeed - myBody.linearVelocity.x;
			float movement = speedDif * accelRate;

			//Implementing run
			myBody.AddForce(movement * Vector2.right, ForceMode2D.Force);
		}
		#endregion

		#region JUMP
		private void jump()
		{
			if (grounded)
				isJumping = false;

			if (jumpButtonPressed && grounded)
			{
				myBody.linearVelocity = new Vector2(myBody.linearVelocity.x, data.jumpHeight);
				isJumping = true;
			}
		}
		#endregion

		#region Wall Sliding and Wall Jump
		private void WallSlidngMechanic()
		{

			//make sensor for checking wall 
			if (transform.localScale.x > 0)
			{
				wall = Physics2D.OverlapBox(WallCheck.position, new Vector2(0.05f, 0.05f), 0, wallLayer);
			}
			else
			{
				wall = Physics2D.OverlapBox(WallCheck.position, new Vector2(0.05f, 0.05f), 0, wallLayer);
			}

			if (!grounded && wall)
			{
				wallSliding = true;
				jumpTime = Time.fixedTime + data.wallJumpTime;
			}
			else if (jumpTime < Time.time)
				wallSliding = false;
			else
				wallSliding = false;

			//wall sliding mechanic
			if (wallSliding)
				myBody.linearVelocity = new Vector2(myBody.linearVelocity.x, Mathf.Clamp(myBody.linearVelocity.y, -data.wallSlideSpeed, float.MaxValue));
		}

		//Implementing wall jumping
		private IEnumerator wallJumpMechanic()
		{
			wallJump = true;
			if (transform.localScale.x == -1f)
				myBody.linearVelocity = new Vector2(data.wallJumpingXPower, data.wallJumpingYPower);
			else
				myBody.linearVelocity = new Vector2(-data.wallJumpingXPower, data.wallJumpingYPower);
			yield return new WaitForSeconds(data.WallJumpTimeInSecond);
			wallJump = false;
		}
		#endregion

		#region OTHER

		//Direction to face handler
		public void CheckDirectionToFace(bool isMovingRight)
		{
			Vector3 tem = transform.localScale;
			if (isMovingRight)
				tem.x = 1;
			else
				tem.x = -1;
			transform.localScale = tem;
		}
		#endregion
	}
}

