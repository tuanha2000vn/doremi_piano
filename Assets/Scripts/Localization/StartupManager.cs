using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupManager : MonoBehaviour
{
    // Use this for initialization
    IEnumerator Start()
    {
        while (!LocalizationManager.instance.LocalizationManagerIsReady)
        {
            yield return null;
        }

        SceneManager.LoadScene("MenuScreen");
    }
}