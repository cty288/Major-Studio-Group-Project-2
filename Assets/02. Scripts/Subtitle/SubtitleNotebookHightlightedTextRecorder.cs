using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using _02._Scripts.Notebook;
using MikroFramework.Architecture;
using MikroFramework.ResKit;
using MikroFramework.Utilities;
using TMPro;
using UnityEngine;

public class SubtitleNotebookHightlightedTextRecorder : AbstractMikroController<MainGame> {
	private TMP_Text text;
//	private TMP_Text currentDraggedText;
    //private ICanHaveDroppableItems lastReceiver;
   // RaycastHit2D[] raycastResults = new RaycastHit2D[5];
    private TMP_Text hintTmpText;
    public static SimpleRC IsMouseOverUIRC = new SimpleRC();
    protected bool isMouseOverUI = false;
    protected NotebookModel NotebookModel;
    
   // public static string CurrentDraggedText { get; private set; } = null;
	private void Awake() {
		text = GetComponent<TMP_Text>();
        NotebookModel = this.GetModel<NotebookModel>();
    }

	private void Update() {
        if (!NotebookModel.HasNotebook) {
            return;
        }
        var wordIndex = TMP_TextUtilities.FindIntersectingWord(text, Input.mousePosition, null);
        string targetText = "";
        
        if (wordIndex != -1) {
            List<string> allRichTexts = GetRichTexts();
                
            if (allRichTexts.Count > 0) {
                string clickedWord = text.textInfo.wordInfo[wordIndex].GetWord();
                    
                List<string> clickedWordNeighbours = GetNeighbours(wordIndex);

                //find all rich texts that contain the clicked word
                List<string> richTexts = allRichTexts.FindAll(richText => richText.Contains(clickedWord));
                if (richTexts.Count == 0) {
                    return;
                }
                targetText = richTexts[0];
                    
                    
                if(richTexts.Count > 1) {
                    clickedWordNeighbours.Add(clickedWord);
                    //find the rich text that contains the most of the clicked word's neighbours
                    int max = 0;
                    foreach (string richText in richTexts) {
                        int count = 0;
                        foreach (string neighbour in clickedWordNeighbours) {
                            if (richText.Contains(neighbour)) {
                                count++;
                            }
                        }

                        if (count > max) {
                            max = count;
                            targetText = richText;
                        }
                    }
                }
            }
                
        }
        
        
        if (wordIndex != -1 && !string.IsNullOrEmpty(targetText)) {
            if (!hintTmpText) {
                hintTmpText = CreateDraggedText("Click to add to the notebook \nto quick-record", new Color(0.7f, 0.7f, 0.7f, 1f), null);
                hintTmpText.fontSize = 20;
            }
            hintTmpText.transform.position = Input.mousePosition + Vector3.up * 30;
            if (!isMouseOverUI) {
                isMouseOverUI = true;
                IsMouseOverUIRC.Retain();
            }
        }
        else {
            if(hintTmpText) {
                Destroy(hintTmpText.gameObject);
                hintTmpText = null;
            }
            
            if (isMouseOverUI) {
                isMouseOverUI = false;
                IsMouseOverUIRC.Release();
            }
        }
        
      
        /*
        if(Input.GetMouseButton(0)) {
            if (currentDraggedText != null) {
                currentDraggedText.transform.position = Input.mousePosition;
                
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
             
                var size = Physics2D.RaycastNonAlloc(ray.origin, ray.direction, raycastResults, float.PositiveInfinity);
                
                if (size > 0) {
                    for (int i = 0; i < size; i++) {
                        RaycastHit2D hit = raycastResults[i];
                        var canHaveDroppedText = hit.collider.GetComponent<ICanHaveDroppableItems>();
                        if (canHaveDroppedText != null) {
                            if(lastReceiver==null || lastReceiver != canHaveDroppedText) {
                                canHaveDroppedText.OnEnter(currentDraggedText.GetComponent<IDroppable>());
                                lastReceiver = canHaveDroppedText;
                                break;
                            }
                        }
                    }
                }else {
                    lastReceiver?.OnExit(currentDraggedText.GetComponent<IDroppable>());
                    lastReceiver = null;
                }
                
            }
        }*/
        
        if(Input.GetMouseButtonUp(0)) {
            if (wordIndex != -1) {
                if(!string.IsNullOrEmpty(targetText)) {
                    NotebookViewController notebookViewController =
                        GameObject.Find("Notebook").GetComponent<NotebookViewController>();
                    DroppableTexts droppableTexts =
                        CreateDraggedText(targetText, Color.yellow, notebookViewController).GetComponent<DroppableTexts>();
                    notebookViewController.OnDrop(droppableTexts);
                    //lastReceiver = null;
                    //CurrentDraggedText = targetText;
                }
            }
        }
	}

    private void OnDestroy() {
        //SubtitleHightlightedTextRecorder.CurrentDraggedText = null;
    }

    private TMP_Text CreateDraggedText(string targetText, Color color, NotebookViewController notebook) {
        TMP_Text spawnedText = GameObject
            .Instantiate(this.GetUtility<ResLoader>().LoadSync<GameObject>("general", "DraggedText"), transform)
            .GetComponent<TMP_Text>();
        Vector2 notebookScreenPos = Vector2.zero;
        
        if (notebook) {
            notebookScreenPos = Camera.main.WorldToScreenPoint(notebook.transform.position);
        }
       
        spawnedText.GetComponent<DroppableTexts>().SetContent(targetText, color, notebookScreenPos);
        return spawnedText;
    }

    private List<string> GetNeighbours(int wordIndex) {
        List<string> neighbours = new List<string>();
        if(wordIndex > 0) {
            neighbours.Add(text.textInfo.wordInfo[wordIndex - 1].GetWord());
        }
        if(wordIndex < text.textInfo.wordCount - 1) {
            neighbours.Add(text.textInfo.wordInfo[wordIndex + 1].GetWord());
        }
        return neighbours;
    }

    private List<string> GetRichTexts() {
        //get all rich texts in between <> and </> tags
        List<string> richTexts = new List<string>();
        string pattern = @"<color=.*?>(.*?)<\/color>";
        MatchCollection matches = Regex.Matches(text.text, pattern);
        foreach (Match match in matches)
        {
            richTexts.Add(match.Groups[1].Value.ToUpper());
        }

        return richTexts;
    }
}
