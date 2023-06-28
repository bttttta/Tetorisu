using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    public MinoController minoController;
    public FieldMinos fieldMinos;
    public MinoManager minoManager;
    public GameObject[] blocks; // ゴーストミノのスプライト

    Mino ghostMino; // ゴーストのミノ
    Mino currentMino; // 動かしている実体のミノ

    Transform fieldTransform;
    Transform[] blockTransforms;
    SpriteRenderer[] blockSpriteRenderers;

    const int MINO_SIZE = 54;

    // Start is called before the first frame update
    void Start()
    {
        fieldTransform = fieldMinos.GetComponent<Transform>();
        blockTransforms = new Transform[4];
        blockSpriteRenderers = new SpriteRenderer[4];
        for(int i = 0; i < blocks.Length; ++i) {
            blockTransforms[i] = blocks[i].GetComponent<Transform>();
            blockSpriteRenderers[i] = blocks[i].GetComponent<SpriteRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        ghostMino = new Mino(minoController.mino);

        // ゴーストの配置位置を検索
        while(fieldMinos.IsLocatable(ghostMino + Vector2Int.down)) {
            ghostMino += Vector2Int.down;
        }

        SetSprites();
    }

    void SetSprites() {
        Vector2Int[] minoPositions = ghostMino.AllPositions;
        for(int i = 0; i < blocks.Length; ++i) {
            blockTransforms[i].position = new Vector3(minoPositions[i].x * MINO_SIZE + fieldTransform.position.x, minoPositions[i].y * MINO_SIZE + fieldTransform.position.y, 0.5F);
            blockSpriteRenderers[i].sprite = minoManager.Sprites[(int)ghostMino.Type];
        }
    }
}
