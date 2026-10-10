using System.Collections.Generic;
using Game;
using Game.Citizens;
using Game.Common;
using Game.Simulation;
using Game.UI;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace BarRush
{
    // Each evening, with some chance, picks random bars and random DJ names, sends
    // sims their way, and queues a popup: "DJ X is playing at Y, N sims coming".
    //
    // The number of sims is a few percent of the city and is always limited by the
    // shared attendance cap (see PopulationCap / AttendanceLedger).
    //
    // VERIFY (Game.dll): the same clock and Leisure assumptions as BarRushSystem,
    // plus NameSystem.GetRenderedLabelName. Bars are announced by the name of the
    // company; if the game returns a localization key instead of readable text,
    // resolve it through the localization manager.
    //
    // KNOWN LIMIT: starting a leisure outing does not by itself send sims to the
    // announced bar. Steering them to that specific building is the next piece of
    // work after the outings are confirmed to start.
    public partial class DjEventSystem : GameSystemBase
    {
        private const float kEventTime = 21f / 24f; // 9 pm if 0 = midnight (VERIFY orientation)

        private SimulationSystem m_SimulationSystem;
        private TimeSystem m_TimeSystem;
        private NameSystem m_NameSystem;
        private EntityQuery m_CandidateQuery;
        private EntityQuery m_PopulationQuery;
        private EntityQuery m_CompanyQuery;
        private readonly List<Entity> m_Bars = new List<Entity>();
        private float m_PreviousTime = -1f;

        protected override void OnCreate()
        {
            base.OnCreate();

            m_SimulationSystem = World.GetOrCreateSystemManaged<SimulationSystem>();
            m_TimeSystem = World.GetOrCreateSystemManaged<TimeSystem>();
            m_NameSystem = World.GetOrCreateSystemManaged<NameSystem>();

            m_CandidateQuery = GetEntityQuery(
                ComponentType.ReadOnly<Citizen>(),
                ComponentType.ReadOnly<CurrentBuilding>(),
                ComponentType.Exclude<Leisure>(),
                ComponentType.Exclude<Deleted>(),
                ComponentType.Exclude<Temp>());

            m_PopulationQuery = GetEntityQuery(
                ComponentType.ReadOnly<Citizen>(),
                ComponentType.Exclude<Deleted>(),
                ComponentType.Exclude<Temp>());

            m_CompanyQuery = BarFinder.CreateCompanyQuery(EntityManager);
        }

        public override int GetUpdateInterval(SystemUpdatePhase phase) => 64;

        protected override void OnUpdate()
        {
            var settings = Mod.Settings;
            if (settings == null || !settings.Enabled || !settings.DjEventsEnabled)
            {
                return;
            }

            float now = m_TimeSystem.normalizedTime; // VERIFY
            bool reachedEventTime = m_PreviousTime >= 0f && m_PreviousTime < kEventTime && now >= kEventTime;
            m_PreviousTime = now;

            if (reachedEventTime)
            {
                RunEvening(settings);
            }
        }

        private void RunEvening(Setting settings)
        {
            uint frame = m_SimulationSystem.frameIndex;
            var random = new Random(math.max(1u, frame * 2246822519u));

            if (random.NextInt(100) >= settings.DjEventChancePercent)
            {
                return;
            }

            BarFinder.CollectBarBuildings(EntityManager, m_CompanyQuery, m_Bars);
            if (m_Bars.Count == 0)
            {
                return;
            }

            int population = m_PopulationQuery.CalculateEntityCount();
            int cap = PopulationCap.MaxAttendees(population, settings.MaxAttendancePercent);

            uint baseOuting = OutingStarter.FramesPerDay / 8u; // VERIFY: placeholder length
            uint outing = baseOuting + (uint)(baseOuting * (settings.StayLongerPercent / 100f));

            using NativeArray<Entity> candidates = m_CandidateQuery.ToEntityArray(Allocator.Temp);

            int events = math.min(settings.DjEventsPerNight, m_Bars.Count);
            for (int i = 0; i < events; i++)
            {
                int remaining = AttendanceLedger.Remaining(cap, frame);
                if (remaining <= 0)
                {
                    break; // The cap is used up; no more events tonight.
                }

                // Each event gets a different bar.
                int index = random.NextInt(m_Bars.Count);
                Entity bar = m_Bars[index];
                m_Bars.RemoveAt(index);

                int wanted = math.max(1, (population * random.NextInt(2, 11)) / 100); // 2%..10%
                int allowed = math.min(wanted, remaining);

                int started = OutingStarter.Start(
                    EntityManager, candidates, allowed, ref random, frame, outing);
                AttendanceLedger.Add(started, frame + outing);

                string dj = DjNames.Pick(ref random);
                string barName = GetBarName(bar);
                DjAnnouncement.Enqueue(dj, barName, started);
                ActiveEvents.Add(dj, barName, started, frame + outing);

                Mod.log.Info($"DJ event: {dj} at {barName}, {started} sims (cap {cap}, population {population}).");
            }
        }

        private string GetBarName(Entity building)
        {
            Entity company = BarFinder.FindBarCompany(EntityManager, m_CompanyQuery, building);
            Entity target = company != Entity.Null ? company : building;

            string name = m_NameSystem.GetRenderedLabelName(target); // VERIFY: name and return type
            return string.IsNullOrEmpty(name) ? "a local bar" : name;
        }
    }
}
