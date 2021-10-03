using UnityEngine;
using DG.Tweening;

public class Beat : MonoBehaviour
{
    private BeatManager beatManager;

    private bool isHitTheMark;
    private bool isMiss;

    public void Init(BeatManager _beatManager)
    {
        transform.DORewind();
        beatManager = _beatManager;
        isHitTheMark = false;
        isMiss = false;
    }

    private void Update()
    {
        if (isMiss == false && transform.position.x == 0)
        {
            isMiss = true;
            beatManager.Miss();
            Inactive();
        }
    }

    public bool Stop()
    {
        return isHitTheMark;
    }

    private void Inactive()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Heart"))
        {
            isHitTheMark = true;
        }
    }
}
