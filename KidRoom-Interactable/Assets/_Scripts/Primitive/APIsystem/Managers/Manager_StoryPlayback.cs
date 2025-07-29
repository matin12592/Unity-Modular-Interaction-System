using Assets._Scripts.Primitive.APIsystem.DataObjects;
using UnityEngine;
using System.Collections;
using Assets._Scripts.Core.Managers;
using Assets._Scripts.Core.Monobehaviours;
using Assets._Scripts.Core.ScriptableObjects;
using Assets._Scripts.Primitive.InteractSystem.Monobehaviours;

namespace Assets._Scripts.Primitive.APIsystem.Managers
{
    public class Manager_StoryPlayback : MonoBehaviour
    {
        public static Manager_StoryPlayback Instance;

        private AudioSource _currentAudioSource;
        private Canvas _currentCanvas;
        private Coroutine _currentRoutine;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public void PlayStory(Canvas canvas, AudioSource audioSource,
            string storyId, MonoBehaviour owner,
            ScriptableObject_Core_State stateOn, ScriptableObject_Core_State stateOff,
            Mono_Core_Data coreData)
        {
            Manager_APIsystem.GetStoryById(this, storyId, story =>
            {
                if (story == null)
                {
                    Debug.LogWarning($"Story not found for ID: {storyId}");
                    return;
                }

                if (_currentRoutine != null)
                {
                    StopCurrentPlayback();
                }

                _currentRoutine = owner.StartCoroutine(PlayRoutine(canvas, audioSource, story, stateOn, stateOff, coreData));
            });
        }

        private IEnumerator PlayRoutine(
            Canvas canvas, AudioSource audioSource,
            DataObject_Story story, ScriptableObject_Core_State stateOn,
            ScriptableObject_Core_State stateOff, Mono_Core_Data coreData)
        {
            _currentAudioSource = audioSource;
            _currentCanvas = canvas;

            var clip = Resources.Load<AudioClip>(story.AudioUrl);
            if (clip != null)
            {
                _currentAudioSource.clip = clip;
                _currentAudioSource.Play();
            }
            else
            {
                Debug.LogWarning("Audio not found at: " + story.AudioUrl);
            }

            canvas.gameObject.SetActive(true);
            canvas.GetComponentInChildren<TMPro.TMP_Text>().text = story.Description;
            Manager_Core.SetState(coreData, stateOn);

            if (clip != null)
                FindAnyObjectByType<Mono_InteractSystem_MouseController>()?.UpdateInteractText(clip.length);

            yield return new WaitForSeconds(clip != null ? clip.length : 3f);

            canvas.gameObject.SetActive(false);
            Manager_Core.SetState(coreData, stateOff);

            _currentAudioSource = null;
            _currentCanvas = null;
            _currentRoutine = null;
        }

        public void StopCurrentPlayback()
        {
            if (_currentRoutine == null) return;

            _currentAudioSource?.Stop();
            if (_currentCanvas != null)
                _currentCanvas.gameObject.SetActive(false);

            _currentRoutine = null;
        }
    }
}
