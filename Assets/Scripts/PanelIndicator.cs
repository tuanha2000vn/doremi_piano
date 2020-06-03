using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelIndicator : MonoBehaviour
{
    public GameObject ImageIndicatorBackground;
    public GameObject ImageIndicatorTriangle;
    public GameObject ImageIndicator;
    public GameObject PanelText;
    public Text TextIndicator;
    public Button ButtonOK;

    private IEnumerator RotateIndicatorIe;
    private IEnumerator LoadSceneIe;
    private IEnumerator LoadSceneWarningIe;

    private int sceneWarningId = 0;

    void Awake()
    {
        HideIndicator("Awake");
    }

    // Use this for initialization
    void Start()
    {
        RotateIndicatorIe = RotateIndicatorCr();
        ButtonOK.onClick.AddListener(HandleClick);
    }

    public void HandleClick()
    {
        SceneManager.LoadScene(sceneWarningId);
    }

    public void LoadLevel(int sceneIndex, string textIndicator = null)
    {
        if (LoadSceneIe != null)
        {
            StopCoroutine(LoadSceneIe);
        }

        LoadSceneIe = LoadSceneCr(sceneIndex, textIndicator);
        StartCoroutine(LoadSceneIe);
    }

    public void LoadWarning(string textIndicator, int delay, int sceneIndex)
    {
        sceneWarningId = sceneIndex;
        ShowWarning(textIndicator);
        if (sceneIndex >= 0)
        {
            if (LoadSceneWarningIe != null)
            {
                StopCoroutine(LoadSceneWarningIe);
            }

            LoadSceneWarningIe = LoadSceneWarning(sceneIndex, delay);

            StartCoroutine(LoadSceneWarningIe);
        }
    }

    IEnumerator LoadSceneWarning(int sceneIndex, int delay)
    {
        yield return new WaitForSeconds(delay);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        while (!operation.isDone)
        {
            yield return null;
        }
    }

    IEnumerator LoadSceneCr(int sceneIndex, string textIndicator)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        while (!operation.isDone)
        {
            ShowIndicator(textIndicator, "PanelIndicator LoadSceneCr");
            yield return null;
        }
    }

    public void ShowWarning(string textIndicator)
    {
        HideIndicator("ShowWarning");
        Debug.Log("ShowWarning " + textIndicator);
        ImageIndicatorBackground.SetActive(true);
        //Debug.Log("ImageIndicatorBackground.activeSelf " + ImageIndicatorBackground.activeSelf);
        ImageIndicatorTriangle.SetActive(true);
        //Debug.Log("ImageIndicatorTriangle.activeSelf " + ImageIndicatorTriangle.activeSelf);
        ButtonOK.gameObject.SetActive(true);
        //Debug.Log("ButtonOK.gameObject.activeSelf " + ButtonOK.gameObject.activeSelf);
        PanelText.SetActive(true);
        //Debug.Log("PanelText.activeSelf " + PanelText.activeSelf);
        TextIndicator.text = textIndicator;
        //Debug.Log("TextIndicator.text " + TextIndicator.text);
    }

    public void ShowIndicator(string textIndicator, string fromCaller)
    {
        Debug.Log("ShowIndicator fromCaller " + fromCaller);
        RotateIndicatorIe = RotateIndicatorCr();
        StartCoroutine(RotateIndicatorIe);
        ImageIndicatorBackground.SetActive(true);
        ImageIndicator.SetActive(true);

        if (!string.IsNullOrEmpty(textIndicator))
        {
            PanelText.SetActive(true);
            TextIndicator.text = textIndicator;
        }
    }

    public void HideIndicator(string fromCaller)
    {
        Debug.Log("HideIndicator " + fromCaller);
        RotateIndicatorIe = RotateIndicatorCr();
        StopCoroutine(RotateIndicatorIe);
        ImageIndicatorBackground.SetActive(false);
        ImageIndicator.SetActive(false);
        ImageIndicatorTriangle.SetActive(false);
        PanelText.SetActive(false);
        TextIndicator.text = string.Empty;
        ButtonOK.gameObject.SetActive(false);
    }

    IEnumerator RotateIndicatorCr()
    {
        while (true)
        {
            ImageIndicator.transform.Rotate(Vector3.forward * -30f);
            //Debug.Log("Rotate " + ImageIndicator.transform.localRotation.z);
            //yield return null;
            yield return new WaitForSeconds(0.1f);
            //yield return new WaitForEndOfFrame();
        }
    }
}