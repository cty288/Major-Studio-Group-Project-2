using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class NotebookPage : DraggableItems {
    
    private GameObject pageCanvas;
    private TMP_Text pageContentText;
    [ES3Serializable]
    private string pageContentString;
    private List<SpriteRenderer> renderers = new List<SpriteRenderer>();
    [ES3Serializable]
    private string first12Chars;



    protected override void Awake() {
        base.Awake();
        renderers = GetComponentsInChildren<SpriteRenderer>(true).ToList();
        pageCanvas = transform.Find("PageCanvas").gameObject;
        pageContentText = pageCanvas.transform.Find("Text").GetComponent<TMP_Text>();
        pageCanvas.SetActive(false);
        
    }

    protected override void Start() {
        base.Start();
        
        this.Delay(0.1f, () => {
            if(!String.IsNullOrEmpty(first12Chars)) {
                pageContentText.text = first12Chars;
            }
        });
        
    }

    

    public void SetContent(string content, DateTime time) {
        pageContentString = content;
        //get the first line of the content
        string firstLine = content.Split('\n')[0];
        //get the first 12 characters of the first line
        first12Chars = firstLine.Substring(0, Math.Min(firstLine.Length, 12));
        if (firstLine.Length > 12) {
            first12Chars += "...";
        }

        string prefix = time.ToString("MM/dd");
        if (String.IsNullOrEmpty(content)) {
            first12Chars = "Empty Note";
        }
        else
        {
            first12Chars = prefix + ": " + first12Chars;
        }
        
        pageContentText.text = first12Chars;

    }

    protected override void OnClick() {
        this.SendCommand<OpenNotePanelCommand>(new OpenNotePanelCommand() {Content = pageContentString });
    }

    public override void OnThrownToRubbishBin() {
        Destroy(gameObject);
    }

   
    public override void SetLayer(int layer)
    {
        foreach (var renderer in renderers) {
            renderer.sortingOrder = layer;
        }
        pageCanvas.GetComponent<Canvas>().sortingOrder = 1000;
    }
}


public class OpenNotePanelCommand : AbstractCommand<OpenNotePanelCommand> {
    public string Content;
    public OpenNotePanelCommand() { }
    protected override void OnExecute()
    {
        this.SendEvent<OnNotePanelOpened>(
            new OnNotePanelOpened() { Content = Content});
    }
}

public struct OnNotePanelOpened {
    public string Content;
}
