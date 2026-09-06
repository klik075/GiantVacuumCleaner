using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    public PrefabGroup groupName;
    public float spawnChance;
    private BoxCollider boxCollider;

    public float distanceBetweenCheck;
    public float heightOfCheck;
    public float rangeOfCheck;
    public LayerMask allLayerMask;
    public LayerMask targetMask;

    private List<GameObject> PrefabList;
    private Vector2 positivePosition, negativePosition;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        CheckSize();
    }
    private void OnEnable()
    {
        Managers.PrefabLoader.EndInit -= SetObject;
        Managers.PrefabLoader.EndInit += SetObject;
    }
    public void CheckSize()
    {
        if (boxCollider != null) 
        {
            float x = boxCollider.size.x / 2f;
            float y = boxCollider.size.z / 2f;
            positivePosition = new Vector2(transform.position.x + x,transform.position.z + y);
            negativePosition = new Vector2(transform.position.x - x,transform.position.z - y);
        }
    }
    public void SetObject()
    {
        PrefabList = Managers.PrefabLoader.GetGroupObject(groupName.ToString());
        SpawnResources();
    }
    private void SpawnResources()
    {
        if(positivePosition == null && negativePosition == null)
            return;

        for (float x = negativePosition.x; x <= positivePosition.x; x += distanceBetweenCheck)
        {
            for (float z = negativePosition.y; z <= positivePosition.y; z += distanceBetweenCheck)
            {
                RaycastHit hit;
                if (Physics.Raycast(new Vector3(x, heightOfCheck, z), Vector3.down, out hit, rangeOfCheck, allLayerMask))//thing이 없을 때만
                {
                    int hitMask = hit.collider.gameObject.layer;
                    //Debugger.Log($"{hit.collider.gameObject} : 5");
                    if (1 << hitMask != targetMask)
                    {
                        //Debugger.Log($"{hitMask} {targetMask}");
                        if (spawnChance > Random.Range(0, 100)) // 0 ~ 99
                        {
                            GameObject randomObject = GetRandomObject();
                            BoxCollider collider = randomObject.GetComponent<BoxCollider>();

                            if (collider != null)
                            {
                                // 콜라이더 중심만큼 올리기
                                Vector3 offset = new Vector3(0, collider.bounds.extents.y, 0);
                                Vector3 spawnPosition = hit.point + offset;

                                Instantiate(randomObject, spawnPosition, Quaternion.identity, transform);
                            }
                            else
                            {
                                // 콜라이더가 없을 경우 기본 위치에 생성
                                Instantiate(randomObject, hit.point, Quaternion.identity, transform);
                            }
                        }
                    }
                }
            }
        }
    }
    private GameObject GetRandomObject()
    {
        int num = PrefabList.Count;
        int randomIndex = Random.Range(0, num);
        return PrefabList[randomIndex];
    }
}
