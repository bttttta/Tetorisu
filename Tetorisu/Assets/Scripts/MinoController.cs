using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoController : MonoBehaviour
{
    public NextManager NextManager;
    public MinoManager MinoManager;
    public HoldManager HoldManager;
    public LevelManager LevelManager;
    public ScoreManager ScoreManager;
    public FieldMinos FieldMinos;
    public GameObject[] blocks;

    public Mino mino;
    Transform fieldTransform;
    Transform[] blockTransforms;
    SpriteRenderer[] blockSpriteRenderers;
    float fallTime = 0F;

    const int MINO_SIZE = 54;
    float leftTime = 0F, rightTime = 0F, dropTime = 0F;
    const float keyFirstTime = 0.2F, keySecondTime = 0.03F;

    // Start is called before the first frame update
    void Start()
    {
        mino = new Mino(NextManager.Get());
        fieldTransform = FieldMinos.GetComponent<Transform>();
        blockTransforms = new Transform[4];
        blockSpriteRenderers = new SpriteRenderer[4];
        for(int i = 0; i < blocks.Length; ++i) {
            blockTransforms[i] = blocks[i].GetComponent<Transform>();
            blockSpriteRenderers[i] = blocks[i].GetComponent<SpriteRenderer>();
        }
        SetSprites();
    }

    // Update is called once per frame
    void Update()
    {
        Operate? operate = GetOperate();
        int freeFall = 0;
        Mino dest;

        if(operate == Operate.Hold) {
            MinoType minoType = HoldManager.Change(mino.Type);
            if(minoType == MinoType.None) {
                mino = new Mino(NextManager.Get());
            } else {
                mino = new Mino(minoType);
            }
        } else if(operate != null) {
            dest = FieldMinos.Move(mino, operate.Value);
            if(dest != null) {
                mino = dest;
                if(operate == Operate.SoftDrop) {
                    ScoreManager.Add(1);
                }
            }
            if(operate == Operate.SoftDrop && dest == null || operate == Operate.HardDrop) {
                FieldMinos.Add(mino);
                mino = new Mino(NextManager.Get());
            }

        }

        if(operate == null || operate == Operate.MoveLeft || operate == Operate.MoveRight) {
            fallTime += Time.deltaTime;
            freeFall = FreeFall();
            for(int i = 0; i < freeFall; ++i) {
                dest = FieldMinos.Move(mino, Operate.SoftDrop);
                if(dest != null) {
                    mino = dest;
                } else {
                    FieldMinos.Add(mino);
                    mino = new Mino(NextManager.Get());
                }
            }
        } else {
            fallTime = 0F;
        }

        SetSprites();
    }

    void SetSprites() {
        Vector2Int[] minoPositions = mino.AllPositions;
        for(int i = 0; i < blocks.Length; ++i) {
            blockTransforms[i].position = new Vector3(minoPositions[i].x * MINO_SIZE + fieldTransform.position.x, minoPositions[i].y * MINO_SIZE + fieldTransform.position.y, 0);
            blockSpriteRenderers[i].sprite = MinoManager.Sprites[(int)mino.Type];
        }
    }

    int FreeFall() {
        int level = LevelManager.Get();
        int ret = 0;

        if(FieldMinos.Move(mino, Operate.SoftDrop) == null) {
            if(fallTime > 0.5F) {
                ret = 1;
            } else {
                ret = 0;
            }
        } else {
            if(level < 10) {
                if(fallTime * level > 1F) {
                    ret = 1;
                } else {
                    ret = 0;
                }
            } else {
                if(level < 12) {
                    ret = 1;
                } else if(level == 12) {
                    ret = 2;
                } else if(level == 13) {
                    ret = 4;
                } else if(level == 14) {
                    ret = 5;
                } else {
                    ret = 20;
                }
            }
        }

        if(ret > 0) {
            fallTime = 0F;
        }
        return ret;
    }

    Operate? GetOperate() {
        if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            dropTime += Time.deltaTime;
        } else {
            dropTime = 0F;
        }
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            rightTime += Time.deltaTime;
        } else {
            rightTime = 0F;
        }
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            leftTime += Time.deltaTime;
        } else {
            leftTime = 0F;
        }

        if(Input.GetKeyDown(KeyCode.Space)) {
            return Operate.Hold;
        }
        if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
            return Operate.HardDrop;
        }
        if(Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.L)) {
            return Operate.RotateRight;
        }
        if(Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.K)) {
            return Operate.RotateLeft;
        }
        if(rightTime == Time.deltaTime || rightTime >= keyFirstTime && (int)(rightTime / keySecondTime) > (int)((rightTime - Time.deltaTime) / keySecondTime)) {
            return Operate.MoveRight;
        }
        if(leftTime == Time.deltaTime || leftTime >= keyFirstTime && (int)(leftTime / keySecondTime) > (int)((leftTime - Time.deltaTime) / keySecondTime)) {
            return Operate.MoveLeft;
        }
        if(dropTime > 0 && (int)(dropTime / keySecondTime) > (int)((dropTime - Time.deltaTime) / keySecondTime)) {
            return Operate.SoftDrop;
        }
        return null;
    }
}

