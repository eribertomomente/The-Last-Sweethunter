using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugWindow : Singleton<DebugWindow>
{
    public float defaultTime = 3f;

    private Text text;
    public CanvasGroup cg;

    private void Awake()
    {
        text = GetComponentInChildren<Text>();
        CanvasGroup cg = GetComponent<CanvasGroup>();

        text.text = "";
        //cg.alpha = 0f;
    }

    public void Log(string message, float seconds = 0f)
    {
        StartCoroutine(WaitCoroutine(message, seconds));
    }

    private IEnumerator WaitCoroutine(string message, float seconds)
    {
        cg.alpha = 1;

        if (seconds <= 0f)
            seconds = defaultTime;


        text.text += message+"\n";

        yield return new WaitForSeconds(seconds);

        text.text = "";

        cg.alpha = 0;

    }
}
