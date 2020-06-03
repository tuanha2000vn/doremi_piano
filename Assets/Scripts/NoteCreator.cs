using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MusicXml.Domain;
using UnityEngine;
using UnityEngine.UI;

public class NoteCreator : MonoBehaviour
{
    public GameObject NoteAnimation;
    public GameObject MeasurePrefab;
    public Sprite NoteNormal;

    private GameObject measureHolder;
    private GameObject noteHolder;
    [SerializeField] private List<GameObject> listNoteCreated;
    private Text songNameTopText;


    // Use this for initialization
    void Awake()
    {
        measureHolder = GameObject.Find("MeasureHolder");
        noteHolder = GameObject.Find("NoteHolder");
    }

    void Start()
    {
        //songNameTopText = GameObject.Find("SongNameTop").GetComponent<Text>();
        //songNameTopText.text = Helpers.songInfoName;
        //if (!string.IsNullOrEmpty(Helpers.songInfoComposer))
        //{
        //    songNameTopText.text = songNameTopText.text + " - " + Helpers.songInfoComposer;
        //}

        listNoteCreated = new List<GameObject>();
    }

    public void CreateMeasure(Measure measure)
    {
        //Debug.LogWarning("CreateMeasure " + measure.Number);
        var measureName = "M:" + measure.Number.ToString("000");
        var measureGenerate = Instantiate(MeasurePrefab, Vector3.zero, Quaternion.identity);

        measureGenerate.transform.SetParent(measureHolder.transform, false);
        measureGenerate.name = measureName;
        float postY = measure.StartPos * Helpers.QuarterNoteLength /
                      (float) measure.Attributes.Divisions;
        if (Helpers.LastMeasurePostY < postY)
        {
            Helpers.LastMeasurePostY = postY;
        }

        measureGenerate.transform.localPosition = new Vector3(0, postY, 0);
        float height = measure.Duration * Helpers.QuarterNoteLength /
                       (float) measure.Attributes.Divisions;
        SpriteRenderer sr = measureGenerate.GetComponent<SpriteRenderer>();
        sr.size = new Vector2(Helpers.KeysWidth, 0.2f);
        BoxCollider2D bc = measureGenerate.GetComponent<BoxCollider2D>();
        bc.size = new Vector2(Helpers.KeysWidth, height);
        bc.offset = new Vector2(0, height / 2);
        var textLeft = measureGenerate.transform.GetChild(0).gameObject;
        var textRight = measureGenerate.transform.GetChild(1).gameObject;
        textLeft.transform.localPosition = new Vector3(0 - Helpers.KeysWidth / 2 + 0.5f, 0, 0);
        textRight.transform.localPosition = new Vector3(Helpers.KeysWidth / 2 - 0.5f, 0, 0);
        var noteSeparatorSortingLayerId = measureGenerate.GetComponent<Renderer>().sortingLayerID;
        textLeft.GetComponent<Renderer>().sortingLayerID = noteSeparatorSortingLayerId;
        textRight.GetComponent<Renderer>().sortingLayerID = noteSeparatorSortingLayerId;
        var measureGenerateSortingOrder = measureGenerate.GetComponent<Renderer>().sortingOrder;
        textLeft.GetComponent<Renderer>().sortingOrder = measureGenerateSortingOrder + 1;
        textRight.GetComponent<Renderer>().sortingOrder = measureGenerateSortingOrder + 1;
        textLeft.GetComponent<TextMesh>().text = measure.Number.ToString();
        textRight.GetComponent<TextMesh>().text = measure.Number.ToString();
    }

