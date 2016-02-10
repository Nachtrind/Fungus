using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class UserMenuButton : MonoBehaviour
{
	[SerializeField] UserMenu menu;

	public UserMenu.UserMenuButtonType type;

	bool _active;

	public void OnActive (BaseEventData data)
	{
		if (!menu)
			return;
		if (_active)
			return;
		_active = true;
		menu.OnMenuButtonActive (type);
	}

	public void OnInactive (BaseEventData data)
	{
		if (!menu)
			return;
		if (!_active)
			return;
		_active = false;
		menu.OnMenuButtonInactive (type);
	}
}
