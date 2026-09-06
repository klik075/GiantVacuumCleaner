using DG.Tweening;
using Scripts.Framework.Modules.DeviceModules.SoundModule;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private PlayerData playerData;
    private PlayerData currentData;
    private BoxCollider _collider;
    [SerializeField]
    private string _objectTag = "object";
    [SerializeField]
    private float changeSizeDuration;
    [SerializeField]
    private float absorptionDuration;
    [SerializeField]
    private float sizeRatio;
    [SerializeField]
    private string cleaningSoundTag = "cleaning";
    [SerializeField]
    private Billboard billboard;
    [SerializeField]
    private CognitiveRange cognitiveRange;
    [SerializeField]
    private SuckController suckController;

    private int _currentGold;
    private int _roundGold = 0;
    public int RoundGold { get => _roundGold; private set => _roundGold = value; }

    public event Action<int> GoldChanged;

    public int Gold { get => _currentGold; set => _currentGold = value; }

    public Dictionary<int,float> sizeDictionary = new Dictionary<int, float>() { {0, 0.5f},{1,1.5f},{2,2.5f},{ 3,3.5f},{4,5f }, { 5, 10f }, { 6, 25f }, { 7, 70f },{ 8,130f},{ 9,160} };

    public PlayerData CurrentData { get => currentData; private set => currentData = value; }
    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        InitData();
    }
    private void OnEnable()
    {
        Managers.UpgradeManager.UpgradeChanged -= UpdateCurrentData;
        Managers.UpgradeManager.UpgradeChanged += UpdateCurrentData;
    }
    public void UpdateCurrentData(UpgradeType type, int value)
    {
        if (type == UpgradeType.Lv) //너무 비효율적이라 인터페이스로 변경 고려
        {
            int preLv = CurrentData.Lv;
            CurrentData.upgradeData.AddCount(value, 0, 0, 0, 0);
            CurrentData.Lv = playerData.Lv + CurrentData.upgradeData.upgradeLvCount;

            if (preLv < CurrentData.Lv)
            {
                SetSize(CurrentData.Lv, changeSizeDuration);// 현재 레벨을 변경하도록?
                //Debugger.Log($"레벨{CurrentData.Lv}");
            }
            //lv 증가로 SetSize()?
        }
        else if (type == UpgradeType.Time)
        {
            CurrentData.upgradeData.AddCount(0, value, 0, 0, 0);
            CurrentData.timeCount = playerData.timeCount + CurrentData.upgradeData.upgradeTimeCount;
            Managers.UpgradeManager.currentUpgradeData.upgradeTimeCount = CurrentData.timeCount - playerData.timeCount;
        }
        else if (type == UpgradeType.Exp)
        {
            CurrentData.upgradeData.AddCount(0, 0, value, 0, 0);
            //경험치 획득량 증가 -> 이거는 바로 적용되어서 획득할 
        }
        else if (type == UpgradeType.Speed)
        {
            CurrentData.upgradeData.AddCount(0, 0, 0, value, 0);
            CurrentData.speedCount = playerData.speedCount + CurrentData.upgradeData.upgradeSpeedCount;//플레이어 강화 정보 + 스테이지 업그레이드 정보
            Managers.UpgradeManager.currentUpgradeData.upgradeSpeedCount = CurrentData.speedCount - playerData.speedCount;
        }
        else if (type == UpgradeType.Income)
        {
            CurrentData.upgradeData.AddCount(0, 0, 0, 0, value);
            CurrentData.incomeCount = playerData.incomeCount + CurrentData.upgradeData.upgradeIncomeCount;
            Managers.UpgradeManager.currentUpgradeData.upgradeIncomeCount = CurrentData.incomeCount - playerData.incomeCount;
        }

    }
    private void InitData()
    {
        _currentGold = LoadGold();

        if (playerData != null)
        {
            playerData.CopyDataValues(ref currentData);
            SetSize(CurrentData.Lv,0f);
            billboard.SetExpBar(currentData.Lv, currentData.Exp, currentData.expData[currentData.Lv]);
            //Debugger.Log($"lv : {CurrentData.Lv}, Exp : {CurrentData.Exp}, Size : {CurrentData.Size}");
        }
    }
    public void CallGoldChanged(int gold)
    {
        GoldChanged?.Invoke(gold);
    }
    public void GainExperience(int amount)
    {
        if (CurrentData != null)
        {
            CurrentData.GainExperience(amount);
            billboard.SetExpBar(currentData.Lv,currentData.Exp, currentData.expData[currentData.Lv]);
            //Debugger.Log("GainExperience");
        }
    }
    public int LoadGold()
    {
        if (PlayerPrefs.HasKey("Gold"))
        {
            return PlayerPrefs.GetInt("Gold");
        }
        else
        {
            return 0;
        }
    }
    public void SaveGold(int goldAmount)//게임 종료 후 확인 버튼을 누르면?
    {
        PlayerPrefs.SetInt("Gold", goldAmount);
        //PlayerPrefs.SetInt("Gold", 1000000000);
        PlayerPrefs.Save();
        CallGoldChanged(Gold);
    }
    public void GetGold(int amount)
    {
        if (CurrentData != null)
        {
            _roundGold += (int)(Managers.UpgradeManager.upgradeList.incomeList[CurrentData.incomeCount].value * amount);
        }
    }

    public void SetSize(int lv,float duration)
    {
        if (CurrentData != null)
        {
            //CurrentData.SetSize(gameObject,size);
            this.gameObject.transform.DOScale(sizeDictionary[lv], duration).SetEase(Ease.InQuad);
            Debugger.Log("SetSize");
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == _objectTag)
        {
            Thing thing = other.gameObject.GetComponent<Thing>();
            if (thing != null)
            {
                int myLv = CurrentData.Lv;
                if (thing.CurrentData.Lv <= myLv)
                {
                    Outline outline = other.gameObject.GetComponent<Outline>();

                    if(outline != null)
                        Destroy(outline);

                    Sequence mySequence = DOTween.Sequence();
                    Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
                    rb.isKinematic = false;
                    rb.constraints = (rb.constraints & ~RigidbodyConstraints.FreezePositionX)
                                    & (rb.constraints & ~RigidbodyConstraints.FreezePositionY)
                                    & (rb.constraints & ~RigidbodyConstraints.FreezePositionZ);

                    Collider collider = other.gameObject.GetComponent<Collider>();
                    collider.isTrigger = true;
                    SoundManager.Instance.PlayAudioClip(cleaningSoundTag);
                    mySequence.Append(rb.DOMove(transform.position + _collider.center, absorptionDuration).SetEase(Ease.Linear));//물체 이동 플레이어 중심으로
                    mySequence.Join(other.gameObject.transform.DOScale(0f, absorptionDuration).SetEase(Ease.Linear));
                    mySequence.AppendCallback(() =>
                    {
                        GainExperience((int)thing.CurrentData.Exp);
                        GetGold(thing.CurrentData.Gold);//골드 추가
                        if (myLv != CurrentData.Lv)
                        {
                            SetSize(CurrentData.Lv,changeSizeDuration);//replace -> size a
                            cognitiveRange.RefreshRange();
                            suckController.OnLevelUp();
                        }
                        //Destroy(rb.gameObject);//삭제 -> 큐에 저장 -> 보스 공격 때 dequeue();
                        Managers.ThingManager.AbsorbThing(rb.gameObject);
                    });
 
                }
            }
        }
    }
}
