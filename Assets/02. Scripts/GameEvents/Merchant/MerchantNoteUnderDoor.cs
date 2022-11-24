using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.EventSystems;

public class MerchantNoteUnderDoor : AbstractMikroController<MainGame>, IPointerClickHandler {
    [SerializeField] private GameObject spawnedNoteOnTablePrefab;
    private void Awake() {
        this.RegisterEvent<MerchantPhoneNumberEvent>(OnMerchantPhoneNumberEvent)
            .UnRegisterWhenGameObjectDestroyed(gameObject);

        gameObject.SetActive(false);
    }

    private void OnMerchantPhoneNumberEvent(MerchantPhoneNumberEvent obj) {
        gameObject.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData) {
        this.SendCommand<SpawnTableItemCommand>(new SpawnTableItemCommand() {Prefab = spawnedNoteOnTablePrefab});
        Destroy(gameObject);
    }
}


public class SpawnTableItemCommand : AbstractCommand<SpawnTableItemCommand> {
    public GameObject Prefab;

    public SpawnTableItemCommand(){}
    protected override void OnExecute() {
        this.SendEvent<SpawnTableItemEvent>(new SpawnTableItemEvent(){Prefab = Prefab});
    }
}

public struct SpawnTableItemEvent{
    public GameObject Prefab;
}
