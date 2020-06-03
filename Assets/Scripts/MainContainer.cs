using UnityEngine;
using UnityEngine.SceneManagement;

public class MainContainer : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            transform.localScale = new Vector3(Helpers.ScaleMin, Helpers.ScaleMin, 1);
        }
        else
        {
            transform.localScale = new Vector3(Helpers.ScaleMiddle, Helpers.ScaleMiddle, 1);
        }
    }


    public void KeyboardClamEdge()
    {
        var edgeLeftPos = transform.localPosition.x -
                          Helpers.KeysWidth / 2 * transform.localScale.x;

        if (edgeLeftPos > Helpers.LeftPos.x)
        {
            //Debug.Log(" edgeLeftPos " + edgeLeftPos + " Helpers.LeftPos.x " + Helpers.LeftPos.x);
            transform.Translate(Helpers.LeftPos.x - edgeLeftPos, 0, 0);
            return;
        }

        //Prevent Zigzag
        //if (Helpers.KeysWidth * Helpers.MainPlaneScale <= Helpers.CamWidth)
        //{
        //    return;
        //}

        var edgeRightPos = transform.localPosition.x +
                           Helpers.KeysWidth / 2 * transform.localScale.x;
        if (edgeRightPos < Helpers.RightPos.x)
        {
            //Debug.Log(" edgeRightPos " + edgeRightPos + " Helpers.LeftPos.x " + Helpers.LeftPos.x);
            transform.Translate(Helpers.RightPos.x - edgeRightPos, 0, 0);
        }
    }
}