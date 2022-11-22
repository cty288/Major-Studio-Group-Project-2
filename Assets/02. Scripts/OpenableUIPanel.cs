using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

public abstract class OpenableUIPanel : AbstractMikroController<MainGame> {
    public abstract void Show();
    public abstract void Hide();

    public abstract void OnDayEnd();

    protected virtual void Awake() {
        this.RegisterEvent<OnNewDay>(OnNewDay).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnNewDay(OnNewDay obj) {
        OnDayEnd();
        //Hide();
    }
}
