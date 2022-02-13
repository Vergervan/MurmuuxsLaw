using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject speechBubblePrefab;
    public SpeechBubble CreateSpeechBubble(Transform target)
    {
        GameObject bubbleObj = Instantiate(speechBubblePrefab, transform);
        SpeechBubble bubble = bubbleObj.GetComponent<SpeechBubble>();
        bubble.SetTarget(target);
        return bubble;
    }
}
