using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSpaceCameraMarker : MonoBehaviour {
    [SerializeField] protected GameObject markerPrefab;
    
    
    protected List<List<Vector3>> markerPositions = new List<List<Vector3>>();


    [SerializeField] private bool isLocalSpace = false;
    
    public LineRenderer AddMarker() {
        LineRenderer marker = Instantiate(markerPrefab, transform).GetComponent<LineRenderer>();
        marker.useWorldSpace = !isLocalSpace;
        markerPositions.Add(new List<Vector3>());
        
        return marker;
    }
    
    public List<Vector3> GetCurrentMarkerPositions() {
        if (markerPositions.Count == 0) {
            return null;
        }
        return markerPositions[markerPositions.Count - 1];
    }

    public void AddMarkerPosition(LineRenderer marker, Vector3 position) {
        if (isLocalSpace) {
            position = transform.InverseTransformPoint(position);
        }
        markerPositions[^1].Add(position);
        
        var positionCount = marker.positionCount;
        positionCount++;
        marker.positionCount = positionCount;
        marker.SetPosition(positionCount - 1, position);
    }
}
