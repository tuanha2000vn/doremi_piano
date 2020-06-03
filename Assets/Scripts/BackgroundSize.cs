using UnityEngine;

public class BackgroundSize : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.size = new Vector2(Helpers.CamWidth, Helpers.CamHeight);
        BoxCollider2D bc = GetComponent<BoxCollider2D>();
        if (bc != null)
        {
            bc.size = new Vector2(Helpers.CamWidth, Helpers.CamHeight);
        }
    }
}