using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    // Update is called once per frame
    void OnCollisionEnter(Collision collision)
    {
        string otherTag = collision.gameObject.tag;
        if(otherTag == "Floor"){
            Destroy(gameObject,3);

        }
        else if(otherTag == "Wall"){
            Destroy(gameObject);
        }
    }
}
