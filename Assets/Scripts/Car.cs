using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class Car : MonoBehaviour
{
    public enum MoveDirection
    {
        Left,
        Right
    }
    [SerializeField] private CarModel model;
    private MoveDirection _moveDirection;
    private SpriteRenderer _renderer;
    private Rigidbody2D _rigidbody;
    private float _speed = 1f;
    private bool _moving;
    private Vector2 _dir;
    public float Speed { get => _speed; set => _speed = value; }
    public MoveDirection Direction
    {
        get => _moveDirection;
        set
        {
            switch (value)
            {
                case MoveDirection.Left:
                    _renderer.flipX = false;
                    _dir = Vector2.left;
                    break;
                case MoveDirection.Right:
                    _renderer.flipX = true;
                    _dir = Vector2.right;
                    break;
            }
            _moveDirection = value;
        }
    }
    public Sprite CarSprite => model.Sprite;
    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        _renderer.sprite = model.Sprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("Player");
            var rend = collision.GetComponent<SpriteRenderer>();
            Debug.Log(_renderer.sprite.texture);
            rend.material.SetTexture("_MainTex1", _renderer.sprite.texture);
            Debug.Log(rend.material.GetTexture("_MainTex1").name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            collision.GetComponent<PlayerController>().SetDefaultShaderTexture();
    }

    private void Update()
    {
        RaycastHit2D hit;
        Vector2 start = transform.position;
        start.x += _renderer.flipX ? 2f : -2f;
        if (hit = Physics2D.Raycast(start, _dir, 1.8f))
        {
            Debug.DrawRay(start, _dir * 1.8f, Color.red);
            if(hit.collider.tag == "Car" && hit.transform != transform)
            {
                float carSpeed = hit.transform.GetComponent<Car>().Speed;
                if (Speed != carSpeed)
                {
                    float speedRatio = Speed / carSpeed;
                    if((speedRatio-1) < 0.01f)
                    {
                        Speed = carSpeed;
                    }
                    float newSpeed = (Speed - carSpeed) / (24/speedRatio);
                    Speed -= newSpeed;
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
                transform.Translate(_dir * Speed * Time.deltaTime);
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
