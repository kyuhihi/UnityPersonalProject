using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;
    bool walkDown;

    Vector3 moveVec;

    Animator anim;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        walkDown = Input.GetButton("Walk");


        moveVec = new Vector3(hAxis,0,vAxis).normalized;
        transform.position += moveVec * speed *(walkDown? 0.3f : 1f)* Time.deltaTime;

        anim.SetBool("IsRun",moveVec!= Vector3.zero);
        anim.SetBool("IsWalk",walkDown);

        transform.LookAt(transform.position + moveVec);

    }
}
