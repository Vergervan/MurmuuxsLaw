using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemCell : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
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
    private GraphicRaycaster gr;
    private RectTransform rectTransform;
    private Vector2 posBeforeDrag;

    private void Awake() {
        rectTransform = itemImage.gameObject.GetComponent<RectTransform>();
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
        rectTransform.anchoredPosition += (eventData.delta / 2) / canvas.scaleFactor; //В конце делим на скейл родительского объекта
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        posBeforeDrag = rectTransform.anchoredPosition;
        itemImage.transform.SetAsLastSibling();
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition = posBeforeDrag;
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
        Type = Inventory.ItemType.Nothing;
    }
}
