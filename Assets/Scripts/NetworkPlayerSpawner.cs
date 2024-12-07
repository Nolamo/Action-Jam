using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerSpawner : NetworkBehaviour
{
    [SerializeField]
    protected NetworkManager _networkManager;

    [SerializeField]
    protected GameObject _playerPrefab;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            _networkManager.OnClientConnectedCallback += OnClientConnected;
        }
    }

    protected void OnClientConnected(ulong clientId)
    {
        SpawnPlayerServerRpc(clientId);
    }

    [ServerRpc]
    protected void SpawnPlayerServerRpc(ulong clientId)
    {
        GameObject playerInstance = Instantiate(_playerPrefab);
        NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
        networkObject.SpawnWithOwnership(clientId);
    }
}
