using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SoundInfo
{
    public string tag;
    public AudioClip clip;
}
public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance { get => instance; }

    [Header("Background")]
    [SerializeField]
    private AudioClip backgroundClip;//배경 음악
    private AudioSource backgroundAudioSource;//배경 음악 오디오 소스

    [SerializeField]
    [Range(0, 100f)] public float bgmVolumePercent; //브금 0 ~ 100 조절
    [SerializeField]
    [Range(0.0f, 1.0f)] public float maxBgmVolume = 0.5f; //최대 브금 0 ~ 1크기
    [SerializeField]
    [Range(0, 100f)] public float sfxVolumePercent;

    [SerializeField]
    private bool isPlayingBgm = true;

    [Header("SoundInfo")]
    public List<SoundInfo> soundEffectList; //할당할 효과음 리스트

    private Dictionary<string, SoundInfo> audioDictionary;//해당하는 효과음들을 내부적으로 저장.
    private Queue<GameObject> audioQueue;//오디오 큐
    private int playCount = 0;

    [SerializeField]
    private GameObject AudioSoundPrefab;
    [SerializeField]
    private float initQueueCount = 10f;
    [SerializeField]
    private float maxQueueCount = 20f;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Init();
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    public void Init()
    {
        backgroundAudioSource = GetComponent<AudioSource>();
        Initialize();
        SetBgm("BGM");
    }

    public void Initialize()
    {
        InitializeQueue();
        InitializeDictionary();
    }
    public GameObject CreateSoundObject() //사운드 오브젝트 생성
    {
        GameObject obj = Instantiate(AudioSoundPrefab);
        obj.name = "AudioSourceObject";
        obj.transform.SetParent(transform);//사운드 매니저 하위로 지정
        obj.transform.localPosition = Vector3.zero;
        obj.SetActive(false);
        audioQueue.Enqueue(obj);//큐에 넣음
        return obj;
    }
    public void InitializeQueue()//소리를 낼 객체 초기화
    {
        audioQueue = new Queue<GameObject>();

        for (int i = 0; i < initQueueCount; i++)
        {
            CreateSoundObject();//객체를 만들고 큐에 넣기
            //Debug.Log("오디오 initilaize");
        }
    }
    public void InitializeDictionary()//드래그 할당한 브금을 내부 사전에 넣는 과정
    {
        audioDictionary = new Dictionary<string, SoundInfo>();

        foreach (var soundInfo in soundEffectList)
        {
            if (soundInfo.tag != "")
            {
                SoundInfo newSoundInfo = new SoundInfo();
                newSoundInfo.tag = soundInfo.tag;
                newSoundInfo.clip = soundInfo.clip;

                audioDictionary[soundInfo.tag] = newSoundInfo; //사운드 정보로 사전에 tag값에 clip을 저장.                
                //Debug.Log($"{soundInfo.tag}");
            }
        }
    }
    public void SetBgm(string bgmTag)
    {
        if (audioDictionary.ContainsKey(bgmTag) == true)// 저장이 되어있을 때만 재생
        {
            backgroundAudioSource.clip = audioDictionary[bgmTag].clip;
            backgroundAudioSource.Play();
            backgroundAudioSource.loop = true;
        }
    }
    public void PlayAudioClip(string tag)//tag값으로 오디오 재생
    {
        if (!audioDictionary.ContainsKey(tag)) //오디오 클립이 존재하는지 확인
        {
            return;
        }

        if (playCount >= maxQueueCount)
        {
            if (tag != "level_up")//레벨업은 재생
            {
                return;
            }
        }

        if (audioQueue.Count > 0)//재생시킬 객체가 있는지 확인
        {
            GameObject obj = audioQueue.Dequeue();//큐에서 꺼낸다.
            playCount++;
            AudioSourceObject objAudioSource = obj.GetComponent<AudioSourceObject>();//소리객체의 스크립트 가져옴
            AudioClip clip = audioDictionary[tag].clip;
            obj.gameObject.SetActive(true);//활성화
            objAudioSource.PlayClip(clip, () => ReturnAudioClip(objAudioSource.gameObject)).Forget();
        }
        else//없으면
        {
            GameObject newObj = CreateSoundObject();//새로 생성한다.
            playCount++;
            AudioSourceObject objAudioSource = newObj.GetComponent<AudioSourceObject>();
            AudioClip clip = audioDictionary[tag].clip;
            newObj.gameObject.SetActive(true);//활성화
            objAudioSource.PlayClip(clip,()=> ReturnAudioClip(objAudioSource.gameObject)).Forget();
        }
    }
    public void ReturnAudioClip(GameObject audioSourceObject)
    {
        playCount--;
        audioQueue.Enqueue(audioSourceObject);
        audioSourceObject.SetActive(false);
    }
    // Update is called once per frame
    //void Update()
    //{
    //    SetBgmVolume();
    //}

    public void SetBgmVolume() //브금 볼륨 bgmVolume 값에 따른 설정
    {
        if (isPlayingBgm)
            backgroundAudioSource.volume = PercentToDegree(bgmVolumePercent) * maxBgmVolume;
        else
            backgroundAudioSource.volume = 0.0f;
    }
    public void MuteBgmButtun() //브금 껐다 켰다
    {
        isPlayingBgm = !isPlayingBgm;
    }
    public float PercentToDegree(float percent)
    {
        return percent / 100f;
    }
}
