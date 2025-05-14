using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent navMeshAgent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if(player == null){
            Debug.LogError("Player Not Found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null){
            navMeshAgent.SetDestination(player.position);
        }
    }
}
