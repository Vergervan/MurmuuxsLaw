using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class Car : MonoBehaviour
{
    [SerializeField] private CarModel model;
    private SpriteRenderer _renderer;
    private Rigidbody2D _rigidbody;
    private float _speed = 1f;
    private bool _moving;
    public float Speed { get => _speed; set => _speed = value; }
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

    private void Update()
    {
        RaycastHit2D hit;
        Vector2 start = transform.position;
        start.x += 2f;
        if(hit = Physics2D.Raycast(start, Vector2.right, 1.5f))
        {
            Debug.DrawRay(start, Vector2.right * 1.5f, Color.red);
            if(hit.collider.tag == "Car" && hit.transform != transform)
            {
                float carSpeed = hit.transform.GetComponent<Car>().Speed;
                if (Speed != carSpeed)
                {
                    float speedRatio = Speed / carSpeed;
                    float newSpeed = (Speed - carSpeed) / (24/speedRatio);
                    Speed -= newSpeed;
                    if (Speed < carSpeed)
                        Speed = carSpeed;
                    Debug.Log(Speed);
                }
            }
        }
    }

    public void SetModel(CarModel model)
    {
        this.model = model;
        _renderer.sprite = model.Sprite;
    }

    public async void MoveTo(Vector3 endpoint)
    {
        _moving = true;
        try
        {
            while (_moving)
            {
                transform.Translate(Vector2.right * Speed * Time.deltaTime);
                await Task.Yield();
            }
        }
        catch (System.Exception) { }
    }
    public void StopMove()
    {
        _moving = false;
    }
}
