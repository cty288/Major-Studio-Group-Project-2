using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.ResKit;
using UnityEngine;

public class MainGame : Architecture<MainGame> {
    protected override void Init() {
        this.RegisterExtensibleUtility<ResLoader>(ResLoader.Allocate());
        this.RegisterSystem<BodyManagmentSystem>(new BodyManagmentSystem());
        this.RegisterSystem<NewspaperSystem>(new NewspaperSystem());
    }
}
