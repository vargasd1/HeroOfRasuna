using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// This script is used to create the typwriter effect that happens for the dialogue.
/// 
/// ATTATCHED TO: Canvas - Dialogue (Text Box)
/// </summary>

public class BossDialogue : MonoBehaviour
{
    // TextBoxes
    public TextMeshProUGUI txt;
    public TextMeshProUGUI rasunaTxt;
    public TextMeshProUGUI englTxt;

    // Strings for text
    public string story;
    public string story2;

    // Text Property Vars
    public float textDelay = 0;
    public bool textChanged = false;
    public float textSpeed = 0.025f;
    public bool doOnce = true;

    // Cutscene Vars
    private BossCutscene cutscene;
    private BossEndCutscene endCutscene;
    public bool isFinalLine = false;

    // Start is called before the first frame update
    void Start()
    {
        // Reset Everything
        txt = GetComponent<TextMeshProUGUI>();
        story = "";
        story2 = "";
        txt.text = "";
        cutscene = FindObjectOfType<BossCutscene>();
        endCutscene = FindObjectOfType<BossEndCutscene>();
    }

    // Update is called once per frame
    void Update()
    {
        // If the text changed start the coroutine once, and if it's the last one
        // play the special text on the eng and rasuna text boxes
        if (textChanged && doOnce)
        {
            StartCoroutine(playText());
            if (isFinalLine) StartCoroutine(playTranslatedText());
            doOnce = false;
        }

        // If the text is done typing out tell the cutscenes they're ready to skip
        if (txt.text == story)
        {
            textChanged = false;
            doOnce = true;
            cutscene.readyToSkip = true;
            endCutscene.readyToSkip = true;
        }

        // If it's the final line check if the english is done typing (longer than other text box)
        if (isFinalLine && englTxt.text == story2)
        {
            endCutscene.readyToSkip = true;
        }
    }

    IEnumerator playText()
    {
        // These foreachs type out the text with some delay
        if (!isFinalLine)
        {
            foreach (char c in story)
            {
                txt.text += c;
                yield return new WaitForSecondsRealtime(textSpeed);
            }
        }
        else
        {
            foreach (char c in story)
            {
                rasunaTxt.text += c;

                yield return new WaitForSecondsRealtime(textSpeed);
            }


        }
    }

    IEnumerator playTranslatedText()
    {
        // This foreach types out the text with some delay to translate our personal font
        foreach (char c in story2)
        {
            englTxt.text += c;

            yield return new WaitForSecondsRealtime(textSpeed - 0.01f);
        }
    }
}
