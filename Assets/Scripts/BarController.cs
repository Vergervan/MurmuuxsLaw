using UnityEngine;

public class BarController : MonoBehaviour
{
    public GameObject journalObject;

    private Journal journal;
    private bool _isJournalOpened = false;
    void Start()
    {
        journal = new Journal();
        journalObject.SetActive(false);
    }

    public void ToggleVisible()
    {
        _isJournalOpened = !_isJournalOpened;
        journalObject.SetActive(_isJournalOpened);
    }
}
