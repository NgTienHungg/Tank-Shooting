using TMPro;
using DG.Tweening;
using UnityEngine;
using Unity.Netcode;

public class RequestPausePanel : NetworkBehaviour
{
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI messageRequest;
    public TextMeshProUGUI approveText;
    public TextMeshProUGUI disapproveText;

    private int _approvedCount;
    private int _disapprovedCount;
    private int _totalPlayerCount;

    // Hide when start scene
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        gameObject.SetActive(false);
    }

    public void Show(ulong clientId, int totalPlayerCount)
    {
        gameObject.SetActive(true);

        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.3f);

        _totalPlayerCount = totalPlayerCount;
        messageRequest.text = $"Player {clientId + 1} requests a 10s pause!";

        // * If is Owner, auto approve.
        if (NetworkManager.Singleton.LocalClientId == clientId)
            RequestApproveServerRpc(clientId);
    }

    public void Hide()
    {
        _approvedCount = _disapprovedCount = 0;

        canvasGroup.interactable = true;
        canvasGroup.DOFade(0f, 0.3f)
            .OnComplete(() => gameObject.SetActive(false));
    }

    public void OnClickApproveButton()
        => RequestApproveServerRpc(NetworkManager.Singleton.LocalClientId);

    public void OnClickDisapproveButton()
        => RequestDisapproveServerRpc(NetworkManager.Singleton.LocalClientId);

    [ServerRpc(RequireOwnership = false)]
    private void RequestApproveServerRpc(ulong clientId)
    {
        _approvedCount++;

        var requestPauseData = new RequestPauseData
        {
            clientId = clientId,
            approvedCount = _approvedCount,
            disapprovedCount = _disapprovedCount,
        };

        // * TotalPlayerCount chỉ được gọi từ server nên cần truyền về client để client sử dụng.
        UpdateStatsClientRpc(requestPauseData);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestDisapproveServerRpc(ulong clientId)
    {
        _disapprovedCount++;

        var requestPauseData = new RequestPauseData
        {
            clientId = clientId,
            approvedCount = _approvedCount,
            disapprovedCount = _disapprovedCount,
        };

        UpdateStatsClientRpc(requestPauseData);
    }

    [ClientRpc]
    private void UpdateStatsClientRpc(RequestPauseData requestPauseData)
    {
        // * Disable interactable with button when client voted.
        if (requestPauseData.clientId == NetworkManager.Singleton.LocalClientId)
            canvasGroup.interactable = false;

        _approvedCount = requestPauseData.approvedCount;
        _disapprovedCount = requestPauseData.disapprovedCount;

        UpdateStats();
    }

    private void UpdateStats()
    {
        approveText.text = "<color=green>Approve: " + $"{_approvedCount}/{_totalPlayerCount}";
        disapproveText.text = "<color=red>Disapprove: " + $"{_disapprovedCount}/{_totalPlayerCount}";

        if (_approvedCount * 2 > _totalPlayerCount)
            GamePlayScreen.Instance.PauseGameServerRpc();
        else if (_disapprovedCount * 2 > _totalPlayerCount || (_approvedCount + _disapprovedCount) == _totalPlayerCount)
            GamePlayScreen.Instance.ResumeGameServerRpc();
    }
}

public struct RequestPauseData : INetworkSerializable
{
    public ulong clientId;
    public int approvedCount;
    public int disapprovedCount;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref approvedCount);
        serializer.SerializeValue(ref disapprovedCount);
    }
}