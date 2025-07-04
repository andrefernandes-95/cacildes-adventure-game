
In Dev:

- Attack Stat Manager to store current damage for right and left weapon, we should also store bonus damage from str, dex and int into separate variables for consultation (for right and left hand)
- Create specific tooltip scripts for equipments
- BUG: TH when unarmed is not adding extra bonus


Next:
- Inventory weight, if too heavy, do heavy roll ( do not hinder jump )
- Rework stat scale to work with module operators for every 2 or 3 levels (25 bonus to health every level)
- BUG: Accessories can be equipped on other slots

[CRAFTING]
- Crafting upgrades to +10
- Create Iron Shards up to 10
- We should be able to upgrade spells and armors

[UI]
- Add category to received items popup
- Add Damage Type Label to weapons (Slash, Blunt, etc)

[CORE_MECHANICS]
- Fix gold dupping by not reloading save data, we need to check boss story events to not use monobehaviour ids, and rely on quest progress instead

To Do:

- Add chest that uses another script for adding items, to avoid bugs with armors
- Weapon should have abilities for right trigger and right bumper and left trigger (this way, the amount of combos is dependant on the weapon itself)
- Unify Player and Enemy Actions (Block, Parry, Shooting Bow, Throwing Firebomb, Consuming Orange Juice)
- Music HUD showing which music is playing
- Moments as jsons
- Remove staffs
- Make item popups pop up animation
- Add note to quest for Maerimond which is : Rewards - Unlock Boss Weapons
- Add note to boss tokens which is: Talk to Altaire in Snailcliff to unlock boss weapons
- Add total game progress
- Buff weapons made easier
- PS4 Gamepad not showing correct icns 
- Review enemies

Low:

- Ability should consume stamina and mana
- Requirements for spells can still exist, but damage will be lessen
- Poise: Simplify Logic of isbroken, too many side effects
- Proper Arena System with weapons awarded at the end
- If jump attack, enable both weapons
- Normalize audio
- Elven bow should be offered by Fenlora in the epilogue
- Fix R key on gamepad
- When swimming, do not take damage, instead build up drowning
- Add pillar sound to the last one
- Fix pillar sounds in Arun garden
- Add opening door sound to arun temple when fenlora is running towards the entrance
- Bee boss fight, the true bee doesnt go to the ground
- Max Stamina values and health on level up screen look wrong
- Make hitbox on shields on kayro better
- Add different music to cecily town
- Soldier in Impossible City has guard dialogue
- Add note explaining scroll wheel distance
- Remove walk
- improve main story bosses
- Improve Roberto boss fight
- Captain drinking bottles always appear outside main quest near the orc cavern
- Stamina Potion should increase max stamina for 60 seconds
- Powerstance ring
- Earthstomp should be prize for winning Arena
- Ring that restores health upon critical attacks
- Ring that restores mana upon critical attacks
- Ring that enhances shield attacks
- Ring that enhances weapon toss attacks
- Ring that enhances power stance attacks
- Weapon crafting should be earlier
- Ring that checks if enemy is hit with arrow, there's 50% chances of recovering that arrow upon enemy death
- Version save files
- Ring that enhances defense when all armor pieces are equipped
- Ring that enhances charged spells damages
- Ring that allows any weapon to use powerstance regardless of differnet categories


BACKLOG:

- Setup Analytics for Unity


Done:

- Add damage popup when healing too so we know the values
- Replace cards with skill system (this is done, but we need to still convert some abilities, and also check the chests)
- Refactor Player Customization
- Create AI Damage Receiver class
- Add armors to characters, show graphic if humanoid and using synty character model
- Cloaks as accessories, same as pauldrons
- Add Stats to characters, and refactor stats bonus controller
- Add database for items with json descriptions
