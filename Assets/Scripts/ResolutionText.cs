using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

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
