using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossDialogue : MonoBehaviour
{

    public TextMeshProUGUI txt;
    public string story;
    public float textDelay = 0;
    public bool textChanged = false;
    public bool doOnce = true;
    private BossCutscene cutscene;
    public float textSpeed = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        txt = GetComponent<TextMeshProUGUI>();
        story = txt.text;
        txt.text = "";
        cutscene = FindObjectOfType<BossCutscene>();
    }

    // Update is called once per frame
    void Update()
    {
        if (textChanged && doOnce)
        {
            StartCoroutine(playText());
            doOnce = false;
        }

        if (txt.text == story)
        {
            textChanged = false;
            doOnce = true;
            cutscene.readyToSkip = true;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                textSpeed = 0.01f;
            }
        }
    }

    IEnumerator playText()
    {
        foreach (char c in story)
        {
            txt.text += c;
            yield return new WaitForSecondsRealtime(textSpeed);
        }
    }
}
