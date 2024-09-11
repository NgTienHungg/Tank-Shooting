using DG.Tweening;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class UIWaitingScreen : NetworkBehaviour
{
    [Header("UI")]
    public CanvasGroup canvasGroup;
    public Button startButton;
    public TextMeshProUGUI roomCapacity;
    public TextMeshProUGUI serverIPAddress;

    [Header("Map")]
    public Button map1Button;
    public Button map2Button;
    public Button map3Button;

    [Header("Player Manager")]
    public GameObject uiClientInfoPrefab;
    public Transform uiClientInfoContainer;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        DebugColor.Log("OnNetworkSpawn UIWaitingScreen", "yellow");

        // show UI in server
        serverIPAddress.gameObject.SetActive(IsServer);
        serverIPAddress.text = "IP Address: " + "<color=green>" + NetworkUtils.GetLocalIPAddress() + "</color>";
    }

    public void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void Show()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // set room capacity
        roomCapacity.text = "Room: " + ClientManager.Instance.ClientCount + "/" + ClientManager.MaxPlayers;

        // enable start button if there are more than 1 player
        startButton.interactable = true; // ClientManager.Instance.ClientCount > 1;

        DestroyAllPlayersInfo();
        RepreparePlayersInfo();
        OnSelectMapButton(1);
    }

    private void DestroyAllPlayersInfo()
    {
        foreach (Transform child in uiClientInfoContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void RepreparePlayersInfo()
    {
        var clientConnectedIds = ClientManager.Instance.ClientConnectedIds;

        foreach (var clientId in clientConnectedIds)
        {
            var uiClientInfo = Instantiate(uiClientInfoPrefab, uiClientInfoContainer);
            uiClientInfo.GetComponent<UIClientInfo>().SetDataByClientId(clientId);
        }
    }

    public void OnClickStartButton()
    {
        UILobbyScreen.Instance.RequestStartGameServerRpc();
    }

    public void OnSelectMapButton(int mapId)
    {
        ClientManager.Instance.SelectMapServerRpc(mapId);
    }

    public void RefreshSelectMap(int mapId)
    {
        map1Button.interactable = mapId != 1;
        map2Button.interactable = mapId != 2;
        map3Button.interactable = mapId != 3;
    }
}