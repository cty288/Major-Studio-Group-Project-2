using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultBodyInfoContainer : MonoBehaviour, IHaveBodyInfo {
    public BodyInfo BodyInfo { get; set; }
}
