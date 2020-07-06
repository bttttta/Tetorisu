using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    private float time;
    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        time = 0F;
        text = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        text.text = Time2String(time);
    }

    static string Time2String(float time) {
        int hour, min, sec, msec;
        int iTime = (int)time;

        hour = iTime / 3600;
        min = (iTime % 3600) / 60;
        sec = iTime % 60;
        msec = (int)((time - iTime) * 1000);

        return $"{hour:00}:{min:00}:{sec:00}.{msec:000}";
    }
}
