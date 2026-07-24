using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineTransposer transposer;

    [SerializeField]
    private float baseOffset = 5f;

    public Transform player;
    private void Start()
    {
        Init();
    }
    private void Update()
    {
        ChangeFollowOffset();
    }
    private void ChangeFollowOffset()
    {
        if (transposer == null)
            return;

        float targetSize = player.localScale.x;
        transposer.m_FollowOffset = new Vector3(0f, targetSize * baseOffset, -targetSize * baseOffset);

    }
    public void Init()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();

        if (virtualCamera == null)
            return;

        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        ChangeFollowOffset();
    }
    //public Transform target;
    //public float initialDistance;  // 초기 카메라 거리

    //[SerializeField]
    //private Vector3 initVector = new Vector3(0f, 8f, -10f);
    //private Vector3 offset;

    //private void Awake()
    //{
    //    Init();
    //}

    //private void Init()
    //{
    //    transform.position = target.position + initVector;
    //    offset = transform.position - target.position;
    //}

    //// Update is called once per frame
    //void LateUpdate()
    //{
    //    float targetSize = target.localScale.magnitude;
    //    Vector3 newPosition = target.position + offset.normalized * (initialDistance * targetSize);
    //    transform.position = newPosition;//
    //    transform.LookAt(target);
    //}
}
