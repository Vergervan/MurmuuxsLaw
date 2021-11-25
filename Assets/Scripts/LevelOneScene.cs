using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelOneScene : MonoBehaviour
{
    [SerializeField] private Animator trashAnim;
    [SerializeField] private GameObject player;
    [SerializeField] private SceneManager sceneManager;
    [SerializeField] private LanguageManager languageManager;

    private void OnEnable()
    {
        player.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            sceneManager.SetScene("mainmenu");
            languageManager.LoadTextFlags(sceneManager.GetCurrentScene());
        }
    }

    public void EndAnim()
    {
        trashAnim.SetBool("endAnim", true);
        Vector2 startPos = new Vector2(-20.7f, 1.37f);
        player.transform.position = startPos;
        player.SetActive(true);
    }
}
