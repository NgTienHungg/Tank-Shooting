using TMPro;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    [Title("Popup")]
    public Transform popup;
    public TextMeshProUGUI title;
    public Button rankButton;
    public GameObject replayButton;

    [Title("Ranking")]
    public GameObject uiPlayerRankPrefab;
    public Transform uiPlayerRankContainer;

    public void Show()
    {
        gameObject.SetActive(true);

        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, 0.3f);

        title.text = GameController.Instance.IsWinner(NetworkUtils.GetLocalClientId())
            ? "YOU WIN"
            : "GAME OVER";

        replayButton.SetActive(GameController.Instance.IsWin);

        OnOpenRank();

        ClearContainer();
        RepreparePlayersRank();
    }

    public void Hide()
    {
        gameObject.SetActive(false);

        canvasGroup.DOKill();
        canvasGroup.alpha = 0;

        popup.DOKill();
        popup.localScale = Vector3.zero;
    }

    private void ClearContainer()
    {
        foreach (Transform child in uiPlayerRankContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void RepreparePlayersRank()
    {
        var deadPlayerIds = GameController.Instance.deadPlayerIds.ToList();
        deadPlayerIds.Reverse(); // reverse to show the highest rank first

        foreach (var clientId in deadPlayerIds)
        {
            var uiPlayerRank = Instantiate(uiPlayerRankPrefab, uiPlayerRankContainer);
            uiPlayerRank.GetComponent<UIPlayerRank>().Init(clientId);
        }
    }

    public void OnCloseButton()
    {
        rankButton.interactable = true;

        popup.DOKill();
        popup.DOScale(0f, 0.3f).SetEase(Ease.InBack);
    }

    public void OnOpenRank()
    {
        rankButton.interactable = false;

        popup.DOKill();
        popup.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
    }

    public void OnReplayButton()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Play", LoadSceneMode.Single);
    }
}