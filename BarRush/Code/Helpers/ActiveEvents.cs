using System.Collections.Generic;
using Unity.Entities;

namespace BarRush
{
    // What is happening right now, for the Bars panel: DJ nights and the midnight
    // rush. The simulation systems add entries when they start something; the
    // panel reads them and they drop off once their time is up.
    public static class ActiveEvents
    {
        public struct Entry
        {
            public string Title;     // "DJ Velvet Fox", "Midnight bar rush"
            public string Place;     // bar name, or empty when it is not tied to one bar
            public int Attendees;
            public uint EndFrame;
            public Entity Building;
            public bool Security;
            public int PromotionCount;
            public bool IsCityEvent;
        }

        private static readonly List<Entry> s_Entries = new List<Entry>();
        private static uint s_LastFrame;
        private static int s_DjArrivals;

        // Number of DJ bookings created for this city since its simulation was loaded.
        public static int DjArrivals => s_DjArrivals;

        public static void RecordDjArrival() => s_DjArrivals++;

        public static void Add(string title, string place, int attendees, uint endFrame)
        {
            Add(title, place, attendees, endFrame, Entity.Null);
        }

        public static void Add(string title, string place, int attendees, uint endFrame, Entity building)
        {
            s_Entries.Add(new Entry
            {
                Title = title,
                Place = place ?? "",
                Attendees = attendees,
                EndFrame = endFrame,
                Building = building,
            });
        }

        public static void AddCityEvent(string title, string place, int attendees, uint endFrame, Entity building)
        {
            Add(title, place, attendees, endFrame, building);
            Entry entry = s_Entries[s_Entries.Count - 1];
            entry.IsCityEvent = true;
            s_Entries[s_Entries.Count - 1] = entry;
        }

        public static bool HasActiveCityEvent(uint frame)
        {
            Prune(frame);
            foreach (Entry entry in s_Entries) if (entry.IsCityEvent) return true;
            return false;
        }

        public static bool TryGet(uint frame, int index, out Entry entry)
        {
            Prune(frame);
            if (index >= 0 && index < s_Entries.Count)
            {
                entry = s_Entries[index];
                return true;
            }

            entry = default;
            return false;
        }

        public static bool AddAttendees(uint frame, int index, int amount)
        {
            Prune(frame);
            if (index < 0 || index >= s_Entries.Count || amount <= 0)
            {
                return false;
            }

            Entry entry = s_Entries[index];
            entry.Attendees += amount;
            entry.PromotionCount++;
            s_Entries[index] = entry;
            return true;
        }

        public static bool HireSecurity(uint frame, int index)
        {
            Prune(frame);
            if (index < 0 || index >= s_Entries.Count || s_Entries[index].Security)
            {
                return false;
            }

            Entry entry = s_Entries[index];
            if (entry.Building == Entity.Null)
            {
                return false;
            }

            entry.Security = true;
            s_Entries[index] = entry;
            return true;
        }

        public static bool HasSecurityAt(Entity building, uint frame)
        {
            Prune(frame);
            foreach (Entry entry in s_Entries)
            {
                if (entry.Security && entry.Building == building)
                {
                    return true;
                }
            }

            return false;
        }

        private static void Prune(uint frame)
        {
            if (frame < s_LastFrame)
            {
                s_Entries.Clear();
                s_DjArrivals = 0;
                FamousSimRoster.ResetVisits();
            }

            s_LastFrame = frame;
            s_Entries.RemoveAll(e => e.EndFrame <= frame);
        }

        // Entries still running at `frame`. Finished ones are dropped.
        public static List<Entry> Current(uint frame)
        {
            // A frame counter that goes backwards means a different save was loaded.
            Prune(frame);
            var result = new List<Entry>(s_Entries.Count);
            var seenCityEvents = new HashSet<string>();
            foreach (Entry entry in s_Entries)
            {
                if (entry.IsCityEvent)
                {
                    string key = $"{entry.Title}|{entry.Place}|{entry.EndFrame}";
                    if (!seenCityEvents.Add(key)) continue;
                }
                result.Add(entry);
            }
            return result;
        }
    }
}
