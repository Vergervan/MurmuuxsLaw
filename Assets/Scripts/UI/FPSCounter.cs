using System.Timers;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    private Timer timer;
    private int counter;
    private int prevCounter;
    [SerializeField] private int _fps;
    [SerializeField] private TMP_Text _text;
    private bool _changeText = false;

    private void Start()
    {
        timer = new Timer(500);
        timer.Elapsed += Timer_Elapsed;
        timer.Start();
    }

    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        _fps = counter + prevCounter;
        prevCounter = counter;
        counter = 0;
        _changeText = true;
    }

    private void Update()
    {
        if (_changeText)
        {
            _text.text = "FPS: " + _fps;
            _changeText = false;
        }
        counter++;
    }
}
