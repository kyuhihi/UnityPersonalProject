using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform target;
    public float orbitSpeed;
    Vector3 offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        offset = transform.position - target.position;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + offset;
        transform.RotateAround(target.position,
                                Vector3.up,
                                orbitSpeed * Time.deltaTime);
        offset = transform.position - target.position;

    }
}
