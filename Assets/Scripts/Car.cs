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
    private float _speed = 1f, _startSpeed = 0, _frontCarSpeed = 0, k, m, _deltaSpeed = 0f;
    private float _strivingSpeed;
    private bool _moving, _changingSpeed;
    private Vector2 _dir;
    private bool _isFrontObject = false;
    public bool IsObjectInFront => _isFrontObject;
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
            if (_speed < 0f)
                _speed = 0f;
            triggerDistance = (_speed - 3f) * 1.5f;
            if (_speed < 3f)
                triggerDistance = 3f;
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
            var rend = collision.GetComponent<SpriteRenderer>();
            rend.material.SetTexture("_MainTex1", _renderer.sprite.texture);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            collision.GetComponent<PlayerController>().SetDefaultShaderTexture();
    }

    private void Update()
    {
        Vector2 start = transform.position;
        start.x += _renderer.flipX ? 2f : -2f;
        RaycastHit2D hit = Physics2D.Raycast(start, _dir, triggerDistance);
        if (hit.collider)
        {
            float distance = (hit.point - start).magnitude;
            Debug.DrawRay(start, _dir * distance, Color.red);
            var tag = hit.collider.tag;
            switch (tag) 
            {
                case "Car":
                    if(hit.transform != transform)
                        OnCarHit(hit.transform.GetComponent<Car>(), distance);
                    break;
                case "Player":
                    OnPlayerHit(start, hit);
                    break;
            }
        }
        else if (_isFrontObject)
        {
            Debug.Log("Front object");
            _isFrontObject = false;
            Speed = _startSpeed;
            _deltaSpeed = 0f;
        }
    }
    private void OnPlayerHit(Vector2 start, RaycastHit2D hit)
    {
        _isFrontObject = true;
        if (_startSpeed == 0f)
        {
            _startSpeed = Speed;
            float _stopDistance = Random.Range(0.2f, 1f);
            m = -_startSpeed * _stopDistance / (triggerDistance - _stopDistance);
            k = m / _stopDistance;
            _changingSpeed = true;
            _strivingSpeed = 0f;
        }
        else
        {
            if (Speed < 0.0001f)
            {
                Speed = 0;
                _changingSpeed = false;
            }
            else
            {
                Speed = (k / (-1 / (hit.point-start).magnitude)) + m;
            }
        }
    }
    private void OnCarHit(Car car, float distance)
    {
        if (car.Speed >= Speed && car.StrivingSpeed >= Speed) return; //Если машина впереди быстрее нашей, то ничего не делаем
        if (_deltaSpeed == 0f) //deltaSpeed = 0, когда машина не изменяет свою скорость
        {
            //_startSpeed = _deltaSpeed;
            _frontCarSpeed = car.IsChangingSpeed ? car.StrivingSpeed : car.Speed; //Проверяем стремиться ли машина спереди к какой-либо скорости
            _deltaSpeed = Speed - _frontCarSpeed;
            float _stopDistance = Random.Range(0.2f, 1f);
            m = -_deltaSpeed * _stopDistance / (triggerDistance - _stopDistance);
            k = m / _stopDistance;
            _changingSpeed = true;
            _strivingSpeed = _frontCarSpeed;
        }
        else
        {
            if (_deltaSpeed < 0.0001f)
            {
                Speed = _frontCarSpeed;
                _deltaSpeed = 0f;
                _changingSpeed = false;
            }
            else
            {
                Speed = _frontCarSpeed + (k / (-1 / distance)) + m;
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
