using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SPACE_UTIL;

using UnityEngine.Animations.Rigging;
using SPACE_TopDownShooter.ID;

namespace SPACE_TopDownShooter
{
	public class WeaponVisualController : MonoBehaviour
	{
		[SerializeField] PlayerInput _playerInput;
		[SerializeField] Animator _animator;
		[SerializeField] Transform _pistol, _revolver, _autoRifle, _shotGun, _sniper;
		[SerializeField] Transform _LeftHandIK_Target;
		[SerializeField] public Rig _rig;

		private void Start()
		{
			Debug.Log("Start(): " + this);
			this.InitWEAPON();
			this.InitIAEvents();
		}

		void InitIAEvents()
		{
			var IA = this._playerInput.IA;
			IA.Character.switchWeapon.performed += (ctx) => { this.GrabWeapon(); /*this.SwitchWeapon();*/ };
			IA.Character.reloadWeapon.performed += (ctx) => { this.ReloadWeapon(); };
			// IA.Character.grabWeapon.performed += (ctx) => { this.GrabWeapon(); };
		}


		#region _rig.weight assignment wont work
		/* console
		// wont work 
			1
			1
			Event Call
			0
			0
			0
		*/
		/*
		bool shouldIncrRigWeight = false;
		private void Update()
		{
			Debug.Log(this._rig.weight);
		}
		public void TrueIncrRigWeight()
		{
			this.shouldIncrRigWeight = true;
			this._rig.weight = 0.95f;
		}
		*/



		#endregion

		#region _rig.weight assignment wont work works with late update over time
		/* works 
			1
			1
			1
			Event Call
			0
			0.2
			0.4
			0.6
			0.8
			0.9
		*/
		/*
		bool shouldIncrRigWeight = false;
		private void Update()
		{
			Debug.Log(this._rig.weight);
			//
			
			if(this.shouldIncrRigWeight == true)
			{
				this._rig.weight += 3f * Time.deltaTime;
				if (this._rig.weight > 0.95f)
					this.shouldIncrRigWeight = false;
			}
			
		}
		public void TrueIncrRigWeight()
		{
			this.shouldIncrRigWeight = true;
			this._rig.weight = 0.95f;
		}
		*/
		#endregion

		private void Update()
		{
			Debug.Log(this._rig.weight);

			/*
			// refer: ./Scripts/UnityLifeCycle - RigWeightUpdateCause.md
			if(INPUT.K.InstantDown(KeyCode.I))
			{
				Debug.Log("InstantDown I");
				_rig.weight = 1f;
			}
			*/
		}


		// Event Call >>
		// refer: ./Scripts/UnityLifeCycle - RigWeightUpdateCause.md
		public void IncrRigWeight(float duration = 0.3f)
		{
			// Debug.Log("called TrueIncrRigWeight()");
			StartCoroutine(RestoreRigWeightCoroutine(duration:duration));
		}
		IEnumerator RestoreRigWeightCoroutine(float duration)
		{
			yield return null; // Wait one frame for Animation Rigging to finish

			// float duration = 0.3f; // ad: linear transition over 0.3 seconds
			float elapsed = 0f;

			while (elapsed < duration)
			{
				elapsed += Time.deltaTime;
				_rig.weight = Z.lerp(0f, 1f, elapsed / duration);
				yield return null;
			}

			_rig.weight = 1f;
		}

		// << Event Call

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
			this.currIndex = -1;
			this.SwitchWeapon();
		}

		int currIndex;
		void SetIKWeapon()
		{
			Transform LeftIKTarget_fromID = this.WEAPON[currIndex].gameObject.GC_InLeaf<ID_LeftHandIKTarget>().transform;
			this._LeftHandIK_Target.position = LeftIKTarget_fromID.position;
			this._LeftHandIK_Target.eulerAngles = LeftIKTarget_fromID.eulerAngles;
		}

		public bool isGrabbing_Animator
		{
			get
			{
				return this._animator.GetBool(PlayerAnimParamType.isGrabbing.ToString());
			}
			set
			{
				this._animator.SetBool(PlayerAnimParamType.isGrabbing.ToString(), value);
			}
		}

		// called from animation event
		public void SwitchWeapon()
		{
			currIndex = (currIndex + 1) % this.WEAPON.Length;
			for (int i0 = 0; i0 < WEAPON.Length; i0 += 1)
			{
				if (i0 == currIndex) this.WEAPON[i0].gameObject.SetActive(true);
				else this.WEAPON[i0].gameObject.SetActive(false);
			}

			this.SetIKWeapon();

			// anim layer >>
			for (int i0 = 1; i0 < _animator.layerCount; i0 += 1)
				_animator.SetLayerWeight(i0, 0f);

			if (currIndex == 3) _animator.SetLayerWeight(layerIndex: 2, 1f); // shotgun layer
			else				_animator.SetLayerWeight(layerIndex: 1, 1f); // rifle pr common layer
																			 // << anim layer
		}

		void ReloadWeapon()
		{
			this.PauseAutoRig();
			this._animator.SetTrigger(PlayerAnimParamType.reload.ToString());
		}

		void GrabWeapon()
		{
			if (this.isGrabbing_Animator == true)
				return;

			DisableAllWeapon();
			this.PauseAutoRig();
			this._animator.SetTrigger(PlayerAnimParamType.weaponGrab.ToString());
			// this._animator.SetBool(PlayerAnimParamType.isGrabbing.ToString(), true);
			this.isGrabbing_Animator = true;
		}

		#region util
		private void PauseAutoRig()
		{
			_rig.weight = 0f; // disable aim, IK constraint
		}

		void DisableAllWeapon()
		{
			for (int i0 = 0; i0 < WEAPON.Length; i0 += 1)
				this.WEAPON[i0].gameObject.SetActive(false);
		}
		#endregion
	}
}