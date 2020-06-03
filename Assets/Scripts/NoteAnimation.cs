using System;
using System.Collections;
using UnityEngine;

public class NoteAnimation : MonoBehaviour
{
    public SpriteRenderer NoteHighlight;
    public BoxCollider2D NoteHighlightBoxCollider;
    public SpriteMask NoteMask;
    public SpriteRenderer NoteNormal;
    public TextMesh NoteFingering;
    public bool InUsed = true;
    public bool PracticePlayed = false;
    public int MeasureNumber = 0;
    public int NoteIndex = 0;
    public int Fingering = 0;
    public float width;
    public float height;
    private IEnumerator animateNoteNormalIe;

    void Start()
    {
        animateNoteNormalIe = AnimateNoteNormal();
    }

    public void SetNoteSize(int noteIndex, float duration)
    {
        noteIndex = NoteIndex + Helpers.LearnTranspose;
        //Debug.Log($"{NoteIndex} {noteIndex}");
        if (Helpers.KeysBlacksDict.ContainsKey(noteIndex))
        {
            width = Helpers.KeyBlackWidth * 1.2f;
            gameObject.layer = LayerMask.NameToLayer("NoteBlackLayer");
            NoteHighlight.sortingLayerName = "NoteBlackSort";
            NoteNormal.sortingLayerName = "NoteBlackSort";
            NoteFingering.GetComponent<Renderer>().sortingLayerName = "NoteBlackSort";
        }
        else
        {
            width = Helpers.KeyWhiteWidth;
            gameObject.layer = LayerMask.NameToLayer("NoteWhiteLayer");
            NoteHighlight.sortingLayerName = "NoteWhiteSort";
            NoteNormal.sortingLayerName = "NoteWhiteSort";
            NoteFingering.GetComponent<Renderer>().sortingLayerName = "NoteWhiteSort";
        }

        NoteFingering.GetComponent<Renderer>().sortingOrder = 2;
        if (Fingering != 0)
        {
            NoteFingering.text = Fingering.ToString();
        }
        else
        {
            NoteFingering.text = String.Empty;
        }

        height = Mathf.Max(duration, Helpers.KeyBlackWidth * 2);

        SetNoteHeight(height);

        if (gameObject.name.Contains("V"))
        {
            NoteHighlight.color = Helpers.ColorWhite00;
            NoteNormal.color = Helpers.ColorWhite00;
        }
        else if (gameObject.name.Contains("R"))
        {
            NoteHighlight.color = Helpers.ColorGreen05;
            NoteNormal.color = Helpers.ColorGreen10;
        }
        else
        {
            NoteHighlight.color = Helpers.ColorBlue05;
            NoteNormal.color = Helpers.ColorBlue10;
        }
    }

    public void SetNoteHeight(float duration)
    {
        height = duration;
        NoteNormal.transform.localPosition = Vector3.zero;
        NoteHighlight.size = new Vector2(width, height);
        var colliderReduce = 1f;
        if (height <= colliderReduce)
        {
            colliderReduce = 0;
        }

        NoteHighlightBoxCollider.size = new Vector2(width, height - colliderReduce);
        NoteHighlightBoxCollider.offset = new Vector2(0, (height * 0.5f) - colliderReduce * 0.5f);
        NoteNormal.size = new Vector2(width, height);
        NoteMask.transform.localScale = new Vector3(width / 2.48f, height / 2.48f, 1);
        NoteMask.transform.localPosition = new Vector3(0, height, 0);
    }

    IEnumerator AnimateNoteNormal()
    {
        while (NoteNormal.size.y > 0)
        {
            var newY = Mathf.Max(0, NoteNormal.size.y - 1f * Helpers.MetronomeSpeed / 10f * Helpers.SpeedValue / 120f);
            NoteNormal.transform.localPosition = new Vector3(0, height - newY);
            NoteNormal.size = new Vector2(width, newY);
            //yield return null;     
            yield return new WaitForSeconds(0.01f);
        }

        StopCoroutine(animateNoteNormalIe);
    }

    public void StartAnimation()
    {
        //Debug.Log($"StartAnimation " + gameObject.name);
        animateNoteNormalIe = AnimateNoteNormal();
        StartCoroutine(animateNoteNormalIe);
    }

    public void NoteNormalReset()
    {
        //Debug.LogWarning($"NoteNormalReset {gameObject.name}");
        if (animateNoteNormalIe != null)
        {
            StopCoroutine(animateNoteNormalIe);
        }

        NoteNormal.size = new Vector2(width, height);
        NoteNormal.transform.localPosition = Vector3.zero;
    }
}