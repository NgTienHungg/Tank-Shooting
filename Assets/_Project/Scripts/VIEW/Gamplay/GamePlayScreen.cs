using Cysharp.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
using Sirenix.OdinInspector;

public class GamePlayScreen : SingletonNetwork<GamePlayScreen>
{
    [Title("Request Pause")]
    public GameObject pauseButton;
    public RequestPausePanel requestPausePanel;

    [Title("Pause")]
    public PauseScreen pauseScreen;
    public float cooldownTime = 10f;

    [Title("Game Over")]
    public GameOverScreen gameOverScreen;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        gameOverScreen.Hide();
    }

    public void OnClickPauseButton()
        => RequestPauseServerRpc(NetworkManager.Singleton.LocalClientId);

    [ServerRpc(RequireOwnership = false)]
    private void RequestPauseServerRpc(ulong clientId)
        => RequestPauseClientRpc(clientId, NetworkManager.Singleton.ConnectedClients.Count);

    [ClientRpc]
    private void RequestPauseClientRpc(ulong clientId, int totalPlayerCount)
    {
        pauseButton.SetActive(false);
        requestPausePanel.Show(clientId, totalPlayerCount);
    }

    [ServerRpc(RequireOwnership = false)]
    public void PauseGameServerRpc()
        => PauseGameClientRpc(cooldownTime);

    [ClientRpc]
    private void PauseGameClientRpc(float cooldownTime)
    {
        requestPausePanel.Hide();
        pauseScreen.Show(cooldownTime);

        GameController.IsPaused = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ResumeGameServerRpc()
        => ResumeGameClientRpc();

    [ClientRpc]
    private void ResumeGameClientRpc()
    {
        requestPausePanel.Hide();
        pauseScreen.Hide();
        pauseButton.SetActive(true);

        GameController.IsPaused = false;
    }

    public void GameOver()
    {
        pauseButton.SetActive(false);
        gameOverScreen.Show();
    }
}