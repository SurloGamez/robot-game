using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextSystem : MonoBehaviour
{
    [SerializeField] GameObject TextPrefab;
    [SerializeField] GameObject backdrop;
    [SerializeField] string Test;
    [SerializeField] TextMeshProUGUI ghost;
    public List<GameObject> toDestroy = new List<GameObject>();
    public Dictionary<string, int> specialChars = new Dictionary<string, int>();

    int textType = 0;
    bool doneTyping = false;
    public bool doneWithAll = false;
    float lastpos;


    private void Start()
    {
        specialChars.Add("/", 0); //reset
        specialChars.Add("~", 1); //wave
        specialChars.Add("#", 2); //shake
        specialChars.Add("%", 3); //size change
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Type(Test, new Vector2(10, 5));
        }
    }

    public void Type(string textInput, Vector2 pos)
    {
        toDestroy = new List<GameObject>();
        doneWithAll = false;
        StopAllCoroutines();

        lastpos = 0;

        ghost.text = "";
        int index = 0;
        while (index < textInput.Length)
        {
            if (specialChars.ContainsKey(textInput[index].ToString()))
            {
                index++;
            }
            ghost.text += textInput[index];
            index++;
        }
        ghost.ForceMeshUpdate();
        float width = ghost.textInfo.characterInfo[ghost.textInfo.characterCount - 1].topRight.x;

        GameObject parent = new GameObject();
        parent.transform.position = new Vector2(pos.x - (width / 2), pos.y);
        parent.transform.SetParent(transform);
        toDestroy.Add(parent);

        GameObject back = Instantiate(backdrop, new Vector2(pos.x - (width / 2), pos.y) + new Vector2(-1, 2.2f), transform.rotation, parent.transform);
        back.transform.GetComponent<SpriteRenderer>().size = new Vector2(width + 2f, 6);
        toDestroy.Add(back);

        StartCoroutine(loopThruSegments(textInput, parent));
    }


    IEnumerator loopThruSegments(string textInput, GameObject parent)
    {
        textType = 0;

        int currentIndex = 0;

        lastpos = 0;

        while(currentIndex < textInput.Length)
        {
            float wait = 0.02f;
            string segment = getTextSegment(ref currentIndex, textInput);

            if (textType != 0) wait = 0.05f;

            StartCoroutine(IEType(segment, parent, new Vector2(lastpos, 0), wait));
            yield return new WaitUntil(() => doneTyping);
        }
        doneWithAll = true;
       
    }

    string getTextSegment(ref int index, string textInput)
    {
        if(specialChars.ContainsKey(textInput[index].ToString()))
        {
            textType = specialChars[textInput[index].ToString()];
            index++;
        }

       

        string characters = "";

        if (textType != 0)
        {
            characters = textInput[index].ToString();
            index++;

            return characters;
        }
        while (index < textInput.Length)
        {

            if (specialChars.ContainsKey(textInput[index].ToString()))
            {
                return characters;
            }
            characters += textInput[index];

            index++;
        }
        return characters;

    }

    IEnumerator IEType(string textInput, GameObject parent, Vector2 offset, float wait) 
    {

       
        doneTyping = false;


        GameObject TextObject = Instantiate(TextPrefab, (Vector2)parent.transform.position + offset, transform.rotation, parent.transform);
        TextMeshProUGUI text = TextObject.GetComponent<TextMeshProUGUI>();
        text.text = "";

        if (textType != 0)
        {
            textType behaviour = TextObject.AddComponent<textType>();
            behaviour.type = textType;
        }

       
        for (int i = 0; i < textInput.Length; i++)
        {

            text.text += textInput[i];


            yield return new WaitForSeconds(wait);


        }


        doneTyping = true;
        lastpos += text.textInfo.characterInfo[text.textInfo.characterCount - 1].topRight.x; //local position

        if (textInput == " ") { lastpos += 0.3f; }
    }

    public void DestroyAll()
    {
        for(int i = 0; i < toDestroy.Count; i++)
        {
            Destroy(toDestroy[i]);
        }
    }
}
