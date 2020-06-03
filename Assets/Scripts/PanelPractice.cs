using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelPractice : MonoBehaviour
{
    public GameObject ButtonPractice;
    public MainMenu MainMenu;
    private GameObject PracticePreAdd;
    private GameObject PracticePause;

    // Use this for initialization
    void Start()
    {
        ButtonPractice.transform.GetChild(1).GetComponent<Image>().sprite =
            Resources.Load("Sprites\\" + Helpers.PracticeMode, typeof(Sprite)) as Sprite;
        SetPracticeStatus();
    }

    public void PracticeClicked(string practiceMode)
    {
        Helpers.PracticeMode = practiceMode;
        ButtonPractice.transform.GetChild(1).GetComponent<Image>().sprite =
            Resources.Load("Sprites\\" + Helpers.PracticeMode, typeof(Sprite)) as Sprite;
        SetPracticeStatus();
        ButtonPractice.GetComponent<Animator>().SetBool("BoxUp", true);
        MainMenu.HideSubPanel();
    }

    void SetPracticeStatus()
    {
        GameObject MainContainer = GameObject.Find("MainContainer");

        if (MainContainer == null)
        {
            return;
        }

        var notePlayStop = GameObject.Find("Keyboard");
        if (notePlayStop != null)
        {
            notePlayStop.GetComponent<NotePlayStop>().ClearAll();
        }
        else
        {
            //Debug.LogWarning($"notePlayStop == null");
        }

        PracticePreAdd = MainContainer.transform.GetChild(0).GetChild(0).gameObject;
        PracticePause = MainContainer.transform.GetChild(0).GetChild(2).gameObject;

        if (Helpers.PracticeMode == "Practice-No-Hand"
            || SceneManager.GetActiveScene().buildIndex != 3)
        {
            if (PracticePreAdd.activeSelf)
            {
                PracticePreAdd.SetActive(false);
            }

            if (PracticePause.activeSelf)
            {
                PracticePause.SetActive(false);
            }
        }
        else
        {
            if (!PracticePreAdd.activeSelf)
            {
                PracticePreAdd.SetActive(true);
            }

            if (!PracticePause.activeSelf)
            {
                PracticePause.SetActive(true);
            }
        }
    }
}