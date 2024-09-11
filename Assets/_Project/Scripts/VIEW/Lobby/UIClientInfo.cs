using TMPro;
using Unity.Netcode;
using UnityEngine.UI;

public class UIClientInfo : NetworkBehaviour
{
    public TextMeshProUGUI playerName;
    public Image tankPreview;

    public void SetDataByClientId(ulong clientId)
    {
        var playerIndex = ClientManager.Instance.GetPlayerIndexByClientId(clientId);
        playerName.text = "Player " + (playerIndex + 1);

        var playerInfo = ClientManager.Instance.GetPlayerInfoByClientId(clientId);
        tankPreview.sprite = playerInfo.tankPreview;
    }
}