using System;

/// <summary>
/// Zero-coupling event bus. UI components subscribe in OnEnable() and
/// unsubscribe in OnDisable() — never access managers directly from UI.
/// </summary>
public static class UIEventBroker
{
    // ── Currency & Economy ────────────────────────────────────────────────────
    public static event Action<double>            OnCurrencyChanged;
    public static event Action<double>            OnXPChanged;
    public static event Action<int>               OnPlayerLevelUp;

    // ── Energy & Risk ─────────────────────────────────────────────────────────
    public static event Action<float, float>      OnEnergyChanged;      // current, max
    public static event Action<float>             OnRaidRiskChanged;    // [0..1]

    // ── Progression ───────────────────────────────────────────────────────────
    public static event Action<int>               OnPhaseUnlocked;      // phaseIndex
    public static event Action<int>               OnLocationChanged;    // locationIndex

    // ── Offline & Prestige ────────────────────────────────────────────────────
    public static event Action<double, TimeSpan>  OnOfflineEarningsReady; // earnings, elapsed
    public static event Action<int, double>       OnPrestigeCompleted;    // founderPts, newMult

    // ── Raise helpers — always use these to avoid null-check boilerplate ──────

    public static void RaiseCurrencyChanged(double value)
        => OnCurrencyChanged?.Invoke(value);

    public static void RaiseXPChanged(double value)
        => OnXPChanged?.Invoke(value);

    public static void RaisePlayerLevelUp(int newLevel)
        => OnPlayerLevelUp?.Invoke(newLevel);

    public static void RaiseEnergyChanged(float current, float max)
        => OnEnergyChanged?.Invoke(current, max);

    public static void RaiseRaidRiskChanged(float riskLevel)
        => OnRaidRiskChanged?.Invoke(riskLevel);

    public static void RaisePhaseUnlocked(int phaseIndex)
        => OnPhaseUnlocked?.Invoke(phaseIndex);

    public static void RaiseLocationChanged(int locationIndex)
        => OnLocationChanged?.Invoke(locationIndex);

    public static void RaiseOfflineEarningsReady(double earnings, TimeSpan elapsed)
        => OnOfflineEarningsReady?.Invoke(earnings, elapsed);

    public static void RaisePrestigeCompleted(int founderPoints, double newMultiplier)
        => OnPrestigeCompleted?.Invoke(founderPoints, newMultiplier);
}
