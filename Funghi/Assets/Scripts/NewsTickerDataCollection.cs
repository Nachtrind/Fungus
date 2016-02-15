using UnityEngine;
using System.Collections.Generic;

public enum NewsTickerCategory
{
	Gossip = 0,
	HearSay = 1,
	Talk = 2,
	Anouncement = 3,
	News = 4,
	Party = 5,
	EventCoverage = 6,
	Forecast = 7,
	Warning = 8,
	GovernmentNotice = 9
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
		new TickerData (NewsTickerCategory.Gossip),
		new TickerData (NewsTickerCategory.HearSay),
		new TickerData (NewsTickerCategory.Talk),
		new TickerData (NewsTickerCategory.Anouncement),
		new TickerData (NewsTickerCategory.News),
		new TickerData (NewsTickerCategory.Party),
		new TickerData (NewsTickerCategory.EventCoverage),
		new TickerData (NewsTickerCategory.Forecast),
		new TickerData (NewsTickerCategory.Warning),
		new TickerData (NewsTickerCategory.GovernmentNotice)

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
