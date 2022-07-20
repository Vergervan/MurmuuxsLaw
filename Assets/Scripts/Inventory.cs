using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventory : MonoBehaviour, ILoadSavedData
{
    [SerializeField] private DialogueManager dialogueManager;
    private SavedDataHandler dataHandler;

    [Serializable]
    public enum ItemType
    {
        Nothing, Pistol, Phone, Cake, Wallet, Magazine, Tape
    }
    private static readonly Dictionary<ItemType, string> SpriteNames = new Dictionary<ItemType, string>(){
        {ItemType.Nothing, string.Empty},
        {ItemType.Pistol, "item-atlas_0"},
        {ItemType.Phone, "item-atlas_1"},
        {ItemType.Cake, "item_cake"},
        {ItemType.Wallet, "item_wallet"},
        {ItemType.Magazine, "item_magazine"},
        {ItemType.Tape, "item_tape"}
    };
    public static Dictionary<ItemType, Sprite> ItemSprites = new Dictionary<ItemType, Sprite>();
    private const int inventoryCells = 6;
    [SaveField] private List<ItemCell> items = new List<ItemCell>();
    void Awake()
    {
        dataHandler = SavedDataHandler.GetInstance();
        dataHandler.Subscribe(this);
        LoadSprites();
    }
    private void OnDestroy()
    {
        dataHandler.Unsubscribe(this);
    }
    void Start()
    {
        for(int i = 0; i < inventoryCells; i++)
        {
            items.Add(GameObject.Find("ItemCell" + i).GetComponent<ItemCell>());
        }
    }
    void ILoadSavedData.OnLoad()
    {

    }

    private void LoadSprites()
    {
        var sprites = Resources.LoadAll<Sprite>("Items");
        foreach(var sprite in sprites)
        {
            foreach(var itemType in (ItemType[])Enum.GetValues(typeof(ItemType))){
                if (sprite.name == SpriteNames[itemType])
                    ItemSprites.Add(itemType, sprite);
            }
        }
    }
    public void AddItemToInventory(ItemType type)
    {
        foreach(var cell in items)
        {
            if(cell.Type == ItemType.Nothing)
            {
                cell.Type = type;
                break;
            }
        }
        SetConditionValue(type.ToString(), true);
    }
    public void RefreshConditions()
    {
        DialogScriptCreator.ConditionKeeper keeper = dialogueManager.GetConditionKeeper();
        foreach (var cell in items)
        {
            if(cell.Type != ItemType.Nothing)
            {
                var cellStr = cell.Type.ToString();
                if (keeper.HasCondition(cellStr))
                {
                    keeper.SetConditionValue(cellStr, true);
                }
            }
        }
    }
    public void SetConditionValue(string name, bool b)
    {
        DialogScriptCreator.ConditionKeeper keeper = dialogueManager.GetConditionKeeper();
        if (keeper.HasCondition(name))
            keeper.SetConditionValue(name, b);
    }
    public bool HasFreeCells()
    {
        foreach(var cell in items)
            if (cell.Type == ItemType.Nothing)
                return true;
        return false;
    }
    public void RemoveItem(ItemType itemType)
    {
        foreach (var cell in items)
        {
            if (cell.Type == itemType)
            {
                cell.SetItem(ItemType.Nothing);
                return;
            }
        }
    }
    public void RemoveItem(string itemName)
    {
        foreach (var cell in items)
        {
            if (cell.Type.ToString() == itemName)
            {
                cell.SetItem(ItemType.Nothing);
                return;
            }
        }
    }
}
