using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;
    bool walkDown;
    bool jumpDown;
    bool isJump = false;
    bool isDodge = false;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigidBody;
    Animator anim;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();

    }

      private void Dodge()
    {
        if(!isJump && moveVec != Vector3.zero && jumpDown&& !isDodge){
            dodgeVec = moveVec;
            speed *= 2f;
            isDodge = true;
            anim.SetTrigger("DoDodge");

            Invoke("DodgeOut",0.5f);
        }

    }

    void DodgeOut(){
        speed *= 0.5f;
        isDodge = false;
    }

    private void Jump()
    {
        if(!isJump && moveVec == Vector3.zero && jumpDown && !isDodge){
            isJump = true;
            anim.SetBool("IsJump", true);
            anim.SetTrigger("DoJump");
            rigidBody.AddForce(Vector3.up * 15f,ForceMode.Impulse);
        }

    }


    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("IsJump", false);
            isJump = false;
        }
    }

    private void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    private void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if(isDodge)
            moveVec = dodgeVec;
        transform.position += moveVec * speed * (walkDown ? 0.3f : 1f) * Time.deltaTime;

        anim.SetBool("IsRun", moveVec != Vector3.zero);
        anim.SetBool("IsWalk", walkDown);
    }

    private void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        walkDown = Input.GetButton("Walk");
        jumpDown = Input.GetButton("Jump");

    }
}
