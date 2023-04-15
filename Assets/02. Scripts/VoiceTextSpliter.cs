using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class VoiceTextSpliter {
    private const int minSentenceLength = 36;
    private static char[] splitBy = new[] { '.', '!', '?', ';'};
    public static List<string> Split(string text) {
        List<string> sentences = new List<string>();
        string[] splitText = text.Split(splitBy, StringSplitOptions.RemoveEmptyEntries);
        char[] splitedChars = text.ToCharArray();
        string sentence = "";
        //should include the split char
        int splitIndex = 0;
        foreach (string s in splitText)
        {
            sentence += s;
            splitIndex += s.Length;
            if (splitIndex < splitedChars.Length)
            {
                sentence += splitedChars[splitIndex];
                splitIndex++;
            }
            if (sentence.Length > minSentenceLength)
            {
                sentences.Add(sentence.TrimEnd());
                sentence = "";
            }
        }
        if (sentence.Length > 0)
        {
            sentences.Add(sentence.TrimEnd());
        }
        return sentences;
    }
}
