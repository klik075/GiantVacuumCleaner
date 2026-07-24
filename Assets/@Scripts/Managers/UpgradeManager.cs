using Cysharp.Threading.Tasks;
using Scripts.Framework.Managers.Asset.Core;
using Scripts.Framework.Modules.DeviceModules.SoundModule;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum UpgradeType
{
    Lv,
    Time,
    Exp,
    Income,
    Speed
}
[Serializable]
public class UpgradeData
{
    public int upgradeLvCount;//
    public int upgradeExpCount;//
    public int upgradeTimeCount;
    public int upgradeSpeedCount;
    public int upgradeIncomeCount;

    public int lvRatio;
    public int timeRatio;
    public int expRatio;
    public int maxCount;
    public UpgradeData(int lvCount = 0,int timeCount = 0,int expCount = 0,int speedCount=0,int incomeCount = 0)
    {
        upgradeLvCount = lvCount;
        upgradeTimeCount = timeCount;
        upgradeExpCount = expCount;
        upgradeSpeedCount = speedCount;
        upgradeIncomeCount = incomeCount;
        lvRatio = 1;
        timeRatio = 2;
        expRatio = 5;
        maxCount = 20;
    }
    public void Load(ref UpgradeData upgradeData)
    {
        upgradeData.upgradeLvCount = this.upgradeLvCount;
        upgradeData.upgradeTimeCount = this.upgradeTimeCount;
        upgradeData.upgradeExpCount = this.upgradeExpCount;
        upgradeData.upgradeSpeedCount = this.upgradeSpeedCount;
        upgradeData.upgradeIncomeCount = this.upgradeIncomeCount;
        upgradeData.lvRatio = this.lvRatio;
        upgradeData.timeRatio = this.timeRatio;
        upgradeData.expRatio = this.expRatio;
    }
    public void Save(UpgradeData upgradeData)
    {
        upgradeLvCount = upgradeData.upgradeLvCount;
        upgradeTimeCount = upgradeData.upgradeTimeCount;
        upgradeExpCount = upgradeData.upgradeExpCount;
        upgradeSpeedCount = upgradeData.upgradeSpeedCount;
        upgradeIncomeCount = upgradeData.upgradeIncomeCount;
        lvRatio = upgradeData.lvRatio;
        timeRatio = upgradeData.timeRatio;
        expRatio = upgradeData.expRatio;
    }
    public void AddCount(int lvCount, int timeCount,int expCount,int speedCount,int incomeCcount )
    {
        upgradeLvCount += lvCount;
        upgradeTimeCount += timeCount;
        upgradeExpCount += expCount;
        upgradeSpeedCount += speedCount;
        upgradeIncomeCount += incomeCcount;
        upgradeLvCount = Mathf.Clamp(upgradeLvCount, 0, maxCount);
        upgradeExpCount = Mathf.Clamp(upgradeExpCount, 0, maxCount);
    }
    public void SaveToPlayerPrefs(string key)
    {
        string json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
        Debugger.Log("저장 성공");
    }

    public static UpgradeData LoadFromPlayerPrefs(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            string json = PlayerPrefs.GetString(key);
            Debugger.Log("로드 성공");
            return JsonUtility.FromJson<UpgradeData>(json);
        }
        Debugger.Log("기본 로드");
        return new UpgradeData(); // 기본값 반환
    }
}
public class UpgradeManager
{
    [SerializeField]
    private float _baseTimeLimit = 60f;//맵의 기본 시간 데이터로 할당?
    [SerializeField]
    private float _currentTimeLimit = 0;
    [SerializeField]
    private string _timeTextTag = "timeText";
    [SerializeField]
    private string _upgradePannelTag = "UpgradePannel";
    [SerializeField]
    private string _retryButtonTag = "RetryButton";
    [SerializeField]
    private string _startButtonTag = "StartButton";
    [SerializeField]
    private string _lobyTag = "Lobby";
    [SerializeField]
    private string _goldTextTag = "GoldText";
    [SerializeField]
    private string _gameOverTag = "GameOver";
    [SerializeField]
    private string _uICanvasPath = "UICanvas";
    [SerializeField]
    private float _redTime = 5f;
    [SerializeField]
    private string startSoundTag = "start_game";
    [SerializeField]
    private string buttonUpgradeSoundTag = "button_upgrade";

    public UpgradePannel upgradePannel;
    public Image lobby;
    public GameOverPannel gameOverPannel;
    public Button timePannel;
    public Button startBtn;
    public Canvas uiCanvas;
    private Button[] upgradeButtons;

    private TextMeshProUGUI _timeText;
    private TextMeshProUGUI _goldText;
    private bool _isPlaying = false;
    public bool IsPlaying {  get { return _isPlaying; } }

    public UpgradeData currentUpgradeData;
    
    public event Action<UpgradeType, int> UpgradeChanged;

    public UpgradeList upgradeList;
    private Player player;
    private int secretCount = 0;
    // Start is called before the first frame update

