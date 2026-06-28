using UnityEngine;

/// <summary>
/// Central singleton that owns all manager references and orchestrates
/// game-wide initialisation in the Bootstrap scene.
///
/// Scene setup:
///   1. Create a "GameManager" GameObject in Bootstrap.unity.
///   2. Attach this script plus EconomyManager and SaveManager to it (or children).
///   3. Wire the SerializeField references in the Inspector.
///   4. The Bootstrap scene is loaded once; everything transitions to MainGame.unity.
/// </summary>
public class GameManager : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────────────────────────
    public static GameManager Instance { get; private set; }

    // ── Manager references — assign in Inspector ──────────────────────────────
    [Header("Core Managers")]
    [SerializeField] private EconomyManager _economyManager;
    [SerializeField] private SaveManager    _saveManager;

    // Future sprints: uncomment and wire up as they are implemented.
    // [SerializeField] private ProgressionManager  _progressionManager;
    // [SerializeField] private ClickManager        _clickManager;
    // [SerializeField] private EnergySystem        _energySystem;
    // [SerializeField] private RaidRiskSystem      _raidRiskSystem;
    // [SerializeField] private UpgradeManager      _upgradeManager;
    // [SerializeField] private PrivateLifeManager  _privateLifeManager;
    // [SerializeField] private PrestigeManager     _prestigeManager;
    // [SerializeField] private PurchaseManager     _purchaseManager;

    // ── Public accessors ──────────────────────────────────────────────────────
    public EconomyManager EconomyManager => _economyManager;
    public SaveManager    SaveManager    => _saveManager;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("[GameManager] Duplicate instance destroyed.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        ValidateReferences();
        Initialize();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // ── Initialisation ────────────────────────────────────────────────────────

    private void Initialize()
    {
        // 1. Wire the auto-save callback before loading, so any early events
        //    can trigger a save if needed.
        _saveManager.OnSaveRequested += BuildSaveData;

        // 2. Load persisted state.
        SaveData save = _saveManager.LoadGame();

        // 3. Apply saved economy state.
        _economyManager.LoadFromSave(
            save.currency,
            save.xp,
            save.playerLevel,
            save.totalEarned
        );
        _economyManager.SetGlobalMultiplier(save.globalMultiplier);

        // 4. Calculate and apply offline earnings (only meaningful if passive income > 0).
        float passiveIncome = (float)_economyManager.GetPassiveIncomeSummary();
        if (passiveIncome > 0f)
        {
            var (earnings, elapsed) = SaveManager.CalculateOfflineEarnings(save, passiveIncome);
            if (earnings > 0.0)
            {
                _economyManager.AddCurrency(earnings);
                UIEventBroker.RaiseOfflineEarningsReady(earnings, elapsed);
            }
        }

        Debug.Log("[GameManager] Initialisation complete.");
    }

    // ── Save snapshot builder ─────────────────────────────────────────────────

    /// <summary>
    /// Constructs a SaveData snapshot from all managers. Called by SaveManager
    /// on auto-save triggers and by explicit save requests.
    /// </summary>
    public SaveData BuildSaveData()
    {
        SaveData data = new SaveData
        {
            currency          = _economyManager.Currency,
            xp                = _economyManager.XP,
            playerLevel       = _economyManager.PlayerLevel,
            totalEarned       = _economyManager.TotalEarnedThisPrestige,
            globalMultiplier  = 1.0, // PrestigeManager sets this in Sprint 4
        };

        // Future sprints add their state here:
        // data.currentPhase    = _progressionManager.CurrentPhase;
        // data.locationIndex   = _progressionManager.LocationIndex;
        // data.totalFounderPoints = _prestigeManager.TotalFounderPoints;

        return data;
    }

    // ── Public save trigger (call from UI "Save & Quit" button etc.) ──────────

    public void SaveNow()
    {
        _saveManager.SaveGame(BuildSaveData());
    }

    // ── Validation ────────────────────────────────────────────────────────────

    private void ValidateReferences()
    {
        if (_economyManager == null)
            Debug.LogError("[GameManager] EconomyManager reference is not set in Inspector!");
        if (_saveManager == null)
            Debug.LogError("[GameManager] SaveManager reference is not set in Inspector!");
    }
}
