using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SPACE_UTIL;
using UnityEngine.InputSystem;

namespace SPACE_TopDownShooter
{
	public class PlayerMovement : MonoBehaviour
	{
		private PlayerInputActions _IA;
		private CharacterController _characterC;
		[SerializeField] float _floorWalkMovementSpeed = 2f;
		[SerializeField] float _floorRunMovementSpeed = 5.5f;
		[SerializeField] LayerMask _aimLayer;

		[SerializeField] Animator _animator;
		[SerializeField] Transform aimTr;

		private void Awake()
		{
			Debug.Log("Awake(): " + this);
			_IA = new PlayerInputActions();
			this.InitIA();

			this._characterC = this.gameObject.GC<CharacterController>();
		}

		[SerializeField] Vector2 inputMovementDir;
		Vector3 movementVel;
		[SerializeField] Vector2 inputAimDir;
		#region InitIA
		void InitIA()
		{
			_IA.Character.Fire.performed += (ctx) => { this.HandleShoot(); };

			_IA.Character.Movement.performed += ctx => this.inputMovementDir = ctx.ReadValue<Vector2>();
			_IA.Character.Movement.canceled += ctx => this.inputMovementDir = Vector2.zero;

			_IA.Character.Aim.performed += ctx => this.inputAimDir = ctx.ReadValue<Vector2>();
			_IA.Character.Aim.canceled += ctx => this.inputMovementDir = Vector2.zero; 
		}
		#endregion

		private void Update()
		{
			#region characterC.Move(movementVel)
			this.HandleFloorMovement();
			this.HandleAirMovement();
			//
			if (this.movementVel.magnitude.zero() == false)
				this._characterC.Move(movementVel);

			#endregion

			#region Rotation
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out var hitInfo, (float)1e3, this._aimLayer) == true)
			{
				Vector3 targetDir = hitInfo.point - this.transform.position; targetDir.y = 0f;
				this.transform.rotation = Quaternion.LookRotation(targetDir);

				// just to log >>
				aimTr.position = hitInfo.point;
				// << just to log
			} 
			#endregion

			//
			this.HandleAnimationController();
		}

		#region HandleFloorMovement, HandleAirMovement
		void HandleFloorMovement()
		{
			this.movementVel = 
				(transform.right   * this.inputMovementDir.x + 
				 transform.forward * this.inputMovementDir.y) * this._floorWalkMovementSpeed * Time.deltaTime;
		}
		void HandleAirMovement()
		{
			if(this._characterC.isGrounded == false)
				this.movementVel.y += -5f * Time.deltaTime;
			else
				this.movementVel.y = -0.01f; // movementVel.xz glitch if its zero
		}
		#endregion

		void HandleAnimationController()
		{
			float xVel = Z.dot(this.movementVel.normalized, this.transform.right);
			float zVel = Z.dot(this.movementVel.normalized, this.transform.forward);

			this._animator.SetFloat("xVel", xVel);
			this._animator.SetFloat("zVel", zVel);
		}

		#region Shoot
		void HandleShoot()
		{
			Debug.Log("Shoot()");
		}
		#endregion

		private void OnEnable()
		{
			_IA.Enable();
		}
		private void OnDisable()
		{
			_IA.Disable();
		}
	}
}