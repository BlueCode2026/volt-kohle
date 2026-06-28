using UnityEngine;

/// <summary>
/// Manages all currency, XP, and passive-income calculations.
/// Does NOT handle click input — that is ClickManager's responsibility (Sprint 2).
/// Fires events through UIEventBroker so UI components stay fully decoupled.
/// </summary>
public class EconomyManager : MonoBehaviour
{
    // ── Serialised state — visible in Inspector for debugging ─────────────────
    [SerializeField] private double _currency;
    [SerializeField] private double _xp;
    [SerializeField] private int    _playerLevel = 1;

    /// <summary>
    /// Base passive income per second. Other systems (EmployeeManager, PrestigeManager)
    /// register their contributions via AddPassiveIncomeSource / RemovePassiveIncomeSource.
    /// </summary>
    [SerializeField] private double _passiveIncomePerSec;

    /// <summary>
    /// Cumulative earnings across the current prestige cycle — used by PrestigeManager
    /// to calculate GruenderPunkte.
    /// </summary>
    [SerializeField] private double _totalEarnedThisPrestige;

    // ── Runtime state ─────────────────────────────────────────────────────────
    private float _tickTimer;
    private double _globalMultiplier = 1.0;

    // ── Public read-only properties ───────────────────────────────────────────
    public double Currency            => _currency;
    public double XP                  => _xp;
    public int    PlayerLevel         => _playerLevel;
    public double PassiveIncomePerSec => _passiveIncomePerSec * _globalMultiplier;
    public double TotalEarnedThisPrestige => _totalEarnedThisPrestige;

    // ── Passive income tick ───────────────────────────────────────────────────
    private void Update()
    {
        _tickTimer += Time.deltaTime;
        if (_tickTimer >= GameConstants.PASSIVE_INCOME_TICK_SECONDS)
        {
            _tickTimer -= GameConstants.PASSIVE_INCOME_TICK_SECONDS;
            if (_passiveIncomePerSec > 0.0)
                AddCurrency(_passiveIncomePerSec * _globalMultiplier);
        }
    }

    // ── Currency ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Adds the given amount to the balance and fires OnCurrencyChanged.
    /// All income paths (clicks, passive, offline earnings) must go through here.
    /// </summary>
    public void AddCurrency(double amount)
    {
        if (amount <= 0.0) return;
        _currency                 += amount;
        _totalEarnedThisPrestige  += amount;
        UIEventBroker.RaiseCurrencyChanged(_currency);
    }

    /// <summary>
    /// Attempts to deduct the given amount. Returns true on success, false if
    /// the balance is insufficient.
    /// </summary>
    public bool SpendCurrency(double amount)
    {
        if (amount <= 0.0 || _currency < amount) return false;
        _currency -= amount;
        UIEventBroker.RaiseCurrencyChanged(_currency);
        return true;
    }

    /// <summary>
    /// Forcibly sets the currency balance — used by SaveManager on load and by
    /// PrestigeManager on reset. Does not count towards TotalEarned.
    /// </summary>
    public void SetCurrency(double amount)
    {
        _currency = Mathf.Max(0f, (float)amount);
        UIEventBroker.RaiseCurrencyChanged(_currency);
    }

    // ── XP & Levelling ────────────────────────────────────────────────────────

    public void AddXP(double amount)
    {
        if (amount <= 0.0) return;
        _xp += amount;
        UIEventBroker.RaiseXPChanged(_xp);
        CheckLevelUp();
    }

    public void SetXP(double amount, int level)
    {
        _xp          = amount;
        _playerLevel = level;
        UIEventBroker.RaiseXPChanged(_xp);
    }

    private void CheckLevelUp()
    {
        double threshold = XPThresholdForLevel(_playerLevel + 1);
        while (_xp >= threshold)
        {
            _playerLevel++;
            UIEventBroker.RaisePlayerLevelUp(_playerLevel);
            threshold = XPThresholdForLevel(_playerLevel + 1);
        }
    }

    /// <summary>
    /// XP required to reach the given level.
    /// Formula: BASE * SCALING_FACTOR^(level-1)
    /// </summary>
    public static double XPThresholdForLevel(int level)
    {
        return GameConstants.XP_PER_LEVEL_BASE
               * System.Math.Pow(GameConstants.XP_LEVEL_SCALING_FACTOR, level - 1);
    }

    // ── Passive income management ─────────────────────────────────────────────

    public void SetBasePassiveIncome(double incomePerSec)
    {
        _passiveIncomePerSec = incomePerSec;
    }

    public void AddPassiveIncomeSource(double incomePerSec)
    {
        _passiveIncomePerSec += incomePerSec;
    }

    public void RemovePassiveIncomeSource(double incomePerSec)
    {
        _passiveIncomePerSec = System.Math.Max(0.0, _passiveIncomePerSec - incomePerSec);
    }

    // ── Global multiplier (applied by PrestigeManager) ────────────────────────

    public void SetGlobalMultiplier(double multiplier)
    {
        _globalMultiplier = System.Math.Max(1.0, multiplier);
    }

    // ── Prestige reset ────────────────────────────────────────────────────────

    /// <summary>
    /// Resets per-prestige state. Called by PrestigeManager after awarding
    /// GruenderPunkte. Does NOT reset the global multiplier.
    /// </summary>
    public void ApplyPrestigeReset()
    {
        _currency                = 0.0;
        _xp                      = 0.0;
        _playerLevel             = 1;
        _passiveIncomePerSec     = 0.0;
        _totalEarnedThisPrestige = 0.0;
        UIEventBroker.RaiseCurrencyChanged(_currency);
        UIEventBroker.RaiseXPChanged(_xp);
    }

    // ── Save/Load helpers ─────────────────────────────────────────────────────

    public double GetPassiveIncomeSummary() => PassiveIncomePerSec;

    public void LoadFromSave(double currency, double xp, int level, double totalEarned)
    {
        _currency                = currency;
        _xp                      = xp;
        _playerLevel             = level;
        _totalEarnedThisPrestige = totalEarned;
        UIEventBroker.RaiseCurrencyChanged(_currency);
        UIEventBroker.RaiseXPChanged(_xp);
    }
}
