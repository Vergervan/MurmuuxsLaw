using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CarModel : ScriptableObject
{
    [SerializeField] private Sprite carSprite;
    public Sprite Sprite => carSprite;
}

public class Car : MonoBehaviour
{
    [SerializeField] private CarModel model;
    public Sprite CarSprite => model.Sprite;
}
