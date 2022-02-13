using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private DialogueWindow window;
    [SerializeField] private RectTransform speechSelector;
    [SerializeField] private RectTransform contentMask;
    [SerializeField] private RectTransform content;
    [SerializeField] private TMP_Text textPrefab;
    [SerializeField] private List<ChoiceItem> choices = new List<ChoiceItem>();
    private int currentSelection = 0;
   
    void Start()
    {
        choices.AddRange(content.GetComponentsInChildren<ChoiceItem>(true));
    }

    private void Update()
    {
        if (window.IsOpened)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && currentSelection > 0)
            {
                --currentSelection;
                SelectChoice(currentSelection, KeyCode.UpArrow);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && currentSelection != choices.Count - 1)
            {
                ++currentSelection;
                SelectChoice(currentSelection, KeyCode.DownArrow);
            }
        }
    }
    public void SelectChoice(int index, KeyCode arrow = KeyCode.None)
    {
        ChoiceItem item = choices[index];
        Canvas.ForceUpdateCanvases();
        Vector2 selectorPos = Vector2.zero;
        switch (arrow)
        {
            case KeyCode.UpArrow:
                selectorPos = ScrollToTop(item);
                break;
            case KeyCode.DownArrow:
                selectorPos = ScrollToBottom(item);
                break;
            default:
                selectorPos = item.TextRect.anchoredPosition;
                break;
        }
        speechSelector.anchoredPosition = selectorPos;
        speechSelector.sizeDelta = item.TextRect.sizeDelta;
    }
    private Vector2 ScrollToTop(ChoiceItem item)
    {
        float fullHeight = GetHeightFromBottom();
        float diff = fullHeight - (content.sizeDelta.y - content.anchoredPosition.y);
        
        Vector2 newPos = content.anchoredPosition;
        if(currentSelection == 0)
        {
            newPos.y = 0;
        }
        else if (diff > 0f)
        {
            newPos.y -= diff;
        }
        content.anchoredPosition = newPos;
        Vector2 selectorPos = item.TextRect.anchoredPosition;
        selectorPos.y += newPos.y;
        return selectorPos;
    }

    private Vector2 ScrollToBottom(ChoiceItem item)
    {
        float fullHeight = GetHeight();
        float diff = contentMask.sizeDelta.y - fullHeight;
        Vector2 newPos = content.anchoredPosition;
        Vector2 selectorPos = item.TextRect.anchoredPosition;
        if (diff < 0f)
        {
            if (content.anchoredPosition.y == 0f)
            {
                newPos.y -= diff;
            }
            else
            {
                newPos.y += item.TextRect.sizeDelta.y;
            }
            selectorPos.y -= diff;
            content.anchoredPosition = newPos;
        }
        return selectorPos;
    }
    private float GetHeight()
    {
        float sum = 0f, counter = -1;
        foreach(var item in choices)
        {
            sum += item.TextRect.sizeDelta.y;
            ++counter;
            if (counter == currentSelection) break;
        }
        return sum;
    }
    //Returns a height from the bottom to the selected item
    private float GetHeightFromBottom()
    {
        float sum = 0f, counter = choices.Count;
        for(int i = choices.Count-1; i != 0; --i)
        {
            sum += choices[i].TextRect.sizeDelta.y;
            --counter;
            if (counter == currentSelection) break;
        }
        return sum;
    }
}