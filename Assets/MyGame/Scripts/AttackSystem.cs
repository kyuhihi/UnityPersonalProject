using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class AttackSystem : MonoBehaviour
{
    private Animator _animator;

    [SerializeField]
    private Transform _transform;

    [SerializeField]
    private float _attackForce = 5f;

    private Rigidbody _rigidbody;

    private int _hashAttackCnt;

    private bool _isFocusing = false;
    private GameObject _focusTarget = null;

    public enum AttackState
    {
        ATTACK_ORIGIN,
        ATTACK_END
    };

    void Awake()
    {
        _hashAttackCnt = Animator.StringToHash("AttackCnt");
    }
    void Start()
    {
        TryGetComponent(out _animator);
        TryGetComponent(out _rigidbody);
    }

    public void OnAttack(InputValue value)
    {
        _animator.SetTrigger("Attack");
        _animator.SetInteger(_hashAttackCnt, 0);
        _animator.applyRootMotion = true;
    }

    public void OnFocus(InputValue value)
    {
        if (value.isPressed)
        {
            StartFocus();
        }
        else
        {
            ReleaseFocus();
        }
    }

    // 포커스 시작
    private void StartFocus()
    {
        List<GameObject> candidates = GridManager.Instance.GetNearbyObjects(transform.position);
        GameObject nearest = null;
        float minSqrDist = float.MaxValue;
        Vector3 myPos = transform.position;

        foreach (var obj in candidates)
        {
            if (obj == gameObject) continue;
            if (!obj.CompareTag("FocusTarget")) continue;

            float sqrDist = (obj.transform.position - myPos).sqrMagnitude;
            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                nearest = obj;
            }
        }

        if (nearest == null)
        {
            _isFocusing = false;
            _focusTarget = null;
            return;
        }

        _isFocusing = true;
        _focusTarget = nearest;
        Debug.Log($"가장 가까운 FocusTarget: {nearest.name}");
    }

    // 포커스 해제
    private void ReleaseFocus()
    {
        _isFocusing = false;
        _focusTarget = null;
    }

    // Move 함수에서 참고할 수 있도록 포커스 상태와 타겟을 외부에서 접근 가능하게 프로퍼티로 제공
    public bool IsFocusing => _isFocusing;
    public GameObject FocusTarget => _focusTarget;

    // 루트 모션 적용
    void OnAnimatorMove()
    {
        Vector3 deltaPos = _animator.deltaPosition;
        Quaternion deltaRot = _animator.deltaRotation;

        _transform.position += deltaPos;
        _transform.rotation *= deltaRot;
    }

    public void AttakForceEvent()
    {
        Debug.Log("Attakck Event");
        _rigidbody.AddForce(_transform.forward * _attackForce, ForceMode.Impulse);
    }
}
