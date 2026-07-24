using Cysharp.Threading.Tasks;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPannel : MonoBehaviour
{
    public Image rewardMainPannel;
    public Button rewardButton;
    public RewardRatioBar rewardRatioBar;
    public Image addTimePannel;
    public Button addTimeButton;
    public TextMeshProUGUI achieveText;
    public TextMeshProUGUI baseRewardText;
    

    private float currentRatio;
    private int currentRatioGold;
    private bool isTried = false;
    private bool isPlaying = false;
    public bool IsTried { get => isTried; set => isTried = value; }
    private void OnEnable()
    {
        isPlaying = true;
    }
    private void Update()
    {
        if (isPlaying == true)
        {
            if (rewardMainPannel.gameObject.activeSelf == true && rewardRatioBar.IsStop == false)
            {
                currentRatio = rewardRatioBar.ReturnRatio();
                int baseReward = Managers.Instance.player.RoundGold;
                baseRewardText.text = string.Format($"{baseReward}");
                currentRatioGold = (int)(baseReward * currentRatio);
                rewardRatioBar.UpdateGoldText(currentRatioGold);
            }
        }
        

    }
    public void GameOver()//게임 종료 시 GameOver(false)호출
    {
        //await UniTask.SwitchToMainThread();

        if (IsTried)//이미 시도 했다면
        {
            OnClickSkipAddTimeButton();
            
        }
        else//죽고 처음
        {
            rewardMainPannel.gameObject.SetActive(false);
            addTimePannel.gameObject.SetActive(true);
            IsTried = true;
        }
    }
    public async UniTask SkipAddTime()//TimePannel에서 넘기는 버튼
    {
        addTimePannel.gameObject.SetActive(false);
        rewardMainPannel.gameObject.SetActive(true);
        await rewardRatioBar.Init();
        //보상 창에서 bar Init과 진행
    }
 
    public void OnClickSkipAddTimeButton()
    {
        SkipAddTime().Forget();
    }
    public void OnClickAddTimeButton()
    {
        TimeReward();
    }
    public void TimeReward()
    {
        Debugger.Log("???");

        AdsCommand command = new(() =>
        {
            addTimePannel.gameObject.SetActive(false);
            rewardMainPannel.gameObject.SetActive(false);
            gameObject.SetActive(false);
            Managers.UpgradeManager.AddTimePlay();
        }, CommandOrderer.Reward);

        SDKIntegrationSystem.Instance.ShowReward(command);
    }

    public void SkipRewardRatio()
    {
        currentRatio = 1f;

        Player player = Managers.Instance.player;
        currentRatioGold = (int)(player.RoundGold * currentRatio);//roundGold 그대로 적용

        isPlaying = false;

        player.SaveGold(player.Gold + currentRatioGold);//현재 돈과 라운드 돈 합
        //SceneManager.LoadScene("GameScene");
        Managers.Instance.ReStart();
        //광고 시작
        //#if UNITY_EDITOR
        //        SceneManager.LoadScene("GameScene");
        //#endif
        //#if !UNITY_EDITOR
        //        Managers.Instance.ReStart();
        //#endif
    }
    public void ClickRewardButton()//리워드 광고 버튼 클릭
    {
        currentRatio = rewardRatioBar.ReturnRatio();//배율 가져오기

        //isPlaying = false;

        Player player = Managers.Instance.player;
        currentRatioGold = (int)(player.RoundGold * currentRatio);//배율 골드



        //광고 보여주기
        AdsCommand command = new(() =>
        {
            player.SaveGold(player.Gold + currentRatioGold);//현재 돈과 라운드 돈 합
            Managers.Instance.ReStart();
        }, CommandOrderer.Reward);

        SDKIntegrationSystem.Instance.ShowReward(command);
    }
}
