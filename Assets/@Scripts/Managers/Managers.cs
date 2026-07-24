using DG.Tweening;
using Scripts.Framework.Managers.Asset.Core;
using Scripts.Framework.Modules.DeviceModules.SoundModule;
using Scripts.Framework.Scenes.SO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Managers : IndividualSingleton<Managers>
{
    [SerializeField]
    private string _bossLayerName = "Boss";
    [SerializeField]
    private string _playerLayerName = "Player";
    public Transform bossTransform;
    public Transform playerTransform;
    public Player player;

    [SerializeField] private RequestSequenceEventChannelSO _sequenceEventChannel;

    [Header("You need Scenes")]
    [SerializeField] private GameSceneSO _gameScene;

    ThingManager _thingManager = new ThingManager();
    RandomSpawnManager _randomSpawnManager = new RandomSpawnManager();
    UpgradeManager _upgradeManager = new UpgradeManager();
    PrefabLoader _prefabLoader = new PrefabLoader();
    public static ThingManager ThingManager { get { return Instance._thingManager; } }
    public static RandomSpawnManager RandomSpawnManager { get { return Instance._randomSpawnManager; } }
    public static UpgradeManager UpgradeManager { get { return Instance._upgradeManager; } }
    public static PrefabLoader PrefabLoader { get { return Instance._prefabLoader; } }

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        //Init();
    }
    void Start()
    {
        Init();
    }
    // Update is called once per frame
    void Update()
    {
        UpgradeManager.Update();
    }
    public void Init()
    {
        Application.targetFrameRate = 60;

//#if !UNITY_EDITOR
//        _gameScene = AMS.GetAsset<GameSceneSO>("GameScene");
//        _sequenceEventChannel = AMS.GetAsset<RequestSequenceEventChannelSO>("RequestSequenceEventChannel");
//#endif

        //FindObjectWithLayer<Player>(_playerLayerName, out playerTransform);
        FindObjectWithTagSetTransform<Player>(_playerLayerName, out playerTransform);
        player = playerTransform.gameObject.GetComponent<Player>();
        //FindObjectWithLayer<Boss>(_bossLayerName, out bossTransform);
        FindObjectWithTagSetTransform<Boss>(_bossLayerName, out bossTransform);

        PrefabLoader.Init();
        ThingManager.Init();
        RandomSpawnManager.Init();
        UpgradeManager.Init();
    }
    public async void ReStart()
    {
        Time.timeScale = 1.0f;
        await _sequenceEventChannel.RaiseEvent(_gameScene, false);
    }
    protected override void OnDestroy()
    {
        _upgradeManager.OnDestroy();
        DOTween.KillAll();
        base.OnDestroy();
    }
    static public bool FindObjectWithLayer<T>(string layerName, out Transform objTransform) where T : MonoBehaviour
    {
        int targetLayer = LayerMask.NameToLayer(layerName);

        if (targetLayer == -1)
        {
            Debugger.LogError("해당 이름의 레이어가 존재하지 않습니다: " + layerName);
            objTransform = null;
            return false;
        }

        T[] allObjects = FindObjectsOfType<T>();

        foreach (T obj in allObjects)
        {
            if (obj.gameObject.layer == targetLayer)
            {
                objTransform = obj.transform;
                return true;
            }
        }
        objTransform = null;
        return false;
    }
    static public T FindObjectWithTag<T>(string tag) where T : MonoBehaviour
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            // 씬이 활성화되어 있는지 확인
            if (scene.isLoaded)
            {
                // 해당 씬의 모든 오브젝트를 검색
                GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject obj in taggedObjects)
                {
                    T component = obj.GetComponent<T>();
                    if (component != null)
                    {
                        return component;
                    }
                }
            }
        }
        return null;

        //GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
        //foreach (GameObject obj in taggedObjects)
        //{
        //    T component = obj.GetComponent<T>();
        //    if (component != null)
        //    {
        //        return component;
        //    }
        //}
        //return null;
    }
    public static string FormatGold(int gold)
    {
        string formattedString;

        if (gold >= 1000000000)
        {
            return formattedString = $"{(gold / 1000000000.0):F2}B";
        }
        else if (gold >= 1000000)
        {
            return formattedString = $"{(gold / 1000000.0):F2}M";
        }
        else if (gold >= 1000)
        {
            return formattedString = $"{(gold / 1000.0):F2}K";
        }
        else
        {
            return formattedString = gold.ToString();
        }
    }
    static public void FindObjectWithTagSetTransform<T>(string tag,out Transform objTransform) where T : MonoBehaviour
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            // 씬이 활성화되어 있는지 확인
            if (scene.isLoaded)
            {
                // 해당 씬의 모든 오브젝트를 검색
                GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject obj in taggedObjects)
                {
                    T component = obj.GetComponent<T>();
                    if (component != null)
                    {
                        objTransform = obj.transform;
                        return;
                    }
                }
            }
        }
        objTransform = null;
    }
}
