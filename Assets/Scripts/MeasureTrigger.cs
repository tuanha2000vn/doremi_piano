using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeasureTrigger : MonoBehaviour
{
    // Use this for initialization
    //private GameObject NoteFlow;
    private NoteCreator noteCreator;
    private NoteFlowControl noteFlowControl;
    private Dictionary<int, IEnumerator> dictGenerate;
    private IEnumerator deGenerateCoroutine;

    void Start()
    {
        dictGenerate = new Dictionary<int, IEnumerator>();
        deGenerateCoroutine = null;
        noteCreator = GetComponent<NoteCreator>();
        noteFlowControl = GameObject.Find("NoteFlow").GetComponent<NoteFlowControl>();
        var scaleUp = 1.2f;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.size = new Vector2(Helpers.CamWidth, Helpers.CamHeight * scaleUp);
        BoxCollider2D bc = GetComponent<BoxCollider2D>();
        bc.size = new Vector2(Helpers.CamWidth, Helpers.CamHeight * scaleUp);
        transform.localPosition = new Vector3(0, (Helpers.CamHeight * scaleUp - Helpers.CamHeight) / 2, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var measureNumberText = other.name.Replace("M:", "");

        int measureNumber;
        if (int.TryParse(measureNumberText, out measureNumber))
        {
            if (!dictGenerate.ContainsKey(measureNumber))
            {
                IEnumerator coroutine = noteCreator.GenerateMeasureScore(measureNumber);
                dictGenerate.Add(measureNumber, coroutine);
                StartCoroutine(coroutine);
                //Debug.Log("OnTriggerEnter2D StartCoroutine " + measureNumber);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var measureNumberText = other.name.Replace("M:", "");

        var measureNumber = 0;
        if (int.TryParse(measureNumberText, out measureNumber))
        {
            if (deGenerateCoroutine != null)
            {
                StopCoroutine(deGenerateCoroutine);
            }

            if (dictGenerate.ContainsKey(measureNumber))
            {
                //Debug.Log("OnTriggerExit2D " + measureNumber + " StopCoroutine DictGenerate.Count " +
                //          dictGenerate.Count);
                StopCoroutine(dictGenerate[measureNumber]);
                dictGenerate.Remove(measureNumber);
            }


            string measureListString = "";
            foreach (var rec in dictGenerate)
            {
                measureListString = measureListString + rec.Key.ToString("000") + "|";
            }


            if (measureListString != "")
            {
                deGenerateCoroutine = noteCreator.DeGenerateMeasureScore(measureListString);
                StartCoroutine(deGenerateCoroutine);
            }


            if (Helpers.IsPlaying
                && measureNumber == Helpers.TotalMeasure)
            {
                Debug.Log("NoteFlowToStart");
                noteFlowControl.NoteFlowToStart(1);
            }
        }
    }

    //private void OnTriggerStay2D(Collider2D other)
    //{
    //    var measureNumberText = other.name.Replace("M:", "");
    //    int measureNumber;
    //    if (int.TryParse(measureNumberText, out measureNumber))
    //    {
    //        if (!dictGenerate.ContainsKey(measureNumber))
    //        {
    //            IEnumerator coroutine = noteCreator.GenerateMeasureScore(measureNumber);
    //            dictGenerate.Add(measureNumber, coroutine);
    //            StartCoroutine(coroutine);
    //            Debug.LogWarning("OnTriggerStay2D StartCoroutine " + measureNumber);
    //        }
    //    }
    //}
}