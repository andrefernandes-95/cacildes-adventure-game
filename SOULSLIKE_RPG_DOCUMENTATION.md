# Soulslike RPG - Technical Documentation

## Project Overview

This is a comprehensive Unity-based Soulslike RPG featuring advanced combat mechanics, character progression, AI systems, and extensive content. The project demonstrates professional architecture with modular design patterns and extensive third-party integrations.

**Engine:** Unity (C#)  
**Genre:** Soulslike Action RPG  
**Architecture:** Component-based with Abstract Base Classes  
**Localization:** English, Portuguese, Spanish, French, German, Ukrainian

---

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Core Systems](#core-systems)
3. [Combat System](#combat-system)
4. [Character System](#character-system)
5. [AI System](#ai-system)
6. [Inventory & Equipment](#inventory--equipment)
7. [Stats & Progression](#stats--progression)
8. [Save System](#save-system)
9. [Third-Party Dependencies](#third-party-dependencies)
10. [Code Quality Issues](#code-quality-issues)
11. [Recommended Improvements](#recommended-improvements)

---

## Architecture Overview

### Design Pattern: Abstract Base Classes + Composition

The project uses a hierarchical architecture where abstract base classes define interfaces and concrete implementations provide player/enemy-specific behavior.

```
CharacterBaseManager (Abstract)
├── PlayerManager (Player Implementation)
└── CharacterManager (Enemy/NPC Implementation)

CharacterBaseStats (Abstract)
├── PlayerStats
└── CharacterStats

CharacterBaseHealth (Abstract)
├── PlayerHealth
└── CharacterHealth
```

### Key Architectural Principles

1. **Separation of Concerns**: Each system is isolated in its own namespace/folder
2. **Event-Driven Communication**: Uses TigerForge EventManager for decoupled messaging
3. **ScriptableObject Data**: Configuration stored in ScriptableObjects (Combatant, Game, etc.)
4. **Component Composition**: Heavy use of MonoBehaviour components attached to GameObjects
5. **Manager Pattern**: Central managers coordinate subsystems (PlayerManager has 50+ sub-managers)

### Directory Structure

```
Assets/Scripts/
├── AI/                    # State machine and AI behaviors
├── Combat/                # Combat actions, damage, hit reactions
├── Player/                # Player-specific controllers
├── Characters/            # Base character classes
├── Stats/                 # Stat management and scaling
├── Health/                # Health, damage, death systems
├── Inventory/             # Item management
├── Equipment/             # Equipment and weapons
├── Abilities/             # Special abilities
├── Status/                # Status effects (poison, paralysis, etc.)
├── Dodge/                 # Dodge mechanics
├── Defense/               # Blocking and parrying
├── Backstab/              # Backstab system
├── Quest/                 # Quest system
├── Dialogue/              # NPC dialogue
├── Save/                  # Save/load system
├── UI/                    # User interface
├── Bonfires/              # Rest points
├── Crafting/              # Crafting system
├── Shop/                  # Merchant system
└── System/                # Core game systems
```

---

## Core Systems

### 1. Character System

**Base Class:** `CharacterBaseManager`

All characters (player and enemies) inherit from this abstract base class which provides:
- Animation control
- State management (busy/idle)
- Faction system
- Material management (for petrification effects)
- Root motion handling

**Player Implementation:** `PlayerManager`
- Central hub with 50+ component managers
- Handles all player-specific functionality
- Manages equipment, combat, movement, UI, etc.

**Enemy Implementation:** `CharacterManager`
- AI-driven behavior
- NavMesh pathfinding
- Target tracking
- State machine integration

### 2. Combat System

**Core Components:**
- `CharacterCombatController`: Manages combat actions and abilities
- `CombatAction`: Individual attack definitions
- `Damage`: Damage calculation with multiple types
- `HitReaction`: Response to being hit

**Combat Features:**
- Multiple damage types (Physical, Fire, Frost, Magic, Lightning, Darkness, Water)
- Poise system (stagger resistance)
- Posture system (guard break mechanic)
- Combo chains (pre-combo → combo → follow-up)
- Hyper armor (uninterruptible attacks)
- Backstab executions
- Guard counters
- Dodge counters
- Stat-based damage scaling (STR, DEX, INT)

**Damage Scaling:**
```csharp
S = 2.4x
A = 2.0x
B = 1.6x
C = 1.2x
D = 0.8x
E = 0.4x
```

### 3. AI System

**State Machine Pattern:**
- `StateManager`: Handles state transitions
- `State`: Abstract base class for all states

**Available States:**
1. `IdleState` - Standing idle
2. `PatrolState` - Patrolling waypoints
3. `ChaseState` - Pursuing target
4. `CombatState` - Engaging in combat
5. `AmbushState` - Waiting to ambush
6. `FleeState` - Running away
7. `ActivityState` - Performing activities
8. `GreetingState` - Greeting player
9. `JumpState` - Jumping to reach target
10. `TeleportState` - Teleporting
11. `VampireCombatState` - Special boss behavior

**AI Features:**
- NavMesh pathfinding
- Target detection and tracking
- Distance-based action selection
- Health-based behavior changes
- Reaction system (dodge counters, backstab reactions)

### 4. Stats System

**Core Stats:**
- Vitality (Health)
- Endurance (Stamina)
- Intelligence (Magic)
- Strength (Physical damage)
- Dexterity (Finesse damage)
- Reputation (NPC interactions)

**Derived Attributes:**
- Maximum Health
- Maximum Stamina
- Maximum Mana
- Maximum Poise
- Maximum Posture
- Carrying Weight

**Resistances:**
- Pierce, Blunt, Slash absorption
- Elemental resistances (Fire, Frost, Lightning, Magic, Darkness, Water)
- Status effect resistances
- Poise damage absorption
- Posture damage absorption

### 5. Inventory System

**Item Categories:**
- Weapons (including Shields)
- Armor (Helmet, Chest, Gauntlets, Legwear)
- Accessories
- Spells
- Arrows
- Consumables
- Key Items
- Crafting Materials
- Upgrade Materials

**Features:**
- Unique item IDs
- Item upgrading (levels)
- Equipment effects
- Weight management
- Favorite items system

### 6. Equipment System

**Equipment Types:**
- Weapons: Melee, Ranged, Shields
- Armor: Full set system
- Accessories: Stat bonuses and special effects

**Equipment Effects:**
- Stat bonuses (Vitality, Strength, etc.)
- Damage bonuses
- Resistance bonuses
- Special abilities (double coins, health restoration, etc.)
- Animation overrides

### 7. Status Effects System

**Status Effects:**
- Poison
- Paralysis
- Petrify
- Confusion
- Slow Down
- Weakness
- Bleeding
- Burning
- Freezing

**Features:**
- Resistance values per character
- Delay rates (buildup speed)
- Status effect behaviors (abstract pattern)
- Visual feedback

### 8. Abilities System

**Ability Types:**
- Combat abilities
- Chase abilities
- Reaction abilities
- Health-dependent abilities

**Features:**
- Cooldown management
- Distance requirements
- Health conditions
- Frequency/probability settings

### 9. Quest System

**Components:**
- `QuestParent`: Base quest class
- `QuestObjective`: Individual objectives
- `QuestsDatabase`: Quest catalog

**Features:**
- Multi-objective quests
- Quest tracking
- Quest rewards
- Quest progression events

### 10. Save System

**Implementation:** QuickSave plugin (CI.QuickSave)

**Saved Data:**
- Player stats and level
- Equipment (weapons, armor, accessories)
- Inventory (all item types)
- Quest progress
- World flags
- Scene settings
- Bonfire states
- Companion states
- Recipes
- Game session settings

**Features:**
- Multiple save slots
- Screenshot thumbnails
- Version compatibility checking
- Game Over recovery
- New Game+ support

---

## Combat System (Detailed)

### Damage Calculation

```csharp
Total Damage = Base Damage + Scaled Damage + Elemental Damage
Scaled Damage = (Stat × Scaling Multiplier) / SCALING_LEVEL_MULTIPLIER
Final Damage = Total Damage × (1 - Absorption) × Bonus Multiplier
```

### Damage Types

1. **Physical Damage**
   - Affected by weapon attack type (Pierce, Blunt, Slash)
   - Scales with STR/DEX
   - Reduced by armor absorption

2. **Elemental Damage**
   - Fire, Frost, Lightning, Magic, Darkness, Water
   - Scales with INT
   - Reduced by elemental resistances

3. **Posture Damage**
   - Breaks guard when posture bar fills
   - Opens enemy for critical attack

4. **Poise Damage**
   - Staggers enemy when poise breaks
   - Interrupts attacks

### Combat Action System

**CombatAction Properties:**
- Animation clips (attack, combo chains)
- Damage configuration
- Distance requirements (min/max)
- Health conditions
- Cooldown timers
- Frequency (probability of use)
- Animation speed modifier
- Hyper armor flag

**Combo System:**
```
Light Attack → Pre-Pre Combo → Pre Combo → Combo → Follow-up
```

### Hit Reactions

Different reactions based on:
- Damage amount
- Damage type
- Character state (blocking, dodging, etc.)
- Poise remaining
- Posture remaining

---

## Character System (Detailed)

### PlayerManager Components

**Movement:**
- ThirdPersonController
- ClimbController
- LadderController

**Combat:**
- PlayerCombatController
- PlayerBlockController
- PlayerBackstabController
- TwoHandingController
- PlayerDodgeController

**Stats:**
- PlayerStats
- StaminaStatManager
- ManaManager
- RageManager
- PlayerPoise
- PlayerPosture

**Equipment:**
- PlayerWeaponsManager
- PlayerInventory
- EquipmentDatabase
- FavoriteItemsManager

**Abilities:**
- PlayerAbilityManager
- PlayerShooter
- ProjectileSpawner

**Buffs/Debuffs:**
- PlayerBuffManager
- PlayerWeaponBuffManager
- PlayerWeaknessesManager
- PlayerConsumableManager

**UI:**
- UIDocumentPlayerHUDV2
- UIDocumentAlert

**Other:**
- LockOnManager
- ExecutionerManager
- PlayerReputation
- PlayerAchievementsManager
- CompanionsSceneManager
- PlayerActivityManager
- PlayerGesture
- FootstepListener
- DiscordNotifier

### CharacterManager Components

**Core:**
- CharacterStats
- CharacterHealth
- CharacterInventory
- CharacterCombatController

**AI:**
- StateManager
- TargetManager
- NavMeshAgent

**Combat:**
- CharacterBlockController
- CharacterPoise
- CharacterPosture
- CharacterLoot

**Other:**
- CharacterActivityManager
- CharacterSoundManager

---

## AI System (Detailed)

### State Machine Flow

```
Idle → Patrol → Chase → Combat
  ↓       ↓       ↓       ↓
Activity  ↓    Flee    Greeting
          ↓       ↓
       Ambush  Teleport
```

### State Transitions

States transition based on:
- Distance to target
- Line of sight
- Health percentage
- Combat conditions
- Time elapsed
- Custom triggers

### Combat AI

**Action Selection:**
1. Check distance to target
2. Filter actions by distance requirements
3. Filter by health conditions
4. Check cooldowns
5. Apply frequency/probability
6. Select random action from valid set

**Reaction System:**
- React to player dodge (dodge counter)
- React to backstab attempts
- React to target behind back
- React to low health

---

## Third-Party Dependencies

### Core Libraries

1. **TigerForge EventManager**
   - Event-driven communication
   - Decoupled system messaging

2. **AYellowpaper.SerializedCollections**
   - Dictionary serialization in Inspector
   - Used for status effect resistances

3. **DOTween (Demigiant)**
   - Animation tweening
   - UI animations

4. **Cinemachine**
   - Camera system
   - Lock-on camera

5. **Ink**
   - Dialogue system
   - Branching narratives

6. **QuickSave (CI.QuickSave)**
   - Save/load system
   - Binary serialization

7. **Steamworks.NET**
   - Steam integration
   - Achievements

8. **Unity Localization**
   - Multi-language support
   - String tables

9. **Unity Input System**
   - Modern input handling
   - Gamepad support

10. **NavMesh**
    - AI pathfinding
    - Navigation

### Asset Packs

1. **Synty Modular Fantasy Hero**
   - Character models
   - Customization system

2. **Polygon Series**
   - Environment assets
   - Props

3. **Animation Packs**
   - Mixamo
   - Kevin Iglesias
   - Various combat animations

4. **Audio Assets**
   - Music packs
   - SFX libraries

5. **VFX Assets**
   - Particle effects
   - Magic effects

---

## Code Quality Issues

### Critical Issues

#### 1. Obsolete Code (Marked for Removal)

**Files:**
- `EV_CompleteObjective.cs` - Entire class marked [Obsolete]
- `EV_Teleport.cs` - sceneName and spawnGameObjectName fields
- `Changelog.cs` - additions[], improvements[], bugfixes[] arrays

**Recommendation:** Remove these obsolete elements or update to new implementations.

#### 2. IKHelper.cs - Disabled System

**Issue:** The entire IK system is disabled with `return false` at the start of `CanUseIK()`.

**Location:** `Assets/Scripts/Animations/IKHelper.cs:78`

```csharp
bool CanUseIK()
{
    return false; // TODO: Remove this script
    // ... rest of logic never executes
}
```

**Recommendation:** Either complete the IK system or remove the script entirely.

#### 3. Guard Counter System - Deprecated

**Location:** `Assets/Scripts/Player/PlayerCombatController.cs:464`

```csharp
// TODO: Remove Guard Counter since we now have poking behind shields
if (playerManager.playerBlockController.isCounterAttacking)
{
    // ... logic
}
```

**Recommendation:** Remove guard counter logic if replaced by new system.

### Medium Priority Issues

#### 4. TODO Comments (Incomplete Features)

**Count:** 15+ TODO comments found

**Notable TODOs:**
1. `ViewQuests.cs:332` - "TODO Remove" (dead code)
2. `ViewComponent_GameSettings.cs:7` - "TODO: Remove" (entire class)
3. `UIDocumentBlacksmith.cs:139` - Filter system incomplete for gamepad
4. `StatsBonusController.cs:109` - Event system needs refactoring
5. `StatsBonusController.cs:648` - Rebirth system needs own class
6. `Weapon.cs:144` - Animation overrides need left/right separation
7. `AmbushState.cs:126` - Float parameter needs removal

**Recommendation:** Create tickets for each TODO and prioritize completion.

#### 5. Hack/Workaround Code

**Locations:**
1. `PlayerManager.cs:276` - "Hack to refresh lock on while switching animations"
2. `EquipmentSlots.cs:482` - Frame delay hack for focus
3. `ItemList.cs:197` - Frame delay hack for focus
4. `UIDocumentCharacterCustomization.cs:173` - Frame delay hack for focus

**Issue:** Frame delay hacks suggest UI focus system needs proper solution.

**Recommendation:** Investigate root cause and implement proper UI focus management.

#### 6. Global Event Listeners

**Location:** `StatsBonusController.cs:109`

```csharp
// TODO: This needs to be a character event, not global, 
// otherwise it will run every time the player changes his equipment!
```

**Issue:** Performance concern - events firing globally instead of per-character.

**Recommendation:** Refactor to character-specific events.

### Low Priority Issues

#### 7. Redundant Class Hierarchies

**Pattern:** Multiple layers of abstraction for similar functionality

**Examples:**
- CharacterAbstractPoise → CharacterPoise → PlayerPoise
- CharacterAbstractPosture → CharacterPosture → PlayerPosture
- CharacterBlockAbstractController → CharacterBlockController → PlayerBlockController

**Recommendation:** Evaluate if all layers are necessary. Consider consolidating.

#### 8. Incomplete Features

**Animation Pause System:**
```csharp
// TODO: Do not pause animation, it creates lots of bugs for now
// SetAnimatorSpeed(Random.Range(0.1f, 0.3f));
```

**Recommendation:** Either fix bugs and enable, or remove commented code.

---

## Unused/Potentially Removable Code

### Confirmed Unused

1. **IKHelper.cs** - Entire system disabled, marked for removal
2. **EV_CompleteObjective.cs** - Marked [Obsolete]
3. **ViewComponent_GameSettings.cs** - Marked "TODO: Remove"
4. **Guard Counter System** - Deprecated in favor of shield poking

### Potentially Unused (Requires Investigation)

#### Ambush Float Parameter
**Location:** `AmbushState.cs:126`
```csharp
// TODO: Remove float begin ambush once we finish changing 
// every ambush state enemy in the game
```

**Action:** Search for "begin ambush" animator parameter usage across all enemies.

#### Changelog Arrays
**Location:** `Changelog.cs:16-18`
```csharp
[Obsolete] public LocalizedString[] additions;
[Obsolete] public LocalizedString[] improvements;
[Obsolete] public LocalizedString[] bugfixes;
```

**Action:** Check if new changelog system is fully implemented, then remove.

#### Teleport Legacy Fields
**Location:** `EV_Teleport.cs:10-11`
```csharp
[Obsolete] public string sceneName;
[Obsolete] public string spawnGameObjectName;
```

**Action:** Verify new teleport system works, then remove old fields.

### Scripts to Investigate for Usage

To find unused scripts, search for references:

```bash
# Search for class name references in all .cs files
# If only found in the file itself, likely unused
```

**Candidates:**
1. Check all scripts in `Assets/Scripts/EditMode_Tests/` - may be test-only
2. Check `Assets/Scripts/UI Experimental/` - experimental features
3. Check `Assets/Scripts/DLC/` - DLC-specific code

---

## Recommended Improvements

### Immediate Actions (High Priority)

1. **Remove Obsolete Code**
   - Delete `EV_CompleteObjective.cs`
   - Remove obsolete fields from `EV_Teleport.cs` and `Changelog.cs`
   - Delete `ViewComponent_GameSettings.cs` if confirmed unused

2. **Remove IKHelper System**
   - Delete `IKHelper.cs`
   - Remove IKHelper components from prefabs
   - Remove related event listeners

3. **Remove Guard Counter**
   - Delete guard counter logic from `PlayerCombatController.cs`
   - Update related UI/animations

4. **Fix UI Focus Hacks**
   - Implement proper UI focus management
   - Remove frame delay workarounds

### Short-term Actions (Medium Priority)

5. **Complete TODO Items**
   - Create tickets for each TODO
   - Prioritize by impact
   - Assign to developers

6. **Refactor Event System**
   - Convert global events to character-specific
   - Improve performance for equipment changes

7. **Consolidate Class Hierarchies**
   - Evaluate necessity of abstract layers
   - Merge redundant classes where appropriate

8. **Separate Weapon Animations**
   - Implement left/right weapon animation overrides
   - Update weapon system accordingly

### Long-term Actions (Low Priority)

9. **Documentation**
   - Add XML comments to public APIs
   - Create system architecture diagrams
   - Document combat formulas

10. **Code Organization**
    - Consider namespace consolidation
    - Evaluate folder structure
    - Standardize naming conventions

11. **Performance Optimization**
    - Profile combat system
    - Optimize event listeners
    - Cache frequently accessed components

12. **Testing**
    - Add unit tests for damage calculations
    - Add integration tests for save system
    - Test edge cases in combat

---

## Performance Considerations

### Potential Bottlenecks

1. **Event System**
   - Global events firing frequently
   - Multiple listeners per event
   - Consider event pooling

2. **Animation System**
   - AnimatorOverrideController creation
   - Frequent clip swapping
   - Consider animation caching

3. **NavMesh**
   - Multiple agents pathfinding
   - Frequent destination updates
   - Consider update throttling

4. **UI Updates**
   - Health bar updates every frame
   - Consider dirty flag pattern

### Optimization Opportunities

1. **Object Pooling**
   - Projectiles
   - Damage numbers
   - Particle effects

2. **Component Caching**
   - Cache GetComponent calls
   - Store frequently accessed references

3. **Update Loop Optimization**
   - Use FixedUpdate only when needed
   - Throttle non-critical updates
   - Consider coroutines for delayed actions

---

## Security Considerations

### Save System

**Current:** Binary serialization with QuickSave

**Risks:**
- Save file tampering
- Stat manipulation
- Item duplication

**Recommendations:**
- Add checksum validation
- Encrypt sensitive data
- Validate loaded data ranges

### Modding System

**Current:** Supports custom content

**Risks:**
- Malicious mod code
- Game balance breaking
- Save corruption

**Recommendations:**
- Sandbox mod execution
- Validate mod data
- Backup saves before mod use

---

## Conclusion

This is a **well-architected, feature-rich Soulslike RPG** with professional-grade systems. The codebase demonstrates:

**Strengths:**
- Modular, extensible architecture
- Comprehensive combat mechanics
- Extensive content and features
- Professional third-party integrations
- Multi-language support

**Areas for Improvement:**
- Remove obsolete/unused code
- Complete TODO items
- Fix UI focus hacks
- Refactor event system
- Add documentation

**Overall Assessment:** The project is in good shape with minor technical debt that can be addressed incrementally. The architecture supports continued development and expansion.

---

## Appendix: File Statistics

**Total Scripts:** 1000+ C# files
**Total Lines of Code:** ~100,000+ (estimated)
**Third-Party Libraries:** 10+
**Asset Packs:** 15+
**Supported Languages:** 6
**Maps/Scenes:** 40+
**Enemy Types:** 50+
**Weapons:** 100+
**Armor Sets:** 50+
**Quests:** 30+

---

*Documentation generated: 2026-02-07*
*Version: 1.0*
