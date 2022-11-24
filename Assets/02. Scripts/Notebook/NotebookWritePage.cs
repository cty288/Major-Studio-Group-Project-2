using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NotebookDragger : MonoBehaviour, IDragHandler, IEndDragHandler {
    private Vector2 initialPosition;
    public Transform Parent;
    public Action OnTear;
    private void Awake() {
        initialPosition = transform.parent.parent.parent.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Parent.transform.position = eventData.position;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(Vector2.Distance(transform.position, initialPosition)) < 300f) {
            Parent.transform.DOMove(initialPosition, 0.3f);
        }
        else {
            Parent.transform.position = initialPosition;
            OnTear?.Invoke();
        }
    }
}
public class NotebookWritePage : MonoBehaviour
{
    public TMP_InputField inputField;
    public string noteString;

    [SerializeField] private Table table;
    [SerializeField] private GameObject noteOnTablePagePrefab;

    [SerializeField] private GameObject followingObject;
    private void Awake()
    {
        inputField.caretColor = Color.clear;
        inputField.onValueChanged.AddListener(OnInputFieldValueChang);
      
        
    }

    private void Start() {
        var dragger = GetComponentInChildren<TMP_SelectionCaret>(true).AddComponent<NotebookDragger>();
        dragger.Parent = transform;
        dragger.gameObject.AddComponent<BoxCollider2D>();
        MouseHoverOutline outline = dragger.gameObject.AddComponent<MouseHoverOutline>();
        outline.followingObjectIsUI = true;
        outline.followingObject = followingObject;
        dragger.OnTear += OnPageTear;
        
        
        
    }

    private void OnPageTear() {
        NotebookPage page = table.SpawnItem(noteOnTablePagePrefab).GetComponent<NotebookPage>();
        page.SetContent(inputField.text);
        
        noteString = "";
        inputField.text = "";
    }


    public void Update()
    {
        if (inputField.text.Length > 0)
        {
            inputField.MoveToEndOfLine(false, true);
        }
    }
    
    private void OnInputFieldValueChang(string inputInfo) {
        //get line number
        int lineNum = GetLineNumber(inputInfo);
       
        if (inputInfo.Length < noteString.Length || inputField.textComponent.textInfo.lineCount > inputField.lineLimit) {
            inputField.text = noteString;
        }
        else {
            inputField.text = inputInfo;
        }
        

        noteString = inputField.text;
    }

    public static int GetLineNumber(string text)
    {
        int lineNum = 0;
        using (StringReader reader = new StringReader(text)) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                lineNum++;
            }
        }

        return lineNum;
    }

    
   
}
