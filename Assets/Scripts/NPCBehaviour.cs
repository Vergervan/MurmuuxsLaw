using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NPCBehaviour : MonoBehaviour
{
    [Serializable]
    public class NPCSettings
    {
        public string currentFlag;
    }

    public NPCSettings npcSettings;
    [SerializeField] private DialogueManager dManager;
    [SerializeField] private PlayerController player;
    private SpeechBubble npcBubble;
    private bool isSpeak = false;
    private Coroutine hideBubble = null;
    void OnMouseDown()
    {
        if (Vector2.Distance(player.transform.position, transform.position) > 2f)
        {
            if (npcBubble != null) npcBubble.gameObject.SetActive(false);
            return;
        }
        if (hideBubble != null) StopCoroutine(hideBubble);
        if (!isSpeak)
        {
            if (npcBubble == null) npcBubble = dManager.CreateSpeechBubble(transform);
            npcBubble.gameObject.SetActive(true);
            npcBubble.SetFlagName(npcSettings.currentFlag);
            isSpeak = true;
            npcBubble.StartSpeech();
        }
        else
        {
            string nextText = npcBubble.GetTextFlag().FlagValue.Next();
            if(string.IsNullOrEmpty(nextText))
            {
                DisableBubble();
                npcBubble.GetTextFlag().FlagValue.SetToStart();
                isSpeak = false;
                return;
            }
            npcBubble.StartSpeech();
        }
    }

    private void Update()
    {
        if (npcBubble)
        {
            Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
            pos.y *= 1.25f;
            npcBubble.transform.position = pos;
        }
        if (isSpeak)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > 2f)
            {
                ResetSpeech();
            }
        }
    }
    public void ResetSpeech()
    {
        StartCoroutine(HideBubble());
        npcBubble.GetTextFlag().FlagValue.SetToStart();
        isSpeak = false;
    }
    private IEnumerator HideBubble()
    {
        yield return new WaitForSeconds(3f);
        DisableBubble();
    }

    private void DisableBubble()
    {
        npcBubble.gameObject.SetActive(false);
        hideBubble = null;
    }
}
