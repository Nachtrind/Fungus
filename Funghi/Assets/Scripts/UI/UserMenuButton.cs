using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UserMenuButton : MonoBehaviour
{
	[SerializeField] UserMenu menu;

	public UserMenu.UserMenuButtonType type;

	bool _active;
	[SerializeField] Sprite normalSprite;
	[SerializeField] Sprite pressedSprite;
	[SerializeField] Image uiImage;


	public void OnActive (BaseEventData data)
	{
		if (!menu)
			return;
		if (_active)
			return;
		_active = true;
		menu.OnMenuButtonActive (type);
		uiImage.sprite = pressedSprite;

	}

	public void OnInactive (BaseEventData data)
	{
		if (!menu)
			return;
		if (!_active)
			return;
		_active = false;
		menu.OnMenuButtonInactive (type);
		uiImage.sprite = normalSprite;
	}
}
