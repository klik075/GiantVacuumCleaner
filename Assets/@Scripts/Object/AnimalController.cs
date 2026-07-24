using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour
{
    public Animator animator; // Animator 컴포넌트
    public Rigidbody rigidBody;
    public float currentSpeed = 2f;  // 동물 이동 속도
    public float idleSpeed = 2f;  // 동물 이동 속도
    public float runSpeed = 6f;  // 동물 이동 속도

    public float changeDirectionTime = 3f; // 방향 변경 주기
    private Vector3 currentDirection; // 현재 이동 방향
    private float timer; // 타이머
    public bool isWalking = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        ChangeDirection(); // 초기 방향 설정
        //플레이어가 가까이 왔을 때(인지 trigger에 닿았을 때) 반대 방향으로 도망치기
    }
    void Update()
    {
        if (isWalking == false)
        {
            // 동물 이동
            rigidBody.velocity = currentDirection * currentSpeed;
            //transform.position += currentDirection * speed * Time.deltaTime;

            // 타이머 업데이트
            timer += Time.deltaTime;
            if (timer >= changeDirectionTime)
            {
                ChangeDirection(); // 방향 변경
                timer = 0; // 타이머 초기화
            }
        }
        else
        {
            rigidBody.velocity = currentDirection * currentSpeed;
        }
    }

    void ChangeDirection()
    {
        int rnd = Random.Range(0, 10);
        int rndTime;

        if (rnd > 2)
        {
            rndTime = Random.Range(1,4);
            // 랜덤한 방향 설정
            float randomX = Random.Range(-1f, 1f);
            float randomZ = Random.Range(-1f, 1f);
            currentDirection = new Vector3(randomX, 0, randomZ).normalized;

            // 방향에 따라 회전
            Quaternion targetRotation = Quaternion.LookRotation(currentDirection);
            transform.rotation = targetRotation;
        }
        else//가만히 있기
        {
            rndTime = Random.Range(1, 2);
            currentDirection = Vector3.zero;
        }
        changeDirectionTime = rndTime;
    }
    public void IsWalking(Vector3 vector3)//인지범위에 플레이어가 들어왔을 때
    {
        if(isWalking == false)
        {
            currentSpeed = runSpeed;
            animator.SetBool("IsWalking", true);
        }

        isWalking = true;
        currentDirection = new Vector3(vector3.x,0,vector3.z).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(currentDirection);
        transform.rotation = targetRotation;
    }
    public void IsIdle()//인지범위에 플레이어가 없을 때
    {
        currentSpeed = idleSpeed;
        animator.SetBool("IsWalking", false);
        isWalking = false;
        ChangeDirection();
    }
}
