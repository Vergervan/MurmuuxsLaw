using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] private bool _muteSpawn = false;
    [SerializeField] private CarPoint startPoint, endPoint;
    [SerializeField] private int _spawnDelayMin, _spawnDelayMax;
    [SerializeField] private float _minSpeed, _maxSpeed;
    [SerializeField] private Car _carPrefab;
    [SerializeField] private List<CarModel> _models;
    private bool _work = false;

    public async void StartSpawn()
    {
        _work = true;
        while (_work)
        {
            await Task.Delay(Random.Range(_spawnDelayMin, _spawnDelayMax));
            if (_muteSpawn) continue;
            Car car = startPoint.CreateCar(_carPrefab, _models[Random.Range(0, _models.Count-1)]);
            car.tag = "Car";
            if ((car.gameObject.transform.position.x - endPoint.transform.position.x) < 0)
                car.GetComponent<SpriteRenderer>().flipX = true;
            car.Speed = Random.Range(_minSpeed, _maxSpeed);
            car.MoveTo(endPoint.transform.position);
            await Task.Yield();
        }
    }
    public void StopSpawn()
    {
        _work = false;
    }

}
