using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Collider2D))]
public class CarPoint : MonoBehaviour
{
    [SerializeField] private bool isEndpoint;
    private Collider2D _collider;
    [SerializeField] private bool _carInTrigger;
    public bool IsCarInTrigger => _carInTrigger;
    [SerializeField] private int _carsInTrigger = 0;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.isTrigger);
        if (collision.tag == "Car")
        {
            if (isEndpoint)
            {
                collision.transform.GetComponent<Car>().StopMove();
                Destroy(collision.gameObject);
            }
            _carsInTrigger++;
        }
        RefreshTriggerOptions();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Car")
            _carsInTrigger--;
        RefreshTriggerOptions();
    }

    private void RefreshTriggerOptions()
    {
        _carInTrigger = _carsInTrigger > 0;
    }

    public Car CreateCar(Car prefab, CarModel model)
    {
        Car car = Instantiate(prefab.gameObject, transform.position, Quaternion.identity).GetComponent<Car>();
        car.SetModel(model);
        return car;
    }
}
