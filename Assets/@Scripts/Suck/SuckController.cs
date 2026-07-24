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
    //private Dictionary<GameObject, Tween> objectTweens = new Dictionary<GameObject, Tween>();
    private Rigidbody _rigidbody;
    private void Awake()
    {
        _rigidbody = GetComponentInParent<Rigidbody>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == _objectTag)
        {
            GameObject objectToSuck = other.gameObject;

            //if (objectTweens.ContainsKey(objectToSuck) && objectTweens[objectToSuck].IsActive())
            //{
            //    return; 
            //}

            Thing thing = objectToSuck.GetComponent<Thing>();
            if (thing == null)
            {
                return;  
            }
            string layerName = LayerMask.LayerToName(other.gameObject.layer);

            int lvDif = _player.CurrentData.Lv - thing.CurrentData.Lv;

            Rigidbody rb = objectToSuck.GetComponent<Rigidbody>();

            if (layerName == _walllayer)//벽의 경우 동일 레벨에서 빨아들이기
            {
                lvDif--;
                
            }

            if (lvDif >= -1) // player LV Compare
            {
                if (layerName == _walllayer)
                {
                    rb.isKinematic = false;
                    rb.constraints = (rb.constraints & ~RigidbodyConstraints.FreezePositionX)
                                    & (rb.constraints & ~RigidbodyConstraints.FreezePositionY)
                                    & (rb.constraints & ~RigidbodyConstraints.FreezePositionZ);
                }

                float myMass = _rigidbody.mass;
                float targetMass = rb.mass;

                // 질량 차이에 따른 forceAmount 계산
                float massDifference = Mathf.Abs(myMass - targetMass);

                rb.AddForce((_suckPosition.position - rb.position).normalized * massDifference * baseForce);
                //Tween tween = rb.DOMove(_suckPosition.position, duration)
                //               .SetEase(Ease.OutSine)
                //               //.OnUpdate(() =>
                //               //{ 
                //               //    //rb.velocity = Vector3.zero;
                                   
                //               //})
                //               .OnComplete(() =>
                //               {
                //                   objectTweens.Remove(objectToSuck);
                //                   //rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * 1f);
                //               });
                //objectTweens[objectToSuck] = tween;

            }
        }
    }
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.tag == _objectTag)
    //    {
    //        GameObject objectToStop = other.gameObject;

    //        // 해당 객체의 Tween이 존재하면 중지하고 Dictionary에서 제거
    //        //if (objectTweens.ContainsKey(objectToStop) && objectTweens[objectToStop].IsActive())
    //        //{
    //        //    //Rigidbody rb = objectToStop.GetComponent<Rigidbody>();
    //        //    //Vector3 currentVelocity = rb.velocity;

    //        //    //objectTweens[objectToStop].Kill();  // Tween 중지
    //        //    //objectTweens.Remove(objectToStop);  // Dictionary에서 제거

    //        //    //rb.velocity = Vector3.Lerp(currentVelocity, Vector3.zero, Time.deltaTime * 2f);
    //        //}
    //    }
    //}
}
