using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public int maxHealth;
    public int curHealth;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {

        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
        
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

