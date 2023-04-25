using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using NHibernate.Mapping;
using UnityEngine;
using UnityEngine.EventSystems;

public class CultLetterUnderDoorViewController : AbstractMikroController<MainGame>, IPointerClickHandler, ICanSendEvent {
    [ES3Serializable] protected List<string> letterContents = new List<string>();
    
    public void SetContents(List<string> contents) {
        letterContents = contents;
    }

    public void OnPointerClick(PointerEventData eventData) {
        this.SendEvent<OnSpawnCultLetterOnTable>(new OnSpawnCultLetterOnTable() {Contents = letterContents});
        Destroy(gameObject);
    }
}

public struct OnSpawnCultLetterOnTable {
    public List<string> Contents;
}
