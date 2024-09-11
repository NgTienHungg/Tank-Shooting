using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

[RequireComponent(typeof(CanvasGroup))]
public abstract class UIScreen : MonoBehaviour
{
    [Title("UI Screen")]
    [InlineButton(nameof(ShowHide))]
    public CanvasGroup canvasGroup;
    public float appearDuration = 0.5f;
    public float disappearDuration = 0.3f;

    public void ShowHide()
        => canvasGroup.alpha = (canvasGroup.alpha == 0) ? 1 : 0;

    protected virtual void OnValidate()
        => canvasGroup = GetComponent<CanvasGroup>();

    public virtual void Hide()
    {
        gameObject.SetActive(false);

        canvasGroup.DOKill();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
    }

    public virtual void Appear()
    {
        gameObject.SetActive(true);

        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, appearDuration).SetEase(Ease.OutSine)
            .OnComplete(() => canvasGroup.interactable = true);
    }

    public virtual void Disappear()
    {
        canvasGroup.DOKill();
        canvasGroup.interactable = false;
        canvasGroup.DOFade(0f, disappearDuration).SetEase(Ease.OutSine)
            .OnComplete(Hide);
    }
}