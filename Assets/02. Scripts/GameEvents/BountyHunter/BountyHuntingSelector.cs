using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public struct OnBodyHuntingSelect {
    public List<BodyInfo> bodyInfos;
}


[RequireComponent(typeof(MouseHoverOutline))]
public class BountyHuntingSelector : AbstractMikroController<MainGame>, IPointerClickHandler, ICanSendEvent {
    [SerializeField] private TMP_Text hintText;
    public string hintTextOriginal = "";

    private IHaveBodyInfo bodyInfo;

    private BountyHunterSystem bountyHunterSystem;

    private PlayerControlModel playerControlModel;
    private void Awake() {
        hintTextOriginal = hintText.text;
        bountyHunterSystem = this.GetSystem<BountyHunterSystem>();
        bountyHunterSystem.IsBountyHunting.RegisterWithInitValue(OnBountyHuntingChanged).UnRegisterWhenGameObjectDestroyed(gameObject);
        bodyInfo = GetComponent<IHaveBodyInfo>();
        if (bodyInfo == null) {
            throw new Exception("BountyHuntingSelector gameobject must have a IHaveBodyInfo component");
        }

        playerControlModel = this.GetModel<PlayerControlModel>();
    }

    private void OnBountyHuntingChanged(bool isHunting) {
        if (isHunting) {
            hintText.text = "Report to the Bounty Hunter";
        }
        else {
            hintText.text = hintTextOriginal;
        }
    }
    
    public void SetHintText(string text) {
        hintText.text = text;
        hintTextOriginal = text;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (playerControlModel.ControlType.Value != PlayerControlType.Normal) {
            return;
        }
        this.SendEvent<OnBodyHuntingSelect>(new OnBodyHuntingSelect() {
            bodyInfos = bodyInfo.BodyInfos
        });
    }
}
