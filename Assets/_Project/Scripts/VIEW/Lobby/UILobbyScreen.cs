using TMPro;
using UnityEngine;
using Unity.Netcode;
using Sirenix.OdinInspector;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;

public class UILobbyScreen : SingletonNetwork<UILobbyScreen>
{
    [Title("Screens")]
    public GameObject startScreen;
    public GameObject enterRoomScreen;
    public UIWaitingScreen waitingScreen;

    [Title("Enter Room")]
    public TMP_InputField serverIPAddressInput;
    public UnityTransport unityTransport;

    private void Start()
    {
        startScreen.SetActive(true);
        enterRoomScreen.SetActive(false);
        waitingScreen.Hide();
    }

    private void StartHost()
    {
        startScreen.SetActive(false);
        NetworkManager.Singleton.StartHost();
    }

    private void EnterRoom()
    {
        startScreen.SetActive(false);
        enterRoomScreen.SetActive(true);
    }

    private void StartClient()
    {
        var ipAddress = serverIPAddressInput.text;
        unityTransport.ConnectionData.Address = ipAddress;
        NetworkManager.Singleton.StartClient();
    }

    public void WaitOtherPlayers()
    {
        enterRoomScreen.SetActive(false);
        waitingScreen.Show();
    }

    public void OnHostButton() => StartHost();

    public void OnJoinButton() => EnterRoom();

    public void OnEnterButton() => StartClient();

    [ServerRpc(RequireOwnership = false)]
    public void RequestStartGameServerRpc()
    {
        DebugColor.Log("RequestStartGameServerRpc", "yellow");
        NetworkManager.Singleton.SceneManager.LoadScene("Play", LoadSceneMode.Single);
    }
}