using UnityEngine;
using UnityEngine.InputSystem;

public class AttackSystem : MonoBehaviour
{
    private Animator _animator;

    [SerializeField]
    private Transform _transform;
    
    private int _hashAttackCnt = Animator.StringToHash("AttackCnt");
    
    public enum AttackState
    {
        ATTACK_ORIGIN,

        ATTACK_END
    };

    void Start()
    {
        TryGetComponent(out _animator);

    }
    public int AttackCount
    {
        get => _animator.GetInteger(_hashAttackCnt);
        set => _animator.SetInteger(_hashAttackCnt, value);
    }
    public void OnAttack(InputValue value)
	{
		_animator.SetTrigger("Attack");
       
        _animator.SetInteger(_hashAttackCnt, 0);
        _animator.applyRootMotion = true;

	}

    // 루트 모션 적용
    void OnAnimatorMove()
    {

        Vector3 deltaPos = _animator.deltaPosition;
        Quaternion deltaRot = _animator.deltaRotation;

        _transform.position += deltaPos;
        _transform.rotation *= deltaRot;
        
    }

}
