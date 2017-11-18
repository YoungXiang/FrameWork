using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public class EntityManager : Singleton<EntityManager>
    {
        public List<Entity> allEntities;
        public List<int> allActiveEntityIndices = new List<int>();

        /// <summary>
        /// key - uid, value - index in allEntities 
        /// </summary>
        private Dictionary<int, int> activeMap = new Dictionary<int, int>();
        private Dictionary<int, int> deactiveMap = new Dictionary<int, int>();

        public Entity NewEntity()
        {
            if (deactiveMap.Count <= 0)
            {
                Entity ent = new Entity();
                allEntities.Add(ent);
                activeMap.Add(ent.uID, allEntities.Count - 1);
                allActiveEntityIndices.Add(allEntities.Count - 1);

                return ent;
            }

            int k = deactiveMap.First().Key;
            int i = deactiveMap[k];
            allActiveEntityIndices.Add(i);

            Entity entCache = allEntities[i];
            activeMap.Add(entCache.uID, i);
            deactiveMap.Remove(k);
            
            return entCache;
        }

        public void RemoveEntity(Entity ent)
        {
            ent.Clear();
            allActiveEntityIndices.Remove(activeMap[ent.uID]);

            deactiveMap.Add(ent.uID, activeMap[ent.uID]);
            activeMap.Remove(ent.uID);
        }

        public Entity GetActiveEntity(int uID)
        {
            if (activeMap.ContainsKey(uID))
            {
                return allEntities[activeMap[uID]];
            }

            return null;
        }
    }
}
