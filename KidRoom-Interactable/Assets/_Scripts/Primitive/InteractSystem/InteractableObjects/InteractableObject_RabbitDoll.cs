using System.Collections;
using Assets._Scripts.Core.Managers;
using Assets._Scripts.Core.Monobehaviours;
using Assets._Scripts.Core.ScriptableObjects;
using Assets._Scripts.Primitive.APIsystem.DataObjects;
using Assets._Scripts.Primitive.APIsystem.Managers;
using Assets._Scripts.Primitive.InteractSystem.Monobehaviours;
using UnityEngine;

namespace Assets._Scripts.Primitive.InteractSystem.InteractableObjects
{
    public class InteractableObject_RabbitDoll : Mono_InteractSystem_InteractableObject
    {
        #region Private Variables
        [SerializeField] private Canvas _canvas;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private ScriptableObject_Core_State _rabbitDollOn;
        [SerializeField] private ScriptableObject_Core_State _rabbitDollOff;

        private float _audioClipDuration;
        private string _storyId = "rabbit";
        private Mono_Core_Data _coreData;


        #endregion

        #region Private Methods
        private void Awake()
        {
            _coreData = GetComponent<Mono_Core_Data>();
        }

        protected override void DoCustomInteraction()
        {
            if (_coreData == null) return;

            if (Manager_Core.CompareState(_coreData, _rabbitDollOff))
            {
                Manager_StoryPlayback.Instance.PlayStory(
                    _canvas,
                    _audioSource,
                    _storyId,
                    this,
                    _rabbitDollOn,
                    _rabbitDollOff,
                    _coreData
                );
            }
        }
        #endregion
    }
}