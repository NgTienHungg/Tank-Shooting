using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerRank : MonoBehaviour
{
    public Image tankPreview;
    public TextMeshProUGUI nameText;

    public void Init(ulong clientId)
    {
        var playerInfo = ClientManager.Instance.GetPlayerInfoByClientId(clientId);
        tankPreview.sprite = playerInfo.tankPreview;

        var rank = GameController.Instance.GetRankOfPlayerByClientId(clientId);
        nameText.text = "No " + rank + ". " + ClientManager.Instance.GetPlayerNameByClientId(clientId);
    }
}