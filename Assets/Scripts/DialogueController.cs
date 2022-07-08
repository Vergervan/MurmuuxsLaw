using DialogScriptCreator;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private ActionTrigger actionTrigger;
    [SerializeField] private DialogueWindow window;
    [SerializeField] private RectTransform speechSelector;
    [SerializeField] private RectTransform contentMask;
    [SerializeField] private RectTransform content;
    [SerializeField] private GameObject choicePrefab;
    [SerializeField] private List<Route> choicesRoutes = new List<Route>();
    private List<ChoiceItem> choices = new List<ChoiceItem>();
    private int currentSelection = 0;
    private NPCBehaviour currentNpc;
    public int ChoicesCount { get => choicesRoutes.Count; }
    public DialogueWindow GetDialogWindow() => window;
    private void Update()
    {
        if (window.IsOpened && !window.IsProcessing)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Route route = choicesRoutes[currentSelection];
                currentNpc.ChangeDialog(route.To);
                if (route.Switchable && !route.To.HasAvailableRoutes)
                {
                    route.TurnOff();
                    CheckParentRoutes(route);
                }
                if (route.HasTriggers)
                {
                    foreach(var trigger in route.Triggers)
                    {
                        actionTrigger.TryToInvokeEvent(trigger);
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) && currentSelection > 0)
            {
                SelectChoice(--currentSelection, KeyCode.UpArrow);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && currentSelection != choices.Count - 1)
            {
                SelectChoice(++currentSelection, KeyCode.DownArrow);
            }
        }
    }
    private void CheckParentRoutes(Route route)
    {
        if (Equals(route.Parent, null) || Equals(route.Parent.ParentRoute, null)) return;
        if(!route.Parent.ParentRoute.To.HasAvailableRoutes)
        {
            route.Parent.ParentRoute.TurnOff();
            CheckParentRoutes(route.Parent.ParentRoute);
        }
    }
    public void SetRoutes(NPCBehaviour npc, IEnumerable<Route> routes)
    {
        currentSelection = 0;
        currentNpc = npc;
        choicesRoutes.Clear();
        //foreach(var route in routes)
        //{
        //    if (route.Available && route.ConditionsMet && (!route.To.IsDialog || route.To.HasAvailableRoutes))
        //        choicesRoutes.Add(route);
        //}
        choicesRoutes.AddRange(routes.Where(route => route.Available && route.ConditionsMet && (!route.To.IsDialog || route.To.HasAvailableRoutes)));
        ClearContent();
        BuildDialogWindowItems();
        //window.ToggleWindow();
    }
    private void ClearContent()
    {
        foreach(Transform obj in content.transform)
        {
            Destroy(obj.gameObject);
        }
    }
    private void BuildDialogWindowItems()
    {
        choices.Clear();
        foreach (var item in choicesRoutes) 
        {
            ChoiceItem choice = Instantiate(choicePrefab, content).GetComponent<ChoiceItem>();
            var speech = LanguageManager.reader.GetUnitSpeech(item.From.Value);
            choice.Text = speech.CurrentText();
            choices.Add(choice);
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
