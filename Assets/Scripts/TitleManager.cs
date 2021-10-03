using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textStart;
    [SerializeField] private GameObject beatSelectScreen;

    private void Start()
    {
#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)
        textStart.text = "Press Any Key to Start";
#elif UNITY_ANDROID
        textStart.text = "Touch to Start";
#endif

        textStart.rectTransform.DOScale(new Vector3(1.2f, 1.2f, 1), 0.4f).SetLoops(-1, LoopType.Yoyo);
    }

    private void Update()
    {
#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)
        if (Input.anyKeyDown)
        {
            beatSelectScreen.SetActive(true);
        }
#elif UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            beatSelectScreen.SetActive(true);
        }
#endif
    }

    public void GoToMain()
    {
        SceneManager.LoadScene("Main");
    }
}
