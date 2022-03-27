using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class ObjectPosition : MonoBehaviour
{
    private TMP_Text textBox;
    [SerializeField] private Transform obj;
    private void Start()
    {
        textBox = GetComponent<TMP_Text>();
    }
    void Update()
    {
        textBox.text = string.Format("{0:0.###}:{0:0.###}", obj.position.x, obj.position.y);        
    }
}
