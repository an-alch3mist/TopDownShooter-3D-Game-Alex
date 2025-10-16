using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using SPACE_UTIL;

namespace SPACE_TopDownShooter
{
	public class PlayerAimCalculation : MonoBehaviour
	{
		[SerializeField] PlayerInput _playerInput;
		[SerializeField] LayerMask _aimLayerMask;
		[SerializeField] Transform _aimTr;

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
			// make sure camera has been moved(possibly inside LateUpdate) before this calculation is made
			#region Calculate Aim Pos After Positive That Camera Has Made Its Move
			Ray ray = Camera.main.ScreenPointToRay(this.inputAimPos);
			if (Physics.Raycast(ray, out var hitInfo, (float)1e3, this._aimLayerMask) == true)
			{
				float botHeight = 1.8f;
				Vector3 targerAimPos = new Vector3()
				{
					x = hitInfo.point.x,
					z = hitInfo.point.z,
					y = (this.transform.position.y + botHeight * 0.75f),
				};
				this._aimTr.position = targerAimPos;
			} 
			#endregion
		}
		#endregion
	}

}