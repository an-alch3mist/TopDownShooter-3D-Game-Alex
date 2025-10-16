using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using SPACE_UTIL;

namespace SPACE_TopDownShooter
{
	/// <summary>
	/// Calculates where the player is aiming in world space by raycasting from screen space.
	/// IMPORTANT: Must run in LateUpdate AFTER CinemachineBrain updates the camera.
	/// See Script Execution Order settings - this script should have higher execution order than CinemachineBrain.
	/// </summary>
	public class PlayerAimCalculation : MonoBehaviour
	{
		[SerializeField] PlayerInput _playerInput;
		[SerializeField] LayerMask _aimLayerMask;
		[SerializeField] Transform _aimTr;
		[SerializeField] Transform _playerTr;

		public Vector3 getAimPos { get { return this._aimTr.position; } }

		#region Unity Life Cycle
		private void Start()
		{
			Debug.Log("Start(): " + this);
			this.InitIAEvents();
		}
		[Header("just to log")]
		private Vector2 inputAimPos;
		private void InitIAEvents()
		{
			var _IA = this._playerInput.IA;

			_IA.Character.Aim.performed += (ctx) => this.inputAimPos = ctx.ReadValue<Vector2>();
			_IA.Character.Aim.canceled += (ctx) => this.inputAimPos = Vector2.zero;
		} 

		private void LateUpdate()
		{
			/*
				+ All Calculation and Movement are made (done inside Update())
				+ Camera is positioned where to be viewed
				+ Ray is casted from Camera toward mouse pos to get the coordinates after cam movement is made
			*/
			// make sure camera has been moved(possibly inside LateUpdate) before this calculation is made in this frame.
			#region Calculate Aim Pos After Positive That Camera Has Made Its Move
			Ray ray = Camera.main.ScreenPointToRay(this.inputAimPos);
			if (Physics.Raycast(ray, out var hitInfo, (float)1e3, this._aimLayerMask) == true)
			{
				float botHeight = 1.8f;
				Vector3 targerAimPos = new Vector3()
				{
					x = hitInfo.point.x,
					z = hitInfo.point.z,
					y = botHeight * 0.75f,
				};
				this._aimTr.position = targerAimPos;
			}
			#endregion
		}
		#endregion
	}

}