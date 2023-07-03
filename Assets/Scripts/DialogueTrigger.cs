using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{ 
    Bounds bounds;
    Vector2 offset;
    [SerializeField] LayerMask PlayerLayer;
    [SerializeField] dialogues[] Dialogues;
    [SerializeField] faceFollow mustache;
    customController player;
    CameraFollow cam;
    Vector3 spawnpos;
    Vector2 Camlock;
    TextSystem tsystem;
    bool used = false;



    void Start() { 
        cam = FindObjectOfType<CameraFollow>();
        bounds = GetComponent<BoxCollider2D>().bounds;
        offset = GetComponent<BoxCollider2D>().offset;
        tsystem = FindObjectOfType<TextSystem>();
        spawnpos = transform.GetChild(0).position;
        Camlock = transform.GetChild(1).position;
        player = FindObjectOfType<customController>();
    }

    
    void FixedUpdate()
    {
        if(!used)
        {
            Collider2D hit = Physics2D.OverlapBox((Vector2)transform.position + offset, bounds.size, 0, PlayerLayer);

            if (hit)
            {
                StartCoroutine(StartDialogue());
                used = true;
            }
        }
       
    }

    IEnumerator StartDialogue()
    {
        player.inControl = false;
        cam.inLock = true;
        cam.targetZoomAmount = 15;
        cam.followPos = Camlock;
        for(int i = 0; i < Dialogues.Length; i++)
        {
            if(mustache)mustache.isTalking = true;
            tsystem.Type(Dialogues[i].text, spawnpos);
            yield return new WaitUntil(() => tsystem.doneWithAll);
            if (mustache) mustache.isTalking = false;
            yield return new WaitForSeconds(Dialogues[i].wait);
            tsystem.DestroyAll();
        }
        player.inControl = true;
        cam.inLock = false;
    }


}

[System.Serializable]
public class dialogues
{
    public string text;
    public float wait;
}

