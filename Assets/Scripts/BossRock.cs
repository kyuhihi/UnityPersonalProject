using System.Collections;
using UnityEngine;

public class BossRock : Bullet
{
    Rigidbody rigid;
    float angularPower =2;
    float scaleValue = 0.1f;
    bool isShot;
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());

    }

    IEnumerator GainPowerTimer(){
        yield return new WaitForSeconds(2.2f);
        isShot = true;

    }
    IEnumerator GainPower(){
        while( !isShot){
            angularPower += 0.01f;
            scaleValue += 0.005f;
            transform.localScale = Vector3.one * scaleValue;
            rigid.AddTorque(transform.right* angularPower,ForceMode.Acceleration);

            yield return null;

        }
    }

}
