using UnityEngine;
using UnityEngine.UI;

public class SongSelectHeader : MonoBehaviour
{
    public InputField InputField;
    public GameObject ButtonClearText;
    public GameObject Content;

    private SongScrollList songScrollList;

    //private Color green = new Color(0f, 204 / 255f, 51 / 255f, 1f);
    private Color green;
    private Color white;

    // Use this for initialization
    void Start()
    {
        Color newCol;
        if (ColorUtility.TryParseHtmlString("#2196F3", out newCol))
        {
            green = newCol;
        }

        if (ColorUtility.TryParseHtmlString("#BBDEFB", out newCol))
        {
            white = newCol;
        }

        ButtonClearText.SetActive(false);
        songScrollList = Content.GetComponent<SongScrollList>();
        OnButtonClick(Helpers.SongViewMode);

    }

    // Update is called once per frame
    //void Update()
    //{
    //}

    public void ClearSearchClicked()
    {
        Debug.Log("ClearSearchClicked");
        if (InputField.text.Trim() != "")
        {
            InputField.text = "";
        }
    }

    public void OnInputValueChanged()
    {
        Debug.Log("OnInputValueChanged " + InputField.text.Trim());
        if (InputField.text.Trim() != "")
        {
            ButtonClearText.SetActive(true);
        }
        else
        {
            ButtonClearText.SetActive(false);
            Debug.Log("OnInputValueChanged Search for all songs");
        }

        songScrollList.RefreshDisplay();
    }

    public void OnButtonClick(string viewModeName)
    {
        Helpers.SongViewMode = viewModeName;

        if (songScrollList == null)
        {
            return;
        }

        foreach (Transform child in transform)
        {
            var childName = child.gameObject.name;
            if (!childName.Contains("Button"))
            {
                continue;
            }

            var image = child.gameObject.GetComponent<Image>();
            if (child.gameObject.name.Contains(viewModeName))
            {
                image.color = green;
            }
            else
            {
                image.color = white;
            }
        }

        songScrollList.RefreshDisplay();
    }
}