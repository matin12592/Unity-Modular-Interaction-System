# Unity Interactive Object System

A modular and extensible Unity framework for creating interactive objects with state management, story playback, and dynamic UI feedback. This system provides a foundation for building immersive interactive experiences with clean separation of concerns and reusable components.

## 🎮 About

This project is a collaborative effort where the interactive systems and code architecture were developed by me, while I used the 3D models and assets, which were created for my game, The Whispers(https://eqido.itch.io/the-whispers), by my friend and collaborator. The system is designed to be modular, extensible, and easy to use for creating various types of interactive objects in Unity games.

## 🏗️ System Architecture

The system is built around several interconnected modules:

### Core Systems
- **State Management**: Centralized state handling with event-driven architecture
- **Interaction System**: Mouse-based interaction with highlighting and UI feedback
- **Story Playback**: Audio story system with UI integration
- **API Integration**: External data fetching and caching

### System Connections
```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Core State    │◄──►│ Interaction      │◄──►│ Story Playback  │
│   Management    │    │ System           │    │ System          │
└─────────────────┘    └──────────────────┘    └─────────────────┘
         ▲                        ▲                       ▲
         │                        │                       │
         ▼                        ▼                       ▼
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│ Interactable    │    │ UI & Highlighting│    │ API System      │
│ Objects         │    │ System           │    │                 │
└─────────────────┘    └──────────────────┘    └─────────────────┘
```

## 🎯 Design Patterns Used

### 1. **Manager Pattern (Static Singleton)**
Centralized system management for consistent global access.

**Example:**
```csharp
public static class Manager_Core
{
    public static void SetState(Mono_Core_Data coreData, ScriptableObject_Core_State state)
    {
        // Centralized state management logic
    }
}
```

### 2. **Observer Pattern**
Event-driven communication between systems without tight coupling.

**Example:**
```csharp
// Publisher
public static event Action<Mono_Core_Data> EventOnStateChanged;

// Subscriber
Manager_Core.EventOnStateChanged += OnStateChanged;
```

### 3. **Strategy Pattern**
Different interaction behaviors through the `IInteractable` interface.

**Example:**
```csharp
public interface IInteractable
{
    void OnInteract();
    void OnHoverEnter();
    void OnHoverExit();
}

// Different strategies for different objects
public class InteractableObject_Door : Mono_InteractSystem_InteractableObject
public class InteractableObject_Light : Mono_InteractSystem_InteractableObject
```

### 4. **Template Method Pattern**
Base interaction behavior with customizable implementations.

**Example:**
```csharp
public abstract class Mono_InteractSystem_InteractableObject : MonoBehaviour, IInteractable
{
    public virtual void OnInteract()
    {
        if (!CanInteract()) return;
        DoCustomInteraction(); // Template method - subclasses override this
    }
    
    protected virtual void DoCustomInteraction() {} // Hook for subclasses
}
```

### 5. **State Pattern**
Using ScriptableObjects as state representations for flexible state management.

**Example:**
```csharp
[CreateAssetMenu(fileName = "Core_GameObject_State", menuName = "EQiDo/Core/States")]
public class ScriptableObject_Core_State : ScriptableObject
{
    public string State;
}
```

### 6. **Component Pattern (Unity ECS-style)**
Modular functionality through MonoBehaviour components.

**Example:**
```csharp
// Core data component
public class Mono_Core_Data : MonoBehaviour
// Highlighter component  
public class Mono_InteractSystem_Highlighter : MonoBehaviour
// Interactable component
public class Mono_InteractSystem_InteractableObject : MonoBehaviour
```

## 🚀 Getting Started

### Prerequisites
- Unity 2021.3 or later
- TextMeshPro package
- Outline asset (for highlighting functionality)

### Setup Instructions

1. **Initialize the Interaction System**
   ```csharp
   // Add to your scene setup script
   public class GameSetup : MonoBehaviour
   {
       void Start()
       {
           LayerMask interactableLayer = LayerMask.GetMask("Interactable");
           LayerMask ignoreLayer = LayerMask.GetMask("Ignore");
           Manager_InteractSystem.Initialize(interactableLayer, ignoreLayer, 100f);
       }
   }
   ```

2. **Create Interactive Objects**
   - Attach `Mono_Core_Data` component to your GameObject
   - Add `Mono_InteractSystem_InteractableObject` (or inherit from it)
   - Create state ScriptableObjects for your object states
   - Configure interaction attributes

3. **Setup UI System**
   - Use `Mono_InteractSystem_Setup` component for automatic setup
   - Create a Canvas prefab for interaction text display

## 🛠️ Creating Custom Interactable Objects

### Step 1: Create Your Interactable Class
```csharp
using Assets._Scripts.Primitive.InteractSystem.InteractableObjects;

public class InteractableObject_CustomItem : Mono_InteractSystem_InteractableObject
{
    [Header("Custom Settings")]
    [SerializeField] private ScriptableObject_Core_State _activeState;
    [SerializeField] private ScriptableObject_Core_State _inactiveState;
    
    private Mono_Core_Data _coreData;
    
    private void Awake()
    {
        _coreData = GetComponent<Mono_Core_Data>();
    }
    
    protected override void DoCustomInteraction()
    {
        if (_coreData == null) return;
        
        // Your custom interaction logic here
        if (Manager_Core.CompareState(_coreData, _inactiveState))
        {
            // Activate the object
            Manager_Core.SetState(_coreData, _activeState);
            // Add your activation logic
        }
        else if (Manager_Core.CompareState(_coreData, _activeState))
        {
            // Deactivate the object
            Manager_Core.SetState(_coreData, _inactiveState);
            // Add your deactivation logic
        }
    }
}
```

### Step 2: Configure in Unity Inspector
1. Create state ScriptableObjects (Right-click → Create → EQiDo → Core → States)
2. Create interaction attributes (Right-click → Create → EQiDo → InteractSystem → Attributes)
3. Assign components and configure settings

### Step 3: Set Up GameObject
1. Add `Mono_Core_Data` component
2. Add your custom interactable component
3. Add `Mono_InteractSystem_Highlighter` for visual feedback
4. Configure layer and collider settings

## 📋 Example Implementations

### Door System
```csharp
public class InteractableObject_Door : Mono_InteractSystem_InteractableObject
{
    // Rotates between open/closed states with smooth animation
    // Maps feel states to different animation durations
    // Prevents interaction during animation
}
```

### Light System
```csharp
public class InteractableObject_Light : Mono_InteractSystem_InteractableObject
{
    // Toggles light on/off with smooth intensity transitions
    // Changes color based on "feel" states from other systems
    // Uses animation curves for smooth transitions
}
```

### Story Playback Objects
```csharp
public class InteractableObject_RabbitDoll : Mono_InteractSystem_InteractableObject
{
    // Plays audio stories with UI feedback
    // Integrates with API system for story data
    // Manages playback states and prevents multiple simultaneous stories
}
```

## 🔧 System Components

### Core State Management
- `Manager_Core`: Central state management with event system
- `Mono_Core_Data`: Component holding object state data
- `ScriptableObject_Core_State`: Flexible state definitions

### Interaction System
- `Manager_InteractSystem`: Handles mouse interaction and events
- `Mono_InteractSystem_MouseController`: Raycasting and input handling
- `Mono_InteractSystem_Highlighter`: Visual feedback for interactables
- `Mono_InteractSystem_WorldUI`: 3D world space UI text display

### Story System
- `Manager_StoryPlayback`: Singleton managing story playback
- `Manager_APIsystem`: API integration with caching
- Story data objects for external content integration

### Utility Systems
- `InteractSystem_Utils`: Helper methods for state matching
- `Attributes_InteractSystem`: Configuration objects for interaction text

## 🎨 Customization Options

### State-Based Interaction Text
Configure different interaction text based on object states:
```csharp
[CreateAssetMenu(fileName = "InteractSystem_GameObject_Attributes", menuName = "EQiDo/InteractSystem/Attributes")]
public class Attributes_InteractSystem : ScriptableObject
{
    public string DefaultText = "Interact";
    public List<GameObjectStatesEntry> GameObjectStates;
}
```

### Visual Highlighting
Customize highlight appearance:
```csharp
[Header("Highlight Settings")]
public Color highlightColor = Color.yellow;
public float highlightWidth = 2f;
public Outline.Mode outlineMode = Outline.Mode.OutlineVisible;
```

### Animation Curves
Use Unity's AnimationCurve for smooth transitions:
```csharp
[SerializeField] private AnimationCurve _transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
```

## 📄 License

This project is developed as a collaborative learning experience. The code architecture and systems are available for educational purposes. 3D models and assets are created by my collaborator and should be credited accordingly.

## 🙏 Credits

- **System Design, Architecture & All Code Logic**: [Pouriya Lakaie]
- **3D Models & Visual Assets**: [Matin Khodabakhshi]
- **Outline Shader**: [Quick Outline] by [Chris Nolet] - [[Asset Store Link](https://assetstore.unity.com/packages/tools/particles-effects/quick-outline-115488)]
- **Unity Engine**: Unity Technologies

---

*Built with Unity and a passion for modular, clean code architecture.*
