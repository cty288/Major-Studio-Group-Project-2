using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoorViewController : AbstractMikroController<MainGame>, IPointerClickHandler {
    [SerializeField] private Camera spyEyeCamera;
    [SerializeField] private Camera mainCamera;
    public void OnPointerClick(PointerEventData eventData) {
        LoadCanvas.Singleton.Load(0.2f, () => {
            BackButton.Singleton.Show();
            spyEyeCamera.gameObject.SetActive(true);
            mainCamera.gameObject.SetActive(false);
        }, null);
    }
}
