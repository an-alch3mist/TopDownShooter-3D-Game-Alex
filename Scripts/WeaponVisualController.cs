using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SPACE_TopDownShooter
{
	public class WeaponVisualController : MonoBehaviour
	{
		[SerializeField] PlayerInput _playerInput;
		[SerializeField] Transform _pistol, _revolver, _autoRifle, _shotGun, _sniper;

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
			currIndex = (currIndex + 1) % this.WEAPON.Length;
		}
	}

}