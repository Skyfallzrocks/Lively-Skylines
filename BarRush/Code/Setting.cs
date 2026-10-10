using System.Collections.Generic;
using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;

namespace BarRush
{
    [FileLocation(nameof(BarRush))]
    [SettingsUIGroupOrder(kGeneralGroup, kRushGroup, kDjGroup, kEventGroup, kCreatorGroup, kCrimeGroup, kOtherGroup)]
    [SettingsUIShowGroupName(kGeneralGroup, kRushGroup, kDjGroup, kEventGroup, kCreatorGroup, kCrimeGroup, kOtherGroup)]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";
        public const string kGeneralGroup = "General";
        public const string kRushGroup = "Rush";
        public const string kDjGroup = "Dj";
        public const string kCrimeGroup = "Crime";
        public const string kEventGroup = "Events";
        public const string kCreatorGroup = "Famous people";
        public const string kOtherGroup = "Other features";

        public Setting(IMod mod) : base(mod) { }

        [SettingsUISection(kSection, kGeneralGroup)]
        public bool Enabled { get; set; }

        // Stops the Bars panel from opening by itself when a save finishes loading.
        // Named as an opt-out on purpose: options saved by an older build do not
        // contain it, and a missing option loads as false, which here means "open".
        [SettingsUISection(kSection, kGeneralGroup)]
        public bool HideBarPanelOnLoad { get; set; }

        // How much longer an outing started by this mod lasts than the base length.
        // Applies only to outings this mod starts.
        [SettingsUISection(kSection, kGeneralGroup)]
        [SettingsUISlider(min = 0, max = 100, step = 5, scalarMultiplier = 1, unit = Unit.kPercentage)]
        public int StayLongerPercent { get; set; }

        // Upper limit on how many sims this mod has out at once, as a share of the
        // whole population. Capped at 50% (half the city).
        [SettingsUISection(kSection, kGeneralGroup)]
        [SettingsUISlider(min = 5, max = 50, step = 5, scalarMultiplier = 1, unit = Unit.kPercentage)]
        public int MaxAttendancePercent { get; set; }

        [SettingsUISection(kSection, kRushGroup)]
        public bool MidnightRushEnabled { get; set; }

        // Share of eligible sims who head to a bar when midnight hits.
        [SettingsUISection(kSection, kRushGroup)]
        [SettingsUISlider(min = 0, max = 100, step = 5, scalarMultiplier = 1, unit = Unit.kPercentage)]
        public int RushPercent { get; set; }

        [SettingsUISection(kSection, kDjGroup)]
        public bool DjEventsEnabled { get; set; }

        // Chance, each evening, that any DJ event happens at all.
        [SettingsUISection(kSection, kDjGroup)]
        [SettingsUISlider(min = 0, max = 100, step = 5, scalarMultiplier = 1, unit = Unit.kPercentage)]
        public int DjEventChancePercent { get; set; }

        [SettingsUISection(kSection, kDjGroup)]
        [SettingsUISlider(min = 1, max = 3, step = 1, scalarMultiplier = 1, unit = Unit.kInteger)]
        public int DjEventsPerNight { get; set; }

        [SettingsUISection(kSection, kEventGroup)]
        public bool CityEventsEnabled { get; set; }

        [SettingsUISection(kSection, kEventGroup)]
        public bool EventPurchasesEnabled { get; set; }

        [SettingsUISection(kSection, kCreatorGroup)]
        public bool FamousPeopleEnabled { get; set; }

        [SettingsUISection(kSection, kCrimeGroup)]
        public bool CrimeReportsEnabled { get; set; }

        [SettingsUISection(kSection, kCrimeGroup)]
        public bool CrimeTweetsEnabled { get; set; }

        [SettingsUISection(kSection, kCrimeGroup)]
        public bool CrimeReportHidingEnabled { get; set; }

        [SettingsUISection(kSection, kCrimeGroup)]
        public bool CrimeChangesCompany { get; set; }

        [SettingsUISection(kSection, kOtherGroup)]
        public bool UnderageSalesEnabled { get; set; }

        [SettingsUISection(kSection, kOtherGroup)]
        public bool HomelessOutreachEnabled { get; set; }

