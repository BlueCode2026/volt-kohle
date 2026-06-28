# Volt & Kohle

Humorvolles UI-basiertes Idle Game im Hochformat (Portrait) für den Google Play Store.

Der Spieler startet als Elektriker-Azubi in Castrop-Rauxel und arbeitet sich zum globalen Tech-Giganten auf dem Mars hoch.

## Tech Stack

- **Engine:** Unity 6000.5.1f1
- **Sprache:** C# (.NET Standard 2.1)
- **Zielplattform:** Android (Google Play Store)
- **Persistenz:** Newtonsoft.Json (`com.unity.nuget.newtonsoft-json`)

## Projekt öffnen

1. **Unity Hub** installieren: [unity.com/download](https://unity.com/download)
2. Unity **6000.5.1f1** (oder neuer) über Unity Hub installieren — Android Build Support Modul mitinstallieren
3. Repo klonen:
   ```
   git clone https://github.com/BlueCode2026/volt-kohle.git
   ```
4. Unity Hub → **Open** → geklonten Ordner auswählen
5. Unity öffnet das Projekt und lädt alle Packages automatisch (inkl. Newtonsoft.Json)

## Ordnerstruktur

```
Assets/
└── _Game/
    └── Scripts/
        └── Core/           ← Sprint 1 — Foundation
            ├── UIEventBroker.cs
            ├── GameConstants.cs
            ├── NumberFormatter.cs
            ├── EconomyManager.cs
            ├── GameManager.cs
            ├── SaveData.cs
            └── SaveManager.cs
Packages/
└── manifest.json           ← Newtonsoft.Json bereits eingetragen
ProjectSettings/
└── ProjectVersion.txt      ← Unity 6000.5.1f1
```

## Implementierter Stand

| Sprint | Status | Inhalt |
|--------|--------|--------|
| Sprint 1 | Fertig | Foundation: UIEventBroker, GameConstants, NumberFormatter, EconomyManager, GameManager, SaveData, SaveManager |
| Sprint 2 | Ausstehend | Core Gameplay: ClickManager, EnergySystem, RaidRiskSystem, ProgressionManager, HUD |
| Sprint 3 | Ausstehend | Upgrades & Lifestyle |
| Sprint 4 | Ausstehend | Phase 2 & Prestige |
| Sprint 5 | Ausstehend | Phase 3 & IAP-Mock |
| Sprint 6 | Ausstehend | Phase 4 & Launch |

## Architektur-Highlights

- **UIEventBroker** — statischer C#-Action-Bus. UI-Klassen subscriben in `OnEnable()`, unsubscriben in `OnDisable()`. Kein direkter Manager-Zugriff aus der UI-Schicht.
- **GameManager** — Singleton mit `DontDestroyOnLoad`, lädt bei Start alle SaveData und berechnet Offline-Earnings.
- **NumberFormatter** — deutsches Suffix-System (K / M / Mrd / Bio / Bld / Tri / Trld) für astronomische Idle-Zahlen.
- **SaveManager** — JSON via Newtonsoft mit Auto-Save bei `OnApplicationPause` und `OnApplicationQuit`.
