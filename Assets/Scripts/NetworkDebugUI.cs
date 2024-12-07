using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace NetworkTest
{
    public class NetworkDebugUI : MonoBehaviour
    {
        private NetworkManager networkManager;

        void Awake()
        {
            Debug.Log("Awake called");
            networkManager = GetComponent<NetworkManager>();
        }
        public void StartHost() { networkManager.StartHost(); }
        public void StartClient() { networkManager.StartClient(); }
        public void StartServer() { networkManager.StartServer(); }

    }
}
