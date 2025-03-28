using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private NetworkManager _NetworkManager;
    [SerializeField] private NavigationController _NavigationController;
    [SerializeField] private CombatRPCHandler _CombatRPCHandler;
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public NetworkManager GetNetworkManager()
    {
        return _NetworkManager;
    }

    public NavigationController GetNavigationController()
    {
        return _NavigationController;
    }

    public CombatRPCHandler GetCombatRPCHandler()
    {
        return _CombatRPCHandler;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Debug.Log("Fields are updated in GameManager.");
        if (_NetworkManager == null)
            _NetworkManager = FindFirstObjectByType<NetworkManager>(FindObjectsInactive.Exclude);

        if (_NavigationController == null)
            _NavigationController = FindFirstObjectByType<NavigationController>(FindObjectsInactive.Exclude);
        
        if (_CombatRPCHandler == null)
            _CombatRPCHandler = FindFirstObjectByType<CombatRPCHandler>(FindObjectsInactive.Exclude);
    }
#endif
}
