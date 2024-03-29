﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemCell : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeReference] LevelItem item;
    [SerializeField]
    private Inventory.ItemType _type;
    public Inventory.ItemType Type
    {
        get { return _type; }
        set
        {
            _type = value;
            SetItemSprite();
        }
    }
    [SerializeField] private Image itemImage;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Canvas canvas;
    [SerializeField] private ItemMenuManager itemMenuManager;
    private GraphicRaycaster gr;
    private RectTransform rectTransform, imageTransform;
    private Vector2 posBeforeDrag;


    public override string ToString()
    {
        return _type.ToString();
    }
    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        imageTransform = itemImage.gameObject.GetComponent<RectTransform>();
        inventory = GetComponentInParent<Inventory>();
        gr = GetComponentInParent<GraphicRaycaster>();
    }
    private void Start()
    {
        SetItem(Inventory.ItemType.Nothing);
    }
    public void SetItem(Inventory.ItemType type)
    {
        Type = type;
    }
    private void SetItemSprite()
    {
        if (_type == Inventory.ItemType.Nothing)
        {
            itemImage.sprite = null;
            SetVisibleSprite(false);
            return;
        }
        itemImage.sprite = Inventory.ItemSprites[_type];
        SetVisibleSprite(true);
    }
    public Sprite GetItemSprite()
    {
        return itemImage.sprite;
    }

    public void SetVisibleSprite(bool b)
    {
        Color color = itemImage.color;
        color.a = b ? 255f : 0f;
        itemImage.color = color;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Type == Inventory.ItemType.Nothing) return;
        if(eventData.button == PointerEventData.InputButton.Left) imageTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; //В конце делим на скейл родительского объекта
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            posBeforeDrag = imageTransform.anchoredPosition;
            itemImage.transform.SetAsLastSibling();
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        imageTransform.anchoredPosition = posBeforeDrag;
        if (Type == Inventory.ItemType.Nothing)
        {
            Debug.Log("Empty");
            return;
        }
        PointerEventData data = new PointerEventData(EventSystem.current);
        data.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        ItemCell itemCell = null;
        gr.Raycast(data, results);
        foreach(var result in results)
        {
            if (result.gameObject.tag == "ItemCell")
            {
                itemCell = result.gameObject.GetComponent<ItemCell>();
                break;
            }
        }
        if (itemCell == null)
        {
            DropItem();
            return;
        }
        SwapItems(this, itemCell);
        
    }
    public static void SwapItems(ItemCell item1, ItemCell item2)
    {
        Inventory.ItemType type = item1.Type;
        item1.SetItem(item2.Type);
        item2.SetItem(type);
    }
    private void DropItem()
    {
        GameObject droppedItem = Resources.Load("Item") as GameObject;
        droppedItem.GetComponent<LevelItem>().item = Type;
        GameObject player = GameObject.Find("Player");
        Vector2 pos = player.transform.position;
        pos.y -= 1.05f;
        Instantiate(droppedItem, pos, Quaternion.identity, player.transform.parent);
        inventory.SetConditionValue(Type.ToString(), false);
        Type = Inventory.ItemType.Nothing;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            itemMenuManager.ExecuteInPosition(this, itemImage.transform.position, rectTransform.sizeDelta * rectTransform.localScale);
        }
    }
}
