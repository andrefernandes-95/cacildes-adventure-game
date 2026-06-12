# Migration Roadmap — Cacildes Adventure 2 → Unholy Sword

**Source repo:** `C:\Users\andre\Desktop\Cacildes Adventure 2 - 080620261804`  
**Target repo:** this project (last shared commit `13d5166`, 2025-12-13)  
**Strategy:** incremental waves — bugfixes first, then camera, then features. One wave per PR/session.

---

## Pre-flight (before Wave 1A)

- [ ] Commit or stash all uncommitted Unholy Sword WIP on a branch (e.g. `feature/unholy-sword-wip`)
- [ ] Add source as a read-only remote for cherry-picks:
  ```bash
  git remote add ca2 "C:/Users/andre/Desktop/Cacildes Adventure 2 - 080620261804"
  git fetch ca2
  ```
- [ ] After each wave: open Unity, fix compile errors, smoke-test Cacildes + Unholy Sword title flow
- [ ] **Never** cherry-pick Dungeon/Roguelike commits (out of scope)

### High-conflict files (expect manual merge every wave)

| File | Why |
|------|-----|
| `Assets/Prefabs/System/-- Core --.prefab` | Touched by almost every wave |
| `Assets/Scripts/System/GameSettings.cs` | Unholy Sword multi-game routing |
| `Assets/Scripts/UI/UIDocumentTitleScreen.cs` | Unholy Sword title screen |
| `Assets/Graphics/Animator/Player.controller` | Combat + gesture changes |
| `Assets/Prefabs/Characters/Core/[PREFAB] Character - Core.prefab` | Ability/component wiring |

---

## Phase 1 — Bugfixes

Goal: stabilize gameplay without new features. Complete waves in order.

---

### Wave 1A — Isolated script fixes (start here)

**Risk:** Low · **Est.:** 1–2 hours · **Conflicts:** minimal

| Commit | Date | Summary | Key files |
|--------|------|---------|-----------|
| `7468030b` | 2026-06-05 | Day/night manager fix | `DayNightManager.cs`, `SceneLocation.cs` |
| `663407eb` | 2026-06-04 | Two-hand reference fix | `CharacterTwoHandRef.cs`, `Bastard Sword.asset` |

**Port notes**
- For `663407eb`, take the **script change only** unless you are actively building Thief Cavern (`Thief Cavern.unity` is large and unrelated to Unholy Sword).
- Verify: day/night transitions in an outdoor scene; two-hand toggle on a sword without animation glitches.

**Cherry-pick**
```bash
git cherry-pick 7468030b
git cherry-pick 663407eb   # resolve map conflict by keeping ours if needed
```

---

### Wave 1B — January targeted fixes

**Risk:** Medium · **Est.:** half day · **Conflicts:** Player controller, localization, inventory

| Commit | Date | Summary | Key C# files |
|--------|------|---------|--------------|
| `dbc1b0c7` | 2026-01-12 | Arrow bugs | `PlayerShooter.cs`, `EmptyArrowsDependant.cs`, `PlayerInventory.cs`, `InventoryDatabase.cs`, `EV_AddWeapon.cs` |
| `821f9d72` | 2026-01-15 | Backstab + bonfire travel menu | `PlayerBackstabController.cs`, `UIDocumentBonfireTravel.cs`, `CharacterBackstabController.cs` |

**Port notes**
- `dbc1b0c7` also deletes many legacy animator controllers — **review deletions** before accepting; skip animator cleanup if those enemies still exist in your build.
- `821f9d72` adds `Unholy Sword` localization tables — you likely already have these untracked; **merge tables, don't overwrite** your WIP localization.
- Verify: equip bow → shoot → empty quiver message; backstab from behind; bonfire fast-travel list populates correctly.

```bash
git cherry-pick dbc1b0c7
git cherry-pick 821f9d72
```

---

### Wave 1C — v2.4 baseline (selective, not blind cherry-pick)

**Risk:** High · **Est.:** 1–2 days · **Conflicts:** project-wide

| Commit | Date | Summary |
|--------|------|---------|
| `789cd2d6` | 2026-01-08 | Code update for v2.4 (thousands of files) |

**Do not** cherry-pick this commit wholesale. Instead, port **by folder** only where Wave 1A/1B exposed gaps:

| Port | Skip |
|------|------|
| `Assets/Scripts/**` changes that fix compile/runtime issues found in 1B | `.plastic/*`, `UserSettings/*`, `Builds/*` |
| `Assets/Resources/Changelogs/2.4.*` if versioning matters | Deleted audio `.meta` churn |
| `ProjectSettings/*` only if required for Unity version parity | Entire map overwrites |
| Unholy Sword audio/localization if missing in target | Legacy enemy animator deletions |

**Suggested workflow**
```bash
git diff 13d5166..789cd2d6 -- Assets/Scripts > /tmp/v24-scripts.patch
# Review patch file-by-file; apply only hunks you need
```

Verify: project compiles; save/load; main quest bonfire still works.

---

### Wave 1D — April bug passes

**Risk:** Medium–High · **Est.:** 1 day · **Conflicts:** Core prefab, character prefabs

