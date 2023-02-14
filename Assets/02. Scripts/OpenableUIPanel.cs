using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;



public abstract class OpenableUIPanel : AbstractMikroController<MainGame> {
    protected List<Collider2D> colliders = new List<Collider2D>();
    protected Canvas canvas;
    protected bool isShow = false;
    
    protected SimpleRC canBack = new SimpleRC();
    
    [SerializeField] private GameObject useCameraMask;
    
    public void Show(float time) {
        OnShow(time);
        this.Delay(time, () => {
            isShow = true;
        });
    }

    public abstract void OnShow(float time);

    public void Hide(float time) {
        OnHide(time);
        isShow = false;
    }

    public abstract void OnHide(float time);
    public abstract void OnDayEnd();

    protected virtual void Awake() {
        this.RegisterEvent<OnNewDay>(OnNewDay).UnRegisterWhenGameObjectDestroyed(gameObject);
        colliders = GetComponentsInChildren<Collider2D>(true).ToList();
        canvas = GetComponentInParent<Canvas>();
        ScreenSpaceImageCropper.Singleton.OnStartCrop+=OnStartCrop;
        ScreenSpaceImageCropper.Singleton.OnEndCrop+=OnEndCrop;
    }

    private void OnDestroy() {
        ScreenSpaceImageCropper.Singleton.OnStartCrop-=OnStartCrop;
        ScreenSpaceImageCropper.Singleton.OnEndCrop-=OnEndCrop;
    }

    private void OnEndCrop() {
        canBack.Release();
        if (useCameraMask) {
            useCameraMask.SetActive(false);
        }
    }

    private void OnStartCrop() {
        canBack.Retain();
        if (useCameraMask) {
            useCameraMask.SetActive(true);
        }
    }

    private void OnNewDay(OnNewDay obj) {
        OnDayEnd();
    }

    protected virtual void Update() {
        if (Input.GetMouseButtonDown(0) && canBack.RefCount<=0) {
            Vector2 mousePos = Input.mousePosition;
            
            if(canvas.renderMode == RenderMode.ScreenSpaceCamera) {
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            }
           
                
            bool mouseClickPanel = colliders.Any(c => c.OverlapPoint(mousePos));

            if (!mouseClickPanel && isShow) {
                Hide(0.5f);
            }
        }
    }
    

}
