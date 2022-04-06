using UnityEngine;

[CreateAssetMenu]
public class CarModel : ScriptableObject
{
    [SerializeField] private Sprite carSprite;
    public Sprite Sprite => carSprite;
}
