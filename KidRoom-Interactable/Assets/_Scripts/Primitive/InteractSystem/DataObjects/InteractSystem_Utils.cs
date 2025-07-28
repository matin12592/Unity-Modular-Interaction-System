using System.Collections.Generic;
using System.Linq;
using Assets._Scripts.Core.Managers;
using Assets._Scripts.Core.Monobehaviours;
using Assets._Scripts.Primitive.InteractSystem.Attributes;

namespace Assets._Scripts.Primitive.InteractSystem.DataObjects
{
    public static class InteractSystem_Utils
    {
        public static Attributes_InteractSystem.GameObjectStatesEntry GetMatchingStateEntry(
            Mono_Core_Data coreData,
            List<Attributes_InteractSystem.GameObjectStatesEntry> entries,
            bool exactMatch = true)
        {
            return entries.FirstOrDefault(entry =>
                Manager_Core.CompareState(coreData, entry.States, exactMatch));
        }
    }
}