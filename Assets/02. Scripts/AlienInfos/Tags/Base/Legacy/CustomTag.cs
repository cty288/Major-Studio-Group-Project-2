using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICustomTag : IAlienTag{
 
}

public abstract class CustomTag: AbstractAlienTag, ICustomTag{

}
