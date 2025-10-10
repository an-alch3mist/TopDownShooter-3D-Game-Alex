using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SPACE_UTIL;
using SPACE_DrawSystem;
using UnityEngine.InputSystem;

namespace SPACE_TopDownShooter
{
	public class PlayerMovement : MonoBehaviour
	{
		private CharacterController _characterC;
		[SerializeField] float _floorWalkMovementSpeed = 2f;
		[SerializeField] float _floorRunMovementSpeed = 5.5f;
		[SerializeField] LayerMask _aimLayer;

		[SerializeField] PlayerInput _playerInput;
		[SerializeField] Animator _animator;
		[SerializeField] Transform aimTr;

		Line dirLine;

		private void Start()
		{
			Debug.Log("Start(): " + this);
			this.InitIAEvents();

			this._characterC = this.gameObject.GC<CharacterController>();

			dirLine = new Line(name: "DirLine");
		}

		private void Update()
		{
			#region characterC.Move(movementVel)
			this.HandleFloorMovement();
			this.HandleAirMovement();
			//
			if (this.movementVel.magnitude.zero() == false)
				this._characterC.Move(movementVel);


			dirLine.a = this.transform.position;
			dirLine.b = this.transform.position + this.movementVel * 10;
			#endregion

			
			#region transform.rotation
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out var hitInfo, (float)1e3, this._aimLayer) == true)
			{
				// just to log + aim rig >>
				Vector3 targerAim = new Vector3()
				{
					x = hitInfo.point.x,
					z = hitInfo.point.z,
					y = (this.transform.position.y + 1f),
				};

				float dist = Vector3.Magnitude(targerAim.xz() - this.transform.position.xz());
				if (dist > 0.5f)
				{
					Vector3 targetDir = hitInfo.point - this.transform.position; targetDir.y = 0f;
					this.transform.rotation = Quaternion.LookRotation(targetDir);
					this.aimTr.position = targerAim;
				}
				// << just to log + aim rig
			}
			#endregion
			

			//
			this.HandleAnimationControllerXZ();
		}

		#region HandleFloorMovement, HandleAirMovement
		void HandleFloorMovement()
		{
			this.movementVel =
				(transform.right * this.inputMovementDir.x +
				 transform.forward * this.inputMovementDir.y) * (this.isRunning ? this._floorRunMovementSpeed : this._floorWalkMovementSpeed) * Time.deltaTime;
		}
		void HandleAirMovement()
		{
			if(this._characterC.isGrounded == false)
				this.movementVel.y += -5f * Time.deltaTime;
			else
				this.movementVel.y = -0.1f * Time.deltaTime; // movementVel.xz glitch, if movementVel.y = 0f zero (no longer the case if AnimationController just depend on the .xz of movementVel)
		}
		#region cause for movementVel.xz glitch
		/*
		The Root Cause
			When you set movementVel.y = 0f, the CharacterController interprets this as no vertical movement at all.Here's what happens:

			- CharacterController.Move() applies the movement vector
			- When y = 0, the controller doesn't push down into the ground
			- Unity's ground detection becomes unreliable - the controller may briefly lose contact with the ground
			- When isGrounded flickers to false, your code switches to the air movement branch
			- Gravity starts applying (movementVel.y += -5f * Time.deltaTime)
			- This causes the vertical velocity to change, which then affects your horizontal movement calculations
			- The next frame, it might detect grounded again, resetting to y = 0
			- This rapid flickering between grounded/not grounded states causes the glitchy behavior

		Why -0.1f * Time.deltaTime Works
			When you use a small downward force (-0.1f), you're:

			Constantly pushing the character slightly into the ground
			Maintaining consistent contact with the ground surface
			Ensuring isGrounded remains reliably true
			Preventing the flickering state that causes velocity resets
			*/
		#endregion
		#endregion

		void HandleAnimationControllerXZ()
		{
			// independent on movementVel.y >>
			Vector3 movementVel_xz = new Vector3(movementVel.x, 0, movementVel.z);

			float xVel = Z.dot(movementVel_xz.normalizedZero(), this.transform.right);
			float zVel = Z.dot(movementVel_xz.normalizedZero(), this.transform.forward);
			// << independent on movementVel.y

			this._animator.SetBool("isRunning", this.isRunning);
			this._animator.SetFloat("xVel", xVel);
			this._animator.SetFloat("zVel", zVel);
		}

		[Header("just to log")]
		[SerializeField] Vector2 inputMovementDir;
		[SerializeField] bool isRunning;
		Vector3 movementVel;
		[SerializeField] Vector2 inputAimDir;
		#region InitIAEvents
		void InitIAEvents()
		{
			var _IA = this._playerInput.IA;

			_IA.Character.Movement.performed += ctx => this.inputMovementDir = ctx.ReadValue<Vector2>();
			_IA.Character.Movement.canceled += ctx => this.inputMovementDir = Vector2.zero;

			_IA.Character.Aim.performed += ctx => this.inputAimDir = ctx.ReadValue<Vector2>();
			_IA.Character.Aim.canceled += ctx => this.inputMovementDir = Vector2.zero;

			_IA.Character.Run.performed += (ctx) => { this.isRunning = true; };
			_IA.Character.Run.canceled += (ctx) => { this.isRunning = false; };
		}
		#endregion
	}
}