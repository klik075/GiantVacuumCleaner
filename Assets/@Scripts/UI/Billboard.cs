using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    [SerializeField]
    private Slider expBar;
    [SerializeField]
    private TextMeshProUGUI currentExpText;
    [SerializeField]
    private TextMeshProUGUI totalExpText;
    [SerializeField]
    private TextMeshProUGUI levelText;

    void Start()
    {
        mainCamera = Camera.main;
        expBar.value = 0;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
    }
    public void Init(int level, float currentExp, float totalExp)
    {
        levelText.text = string.Format($"Lv{level + 1}");
        currentExpText.text = ((int)currentExp).ToString();
        totalExpText.text = ((int)totalExp).ToString();
    }
    public void SetExpBar(int level,float currentExp, int totalExp) // 현재와 전체를 받음
    {
        Init(level, currentExp, totalExp);
        expBar.value = currentExp / (float)totalExp;
    }
    //현재 exp / 지금 레벨 최대 경험치
}