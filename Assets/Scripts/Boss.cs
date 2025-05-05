using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Boss : Enemy
{
    public GameObject missle;
    public Transform misslePortA;
    public Transform misslePortB;

    Vector3 lookVec;
    Vector3 tauntVec;
    bool isLook = true;

    protected override void Awake()
    {
        base.Awake();
        nav.isStopped =true;
        StartCoroutine(Think());
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead)
        {
            StopAllCoroutines();
            return;
        }

        if(isLook){//입력방향을 받아 예측하기
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) *5f;
            transform.LookAt(target.position + lookVec);
        }
        else
            nav.SetDestination(tauntVec);
        
    }

    IEnumerator Think(){
        yield return new WaitForSeconds(0.1f);

        int randomAction = Random.Range(0,5);
        switch (randomAction)
        {
            case 0:
            case 1://미사일발사
                StartCoroutine(MissleShot());
                break;

            case 2:
            case 3://돌굴러가유
                StartCoroutine(RockShot());
                break;

            case 4://점프공경\
                StartCoroutine(Taunt());
                break;
            default:
                break;
        }
    }

    IEnumerator MissleShot(){
        anim.SetTrigger("DoShot");
        yield return new WaitForSeconds(0.2f);
        GameObject instantMissleA = Instantiate(missle,misslePortA.position,misslePortA.rotation);
        BossMissile bossMissileA = instantMissleA.GetComponent<BossMissile>();
        bossMissileA.target = target;
        yield return new WaitForSeconds(0.3f);
        GameObject instantMissleB = Instantiate(missle,misslePortB.position,misslePortB.rotation);
        BossMissile bossMissileB = instantMissleB.GetComponent<BossMissile>();
        bossMissileB.target = target;

        yield return new WaitForSeconds(2f);

        StartCoroutine(Think());
    }

    IEnumerator RockShot(){
        isLook =false;
        anim.SetTrigger("DoBigShot");
        Instantiate(Bullet,transform.position,transform.rotation);
        
        yield return new WaitForSeconds(3f);
        isLook = true;
        StartCoroutine(Think());

    }

    IEnumerator Taunt(){
        tauntVec = target.position +lookVec;

        isLook = false;
        nav.isStopped = false;
        boxCollider.enabled =false;
        anim.SetTrigger("DoTaunt");

        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled= false;

        yield return new WaitForSeconds(1f);
        isLook =true;
        nav.isStopped = true;
        boxCollider.enabled = true;
        StartCoroutine(Think());
    }

}