    public IEnumerator GenerateMeasureScore(int measureNumber)
    {
        //var startTime = DateTime.Now;
        //int i = 0;
        //Debug.Log("GenerateMeasureScore " + measureNumber + " Start " + startTime.ToString("MM:SS:ffff"));

        foreach (var part in Helpers.Score.Parts)
        {
            Measure measure = part.Measures
                .FirstOrDefault(m => m.Number == measureNumber);

            if (measure == null)
            {
                continue;
            }

            //var listNote = new List<Note>();
            foreach (var element in measure.MeasureElements)
            {
                if (element.Type == MeasureElementType.Note)
                {
                    Note note = element.Element as Note;
                    if (note != null
                        && note.Pitch != null
                        && note.Tied != "stop")
                    {
                        //listNote.Add(note);
                        CreateNote(measure, note, part.Name);
                        //i++;
                        yield return null;
                    }
                }
            }
        }

        FixNoteSize(measureNumber);
        yield return null;

        //Debug.Log("GenerateMeasureScore " + measureNumber + " Created " + i + " Note Take " +
        //          (DateTime.Now - startTime).TotalMilliseconds.ToString("0") +
        //          " TotalMilliseconds");
    }

    public IEnumerator DeGenerateMeasureScore(string measureListString)
    {
        //var startTime = DateTime.Now;
        //Debug.Log("DeGenerateMeasureScore " + measureListString + " Start " + startTime.ToString("MM:SS:ffff"));

//#if UNITY_EDITOR
        var tempList = listNoteCreated.Where(obj =>
            obj.GetComponent<NoteAnimation>().InUsed
            && !measureListString.Contains(obj.GetComponent<NoteAnimation>().MeasureNumber.ToString("000"))
            //&& obj.activeSelf
            && !obj.GetComponent<SpriteRenderer>().IsVisibleFrom(Camera.main)).ToList();
//#else
//        var tempList = listNoteCreated.Where(obj =>
//            obj.activeSelf
//            && !measureListString.Contains(GetNoteMeasureNumberString(obj.name))
//            && !obj.GetComponent<Renderer>().isVisible).ToList();
//#endif

        foreach (GameObject obj in tempList)
        {
            //obj.SetActive(false);
            obj.GetComponent<NoteAnimation>().InUsed = false;
        }

        yield return null;

        //Debug.Log("DeGenerateMeasureScore " + measureListString + " End Take " +
        //          (DateTime.Now - startTime).TotalMilliseconds.ToString("0") +
        //          " TotalMilliseconds");
    }

    private string GetNoteMeasureNumberString(string noteName)
    {
        var returnVal = noteName.Substring(2, 3);
        //Debug.Log("noteName " + noteName + " returnVal " + returnVal);
        return returnVal;
    }

    private List<GameObject> listGraceNote = new List<GameObject>();
    private List<GameObject> listArpeggiateNote = new List<GameObject>();

