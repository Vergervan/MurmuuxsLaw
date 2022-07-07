using UnityEngine;
using DG.Tweening;

public class ItemMenuManager : MonoBehaviour
{
    [SerializeField] private RectTransform menuObject;
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform menuConnector;
    [SerializeField] private Canvas canvas;
    [SerializeField] private bool _enabled;
    private ItemCell _currentCell = null;
    private Vector2 menuScaledSize;

    private void Start()
    {
        menuScaledSize = menuObject.sizeDelta * menuObject.localScale;
    }

    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            menuObject.gameObject.SetActive(value);
            menuConnector.gameObject.SetActive(value);
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

        menuConnector.position = new Vector2(pos.x, pos.y + (77.5f * canvas.scaleFactor));

        //float gapX = canvas.pixelRect.width - (pos.x + (menuScaledSize.y * 0.5f) * canvas.scaleFactor);
        //Debug.Log("Gap: " + gapX);
        //if (gapX < 0f)
        //{
        //    pos.x += gapX * 0.5f;
        //}
        pos.y += ((objectSize.y * 0.5f + 11f) * canvas.scaleFactor);
        menuObject.transform.position = pos;
        if (!Enabled) Enabled = true;

        AppearMenu();
    }

    private void AppearMenu()
    {
        content.gameObject.SetActive(false);
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
