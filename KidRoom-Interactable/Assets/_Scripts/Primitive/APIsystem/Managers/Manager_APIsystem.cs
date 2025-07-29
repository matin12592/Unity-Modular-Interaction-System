using System.Linq;
using Assets._Scripts.Primitive.APIsystem.DataObjects;
using UnityEngine;

namespace Assets._Scripts.Primitive.APIsystem.Managers
{
    public static class Manager_APIsystem
    {
        private static DataObject_Story[] _cachedStories;

        public static DataObject_Story[] GetAllStories()
        {
            if (_cachedStories != null) return _cachedStories;

            var jsonFile = Resources.Load<TextAsset>("stories");
            var wrapper = JsonUtility.FromJson<StoryListWrapper>(jsonFile.text);
            _cachedStories = wrapper.stories;
            return _cachedStories;
        }

        public static DataObject_Story GetStoryById(string id)
        {
            var stories = GetAllStories();
            return stories.FirstOrDefault(story => story.Id == id);
        }
    }
}