| Commit | Date | Summary |
|--------|------|---------|
| `6e8bb197` | 2026-04-27 | Agent/AI bugs, prefab cleanup |
| `67847672` | 2026-04-30 | Bulk bug fix pass (UI, items, prefabs) |

**Port notes**
- Both touch `[PREFAB] Character - Core.prefab` and `-- Core --.prefab` — merge carefully with Unholy Sword core prefab split.
- `67847672` adds Spanish UI localization (`UIDocuments_es`) — port if wanted, skip otherwise.
- Verify: humanoid NPC patrol + combat; shop UI; item tooltips; no missing references on Core prefab.

```bash
git cherry-pick 6e8bb197
git cherry-pick 67847672
```

---

### Wave 1E — May targeted bug fixes

**Risk:** Medium · **Est.:** 1 day · **Conflicts:** Core prefab, HUD, maps

| Commit | Date | Summary | Key files |
|--------|------|---------|-----------|
| `b0b3d8d8` | 2026-05-22 | Music stops when hour changes | `DayNightManager` wiring in `-- Core --.prefab`, weapon asset cleanup |
| `72db2143` | 2026-05-22 | 1H weapon bug + HUD tidy | `HUDCombatStanceController.cs`, `-- Core --.prefab` |
| `b078dc65` | 2026-05-23 | Consumables not updating inventory UI | Inventory UI listeners, Cacildes Home dialogue (skip map/dialogue if unrelated) |

**Port notes**
- `b078dc65` bundles mom-quest dialogue — take **inventory UI fix only** unless you want that quest content.
- `72db2143` adds `HUDCombatStanceController.cs` — ensure `UIDocumentPlayerHUDV2` exists or port HUD changes together.
- Verify: wait for hour change → BGM continues; one-hand iron sword attacks; pick up consumable → HUD slot updates immediately.

```bash
git cherry-pick b0b3d8d8
git cherry-pick 72db2143
git cherry-pick b078dc65
```

---

### Wave 1F — May/June bulk + arena

**Risk:** High · **Est.:** 1–2 days · **Conflicts:** maps, combatants, abilities

| Commit | Date | Summary |
|--------|------|---------|
| `4c70242a` | 2026-05-24 | Bulk bug fix (Cecily Fields, slime, items) |
| `dc0a2524` | 2026-06-02 | Arena bugs (weapon abilities, charged attacks) |

**Port notes**
- `4c70242a` massively changes `Cecily Fields.unity` — **skip the map** unless you need it; port scripts/combatants (`Slime.asset`, ring rename).
- `dc0a2524` touches `CharacterAbilityManager.cs` and `UseWeaponAttack.cs` — test arena + basic attack chain after merge.
- Verify: arena tree challenge; slime enemy sounds/stats; Bear Cavern arena prefab if ported.

```bash
git cherry-pick 4c70242a   # resolve: keep our Cecily Fields map
git cherry-pick dc0a2524
```

---

### Phase 1 completion checklist

- [ ] Wave 1A — day/night + two-hand ref
- [ ] Wave 1B — arrows + backstab/bonfire
- [ ] Wave 1C — v2.4 selective baseline
- [ ] Wave 1D — April agent + bulk
- [ ] Wave 1E — music, 1H, consumables UI
- [ ] Wave 1F — May bulk + arena
- [ ] Full regression: combat, inventory, save/load, bonfire travel, both game modes on title screen

---

## Phase 2 — Camera / lock-on

**Prerequisite:** Phase 1 complete (especially `6e8bb197` agent fixes).

| Commit | Date | Summary |
|--------|------|---------|
| `07c84ecd` | 2026-04-29 | Camera collision when locked on; target manager contracts |

**New files to add**
- `Assets/Scripts/Combat/ITargetDetection.cs`
- `Assets/Scripts/Combat/ITargetSetHandler.cs`
- `Assets/Scripts/Combat/TargetDetection_EnemyFaction.cs`
- `Assets/Scripts/Combat/TargetDetection_PlayerOnly.cs`
- `Assets/Scripts/Combat/TargetSetHandler_ChaseTarget.cs`
- `Assets/Scripts/Combat/TargetSetHandler_ExitAmbush.cs`

**Modified systems**
- `LockOnManager.cs`, `PlayerCamera.cs`, `LockOnCameraCollision.cs`, `TargetManager.cs`
- Many AI states (chase, ambush, idle) — interface swap to `ITargetDetection`
- Skip `Prototype - V3.unity` map changes unless you use that scene

**Risk:** Medium–High · **Est.:** 1 day

```bash
git cherry-pick 07c84ecd
```

Verify: lock on → camera doesn't clip through walls; cycle targets; ambush enemies still acquire player.

---

## Phase 3 — Features

Port in this order (each builds on the last).

---

### Wave 3A — Armor slot expansion

| Commit | Date | Summary |
|--------|------|---------|
| `c3dba432` | 2026-05-25 | Shoulder pads + cloak as separate armor slots |

**Touches:** `EquipmentDatabase`, `CharacterBaseEquipment`, `PlayerEquipment`, defense managers, item localization, equipment UI.

