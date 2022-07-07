using UnityEngine;
using DG.Tweening;

public class ItemMenuManager : MonoBehaviour
{
    [SerializeField] private RectTransform menuObject;
    [SerializeField] private RectTransform content;
    [SerializeField] private Canvas canvas;
    [SerializeField] private bool _enabled;
    private ItemCell _currentCell = null;
    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            menuObject.gameObject.SetActive(value);
        }
    }
    public void ExecuteInPosition(ItemCell cell, Vector2 pos, Vector2 objectSize)
    {
        if (cell != null)
        {
            if (_currentCell == cell && Enabled)
            {
                DisappearMenu();
                _currentCell = null;
                return;
            }
        }
                
        _currentCell = cell;
        Vector2 menuScaledSize = menuObject.sizeDelta * menuObject.localScale;
        Debug.Log("Start pos: " + pos);
        pos.y += (((objectSize.y) * 0.5f) * canvas.scaleFactor);
        menuObject.transform.position = pos;
        Debug.Log("Processed pos: " + pos);
        if (!Enabled) Enabled = true;

        AppearMenu();
    }

    private void AppearMenu()
    {
        content.gameObject.SetActive(false);
        //menuObject.localScale = Vector2.zero;
        //menuObject.transform.DOScale(new Vector3(6, 6, 1), 0.5f);
        Vector2 size = menuObject.sizeDelta;
        menuObject.sizeDelta = new Vector2(size.x, 0);
        menuObject.DOSizeDelta(size, 0.15f).OnComplete(() =>
        {
            content.gameObject.SetActive(true);
        });
    }
    private void DisappearMenu()
    {
        content.gameObject.SetActive(false);
        Vector2 size = menuObject.sizeDelta;
        menuObject.DOSizeDelta(new Vector2(size.x, 0), 0.15f).OnComplete(() =>
        {
            Enabled = false;
            menuObject.sizeDelta = size;
        });
    }
}
