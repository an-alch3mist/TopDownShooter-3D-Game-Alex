using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SPACE_UTIL;

namespace SPACE_TopDownShooter
{
	// attached to different Weapon
	public class PlayerWeaponController : MonoBehaviour
	{
		[SerializeField] PlayerInput _playerInput;
		[SerializeField] float _bulletSpeed = 100f; // 100m/s
		[SerializeField] Transform _gunTr;
		[SerializeField] GameObject _bulletPrefab;

		#region UnityLifeCycle
		private void Start()
		{
			Debug.Log("Start(): " + this);
			this.InitIAEvents();
		}

		void InitIAEvents()
		{
			var IA = this._playerInput.IA;
			IA.Character.Fire.performed += (ctx) => Shoot();
		}
		#endregion

		private void Shoot()
		{
			if (this._gunTr.gameObject.activeSelf == true)
			{
				// init bullet prefab >>
				GameObject objBullet = GameObject.Instantiate(this._bulletPrefab, C.PrefabHolder);
				Transform bulletPointTr = this._gunTr.gameObject.GTr_Leaf<ID_BulletPoint>();
				objBullet.transform.position = bulletPointTr.position;
				objBullet.transform.eulerAngles = bulletPointTr.eulerAngles;
				// << init bullet

				objBullet.GC<Rigidbody>().velocity = bulletPointTr.forward * this._bulletSpeed;
				GameObject.Destroy(objBullet, 10); // destroy after 10sec
			}
		}

	}
}