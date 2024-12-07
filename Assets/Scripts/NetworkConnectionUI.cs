using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkConnectionUI : MonoBehaviour
{
    protected NetworkManager networkManager;

    [SerializeField]
    protected GameObject connectionUICanvas;

    [SerializeField]
    protected TMP_InputField ipInputField;

    [SerializeField]
    protected TMP_InputField portInputField;

    [SerializeField]
    protected Button startHostButton;

    [SerializeField]
    protected Button startClientButton;

    [SerializeField]
    protected Button startServerButton;

    void Awake()
    {
        networkManager = GetComponent<NetworkManager>();

        if (ipInputField != null && portInputField != null)
        {

        }

        if (startHostButton != null)
        {
            startHostButton.onClick.AddListener(StartHost);
        }

        if (startClientButton != null)
        {
            startClientButton.onClick.AddListener(OnStartClientButtonClicked);
        }
    }

    protected void OnStartClientButtonClicked()
    {

    }

    public void StartHost() { networkManager.StartHost(); HideConnectionUICanvase(); }
    public void StartClient() { networkManager.StartClient(); HideConnectionUICanvase(); }
    public void StartServer() { networkManager.StartServer(); HideConnectionUICanvase(); }

    public void ShowConnectionUICanvas()
    {
        if (connectionUICanvas != null)
        {
            connectionUICanvas.SetActive(true);
        }
    }

    public void HideConnectionUICanvase()
    {
        if (connectionUICanvas != null)
        {
            connectionUICanvas.SetActive(false);
        }
    }

}
