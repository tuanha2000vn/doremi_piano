using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabScrollList : MonoBehaviour
{
    public InputField InputField;
    public GameObject ButtonClearText;
    public GameObject Content;

    private SongScrollList songScrollList;

    // Use this for initialization
    void Start()
    {
        ButtonClearText.SetActive(false);
        songScrollList = Content.GetComponent<SongScrollList>();
        InputField.text = Helpers.SearchText;
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
        Helpers.SearchText = InputField.text.Substring(0, Mathf.Min(InputField.text.Trim().Length, 100));
        Helpers.SearchText = Helpers.SearchText.Trim();
        Debug.Log("OnInputValueChanged " + Helpers.SearchText);

        if (!string.IsNullOrEmpty(Helpers.SearchText))
        {
            ButtonClearText.SetActive(true);
        }
        else
        {
            ButtonClearText.SetActive(false);
            Debug.Log("OnInputValueChanged Search for all songs");
        }


        songScrollList.RefreshDisplay();


        foreach (Transform child in transform)
        {
            if (!child.name.Contains("Button"))
            {
                continue;
            }

            if (string.IsNullOrEmpty(Helpers.SearchText))
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    public void OnButtonClick(string viewModeName)
    {
        Debug.Log("OnButtonClick " + viewModeName);
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

            var image = child.GetChild(0).GetComponent<Image>();
            if (child.gameObject.name.Contains(viewModeName))
            {
                image.color = Helpers.ColorAmber10;
            }
            else
            {
                image.color = Helpers.ColorWhite75;
            }

            var text = child.GetChild(1).GetComponent<Text>();
            if (child.gameObject.name.Contains(viewModeName))
            {
                text.color = Helpers.ColorAmber10;
            }
            else
            {
                text.color = Helpers.ColorWhite75;
            }
        }

        songScrollList.RefreshDisplay();
    }
}