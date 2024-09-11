using TMPro;
using DG.Tweening;
using UnityEngine;
using Unity.Netcode;

public class PauseScreen : NetworkBehaviour
{
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI timerText;

    private bool _isCooldown;
    private float _timeRemaining;
    private float _lastSecond;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        gameObject.SetActive(false);
    }

    public void Show(float cooldownTime)
    {
        gameObject.SetActive(true);

        // UI
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.3f);
        timerText.text = cooldownTime.ToString("0s");

        // logic
        _isCooldown = true;
        _timeRemaining = cooldownTime;
        _lastSecond = cooldownTime + 1;
    }

    public void Hide()
    {
        canvasGroup.DOFade(0f, 0.2f)
            .OnComplete(() => gameObject.SetActive(false));

        _isCooldown = false;
    }

    private void Update()
    {
        if (!_isCooldown || !IsServer)
            return;

        _timeRemaining -= Time.deltaTime;

        // Update UI khi đã trôi qua 1s
        if (_timeRemaining <= (_lastSecond - 1))
        {
            _lastSecond--;

            if (_lastSecond >= 0)
                UpdateUIServerRpc();
            else
                GamePlayScreen.Instance.ResumeGameServerRpc();
        }
    }

    [ServerRpc]
    private void UpdateUIServerRpc()
        => CooldownClientRpc(_lastSecond);

    [ClientRpc]
    private void CooldownClientRpc(float lastSecond)
    {
        timerText.text = lastSecond.ToString("0s");
        timerText.transform.localScale = Vector3.one * 1.3f;
        timerText.transform.DOScale(1f, 0.3f).SetEase(Ease.OutQuad);
    }
}