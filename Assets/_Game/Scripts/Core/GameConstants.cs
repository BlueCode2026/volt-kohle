/// <summary>
/// Centralised game balance constants. Designers adjust values here only —
/// no magic numbers anywhere else in the codebase.
/// </summary>
public static class GameConstants
{
    // ── Offline Progress ──────────────────────────────────────────────────────
    public const int    OFFLINE_EARNINGS_CAP_HOURS  = 12;

    // ── Click Economy ─────────────────────────────────────────────────────────
    public const double BASE_CLICK_VALUE             = 1.0;

    // ── Passive Income ────────────────────────────────────────────────────────
    /// <summary>Seconds between each passive-income tick in EconomyManager.</summary>
    public const float  PASSIVE_INCOME_TICK_SECONDS  = 1f;

    // ── Prestige ("Insolvenz-Pivot") ──────────────────────────────────────────
    /// <summary>GruenderPunkte = floor(sqrt(totalEarned / PRESTIGE_COST_DIVISOR))</summary>
    public const double PRESTIGE_COST_DIVISOR        = 1_000_000.0;
    /// <summary>globalMult = 1 + (totalFounderPts * PRESTIGE_MULTIPLIER_PER_PT)</summary>
    public const double PRESTIGE_MULTIPLIER_PER_PT   = 0.02;

    // ── Progression ───────────────────────────────────────────────────────────
    public const double XP_PER_LEVEL_BASE            = 100.0;
    public const double XP_LEVEL_SCALING_FACTOR      = 1.5;

    // ── Save System ───────────────────────────────────────────────────────────
    public const string SAVE_FILE_NAME               = "volt_kohle_save.json";
    public const int    CURRENT_SAVE_SCHEMA_VERSION  = 1;

    // ── Raid Risk (Phase 1) ───────────────────────────────────────────────────
    public const float  RAID_RISK_INCREMENT          = 0.15f; // per Schwarzarbeit click
    public const float  RAID_RISK_DECAY_PER_SEC      = 0.005f;

    // ── Caffeine Boost (Phase 2) ──────────────────────────────────────────────
    public const double CAFFEINE_BOOST_BASE          = 1.0;
}
