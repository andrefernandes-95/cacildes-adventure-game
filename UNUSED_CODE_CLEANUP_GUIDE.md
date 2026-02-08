# Unused Code Cleanup Guide

## Overview

This document identifies code that can be safely removed from the Soulslike RPG project. Each item includes the reason for removal and any dependencies to check before deletion.

---

## Table of Contents

1. [Confirmed Safe to Delete](#confirmed-safe-to-delete)
2. [Requires Investigation](#requires-investigation)
3. [Deprecated Features](#deprecated-features)
4. [Cleanup Checklist](#cleanup-checklist)

---

## Confirmed Safe to Delete

### 1. IKHelper.cs - Disabled IK System

**File:** `Assets/Scripts/Animations/IKHelper.cs`

**Reason:** Entire system is disabled with `return false` at the start of `CanUseIK()`. The TODO comment explicitly states "Remove this script".

**Code:**
```csharp
bool CanUseIK()
{
    return false; // TODO: Remove this script
    // ... rest of logic never executes
}
```

**Dependencies to Check:**
- Search for `IKHelper` component references in prefabs
- Search for `EventMessages.ON_CAN_USE_IK_IS_TRUE` event usage
- Check if any scripts reference `IKHelper`

**Cleanup Steps:**
1. Search project for "IKHelper" references
2. Remove IKHelper components from any prefabs
3. Remove event listener registration for `ON_CAN_USE_IK_IS_TRUE`
4. Delete `IKHelper.cs`
5. Test player animations to ensure no issues

**Estimated Impact:** Low - System already disabled

---

### 2. EV_CompleteObjective.cs - Obsolete Event

**File:** `Assets/Scripts/Events/EV_CompleteObjective.cs`

**Reason:** Entire class marked with `[Obsolete]` attribute.

**Code:**
```csharp
[Obsolete]
public class EV_ProgressQuest : EventBase
{
    // ... class implementation
}
```

**Dependencies to Check:**
- Search for `EV_ProgressQuest` or `EV_CompleteObjective` usage
- Check if any scenes have this component attached
- Verify new quest completion system is in place

**Cleanup Steps:**
1. Search project for "EV_CompleteObjective" and "EV_ProgressQuest"
2. Remove from any GameObjects in scenes
3. Update any scripts that reference it
4. Delete `EV_CompleteObjective.cs`
5. Test quest completion to ensure new system works

**Estimated Impact:** Low - Already marked obsolete

---

### 3. ViewComponent_GameSettings.cs - Marked for Removal

**File:** `Assets/Scripts/Views/ViewComponent_GameSettings.cs`

**Reason:** TODO comment states "TODO: Remove" at class level.

**Code:**
```csharp
// TODO: Remove
public class ViewComponent_GameSettings : MonoBehaviour
{
    // ... class implementation
}
```

**Dependencies to Check:**
- Search for `ViewComponent_GameSettings` usage
- Check if settings UI still uses this component
- Verify replacement component exists

**Cleanup Steps:**
1. Search project for "ViewComponent_GameSettings"
2. Identify replacement component
3. Update UI prefabs to use new component
4. Delete `ViewComponent_GameSettings.cs`
5. Test settings menu functionality

**Estimated Impact:** Medium - UI component, test thoroughly

---

### 4. Obsolete Changelog Fields

**File:** `Assets/Scripts/Changelog/Changelog.cs`

**Reason:** Three array fields marked `[Obsolete]`.

**Code:**
```csharp
[Obsolete] public LocalizedString[] additions;
[Obsolete] public LocalizedString[] improvements;
[Obsolete] public LocalizedString[] bugfixes;
```

**Dependencies to Check:**
- Check if new changelog system is fully implemented
- Search for references to these fields
- Verify changelog UI doesn't use these arrays

**Cleanup Steps:**
1. Search for "additions", "improvements", "bugfixes" in Changelog context
2. Verify new changelog system works
3. Remove obsolete fields from `Changelog.cs`
4. Update any ScriptableObject instances
5. Test changelog display

**Estimated Impact:** Low - Fields already obsolete

---

### 5. Obsolete Teleport Fields

**File:** `Assets/Scripts/Events/EV_Teleport.cs`

**Reason:** Two fields marked `[Obsolete]`.

**Code:**
```csharp
[Obsolete] public string sceneName;
[Obsolete] public string spawnGameObjectName;
```

**Dependencies to Check:**
- Verify new teleport system uses different fields
- Check if any teleport events still reference these
- Search for "sceneName" and "spawnGameObjectName" in teleport context

**Cleanup Steps:**
1. Search for usage of obsolete fields
2. Verify new teleport system works
3. Remove obsolete fields from `EV_Teleport.cs`
4. Update any teleport event instances
5. Test teleportation functionality

**Estimated Impact:** Low - Fields already obsolete

---

## Requires Investigation

### 6. Guard Counter System

**Files:**
- `Assets/Scripts/Player/PlayerCombatController.cs:464`
- Related guard counter logic

**Reason:** TODO comment states "Remove Guard Counter since we now have poking behind shields".

**Code:**
```csharp
// TODO: Remove Guard Counter since we now have poking behind shields
if (playerManager.playerBlockController.isCounterAttacking)
{
    // ... guard counter logic
}
```

**Investigation Required:**
1. Verify shield poking system is fully implemented
2. Check if any enemies still trigger guard counters
3. Test blocking mechanics without guard counter
4. Verify UI doesn't reference guard counter

**Cleanup Steps (After Investigation):**
1. Search for "isCounterAttacking" references
2. Remove guard counter logic from PlayerCombatController
3. Remove guard counter animations
4. Remove guard counter UI elements
5. Update player block controller
6. Test blocking and counter-attack mechanics

**Estimated Impact:** High - Core combat mechanic, test extensively

---

### 7. Ambush State Float Parameter

**File:** `Assets/Scripts/AI/AI_States/AmbushState.cs:126`

**Reason:** TODO comment states "Remove float begin ambush once we finish changing every ambush state enemy".

**Code:**
```csharp
// TODO: Remove float begin ambush once we finish changing 
// every ambush state enemy in the game
IEnumerator BeginAmbush_Coroutine()
{
    // Uses deprecated animator parameter
}
```

**Investigation Required:**
1. Search all enemy prefabs for "begin ambush" animator parameter
2. Verify all ambush enemies use new system
3. Check if any animations reference the float parameter

**Cleanup Steps (After Investigation):**
1. List all enemies with ambush behavior
2. Update each enemy to new ambush system
3. Remove float parameter from animator controllers
4. Remove BeginAmbush_Coroutine if no longer needed
5. Test ambush behavior for all enemies

**Estimated Impact:** Medium - Affects multiple enemies

---

### 8. ViewQuests.cs Dead Code

**File:** `Assets/Scripts/Views/ViewQuests.cs:332`

**Reason:** TODO comment states "TODO Remove" for else block.

**Code:**
```csharp
else // TODO Remove
{
    if (questObjective?.objectiveImage != null)
    {
        // ... old quest UI logic
    }
}
```

**Investigation Required:**
1. Verify new quest UI system works
2. Check if this code path is ever reached
3. Test quest display with various quest types

**Cleanup Steps (After Investigation):**
1. Add logging to determine if code is reached
2. Test all quest types
3. If never reached, remove else block
4. Test quest UI thoroughly

**Estimated Impact:** Low - UI code, likely safe

---

### 9. Weapon Animation Override TODO

**File:** `Assets/Scripts/Items/Weapon.cs:144`

**Reason:** TODO comment states "This needs to be divided between right and left".

**Code:**
```csharp
// TODO: This needs to be divided between right and left 
[Tooltip("Optional")] public List<AnimationOverride> oh_weaponAnimationOverrides = new();
```

**Investigation Required:**
1. Determine if dual-wielding is implemented
2. Check if left-hand weapon animations work correctly
3. Verify if this is a future feature or current bug

**Cleanup Steps (If Implementing):**
1. Add `oh_leftWeaponAnimationOverrides` field
2. Update weapon manager to use correct overrides
3. Update all weapon ScriptableObjects
4. Test dual-wielding animations

**Cleanup Steps (If Not Implementing):**
1. Remove TODO comment
2. Document that only right-hand overrides are supported

**Estimated Impact:** Medium - Affects weapon system

---

### 10. Rebirth System in StatsBonusController

**File:** `Assets/Scripts/Stats/StatsBonusController.cs:648`

**Reason:** TODO comment states "Stuff related to rebirth, move it to its own proper class".

**Code:**
```csharp
// TODO: Stuff related to rebirth, move it to its own proper class
public void ReturnGoldAndResetStats()
{
    // Rebirth logic
}
```

**Investigation Required:**
1. Identify all rebirth-related code
2. Determine scope of rebirth system
3. Check if rebirth is used in game

**Cleanup Steps (Refactoring):**
1. Create new `RebirthManager.cs` class
2. Move rebirth logic to new class
3. Update references to use new manager
4. Test rebirth functionality

**Estimated Impact:** Medium - Refactoring, not deletion

---

## Deprecated Features

### 11. Animation Pause System

**File:** `Assets/Scripts/Animations/CharacterAnimationEventListener.cs:324`

**Reason:** TODO comment states "Do not pause animation, it creates lots of bugs".

**Code:**
```csharp
// TODO: Do not pause animation, it creates lots of bugs for now
// SetAnimatorSpeed(Random.Range(0.1f, 0.3f));
```

**Investigation Required:**
1. Determine if animation pause was intended feature
2. Check if any systems depend on it
3. Verify if bugs are fixed

**Cleanup Steps:**
1. If feature not needed, remove commented code
2. If feature needed, fix bugs and re-enable
3. Document decision

**Estimated Impact:** Low - Already commented out

---

### 12. UI Focus Frame Delay Hacks

**Files:**
- `Assets/Scripts/Views/View Equipment Menu Components/EquipmentSlots.cs:482`
- `Assets/Scripts/Views/View Equipment Menu Components/ItemList.cs:197`
- `Assets/Scripts/UI/UIDocumentCharacterCustomization.cs:173`

**Reason:** Multiple "hack" comments for frame delay workarounds.

**Code:**
```csharp
// Delay the focus until the next frame, required as a hack for now
Invoke(nameof(GiveFocus), 0f);
```

**Investigation Required:**
1. Identify root cause of focus timing issue
2. Research proper UI Toolkit focus management
3. Test if issue still exists in current Unity version

**Cleanup Steps (If Fixing):**
1. Implement proper focus management
2. Remove Invoke workarounds
3. Test all UI focus scenarios

**Cleanup Steps (If Keeping):**
1. Change comment from "hack" to explanation
2. Document why frame delay is necessary

**Estimated Impact:** Medium - UI functionality

---

### 13. Lock-On Animation Refresh Hack

**File:** `Assets/Scripts/Player/PlayerManager.cs:274`

**Reason:** Comment states "Hack to refresh lock on while switching animations".

**Code:**
```csharp
// Hack to refresh lock on while switching animations
if (lockOnManager.isLockedOn)
{
    lockOnManager.DisableLockOn();
    lockOnManager.EnableLockOn();
}
```

**Investigation Required:**
1. Identify why lock-on breaks during animation switch
2. Check if issue is in lock-on system or animation system
3. Test if issue still occurs

**Cleanup Steps (If Fixing):**
1. Fix root cause in lock-on or animation system
2. Remove disable/enable workaround
3. Test lock-on during all animation transitions

**Cleanup Steps (If Keeping):**
1. Change comment to explain why necessary
2. Document as known limitation

**Estimated Impact:** Medium - Core gameplay feature

---

## Cleanup Checklist

### Phase 1: Safe Deletions (Low Risk)

- [ ] Remove `IKHelper.cs` and references
- [ ] Remove `EV_CompleteObjective.cs`
- [ ] Remove obsolete Changelog fields
- [ ] Remove obsolete EV_Teleport fields
- [ ] Remove `ViewComponent_GameSettings.cs`

**Estimated Time:** 2-4 hours  
**Testing Required:** Light testing of affected systems

---

### Phase 2: Investigated Deletions (Medium Risk)

- [ ] Investigate and remove guard counter system
- [ ] Investigate and remove ambush float parameter
- [ ] Investigate and remove ViewQuests dead code
- [ ] Investigate and remove animation pause code

**Estimated Time:** 4-8 hours  
**Testing Required:** Thorough testing of combat and AI

---

### Phase 3: Refactoring (Medium Risk)

- [ ] Refactor rebirth system to dedicated class
- [ ] Fix or document weapon animation override TODO
- [ ] Fix or document UI focus hacks
- [ ] Fix or document lock-on animation hack

**Estimated Time:** 8-16 hours  
**Testing Required:** Comprehensive testing

---

## Testing Checklist

After each cleanup phase, test the following:

### Combat System
- [ ] Player attacks work correctly
- [ ] Enemy attacks work correctly
- [ ] Blocking and parrying work
- [ ] Dodge mechanics work
- [ ] Backstab mechanics work
- [ ] Damage calculations are correct
- [ ] Status effects apply correctly

### AI System
- [ ] Enemies patrol correctly
- [ ] Enemies chase player
- [ ] Enemies engage in combat
- [ ] Ambush enemies work
- [ ] State transitions are smooth
- [ ] NavMesh pathfinding works

### UI System
- [ ] Equipment menu works
- [ ] Inventory menu works
- [ ] Quest menu works
- [ ] Settings menu works
- [ ] Focus navigation works
- [ ] Gamepad navigation works

### Save System
- [ ] Game saves correctly
- [ ] Game loads correctly
- [ ] All data persists
- [ ] No corruption occurs

### Animation System
- [ ] Player animations play correctly
- [ ] Enemy animations play correctly
- [ ] Animation transitions are smooth
- [ ] Lock-on doesn't break animations
- [ ] Equipment changes update animations

---

## Backup Strategy

Before any deletions:

1. **Create Git Branch**
   ```bash
   git checkout -b cleanup/unused-code
   ```

2. **Commit Current State**
   ```bash
   git add .
   git commit -m "Pre-cleanup snapshot"
   ```

3. **Create Backup**
   - Export Unity package of entire project
   - Store in safe location
   - Label with date and version

4. **Document Changes**
   - Keep log of all deletions
   - Note any issues encountered
   - Record testing results

---

## Rollback Plan

If issues occur after cleanup:

1. **Immediate Rollback**
   ```bash
   git reset --hard HEAD~1
   ```

2. **Selective Rollback**
   ```bash
   git revert <commit-hash>
   ```

3. **File-Level Rollback**
   ```bash
   git checkout HEAD~1 -- path/to/file.cs
   ```

4. **Full Project Restore**
   - Restore from Unity package backup
   - Reimport into clean project

---

## Estimated Cleanup Impact

### Code Reduction
- **Phase 1:** ~500-1000 lines removed
- **Phase 2:** ~1000-2000 lines removed
- **Phase 3:** ~500 lines refactored

**Total:** ~2000-3500 lines cleaned up

### Performance Impact
- Reduced script compilation time
- Fewer components to process
- Cleaner codebase for maintenance

### Maintenance Impact
- Fewer TODOs to track
- Less technical debt
- Clearer code intent
- Easier onboarding for new developers

---

## Post-Cleanup Actions

After completing cleanup:

1. **Update Documentation**
   - Remove references to deleted systems
   - Update architecture diagrams
   - Update feature lists

2. **Update TODO List**
   - Remove completed TODOs
   - Prioritize remaining TODOs
   - Create tickets for future work

3. **Performance Testing**
   - Profile before/after
   - Measure load times
   - Check memory usage
   - Verify frame rates

4. **Code Review**
   - Review all changes
   - Verify no regressions
   - Check code quality
   - Ensure consistency

5. **Merge to Main**
   ```bash
   git checkout main
   git merge cleanup/unused-code
   git push origin main
   ```

---

## Conclusion

This cleanup will remove approximately **2000-3500 lines of unused/obsolete code**, improving:
- Code maintainability
- Project clarity
- Compilation times
- Developer productivity

**Recommended Approach:** Complete Phase 1 first, test thoroughly, then proceed to Phase 2 and 3.

**Estimated Total Time:** 14-28 hours including testing

**Risk Level:** Low to Medium (with proper testing and backups)

---

*Cleanup Guide created: 2026-02-07*  
*Version: 1.0*
