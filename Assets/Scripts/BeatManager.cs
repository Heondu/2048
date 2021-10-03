using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class BeatManager : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private Transform startingPoint1;
    [SerializeField] private Transform startingPoint2;
    [SerializeField] private Beat beatPrefab;

    [Header("Component")]
    [SerializeField] private MemoryPool memoryPool;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Heart heart;
    [SerializeField] private Animator animator;
    [SerializeField] private ScoreManager scoreManager;

    [Header("Settings")]
    [SerializeField] float musicBPM = 60f;
    [SerializeField] float stdBPM = 60f;
    [SerializeField] int musicTempo = 4;
    [SerializeField] int stdTempo = 4;

    [Header("Audio")]
    [SerializeField] private AudioClip metronome;
    [SerializeField] private bool playMetronome = false;
    [SerializeField] private AudioClip[] beats;

    private Queue<Beat> beatBars;
    private int missCount = 0;
    private AudioClip currentBeat;

    private void Start()
    {
        memoryPool.Init(beatPrefab);

        beatBars = new Queue<Beat>();

        currentBeat = beats[BeatSelector.BeatNumber];

        StartCoroutine("SpawnBeat");
    }

    public bool InactivateBeat()
    {
        bool isHitTheMark = false;
        for (int i = 0; i < 2; i++)
        {
            if (beatBars.Count > 0)
            {
                if (beatBars.Dequeue().Stop())
                {
                    isHitTheMark = true;
                }
            }
        }
        if (isHitTheMark)
        {
            animator.SetTrigger("hitTheMark");
            missCount = 0;
        }
        return isHitTheMark;
    }

    public void Miss()
    {
        missCount++;
        if (missCount % 4 == 0)
        {
            missCount = 0;
            scoreManager.ResetCombo();
        }
        //if (missCount % 2 == 0)
        //{
        //    missCount = 0;
        //    TakeDamage();
        //}
    }

    public void TakeDamage()
    {
        heart.TakeDamage();
        if (gameManager.State == GameState.GameOver)
        {
            animator.SetTrigger("gameOver");
        }
        else
        {
            animator.SetTrigger("takeDamage");
        }
    }

    private IEnumerator SpawnBeat()
    {
        SoundManager.Instance.Play(currentBeat, SoundType.BGM);

        while (true)
        {
            if (gameManager.State == GameState.GameOver)
            {            
                yield break;
            }

            float tick = (stdBPM / musicBPM) * (musicTempo / stdTempo);

            Beat beat = memoryPool.Get();
            beat.Init(this);
            beatBars.Enqueue(beat);
            beat.transform.position = startingPoint1.position;
            beat.transform.DOMoveX(0f, tick+0.3f).SetEase(Ease.Unset);
            
            beat = memoryPool.Get();
            beat.Init(this);
            beatBars.Enqueue(beat);
            beat.transform.position = startingPoint2.position;
            beat.transform.DOMoveX(0, tick+0.3f).SetEase(Ease.Unset);
            

            yield return new WaitForSeconds(tick);

            if (gameManager.State != GameState.GameOver)
            {
                heart.transform.DOScale(new Vector3(1.1f, 1.1f, 1), 0.2f).SetLoops(2, LoopType.Yoyo);
                if (playMetronome)
                {
                    SoundManager.Instance.Play(metronome, SoundType.SE);
                }
            }
            else
            {
                SoundManager.Instance.Stop(true);
            }
        }
    }
}