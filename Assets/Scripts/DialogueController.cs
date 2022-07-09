using DialogScriptCreator;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private InputAction dialogInput;
    [SerializeField] private ActionTrigger actionTrigger;
    [SerializeField] private DialogueWindow window;
    [SerializeField] private RectTransform speechSelector;
    [SerializeField] private RectTransform contentMask;
    [SerializeField] private RectTransform content;
    [SerializeField] private GameObject choicePrefab;
    [SerializeField] private List<Route> choicesRoutes = new List<Route>();
    private List<ChoiceItem> choices = new List<ChoiceItem>();
    private int currentSelection = 0;
    private float contentHeight = 0f;
    private float currentHeight = 0f;
    private float endBorder = 0f;
    private NPCBehaviour currentNpc;
    private bool selectorMoves = false;
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
                UpdatePrefferedHeight();
            }
            UpdateSelector();
        }
    }
    public void UpdatePrefferedHeight()
    {
        contentHeight = LayoutUtility.GetPreferredHeight(content);
        endBorder = -contentMask.sizeDelta.y + contentHeight;
    }

    private void UpdateSelector()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveSelector(KeyCode.UpArrow);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveSelector(KeyCode.DownArrow);
        }
    }

    private async void MoveSelector(KeyCode key)
    {
        if (selectorMoves) return;
        selectorMoves = TryUpdateSelection(key);
        if (!selectorMoves) return;
        SelectChoice(currentSelection, key);
        await Task.Delay(300);
        while (Input.GetKey(key))
        {
            selectorMoves = TryUpdateSelection(key);
            if (!selectorMoves) return;
            SelectChoice(currentSelection, key);
            await Task.Delay(80);
            await Task.Yield();
        }
        selectorMoves = false;
    }

    private bool TryUpdateSelection(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.UpArrow:
                if (currentSelection == 0)
                    return false;
                --currentSelection;
                return true;
            case KeyCode.DownArrow:
                if (currentSelection == choicesRoutes.Count - 1)
                    return false;
                ++currentSelection;
                return true;
        }
        return true;
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
        content.anchoredPosition = new Vector2(content.anchoredPosition.x, 0);
    }

    public void SelectChoice(int index, KeyCode arrow = KeyCode.None)
    {
        ChoiceItem item = choices[index];
        Canvas.ForceUpdateCanvases();
        switch (arrow)
        {
            case KeyCode.UpArrow:
                ScrollToTop(item);
                break;
            case KeyCode.DownArrow:
                ScrollToBottom(item);
                break;
            default:
                speechSelector.anchoredPosition = item.TextRect.anchoredPosition;
                currentHeight = item.TextRect.sizeDelta.y;
                break;
        }
        speechSelector.sizeDelta = item.TextRect.sizeDelta;
    }
    private void ScrollToTop(ChoiceItem item)
    {
        currentHeight -= item.TextRect.sizeDelta.y;
        Vector2 newPos = content.anchoredPosition;
        Vector2 selectorPos = item.TextRect.anchoredPosition;
        float bottomHeight = contentHeight - currentHeight + item.TextRect.sizeDelta.y;
        float diff = bottomHeight - (content.sizeDelta.y - content.anchoredPosition.y);
        if(currentSelection == 0)
        {
            newPos.y = 0;
        }
        else if (diff > 0f)
        {
            newPos.y -= diff;
        }
        content.anchoredPosition = newPos;
        selectorPos.y += newPos.y;
        speechSelector.anchoredPosition = selectorPos;
    }

    private void ScrollToBottom(ChoiceItem item)
    {
        currentHeight += item.TextRect.sizeDelta.y;
        Vector2 newPos = content.anchoredPosition;
        Vector2 selectorPos = item.TextRect.anchoredPosition;
        float diff = currentHeight - (contentMask.sizeDelta.y + content.anchoredPosition.y);
        if(diff > 0f)
        {
            if (currentSelection == (choicesRoutes.Count - 1))
            {
                newPos.y = endBorder;
            }
            else
            {
                newPos.y += diff;
            }
        }
        content.anchoredPosition = newPos;
        selectorPos.y += newPos.y;
        speechSelector.anchoredPosition = selectorPos;
    }
}
