using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SuckController : MonoBehaviour
{
    [SerializeField]
    private Transform _suckPosition;
    [SerializeField]
    Player _player;
    [SerializeField]
    private string _objectTag = "object";
    [SerializeField]
    private string _walllayer = "Wall";
    [SerializeField]
    private float duration;
    [SerializeField]
    private float baseForce = 5f; // 기본 힘 값
    private Rigidbody _rigidbody;
    private Collider _collider;
    private readonly HashSet<Collider> _candidates = new HashSet<Collider>();
    private readonly HashSet<Rigidbody> _targetsInSuckRange = new HashSet<Rigidbody>();
    private void Awake()
    {
        _rigidbody = GetComponentInParent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_objectTag))
        {
            // 범위를 들어온 순간 일단 후보군에 등록
            _candidates.Add(other);

            // 레벨 조건 체크 후 흡수 대상에 추가 시도
            TryEvaluateAndAddTarget(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_objectTag))
        {
            // 범위를 나가면 후보군과 실제 흡수 대상 양쪽에서 모두 제거
            _candidates.Remove(other);

            if (other.TryGetComponent<Rigidbody>(out var rb))
            {
                _targetsInSuckRange.Remove(rb);
            }
        }
    }
    private void FixedUpdate()
    {
        // 빨아들일 대상이 없으면 빠른 리턴
        if (_targetsInSuckRange.Count == 0) return;

        // 흡수 과정 중 파괴되었거나 비활성화된 오브젝트 자동 정리
        _targetsInSuckRange.RemoveWhere(rb => rb == null || !rb.gameObject.activeInHierarchy);

        float myMass = _rigidbody.mass;

        foreach (var rb in _targetsInSuckRange)
        {
            float targetMass = rb.mass;
            float massDifference = Mathf.Abs(myMass - targetMass);

            Vector3 direction = (_suckPosition.position - rb.position).normalized;
            rb.AddForce(direction * (massDifference * baseForce), ForceMode.Force);
        }
    }

    /// <summary>
    /// 플레이어가 레벨업을 했을 때 Player 스크립트에서 호출해 주는 메서드
    /// </summary>
    public void OnLevelUp()
    {
        // 파괴된 콜라이더 정리
        _candidates.RemoveWhere(col => col == null || !col.gameObject.activeInHierarchy);

        // 범위 내 남아있는 후보군들을 순회하며 레벨 조건 재검사
        foreach (var col in _candidates)
        {
            TryEvaluateAndAddTarget(col);
        }
    }

    /// <summary>
    /// 레벨 및 레이어 조건을 검사하여 _targetsInSuckRange에 등록하는 핵심 로직
    /// </summary>
    private void TryEvaluateAndAddTarget(Collider other)
    {
        if (!other.TryGetComponent<Thing>(out var thing)) return;

        string layerName = LayerMask.LayerToName(other.gameObject.layer);
        int lvDif = _player.CurrentData.Lv - thing.CurrentData.Lv;

        if (layerName == _walllayer)
        {
            lvDif--;
        }

        // 레벨 조건 만족 시 (lvDif >= -1)
        if (lvDif >= -1)
        {
            if (other.TryGetComponent<Rigidbody>(out var rb))
            {
                // 이미 끌어당기는 중이면 중복 처리 방지
                if (_targetsInSuckRange.Contains(rb)) return;

                // 벽일 경우 물리 해제 연산
                if (layerName == _walllayer)
                {
                    rb.isKinematic = false;
                    rb.constraints &= ~RigidbodyConstraints.FreezePosition;
                }

                _targetsInSuckRange.Add(rb);
            }
        }
    }
    //private void OnTriggerStay(Collider other) 이전 사용 방식
    //{
    //    if (other.tag == _objectTag)
    //    {
    //        GameObject objectToSuck = other.gameObject;

    //        Thing thing = objectToSuck.GetComponent<Thing>();
    //        if (thing == null)
    //        {
    //            return;  
    //        }
    //        string layerName = LayerMask.LayerToName(other.gameObject.layer);

    //        int lvDif = _player.CurrentData.Lv - thing.CurrentData.Lv;

    //        Rigidbody rb = objectToSuck.GetComponent<Rigidbody>();

    //        if (layerName == _walllayer)//벽의 경우 동일 레벨에서 빨아들이기
    //        {
    //            lvDif--;

    //        }

    //        if (lvDif >= -1) // player LV Compare
    //        {
    //            if (layerName == _walllayer)
    //            {
    //                rb.isKinematic = false;
    //                rb.constraints = (rb.constraints & ~RigidbodyConstraints.FreezePositionX)
    //                                & (rb.constraints & ~RigidbodyConstraints.FreezePositionY)
    //                                & (rb.constraints & ~RigidbodyConstraints.FreezePositionZ);
    //            }

    //            float myMass = _rigidbody.mass;
    //            float targetMass = rb.mass;

    //            // 질량 차이에 따른 forceAmount 계산
    //            float massDifference = Mathf.Abs(myMass - targetMass);

    //            rb.AddForce((_suckPosition.position - rb.position).normalized * massDifference * baseForce);
    //        }
    //    }
    //}
}
