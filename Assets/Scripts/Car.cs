using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class Car : MonoBehaviour
{
    [SerializeField] private CarModel model;
    private SpriteRenderer _renderer;
    private Rigidbody2D _rigidbody;
    public Sprite CarSprite => model.Sprite;
    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        _renderer.sortingOrder = 14;
        _renderer.sprite = model.Sprite;
    }

    public void SetModel(CarModel model)
    {
        this.model = model;
        _renderer.sprite = model.Sprite;
    }

    public void MoveTo(Vector3 endpoint)
    {
        transform.DOMove(endpoint, 10f).SetEase(Ease.Linear);
    }
}
