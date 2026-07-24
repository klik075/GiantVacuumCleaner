using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePannel : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public Button button3;

    public TextMeshProUGUI titleText1;
    public TextMeshProUGUI titleText2;
    public TextMeshProUGUI titleText3;

    public TextMeshProUGUI goldText1;
    public TextMeshProUGUI goldText2;
    public TextMeshProUGUI goldText3;
    public void updateText(int gold)//플레이어의 현재 골드를 받는다.
    {
        if (Managers.UpgradeManager.upgradeList == null)
            return;

        Player player = Managers.Instance.player;

        int nextSpeedCount = Managers.UpgradeManager.upgradeList.ReturnNextCount(UpgradeType.Speed, player.CurrentData.speedCount);
        int nextTimeCount = Managers.UpgradeManager.upgradeList.ReturnNextCount(UpgradeType.Time, player.CurrentData.timeCount);
        int nextIncomeCount = Managers.UpgradeManager.upgradeList.ReturnNextCount(UpgradeType.Income, player.CurrentData.incomeCount);

        int nextSpeedCost = -1;
        int nextTimeCost = -1;
        int nextIncomeCost = -1;

        if (nextSpeedCount != -1)
            nextSpeedCost = Managers.UpgradeManager.upgradeList.speedList[nextSpeedCount].cost;
        else
            Managers.UpgradeManager.ButtonInit();

        if(nextTimeCount != -1)
            nextTimeCost = Managers.UpgradeManager.upgradeList.timeList[nextTimeCount].cost;
        else
            Managers.UpgradeManager.ButtonInit();

        if (nextIncomeCount != -1)
            nextIncomeCost = Managers.UpgradeManager.upgradeList.incomeList[nextIncomeCount].cost;
        else
            Managers.UpgradeManager.ButtonInit();

        titleText1.text = string.Format($"Speed lv{nextSpeedCount}");
        titleText2.text = string.Format($"Time lv{nextTimeCount}");
        titleText3.text = string.Format($"Income lv{nextIncomeCount}");

        if (nextSpeedCost != -1)
        {
            if (nextSpeedCost <= player.Gold)
            {
                goldText1.text = string.Format($"<color=\"white\">{Managers.FormatGold(nextSpeedCost)}</color>");
                button1.interactable = true;
            }
            else
            {
                goldText1.text = string.Format($"<color=\"red\">{Managers.FormatGold(nextSpeedCost)}</color>");
                button1.interactable = false;
            }
        }
        else
        {
            goldText1.text = "";
        }
        //플레이어의 골드보다 많다면 버튼 해제
        if (nextTimeCost != -1)
        {
            if (nextTimeCost <= player.Gold)
            {
                goldText2.text = string.Format($"<color=\"white\">{Managers.FormatGold(nextTimeCost)}</color>");
                button2.interactable = true;
            }
            else
            {
                goldText2.text = string.Format($"<color=\"red\">{Managers.FormatGold(nextTimeCost)}</color>");
                button2.interactable = false;
            }
        }
        else
        {
            goldText2.text = "";
        }

        if (nextIncomeCost != -1)
        {
            if (nextIncomeCost <= player.Gold)
            {
                goldText3.text = string.Format($"<color=\"white\">{Managers.FormatGold(nextIncomeCost)}</color>");
                button3.interactable = true;
            }
            else
            {
                goldText3.text = string.Format($"<color=\"red\">{Managers.FormatGold(nextIncomeCost)}</color>");
                button3.interactable = false;
            }
        }
        else
        {
            goldText3.text = "";
        }

    }
}
