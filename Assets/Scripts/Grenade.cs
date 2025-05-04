using System.Collections;
using UnityEngine;

public class Grenade : MonoBehaviour
{

    public GameObject meshObj;
    public GameObject effectObj;
    public Rigidbody rigid;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Explosion());
        
    }

    IEnumerator Explosion(){
        yield return new WaitForSeconds(3f);
        rigid.linearVelocity = Vector3.zero;
        rigid.angularVelocity= Vector3.zero;
        meshObj.SetActive(false);
        effectObj.SetActive(true);
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,
                                                    15f,Vector3.up,0f,
                                                    LayerMask.GetMask("Enemy"));

        foreach(RaycastHit hitObj in rayHits){
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }

        Destroy(gameObject,5f);
    }
}
