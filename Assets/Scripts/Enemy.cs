using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { ENEMY_A,ENEMY_B,ENEMY_C,ENEMY_END};
    public Type eType;

    public int maxHealth;
    public int curHealth;

    public Transform target;

    public BoxCollider meleeArea;
    public GameObject Bullet;

    bool isChase = false;
    bool isAttack = false;


    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;
    NavMeshAgent nav;
    Animator anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {

        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        Invoke("ChaseStart",2);
    }

    void FreezeVelocity(){
        if(isChase){
            rigid.linearVelocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }


    }

    void Targeting(){
        float targetRadius= 0f;
        float targetRange = 0f;

        switch (eType)
        {
            case Type.ENEMY_A:
                targetRadius = 1.5f;
                targetRange = 3f;            
                break;
            case Type.ENEMY_B:
                targetRadius = 1f;
                targetRange = 12f;                
                break;

            case Type.ENEMY_C:
   
                targetRadius = 0.5f; 
                targetRange = 25f;            
                break;
            default:
                break;
        }
        Debug.DrawRay(transform.position, transform.forward * targetRange , Color.green);

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,
                                                    targetRadius,transform.forward,
                                                    targetRange,
                                                    LayerMask.GetMask("Player"));

        Debug.Log("rayHits.Length? " +rayHits.Length);

        if(rayHits.Length >0 && !isAttack){
            //Time.timeScale = 0f;
            StartCoroutine(Attack());
        }


    }
    IEnumerator Attack(){
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack",true);

        switch (eType)
        {
            case Type.ENEMY_A:

                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false; 

                yield return new WaitForSeconds(1f);            
                break;
            case Type.ENEMY_B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20f, ForceMode.Impulse);
                meleeArea.enabled =true;
                
                yield return new WaitForSeconds(0.5f);
                rigid.linearVelocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2f);

                break;            
            case Type.ENEMY_C:
                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = Instantiate(Bullet,transform.position,transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.linearVelocity = transform.forward *20f;
                Debug.Log("Call?");
                yield return new WaitForSeconds(2f);

        
                break;            
            default:
                break;
        }

        
        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack",false);       
    }

    public void FixedUpdate()
    {
        FreezeVelocity();
        Targeting();
    }

    void ChaseStart(){
        isChase = true;
        anim.SetBool("isWalk",true); 
    }
    void Update()
    {
        if(nav.enabled){
            nav.SetDestination(target.position);
            nav. isStopped = !isChase;
        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee"){
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Debug.Log("Melleeee " + curHealth);
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec,false));

        }
        else if( other.tag == "Bullet"){
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;

            Debug.Log("Bullet " + curHealth);
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);
            StartCoroutine(OnDamage(reactVec,false));
        }
        
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade){
        mat.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        if(curHealth >0){
            mat.color = Color.white;
        }
        else{
            mat.color = Color.gray;
            gameObject.layer = 12;
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("DoDie");

            if(isGrenade){
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3f;
                rigid.freezeRotation =false;
                rigid.AddForce(reactVec * 5f ,ForceMode.Impulse);
                rigid.AddTorque(reactVec* 15f, ForceMode.Impulse);
            }
            else 
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                rigid.AddForce(reactVec *5f ,ForceMode.Impulse);
            }

           
            Destroy(gameObject,4);
        }
    }

    public void HitByGrenade(Vector3 GrenadePos){
        curHealth -= 100;
        Debug.Log("HitByGrenade " + curHealth);
        Vector3 reactVec = transform.position - GrenadePos;
         
        StartCoroutine(OnDamage(reactVec,true));
    }
}

