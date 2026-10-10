using System.Collections.Generic;
using Game.Buildings;
using Game.Companies;
using Game.Economy;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;

namespace BarRush
{
    // Finds "bars": commercial companies whose output is the Entertainment
    // resource, and the building each one rents.
    //
    // VERIFY (all in Game.dll): CommercialCompany (Game.Companies), PropertyRenter
    // (Game.Buildings), IndustrialProcessData and PrefabRef (Game.Prefabs),
    // Resource.Entertainment (Game.Economy), and that the company's prefab, not
    // the building's, carries the process data.
    public static class BarFinder
    {
        public static EntityQuery CreateCompanyQuery(EntityManager em)
        {
            return em.CreateEntityQuery(
                ComponentType.ReadOnly<CommercialCompany>(),
                ComponentType.ReadOnly<PropertyRenter>(),
                ComponentType.Exclude<Game.Common.Deleted>(),
                ComponentType.Exclude<Game.Common.Temp>());
        }

        public static bool IsBar(EntityManager em, Entity company)
        {
            if (!em.HasComponent<PrefabRef>(company))
            {
                return false;
            }

            Entity prefab = em.GetComponentData<PrefabRef>(company).m_Prefab;
            if (!em.HasComponent<IndustrialProcessData>(prefab))
            {
                return false;
            }

            return em.GetComponentData<IndustrialProcessData>(prefab).m_Output.m_Resource
                == Resource.Entertainment;
        }

        // Fills `buildings` with every building that currently houses a bar.
        public static void CollectBarBuildings(
            EntityManager em, EntityQuery companyQuery, List<Entity> buildings)
        {
            buildings.Clear();
            using NativeArray<Entity> companies = companyQuery.ToEntityArray(Allocator.Temp);
            foreach (Entity company in companies)
            {
                if (!IsBar(em, company))
                {
                    continue;
                }

                Entity building = em.GetComponentData<PropertyRenter>(company).m_Property;
                if (building != Entity.Null && em.Exists(building))
                {
                    buildings.Add(building);
                }
            }
        }

        // Fills `companies` with every company that currently runs a bar.
        public static void CollectBarCompanies(
            EntityManager em, EntityQuery companyQuery, List<Entity> companies)
        {
            companies.Clear();
            using NativeArray<Entity> all = companyQuery.ToEntityArray(Allocator.Temp);
            foreach (Entity company in all)
            {
                if (IsBar(em, company))
                {
                    companies.Add(company);
                }
            }
        }

        // Returns the bar company renting `building`, or Entity.Null.
        public static Entity FindBarCompany(
            EntityManager em, EntityQuery companyQuery, Entity building)
        {
            using NativeArray<Entity> companies = companyQuery.ToEntityArray(Allocator.Temp);
            foreach (Entity company in companies)
            {
                if (em.GetComponentData<PropertyRenter>(company).m_Property == building
                    && IsBar(em, company))
                {
                    return company;
                }
            }

            return Entity.Null;
        }
    }
}
