using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CognitiveRange : MonoBehaviour
{
    private Player player;
    private Collider rangeCollider;
    private void Awake()
    {
        player = gameObject.GetComponentInParent<Player>();
        rangeCollider = GetComponent<Collider>();
    }
    //private void OnTriggerStay(Collider other) 기존 사용 방식
    //{
    //    if (other.CompareTag("object"))
    //    {
    //        Thing thing = other.GetComponent<Thing>();
    //        if (thing == null)
    //            return;
    //        int lvDif = player.CurrentData.Lv - thing.CurrentData.Lv;
    //        if (lvDif >= 0)
    //        {
    //            Outline outline = other.GetComponent<Outline>();
    //            if(outline != null)
    //                outline.enabled = true;    
    //        }
    //    }
    //}
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("object"))
        {
            if (other.TryGetComponent<Thing>(out var thing))
            {
                // 내 레벨이 물체 레벨 이상이면 아웃라인 켬
                if (player.CurrentData.Lv >= thing.CurrentData.Lv)
                {
                    if (other.TryGetComponent<Outline>(out var outline))
                    {
                        outline.enabled = true;
                    }
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("object")) 
        {
            Outline outline = other.GetComponent<Outline>();
            if (outline != null && outline.enabled == true)
                outline.enabled = false;
        }
    }
    public void RefreshRange()
    {
        StartCoroutine(RoutineRefresh());
    }

    private IEnumerator RoutineRefresh()
    {
        // 1. 콜라이더를 끄면 감지 중이던 물체들에게 OnTriggerExit가 발생함 (아웃라인 꺼짐)
        rangeCollider.enabled = false;

        // 2. 물리 엔진이  꺼진 상태를 인지할 수 있도록 한 프레임(또는 FixedUpdate) 대기
        yield return new WaitForFixedUpdate();

        // 3. 다시 켜면 새로 변경된 레벨 기준으로 OnTriggerEnter가 발생함 (먹을 수 있게 된 물체 아웃라인 켜짐)
        rangeCollider.enabled = true;
    }
}
