using Game;
using Game.Citizens;
using Game.Common;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace BarRush
{
    // Prototype: schedules a "midnight bar rush" by starting leisure outings for a
    // slice of the population when the in-game clock crosses midnight.
    //
    // STATUS: written against the documented mod entry/system pattern, but NOT
    // compiled or run (no game assemblies were available while writing it).
    // Every line tagged VERIFY names a game type or field that must be checked in
    // a decompiler (ILSpy/dotPeek on Cities2_Data\Managed\Game.dll) and may need
    // renaming to match the current game version.
    public partial class BarRushSystem : GameSystemBase
    {
        private SimulationSystem m_SimulationSystem;
        private TimeSystem m_TimeSystem; // VERIFY: Game.Simulation.TimeSystem and normalizedTime
        private EntityQuery m_CandidateQuery;
        private EntityQuery m_PopulationQuery;
        private float m_PreviousTime = -1f;

        protected override void OnCreate()
        {
            base.OnCreate();

            m_SimulationSystem = World.GetOrCreateSystemManaged<SimulationSystem>();
            m_TimeSystem = World.GetOrCreateSystemManaged<TimeSystem>();

            // Sims who are somewhere (CurrentBuilding) and not already on a leisure
            // outing. VERIFY: Citizen, CurrentBuilding, Leisure live in Game.Citizens.
            m_CandidateQuery = GetEntityQuery(
                ComponentType.ReadOnly<Citizen>(),
                ComponentType.ReadOnly<CurrentBuilding>(),
                ComponentType.Exclude<Leisure>(),
                ComponentType.Exclude<Deleted>(),
                ComponentType.Exclude<Temp>());

            // Everyone in the city, used for the attendance cap.
            m_PopulationQuery = GetEntityQuery(
                ComponentType.ReadOnly<Citizen>(),
                ComponentType.Exclude<Deleted>(),
                ComponentType.Exclude<Temp>());

            RequireForUpdate(m_CandidateQuery);
        }

        // Check the clock a few times per in-game hour, not every frame.
        public override int GetUpdateInterval(SystemUpdatePhase phase) => 64;

        protected override void OnUpdate()
        {
            var settings = Mod.Settings;
            if (settings == null || !settings.Enabled)
            {
                return;
            }

            float now = m_TimeSystem.normalizedTime; // VERIFY: 0..1 across the day, 0 = midnight
            bool crossedMidnight = m_PreviousTime > 0.75f && now < 0.25f;
            m_PreviousTime = now;

            if (crossedMidnight)
            {
                StartRush(settings);
            }
        }

        private void StartRush(Setting settings)
        {
            if (settings.RushPercent <= 0)
            {
                return;
            }

            uint frame = m_SimulationSystem.frameIndex;
            var random = new Random(math.max(1u, frame * 2654435761u));

            // A normal evening outing, then stretched by the "stay longer" setting.
            // VERIFY: real outing length in the game; this value is a placeholder.
            uint baseOuting = OutingStarter.FramesPerDay / 12u;
            uint outing = baseOuting + (uint)(baseOuting * (settings.StayLongerPercent / 100f));

            using NativeArray<Entity> candidates = m_CandidateQuery.ToEntityArray(Allocator.Temp);

            // Aim for RushPercent of the candidates, but never past the shared cap.
            int wanted = (candidates.Length * settings.RushPercent) / 100;
            int population = m_PopulationQuery.CalculateEntityCount();
            int cap = PopulationCap.MaxAttendees(population, settings.MaxAttendancePercent);
            int allowed = math.min(wanted, AttendanceLedger.Remaining(cap, frame));

            int started = OutingStarter.Start(
                EntityManager, candidates, allowed, ref random, frame, outing);
            AttendanceLedger.Add(started, frame + outing);
            if (started > 0)
            {
                ActiveEvents.Add("Midnight bar rush", "", started, frame + outing);
            }

            Mod.log.Info(
                $"Midnight bar rush: started {started} outings (wanted {wanted}, cap {cap}, population {population}).");
        }

        // NOT IMPLEMENTED: "sims stay outside the bar".
        // The vanilla simulation has no loitering-outside state: a sim that arrives
        // at a venue is hidden inside the building until the visit ends. Showing
        // them out front means spawning or re-showing a pedestrian creature near
        // the building's entrance and keeping its path/animation state from
        // resolving. That is the hardest piece of this mod and needs
        // experimentation with the game's creature and building-access components
        // after the rush above is confirmed working.
    }
}
