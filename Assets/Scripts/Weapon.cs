using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type {Melee, Range};
    public Type type;
    public int damage;
    public float rate;
    public int maxAmmo;
    public int curAmmo;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;
    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;

    public void Use()
    {
        if(type == Type.Melee){
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if(type ==Type.Range && curAmmo >0){
            --curAmmo;
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing(){
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;

    }

      IEnumerator Shot(){//instantiate�� �ν��Ͻ̵� ��ü�� �ǹ�
        GameObject instantBullet =  Instantiate(bullet, bulletPos.position,bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.linearVelocity = bulletPos.forward * 50f;

        yield return null;

        GameObject instantCase =  Instantiate(bulletCase, bulletCasePos.position,bulletCasePos.rotation);
        Rigidbody bulletCaseRigid = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3,-2) + Vector3.up * Random.Range(2,3);
        bulletCaseRigid.AddForce(caseVec,ForceMode.Impulse);
        bulletCaseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
      }
}
