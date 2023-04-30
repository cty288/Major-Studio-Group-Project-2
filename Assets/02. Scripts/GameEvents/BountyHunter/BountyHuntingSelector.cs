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
        playerControlModel = this.GetModel<PlayerControlModel>();
        //playerControlModel.ControlType.RegisterWithInitValue(OnBountyHuntingChanged).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnAddControlType>(OnAddControlType).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnRemoveControlType>(OnRemoveControlType).UnRegisterWhenGameObjectDestroyed(gameObject);
        bodyInfo = GetComponent<IHaveBodyInfo>();
        if (bodyInfo == null) {
            throw new Exception("BountyHuntingSelector gameobject must have a IHaveBodyInfo component");
        }

        //playerControlModel = this.GetModel<PlayerControlModel>();
    }

    private void OnRemoveControlType(OnRemoveControlType controlType) {
        if (controlType.controlType == PlayerControlType.BountyHunting) {
            hintText.text = hintTextOriginal;
        }
    }

    private void OnAddControlType(OnAddControlType controlType) {
        if (controlType.controlType == PlayerControlType.BountyHunting) {
            hintText.text = "Report this Person";
        }
    }
    
    
    public void SetHintText(string text) {
        Awake();
        if (!playerControlModel.HasControlType(PlayerControlType.BountyHunting)) {
            hintText.text = text;
        }
        
        hintTextOriginal = text;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (!playerControlModel.HasControlType(PlayerControlType.BountyHunting)) {
            return;
        }
        this.SendEvent<OnBodyHuntingSelect>(new OnBodyHuntingSelect() {
            bodyInfos = bodyInfo.BodyInfos
        });
    }
}
