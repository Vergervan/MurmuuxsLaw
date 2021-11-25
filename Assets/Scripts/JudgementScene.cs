using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgementScene : MonoBehaviour
{
    [SerializeField] private SceneManager sceneManager;
    [SerializeField] private DialogueManager dManager;
    [SerializeField] private SpeechBubble satanBubble;

    public void StartJudgeSpeech()
    {
        if(!satanBubble) satanBubble = dManager.CreateSpeechBubble(transform);
        dManager.gameObject.SetActive(true);
        satanBubble.gameObject.SetActive(true);
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        pos.x += 50f;
        pos.y += 15f;
        satanBubble.transform.position = pos;
        satanBubble.SetFlagName("satanJudge");
        StartCoroutine(JudgeSpeech());
    }
    private IEnumerator JudgeSpeech()
    {
        if (satanBubble.GetTextFlag().HasValue)
        {
            GetComponent<AudioSource>().mute = false;
            
            do
            {
                satanBubble.StartSpeech();
                yield return new WaitForSeconds(satanBubble.GetTextFlag().FlagValue.CurrentText().Length * 0.05f + 1.7f);
                if (!satanBubble.GetTextFlag().FlagValue.HasNext()) break;
                satanBubble.GetTextFlag().FlagValue.Next();
            } while (true);
            dManager.gameObject.SetActive(false);
            GetComponent<AudioSource>().mute = true;
        }
        GetComponent<Animator>().SetBool("start", true);
    }
    private IEnumerator Timeout(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        sceneManager.SetScene("city");
    }
}