**Risk:** Medium · **Est.:** 1 day  
**Note:** May conflict with uncommitted item/localization work — merge `Items_*.asset` tables carefully.

Verify: equip cloak + shoulder; armor set bonuses; companion equipment still works.

---

### Wave 3B — Stamina wheel

| Commit | Date | Summary |
|--------|------|---------|
| `c369f971` | 2026-05-25 | Add Stamina Wheel (initial) |
| `a86447c2` | 2026-05-25 | Stamina Wheel refinements |

**New files:** `StaminaWheelController.cs`, `StaminaWheelVisualElement.cs`  
**Wiring:** `-- Core --.prefab`, `PlayerManager.cs`, `UIDocumentPlayerHUDV2.cs`

**Risk:** Low–Medium · **Est.:** half day

```bash
git cherry-pick c369f971
git cherry-pick a86447c2
```

Verify: stamina ring visible; drains on sprint/dodge; hides when full if configured.

---

### Wave 3C — Companion menu

| Commit | Date | Summary |
|--------|------|---------|
| `fb5bd4a9` | 2026-05-23 | Companion menu UI |

**New files:** `ViewCompanionsMenu.cs`, `ViewMenu_Companions.uxml`, `CompanionListButton.uxml`  
**Wiring:** `CompanionsDatabase.cs`, `-- Core --.prefab`, combatant assets, localization

**Risk:** Medium · **Est.:** 1 day  
**Note:** Best done after Wave 3A if armor slots affect companion equipment display.

Verify: open companions menu; list party members; dismiss/summon if supported.

---

### Wave 3D — Light + heavy attack unification

| Commit | Date | Summary |
|--------|------|---------|
| `e51041e8` | 2026-05-23 | **Prerequisite:** `IntEnum` foundation |
| `bf1d18a9` | 2026-05-23 | Single input; charged heavy attack |

**Prerequisite warning:** `bf1d18a9` depends on `e51041e8` (`Assets/Scripts/Data/IntEnum.cs`) and touches `CharacterAbilityManager`, `Player.controller`, weapon charged-attack assets.

**Risk:** High · **Est.:** 1–2 days  
**Note:** This is the most invasive feature. If cherry-pick conflicts heavily, consider manual port of:
1. `IntEnum.cs` + `Damage.cs` enum usage
2. Input binding change (heavy attack on hold)
3. `Straight Sword Charged Attack 01` ability asset
4. Animator transitions in `Player.controller`

```bash
git cherry-pick e51041e8
git cherry-pick bf1d18a9
```

Verify: tap = light attack; hold = charged heavy; stamina/mana costs correct; AI humanoid still attacks.

---

## Phase summary

| Phase | Waves | Focus | Cumulative est. |
|-------|-------|-------|-----------------|
| **1** | 1A → 1F | Bugfixes only | ~5–8 days |
| **2** | — | Camera / lock-on | +1 day |
| **3** | 3A → 3D | Features (armor → stamina → companions → attacks) | +3–5 days |

**Total:** ~2–3 weeks at one wave per session with testing.

---

## Wave workflow template

Use this checklist every session:

1. `git checkout -b migrate/wave-1A` (or current wave)
2. Cherry-pick or manual-port from `ca2/master`
3. Resolve conflicts — **prefer keeping Unholy Sword assets** (`-- Core -- Unholy Sword.prefab`, `Resources/Games/`, DLC dialogues)
4. Unity compile → fix errors
5. Playmode smoke test (see wave verify list)
6. Commit: `migrate: wave 1A day/night and two-hand ref from ca2`
7. Merge to main integration branch

---

## Excluded from this roadmap

- Dungeon Generator / Roguelike (Jun 2026 commits) — not relevant
- Stats/attributes major refactor (Jun 3) — only port if a feature wave blocks without it
- Third Person Controller / swimming / strafing — not requested

---

## Quick reference — all commits in scope

| ID | Wave | Description |
|----|------|-------------|
| `7468030b` | 1A | Day/night manager fix |
| `663407eb` | 1A | Two-hand ref fix |
| `dbc1b0c7` | 1B | Arrow bugs |
| `821f9d72` | 1B | Backstab + bonfire menu |
| `789cd2d6` | 1C | v2.4 baseline (selective) |
| `6e8bb197` | 1D | Agent/AI bugs |
| `67847672` | 1D | April bulk fixes |
| `b0b3d8d8` | 1E | Music hour-change bug |
| `72db2143` | 1E | 1H weapon bug |
| `b078dc65` | 1E | Consumables UI update |
| `4c70242a` | 1F | May bulk fixes |
| `dc0a2524` | 1F | Arena bugs |
| `07c84ecd` | 2 | Camera / lock-on |
| `c3dba432` | 3A | Armor slots (cloak/shoulders) |
| `c369f971` | 3B | Stamina wheel (add) |
| `a86447c2` | 3B | Stamina wheel (refine) |
| `fb5bd4a9` | 3C | Companion menu |
| `e51041e8` | 3D | IntEnum (prerequisite) |
| `bf1d18a9` | 3D | Light + heavy unification |
