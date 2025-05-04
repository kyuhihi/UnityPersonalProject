using UnityEngine;
using UnityEngine.AI;
public class BossMissile : Bullet
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Transform target;
    NavMeshAgent nav;
    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        nav.SetDestination(target.position);
    }
}
