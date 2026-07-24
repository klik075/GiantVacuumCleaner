using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ThingManager
{
    public Transform bossTransform;
    public Transform playerTransform;
    [SerializeField]
    private float _shootDuration = 0.5f;
    [SerializeField]
    private float middleXOffset = 4f;
    [SerializeField]
    private float middleYOffset = 5f;
    [SerializeField]
    private float endXOffset = 2f;
    [SerializeField]
    private float endYOffset = 2f;
    [SerializeField]
    private float endSizeOffset = 5f;

    private Queue<GameObject> _absorbingThings = new Queue<GameObject>();

    public void AbsorbThing(GameObject gameObject)
    {
        _absorbingThings.Enqueue(gameObject);
        gameObject.SetActive(false);
        //Debugger.Log($"{_absorbingThings.Count}");
    }
    public void Init()
    {
        bossTransform = Managers.Instance.bossTransform;
        playerTransform = Managers.Instance.playerTransform;
    }
    public void Attack()
    {
        if (bossTransform == null || playerTransform == null)
            return;

        AttackBoss().Forget();
    }
    public async UniTask AttackBoss()
    {
        Debugger.Log("callAttack");
        while (_absorbingThings.Count > 0)
        {
            GameObject gameObject = _absorbingThings.Dequeue();
            gameObject.transform.position = playerTransform.position;
            gameObject.transform.localScale = Vector3.one;
            gameObject.SetActive (true);
            Thing thing = gameObject.GetComponent<Thing>();
            float size = thing.CurrentData.Size;
            Sequence mySequence = DOTween.Sequence();
            Object.Destroy(thing);

            Vector3 middleOffset = new Vector3(Random.Range(-middleXOffset, middleXOffset), Random.Range(0, middleYOffset), 0);
            Vector3 endOffset = new Vector3(Random.Range(-endXOffset, endXOffset), Random.Range(0, middleYOffset), 0f);

            Vector3[] path = new Vector3[]
            {
                gameObject.transform.position,
                (gameObject.transform.position + bossTransform.position) / 2f + middleOffset, // 중간 위치 랜덤
                bossTransform.position + endOffset
            };

            TrailRenderer tr = gameObject.GetComponent<TrailRenderer>();
            if (tr != null)
                tr.enabled = true;

            mySequence.Append(gameObject.transform.DOPath(path, _shootDuration,PathType.CatmullRom).SetEase(Ease.Linear));
            mySequence.Join(gameObject.transform.DOScale( size + endSizeOffset, _shootDuration).SetEase(Ease.Linear));
            //플레이어 위치에서 시작해서 보스 쪽으로 날리기
            //처음에는 size = 0 DoScale로 점점 커지면서(원래 크기에 비례해서) 공격
            await UniTask.Delay(50);
        }
    }
}
