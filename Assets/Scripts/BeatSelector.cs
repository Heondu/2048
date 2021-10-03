using UnityEngine;

public class BeatSelector : MonoBehaviour
{
    [SerializeField] private TitleManager titleManager;

    public static int BeatNumber = 0;

    public void SelectBeat(int num)
    {
        BeatNumber = num;
        titleManager.GoToMain();
    }
}
