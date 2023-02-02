using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.EventSystems;

public class NotebookViewController : AbstractMikroController<MainGame>, IPointerClickHandler{
    [SerializeField] private OpenableUIPanel panel;
    public void OnPointerClick(PointerEventData eventData) {
        panel.Show(0.5f);
    }
}
