using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SPACE_UTIL;

using SPACE_TopDownShooter.ID;

namespace SPACE_TopDownShooter
{
	public class WeaponVisualController : MonoBehaviour
	{
		[SerializeField] PlayerInput _playerInput;
		[SerializeField] Animator _animator;
		[SerializeField] Transform _pistol, _revolver, _autoRifle, _shotGun, _sniper;
		[SerializeField] Transform _LeftHandIK_Target;

		private void Start()
		{
			Debug.Log("Start(): " + this);
			this.InitWEAPON();
			this.InitIAEvents();
		}

		void InitIAEvents()
		{
			var IA = this._playerInput.IA;
			IA.Character.switchWeapon.performed += (ctx) => { this.SwitchWeapon(); };
		}

		Transform[] WEAPON; 
		void InitWEAPON()
		{
			WEAPON = new Transform[]
			{
				this._pistol,
				this._revolver,
				this._autoRifle,
				this._shotGun,
				this._sniper,
			};
			this.currIndex = 0;
			this.SwitchWeapon();
		}

		int currIndex;
		void SwitchWeapon()
		{
			for (int i0 = 0; i0 < WEAPON.Length; i0 += 1)
			{
				if (i0 == currIndex) this.WEAPON[i0].gameObject.SetActive(true);
				else this.WEAPON[i0].gameObject.SetActive(false);
			}

			Transform LeftIKTarget_fromID = this.WEAPON[currIndex].gameObject.GC_InLeaf<ID_LeftHandIKTarget>().transform;
			this._LeftHandIK_Target.position = LeftIKTarget_fromID.position;
			this._LeftHandIK_Target.eulerAngles = LeftIKTarget_fromID.eulerAngles;


			// anim layer >>
			for (int i0 = 1; i0 < _animator.layerCount; i0 += 1)
				_animator.SetLayerWeight(i0, 0f);

			if (currIndex == 3) _animator.SetLayerWeight(layerIndex: 2, 1f); // shotgun layer
			else				_animator.SetLayerWeight(layerIndex: 1, 1f); // rifle pr common layer
			// << anim layer

			currIndex = (currIndex + 1) % this.WEAPON.Length;
		}
	}

}