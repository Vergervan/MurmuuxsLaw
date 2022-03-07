using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class TextFlag : MonoBehaviour
{
    [SerializeField] private string _flagName;
    [SerializeField] private UnitSpeech _flagValue;
    private TMP_Text _text;
    private void Awake()
    {
        _text = transform.GetComponent<TMP_Text>();
        _flagValue = null;
    }
    //private void Start()
    //{
    //    if (_flagValue == null) return;
    //    if (_flagName != string.Empty)
    //    {
    //        Debug.Log(_flagName);
    //        if (_textAutoChange) _text.text = _flagValue.CurrentText();
    //    }
    //}
    public bool HasValue => _flagValue == null ? false : true;
    public string FlagName
    {
        get => _flagName;
    }
    public UnitSpeech FlagValue
    {
        get => _flagValue;
    }
    public void RefreshFlagValue()
    {
        if (_flagValue == null) return;
        try
        {
            _text.text = _flagValue.CurrentText();
        }catch(Exception) { Debug.LogError("Error load flag: " + _flagName); }
    }
    public void SetFlagValue(UnitSpeech speechValue)
    {
        _flagValue = speechValue;
    }
}
