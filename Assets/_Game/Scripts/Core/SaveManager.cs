using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// Handles serialisation of SaveData to and from a JSON file stored in
/// Application.persistentDataPath.
///
/// Dependency: com.unity.nuget.newtonsoft-json (Unity Package Manager).
/// Window -> Package Manager -> Unity Registry -> "Newtonsoft Json" -> Install.
///
/// Auto-saves on application pause and quit. Call SaveGame() manually after any
/// significant purchase or progression milestone.
/// </summary>
public class SaveManager : MonoBehaviour
{
    private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
    {
        Formatting        = Formatting.Indented,
        NullValueHandling = NullValueHandling.Ignore,
    };

    private string SaveFilePath => Path.Combine(Application.persistentDataPath, GameConstants.SAVE_FILE_NAME);

    // ── Auto-save hooks ───────────────────────────────────────────────────────

    private void OnApplicationPause(bool paused)
    {
        if (paused) RequestAutoSave();
    }

    private void OnApplicationQuit()
    {
        RequestAutoSave();
    }

    /// <summary>
    /// Called by GameManager to wire up the auto-save trigger.
    /// The GameManager provides the current SaveData snapshot.
    /// </summary>
    public event Func<SaveData> OnSaveRequested;

    private void RequestAutoSave()
    {
        SaveData data = OnSaveRequested?.Invoke();
        if (data != null) SaveGame(data);
    }

    // ── Core IO ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Serialises the given SaveData to disk. Sets lastSaveTime to UtcNow.
    /// Thread-safe: runs on the main thread only (Unity lifecycle methods).
    /// </summary>
    public void SaveGame(SaveData data)
    {
        data.lastSaveTime = DateTime.UtcNow.ToString("o");
        try
        {
            string json = JsonConvert.SerializeObject(data, SerializerSettings);
            File.WriteAllText(SaveFilePath, json);
            Debug.Log($"[SaveManager] Game saved to: {SaveFilePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] Failed to save: {e.Message}");
        }
    }

    /// <summary>
    /// Loads and deserialises SaveData from disk. Returns a fresh SaveData
    /// (new game defaults) if no file exists or if the file is corrupt.
    /// Applies schema migrations when the saved version is older than the current one.
    /// </summary>
    public SaveData LoadGame()
    {
        if (!File.Exists(SaveFilePath))
        {
            Debug.Log("[SaveManager] No save file found — starting new game.");
            return new SaveData();
        }

        try
        {
            string   json = File.ReadAllText(SaveFilePath);
            SaveData data = JsonConvert.DeserializeObject<SaveData>(json, SerializerSettings);
            data = ApplyMigrations(data);
            Debug.Log($"[SaveManager] Save loaded. Schema v{data.schemaVersion}, last save: {data.lastSaveTime}");
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] Corrupt save file, starting fresh. Error: {e.Message}");
            return new SaveData();
        }
    }

    /// <summary>
    /// Permanently deletes the save file. Intended for debug builds and the
    /// "Insolvenz-Pivot" prestige confirmation — call only after user confirms.
    /// </summary>
    public void DeleteSave()
    {
        if (!File.Exists(SaveFilePath)) return;
        File.Delete(SaveFilePath);
        Debug.Log("[SaveManager] Save file deleted.");
    }

    // ── Schema migration ──────────────────────────────────────────────────────

    private static SaveData ApplyMigrations(SaveData data)
    {
        // Add a new block here for each schema version bump in GameConstants.
        // Example for a future v2 migration:
        // if (data.schemaVersion < 2)
        // {
        //     data.someNewField = legacyDefault;
        //     data.schemaVersion = 2;
        // }

        data.schemaVersion = GameConstants.CURRENT_SAVE_SCHEMA_VERSION;
        return data;
    }

    // ── Offline earnings helper ───────────────────────────────────────────────

    /// <summary>
    /// Calculates how much passive income was generated while the app was closed.
    /// Capped at GameConstants.OFFLINE_EARNINGS_CAP_HOURS hours.
    /// Returns (earnings, elapsed). Call after LoadGame() and before UI init.
    /// </summary>
    public static (double earnings, TimeSpan elapsed) CalculateOfflineEarnings(
        SaveData data, double passiveIncomePerSec)
    {
        if (!DateTime.TryParse(data.lastSaveTime, null,
                System.Globalization.DateTimeStyles.RoundtripKind,
                out DateTime lastSave))
        {
            return (0.0, TimeSpan.Zero);
        }

        TimeSpan elapsed = DateTime.UtcNow - lastSave;
        TimeSpan cap     = TimeSpan.FromHours(GameConstants.OFFLINE_EARNINGS_CAP_HOURS);
        TimeSpan capped  = elapsed > cap ? cap : elapsed;

        double earnings = passiveIncomePerSec * capped.TotalSeconds;
        return (earnings, capped);
    }
}
