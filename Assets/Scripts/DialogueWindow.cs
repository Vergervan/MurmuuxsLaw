using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueWindow : MonoBehaviour
{
    [SerializeField] private Image buttonBackground;
    [SerializeField] private Sprite altImage;

    [SerializeField] private GameObject windowObject;

    [SerializeField] private GameObject answerVariantPrefab;
    private Sprite baseSprite;
    private bool _opened = false;
    void Start()
    {
        baseSprite = buttonBackground.sprite;
    }
    public void ToggleWindow()
    {
        _opened = !_opened;
        buttonBackground.sprite = _opened ? altImage : baseSprite;
        windowObject.SetActive(_opened);
    }
}
