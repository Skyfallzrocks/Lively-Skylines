using System.Collections.Generic;
using Game;
using Game.Common;
using Game.Events;
using Unity.Collections;
using Unity.Entities;

namespace BarRush
{
    // When a crime happens at a bar, the company running it is removed so the
    // building goes vacant and the game's normal commercial logic brings in a new
    // one.
    //
    // VERIFY (Game.dll):
    //  - AccidentSite and AccidentSiteFlags.CrimeScene (Game.Events). Crimes are
    //    assumed to be marked as an accident site on the building where they happen.
    //  - That adding Deleted to a company entity is how the game itself retires
    //    companies, and that nothing else (workers, resources, vehicles) needs
    //    cleaning up by hand.
    //
    // KNOWN LIMIT: the game picks the replacement tenant. It may not be another bar.
    public partial class CrimeCompanySwapSystem : GameSystemBase
    {
        private EntityQuery m_CompanyQuery;
        private EntityQuery m_AccidentSiteQuery;
        private readonly HashSet<Entity> m_Handled = new HashSet<Entity>();

        protected override void OnCreate()
        {
            base.OnCreate();

            m_CompanyQuery = BarFinder.CreateCompanyQuery(EntityManager);
            m_AccidentSiteQuery = GetEntityQuery(
                ComponentType.ReadOnly<AccidentSite>(),
                ComponentType.Exclude<Deleted>(),
                ComponentType.Exclude<Temp>());

            RequireForUpdate(m_AccidentSiteQuery);
        }

        public override int GetUpdateInterval(SystemUpdatePhase phase) => 64;

        protected override void OnUpdate()
        {
            var settings = Mod.Settings;
            if (settings == null || !settings.Enabled || !settings.CrimeChangesCompany)
            {
                return;
            }

            m_Handled.RemoveWhere(e => !EntityManager.Exists(e));

            using NativeArray<Entity> sites = m_AccidentSiteQuery.ToEntityArray(Allocator.Temp);
            foreach (Entity building in sites)
            {
                AccidentSite site = EntityManager.GetComponentData<AccidentSite>(building);
                if ((site.m_Flags & AccidentSiteFlags.CrimeScene) == 0)
                {
                    continue;
                }

                // One swap per incident, even though the site lingers for a while.
                Entity incident = site.m_Event != Entity.Null ? site.m_Event : building;
                if (!m_Handled.Add(incident))
                {
                    continue;
                }

                Entity company = BarFinder.FindBarCompany(EntityManager, m_CompanyQuery, building);
                if (company == Entity.Null)
                {
                    continue; // The crime was not at a bar.
                }

                EntityManager.AddComponent<Deleted>(company);
                Mod.log.Info($"Crime at a bar: removed company {company.Index} so the building can get a new one.");
            }
        }
    }
}
