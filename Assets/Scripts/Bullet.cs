using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;

    // Update is called once per frame
    void OnCollisionEnter(Collision collision)
    {
        string otherTag = collision.gameObject.tag;
        if(otherTag == "Floor"){
            Destroy(gameObject,3);

        }
    }
    void OnTriggerEnter(Collider other)
    {        
        string otherTag = other.gameObject.tag;

        if(!isMelee && otherTag == "Wall"){
            Destroy(gameObject);
        }
    }
}
