using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public struct OnBodyHuntingSelect {
    public BodyInfo bodyInfo;
}


[RequireComponent(typeof(MouseHoverOutline))]
public class BountyHuntingSelector : AbstractMikroController<MainGame>, IPointerClickHandler, ICanSendEvent {
    [SerializeField] private TMP_Text hintText;
    private string hintTextOriginal = "";

    private IHaveBodyInfo bodyInfo;

    private BountyHunterSystem bountyHunterSystem;
    private void Awake() {
        hintTextOriginal = hintText.text;
        bountyHunterSystem = this.GetSystem<BountyHunterSystem>();
        bountyHunterSystem.IsBountyHunting.RegisterWithInitValue(OnBountyHuntingChanged).UnRegisterWhenGameObjectDestroyed(gameObject);
        bodyInfo = GetComponent<IHaveBodyInfo>();
        if (bodyInfo == null) {
            throw new Exception("BountyHuntingSelector gameobject must have a IHaveBodyInfo component");
        }
    }

    private void OnBountyHuntingChanged(bool isHunting) {
        if (isHunting) {
            hintText.text = "Report to the Bounty Hunter";
        }
        else {
            hintText.text = hintTextOriginal;
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        this.SendEvent<OnBodyHuntingSelect>(new OnBodyHuntingSelect() {
            bodyInfo = bodyInfo.BodyInfo
        });
    }
}
