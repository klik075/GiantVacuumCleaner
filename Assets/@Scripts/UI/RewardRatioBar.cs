using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardRatioBar : MonoBehaviour
{
    public TextMeshProUGUI ratioGoldText;
    public float rotationSpeed = 160f; // 초당 회전 속도
    public float angleChangeRate = 0.9f; // 회전 속도 감소율

    [SerializeField]
    private float minAngle = 80f;
    [SerializeField]
    private float oneAngle = 40f;
    [SerializeField]
    private float twoAngle = -10f;
    [SerializeField]
    private float threeAngle = -55f;
    [SerializeField]
    private float maxAngle = -80f;

    private float currentAngle = 0f;
    private bool isRotatingClockwise = true;
    private Coroutine rotateCoroutine;
    private bool isStop = false;
    public bool IsStop { get => isStop; private set => isStop = value; }

    // Update is called once per frame
    //void Update()
    //{
    //    if(!isStop)
    //        Rotate();
    //}
    public async UniTask Init()
    {
        currentAngle = minAngle;
        isRotatingClockwise = true;
        IsStop = false;
        await Rotate();
    }
    private void OnDisable()
    {
        IsStop = true;
    }
    public float ReturnRatio()
    {
        if (minAngle >= currentAngle && currentAngle >= oneAngle)
            return 1.5f; // 변수로 나중에 처리
        else if (oneAngle > currentAngle && currentAngle >= twoAngle)
            return 2f;
        else if (twoAngle > currentAngle && currentAngle >= threeAngle)
            return 2.5f;
        else
            return 3f;
    }
    //private void Rotate()
    //{
    //    if (isRotatingClockwise)
    //    {
    //        currentAngle -= rotationSpeed * Time.deltaTime;
    //    }
    //    else
    //    {
    //        currentAngle += rotationSpeed * Time.deltaTime;
    //    }

    //    transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);

    //    if (currentAngle >= minAngle)
    //        isRotatingClockwise = true;
    //    else if(currentAngle <= maxAngle)
    //        isRotatingClockwise = false;

    //}
    private async UniTask Rotate()
    {
        try
        {
            while (!IsStop)
            {
                if (isRotatingClockwise)
                {
                    currentAngle -= rotationSpeed * Time.unscaledDeltaTime;
                }
                else
                {
                    currentAngle += rotationSpeed * Time.unscaledDeltaTime;
                }

                transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);

                if (currentAngle >= minAngle)
                    isRotatingClockwise = true;
                else if (currentAngle <= maxAngle)
                    isRotatingClockwise = false;

                await UniTask.Yield();
            }
        }
        catch (System.Exception ex)
        {
            Debugger.LogError($"Rotate 메서드에서 오류 발생: {ex.Message}");
        }
    }
    public void SetBoolIsStop(bool boolean)
    {
        isStop = boolean;
    }

    public void UpdateGoldText(int gold)
    {
        ratioGoldText.text = string.Format($"+{gold}");
    }
}
