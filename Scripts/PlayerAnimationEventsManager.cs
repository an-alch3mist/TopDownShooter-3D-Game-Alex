using UnityEngine;
using System.Collections;

using UnityEngine.Animations.Rigging;

using SPACE_UTIL;

/* Event System
	#region event subsribe approach
	public static event EventHandler _subscribeChannel_WhenResourceCountAltered;
	#endregion
	public static void AddResource(SO_ResourceType _SO_ResourceType, int count)
	{
		ResourceManager.MAP_ResourceCount[_SO_ResourceType] += count;


		_subscribeChannel_WhenResourceCountAltered? // check there are subsribers, otherwise error
			.Invoke(null, EventArgs.Empty);
	}

	// ====================== SUBSCRIBE ======================== //
	ResourceManager._subscribeChannel_WhenResourceCountAltered += (o, e) => UpdateResourceCount();
*/


namespace SPACE_TopDownShooter
{
	public class PlayerAnimationEventsManager : MonoBehaviour
	{
		[SerializeField] WeaponVisualController _weaponVisualController;

		public void ResetRigWeightForIK()
		{
			Debug.Log("AnimationEvent: ResetRigWeightForIK()".colorTag("cyan"));
			this._weaponVisualController.IncrRigWeight();
		}

		public void GrabAndSwitchWeapon()
		{
			Debug.Log("AnimationEvent: GrabAndSwitchWeapon()".colorTag("cyan"));
			this._weaponVisualController.SwitchWeapon();
			this._weaponVisualController.IncrRigWeight(0.1f);
		}

		public void CanGrabAgain()
		{
			Debug.Log("AnimationEvent: CanGrab()".colorTag("cyan"));
			this._weaponVisualController.isGrabbing_Animator = false;
		}
	}
}