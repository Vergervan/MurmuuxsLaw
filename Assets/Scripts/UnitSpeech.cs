﻿using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class UnitSpeech
{
    private List<string> m_dialogueList;
    [SerializeField] private int index = 0;
    public int ExcerptCount => m_dialogueList == null ? 0 : m_dialogueList.Count;
    public UnitSpeech()
    {
        m_dialogueList = new List<string>();
    }
    public UnitSpeech(IEnumerable<string> strarr)
    {
        m_dialogueList = new List<string>();
        AddText(strarr);
    }
    public bool HasNext() => (index + 1 == m_dialogueList.Count) ? false : true;
    public string Next()
    {
        ++index;
        return (index == m_dialogueList.Count) ? null : m_dialogueList[index];
    }
    public string CurrentText() => m_dialogueList[index];
    public void SetToStart() => index = 0;

    public void AddText(IEnumerable<string> strarr)
    {
        foreach(var str in strarr)
        {
            AddText(str.Trim());
        }
    }
    public void AddText(string str) => m_dialogueList.Add(str);
    public void ClearDialogue()
    {
        m_dialogueList.Clear();
        index = 0;
    }

    public UnitSpeech Clone() => new UnitSpeech(m_dialogueList);
}
