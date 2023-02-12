using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewspaperMarker : MonoBehaviour {
    [SerializeField] protected GameObject markerPrefab;
    
    
    protected List<List<Vector3>> markerPositions = new List<List<Vector3>>();


   
    
    public LineRenderer AddMarker() {
        LineRenderer marker = Instantiate(markerPrefab, transform).GetComponent<LineRenderer>();
        markerPositions.Add(new List<Vector3>());
        return marker;
    }
    
    public List<Vector3> GetCurrentMarkerPositions() {
        return markerPositions[markerPositions.Count - 1];
    }

    public void AddMarkerPosition(LineRenderer marker, Vector3 position) {
        markerPositions[^1].Add(position);
        
        var positionCount = marker.positionCount;
        positionCount++;
        marker.positionCount = positionCount;
        marker.SetPosition(positionCount - 1, position);
    }
}
