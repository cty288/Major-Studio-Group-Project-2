using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunResource : IPlayerResource{
    public string PrefabName { get; } = "Gun";
    public int MaxCount { get; } = 1;
    public string DisplayName { get; } = "Gun";
}
