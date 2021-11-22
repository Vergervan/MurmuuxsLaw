using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCityEvents : MonoBehaviour
{
    [SerializeField] private GameObject suspectEyes;
    void Start()
    {
        suspectEyes.SetActive(false);
        StartCoroutine(RandomActivateTime(suspectEyes));
    }
    private IEnumerator RandomActivateTime(GameObject obj)
    {
        obj.SetActive(true);
        Animator animator = obj.GetComponent<Animator>();
        while (true)
        {
            System.Random rand = new System.Random();
            int randTime = rand.Next(10, 150);
            yield return new WaitForSeconds(randTime);
            animator.SetBool("show", true);
            yield return new WaitForSeconds(6);
            animator.SetBool("show", false);
            yield return null;
        }
    }
}
