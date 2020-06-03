using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class RayCast01 : MonoBehaviour
{
    public MainMenu mainMenu;
    public PanelMeasure PanelMeasure;
    public GameObject MainContainer;
    public GameObject NoteFlow;
    public LayerMask[] LayerMasks;
    public Sprite KeyWhite;
    public Sprite KeyWhiteDowm;
    public Sprite KeyBlack;
    public Sprite KeyBlackDown;

    private int currentScene;
    private Dictionary<int, int> touchBeganDict;
    private NotePlayStop notePlayStop;
#if UNITY_EDITOR
    private Vector3 _mousePositionDelta = Vector3.zero;
#endif

    //private string currentScene;

    // Use this for initialization
    void Start()
    {
        touchBeganDict = new Dictionary<int, int>();
        currentScene = SceneManager.GetActiveScene().buildIndex;
        notePlayStop = MainContainer.GetComponentInChildren<NotePlayStop>();
    }

    RaycastHit2D GetRayCastHit(Vector2 touchPos)
    {
        //Debug.Log("GetRayCastHit " + touchPos);
        RaycastHit2D hit = new RaycastHit2D();
        for (int i = 0; i < LayerMasks.Length; i++)
        {
            hit = Physics2D.Raycast(touchPos, Vector2.zero, 1000, LayerMasks[i]);
            if (hit)
            {
                //Debug.Log(LayerMasks[i].value + " hit.transform.gameObject.Id " +
                //          hit.transform.gameObject.name);
                break;
            }
        }

        return hit;
    }

    // Update is called once per frame
    void Update()
    {
        float deltaX = 0;
        float deltaY = 0;
        string hitObjectName = string.Empty;
        foreach (var touch in Input.touches)
        {
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                continue;
            }

            if (touch.phase == TouchPhase.Began)
            {
                RaycastHit2D hit = GetRayCastHit(Camera.main.ScreenToWorldPoint(touch.position));
                if (hit)
                {
                    hitObjectName = hit.transform.gameObject.name;
                    //Debug.Log("hit: " + hitObjectName);

                    if (currentScene == 3
                        && hitObjectName == "Background")
                    {
                        if (touchBeganDict.ContainsKey(touch.fingerId))
                        {
                            touchBeganDict[touch.fingerId] = -2;
                        }
                        else
                        {
                            touchBeganDict.Add(touch.fingerId, -2);
                        }

                        PlayPauseOnClick();
                    }

                    if (currentScene == 3
                        && (hitObjectName == "A-Point"
                            || hitObjectName == "A-PointR")
                        && hit.transform.GetComponent<SpriteRenderer>().color != Helpers.ColorWhite00)
                    {
                        mainMenu.PracticeAClicked();
                        //Debug.Log($"Hit {hitObjectName}");
                    }

                    if (currentScene == 3
                        && (hitObjectName == "B-Point"
                            || hitObjectName == "B-PointR")
                        && hit.transform.GetComponent<SpriteRenderer>().color != Helpers.ColorWhite00)
                    {
                        mainMenu.PracticeBClicked();
                        //Debug.Log($"Hit {hitObjectName}");
                    }

                    int objectId;
                    if (int.TryParse(hitObjectName, out objectId))
                    {
                        notePlayStop.NotePlayMidi(objectId, 100,
                            currentScene == 3
                                ? Helpers.GetNoteChannel(objectId, Helpers.InputChannel.InGamePiano)
                                : Helpers.GetNoteChannel(objectId, Helpers.InputChannel.InGameKeyboard));


                        //Sprite sprite = KeyWhiteDowm;
                        //Color color = Color.white;
                        //if (Helpers.KeysBlacksDict.ContainsKey(objectId))
                        //{
                        //    sprite = KeyBlackDown;
                        //    color = Color.gray;
                        //}

                        //notePlayStop.ChangeKeyColor(hit.transform.gameObject, sprite, color);


                        if (touchBeganDict.ContainsKey(touch.fingerId))
                        {
                            touchBeganDict[touch.fingerId] = objectId;
                        }
                        else
                        {
                            touchBeganDict.Add(touch.fingerId, objectId);
                        }
                    }
                }
                else
                {
                    if (touchBeganDict.ContainsKey(touch.fingerId))
                    {
                        touchBeganDict[touch.fingerId] = -1;
                    }
                    else
                    {
                        touchBeganDict.Add(touch.fingerId, -1);
                    }
                }
            }

            if (touch.phase == TouchPhase.Ended
                || touch.phase == TouchPhase.Canceled)
            {
                if (touchBeganDict.ContainsKey(touch.fingerId)
                    && touchBeganDict[touch.fingerId] >= 0)
                {
                    //notePlayStop.NoteStopMidi(touchBeganDict[touch.fingerId],
                    //    Helpers.GetNoteChannel(touchBeganDict[touch.fingerId], Helpers.InputChannel.InGameKeyboard));

                    notePlayStop.NoteStopMidi(touchBeganDict[touch.fingerId],
                        currentScene == 3
                            ? Helpers.GetNoteChannel(touchBeganDict[touch.fingerId], Helpers.InputChannel.InGamePiano)
                            : Helpers.GetNoteChannel(touchBeganDict[touch.fingerId],
                                Helpers.InputChannel.InGameKeyboard));


                    Sprite sprite = KeyWhite;
                    if (Helpers.KeysBlacksDict.ContainsKey(touchBeganDict[touch.fingerId]))
                    {
                        sprite = KeyBlack;
                    }

                    notePlayStop.ChangeKeyColor(Helpers.KeysDict[touchBeganDict[touch.fingerId]], sprite, Color.white);
                }

                if (touchBeganDict.ContainsKey(touch.fingerId))
                {
                    touchBeganDict[touch.fingerId] = -1;
                }

                if (currentScene == 3)
                {
                    var middleScreen = Camera.main.orthographicSize * (1 / Helpers.ScaleMin);
                    var noteFlowY = NoteFlow.transform.localPosition.y;
                    if (noteFlowY > middleScreen)
                    {
                        NoteFlow.transform.localPosition = new Vector3(0, middleScreen, 0);
                    }

                    if (noteFlowY < 0 - (Helpers.LastMeasurePostY + middleScreen * 2))
                    {
                        NoteFlow.transform.localPosition =
                            new Vector3(0, 0 - (Helpers.LastMeasurePostY + middleScreen * 2), 0);
                    }
                }

                MainContainer.GetComponent<MainContainer>().KeyboardClamEdge();
            }

            if (touch.phase == TouchPhase.Moved
                && currentScene != 3
                && touch.deltaPosition.x != 0
                && MainContainer.transform.localScale.x > Helpers.ScaleMin)
            {
                if (touchBeganDict.ContainsKey(touch.fingerId)
                    && touchBeganDict[touch.fingerId] == -1)
                {
                    continue;
                }

                var pos = Camera.main.ScreenToWorldPoint(touch.position);
                var posDelta = Camera.main.ScreenToWorldPoint(touch.position - touch.deltaPosition);
                //Debug.Log("touch.position " + pos + " touch.deltaPosition " + posDelta);
                var diffX = pos.x - posDelta.x;
                if (Mathf.Abs(deltaX) < Mathf.Abs(diffX))
                {
                    deltaX = diffX;
                }
            }

            if (touch.phase == TouchPhase.Moved
                && currentScene == 3
                && touchBeganDict.ContainsKey(touch.fingerId)
                && touchBeganDict[touch.fingerId] == -2
                && touch.deltaPosition.y != 0)
            {
                var pos = Camera.main.ScreenToWorldPoint(touch.position);
                var posDelta = Camera.main.ScreenToWorldPoint(touch.position - touch.deltaPosition);
                //Debug.Log("touch.position " + pos + " touch.deltaPosition " + posDelta);
                var diffY = pos.y - posDelta.y;
                if (Mathf.Abs(deltaY) < Mathf.Abs(diffY))
                {
                    deltaY = diffY;
                }
            }
        }


#if (UNITY_EDITOR)
        if (Input.touches.Length <= 0
            && !EventSystem.current.IsPointerOverGameObject())
        {
            for (int i = 0; i < 3; i++)
            {
                if (Input.GetMouseButtonDown(i))
                {
                    RaycastHit2D hit = GetRayCastHit(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    if (hit)
                    {
                        hitObjectName = hit.transform.gameObject.name;
                        //Debug.Log("hit: " + hitObjectName);

                        if (currentScene == 3
                            && hitObjectName == "Background")
                        {
                            if (touchBeganDict.ContainsKey(11 + i))
                            {
                                //Debug.Log("touchBeganDict.ContainsKey(11 + i): " + touchBeganDict.ContainsKey(11 + i));
                                touchBeganDict[11 + i] = -2;
                            }
                            else
                            {
                                //Debug.Log("touchBeganDict.Add(11 + i): " + touchBeganDict.ContainsKey(11 + i));
                                touchBeganDict.Add(11 + i, -2);
                            }

                            PlayPauseOnClick();
                        }

                        if (currentScene == 3
                            && (hitObjectName == "A-Point"
                                || hitObjectName == "A-PointR")
                            && hit.transform.GetComponent<SpriteRenderer>().color != Helpers.ColorWhite00)
                        {
                            mainMenu.PracticeAClicked();
                            //Debug.Log($"Hit {hitObjectName}");
                        }

                        if (currentScene == 3
                            && (hitObjectName == "B-Point"
                                || hitObjectName == "B-PointR")
                            && hit.transform.GetComponent<SpriteRenderer>().color != Helpers.ColorWhite00)
                        {
                            mainMenu.PracticeBClicked();
                            //Debug.Log($"Hit {hitObjectName}");
                        }

                        int objectId;
                        if (int.TryParse(hitObjectName, out objectId))
                        {
                            notePlayStop.NotePlayMidi(objectId, 100,
                                currentScene == 3
                                    ? Helpers.GetNoteChannel(objectId, Helpers.InputChannel.InGamePiano)
                                    : Helpers.GetNoteChannel(objectId, Helpers.InputChannel.InGameKeyboard));
                            //Sprite sprite = KeyWhiteDowm;
                            //Color color = Color.white;
                            //if (Helpers.KeysBlacksDict.ContainsKey(objectId))
                            //{
                            //    sprite = KeyBlackDown;
                            //    color = Color.gray;
                            //}

                            //notePlayStop.ChangeKeyColor(hit.transform.gameObject, sprite, color);

                            if (touchBeganDict.ContainsKey(11 + i))
                            {
                                touchBeganDict[11 + i] = objectId;
                            }
                            else
                            {
                                touchBeganDict.Add(11 + i, objectId);
                            }
                        }
                    }
                    else
                    {
                        if (touchBeganDict.ContainsKey(11 + i))
                        {
                            touchBeganDict[11 + i] = -1;
                        }
                        else
                        {
                            touchBeganDict.Add(11 + i, -1);
                        }
                    }
                }

                if (Input.GetMouseButtonUp(i))
                {
                    if (touchBeganDict.ContainsKey(11 + i)
                        && touchBeganDict[11 + i] >= 0)
                    {
                        //notePlayStop.NoteStopMidi(touchBeganDict[11 + i],
                        //    Helpers.GetNoteChannel(touchBeganDict[11 + i], Helpers.InputChannel.InGameKeyboard));

                        notePlayStop.NoteStopMidi(touchBeganDict[11 + i],
                            currentScene == 3
                                ? Helpers.GetNoteChannel(touchBeganDict[11 + i], Helpers.InputChannel.InGamePiano)
                                : Helpers.GetNoteChannel(touchBeganDict[11 + i], Helpers.InputChannel.InGameKeyboard));

                        Sprite sprite = KeyWhite;
                        if (Helpers.KeysBlacksDict.ContainsKey(touchBeganDict[11 + i]))
                        {
                            sprite = KeyBlack;
                        }

                        notePlayStop.ChangeKeyColor(Helpers.KeysDict[touchBeganDict[11 + i]], sprite,
                            Helpers.ColorWhite10);
                    }

                    if (touchBeganDict.ContainsKey(11 + i))
                    {
                        touchBeganDict[11 + i] = -1;
                    }

                    if (currentScene == 3)
                    {
                        //var middleScreen = Camera.main.orthographicSize * (1 / Helpers.ScaleMin);
                        var noteFlowY = NoteFlow.transform.localPosition.y;
                        if (noteFlowY > Helpers.OffSet)
                        {
                            NoteFlow.transform.localPosition = new Vector3(0, Helpers.OffSet, 0);
                        }

                        //if (noteFlowY < 0 - (Helpers.MeasureBounds.extents.y * 2 + middleScreen))
                        //{
                        //    NoteFlow.transform.localPosition =
                        //        new Vector3(0, 0 - (Helpers.MeasureBounds.extents.y * 2 + middleScreen), 0);
                        //}
                    }

                    MainContainer.GetComponent<MainContainer>().KeyboardClamEdge();
                }

                if (Input.GetMouseButton(i)
                    && MainContainer.transform.localScale.x > Helpers.ScaleMin)
                {
                    if (touchBeganDict.ContainsKey(11 + i)
                        && touchBeganDict[11 + i] < 0)
                    {
                        continue;
                    }

                    if (_mousePositionDelta == Input.mousePosition)
                    {
                        continue;
                    }

                    var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    var posDelta = Camera.main.ScreenToWorldPoint(_mousePositionDelta);
                    //Debug.Log("touch.position " + pos + " touch.deltaPosition " + posDelta);
                    var diffX = pos.x - posDelta.x;
                    if (Mathf.Abs(deltaX) < Mathf.Abs(diffX))
                    {
                        deltaX = diffX;
                    }
                }

                if (Input.GetMouseButton(i)
                    && currentScene == 3
                    && touchBeganDict.ContainsKey(11 + i)
                    && touchBeganDict[11 + i] == -2
                    && _mousePositionDelta != Input.mousePosition)
                {
                    var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    var posDelta = Camera.main.ScreenToWorldPoint(_mousePositionDelta);
                    //Debug.Log("touch.position " + pos + " touch.deltaPosition " + posDelta);
                    var diffY = pos.y - posDelta.y;
                    if (Mathf.Abs(deltaY) < Mathf.Abs(diffY))
                    {
                        deltaY = diffY;
                    }
                }
            }

            _mousePositionDelta = Input.mousePosition;
        }
#endif

        if (currentScene != 3
            && deltaX != 0)
        {
            MainContainer.transform.Translate(new Vector3(deltaX, 0, 0));
            //Debug.Log("deltaX " + deltaX);
        }

        if (currentScene == 3
            && deltaY != 0)
        {
            mainMenu.StopPlaying();
            //var modifier = Mathf.Max(1, 1 + (Mathf.Abs(deltaY) - 0.05f) * 5);
            //Debug.Log("deltaY " + deltaY + " modifier " + modifier);
            NoteFlow.transform.Translate(new Vector3(0, deltaY * 2, 0));
            notePlayStop.ClearAll();
            PanelMeasure.UpdateSliderValue(NoteFlow.transform.localPosition.y);
            Helpers.PracticeRepeatTimeOut = DateTime.Now;
        }
    }

    void PlayPauseOnClick()
    {
        if (Helpers.IsPlaying)
        {
            mainMenu.StopPlaying();
            Debug.Log("PlayPauseOnClick mainMenu.StopPlaying()");
        }
        else
        {
            mainMenu.StartPlaying();
            Debug.Log("PlayPauseOnClick mainMenu.StartPlaying()");
        }
    }
}