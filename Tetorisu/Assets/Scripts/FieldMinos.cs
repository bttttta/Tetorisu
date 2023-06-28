using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldMinos : MonoBehaviour
{
    public MinoManager MinoManager;
    public GameObject FieldMinoPrefab;
    public ScoreManager ScoreManager, LineManager;
    public LevelManager LevelManager;

    GameObject fieldObjectNode;
    GameObject[,] fieldMinos;
    SpriteRenderer[,] fieldMinoSprites;
    bool[,] minoExists;

    const int HEIGHT = 30;
    const int WIDTH = 10;
    const int MINO_SIZE = 54;
    static readonly int[] SCORE_NORMAL = { 0, 100, 300, 500, 800 };

    // Start is called before the first frame update
    void Start()
    {
        fieldObjectNode = gameObject;
        fieldMinos = new GameObject[WIDTH, HEIGHT];
        fieldMinoSprites = new SpriteRenderer[WIDTH, HEIGHT];
        minoExists = new bool[WIDTH, HEIGHT];

        for(int i = 0; i < WIDTH; ++i) {
            for(int j = 0; j < HEIGHT; ++j) {
                fieldMinos[i, j] = Instantiate(FieldMinoPrefab, gameObject.transform);
                fieldMinos[i, j].transform.position = new Vector3(i * MINO_SIZE + gameObject.transform.position.x, j * MINO_SIZE + gameObject.transform.position.y, 0);
                fieldMinoSprites[i, j] = fieldMinos[i, j].GetComponent<SpriteRenderer>();
                minoExists[i, j] = false;
            }
        }
    }

    public void Add(Mino mino) {
        foreach(Vector2Int m in mino.AllPositions) {
            fieldMinoSprites[m.x, m.y].sprite = MinoManager.Sprites[(int)mino.Type];
            minoExists[m.x, m.y] = true;
        }

        int deleteLines = 0;
        for(int i = 0; i < HEIGHT; ++i) {
            bool lineDelete = true;
            for(int j = 0; j < WIDTH; ++j) {
                if(minoExists[j, i] == false) {
                    lineDelete = false;
                }
            }
            if(lineDelete) {
                for(int ii = i; ii < HEIGHT - 1; ++ii) {
                    for(int j = 0; j < WIDTH; ++j) {
                        fieldMinoSprites[j, ii].sprite = fieldMinoSprites[j, ii + 1].sprite;
                        minoExists[j, ii] = minoExists[j, ii + 1];
                    }
                }
                --i;
                ++deleteLines;
            }
        }
        LineManager.Add(deleteLines);
        ScoreManager.Add(SCORE_NORMAL[deleteLines] * LevelManager.Get());
    }

    public Mino Move(Mino source, Operate operate) {
        Mino dest = new Mino(source);
        Mino dest_srs;
        switch(operate) {
            case Operate.MoveRight:
                dest.Position.x += 1;
                if(!IsLocatable(dest)) {
                    return null;
                } else {
                    return dest;
                }
            case Operate.MoveLeft:
                dest.Position.x -= 1;
                if(!IsLocatable(dest)) {
                    return null;
                } else {
                    return dest;
                }
            case Operate.SoftDrop:
                dest.Position.y -= 1;
                if(!IsLocatable(dest)) {
                    return null;
                } else {
                    return dest;
                }
            case Operate.HardDrop:
                do {
                    dest.Position.y -= 1;
                    ScoreManager.Add(1);
                } while(IsLocatable(dest));
                dest.Position.y += 1;
                ScoreManager.Add(-1);
                return dest;
            case Operate.RotateRight:
                // 通常回転
                dest.Rotate++;
                dest.Rotate %= 4;
                if(IsLocatable(dest)) {
                    return dest;
                }
                // SRS回転
                dest_srs = new Mino(dest);
                if(dest.Type == MinoType.I) {
                    // SRS-I1
                    switch(dest.Rotate) {
                        case 0:
                            dest_srs.Position.x -= 2;
                            break;
                        case 1:
                            dest_srs.Position.x -= 2;
                            break;
                        case 2:
                            dest_srs.Position.x -= 1;
                            break;
                        case 3:
                            dest_srs.Position.x += 2;
                            break;
                    }
                    if(IsLocatable(dest_srs)) {
                        return dest_srs;
                    }
                    // SRS-I2
                    dest_srs.Position = dest.Position;
                    switch(dest.Rotate) {
                        case 0:
                            dest_srs.Position.x += 1;
                            break;
                        case 1:
                            dest_srs.Position.x += 1;
                            break;
                        case 2:
                            dest_srs.Position.x += 2;
                            break;
                        case 3:
                            dest_srs.Position.x -= 1;
                            break;
                    }
                    if(IsLocatable(dest_srs)) {
                        return dest_srs;
                    }
                    // SRS-I3
                    dest_srs.Position = dest.Position;
                    switch(dest.Rotate) {
                        case 0:
                            dest_srs.Position.x += 1;
                            dest_srs.Position.y -= 2;
                            break;
                        case 1:
                            dest_srs.Position.x -= 2;
                            dest_srs.Position.y -= 1;
                            break;
                        case 2:
                            dest_srs.Position.x -= 1;
                            dest_srs.Position.y += 2;
                            break;
                        case 3:
                            dest_srs.Position.x += 2;
                            dest_srs.Position.y += 1;
                            break;
                    }
                    if(IsLocatable(dest_srs)) {
                        return dest_srs;
                    }
                    // SRS-I4
                    dest_srs.Position = dest.Position;
                    switch(dest.Rotate) {
                        case 0:
                            dest_srs.Position.x -= 2;
                            dest_srs.Position.y += 1;
                            break;
                        case 1:
                            dest_srs.Position.x += 1;
                            dest_srs.Position.y += 2;
                            break;
                        case 2:
                            dest_srs.Position.x += 2;
                            dest_srs.Position.y -= 1;
                            break;
                        case 3:
                            dest_srs.Position.x -= 1;
                            dest_srs.Position.y -= 2;
                            break;
                    }
                    if(IsLocatable(dest_srs)) {
                        return dest_srs;
                    }
                } else {
                    // SRS-T1
                    switch(dest.Rotate) {
                        case 0:
                            dest_srs.Position.x -= 1;
                            break;
                        case 1:
                            dest_srs.Position.x -= 1;
                            break;
                        case 2:
                            dest_srs.Position.x += 1;
                            break;
                        case 3:
                            dest_srs.Position.x += 1;
                            break;
                    }
                    if(IsLocatable(dest_srs)) {
                        return dest_srs;
                    }
                    // SRS-T2
                    dest_srs.Position = dest.Position;
                    switch(dest.Rotate) {
                        case 0:
                            dest_srs.Position.x -= 1;
                            dest_srs.Position.y -= 1;
                            break;
                        case 1:
                            dest_srs.Position.x -= 1;
                            dest_srs.Position.y += 1;
                            break;
                        case 2:
                            dest_srs.Position.x += 1;
                            dest_srs.Position.y -= 1;
                            break;
                        case 3:
                            dest_srs.Position.x += 1;
                            dest_srs.Position.y += 1;
                            break;
                    }
                    if(IsLocatable(dest_srs)) {
                        return dest_srs;
                    }
                    // SRS-T3
                    dest_srs.Position = dest.Position;
                    switch(dest.Rotate) {
                        case 0:
                            dest_srs.Position.y += 2;
                            break;
                        case 1:
                            dest_srs.Position.y -= 2;
                            break;
                        case 2:
                            dest_srs.Position.y += 2;
                            break;
                        case 3:
                            dest_srs.Position.y -= 2;
                            break;
                    }
                    if(IsLocatable(dest_srs)) {
                        return dest_srs;
                    }
                    // SRS-T4
                    dest_srs.Position = dest.Position;
                    switch(dest.Rotate) {
                        case 0:
                            dest_srs.Position.x -= 1;
                            dest_srs.Position.y += 2;
                            break;
                        case 1:
                            dest_srs.Position.x -= 1;
                            dest_srs.Position.y -= 2;
                            break;
                        case 2:
                            dest_srs.Position.x += 1;
                            dest_srs.Position.y += 2;
                            break;
                        case 3:
                            dest_srs.Position.x += 1;
                            dest_srs.Position.y -= 2;
                            break;
                    }
                    if(IsLocatable(dest_srs)) {
                        return dest_srs;
                    }
                }
                return null;
            case Operate.RotateLeft:
                // 通常回転
                dest.Rotate += 3;
                dest.Rotate %= 4;
                if(IsLocatable(dest)) {
                    return dest;
                }
                // SRS回転
                dest_srs = new Mino(dest);
                if(dest.Type == MinoType.I) {
                    // SRS-I1
                    switch(dest.Rotate) {
                        case 0:
                            dest_srs.Position.x += 2;
                            break;
                        case 1:
                            dest_srs.Position.x += 1;
                            break;
                        case 2:
                            dest_srs.Position.x += 1;
                            break;
                        case 3:
                            dest_srs.Position.x -= 1;
                            break;
                    }
                    if(IsLocatable(dest_srs)) {
                        return dest_srs;
                    }
                    // SRS-I2
                    dest_srs.Position = dest.Position;
                    switch(dest.Rotate) {
                        case 0:
                            dest_srs.Position.x -= 1;
                            break;
                        case 1:
                            dest_srs.Position.x -= 2;
                            break;
                        case 2:
                            dest_srs.Position.x -= 2;
                            break;
                        case 3:
                            dest_srs.Position.x += 2;
                            break;
                    }
                    if(IsLocatable(dest_srs)) {
                        return dest_srs;
                    }
                    // SRS-I3
                    dest_srs.Position = dest.Position;
                    switch(dest.Rotate) {
                        case 0:
                            dest_srs.Position.x += 2;
                            dest_srs.Position.y += 1;
                            break;
                        case 1:
                            dest_srs.Position.x += 2;
                            dest_srs.Position.y -= 2;
                            break;
                        case 2:
                            dest_srs.Position.x -= 2;
                            dest_srs.Position.y -= 2;
                            break;
                        case 3:
                            dest_srs.Position.x -= 2;
                            dest_srs.Position.y += 2;
                            break;
                    }
                    if(IsLocatable(dest_srs)) {
                        return dest_srs;
                    }
                    // SRS-I4
                    dest_srs.Position = dest.Position;
                    switch(dest.Rotate) {
                        case 0:
                            dest_srs.Position.x -= 1;
                            dest_srs.Position.y -= 2;
                            break;
                        case 1:
                            dest_srs.Position.x -= 2;
                            dest_srs.Position.y += 1;
                            break;
                        case 2:
                            dest_srs.Position.x += 1;
                            dest_srs.Position.y += 2;
                            break;
                        case 3:
                            dest_srs.Position.x += 2;
                            dest_srs.Position.y -= 1;
                            break;
                    }
                    if(IsLocatable(dest_srs)) {
                        return dest_srs;
                    }
                } else {
                    // SRS-T1
                    switch(dest.Rotate) {
                        case 0:
                            dest_srs.Position.x += 1;
                            break;
                        case 1:
                            dest_srs.Position.x -= 1;
                            break;
                        case 2:
                            dest_srs.Position.x -= 1;
                            break;
                        case 3:
                            dest_srs.Position.x += 1;
                            break;
                    }
                    if(IsLocatable(dest_srs)) {
                        return dest_srs;
                    }
                    // SRS-T2
                    dest_srs.Position = dest.Position;
                    switch(dest.Rotate) {
                        case 0:
                            dest_srs.Position.x += 1;
                            dest_srs.Position.y -= 1;
                            break;
                        case 1:
                            dest_srs.Position.x -= 1;
                            dest_srs.Position.y += 1;
                            break;
                        case 2:
                            dest_srs.Position.x -= 1;
                            dest_srs.Position.y -= 1;
                            break;
                        case 3:
                            dest_srs.Position.x += 1;
                            dest_srs.Position.y += 1;
                            break;
                    }
                    if(IsLocatable(dest_srs)) {
                        return dest_srs;
                    }
                    // SRS-T3
                    dest_srs.Position = dest.Position;
                    switch(dest.Rotate) {
                        case 0:
                            dest_srs.Position.y += 2;
                            break;
                        case 1:
                            dest_srs.Position.y -= 2;
                            break;
                        case 2:
                            dest_srs.Position.y += 2;
                            break;
                        case 3:
                            dest_srs.Position.y -= 2;
                            break;
                    }
                    if(IsLocatable(dest_srs)) {
                        return dest_srs;
                    }
                    // SRS-T4
                    dest_srs.Position = dest.Position;
                    switch(dest.Rotate) {
                        case 0:
                            dest_srs.Position.x += 1;
                            dest_srs.Position.y += 2;
                            break;
                        case 1:
                            dest_srs.Position.x -= 1;
                            dest_srs.Position.y -= 2;
                            break;
                        case 2:
                            dest_srs.Position.x -= 1;
                            dest_srs.Position.y += 2;
                            break;
                        case 3:
                            dest_srs.Position.x += 1;
                            dest_srs.Position.y -= 2;
                            break;
                    }
                    if(IsLocatable(dest_srs)) {
                        return dest_srs;
                    }
                }
                return null;
            default:
                return null;
        }
    }

    /// <summary>
    /// minoがその位置に配置できるか
    /// </summary>
    /// <param name="mino">調べたいテトリミノ</param>
    /// <returns>配置可能か</returns>
    public bool IsLocatable(Mino mino) {
        foreach(var m in mino.AllPositions) {
            if(m.x < 0 || m.x >= WIDTH) {
                return false;
            }
            if(m.y < 0 || m.y >= HEIGHT) {
                return false;
            }
            if(minoExists[m.x, m.y]) {
                return false;
            }
        }
        return true;
    }
}
