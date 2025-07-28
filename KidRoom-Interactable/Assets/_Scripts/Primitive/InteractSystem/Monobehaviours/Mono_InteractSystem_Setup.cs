using Assets._Scripts.Primitive.InteractSystem.Managers;
using UnityEngine;

namespace Assets._Scripts.Primitive.InteractSystem.Monobehaviours
{
    public class Mono_InteractSystem_Setup : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private LayerMask _interactableLayer;
        [SerializeField] private LayerMask _ignoreLayer;
        [SerializeField] private float _maxInteractionDistance = 100f;

        [Header("UI Settings")]
        [SerializeField] private GameObject _canvasPrefab;

        private void Start()
        {
            SetupInteractionSystem();
            SetupUi();
        }

        private void SetupInteractionSystem()
        {
            // Initialize the interaction system
            Manager_InteractSystem.Initialize(_interactableLayer, _ignoreLayer, _maxInteractionDistance);
        }

        private void SetupUi()
        {
            // Create UI system
            var uiGo = new GameObject("Interact System UI Manager");
            var worldUi = uiGo.AddComponent<Mono_InteractSystem_WorldUI>();

            if (_canvasPrefab == null) return;

            worldUi.CanvasPrefab = _canvasPrefab;
        }
    }
}

// ===== SETUP INSTRUCTIONS =====
/*
SETUP GUIDE FOR INTERACT SYSTEM:

1. LAYER SETUP:
   - Create a new layer (e.g., "Interactable")
   - Assign this layer to all interactable objects

2. CAMERA SETUP:
   - Make sure you have a Camera tagged as "MainCamera" or assign it manually
   - For fixed camera games, position your camera as needed

3. INTERACTABLE OBJECT SETUP:
   For each interactable object:
   a) Add the object to the "Interactable" layer
   b) Add Mono_Core_Data component
   c) Create and assign Attributes_InteractSystem ScriptableObject
   d) Add Mono_InteractSystem_Highlighter component
   e) Add Mono_InteractSystem_InteractableObject component
   f) Set up states and display texts in the ScriptableObject

4. SCENE SETUP:
   a) Add Mono_InteractSystem_GameSetup to an empty GameObject in your scene
   b) Configure the settings (layer mask, distance, camera)
   c) Create or assign a text display prefab for UI

5. UI TEXT PREFAB:
   Create a prefab with:
   - GameObject with Text component
   - Set font, size, color as desired
   - Add Outline component for better visibility
   - No need for Canvas (handled automatically)

6. STATES SETUP:
   - Create ScriptableObject_Core_State assets for different object states
   - Create Attributes_InteractSystem assets with state-text mappings
   - Assign these to your Mono_Core_Data components

TESTING:
- Move mouse over interactable objects to see highlight and text
- Click on objects to interact
- Press T to toggle interaction system on/off
*/