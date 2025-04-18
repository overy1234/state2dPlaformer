using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiVagabond
{
	public class MovementController : MonoBehaviour
	{
        #region FIELD

        [SerializeField] private MovementData data;
		[SerializeField] private float lastOnGroundTime;
		[SerializeField] private Transform groundCheckPoint;
		[SerializeField] private Vector2 groundCheckSize = new Vector2(0.49f, 0.03f);
		[SerializeField] private LayerMask groundLayer;
		[SerializeField] private Transform WallCheck;
		[SerializeField] private LayerMask wallLayer;

		[Space(50)]

		[SerializeField] private GameObject dashVfxPf;
		private bool summonDasVfxhOnce;
		[SerializeField] private GameObject jumpAscenVfxPf;
		private bool summonJumpAscenVfxOnce;
		[SerializeField] private GameObject jumpLandingVfxPf;
		private bool summonJumpLandingVfxOnce;
		[SerializeField] private GameObject wallSlideVfxPf;
		private int wallSlideSummonRate;
		[SerializeField] private GameObject wallJumpVfxPf;
		private bool summonWallJumpVfxOnce;

		private Rigidbody2D myBody;
		private Animator myAnim;
		private BoxCollider2D myCollider;

		//Run
		[HideInInspector] public Vector2 move;

		//Jump
		[HideInInspector] public bool grounded;
		private bool jumpButtonPressed;
		[HideInInspector] public bool jumping;

		//Dash
		[HideInInspector] public bool dashing;
		private bool canDash = true;
		private bool dashButton;

		//Wall Slide & Jump
		[HideInInspector] public bool wallJumping;
		[HideInInspector] public bool wallSliding;
		private bool wall;
		private float jumpTime;
		private bool doOnce;

		//Ladder Up & Down
		/*[HideInInspector] */public bool doingLadder;
		private bool collideWLadder;
		private Vector3 ladderUpNDownXPos;

		#endregion

		#region MONOBEHAVIOUR
		void Awake()
		{
			myBody = GetComponent<Rigidbody2D>();
			myAnim = GetComponent<Animator>();
			myCollider = GetComponent<BoxCollider2D>();
		}

		void Update()
		{
			if (dashing || wallJumping)
				return;

			if (wallSliding)
				doOnce = true;

			lastOnGroundTime -= Time.deltaTime;

			//Input handler
				move.x = Input.GetAxisRaw("Horizontal");


			jumpButtonPressed = Input.GetButtonDown("Jump");
			dashButton = Input.GetMouseButtonDown(1);


			if (move.x != 0 && !doingLadder)
				CheckDirectionToFace(move.x > 0);

			Jump();

            if (dashButton && canDash && !wallSliding)
            {
				//Instantiate dash vfx
				if (summonDasVfxhOnce && grounded)
				{
					GameObject tem = Instantiate(dashVfxPf, transform.Find("VfxSpawner").gameObject.transform.Find("Dash").position, Quaternion.identity);

					Vector3 tem2 = transform.localScale;
					tem2.x *= -1f;
					tem.transform.localScale = tem2;

					summonDasVfxhOnce = false;
				}

				StartCoroutine(Dash());
			}

			if(wallSliding && jumpButtonPressed)
            {
				//Instantiate wall jump vfx
                if (summonWallJumpVfxOnce)
                {
					GameObject tem = Instantiate(wallJumpVfxPf, transform.Find("VfxSpawner").Find("WallJump").position, Quaternion.identity);
					Vector3 tem2 = transform.localScale;
					tem2.x *= -1f;
					tem.transform.localScale = tem2;
					summonWallJumpVfxOnce = false;
                }

				StartCoroutine(WallJump());
			}

			LadderUpNDown();

			//Vfx instantiate condition
			if (!dashing)
				summonDasVfxhOnce = true;

			if (!jumping)
				summonJumpAscenVfxOnce = true;

			if (!grounded)
				summonJumpLandingVfxOnce = true;

			if (!wallJumping)
				summonWallJumpVfxOnce = true;

		}

		void FixedUpdate()
        {
			if (dashing || wallJumping)
				return;

			if(!wallSliding && !doingLadder && !GetComponent<AnimationController>().parryIdle && !GetComponent<AnimationController>().parryHit && !myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack01") && !myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack02") && !myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack03"))
				Run(1);

			WallSlide();

			//checks if set box overlaps with ground
			Collider2D groundCollider = Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer);
			if (groundCollider)
			{
				float PlayerDownPoint = (transform.position.y + myCollider.offset.y) - (myCollider.size.y / 2);
				float GroundUpPoint = (groundCollider.gameObject.transform.position.y + groundCollider.gameObject.GetComponent<BoxCollider2D>().offset.y) + (groundCollider.gameObject.GetComponent<BoxCollider2D>().size.y / 2);

				if ((PlayerDownPoint >= GroundUpPoint) && (myBody.linearVelocity.y <= 0) && !doingLadder)
				{
					lastOnGroundTime = 0.1f;
					grounded = true;
				}	

			}
            else
            {
				grounded = false;
			}
		}
        #endregion

        #region RUN
        private void Run(float lerpAmount)
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
		private void Jump()
		{ 
			if (grounded)
            {
				jumping = false;

                //Instantiate jump landing vfx
                if (summonJumpLandingVfxOnce && !doingLadder && !wallSliding)
                {
					Instantiate(jumpLandingVfxPf, transform.Find("VfxSpawner").transform.Find("JumpAscen&Landing").position, Quaternion.identity);
					summonJumpLandingVfxOnce = false;
				}
			}

			if (grounded && jumpButtonPressed)
            {
                //Instantiate jump ascending vfx
                if (summonJumpAscenVfxOnce && !doingLadder)
                {
					Instantiate(jumpAscenVfxPf, transform.Find("VfxSpawner").gameObject.transform.Find("JumpAscen&Landing").position, Quaternion.identity);
					summonJumpAscenVfxOnce = false;
				}

				myBody.linearVelocity = new Vector2(myBody.linearVelocity.x, data.jumpPower);
				jumping = true;
			}
        }
        #endregion

        #region DASH

		private IEnumerator Dash()
        {
			dashing = true;
			canDash = false;

			float oriGrav = myBody.gravityScale;
			myBody.gravityScale = 0;
			myBody.linearVelocity = new Vector2(myBody.transform.localScale.x * data.dashPower, 0f);

			yield return new WaitForSeconds(data.dashingTime);
			
			if (move.x > 0)
				myBody.linearVelocity = new Vector2(data.runMaxSpeed, myBody.linearVelocity.y);
			else if(move.x < 0)
				myBody.linearVelocity = new Vector2(-data.runMaxSpeed, myBody.linearVelocity.y);

			myBody.gravityScale = oriGrav;

			dashing = false;
			yield return new WaitForSeconds(data.dashingCoolDown);
			canDash = true;
		}

        #endregion

        #region WALLSIDE&JUMP

		private void WallSlide()
        {
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
			else if (jumpTime <= Time.fixedTime)
				wallSliding = false;
			else
				wallSliding = false;

			if (wallSliding)
				myBody.linearVelocity = new Vector2(myBody.linearVelocity.x, Mathf.Clamp(myBody.linearVelocity.y, -data.wallSlideSpeed, float.MaxValue));

			//Instantiate wall slide vfx
			wallSlideSummonRate++;

			if (wallSliding && wallSlideSummonRate % 10 == 0)
			{
				Instantiate(wallSlideVfxPf, transform.Find("VfxSpawner").Find("WallSlide").position, Quaternion.identity); ;
			}
		}

		private IEnumerator WallJump()
        {
			wallSliding = false;
			wallJumping = true;
			if (transform.localScale.x == -1f)
				myBody.linearVelocity = new Vector2(data.wallJumpingXPower, data.wallJumpingYPower);
			else
				myBody.linearVelocity = new Vector2(-data.wallJumpingXPower, data.wallJumpingYPower);
			yield return new WaitForSeconds(data.wallJumpTimeInSecond);

			

			wallJumping = false;


			myAnim.SetBool("WallJump", false);

			if (doOnce)
            {
				doOnce = false;
				Vector3 tem = transform.localScale;
				tem.x *= -1f;

				transform.localScale = tem;
				
            }
		}
        #endregion

        #region LADDERUP&DOWN

		private void LadderUpNDown()
        {
			


			if (Input.GetKeyDown(KeyCode.L) && collideWLadder && !doingLadder)
			{
				doingLadder = true;

				//Set gravity scale and velocity to zero
				myBody.linearVelocity = new Vector3(0, 0, 0);
				myBody.gravityScale = 0f;
				
				ladderUpNDownXPos.y = transform.position.y;
				ladderUpNDownXPos.z = transform.position.z;
				transform.position = ladderUpNDownXPos;
			}
			else if (Input.GetKeyDown(KeyCode.L) && collideWLadder && doingLadder)
				doingLadder = false;
			else if (!collideWLadder)
				doingLadder = false;


			if (doingLadder)
			{
				

				if (Input.GetKey(KeyCode.W))
				{
					myAnim.SetFloat("LadderUp&DownShouldPause", 1);
					transform.position += new Vector3(0, 1, 0) * data.ladderUpNDownPower * Time.deltaTime;
				}
				else if (Input.GetKey(KeyCode.S))
				{
					myAnim.SetFloat("LadderUp&DownShouldPause", -1);
					transform.position += new Vector3(0, -1, 0) * data.ladderUpNDownPower * Time.deltaTime;
				}
				else
					myAnim.SetFloat("LadderUp&DownShouldPause", 0);
			}
            else
            {
				myCollider.isTrigger = false;
				myBody.gravityScale = 2f;
            }
			
        }

        #endregion

        #region OTHER

        private void CheckDirectionToFace(bool isMovingRight)
		{
			Vector3 tem = transform.localScale;
			if (!isMovingRight)
				tem.x = -1f;
			else
				tem.x = 1f;
			transform.localScale = tem;
		}

        private void OnTriggerStay2D(Collider2D collision)
        {
			if (collision.CompareTag("Ladder"))
            {
				collideWLadder = true;
				ladderUpNDownXPos.x = collision.gameObject.transform.parent.position.x;
			}
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
			collideWLadder = false;
        }

        #endregion
    }
}