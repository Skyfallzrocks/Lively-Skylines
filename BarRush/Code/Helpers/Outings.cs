using System.Collections.Generic;
using Game.Citizens;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace BarRush
{
    // Population cap shared by the midnight rush and DJ events.
    //
    // Reading of the request: "keep attendance below 2 times below the city's
    // population" is treated as "fewer than half the population". The options page
    // lets you pick the share (5%..50%), and the cap covers every outing this mod
    // has running at the same time, not each event separately.
    public static class PopulationCap
    {
        public const int kMaxPercent = 50;

        public static int MaxAttendees(int population, int percent)
        {
            int clamped = math.clamp(percent, 0, kMaxPercent);
            return (population * clamped) / 100;
        }
    }

    // Tracks how many sims this mod has sent out and until when, so overlapping
    // events cannot add up past the cap.
    public static class AttendanceLedger
    {
        private struct Entry
        {
            public uint EndFrame;
            public int Count;
        }

        private static readonly List<Entry> s_Entries = new List<Entry>();
        private static uint s_LastFrame;

        private static void Prune(uint frame)
        {
            // A frame counter that goes backwards means a different save was loaded.
            if (frame < s_LastFrame)
            {
                s_Entries.Clear();
            }

            s_LastFrame = frame;
            s_Entries.RemoveAll(e => e.EndFrame <= frame);
        }

        public static int Remaining(int cap, uint frame)
        {
            Prune(frame);
            int committed = 0;
            foreach (Entry e in s_Entries)
            {
                committed += e.Count;
            }

            return math.max(0, cap - committed);
        }

        public static void Add(int count, uint endFrame)
        {
            if (count > 0)
            {
                s_Entries.Add(new Entry { EndFrame = endFrame, Count = count });
            }
        }
    }

    public static class OutingStarter
    {
        public const uint FramesPerDay = 262144u; // VERIFY: ticks per in-game day

        // Starts up to maxCount leisure outings among the candidates and returns
        // how many were actually started.
        public static int Start(
            EntityManager em,
            NativeArray<Entity> candidates,
            int maxCount,
            ref Random random,
            uint frame,
            uint outingFrames)
        {
            return Start(em, candidates, maxCount, ref random, frame, outingFrames, Entity.Null);
        }

        // Targeted version used by a creator recommendation so vanilla leisure
        // pathfinding sends selected residents to the featured place.
        public static int Start(
            EntityManager em,
            NativeArray<Entity> candidates,
            int maxCount,
            ref Random random,
            uint frame,
            uint outingFrames,
            Entity target)
        {
            if (maxCount <= 0 || candidates.Length == 0)
            {
                return 0;
            }

            // Pick each candidate with the chance that yields roughly maxCount.
            int chance = (int)math.ceil(100f * maxCount / candidates.Length);
            chance = math.clamp(chance, 1, 100);

            int started = 0;
            foreach (Entity citizen in candidates)
            {
                if (started >= maxCount)
                {
                    break;
                }

                // Skip sims already given an outing earlier (for example by another
                // DJ event tonight); adding the component twice is not safe.
                if (em.HasComponent<Leisure>(citizen))
                {
                    continue;
                }

                if (random.NextInt(100) >= chance || !IsEligible(citizen))
                {
                    continue;
                }

                // VERIFY: field names on Leisure, and whether adding the component
                // is enough for the vanilla leisure logic to pick a destination
                // (it may also need a LeisureType or a TripNeeded entry).
                em.AddComponentData(citizen, new Leisure
                {
                    m_TargetAgent = target,
                    m_LastPossibleFrame = frame + outingFrames,
                });
                started++;
            }

            return started;
        }

        // TODO: filter out children and teens (age lookup via the game's citizen
        // utilities; VERIFY the current helper name) and sims who are sick or
        // already traveling.
        private static bool IsEligible(Entity citizen)
        {
            return true;
        }
    }
}
