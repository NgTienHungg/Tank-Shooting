using TMPro;
using DG.Tweening;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private TankConfig tankConfig;
    [SerializeField] private TextMeshProUGUI nameText;

    private TankHealth _tankHealth;
    private TankMovement _tankMovement;
    private TankController _tankController;

    private Sequence _nameAnimation;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        GetComponents();
        Initialize();

        nameText.text = "Player " + (NetworkObject.OwnerClientId + 1);
        nameText.color = IsOwner ? Color.yellow : Color.red;
        gameObject.name = nameText.text;

        // * ĐỔI TÊN CHO TANK MỖI 2s
        if (IsOwner)
        {
            _nameAnimation = DOTween.Sequence();
            _nameAnimation.AppendInterval(2f);
            _nameAnimation.AppendCallback(() => nameText.text = "YOU");
            _nameAnimation.AppendInterval(2f);
            _nameAnimation.AppendCallback(() => nameText.text = "Player " + (NetworkObject.OwnerClientId + 1));
            _nameAnimation.SetLoops(-1);
            _nameAnimation.Play();
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        _nameAnimation.Kill();
    }

    private void GetComponents()
    {
        _tankHealth = GetComponent<TankHealth>();
        _tankMovement = GetComponent<TankMovement>();
        _tankController = GetComponent<TankController>();
    }

    private void Initialize()
    {
        _tankHealth.Init(tankConfig);
        _tankMovement.Init(tankConfig);
        _tankController.Init(tankConfig);
    }

    private void Update()
    {
        if (!IsOwner || !Application.isFocused)
            return;

        _tankMovement.RotateBarrel(Input.mousePosition);

        if (GameController.IsPaused)
            return;

        _tankMovement.MoveAndRotate(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (Input.GetMouseButtonDown(0) && !Utils.IsMouseOverUI())
            _tankController.RequestFireServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    public void Despawn()
    {
        NetworkObjectSpawner.DespawnNetworkObject(NetworkObject);
    }
}