    private void CreateNote(Measure measure, Note note, string partName)
    {
        GameObject noteGenerate = null;
        int noteIndex = Helpers.NoteIndex(note.Pitch, 0);

        var noteName = "M:" + measure.Number.ToString("000") +
                       "@" + note.StartPos +
                       "|" + note.Pitch.Octave + note.Pitch.Step + note.Pitch.Alter +
                       "(" + noteIndex + ")" +
                       note.Duration +
                       "<" + note.Pmn + ">";

        var noteType = "";
        if (partName != ""
            && !partName.Contains("Piano")
            && !partName.Contains("Klavier"))
        {
            noteType = "V";
        }
        else if (note.Staff == 1)
        {
            noteType = "R";
        }
        else
        {
            noteType = "L";
        }

        noteName = noteName + noteType;
        //Debug.Log("Creating " + noteName);

        noteGenerate = listNoteCreated.FirstOrDefault(rec => rec.name == noteName);
        if (noteGenerate != null)
        {
            noteGenerate.GetComponent<NoteAnimation>().InUsed = true;
            noteGenerate.SetActive(true);
            //var sr = noteGenerate.GetComponent<SpriteRenderer>();
            //sr.sprite = NoteNormal;
            ////Debug.LogWarning(noteName + " is still there");
            return;
        }

        noteGenerate = listNoteCreated.FirstOrDefault(rec => !rec.GetComponent<NoteAnimation>().InUsed);

        if (noteGenerate == null)
        {
            //noteGenerate = Instantiate(NotePrefab, Vector3.zero, Quaternion.identity);
            noteGenerate = Instantiate(NoteAnimation, Vector3.zero, Quaternion.identity);
            noteGenerate.transform.SetParent(noteHolder.transform, false);
            var meshRenderer = noteGenerate.GetComponentInChildren<MeshRenderer>();
            meshRenderer.sortingLayerName = "NoteWhiteSort";
            meshRenderer.sortingOrder = 2;
            listNoteCreated.Add(noteGenerate);
        }
        else
        {
            //noteGenerate.SetActive(true);
            noteGenerate.GetComponent<NoteAnimation>().InUsed = true;
            noteGenerate.SetActive(true);
        }


        noteGenerate.name = noteName;
        var noteAnimation = noteGenerate.GetComponent<NoteAnimation>();
        noteAnimation.MeasureNumber = measure.Number;
        noteAnimation.NoteIndex = noteIndex;
        noteAnimation.PracticePlayed = false;
        noteAnimation.Fingering = note.Finger;

        noteAnimation.SetNoteSize(noteIndex, note.Duration
                                             * Helpers.QuarterNoteLength /
                                             (float) measure.Attributes.Divisions);
        noteAnimation.NoteNormalReset();

        float postX = Helpers.KeysDict[noteIndex + Helpers.LearnTranspose].transform.localPosition.x;
        float postY = (measure.StartPos + note.StartPos) * Helpers.QuarterNoteLength /
                      (float) measure.Attributes.Divisions;


        noteGenerate.transform.localPosition = new Vector3(postX, postY, 0);

        if (note.IsGrace)
        {
            listGraceNote.Add(noteGenerate);
            //Debug.LogWarning("note.IsGrace " + noteGenerate.name + " listGraceNote.Count " + listGraceNote.Count +
            //                 " note.StartPos " + note.StartPos);
            noteGenerate.transform.localPosition = new Vector3(noteGenerate.transform.localPosition.x,
                noteGenerate.transform.localPosition.y - 0.4f, 0);

            foreach (var graceNote in listGraceNote)
            {
                if (graceNote == noteGenerate)
                {
                    continue;
                }

                graceNote.transform.localPosition = new Vector3(graceNote.transform.localPosition.x,
                    graceNote.transform.localPosition.y - 0.4f, 0);
                //Debug.LogWarning(graceNote.name + " Changed Y " + graceNote.transform.localPosition.y);
            }
        }
        else
        {
            if (listGraceNote.Count > 0)
            {
                listGraceNote.Clear();
            }
        }

        if (note.IsArpeggiate)
        {
            if (!note.IsChordTone)
            {
                listArpeggiateNote.Clear();
            }

            listArpeggiateNote.Add(noteGenerate);
            //Debug.LogWarning("note.IsArpeggiate " + noteGenerate.name + " listGraceNote.Count " + listGraceNote.Count);

            foreach (var arpeggiateNote in listArpeggiateNote)
            {
                if (arpeggiateNote == noteGenerate)
                {
                    continue;
                }

                arpeggiateNote.transform.localPosition = new Vector3(arpeggiateNote.transform.localPosition.x,
                    arpeggiateNote.transform.localPosition.y - 0.2f, 0);
                //Debug.LogWarning(graceNote.name + " Changed Y " + graceNote.transform.localPosition.y);
            }
        }

        //FixNoteSize(noteGenerate);
        //Debug.Log("Created note " + noteName);
    }

