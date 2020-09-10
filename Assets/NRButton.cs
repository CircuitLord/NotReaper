using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Windows.Forms;

public class NRButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    public UnityEvent NROnClick;

    public Image buttonBackground;

    public bool destroyOnClick = false;

    public string ButtonTooltip = "";

    public float onHoverValue = 0.8f;

    public Window parentWindow;

    private void Awake()
    {
        buttonBackground = gameObject.GetComponent<Image>();
        parentWindow = gameObject.GetComponentInParent<Window>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (destroyOnClick)
        {
            buttonBackground.DOFade(0.0f, 0.3f).SetEase(Ease.OutQuart).OnComplete(() => { Destroy(gameObject); NROnClick.Invoke(); });
        }
        else
        {
            NROnClick.Invoke();
            buttonBackground.DOKill(true);
            transform.DOKill(true);
            transform.DOPunchScale(new Vector3(0.04f, 0.04f, 0.04f), 0.13f, 2, 0.2f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonBackground.DOKill(true);
        buttonBackground.DOFade(onHoverValue, 0.15f).SetEase(Ease.OutQuart);
        if(parentWindow != null) parentWindow.canDrag = false;
        ToolTips.I.SetText(ButtonTooltip);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        buttonBackground.DOKill(true);
        buttonBackground.DOFade(1f, 0.5f).SetEase(Ease.InQuart);
        if (parentWindow != null) parentWindow.canDrag = true;
        ToolTips.I.SetText("");
    }
}
