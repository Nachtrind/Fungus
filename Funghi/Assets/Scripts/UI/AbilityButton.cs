using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ButtonName;

public class AbilityButton : MonoBehaviour
{
	Image image;
	public ButtonName.ButtonName buttonName;
	public bool isSelected;
	public bool isUnlocked;


	// Use this for initialization
	void Start ()
	{
		image = this.GetComponent<Image> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void SetTint (Color _tint)
	{
		image.color = _tint;
	}

}

namespace ButtonName
{
	public enum ButtonName
	{
		BeatNEat,
		ScentNode,
		SlowDown,
		Zombies,
		NewNode,
		GrowthSpores,
		SpeedUp
	}
}