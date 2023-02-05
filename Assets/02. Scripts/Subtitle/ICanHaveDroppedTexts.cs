using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanHaveDroppedTexts {
    public void OnEnter(string text);
    public void OnExit(string text);
    public void OnDrop(string text);
}
