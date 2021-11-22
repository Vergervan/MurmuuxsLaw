using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartWindow : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private SceneManager sceneManager;
    void Start()
    {
        StartCoroutine(ChangeText());
    }
    
    public IEnumerator ChangeText()
    {
        yield return new WaitForSeconds(5f);
        text.text = "Как с разработкой этой игры";
        yield return new WaitForSeconds(5f);
        sceneManager.SetScene("mainmenu");
    }
}
