using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] private bool _muteSpawn = false;
    [SerializeField] private CarPoint startPoint, endPoint;
    [SerializeField] private int _spawnDelayMin, _spawnDelayMax;
    [SerializeField] private float _minSpeed, _maxSpeed;
    [SerializeField] private Car _carPrefab;
    private List<CarModel> _models;
    private bool _work = false;

    private void Awake()
    {
        if (_models == null || _models.Count <= 0)
        {
            _models = Resources.LoadAll<CarModel>("Models/Cars").ToList();
        }
    }

    public async void StartSpawn()
    {
        _work = true;
        try
        {
            while (_work)
            {
                await Task.Delay(Random.Range(_spawnDelayMin, _spawnDelayMax));
                if (_muteSpawn) continue;
                Car car = startPoint.CreateCar(_carPrefab, _models[Random.Range(0, _models.Count - 1)]);
                car.tag = "Car";
                if ((car.gameObject.transform.position.x - endPoint.transform.position.x) < 0)
                    car.Direction = Car.MoveDirection.Right;
                else
                    car.Direction = Car.MoveDirection.Left;
                car.Speed = Random.Range(_minSpeed, _maxSpeed);
                car.MoveTo(endPoint.transform.position);
                await Task.Yield();
            }
        }
        catch (System.Exception) { }
    }
    public void StopSpawn()
    {
        _work = false;
    }
}
