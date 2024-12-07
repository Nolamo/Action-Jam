using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class NetworkConnectionUI : MonoBehaviour
{
    protected NetworkManager _networkManager;
    protected UnityTransport _networkTransport;

    //protected ulong _clientId;

    [SerializeField]
    protected GameObject _connectionUICanvas;

    [SerializeField]
    protected TMP_InputField _ipInputField;

    [SerializeField]
    protected TMP_InputField _portInputField;

    [SerializeField]
    protected Button _startHostButton;

    [SerializeField]
    protected Button _startClientButton;

    [SerializeField]
    protected Button _startServerButton;

    void Awake()
    {
        _networkManager = GetComponent<NetworkManager>();
        _networkTransport = GetComponent<UnityTransport>();

        _networkManager.OnClientConnectedCallback += OnClientConnected;
        _networkManager.OnClientDisconnectCallback += OnClientDisconnected;

        //_clientId = _networkManager.LocalClientId;

        if (_ipInputField != null && _portInputField != null)
        {

        }

        if (_startHostButton != null)
        {
            _startHostButton.onClick.AddListener(StartHost);
        }

        if (_startClientButton != null)
        {
            _startClientButton.onClick.AddListener(OnStartClientButtonClicked);
        }

        if (_startServerButton != null)
        {
            _startServerButton.onClick.AddListener(OnStartServerButtonClicked);
        }

        if (_connectionUICanvas != null)
        {
            ShowConnectionUICanvas();
        }
    }

    protected void OnStartClientButtonClicked()
    {
        if (_networkManager.IsServer == false && _networkManager.IsHost == false)
        {
            string ipAddress = _ipInputField.text;
            ushort port;

            if (ushort.TryParse(_portInputField.text, out port))
            {
                _networkTransport.SetConnectionData(ipAddress, port);
                StartClient();
            }
            else
            {
                Debug.Log("Invalid Port");
            }
        }
    }

    protected void OnStartServerButtonClicked()
    {
        StartServer();
    }

    protected void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} connected successfully.");

        if (clientId == _networkManager.LocalClientId)
        {
            HideConnectionUICanvas();
        }
    }

    protected void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"{clientId} has disconnected.");

        if (clientId == _networkManager.LocalClientId)
        {
            ShowConnectionUICanvas();
        }
    }

    public void StartHost() { _networkManager.StartHost();}
    public void StartClient() { _networkManager.StartClient(); }
    public void StartServer() { _networkManager.StartServer(); }

    public void ShowConnectionUICanvas()
    {
        if (_connectionUICanvas != null)
        {
            _connectionUICanvas.SetActive(true);
        }
    }

    public void HideConnectionUICanvas()
    {
        if (_connectionUICanvas != null)
        {
            _connectionUICanvas.SetActive(false);
        }
    }

}
