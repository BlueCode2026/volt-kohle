using System;
using System.Collections.Generic;

/// <summary>
/// Plain C# data container for all persistent game state.
/// Serialised to JSON by SaveManager via Newtonsoft.Json.
///
/// Schema migration: bump CURRENT_SAVE_SCHEMA_VERSION in GameConstants and
/// add a migration block in SaveManager.LoadGame() for each version increment.
/// </summary>
[Serializable]
public class SaveData
{
    // ── Meta ──────────────────────────────────────────────────────────────────
    public int    schemaVersion  = GameConstants.CURRENT_SAVE_SCHEMA_VERSION;

    /// <summary>ISO 8601 UTC timestamp of last save — used for offline-earnings calculation.</summary>
    public string lastSaveTime   = DateTime.UtcNow.ToString("o");

    // ── Economy ───────────────────────────────────────────────────────────────
    public double currency;
    public double xp;
    public int    playerLevel    = 1;
    public double totalEarned;
    public double globalMultiplier = 1.0;

    // ── Progression ───────────────────────────────────────────────────────────
    public int    currentPhase   = 0;
    public int    locationIndex  = 0;

    // ── Prestige ──────────────────────────────────────────────────────────────
    public int    totalFounderPoints;
    public int    prestigeCount;

    // ── Upgrades (Sprint 3) ───────────────────────────────────────────────────
    /// <summary>Names/IDs of purchased UpgradeSOs.</summary>
    public List<string> purchasedUpgradeIds = new List<string>();

    // ── Private Life Tiers (Sprint 3) ─────────────────────────────────────────
    public int    housingTier    = 0;
    public int    vehicleTier    = 0;
    public int    relationshipTier = 0;

    // ── Employees (Sprint 4) ─────────────────────────────────────────────────
    public List<EmployeeSaveEntry> employees = new List<EmployeeSaveEntry>();

    // ── Stocks (Sprint 5) ────────────────────────────────────────────────────
    public List<StockHoldingSaveEntry> stockHoldings = new List<StockHoldingSaveEntry>();

    // ── Research (Sprint 6) ──────────────────────────────────────────────────
    public List<string> completedResearchIds = new List<string>();
}

/// <summary>Minimal employee record — expanded in Sprint 4.</summary>
[Serializable]
public class EmployeeSaveEntry
{
    public string employeeSoId;
    public int    count;
    public int    caffeineLevel;
}

/// <summary>Minimal stock holding — expanded in Sprint 5.</summary>
[Serializable]
public class StockHoldingSaveEntry
{
    public string tickerSymbol;
    public double sharesOwned;
    public double averageBuyPrice;
}
