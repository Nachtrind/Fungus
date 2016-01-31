using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightInHouses : MonoBehaviour
{


	public List<HouseWithLight> houses;
	public GameObject mapWithHouses;

	public float lightTick;
	float lightTimer;


	// Use this for initialization
	void Start ()
	{
		houses = new List<HouseWithLight> ();
		Transform[] potentialHouses = mapWithHouses.GetComponentsInChildren<Transform> ();

		foreach (Transform house in potentialHouses) {
			if (house.name.Contains ("Haus")) {
				HouseWithLight houseWL = new HouseWithLight ();
				Transform[] potentialWindows = house.GetComponentsInChildren<Transform> ();

				foreach (Transform t in potentialWindows) {
					if (t.name.Contains ("Fenster")) {
						houseWL.lights.Add (t);
					}
				}
				houseWL.ToggleLights ();
				houses.Add (houseWL);
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
		if (lightTimer > lightTick) {

			for (int i = 0; i < 7; i++) {
				houses [Random.Range (0, houses.Count - 1)].ToggleLights ();
			}
			lightTimer = 0.0f;
		}

		lightTimer += Time.deltaTime;

	}

}

public class HouseWithLight
{
	public List<Transform> lights;

	public HouseWithLight ()
	{
		lights = new List<Transform> ();
	}

	public void InitializeLights ()
	{
		if (lights.Count > 2) {
			for (int i = 0; i > lights.Count - 2; i++) {
				lights [i].GetComponent<SpriteRenderer> ().enabled = false;
				lights [i].GetChild (0).GetComponent<SpriteRenderer> ().enabled = false;
			}

		}
	}

	public void ToggleLights ()
	{
		int lightsOn = 0;
		foreach (Transform l in lights) {
			if (Random.Range (0, 2) == 1 && lightsOn < 2) {
				lightsOn++;
				l.GetComponent<SpriteRenderer> ().enabled = true;
				l.GetChild (0).GetComponent<SpriteRenderer> ().enabled = true;
			} else {
				l.GetComponent<SpriteRenderer> ().enabled = false;
				l.GetChild (0).GetComponent<SpriteRenderer> ().enabled = false;
			}

		}
	}

}