public class Mino {
    public MinoType Type;
    public Vector2Int Position;
    public byte Rotate;

    public Vector2Int[] AllPositions {
        get {
            int px = Position.x, py = Position.y;
            switch(Type) {
                case MinoType.I:
                    switch(Rotate) {
                        case 0:
                            return new Vector2Int[] {
                                new Vector2Int { x = px, y = py },
                                new Vector2Int { x = px - 1, y = py },
                                new Vector2Int { x = px + 1, y = py },
                                new Vector2Int { x = px + 2, y = py },
                            };
                        case 1:
                            return new Vector2Int[] {
                                new Vector2Int { x = px + 1, y = py + 1 },
                                new Vector2Int { x = px + 1, y = py },
                                new Vector2Int { x = px + 1, y = py - 1 },
                                new Vector2Int { x = px + 1, y = py - 2 },
                            };
                        case 2:
                            return new Vector2Int[] {
                                new Vector2Int { x = px + 1, y = py - 1 },
                                new Vector2Int { x = px + 2, y = py - 1 },
                                new Vector2Int { x = px, y = py - 1 },
                                new Vector2Int { x = px - 1, y = py - 1 },
                            };
                        default:
                            return new Vector2Int[] {
                                new Vector2Int { x = px, y = py - 1 },
                                new Vector2Int { x = px, y = py - 2 },
                                new Vector2Int { x = px, y = py },
                                new Vector2Int { x = px, y = py + 1 },
                            };
                    }
                case MinoType.J:
                    switch(Rotate) {
                        case 0:
                            return new Vector2Int[] {
                                new Vector2Int { x = px, y = py },
                                new Vector2Int { x = px + 1, y = py },
                                new Vector2Int { x = px - 1, y = py },
                                new Vector2Int { x = px - 1, y = py + 1 },
                            };
                        case 1:
                            return new Vector2Int[] {
                                new Vector2Int { x = px, y = py },
                                new Vector2Int { x = px, y = py - 1 },
                                new Vector2Int { x = px, y = py + 1 },
                                new Vector2Int { x = px + 1, y = py + 1 },
                            };
                        case 2:
                            return new Vector2Int[] {
                                new Vector2Int { x = px, y = py },
                                new Vector2Int { x = px - 1, y = py },
                                new Vector2Int { x = px + 1, y = py },
                                new Vector2Int { x = px + 1, y = py - 1 },
                            };
                        default:
                            return new Vector2Int[] {
                                new Vector2Int { x = px, y = py },
                                new Vector2Int { x = px, y = py + 1 },
                                new Vector2Int { x = px, y = py - 1 },
                                new Vector2Int { x = px - 1, y = py - 1 },
                            };
                    }
                case MinoType.L:
                    switch(Rotate) {
                        case 0:
                            return new Vector2Int[] {
                                new Vector2Int { x = px, y = py },
                                new Vector2Int { x = px - 1, y = py },
                                new Vector2Int { x = px + 1, y = py },
                                new Vector2Int { x = px + 1, y = py + 1 },
                            };
                        case 1:
                            return new Vector2Int[] {
                                new Vector2Int { x = px, y = py },
                                new Vector2Int { x = px, y = py + 1 },
                                new Vector2Int { x = px, y = py - 1 },
                                new Vector2Int { x = px + 1, y = py - 1 },
                            };
                        case 2:
                            return new Vector2Int[] {
                                new Vector2Int { x = px, y = py },
                                new Vector2Int { x = px + 1, y = py },
                                new Vector2Int { x = px - 1, y = py },
                                new Vector2Int { x = px - 1, y = py - 1 },
                            };
                        default:
                            return new Vector2Int[] {
                                new Vector2Int { x = px, y = py },
                                new Vector2Int { x = px, y = py - 1 },
                                new Vector2Int { x = px, y = py + 1 },
                                new Vector2Int { x = px - 1, y = py + 1 },
                            };
                    }
                case MinoType.O:
                    return new Vector2Int[] {
                        new Vector2Int { x = px, y = py },
                        new Vector2Int { x = px + 1, y = py },
                        new Vector2Int { x = px, y = py - 1 },
                        new Vector2Int { x = px + 1, y = py - 1 },
                    };
                case MinoType.S:
                    switch(Rotate) {
                        case 0:
                            return new Vector2Int[] {
                                new Vector2Int { x = px, y = py },
                                new Vector2Int { x = px - 1, y = py },
                                new Vector2Int { x = px, y = py + 1 },
                                new Vector2Int { x = px + 1, y = py + 1 },
                            };
                        case 1:
                            return new Vector2Int[] {
                                new Vector2Int { x = px, y = py },
                                new Vector2Int { x = px, y = py + 1 },
                                new Vector2Int { x = px + 1, y = py },
                                new Vector2Int { x = px + 1, y = py - 1 },
                            };
                        case 2:
                            return new Vector2Int[] {
                                new Vector2Int { x = px, y = py },
                                new Vector2Int { x = px + 1, y = py },
                                new Vector2Int { x = px, y = py - 1 },
                                new Vector2Int { x = px - 1, y = py - 1 },
                            };
                        default:
                            return new Vector2Int[] {
                                new Vector2Int { x = px, y = py },
                                new Vector2Int { x = px, y = py - 1 },
                                new Vector2Int { x = px - 1, y = py },
                                new Vector2Int { x = px - 1, y = py + 1 },
                            };
                    }
                case MinoType.T:
                    switch(Rotate) {
                        case 0:
                            return new Vector2Int[] {
                                new Vector2Int { x = px, y = py },
                                new Vector2Int { x = px - 1, y = py },
                                new Vector2Int { x = px, y = py + 1 },
                                new Vector2Int { x = px + 1, y = py },
                            };
                        case 1:
                            return new Vector2Int[] {
                                new Vector2Int { x = px, y = py },
                                new Vector2Int { x = px, y = py + 1 },
                                new Vector2Int { x = px + 1, y = py },
                                new Vector2Int { x = px, y = py - 1 },
                            };
                        case 2:
                            return new Vector2Int[] {
                                new Vector2Int { x = px, y = py },
                                new Vector2Int { x = px + 1, y = py },
                                new Vector2Int { x = px, y = py - 1 },
                                new Vector2Int { x = px - 1, y = py },
                            };
                        default:
                            return new Vector2Int[] {
                                new Vector2Int { x = px, y = py },
                                new Vector2Int { x = px, y = py - 1 },
                                new Vector2Int { x = px - 1, y = py },
                                new Vector2Int { x = px, y = py + 1 },
                            };
                    }
                case MinoType.Z:
                    switch(Rotate) {
                        case 0:
                            return new Vector2Int[] {
                                new Vector2Int { x = px, y = py },
                                new Vector2Int { x = px + 1, y = py },
                                new Vector2Int { x = px , y = py + 1 },
                                new Vector2Int { x = px - 1, y = py + 1 },
                            };
                        case 1:
                            return new Vector2Int[] {
                                new Vector2Int { x = px, y = py },
                                new Vector2Int { x = px, y = py - 1 },
                                new Vector2Int { x = px + 1, y = py },
                                new Vector2Int { x = px + 1, y = py + 1 },
                            };
                        case 2:
                            return new Vector2Int[] {
                                new Vector2Int { x = px, y = py },
                                new Vector2Int { x = px - 1, y = py },
                                new Vector2Int { x = px, y = py - 1 },
                                new Vector2Int { x = px + 1, y = py - 1 },
                            };
                        default:
                            return new Vector2Int[] {
                                new Vector2Int { x = px, y = py },
                                new Vector2Int { x = px, y = py + 1 },
                                new Vector2Int { x = px - 1, y = py },
                                new Vector2Int { x = px - 1, y = py - 1 },
                            };
                    }
                default:
                    return null;
            }
        }
    }

    public static Mino operator+ (Mino source, Vector2Int diff) {
        Mino ret = new Mino(source);
        ret.Position += diff;
        return ret;
    }

    public override string ToString() {
        string ret;
        ret = "[Mino] MinoType: " + Type.ToString();
        ret += "Positions: ";
        foreach(var position in this.AllPositions) {
            ret += position.ToString();
        }
        return ret;
    }

    public Mino() { }
    public Mino(MinoType type) {
        Type = type;
        Position = new Vector2Int(4, type == MinoType.O ? 21 : 20);
        Rotate = 0;
    }
    public Mino(Mino source) {
        Type = source.Type;
        Position = source.Position;
        Rotate = source.Rotate;
    }
}

public enum Operate {
    MoveRight,
    MoveLeft,
    SoftDrop,
    HardDrop,
    RotateRight,
    RotateLeft,
    Hold
}
