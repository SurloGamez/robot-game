using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevatorDetect : MonoBehaviour
{

    [SerializeField] bool endDoor;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("end door", endDoor);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
