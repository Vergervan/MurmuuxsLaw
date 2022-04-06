using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Collider2D))]
public class CarPoint : MonoBehaviour
{
    [SerializeField] private bool isEndpoint;
    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isEndpoint && collision.tag == "Car")
        {
            collision.transform.DOKill();
            collision.transform.GetComponent<Car>().StopMove();
            Destroy(collision.gameObject);
        }
    }

    public Car CreateCar(Car prefab, CarModel model)
    {
        Car car = Instantiate(prefab.gameObject, transform.position, Quaternion.identity).GetComponent<Car>();
        car.SetModel(model);
        return car;
    }
}
