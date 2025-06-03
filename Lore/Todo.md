
1.6.2

DONE:
Intro now delivers the King’s letter immediately. Chicken quest is optional; players can go straight to Cecily Town at the start.

Rethought the stone and Molok story with a stronger story and dialogue. Redesign some aspects of the Main Quest and the Forest Wanderers.

Added poison barrel minigame to Lenny boss fight.

Enhanced enemy sighting and added jumping ability for some enemies.

Revamped following boss fights: Duo orcs, Balbino and Drogo - they have new attacks and increased difficulty.

Bosses now counter backstabs more often—thanks to Peaches for highlighting this exploit.

Made Kayro fight in Elven Village tougher; switched his weapons to shields. Tombstone Shield now drops here, while his previous spell is hidden in a chest.

Increased difficulty of Celes and Molok boss fights; added Vael’Noor statue puzzle to aid Celes fight.

Added two new main story endings and improved the epilogue.

Overhauled Quest Journal with detailed objectives, character bios, and trivia. Multiple quest tracking now available.

Added cutscene with camera pan on first entering Snailcliff.

Fixed floating bees bug.

Added note that a staff must be equipped for spells.

Added collision to castle area to prevent falling off map (inspired by Peaches).

Fixed weapon buffs not giving status effect buildups (fire resins are now causing burnt, frost now causes frostbite, etc)

IMPROVEMENTS
Add Royal Music to Alcino and King discussion

dialogo de Alcino:
"Aldeia élfica de Arun", tambem ha um erro ortografico la com um espaço

Marcel:
Cecily town, na biofragia, corrigir

fogwall on boss fight duo orc not working

fogwall on balbino boss fight not working

BUGS

Eleves vao arrasar na (Dialogo na tavern de Alcino, sobre a vila elfica) Na mesma conversa ,o rei diz Aldeia Elfica

Error Name: ArgumentException: Object of type 'UnityEngine.Object' cannot be converted to type 'AF.Projectile'.
 Stack Trace: System.RuntimeType.CheckValue (System.Object value, System.Reflection.Binder binder, System.Globalization.CultureInfo culture, System.Reflection.BindingFlags invokeAttr) (at <8db6ec373e6d40bd9d38c8037d358c4e>:0)
System.Reflection.RuntimeMethodInfo.ConvertValues (System.Reflection.Binder binder, System.Object[] args, System.Reflection.ParameterInfo[] pinfo, System.Globalization.CultureInfo culture, System.Reflection.BindingFlags invokeAttr) (at <8db6ec373e6d40bd9d38c8037d358c4e>:0)
System.Reflection.RuntimeConstructorInfo.DoInvoke (System.Object obj, System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, System.Object[] parameters, System.Globalization.CultureInfo culture) (at <8db6ec373e6d40bd9d38c8037d358c4e>:0)
System.Reflection.RuntimeConstructorInfo.Invoke (System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, System.Object[] parameters, System.Globalization.CultureInfo culture) (at <8db6ec373e6d40bd9d38c8037d358c4e>:0)
System.Reflection.ConstructorInfo.Invoke (System.Object[] parameters) (at <8db6ec373e6d40bd9d38c8037d358c4e>:0)
UnityEngine.Events.PersistentCall.GetObjectCall (UnityEngine.Object target, System.Reflection.MethodInfo method, UnityEngine.Events.ArgumentCache arguments) (at <89527d299cb64ba88f87240781b415e5>:0)
UnityEngine.Events.PersistentCall.GetRuntimeCall (UnityEngine.Events.UnityEventBase theEvent) (at <89527d299cb64ba88f87240781b415e5>:0)
UnityEngine.Events.PersistentCallGroup.Initialize (UnityEngine.Events.InvokableCallList invokableList, UnityEngine.Events.UnityEventBase unityEventBase) (at <89527d299cb64ba88f87240781b415e5>:0)
UnityEngine.Events.UnityEventBase.RebuildPersistentCallsIfNeeded () (at <89527d299cb64ba88f87240781b415e5>:0)
UnityEngine.Events.UnityEventBase.PrepareInvoke () (at <89527d299cb64ba88f87240781b415e5>:0)
UnityEngine.Events.UnityEvent.Invoke () (at <89527d299cb64ba88f87240781b415e5>:0)
AF.Combat.CharacterCombatController.OnAttackStart () (at Assets/Scripts/Combat/CharacterCombatController.cs:334)
AF.Combat.CharacterCombatController.ExecuteCurrentCombatAction (System.Single crossFade) (at Assets/Scripts/Combat/CharacterCombatController.cs:293)
AF.Combat.CharacterCombatController.UseChaseAction () (at Assets/Scripts/Combat/CharacterCombatController.cs:197)
AF.ChaseState.Tick (AF.StateManager stateManager) (at Assets/Scripts/AI/AI_States/ChaseState.cs:126)
AF.StateManager.FixedUpdate () (at Assets/Scripts/AI/StateMachine.cs:37)

- The stone should not be gone when drogo strikes


- NullReferenceException: Object reference not set to an instance of an object
AF.Shooting.PlayerShooter.HandleShootingArrowSideEffects (AF.PlayerManager playerManager, AF.Arrow arrowThatWasShot) (at Assets/Scripts/Shooting/PlayerShooter.cs:740)
AF.Shooting.PlayerShooter.HandleArrowProjectile (UnityEngine.GameObject projectile) (at Assets/Scripts/Shooting/PlayerShooter.cs:466)
AF.Shooting.PlayerShooter.ShootWithoutClearingProjectilesAndSpells (System.Boolean ignoreSpawnFromCamera) (at Assets/Scripts/Shooting/PlayerShooter.cs:402)
AF.Shooting.PlayerShooter.OnShoot () (at Assets/Scripts/Shooting/PlayerShooter.cs:421)
AF.Animations.PlayerAnimationEventListener.OnFireArrow () (at Assets/Scripts/Animations/PlayerAnimationEventListener.cs:172)

- Add flaming brazier to dragon boss fight


- Cacildes not falling in aruin temple when asking for orange juice

- Can not save game when molok boss fight begins




READY FOR DEV:

- Add opening door sound to arun temple when fenlora is running towards the entrance
- bee boss fight, the true bee doesnt go to the ground
. Max Stamina values and health on level up screen look wrong
- Make hitbox on shields on kayro better
- Stamina and health not levelling well
- Improve music system
- Normalize musics
- Add different music to cecily town
- Add total game progress
- Soldier in Impossible City has guard dialogue
- Add note explaining scroll wheel distance
- Remove walk
- th when unarmed is not adding extra bonus
- improve main story bosses
- Levelling up too fast
- Wooden Staff walk is strange
- Improve Roberto boss fight


BACKLOG:
- Adjust unarmed and weapon staminas
- Review item descriptions
- Allow messages to be continued by pressing mouse click ?
- Setup Analytics for Unity
- Earthstomp should be prize for winning Arena
- Ring that restores health upon critical attacks
- Ring that restores mana upon critical attacks

Bonus:
- Ring that checks if enemy is hit with arrow, there's 50% chances of recovering that arrow upon enemy death
- Version save files
- crafting should be earlier

1.6.2
Simplify Inputs for Sprint and Dodge to share same hotkey
Jump should be on space, enable rebinding
Gold Dooping - Check the gold and die, the pickup its still there. it should not reset during in-game

Shields as weapons
Cloaks as accessories, same as pauldrons
Arrow jump attacks
missao recuperar vinho
