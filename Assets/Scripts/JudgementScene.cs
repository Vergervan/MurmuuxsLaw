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
        satanBubble = dManager.CreateSpeechBubble(transform);
        satanBubble.gameObject.SetActive(true);
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        pos.x += 180f;
        pos.y += 60f;
        satanBubble.transform.position = pos;
        satanBubble.SetFlagName("satanJudge");
        StartCoroutine(JudgeSpeech());
    }
    private IEnumerator JudgeSpeech()
    {
        if (satanBubble.GetTextFlag().HasValue)
        {
            GetComponent<AudioSource>().enabled = true;
            
            do
            {
                satanBubble.StartSpeech();
                yield return new WaitForSeconds(satanBubble.GetTextFlag().FlagValue.CurrentText().Length * 0.05f + 1f);
                if (!satanBubble.GetTextFlag().FlagValue.HasNext()) break;
                satanBubble.GetTextFlag().FlagValue.Next();
            } while (true);
            dManager.gameObject.SetActive(false);
            GetComponent<AudioSource>().enabled = false;
        }
        GetComponent<Animator>().SetBool("start", true);
    }
    private IEnumerator Timeout(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        sceneManager.SetScene("city");
    }
}
