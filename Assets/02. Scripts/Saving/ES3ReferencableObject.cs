using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ES3Serializable]
public class ES3ReferencableObject<T> where T: ES3ReferencableObject<T> {
    public string guid;
    
    public ES3ReferencableObject(bool randomGuid) {
        if (randomGuid) {
            guid = Guid.NewGuid().ToString();
        }
    }
    
    public ES3ReferencableObject() {
       
    }
}