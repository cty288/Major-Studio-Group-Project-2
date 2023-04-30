using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.UI;

public class NotebookTool : AbstractMikroController<MainGame> {
    private Material selectedMaterial;
    private MouseHoverOutline mouseHoverOutline;
    private Image image;
    private Button button;
    
    public Action<NotebookTool> OnToolClicked;
    private PlayerControlModel playerControlModel;
    private void Awake() {
        mouseHoverOutline = GetComponent<MouseHoverOutline>();
        selectedMaterial = Material.Instantiate(mouseHoverOutline.OutlineMaterial);
        
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        playerControlModel = this.GetModel<PlayerControlModel>();
    }

    private void OnClick() {
        if (!playerControlModel.IsNormal()) {
            return;
        }
        OnToolClicked?.Invoke(this);
    }

    public void Select() {
        mouseHoverOutline.enabled = false;
        image.material = selectedMaterial;
    }
    
    public void Deselect() {
        mouseHoverOutline.enabled = true;
        image.material = null;
        mouseHoverOutline.StopHovering();
    }
    
}