        [SettingsUISection(kSection, kOtherGroup)]
        public bool RoadClosuresEnabled { get; set; }

        public override void SetDefaults()
        {
            Enabled = true;
            HideBarPanelOnLoad = false;
            StayLongerPercent = 50;
            MaxAttendancePercent = 25;
            RushPercent = 25;
            MidnightRushEnabled = true;
            DjEventsEnabled = true;
            DjEventChancePercent = 60;
            DjEventsPerNight = 1;
            CrimeChangesCompany = true;
            CityEventsEnabled = true;
            EventPurchasesEnabled = true;
            FamousPeopleEnabled = true;
            CrimeReportsEnabled = true;
            CrimeTweetsEnabled = true;
            CrimeReportHidingEnabled = true;
            UnderageSalesEnabled = true;
            HomelessOutreachEnabled = true;
            RoadClosuresEnabled = true;
        }
    }

    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Bar Rush" },
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Main" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kGeneralGroup), "General" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kRushGroup), "Midnight rush" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDjGroup), "Live music events" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCrimeGroup), "Crime" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kEventGroup), "Events" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCreatorGroup), "Famous people" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kOtherGroup), "Other features" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Enabled)), "Enable Bar Rush" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.Enabled)), "Turns the whole mod on or off." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.HideBarPanelOnLoad)), "Don't open Bars panel on load" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.HideBarPanelOnLoad)), "By default a thank-you panel with all bars and live events opens when a save finishes loading. Turn this on to stop that; the tab button can still open it." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StayLongerPercent)), "Stay longer at bars" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StayLongerPercent)), "Extends outings started by this mod, as a percentage of the base outing length." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.MaxAttendancePercent)), "Max sims out at once" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.MaxAttendancePercent)), "Most sims this mod will have out at the same time, as a share of the city's population. Never more than half." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.MidnightRushEnabled)), "Enable midnight bar rush" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.MidnightRushEnabled)), "Turns the automatic midnight outing rush on or off." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RushPercent)), "Midnight rush size" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RushPercent)), "Share of eligible sims who start a night out when the clock hits midnight (still limited by the cap above)." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DjEventsEnabled)), "Enable live music events" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DjEventsEnabled)), "A small number of DIY music venues book random DJs, bands, and solo artists in the evening, with a popup announcing each act." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DjEventChancePercent)), "Chance of a live music night" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DjEventChancePercent)), "Chance each evening that a live music event happens." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DjEventsPerNight)), "Live music events per night" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DjEventsPerNight)), "How many DIY venues book an act on an evening with live music." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CityEventsEnabled)), "Enable park city events" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CityEventsEnabled)), "Allows planning small, medium, or big events at parks." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EventPurchasesEnabled)), "Enable event promotion and security" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EventPurchasesEnabled)), "Allows spending city funds to promote events or hire security." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.FamousPeopleEnabled)), "Enable famous residents and posts" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.FamousPeopleEnabled)), "Selects real city residents for the famous people list and their posts about local places." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CrimeReportsEnabled)), "Enable crime reports and cases" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CrimeReportsEnabled)), "Shows game crime events in the city feed and venue case list." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CrimeTweetsEnabled)), "Enable posts about crimes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CrimeTweetsEnabled)), "Lets famous residents post about reported crimes." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CrimeReportHidingEnabled)), "Enable paid crime report hiding" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CrimeReportHidingEnabled)), "Allows the city to pay to remove a report from the public list." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UnderageSalesEnabled)), "Enable underage-sale incidents" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UnderageSalesEnabled)), "Enables rare fictional underage-sale reports and city response choices." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.HomelessOutreachEnabled)), "Enable homeless outreach at venues" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.HomelessOutreachEnabled)), "Routes a small number of unhoused residents to bars when the city rate is high." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RoadClosuresEnabled)), "Enable road closure tool" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RoadClosuresEnabled)), "Shows the Shut Down Road control while road construction tools are open." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CrimeChangesCompany)), "Crime changes the bar's company" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CrimeChangesCompany)), "When a crime happens at a bar, its company is removed and the building gets a new tenant." },
            };
        }

        public void Unload() { }
    }
}
