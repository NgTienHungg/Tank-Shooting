using Unity.Netcode;
using UnityEngine;

public class StartNetwork : MonoBehaviour
{
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        gameObject.SetActive(false);
    }

    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        gameObject.SetActive(false);
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        gameObject.SetActive(false);
    }
}