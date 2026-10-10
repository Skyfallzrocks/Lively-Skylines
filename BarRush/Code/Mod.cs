using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;

namespace BarRush
{
    // Entry point. The in-game "Create Mod" template generates a file like this;
    // keep your template's namespace/class names if they differ and adjust the
    // other files to match.
    public class Mod : IMod
    {
        public static ILog log = LogManager
            .GetLogger($"{nameof(BarRush)}.{nameof(Mod)}")
            .SetShowsErrorsInUI(false);

        public static Setting Settings { get; private set; }

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            Settings = new Setting(this);
            Settings.RegisterInOptionsUI();
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(Settings));
            AssetDatabase.global.LoadSettings(nameof(BarRush), Settings, new Setting(this));

            // Options saved by an older build lack the newer ones, which then load as
            // 0 / off (no outings, no DJ nights). The sliders never go that low, so a
            // value this low means the file predates them: reset to defaults.
            // VERIFY: ApplyAndSave exists on ModSetting.
            if (Settings.MaxAttendancePercent < 5 || Settings.DjEventsPerNight < 1)
            {
                Settings.SetDefaults();
                Settings.ApplyAndSave();
                log.Info("Reset Bar Rush options to defaults (saved options were from an older version).");
            }

            // Runs with the main simulation. BarRushSystem throttles itself.
            updateSystem.UpdateAt<BarRushSystem>(SystemUpdatePhase.GameSimulation);
            updateSystem.UpdateAt<DjEventSystem>(SystemUpdatePhase.GameSimulation);
            updateSystem.UpdateAt<FamousVisitorSystem>(SystemUpdatePhase.GameSimulation);
            updateSystem.UpdateAt<ParkEventPlanningSystem>(SystemUpdatePhase.GameSimulation);
            updateSystem.UpdateAt<CrimeCompanySwapSystem>(SystemUpdatePhase.GameSimulation);
            updateSystem.UpdateAt<CrimeDisclosureSystem>(SystemUpdatePhase.GameSimulation);
            updateSystem.UpdateAt<UnderageSalesSystem>(SystemUpdatePhase.GameSimulation);
            updateSystem.UpdateAt<HomelessBarOutreachSystem>(SystemUpdatePhase.GameSimulation);
            updateSystem.UpdateAt<ShutDownRoadToolSystem>(SystemUpdatePhase.ToolUpdate);
            updateSystem.UpdateAt<ShutDownRoadUISystem>(SystemUpdatePhase.UIUpdate);

            // Feeds the DJ popup. The React side is in the ui/ folder.
            updateSystem.UpdateAt<DjAnnouncementUISystem>(SystemUpdatePhase.UIUpdate);

            // Opens the bar-list window when a save finishes loading.
            updateSystem.UpdateAt<BarListUISystem>(SystemUpdatePhase.UIUpdate);
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
            if (Settings != null)
            {
                Settings.UnregisterInOptionsUI();
                Settings = null;
            }
        }
    }
}
