using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;
[CreateAssetMenu(fileName = "Object", menuName = "object/PlayerData")] 
public class PlayerData : ObjectBase
{
    [SerializeField]
    public UpgradeData upgradeData;

    public List<int> expData = new List<int>() { 10, 20, 46, 120, 347, 902, 2074, 4148, 7052 };//필요 경험치
    public void GainExperience(int amount)
    {
        Exp += amount * (1f + (upgradeData.upgradeExpCount * upgradeData.expRatio) / 100f);
        //Debugger.Log($"current : {Exp}");
        CheckLevelUp();
    }
    public int timeCount = 0;
    public int speedCount = 0;
    public int incomeCount = 0;
    public void CopyDataValues(ref PlayerData currentData)//base + stageUpgrade 처음 객체를 불러올때만 사용?
    {
        currentData = new PlayerData();//base
        currentData.Lv = this.Lv;
        currentData.Exp = this.Exp;
        currentData.Size = this.Size;
        currentData.timeCount = this.timeCount;
        currentData.speedCount = this.speedCount;
        currentData.incomeCount = this.incomeCount;
        currentData.upgradeData = new UpgradeData();
        this.upgradeData.Load(ref currentData.upgradeData);//copy

        Managers.UpgradeManager.LoadStageUpgradeData();
        UpgradeData upgradeData = Managers.UpgradeManager.currentUpgradeData;//stage
        currentData.upgradeData.AddCount(upgradeData.upgradeLvCount, upgradeData.upgradeTimeCount, upgradeData.upgradeExpCount,upgradeData.upgradeSpeedCount,upgradeData.upgradeIncomeCount);//add stage Count
        currentData.Lv += currentData.upgradeData.upgradeLvCount;
        currentData.timeCount += currentData.upgradeData.upgradeTimeCount;
        currentData.speedCount += currentData.upgradeData.upgradeSpeedCount;//현재 카운트와 업그레이드 카운트 합?
        currentData.incomeCount += currentData.upgradeData.upgradeIncomeCount;
    }
    private void CheckLevelUp()
    {
        //int experienceNeeded = Lv * 100; //필요 경험치 잖아
        int experienceNeeded = expData[Lv];//0일 때 1Lv의 경험치량이 필요 

        while (Exp >= experienceNeeded) //
        {
            Exp -= experienceNeeded;
            Lv++;
            SoundManager.Instance.PlayAudioClip("level_up");
            experienceNeeded = expData[Lv];
        }//레벨업이 안되었을 때
    }
}
