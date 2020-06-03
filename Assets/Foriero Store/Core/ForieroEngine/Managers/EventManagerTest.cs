using UnityEngine;
using System.Collections;



public class EventManagerTest : MonoBehaviour
{


    // Use this for initialization
    void Start()
    {
        ForieroEngine.EventManager.AddListener<ForieroEngine.EventManager.Test>(B);
    }

    void B(ForieroEngine.EventManager.Test t)
    {
        Debug.Log("WOW : " + t.i);
    }

    void OnDestroy()
    {
        ForieroEngine.EventManager.RemoveListener<ForieroEngine.EventManager.Test>(B);
    }
}
