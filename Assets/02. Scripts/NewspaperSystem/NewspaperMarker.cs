using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewspaperMarker : MonoBehaviour {
    [SerializeField] protected GameObject markerPrefab;
    
  

    [ES3Serializable]
    protected List<List<Vector3>> markerPositions = new List<List<Vector3>>();


    protected LineRenderer currentLine;
    private void Awake() {
        SpawnInitialMarkers();
    }

    protected void SpawnInitialMarkers() {
        foreach (List<Vector3> position in markerPositions) {
            GameObject marker = Instantiate(markerPrefab, transform);
            marker.GetComponent<LineRenderer>().SetPositions(position.ToArray());
        }
    }
    
    public LineRenderer AddMarker() {
        LineRenderer marker = Instantiate(markerPrefab, transform).GetComponent<LineRenderer>();
        markerPositions.Add(new List<Vector3>());
        currentLine = marker;
        return marker;
    }
    
    public void AddMarkerPosition(LineRenderer marker, Vector3 position) {
        markerPositions[^1].Add(position);
        
        var positionCount = marker.positionCount;
        positionCount++;
        marker.positionCount = positionCount;
        marker.SetPosition(positionCount - 1, position);
    }
}
