using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("Score UI")]
    [SerializeField] private TextMeshProUGUI currentScore;
    [SerializeField] private TextMeshProUGUI bestScore;

    private int score = 0;

    public void Init()
    {
        currentScore.text = score.ToString();
        bestScore.text = PlayerPrefs.GetInt("BestScore").ToString();
    }

    public void Add(int value)
    {
        score += value;
        currentScore.text = score.ToString();
    }

    public void Save()
    {
        if (score > PlayerPrefs.GetInt("BestScore"))
        {
            PlayerPrefs.SetInt("BestScore", score);
        }
    }
}
