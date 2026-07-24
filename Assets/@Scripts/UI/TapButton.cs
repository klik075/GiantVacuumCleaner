using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class TapButton : MonoBehaviour
{
    public UnityEvent onTabSelected;//눌렀을 때 실행할 이벤트
    public UnityEvent onTabUnselected;//해제했을 때 실행항 이벤트
    public RectTransform rectTransform;
    public Vector2 baseSize = new Vector2(213f,207f);
    public Vector2 selectedSizeRatio = new Vector2(1.5f,1.5f);

    private float duration = 0.2f;

    // Start is called before the first frame update
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Select()
    {
        if (onTabSelected != null)
        {
            onTabSelected.Invoke();
            rectTransform.DOSizeDelta(baseSize * selectedSizeRatio, duration);

            if(SoundManager.Instance != null)
                SoundManager.Instance.PlayAudioClip("button_UI");
        }
    }

    public void Unselect()
    {
        if (onTabUnselected != null)
        { 
            onTabUnselected.Invoke();
            rectTransform.DOSizeDelta(baseSize, duration);
        }
    }
    public void OnSelectTab(TapButton button)
    {
        TapController.Instance.SelectedButton(button);
    }
}
