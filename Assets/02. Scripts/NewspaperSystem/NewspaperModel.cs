using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public class Newspaper: ES3ReferencableObject<Newspaper> {
	public DateTime date;
	public string dateString;
	public List<BodyTimeInfo> timeInfos = new List<BodyTimeInfo>();

	public List<List<Vector3>> markerPositions = new List<List<Vector3>>();
	
	public Newspaper(){}

	public Newspaper(bool randomGuid) : base(randomGuid) {
        
	}
}

public class NewspaperModel : AbstractSavableModel {
	[field: ES3Serializable]
	public Dictionary<string, Newspaper> AvailableNewspapers { get; } = new Dictionary<string, Newspaper>();

	public Newspaper CreateNewspaper(DateTime time, List<BodyTimeInfo> newsPaperBodyInfos) {
		
		Newspaper newspaper = new Newspaper(true)
			{date = time, timeInfos = newsPaperBodyInfos};

		AvailableNewspapers.Add(newspaper.guid, newspaper);
		
		this.SendEvent<OnNewspaperGenerated>(new OnNewspaperGenerated() {Newspaper = newspaper});
		return newspaper;
	}
	
	public Newspaper GetNewspaper(string guid) {
		return AvailableNewspapers[guid];
	}
	
	public List<Newspaper> GetNewspapers() {
		return new List<Newspaper>(AvailableNewspapers.Values);
	}
	
	public void DeleteNewspaper(Newspaper newspaper) {
		AvailableNewspapers.Remove(newspaper.guid);
	}
	
	public void DeleteNewspaper(string guid) {
		AvailableNewspapers.Remove(guid);
	}

	public void MarkNewspaper(Newspaper newspaper, List<Vector3> markerPositions) {
		if(AvailableNewspapers.ContainsKey(newspaper.guid)) {
			AvailableNewspapers[newspaper.guid].markerPositions.Add(markerPositions);
		}
	}
	
	public void MarkNewspaper(string guid, List<Vector3> markerPositions) {
		if(AvailableNewspapers.ContainsKey(guid)) {
			AvailableNewspapers[guid].markerPositions.Add(markerPositions);
		}
	}
	
	
}
