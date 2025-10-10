using UnityEngine;
using System.Collections;

namespace SPACE_TopDownShooter
{
	public class PlayerInput : MonoBehaviour
	{
		public PlayerInputActions IA;

		private void Awake()
		{
			Debug.Log("Awake(): " + this);
			this.IA = new PlayerInputActions();
		}

		private void OnEnable()
		{
			IA.Enable();
		}
		private void OnDisable()
		{
			IA.Disable();
		}
	}

}