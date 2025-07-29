using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System;
using Assets._Scripts.Primitive.APIsystem.DataObjects;

namespace Assets._Scripts.Primitive.APIsystem.Managers
{
    public static class Manager_APIsystem
    {
        private static DataObject_Story[] _cachedStories;
        private const string ApiUrl = "https://68887e95adf0e59551ba2ef1.mockapi.io/api/stories";

        public static void FetchAllStories(MonoBehaviour context, Action<DataObject_Story[]> onSuccess)
        {
            if (_cachedStories != null)
            {
                onSuccess?.Invoke(_cachedStories);
                return;
            }

            context.StartCoroutine(FetchStoriesCoroutine(onSuccess));
        }

        private static IEnumerator FetchStoriesCoroutine(Action<DataObject_Story[]> onSuccess)
        {
            using var request = UnityWebRequest.Get(ApiUrl);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error fetching stories: " + request.error);
                yield break;
            }

            var json = "{\"stories\":" + request.downloadHandler.text + "}";
            var wrapper = JsonUtility.FromJson<StoryListWrapper>(json);
            _cachedStories = wrapper.stories;
            onSuccess?.Invoke(_cachedStories);
        }

        public static void GetStoryById(MonoBehaviour context, string id, Action<DataObject_Story> onSuccess)
        {
            FetchAllStories(context, stories =>
            {
                var story = stories.FirstOrDefault(s => s.Id == id);
                onSuccess?.Invoke(story);
            });
        }
    }
}