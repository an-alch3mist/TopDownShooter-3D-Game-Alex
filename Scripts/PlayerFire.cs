using UnityEngine;
using System.Collections;

namespace SPACE_TopDownShooter
{
	public class PlayerFire : MonoBehaviour
	{
		[SerializeField] PlayerInput _playerInput;
		[SerializeField] Animator _animator;

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
			Debug.Log("Shoot()");
			this.HandleAnimationControllerShoot();
		}
		#endregion

		void HandleAnimationControllerShoot()
		{
			this._animator.SetTrigger("fire");
		}

	}

}