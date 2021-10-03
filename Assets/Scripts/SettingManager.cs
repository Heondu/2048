using UnityEngine;

public class SettingManager : MonoBehaviour
{
    [SerializeField] private GameObject Settings;
    [SerializeField] private GameManager gameManager;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Settings.activeSelf == false)
            {
                Show();
            }
            else
            {
                Close();
            }
        }
    }

    public void Show()
    {
        Time.timeScale = 0;
        SoundManager.Instance.Stop(true);
        Settings.SetActive(true);
    }

    public void Close()
    {
        Settings.SetActive(false);
        if (gameManager.State != GameState.GameOver)
        {
            SoundManager.Instance.Stop(false);
            Time.timeScale = 1;
        }
    }

    public void TurnOnBGM(bool value)
    {
        SoundManager.Instance.TurnOnBGM(value);
    }

    public void TurnOnSE(bool value)
    {
        SoundManager.Instance.TurnOnSE(value);
    }
}
