using System.Collections.Generic;
using UnityEngine;

public class KeyboardCreator : MonoBehaviour
{
    public GameObject KeyWhite;
    public GameObject KeyBlack;
    public GameObject RedCarpet;

    public GameObject NoteTrigger;
    //public GameObject KeyDummy;

    private string[] Notes = {"A", "B", "C", "D", "E", "F", "G"};

    // Use this for initialization
    void Awake()
    {
        //Debug.LogWarning($"KeyboardCreator Awake");
        Helpers.KeyBlackHeight = KeyBlack.GetComponent<SpriteRenderer>().bounds.extents.y * 2;
        Helpers.KeyBlackWidth = KeyBlack.GetComponent<SpriteRenderer>().bounds.extents.x * 2;
        Helpers.KeysWhiteHeight = KeyWhite.GetComponent<SpriteRenderer>().bounds.extents.y * 2;
        Helpers.KeyWhiteWidth = KeyWhite.GetComponent<SpriteRenderer>().bounds.extents.x * 2;

        Helpers.KeysDict = new Dictionary<int, GameObject>();
        Helpers.KeysWhitesDict = new Dictionary<int, GameObject>();
        Helpers.KeysBlacksDict = new Dictionary<int, GameObject>();

        float blackKeyOffsetX = Helpers.KeyWhiteWidth * 0.06f;
        //float blackKeyOffsetY = (float) (Helpers.KeysWhiteHeight * 0.5 - Helpers.KeyBlackHeight * 0.5);
        //float blackKeyOffsetY = 0;

        //var noteTrigger = Instantiate(NoteTrigger, Vector3.zero, Quaternion.identity);
        //noteTrigger.name = "noteTrigger";
        //noteTrigger.transform.SetParent(transform, false);
        //noteTrigger.GetComponent<SpriteRenderer>().size = new Vector2(Helpers.KeysWidth, Helpers.KeysWhiteHeight);
        //noteTrigger.GetComponent<BoxCollider2D>().size = new Vector2(Helpers.KeysWidth, Helpers.KeysWhiteHeight);
        //noteTrigger.transform.localPosition = new Vector3(0, Helpers.KeysWhiteHeight * 3, 0);

        int noteIndex = 1;
        int j = 5;
        int note = 0;
        int range = 0;
        for (int i = -26; i < 26; i++)
        {
            //Debug.Log("Helpers.KeyWhiteWidth " + keyWidthandSpace + " i " + i + " result " + i * keyWidthandSpace);
            Vector2 keyWhitePosition =
                new Vector2((float) (i * Helpers.KeyWhiteWidth + Helpers.KeyWhiteWidth * 0.5), 0);
            var keyWhite = Instantiate(KeyWhite, keyWhitePosition, Quaternion.identity);
            keyWhite.transform.SetParent(transform, false);
            keyWhite.gameObject.name = range + Notes[note];
            keyWhite.gameObject.name = noteIndex.ToString();
            //Helpers.DictNoteName.Add(noteIndex, range + Notes[note]);
            //Helpers.DictNotePos.Add(noteIndex, keyWhitePosition);
            //Helpers.DictNoteWidth.Add(noteIndex, Helpers.KeyWhiteWidth);
            Helpers.KeysDict.Add(noteIndex, keyWhite);
            Helpers.KeysWhitesDict.Add(noteIndex, keyWhite);
            if (note == 2)
            {
                GameObject keyWhiteText = keyWhite.transform.GetChild(0).gameObject;
                TextMesh keyWhiteTextMesh = keyWhite.GetComponentInChildren<TextMesh>();
                keyWhiteTextMesh.text = "C" + (range - 1);

                SpriteRenderer keyWhiteRenderer = keyWhite.GetComponent<SpriteRenderer>();
                //Debug.Log(noteRenderer == null ? "noteRenderer==null" : "noteRenderer!=null");
                Renderer keyWhiteTextRenderer = keyWhiteText.GetComponent<Renderer>();
                //Debug.Log(noteTextRenderer == null ? "noteTextRenderer==null" : "noteTextRenderer!=null");
                if (keyWhiteRenderer != null
                    && keyWhiteTextRenderer != null)
                {
                    keyWhiteTextRenderer.sortingLayerID = keyWhiteRenderer.sortingLayerID;
                    keyWhiteTextRenderer.sortingOrder = keyWhiteRenderer.sortingOrder + 1;
                }

                //keyWhite.GetComponent<SpriteRenderer>().sprite = CSprite[range - 1];
            }


            //var keyDummyWhite = Instantiate(KeyDummy,
            //    keyWhitePosition + new Vector2(0, 0), Quaternion.identity);
            //keyDummyWhite.transform.SetParent(transform, false);
            //keyDummyWhite.transform.gameObject.name = "KeyDummy" + noteIndex;
            //var keyDummyWhiteSpriteRenderer = keyDummyWhite.GetComponent<SpriteRenderer>();
            //keyDummyWhiteSpriteRenderer.sortingLayerName = "WhiteMovingLayer";
            //keyDummyWhiteSpriteRenderer.size =
            //    new Vector2((float) (Helpers.KeyWhiteWidth * 0.5), .1f);
            //keyDummyWhite.GetComponent<BoxCollider2D>().size =
            //    new Vector2((float) (Helpers.KeyWhiteWidth * 0.5), .1f);

            noteIndex++;

            if (i < 25
                && j != 2
                && j != 6)
            {
                float keyAdjust = blackKeyOffsetX;

                if (j == 4)
                {
                    keyAdjust = 0;
                }
                else if (j == 0
                         || j == 3)
                {
                    keyAdjust = 0 - blackKeyOffsetX;
                }

                Vector2 keyBlackPosition =
                    new Vector2(i * Helpers.KeyWhiteWidth + Helpers.KeyWhiteWidth + keyAdjust,
                        Helpers.KeysWhiteHeight - Helpers.KeyBlackHeight);
                var keyBlack = Instantiate(KeyBlack, keyBlackPosition, Quaternion.identity);
                keyBlack.transform.SetParent(transform, false);
                keyBlack.gameObject.name = range + Notes[note] + "#";
                keyBlack.gameObject.name = noteIndex.ToString();
                //Helpers.DictNoteName.Add(noteIndex, range + Notes[note] + "#");
                //Helpers.DictNotePos.Add(noteIndex, keyBlackPosition);
                //Helpers.DictNoteWidth.Add(noteIndex, Helpers.KeyBlackWidth);
                Helpers.KeysDict.Add(noteIndex, keyBlack);
                Helpers.KeysBlacksDict.Add(noteIndex, keyBlack);

                noteIndex++;
            }

            j++;
            if (j > 6)
            {
                j = 0;
            }

            note++;
            if (note > 6)
            {
                note = 0;
            }

            if (note == 2)
            {
                range++;
            }
        }

        var redCarpet = Instantiate(RedCarpet, Vector3.zero, Quaternion.identity);
        redCarpet.transform.SetParent(transform, false);
        redCarpet.name = "Red Carpet";
        redCarpet.GetComponent<SpriteRenderer>().size = new Vector2(Helpers.KeysWidth, 0.4f);
        redCarpet.transform.localPosition = new Vector3(0, Helpers.KeysWhiteHeight, 0);
    }
}