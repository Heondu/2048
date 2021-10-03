using UnityEngine;
using TMPro;
using DG.Tweening;

public class ScoreManager : MonoBehaviour
{
    [Header("Score UI")]
    [SerializeField] private TextMeshProUGUI currentScore;
    [SerializeField] private TextMeshProUGUI bestScore;
    [SerializeField] private TextMeshProUGUI textNewRecord;
    [SerializeField] private TextMeshProUGUI textScore;
    [SerializeField] private TextMeshProUGUI textCombo;

    private int score = 0;
    public int Score => score;
    private int combo = 0;

    public void Init()
    {
        currentScore.text = score.ToString();
        bestScore.text = PlayerPrefs.GetInt("BestScore").ToString();
    }

    public void IncreaseScore()
    {
        if (combo == 2) score += 1;
        else if (combo == 3) score += 2;
        else if (combo == 4) score += 3;
        else if (combo == 5) score += 4;
        else if (combo == 6) score += 5;
        else if (combo == 7) score += 6;
        else if (combo == 8) score += 8;
        else if (combo >= 9) score += 10;
    }

    public void DisplayScore()
    {
        currentScore.text = score.ToString();
        currentScore.DORewind();
        currentScore.rectTransform.localScale = Vector3.one;
        currentScore.rectTransform.DOScale(new Vector3(1.5f, 1.5f, 1), 0.2f).SetLoops(2, LoopType.Yoyo);
    }

    public void IncreaseCombo()
    {
        combo++;
        DisplayCombo();
        IncreaseScore();
    }

    private void DisplayCombo()
    {
        textCombo.text = combo.ToString();
        textCombo.DORewind();
        textCombo.rectTransform.localScale = Vector3.one;
        textCombo.rectTransform.DOScale(new Vector3(1.5f, 1.5f, 1), 0.2f).SetLoops(2, LoopType.Yoyo);
    }

    public void ResetCombo()
    {
        int prevCombo = combo;
        combo = 0;
        if (prevCombo != combo)
        {
            DisplayCombo();
        }
    }

    public void Show(int highestValue)
    {
        score += highestValue;
        if (score > PlayerPrefs.GetInt("BestScore"))
        {
            PlayerPrefs.SetInt("BestScore", score);
            textNewRecord.text = "New Record!";
        }
        else
        {
            textNewRecord.text = "Score";
        }

        textScore.text = score.ToString();
    }
}
