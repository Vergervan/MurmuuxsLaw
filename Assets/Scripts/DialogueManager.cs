using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public SpeechBubble CreateSpeechBubble(Transform target)
    {
        GameObject bubbleObj = (GameObject)Instantiate(Resources.Load("Speech Bubble"), transform);
        SpeechBubble bubble = bubbleObj.GetComponent<SpeechBubble>();
        bubble.SetTarget(target);
        return bubble;
    }
}
