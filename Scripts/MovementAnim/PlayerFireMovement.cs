using UnityEngine;
using System.Collections;

namespace SPACE_TopDownShooter
{
	public class PlayerFireMovement : MonoBehaviour
	{
		[SerializeField] PlayerInput _playerInput;
		[SerializeField] Animator _animator;
		[SerializeField] PlayerWeaponVisualsController _playerWeaponVisualsController;

		private void Start()
		{
			Debug.Log("Start(): " + this);
			this.InitIAEvents();
		}

		#region InitIAEvents
		void InitIAEvents()
		{
			var _IA = this._playerInput.IA;
			_IA.Character.Fire.performed += (ctx) => { this.HandleShoot(); };
		}
		#endregion

		#region Shoot
		void HandleShoot()
		{
			if (this._playerWeaponVisualsController.isGrabbing_Animator == true || 
				this._playerWeaponVisualsController.isReloading_Animator == true)
				return;

			this.HandleAnimationControllerShoot();
		}

		void HandleAnimationControllerShoot()
		{
			this._animator.SetTrigger(PlayerAnimParamType.fire.ToString());
		}
		#endregion
	}
}