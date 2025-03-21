using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogView : MonoBehaviour
{
    public Text LogContent;

    public ScrollRect ScrollRect;
    // Start is called before the first frame update
    void Start()
    {
        Application.logMessageReceived += LogRecevied;
    }

    private void LogRecevied(string condition, string stackTrace, LogType type)
    {
        if (LogContent.text.Length>1000)
            LogContent.text = "";

        LogContent.text = LogContent.text + "\n"+condition;
        ScrollRect.verticalNormalizedPosition = 0;
    }
}