    void FixNoteSize(int measureNumber)
    {
        listNoteCreated = listNoteCreated
            .OrderBy(obj => obj.transform.localPosition.y)
            .ToList();

        var tempList = listNoteCreated.Where(obj =>
            obj.GetComponent<NoteAnimation>().MeasureNumber == measureNumber);

        foreach (var noteGenerated in tempList)
        {
            var noteX = noteGenerated.transform.localPosition.x;
            var noteYStart = noteGenerated.transform.localPosition.y;

            var previousNote = listNoteCreated
                .Where(obj =>
                    obj != noteGenerated
                    && !obj.name.Contains("V")
                    && obj.activeSelf
                    && Mathf.Approximately(noteX, obj.transform.localPosition.x)
                    && obj.transform.localPosition.y <= noteYStart)
                .OrderByDescending(obj => obj.transform.localPosition.y)
                .FirstOrDefault();

            if (previousNote == null)
            {
                //Debug.Log(note.name + " previousNote == null");
                continue;
            }

            //Debug.Log("Previous note " + note.name + " is " + previousNote.name);
            var previousNoteYStart = previousNote.transform.localPosition.y;
            var previousNoteYEnd = previousNoteYStart + GetNoteHeight(previousNote);

            if (previousNoteYEnd <= noteYStart)
            {
                //Debug.Log(note.name + " previousNote previousNoteYEnd <= noteYStart " + previousNoteYEnd + " < " +
                //          noteYStart);
                continue;
            }


            //Debug.LogWarning(previousNote.name + " " +
            //                 previousNoteYStart + "-" + previousNoteYEnd + " overlap " + note.name + " " +
            //                 noteYStart +
            //                 "-" + noteYEnd);

            //note.GetComponent<SpriteRenderer>().color = Helpers.ColorOrange05;
            //previousNote.GetComponent<SpriteRenderer>().color = Helpers.ColorRed05;

            //Two Note Have Same Start
            if (Mathf.Approximately(noteYStart, previousNoteYStart))
            {
                var noteYEnd = noteYStart + GetNoteHeight(noteGenerated);
                //Have Same End
                //Disable Previous Note
                if (Mathf.Approximately(noteYEnd, previousNoteYEnd))
                {
                    //previousNote.SetActive(false);
                    //Debug.LogWarning(
                    //    $"{noteGenerated.name} and {previousNote.name} Have Same Start Have Same End: Disable Previous Note");
                }
                //This Note Is Longer
                //Disable Previous Note
                else if (noteYEnd > previousNoteYEnd)
                {
                    previousNote.SetActive(false);
                    //Debug.LogWarning(
                    //    $"{noteGenerated.name} and {previousNote.name} Have Same Start {noteGenerated.name} Is Longer: Disable: Disable {previousNote.name}");
                }
                //Previous Note Is Longer
                //Disable This Note
                else
                {
                    noteGenerated.SetActive(false);
                    //Debug.LogWarning(
                    //    $"{noteGenerated.name} and {previousNote.name} Have Same Start {previousNote.name} Is Longer: Disable: Disable {noteGenerated.name}");
                }
            }
            //Previous Note < Current Note
            //Cut Previous Note to Current Note Start
            else if (previousNoteYStart < noteYStart)
            {
                var newSizeY = noteYStart - previousNoteYStart;
                var objectToChangeSize = previousNote;
                objectToChangeSize.GetComponent<NoteAnimation>().SetNoteHeight(newSizeY);
                //var sr = objectToChangeSize.GetComponent<SpriteRenderer>();
                //sr.size = new Vector2(sr.size.x, newSizeY);
                //var bc = objectToChangeSize.GetComponent<BoxCollider2D>();
                //bc.size = new Vector2(bc.size.x, newSizeY);
                //bc.offset = new Vector2(0, newSizeY / 2);
                //Debug.LogWarning(
                //    $"{previousNote.name} < {noteGenerated.name}: Cut {previousNote.name} End to {noteGenerated.name} Start");
            }
            //Previous Note > Current Note
            //Cut Current Note to Previous Note Start
            else
            {
                var newSizeY = previousNoteYStart - noteYStart;
                var objectToChangeSize = noteGenerated;
                objectToChangeSize.GetComponent<NoteAnimation>().SetNoteHeight(newSizeY);
                //var sr = objectToChangeSize.GetComponent<SpriteRenderer>();
                //sr.size = new Vector2(sr.size.x, newSizeY);
                //var bc = objectToChangeSize.GetComponent<BoxCollider2D>();
                //bc.size = new Vector2(bc.size.x, newSizeY);
                //bc.offset = new Vector2(0, newSizeY / 2);
                //Debug.LogWarning(
                //    $"{previousNote.name} > {noteGenerated.name}: Cut {previousNote.name} End to {noteGenerated.name} Start");
            }
        }
    }

    float GetNoteHeight(GameObject note)
    {
        var sr = note.GetComponent<SpriteRenderer>();
        return sr.size.y;
    }

    float GetNoteWidth(GameObject note)
    {
        var sr = note.GetComponent<SpriteRenderer>();
        return sr.size.x;
    }
}