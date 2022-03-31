using UnityEngine;
using UnityEngine.Events;

public class Scene : MonoBehaviour
{
    public Vector2 CameraPosition;
    public string SceneFlag;
    public GameObject[] DependentObjects;
    public GameObject[] InactiveObjects;
    public UnityEvent OnStartScene;
}
