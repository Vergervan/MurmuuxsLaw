using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] private CarPoint startPoint, endPoint;
    [SerializeField] private int _spawnDelay;
    [SerializeField] private List<Car> _cars;
    private bool _work = false;

    public async void StartSpawn()
    {
        _work = true;
        while (_work)
        {
            await Task.Delay(_spawnDelay);
            //startPoint.CreateCar(_cars[0]);
            await Task.Yield();
        }
    }
    public void StopSpawn()
    {
        _work = false;
    }

}
