using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiVagabond
{
	[CreateAssetMenu(menuName = "Samurai Movement Data")]
	public class MovementData : ScriptableObject
	{
		[Header("Run")]
		public float runMaxSpeed; //Target speed we want the player to reach.
		public float runAcceleration; //Time (approx.) time we want it to take for the player to accelerate from 0 to the runMaxSpeed.
		[HideInInspector] public float runAccelAmount; //The actual force (multiplied with speedDiff) applied to the player.
		public float runDecceleration; //Time (approx.) we want it to take for the player to accelerate from runMaxSpeed to 0.
		[HideInInspector] public float runDeccelAmount; //Actual force (multiplied with speedDiff) applied to the player 
		[Space(10)]
		[Range(0.01f, 1)] public float accelInAir; //Multipliers applied to acceleration rate when airborne.
		[Range(0.01f, 1)] public float deccelInAir;
		public bool doConserveMomentum;

		[Space(20)]

		[Header("Jump")]
		public float jumpPower;

		[Space(20)]

		[Header("Dash")]
		public float dashPower = 10;
		public float dashingTime = 0.2f;
		public float dashingCoolDown = 0.5f;

		[Space(20)]

		[Header("WallSlide&Jump")]
		public float wallSlideSpeed;
		[HideInInspector] public float wallJumpTime = 0.2f;
		public float wallJumpingXPower;
		public float wallJumpingYPower;
		public float wallJumpTimeInSecond;

		[Space(20)]

		[Header("LadderUp&Down")]
		public float ladderUpNDownPower = 0.4f;

		private void OnValidate()
		{

			runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
			runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;



			#region Variable Ranges
			runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
			runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
			#endregion
		}
	}
}