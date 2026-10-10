using System.Collections.Generic;
using Colossal.UI.Binding; // VERIFY: namespace of GetterValueBinding / TriggerBinding
using Game.UI;
using UnityEngine;

namespace BarRush
{
    // Shared state between the simulation (which announces events) and the UI
    // system below (which hands the current announcement to the popup).
    public static class DjAnnouncement
    {
        private struct Item
        {
            public string Dj;
            public string Bar;
            public int Attendees;
        }

        private static readonly Queue<Item> s_Queue = new Queue<Item>();

        public static bool Visible { get; private set; }
        public static string DjName { get; private set; } = "";
        public static string BarName { get; private set; } = "";
        public static int Attendees { get; private set; }
        public static float ShownAt { get; private set; }

        public static void Enqueue(string dj, string bar, int attendees)
        {
            s_Queue.Enqueue(new Item { Dj = dj, Bar = bar, Attendees = attendees });
        }

        // Shows the next queued announcement if none is on screen.
        public static void TryAdvance(float now)
        {
            if (Visible || s_Queue.Count == 0)
            {
                return;
            }

            Item item = s_Queue.Dequeue();
            DjName = item.Dj;
            BarName = item.Bar;
            Attendees = item.Attendees;
            ShownAt = now;
            Visible = true;
        }

        public static void Dismiss()
        {
            Visible = false;
        }
    }

    // Feeds the popup. The React side lives in ui/ and reads these bindings.
    public partial class DjAnnouncementUISystem : UISystemBase
    {
        private const string kGroup = "BarRush";
        private const float kVisibleSeconds = 12f;

        protected override void OnCreate()
        {
            base.OnCreate();

            AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "visible", () => DjAnnouncement.Visible));
            AddUpdateBinding(new GetterValueBinding<string>(kGroup, "djName", () => DjAnnouncement.DjName));
            AddUpdateBinding(new GetterValueBinding<string>(kGroup, "barName", () => DjAnnouncement.BarName));
            AddUpdateBinding(new GetterValueBinding<int>(kGroup, "attendees", () => DjAnnouncement.Attendees));

            // VERIFY: the parameterless TriggerBinding(group, name, Action) overload.
            AddBinding(new TriggerBinding(kGroup, "dismiss", DjAnnouncement.Dismiss));
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            float now = Time.realtimeSinceStartup;
            if (DjAnnouncement.Visible && now - DjAnnouncement.ShownAt > kVisibleSeconds)
            {
                DjAnnouncement.Dismiss();
            }

            DjAnnouncement.TryAdvance(now);
        }
    }
}
