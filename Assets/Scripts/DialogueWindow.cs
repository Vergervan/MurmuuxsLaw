using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueWindow : MonoBehaviour
{
    [SerializeField] private DialogueController controller;
    [SerializeField] private Image buttonBackground;
    [SerializeField] private Sprite altImage;

    [SerializeField] private GameObject windowObject;

    [SerializeField] private GameObject answerVariantPrefab;
    private Sprite baseSprite;
    private bool _opened = false;
    public bool IsOpened { get => _opened; }
    void Start()
    {
        baseSprite = buttonBackground.sprite;
    }
    public void ToggleWindow()
    {
        _opened = !_opened;
        buttonBackground.sprite = _opened ? altImage : baseSprite;
        windowObject.SetActive(_opened);
        if (_opened) 
            controller.SelectChoice(0);
    }
}