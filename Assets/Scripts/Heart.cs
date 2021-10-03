using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Image imageTakeDamage;
    private Color originColor;

    [SerializeField] private int hp = 5;
    [SerializeField] private List<HeartType> types;

    private SpriteRenderer spriteRenderer;

    private HeartType GetHeartTypeByValue(int value) => types.First(t => t.value == value);

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originColor = imageTakeDamage.color;
    }

    public void TakeDamage()
    {
        if (gameManager.State == GameState.GameOver) return;

        hp -= 1;
        hp = Mathf.Max(hp, 0);
        gameManager.SpawnBlocks(1);
        spriteRenderer.color = GetHeartTypeByValue(hp).color;
        StopCoroutine("TakeDamageEffect");
        StartCoroutine("TakeDamageEffect", 0.5f);

        if (hp == 0)
        {
            gameManager.ChangeState(GameState.GameOver);
        }
    }

    private IEnumerator TakeDamageEffect(float duration)
    {
        float percent = 0;
        float current = 0;
        imageTakeDamage.color = originColor;
        imageTakeDamage.gameObject.SetActive(true);
        while (current < 1)
        {
            percent += Time.deltaTime;
            current = percent / duration;
            imageTakeDamage.color = Color.Lerp(originColor, Color.clear, current);

            yield return null;
        }
        imageTakeDamage.gameObject.SetActive(false);
    }
}

[System.Serializable]
public struct HeartType
{
    public int value;
    public Color color;
}
