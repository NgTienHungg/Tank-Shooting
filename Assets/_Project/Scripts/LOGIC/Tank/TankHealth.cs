using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class TankHealth : NetworkBehaviour, IDamageable
{
    [SerializeField] private Image hpBarFill;
    [SerializeField] private TextMeshProUGUI hpText;

    private int _maxHp;
    private int _currentHp;

    public void Init(TankConfig tankConfig)
    {
        _maxHp = tankConfig.MaxHp;
        _currentHp = _maxHp;

        // setup UI
        hpText.text = _currentHp + "/" + _maxHp;
        hpBarFill.fillAmount = (float)_currentHp / _maxHp;
    }

    public void TakeDamage(int damage)
    {
        TakeDamageServerRpc(damage);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TakeDamageServerRpc(int damage, ServerRpcParams serverRpcParams = default)
    {
        Debug.LogWarning("Server request take damage");
        _currentHp = Mathf.Clamp(_currentHp - damage, 0, _maxHp);

        UpdateHpClientRpc(_currentHp);
    }

    [ClientRpc]
    private void UpdateHpClientRpc(int currentHp)
    {
        Debug.LogWarning("Client update hp");
        _currentHp = currentHp;
        hpText.text = _currentHp + "/" + _maxHp;
        hpBarFill.fillAmount = (float)_currentHp / _maxHp;

        if (_currentHp == 0)
        {
            GetComponent<PlayerController>().Despawn();

            if (IsOwner)
                GameController.Instance.NewPlayerDeadServerRpc(NetworkUtils.GetLocalClientId());
        }
    }
}