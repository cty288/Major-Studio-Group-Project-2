using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface  IFoodResouce : IPlayerResource{
    
}
public class CanFood : IFoodResouce
{
    public string PrefabName => "CanFood";
    public int MaxCount => 10;
}
public class DryFood : IFoodResouce
{
    public string PrefabName => "DryFood";
    public int MaxCount => 10;
}
public class CrackerPackages: IFoodResouce
{
    public string PrefabName => "CrackerPackages";
    public int MaxCount => 10;
}

