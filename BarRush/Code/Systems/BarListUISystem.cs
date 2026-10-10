using System.Collections.Generic;
using Colossal.UI.Binding;
using Game;
using Game.Simulation;
using Game.UI;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace BarRush
{
    // Supplies live data and click actions for the Bars and City Events HUD panels.
    public partial class BarListUISystem : UISystemBase
    {
        private const string kGroup = "BarRush";
        private const float kRefreshSeconds = 2f;

        private NameSystem m_NameSystem;
        private SimulationSystem m_SimulationSystem;
        private EntityQuery m_CompanyQuery;
        private readonly List<Entity> m_Companies = new List<Entity>();

        private bool m_BarsVisible;
        private bool m_CityEventsVisible;
        private string m_BarsText = "";
        private string m_EventsText = "";
        private int m_BarCount;
        private int m_EventCount;
        private float m_LastRefresh;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_NameSystem = World.GetOrCreateSystemManaged<NameSystem>();
            m_SimulationSystem = World.GetOrCreateSystemManaged<SimulationSystem>();
            m_CompanyQuery = BarFinder.CreateCompanyQuery(EntityManager);

            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "barPanelVisible", () => m_BarsVisible));
            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "cityEventsVisible", () => m_CityEventsVisible));
            AddUpdateBinding(new GetterValueBinding<string>(kGroup, "barPanelBars", () => m_BarsText));
            AddUpdateBinding(new GetterValueBinding<string>(kGroup, "barPanelEvents", () => m_EventsText));
            AddUpdateBinding(new GetterValueBinding<int>(kGroup, "barPanelBarCount", () => m_BarCount));
            AddUpdateBinding(new GetterValueBinding<int>(kGroup, "cityEventsCount", () => m_EventCount));

            AddBinding(new TriggerBinding(kGroup, "toggleBarPanel", ToggleBars));
            AddBinding(new TriggerBinding(kGroup, "closeBarPanel", () => m_BarsVisible = false));
            AddBinding(new TriggerBinding(kGroup, "toggleCityEvents", ToggleCityEvents));
            AddBinding(new TriggerBinding(kGroup, "closeCityEvents", () => m_CityEventsVisible = false));
            Mod.log.Info("Bars and City Events UI system created.");
        }

        public override int GetUpdateInterval(SystemUpdatePhase phase) => 16;

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if ((m_BarsVisible || m_CityEventsVisible)
                && UnityEngine.Time.realtimeSinceStartup - m_LastRefresh >= kRefreshSeconds)
            {
                Refresh();
            }
        }

        private void ToggleBars()
        {
            m_BarsVisible = !m_BarsVisible;
            if (m_BarsVisible)
            {
                m_CityEventsVisible = false;
                Refresh();
            }
        }

        private void ToggleCityEvents()
        {
            m_CityEventsVisible = !m_CityEventsVisible;
            if (m_CityEventsVisible)
            {
                m_BarsVisible = false;
                Refresh();
            }
        }

        private void Refresh()
        {
            m_LastRefresh = UnityEngine.Time.realtimeSinceStartup;
            uint frame = m_SimulationSystem.frameIndex;

            List<ActiveEvents.Entry> events = ActiveEvents.Current(frame);
            var eventLines = new List<string>(events.Count);
            foreach (ActiveEvents.Entry entry in events)
            {
                float hoursLeft = (entry.EndFrame - frame) * 24f / OutingStarter.FramesPerDay;
                string location = string.IsNullOrEmpty(entry.Place) ? "" : $" at {entry.Place}";
                eventLines.Add($"{entry.Title}{location} — {entry.Attendees:N0} sims · about {hoursLeft:0.#} in-game hours left");
            }

            m_EventCount = eventLines.Count;
            m_EventsText = string.Join("\n", eventLines);

            BarFinder.CollectBarCompanies(EntityManager, m_CompanyQuery, m_Companies);
            var names = new List<string>(m_Companies.Count);
            foreach (Entity company in m_Companies)
            {
                string name = m_NameSystem.GetRenderedLabelName(company);
                names.Add(string.IsNullOrEmpty(name) ? "Unnamed bar" : name);
            }

            names.Sort(string.CompareOrdinal);
            m_BarCount = names.Count;
            m_BarsText = string.Join("\n", names);
        }
    }
}
