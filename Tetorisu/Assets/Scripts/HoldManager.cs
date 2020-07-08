using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldManager : MonoBehaviour
{
    public PreliminalManager PreliminalManager;

    MinoType minoType = MinoType.None;
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public MinoType Change(MinoType newMinoType) {
        MinoType ret = minoType;
        minoType = newMinoType;
        spriteRenderer.sprite = PreliminalManager.Sprites[(int)newMinoType];
        return ret;
    }
}
