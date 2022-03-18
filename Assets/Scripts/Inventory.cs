using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Inventory : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;

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
    public List<ItemCell> items = new List<ItemCell>();
    void Awake()
    {
        LoadSprites();
    }
    void Start()
    {
        for(int i = 0; i < 6; i++)
        {
            items.Add(GameObject.Find("ItemCell" + i).GetComponent<ItemCell>());
        }
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
}
