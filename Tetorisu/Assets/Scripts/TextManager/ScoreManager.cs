using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private int score = 0;
    private Text text;

    // Start is called before the first frame update
    void Start() {
        text = gameObject.GetComponent<Text>();
    }

    public void Add(int adder) {
        score += adder;
        text.text = score.ToString();
    }

    public int Get() {
        return score;
    }
}
