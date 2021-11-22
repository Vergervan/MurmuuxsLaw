using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelOneScene : MonoBehaviour
{
    [SerializeField] private Animator trashAnim;
    [SerializeField] private GameObject player;

    private void Start()
    {
        player.SetActive(false);
    }

    public void EndAnim()
    {
        trashAnim.SetBool("endAnim", true);
        player.SetActive(true);
    }
}
