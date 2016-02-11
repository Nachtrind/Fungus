using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Anemometer : MonoBehaviour
{

	[SerializeField] UserMenu menu;
	[SerializeField] RectTransform needle;

	public void SetOrientation (float degree)
	{
		needle.rotation = Quaternion.Euler (0, 0, degree);
	}

	public void RequestOrientTo (Vector2 screenPoint)
	{
		Vector2 dirVec = transform.InverseTransformPoint (screenPoint).normalized;
		var degree = Vector2.Angle (Vector2.down, dirVec);
		if (dirVec.x < 0) {
			degree = 360 - degree;
		}
		menu.OnAnemoRequestsWindDirection (degree - 90);
		SetOrientation (degree - 90);
	}

	public void OnPointerDown (BaseEventData eventData)
	{
		var pos = (eventData as PointerEventData).position;
		RequestOrientTo (pos);
		menu.StartChangingWind ();
	}

	public void OnPointerAway (BaseEventData eventData)
	{
		menu.StopChangingWind ();
	}
}
