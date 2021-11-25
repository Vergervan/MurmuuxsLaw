using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelItem : MonoBehaviour
{
    public enum ItemProperty
    {
        Slip
    }
    private static Dictionary<ItemProperty, Type> ItemPropertyScripts = new Dictionary<ItemProperty, Type>()
    {
        {ItemProperty.Slip, typeof(SlipItemEffect)}
    };
    public static Dictionary<Inventory.ItemType, ItemProperty[]> ItemProperties = new Dictionary<Inventory.ItemType, ItemProperty[]>()
    {
        { Inventory.ItemType.Cake, new ItemProperty[]{ 
                ItemProperty.Slip
            } 
        }
    };

    public Inventory.ItemType item;

    private void Start()
    {
        try
        {
            GetComponent<SpriteRenderer>().sprite = Inventory.ItemSprites[item];
            foreach (var prop in ItemProperties[item])
            {
                gameObject.AddComponent(ItemPropertyScripts[prop]);
            }
        }
        catch (KeyNotFoundException) { }
    }
}
