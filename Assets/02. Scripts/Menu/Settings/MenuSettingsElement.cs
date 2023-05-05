using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public abstract class MenuSettingsElement : AbstractMikroController<MainGame> {
    public abstract void OnRefresh();
}
