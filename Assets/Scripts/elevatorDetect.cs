using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class elevatorDetect : MonoBehaviour
{

    [SerializeField] bool endDoor;
    Animator anim;
    Vector2 offset;
    Vector2 size;
    [SerializeField] LayerMask playerLayer;
    bool used = false;
    CameraFollow cam;
    int currLevel;
    customController player;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("end door", endDoor);
        cam = FindObjectOfType<CameraFollow>();
        currLevel = SceneManager.GetActiveScene().buildIndex;
        
        player = FindObjectOfType<customController>();

        if (endDoor)
        {
            transform.position += Vector3.forward * 0.4f;
            offset = GetComponent<BoxCollider2D>().offset;
            size = GetComponent<BoxCollider2D>().size;
           
        }
        else
        {
            player.inControl = false;
            transform.position += Vector3.forward * -0.4f;
            StartCoroutine(openDoor());
        }


    }

   
    void FixedUpdate()
    {
        if(endDoor)
        {
            checkForPlayer();
        }
       
    }

    void checkForPlayer()
    {
        if (used) return;

        Collider2D hit = Physics2D.OverlapBox((Vector2)transform.position + offset, size, 0, playerLayer);
        if (hit)
        {
            transform.position -= Vector3.forward * 0.5f;
           
            player.inControl = false;
            player.smoothInput = Vector2.zero;
            used = true;
            anim.SetBool("touchingPlayer", true);
            StartCoroutine(nextLevelTransition());
        }
    }

    IEnumerator nextLevelTransition()
    {

        yield return new WaitForSeconds(1);
        cam.nextLevelTransition();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(currLevel + 1);
    }

    IEnumerator openDoor()
    {
        yield return new WaitForSeconds(2);
        transform.position += Vector3.forward * 0.5f;
        player.inControl = true;
    }
}
