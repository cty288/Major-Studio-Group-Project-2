using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.UI;

public class TelephoneNotepadUIPanel : AbstractMikroController<MainGame> {
    private TelephoneNumberRecordModel telephoneNumberRecordModel;
    private Button lastPageButton;
    private Button nextPageButton;
    
    [SerializeField] private GameObject telephoneNumberRecordItemPrefab;
    private Transform telephoneNumberRecordItemContainer;

    [SerializeField] private int maxRecordPerPage = 4;
    private int currentPage = 0;
    private List<TelephoneNumberRecordInfo> telephoneNumberRecordInfos = new List<TelephoneNumberRecordInfo>();
    
    public void Init() {
        telephoneNumberRecordModel = this.GetModel<TelephoneNumberRecordModel>();
        lastPageButton = transform.Find("LastPageButton").GetComponent<Button>();
        nextPageButton = transform.Find("NextPageButton").GetComponent<Button>();
        telephoneNumberRecordItemContainer = transform.Find("ContentArea");
        lastPageButton.onClick.AddListener(OnLastPageButtonClicked);
        nextPageButton.onClick.AddListener(OnNextPageButtonClicked);
    }

    private void OnNextPageButtonClicked() {
        currentPage++;
        ShowPage(currentPage);
    }

    private void OnLastPageButtonClicked() {
        currentPage--;
        ShowPage(currentPage);
    }

    public void OnPanelOpened() {
        currentPage = 0;
        telephoneNumberRecordInfos = telephoneNumberRecordModel.GetRecordList();
        ShowPage(0);
    }

    private void ShowPage(int pageNumber) {
        DestroyCurrentPage();
        lastPageButton.gameObject.SetActive(pageNumber > 0);
        int totalPage = (telephoneNumberRecordInfos.Count - 1) / maxRecordPerPage + 1;
        nextPageButton.gameObject.SetActive(pageNumber < totalPage - 1);
        if(pageNumber < 0 || pageNumber > totalPage) {
            return;
        }
        
       for (int i = pageNumber * maxRecordPerPage; i < (pageNumber + 1) * maxRecordPerPage; i++) {
            if (i >= telephoneNumberRecordInfos.Count) {
                break;
            }
            GameObject item = Instantiate(telephoneNumberRecordItemPrefab, telephoneNumberRecordItemContainer);
            item.GetComponent<TelephoneNumberRecordItem>().OnInit(telephoneNumberRecordInfos[i].RecordedName,
                telephoneNumberRecordInfos[i].PhoneNumber);
       }
        
    }
    
    private void DestroyCurrentPage() {
        for (int i = 0; i < telephoneNumberRecordItemContainer.childCount; i++) {
            Destroy(telephoneNumberRecordItemContainer.GetChild(i).gameObject);
        }
    }
    
    public void OnPanelClosed() {
        DestroyCurrentPage();
    }
}
