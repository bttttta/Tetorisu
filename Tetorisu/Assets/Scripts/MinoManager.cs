using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoManager : MonoBehaviour
{
    public Sprite[] Sprites;

    public Sprite GetSprite(MinoType minoType) {
        return Sprites[(int)minoType];
    }

}
