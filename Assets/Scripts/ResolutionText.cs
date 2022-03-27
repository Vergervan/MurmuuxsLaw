using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class ResolutionText : MonoBehaviour
{
    private TMP_Text textBox;
    private void Start()
    {
        textBox = GetComponent<TMP_Text>();
    }
    void Update()
    {
        textBox.text = Screen.width + ":" + Screen.height;
    }
}
