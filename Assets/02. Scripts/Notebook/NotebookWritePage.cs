using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.VersionControl;

public class NotebookWritePage : MonoBehaviour
{
    public TMP_InputField inputField;
    public string noteString;
    private void Awake()
    {
        inputField.caretColor = Color.clear;
        inputField.onValueChanged.AddListener(OnInputFieldValueChang);
    }

    public void Update()
    {
        if (inputField.text.Length > 0)
        {
            inputField.MoveToEndOfLine(false, true);
        }
    }
    
    private void OnInputFieldValueChang(string inputInfo)
    {
        if (inputInfo.Length < noteString.Length)
        {
            inputField.text = noteString;
        }
        else
        {
            inputField.text = inputInfo;
        }

        noteString = inputField.text;
    } 
}
