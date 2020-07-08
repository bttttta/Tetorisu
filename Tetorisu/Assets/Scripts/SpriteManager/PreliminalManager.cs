using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreliminalManager : MonoBehaviour
{
    public Sprite[] Sprites;

    public Sprite GetSprite(MinoType minoType) {
        return Sprites[(int)minoType];
    }
}
