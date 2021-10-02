using UnityEngine;

public class Beat : MonoBehaviour
{
    [SerializeField] private float waitingTime = 0.5f;

    private BeatManager beatManager;
    private Vector2 dir;
    private float speed;
    private Vector2 originPos;
    private bool canMove;

    public void Init(BeatManager beatManager, Vector2 dir, float speed)
    {
        this.beatManager = beatManager;
        this.dir = dir;
        this.speed = speed;
        originPos = transform.position;
        canMove = true;
    }

    private void Update()
    {
        if (canMove)
        {
            transform.Translate(dir * speed * Time.deltaTime);

            if (Vector2.Distance(originPos, transform.position) > Mathf.Abs(originPos.x))
            {
                beatManager.Miss();
                Invoke("Inactive", waitingTime);
                canMove = false;
            }
        }
    }

    public ScoreType Stop()
    {
        dir = Vector2.zero;
        Invoke("Inactive", waitingTime);

        float distance = Vector2.Distance(transform.position, new Vector2(0, transform.position.y));
        if (distance < 0.1f) return ScoreType.perfect;
        else if (distance < 0.3f) return ScoreType.great;
        else if (distance < 0.6f) return ScoreType.good;
        else if (distance < 1f) return ScoreType.bad;
        else return ScoreType.miss;
    }

    private void Inactive()
    {
        gameObject.SetActive(false);
    }
}
