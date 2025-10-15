using UnityEngine;
using System.Collections;

using SPACE_UTIL;
using UnityEngine.InputSystem;

public class DEBUG_Check : MonoBehaviour
{
	private void Update()
	{
		// this.NewInputSystem();
	}

	void NewInputSystem()
	{
		Keyboard keyboard = Keyboard.current;
		/*
			// instant down	: was pressed this frame
			// instant up	: was released this frame
			// held			: is pressed
		*/

		if (keyboard != null)
		{
			if (keyboard.spaceKey.wasPressedThisFrame) Debug.Log("space instant down");
			if (keyboard.spaceKey.wasReleasedThisFrame) Debug.Log("space instant up");
			if (keyboard.spaceKey.isPressed) Debug.Log("space held down");
			if (keyboard.anyKey.wasPressedThisFrame) Debug.Log("pressed any key");
		}


		Mouse mouse = Mouse.current;
		if (mouse != null)
		{
			if (mouse.leftButton.wasPressedThisFrame) Debug.Log("mouse left instant down");
			if (mouse.leftButton.wasReleasedThisFrame) Debug.Log("mouse left instant up");
			if (mouse.leftButton.isPressed) Debug.Log("mouse left held down");

			if (mouse.scroll.ReadValue().y > 0) Debug.Log("scrolling up");
			// Debug.Log(mouse.position.ReadValue());
			// Debug.Log(INPUT.UI.convert(mouse.position.ReadValue()));
		}
	}
}
