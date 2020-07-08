using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public ScoreManager LineManager;

    private int level = 0;
    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        level = LineManager.Get() / 10 + 1;
        text.text = level.ToString();
    }

    public int Get() {
        return level;
    }
}
