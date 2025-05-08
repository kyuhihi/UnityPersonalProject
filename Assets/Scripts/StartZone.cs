using UnityEngine;

public class StartZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            GameMgr.GetInstance.StageStart();
        }
    }
}
