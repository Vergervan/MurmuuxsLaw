using System;
using System.Collections.Generic;
using UnityEngine;

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
    private SpriteRenderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.material.EnableKeyword("_MainTex");
        _renderer.material.EnableKeyword("_Scale");
        _renderer.material.SetTexture("_MainTex", _renderer.material.mainTexture);
        try
        {
            _renderer.sprite = Inventory.ItemSprites[item];
            foreach (var prop in ItemProperties[item])
            {
                gameObject.AddComponent(ItemPropertyScripts[prop]);
            }
        }
        catch (KeyNotFoundException) { }
    }

    private void OnMouseEnter()
    {
        _renderer.material.SetFloat("_Scale", 0.95f);
    }

    private void OnMouseExit()
    {
        _renderer.material.SetFloat("_Scale", 1f);
    }
}
