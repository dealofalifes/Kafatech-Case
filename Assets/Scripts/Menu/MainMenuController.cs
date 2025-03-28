using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button _JoinLobbyButton;
    [SerializeField] private Button _CancelLobbyButton;

    [SerializeField] private Text _InformationText;

    [SerializeField] private GameObject _ExampleHolder;

    private INetworkManager _NetworkManager;
    private void Awake()
    {
        _JoinLobbyButton.onClick.AddListener(OnJoinButtonClicked);
        _CancelLobbyButton.onClick.AddListener(OnCancelButtonClicked);

        Init();
    }

    public void Init()
    {
        _NetworkManager = GameManager.Instance.GetNetworkManager();

        _NetworkManager.OnClientConnected += OnClientConnected;
        _NetworkManager.OnClientDisconnected += OnClientDisconnected;
        _NetworkManager.OnRemoteConnected += OnRemoteConnected;
        _NetworkManager.OnRemoteDisconnected += OnRemoteDisconnected;
    }

    private void OnJoinButtonClicked()
    {
        _JoinLobbyButton.interactable = false;
        NetworkManager.Instance.ClientJoinLobby();

        _InformationText.text = "Connecting to the server!";
    }

    private void OnCancelButtonClicked()
    {
        NetworkManager.Instance.ClientExitLobby();
    }

    private void OnClientConnected()
    {
        SetButtonsConnectedState(true);
        _InformationText.text = "You are Connected. Waiting for a matching!";
    }

    private void OnClientDisconnected()
    {
        SetButtonsConnectedState(false);
        _InformationText.text = "Connection has been ended!";
    }

    private void OnRemoteConnected(int _clientID)
    {
        SetButtonsConnectedState(true);
        //_InformationText.text = "";
    }

    private void OnRemoteDisconnected(int _clientID)
    {
        SetButtonsConnectedState(false);
        //_InformationText.text = "";
    }

    private void SetButtonsConnectedState(bool _connected)
    {
        _JoinLobbyButton.interactable = !_connected;
        _JoinLobbyButton.gameObject.SetActive(!_connected);

        _CancelLobbyButton.interactable = _connected;
        _CancelLobbyButton.gameObject.SetActive(_connected);
    }

    public void UpdateInformationText(string _text)
    {
        _InformationText.text = _text;
    }

    public void HideMenuOnServer()
    {
        _ExampleHolder.SetActive(false);
    }
    private void OnDestroy()
    {
        _JoinLobbyButton.onClick.RemoveAllListeners();
        _CancelLobbyButton.onClick.RemoveAllListeners();
    }
}
