using System.Collections;
using System.Collections.Generic;
using MikroFramework.BindableProperty;
using Polyglot;
using UnityEngine;

public class DescriptionData {
    public string DisplayName;
    public string Description;
    
    
}

public class OutdoorActivityModel : AbstractSavableModel {
    [field: ES3Serializable]
    public BindableProperty<bool> HasMap { get; private set; } = new BindableProperty<bool>(false);
    
    [field: ES3Serializable]
    public BindableProperty<bool> IsOutdoor { get; private set; } = new BindableProperty<bool>(false);

    
    protected Dictionary<string, DescriptionData> placeDescription = new Dictionary<string, DescriptionData>();
    protected Dictionary<string, DescriptionData> activityDescription = new Dictionary<string, DescriptionData>();

    protected override void OnInit() {
        base.OnInit();
        LoadInfo();
        
    }

    private void LoadInfo() {
        DownloadSheet("735041737", placeDescription);
        DownloadSheet("1072689198", activityDescription);
    }
    
    
    public void DownloadSheet(string sheetID, Dictionary<string, DescriptionData> descriptionData) {
        string result = "";
        var enumerator = GoogleDownload.DownloadSheet("1lmnHrIwzQdimzbfLRgKDrLi43lfnCQKSiukUL9kffQo",
            sheetID, s => {
                result = s;
            }, GoogleDriveDownloadFormat.CSV);
        while (enumerator.MoveNext()) {
				
        }
        //Debug.Log("result: " + result);
        OnDownloadSheet(result, descriptionData);
    }


    public void OnDownloadSheet(string text, Dictionary<string, DescriptionData> descriptionData) {
        if (!string.IsNullOrEmpty(text)) {
            List<List<string>> rows;
            text = text.Replace("\r\n", "\n");
            rows = CsvReader.Parse(text);


            int startRow = 1;
            string currentTag = rows[startRow][0].ToLower();
            DescriptionData currentTagDescription = new DescriptionData();
            descriptionData.Add(currentTag, currentTagDescription);

            for (int i = startRow; i < rows.Count; i++) {
                List<string> row = rows[i];
                if (row.Count > 0) {
                    if (!string.IsNullOrEmpty(row[0].ToLower()) && row[0].ToLower() != currentTag) {
                        currentTag = row[0].ToLower();
                        currentTagDescription = new DescriptionData();
                        descriptionData.Add(currentTag, currentTagDescription);
                    }

                    if (!string.IsNullOrEmpty(row[1])) {
                        currentTagDescription.DisplayName = row[1];
                    }

                    if (!string.IsNullOrEmpty(row[2])) {
                        currentTagDescription.Description = row[2];
                    }
                }
            }

        }
    }
    
    public DescriptionData GetPlaceDescription(string placeName) {
        if (placeDescription.ContainsKey(placeName.ToLower())) {
            return placeDescription[placeName.ToLower()];
        }
        else {
            return new DescriptionData();
        }
    }
    
    public DescriptionData GetActivityDescription(string activityName) {
        if (activityDescription.ContainsKey(activityName.ToLower())) {
            return activityDescription[activityName.ToLower()];
        }
        else {
            return new DescriptionData();
        }
    }

    public void GetMap() {
        HasMap.Value = true;
    }
}
