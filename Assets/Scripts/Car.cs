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
    private float triggerDistance = 3f;
    private float _speed = 1f, _startSpeed = 0, _frontCarSpeed = 0, k, m;
    private float _firstSpeed, _strivingSpeed;
    private bool _moving, _changingSpeed;
    private Vector2 _dir;
    public bool IsChangingSpeed => _changingSpeed;
    public float StrivingSpeed => _strivingSpeed;
    public float Speed
    {
        get
        {
            return _speed;
        }
        set
        {
            _speed = value;
            triggerDistance = (_speed - 3f) * 1.8f;
        }
    }
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
        if (hit = Physics2D.Raycast(start, _dir, triggerDistance))
        {
            Debug.DrawRay(start, _dir * triggerDistance, Color.red);
            var tag = hit.collider.tag;
            switch (tag) 
            {
                case "Car":
                    if(hit.transform != transform)
                        OnCarHit(hit.transform.GetComponent<Car>(), start, hit);
                    break;
                case "Player":
                    OnPlayerHit(hit);
                    break;
            }
        }
    }
    private void OnPlayerHit(RaycastHit2D hit)
    {

    }
    private void OnCarHit(Car car, Vector2 start, RaycastHit2D hit)
    {
        if (car.Speed >= Speed) return;
        if (_startSpeed == 0f)
        {
            _frontCarSpeed = car.IsChangingSpeed ? car.StrivingSpeed : car.Speed;
            _startSpeed = Speed - _frontCarSpeed;
            float _stopDistance = Random.Range(0.2f, 1f);
            m = -_startSpeed * _stopDistance / (triggerDistance - _stopDistance);
            k = m / _stopDistance;
            _changingSpeed = true;
            _strivingSpeed = _frontCarSpeed;
        }
        else
        {
            if ((Speed - _frontCarSpeed) < 0.0001f)
            {
                Speed = _frontCarSpeed;
                _startSpeed = 0f;
                _changingSpeed = false;
            }
            else
            {
                float distance = (hit.point - start).magnitude;
                if (distance < 0.1f)
                {
                    Speed = _frontCarSpeed;
                    _startSpeed = 0f;
                }
                else
                {
                    Speed = _frontCarSpeed + (k / (-1 / distance)) + m;
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