    // Update is called once per frame
    public void Update()
    {
        if (_isPlaying == true)
        {
            _currentTimeLimit -= Time.deltaTime;

            if (_currentTimeLimit > 0f)
                UpdateTimeText();
            else
            {
                _currentTimeLimit = 0f;
                SetGameState(false);
                TimeOver();
                //retryBtn.gameObject.SetActive(true);
            }
        }
    }
    
    //void OnApplicationQuit()
    //{
    //    currentUpgradeData.SaveToPlayerPrefs("UpgradeData");
    //}
    public void OnDestroy()
    {
        currentUpgradeData.SaveToPlayerPrefs("UpgradeData");
    }
    public void LoadStageUpgradeData()
    {
        currentUpgradeData = UpgradeData.LoadFromPlayerPrefs("UpgradeData");
    }
    public async UniTask LoadUpgradeList()
    {
        var handle = Addressables.LoadAssetAsync<UpgradeList>("UpgradeList");
        await handle.Task.AsUniTask();

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            upgradeList = handle.Result;
            InitTimeLimit();
            upgradePannel.updateText(player.Gold);//플레이어의 골드로 버튼 텍스트들 적용
            if (upgradeButtons != null)
                ButtonInit();

        }
        else
        {
            Debugger.LogError("Failed to load PrefabAddressList.");
        }
    }
    //public void ClickLvButton()//밖에서 구매가 가능한지 체크 혹은 광고로 대체?
    //{
    //    CallUpgradeChanged(UpgradeType.Lv,currentUpgradeData.lvRatio);
    //}
    //public void ClickExpButton()
    //{
    //    //플레이어의 돈 계산
    //    CallUpgradeChanged(UpgradeType.Exp, currentUpgradeData.expRatio);
    //}
    public void ClickSpeedButton()//밖에서 구매가 가능한지 체크 혹은 광고로 대체?
    {
        if (player.CurrentData.speedCount >= upgradeList.speedList.Count)//스피드가 맥스보다 많을 때
        {
            upgradeButtons[0].gameObject.SetActive(false);
            return;
        }

        int? cost = upgradeList.speedList[player.CurrentData.speedCount + 1].cost;
        if (cost == null)
            return;

        if (cost <= player.Gold)
        {
            player.Gold -= cost.Value;
            CallUpgradeChanged(UpgradeType.Speed, 1);
            player.SaveGold(player.Gold);
            SoundManager.Instance.PlayAudioClip(buttonUpgradeSoundTag);
        }
        else
        {
            Debugger.Log("골드가 부족합니다.");
            return;
        }
        //플레이어의 돈 계산
       
    }
    public void ClickTimeButton()
    {
        if (player.CurrentData.timeCount >= upgradeList.timeList.Count)//스피드가 맥스보다 많을 때
        {
            upgradeButtons[1].gameObject.SetActive(false);
            return;
        }

        //플레이어의 돈 계산
        int? cost = upgradeList.timeList[player.CurrentData.timeCount+1].cost;
        if (cost == null)
            return;

        if (cost <= player.Gold)
        {
            player.Gold -= cost.Value;
            CallUpgradeChanged(UpgradeType.Time, 1);
            player.SaveGold(player.Gold);
            SoundManager.Instance.PlayAudioClip(buttonUpgradeSoundTag);
        }
        else
        {
            Debugger.Log("골드가 부족합니다.");
            return;
        }
    }
    public void ClickIncomeButton()//밖에서 구매가 가능한지 체크 혹은 광고로 대체?
    {
        if (player.CurrentData.incomeCount >= upgradeList.incomeList.Count)//스피드가 맥스보다 많을 때
        {
            upgradeButtons[2].gameObject.SetActive(false);
            return;
        }

        int? cost = upgradeList.incomeList[player.CurrentData.incomeCount + 1].cost;
        if (cost == null)
            return;

        if (cost <= player.Gold)
        {
            player.Gold -= cost.Value;
            CallUpgradeChanged(UpgradeType.Income, 1);
            player.SaveGold(player.Gold);
            SoundManager.Instance.PlayAudioClip(buttonUpgradeSoundTag);
        }
        else
        {
            Debugger.Log("골드가 부족합니다.");
            return;
        }
        //플레이어의 돈 계산
       
    }
    public void CallUpgradeChanged(UpgradeType type, int value)//스테이지 업그레이드 버튼을 눌렀을 때?
    {
        if (type == UpgradeType.Time)
        {
            UpgradeChanged?.Invoke(type, value);//1증가
            InitTimeLimit();//현재 데이터로 시간 설정
        }
        else
        { 
            UpgradeChanged?.Invoke(type, value);
        }
    }
    
    public void Init()
    {
        _isPlaying = false;
        player = Managers.Instance.player;
        player.GoldChanged -= UpdateGoldText;
        player.GoldChanged += UpdateGoldText;

        LoadUpgradeList().Forget();
        AllocateFields();
        SetGameState(false);
        player.CallGoldChanged(player.Gold);
    }
    public void TimeOver()
    {
        gameOverPannel.gameObject.SetActive(true);//켜고
        Time.timeScale = 0;
        gameOverPannel.GameOver();
    }
    public void AddTimePlay()
    {
        AddTime(10f);
        Time.timeScale = 1;
        SetGameState(true);
        //await UniTask.Delay(10000);

        //gameOverPannel.GameOver(); 
    }
    public void SetGameState(bool state)
    {
        _isPlaying = state;
        if (_isPlaying == true)
        {
            SoundManager.Instance.PlayAudioClip(startSoundTag);
        }
    }
    public void StartGame()
    {
        SetGameState(true);
        lobby.gameObject.SetActive(!_isPlaying);
        Time.timeScale = 1;
        player.GetComponentInChildren<SuckController>().enabled = _isPlaying;
    }

    public void InitTimeLimit()//현재 플레이어 타임 강화정도로 설정
    {
        _currentTimeLimit = upgradeList.timeList[player.CurrentData.timeCount].value;//기본 시간 설정
        UpdateTimeText();
        //플레이어 능력치에 따른 설정(스킨과 연관)
        //업그레이드에 따른 설정
    }
    public void AddTime(float time)
    {
        _currentTimeLimit += time;
        UpdateTimeText();
    }
    public void UpdateGoldText(int gold)
    {
        if (_goldText != null)
        {
            _goldText.text = Managers.FormatGold(gold);//플레이어의 골드 텍스트 적용    
            upgradePannel.updateText(gold);
        }
    }
    public void UpdateTimeText()
    {
        if (_timeText != null)
        {
            _timeText.text = FormatTime(_currentTimeLimit);
        }
    }
    //public string FormatGold(int gold)
    //{
    //    string formattedString;

    //    if (gold >= 1000000000)
    //    {
    //        return formattedString = $"{(gold / 1000000000.0):F2}B";
    //    }
    //    else if (gold >= 1000000)
    //    {
    //        return formattedString = $"{(gold / 1000000.0):F2}M";
    //    }
    //    else if (gold >= 1000)
    //    {
    //        return formattedString = $"{(gold / 1000.0):F2}K";
    //    }
    //    else
    //    {
    //        return formattedString = gold.ToString();
    //    }
    //}
    public void SecretButton()
    {
        secretCount++;
        if (secretCount >= 10)
        {
            uiCanvas.gameObject.SetActive(false);
        }
    }
    private string FormatTime(float seconds)
    {
        int minutes = (int)(seconds / 60);
        int remainingSeconds = (int)(seconds % 60);

        if(minutes == 0 && remainingSeconds <= _redTime)
            return $"<color=\"red\">{minutes:D2}:{remainingSeconds:D2}</color>";
        else
            return $"<color=\"white\">{minutes:D2}:{remainingSeconds:D2}</color>";
    }
    public void ButtonInit()
    {
        if(player.CurrentData.speedCount + 1 >= upgradeList.speedList.Count)
            upgradeButtons[0].gameObject.SetActive(false);

        if (player.CurrentData.timeCount + 1>= upgradeList.timeList.Count)
            upgradeButtons[1].gameObject.SetActive(false);

        if (player.CurrentData.incomeCount + 1>= upgradeList.incomeList.Count)
            upgradeButtons[2].gameObject.SetActive(false);
    }
    public void AllocateFields()
    {
        if (uiCanvas == null)
        {
            GameObject uiCanvasObject = GameObject.Find(_uICanvasPath);
            if (uiCanvasObject == null)
            {
                //uiCanvasObject = Managers.Resource.Instantiate(_uICanvasPath);
                GameObject go = AMS.GetAsset<GameObject>(_uICanvasPath);
                uiCanvasObject = UnityEngine.Object.Instantiate(go);
            }
            uiCanvas = uiCanvasObject.GetComponent<Canvas>();

        }
        upgradePannel = Managers.FindObjectWithTag<UpgradePannel>(_upgradePannelTag);
        lobby = Managers.FindObjectWithTag<Image>(_lobyTag);
        gameOverPannel = Managers.FindObjectWithTag<GameOverPannel>(_gameOverTag);
        gameOverPannel.gameObject.SetActive(false);
        //timePannel = Managers.FindObjectWithTag<Button>("TimePannel");
        //timePannel.onClick.AddListener(SecretButton);

        //retryBtn = Managers.FindObjectWithTag<Button>(_retryButtonTag);
        startBtn = Managers.FindObjectWithTag<Button>(_startButtonTag);
        _timeText = Managers.FindObjectWithTag<TextMeshProUGUI>(_timeTextTag);
        _goldText = Managers.FindObjectWithTag<TextMeshProUGUI>(_goldTextTag);

        upgradeButtons = upgradePannel.GetComponentsInChildren<Button>(); // 이부분을 어떻게 고치면 좋을까?
        upgradeButtons[0].onClick.AddListener(ClickSpeedButton);
        upgradeButtons[1].onClick.AddListener(ClickTimeButton);
        upgradeButtons[2].onClick.AddListener(ClickIncomeButton);

        if (upgradeList != null)
            ButtonInit();

        //retryBtn.onClick.AddListener(Retry);
        //retryBtn.gameObject.SetActive(false);
        startBtn.onClick.AddListener(StartGame);
    }
}
