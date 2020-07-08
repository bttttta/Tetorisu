using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextManager : MonoBehaviour
{
    public GameObject[] nextObjs;
    public PreliminalManager PreliminalManager;

    const int NEXT_NUM = 3;
    MinoType[] nextTypes;
    SpriteRenderer[] nextRenderers;

    Queue<MinoType> nextBugs;

    void Awake()
    {
        nextTypes = new MinoType[NEXT_NUM];
        nextRenderers = new SpriteRenderer[NEXT_NUM];
        nextBugs = new Queue<MinoType>();
        for(int i = 0; i < NEXT_NUM; ++i) {
            nextTypes[i] = RandomMino();
            nextRenderers[i] = nextObjs[i].GetComponent<SpriteRenderer>();
            nextRenderers[i].sprite = PreliminalManager.GetSprite(nextTypes[i]);
        }
    }

    public MinoType Get() {
        MinoType ret = nextTypes[0];

        nextTypes[0] = nextTypes[1];
        nextTypes[1] = nextTypes[2];
        nextTypes[2] = RandomMino();
        nextRenderers[0].sprite = nextRenderers[1].sprite;
        nextRenderers[1].sprite = nextRenderers[2].sprite;
        nextRenderers[2].sprite = PreliminalManager.GetSprite(nextTypes[2]);

        return ret;
    }

    // ランダム（ランダムとは言ってない）に次のミノを返す
    MinoType RandomMino() {
        if(nextBugs.Count == 0) {
            // ランダム列の生成
            int[] randomBug = new int[7];
            for(int i = 0; i < randomBug.Length; ++i) {
                randomBug[i] = i;
            }
            for(int i = 0; i < randomBug.Length; ++i) {
                int dest = i + (int)(Random.value * (7 - i));
                int buf = randomBug[i];
                randomBug[i] = randomBug[dest];
                randomBug[dest] = buf;
            }

            // nextBugsに突っ込む
            foreach(int item in randomBug) {
                nextBugs.Enqueue((MinoType)item);
            }
        }

        return nextBugs.Dequeue();
    }
}
