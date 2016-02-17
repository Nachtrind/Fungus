using UnityEngine;
using System.Collections.Generic;

public enum NewsTickerCategory
{
	Diner = 1,
	SlimeSighting = 2,
	Special1 = 3,
	Special2 = 4,
	Special3 = 5,
	Special4 = 6,
	Special5 = 7,
	Special6 = 8,
	Party = 9,
	SpeedUp = 10,
	Growth = 11,
	Enslave = 12,
	SlowDown = 13,
	President = 14
}

[CreateAssetMenu (menuName = "NewsTickerDataCollection")]
public class NewsTickerDataCollection : ScriptableObject
{

	readonly TickerEntry noNews = new TickerEntry ("No News");

	[System.Serializable]
	public class TickerEntry
	{
		public string text;

		public  TickerEntry (string text)
		{
			this.text = text;
		}
	}

	[System.Serializable]
	public class TickerData
	{
		#pragma warning disable 0414
		[SerializeField, HideInInspector] string categoryName;
		#pragma warning restore 0414
		public List<TickerEntry> entries = new List<TickerEntry> ();

		public TickerData (NewsTickerCategory category)
		{
			categoryName = category.ToString ();
		}
	}

	[SerializeField]
	List<TickerData> data = new List<TickerData> {
		new TickerData (NewsTickerCategory.Diner),
		new TickerData (NewsTickerCategory.SlimeSighting),
		new TickerData (NewsTickerCategory.Special1),
		new TickerData (NewsTickerCategory.Special2),
		new TickerData (NewsTickerCategory.Special3),
		new TickerData (NewsTickerCategory.Special4),
		new TickerData (NewsTickerCategory.Special5),
		new TickerData (NewsTickerCategory.Special6),
		new TickerData (NewsTickerCategory.Party),
		new TickerData (NewsTickerCategory.SpeedUp),
		new TickerData (NewsTickerCategory.Growth),
		new TickerData (NewsTickerCategory.Enslave),
		new TickerData (NewsTickerCategory.SlowDown),
		new TickerData (NewsTickerCategory.President)

	};

	public TickerEntry GetTickerEntry (NewsTickerCategory category)
	{
		if (data.Count == 0) {
			return noNews;
		}
		TickerData td = data [(int)category];
		if (td.entries.Count == 0) {
			return noNews;
		}
		return td.entries [Random.Range (0, td.entries.Count - 1)];
	}
}
