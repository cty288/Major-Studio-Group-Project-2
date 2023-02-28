using System.Collections;
using System.Collections.Generic;
using _02._Scripts.Notebook;
using UnityEngine;

public interface ICanHaveDroppableItems {
    public void OnEnter(IDroppable content);
    public void OnExit(IDroppable content);
    public void OnDrop(IDroppable content);
}

