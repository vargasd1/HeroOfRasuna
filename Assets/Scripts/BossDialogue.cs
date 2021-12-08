using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossDialogue : MonoBehaviour
{

    public TextMeshProUGUI txt;
    public TextMeshProUGUI rasunaTxt;
    public TextMeshProUGUI englTxt;
    public string story;
    public string story2;
    public float textDelay = 0;
    public bool textChanged = false;
    public bool doOnce = true;
    private BossCutscene cutscene;
    private BossEndCutscene endCutscene;
    public float textSpeed = 0.025f;
    public bool isFinalLine = false;

    // Start is called before the first frame update
    void Start()
    {
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
        if (textChanged && doOnce)
        {
            StartCoroutine(playText());
            if (isFinalLine) StartCoroutine(playTranslatedText());
            doOnce = false;
        }

        if (txt.text == story)
        {
            textChanged = false;
            doOnce = true;
            cutscene.readyToSkip = true;
            endCutscene.readyToSkip = true;
        }

        if (isFinalLine && englTxt.text == story2)
        {
            endCutscene.readyToSkip = true;
        }
    }

    IEnumerator playText()
    {
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
        foreach (char c in story2)
        {
            englTxt.text += c;

            yield return new WaitForSecondsRealtime(textSpeed - 0.01f);
        }
    }
}
