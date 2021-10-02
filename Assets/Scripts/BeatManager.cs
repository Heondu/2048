using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    [SerializeField] private Transform startingPoint1;
    [SerializeField] private Transform startingPoint2;
    [SerializeField] private Beat beatPrefab;
    [SerializeField] private float speed = 1;
    [SerializeField] private MemoryPool memoryPool;
    [SerializeField] private GameManager gameManager;

    private Queue<Beat> beats;
    private int missCount = 0;

    private void Start()
    {
        memoryPool.Init(beatPrefab);

        beats = new Queue<Beat>();

        StartCoroutine("SpawnBeat");
    }

    public ScoreType InactivateBeat()
    {
        ScoreType type = ScoreType.NULL;
        for (int i = 0; i < 2; i++)
        {
            if (beats.Count > 0)
            {
                type = beats.Dequeue().Stop();
            }
        }
        return type;
    }

    public void Miss()
    {
        missCount++;
        if (missCount % 2 == 0)
        {
            missCount -= 2;
            gameManager.SpawnBlocks(1);
        }
    }

    private IEnumerator SpawnBeat()
    {
        while (true)
        {
            if (gameManager.State == GameState.Win || gameManager.State == GameState.Lose)
            {
                while (beats.Count > 0)
                {
                    beats.Dequeue().Stop();
                    yield return null;
                }

                yield break;
            }

            Beat beat = memoryPool.Get();
            beats.Enqueue(beat);
            beat.transform.position = startingPoint1.position;
            beat.Init(this, Vector2.right, speed);

            beat = memoryPool.Get();
            beats.Enqueue(beat);
            beat.transform.position = startingPoint2.position;
            beat.Init(this, Vector2.left, speed);

            yield return new WaitForSeconds(1 / speed);
        }
    }
}

public enum ScoreType
{
    perfect,
    great,
    good,
    bad,
    miss,
    NULL
}