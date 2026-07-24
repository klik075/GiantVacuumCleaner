using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    #region Fields

    [SerializeField] public InputReader _inputReader;
    [SerializeField] private Rigidbody _rigidbody;
    public event Action<Vector2> OnWalkEvent;
    public event Action<Vector2> OnRunEvent;
    private Player player;

    private int _speedCount;
    [SerializeField]
    private float _speedConstant = 3f;
    [SerializeField]
    private float _rotationDuration = 0.05f;
    private Vector2 _inputVector;
    private float _movingTime = 0f;
    [SerializeField]
    private float _minimumTravelTime = 2f;
    private Vector3 lastMoveDirection = new Vector3(0,0,-1);

    public float Speed { get => Managers.UpgradeManager.upgradeList.speedList[_speedCount].value * _speedConstant * transform.localScale.x; }
    #endregion
    private void Awake()
    {
        player = GetComponent<Player>();
        _speedCount = player.CurrentData.speedCount;
    }
    private void OnEnable()
    {
        _inputReader.OnMovement -= Movement;
        _inputReader.OnMovement += Movement;
    }
    private void OnDisable()
    {
        _inputReader.OnMovement -= Movement;
    }
    private void Update()
    {
        if (Managers.UpgradeManager.IsPlaying == false)//매니저에 의존? -> 교체 대상
            return;

        if (_inputVector.magnitude > 0)
        {
            if (_movingTime <= _minimumTravelTime)
                _movingTime += Time.deltaTime;
        }
        else
        { 
            _movingTime = 0f;
        }

        CallMoveEvent(_inputVector);

        float currentYVelocity = _rigidbody.velocity.y;
        Vector3 adjustMovement = new Vector3(_inputVector.x, 0f, _inputVector.y) * Speed;
        _rigidbody.velocity = new Vector3(adjustMovement.x, currentYVelocity,adjustMovement.z);

        if (_rigidbody.velocity.y > 0)
        {
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, Mathf.Min(_rigidbody.velocity.y, 0), _rigidbody.velocity.z);
        }
        Vector3 moveDirection = adjustMovement.normalized;

        if (_inputVector != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), 0.5f);//시작, 끝 중간 지점 구하기
            transform.rotation = targetRotation;
            lastMoveDirection = moveDirection;
        }
        else
        {
            Quaternion targetRotation = Quaternion.LookRotation(lastMoveDirection);
            transform.rotation = targetRotation;
        }
    }

    private void Movement(Vector2 contextVector) => _inputVector = contextVector; 
    public void CallMoveEvent(Vector2 direction)
    {
        if (_movingTime >= _minimumTravelTime)
        { 
            OnRunEvent?.Invoke(direction);
        }
        else
        {
            OnWalkEvent?.Invoke(direction);
        }
    }
}
