using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CarPoint : MonoBehaviour
{
    [SerializeField] private bool isEndpoint;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isEndpoint && collision.tag == "Car")
            Destroy(collision.gameObject);
    }

    public void CreateCar(Car car)
    {
        Instantiate(car.gameObject);
    }
}
