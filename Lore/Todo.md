# Unholy Sword

BAckstab skeleton makes it spin
Pick bow and flambe not showing the GUI of received item

# Set time in midnight omen boss not on night

# Captain ahrold stop fight midst fight

# samurai dodge into water

If player has alcino after molok fight, alcino appears twice in cutscene with king and drogo

- Whispering vale, check the pit illusionary wall, not triggering with celes sword
  Something buggy with celes sword, cant hit the wood tree pzuzle on whipsering vale

# Analytics

- track how long a player wanders a map
- track if player recruits companions
- track if player dismisses companions
- track what items a player buys
- track what items a player sells
- track what kills a player
- track if player kills while two handing

- if we drink 1 orange juice, then pick the other, the counter doesnt uipdate

# IN PROGRESS

BUG GRAVE:

- Slow Down player doesnt have animation speed restored after it wears off
  (Cant reproduce it all the time)

needs fixing
alcool atrasa regeneracao vida deveriamos remover loading

Ideas:

- Add a resistance stat which increases 2 points per level on every status effect
- Make categories for each location to make naviation easier

Possibly fixed:
? Hugo dialogue when asking him to join the party might be fixed; used fade to clear dialogue window.
? We must test orc, alcino quest again to make sure we didnt break anything
? Check if dragon is fixed: Received blizzard at the start
? Can not save at this time after drogocutscene
? Test the epilogue again with the king to test in english if rise is working.

? Chest is activating the pcikup id for the trio boss fight - check if we can fight with them again
? Check Quests on new game plus
? Chest is disappearing in veilglade for impossible city

Cacildes Home:

- should have autosave feature after game starts
- check camera issue when starting game

Mountainpass

- midnight omen hitbox instant start on one of the attacks

- Waterborne ring looks weird in its delay rate

- Check Achievements
- Adjust spell mana costs

ACHIEVEMENTS
X maerimond achievement was not set
X thief who tried to steal you

Achievement Ideas

- Rest at bonfire
- Mimic chest defeat (mimic-chest.png)
- Gargoyle Boss (gargoyle.png)

- Shield that acts a mirror, deflecting damage

To Do:

- Companions should mirror player stats
- ashes should give error notification
- prison key used danuris, looks like its not being lost
- Mimic chest pickup active
- Bella doesnt appear kneeled when vampire is killed
- Alcino not fighting Notify Companions, check why
- Fix scroll on blacksmith, it works fine on shop
- fix scroll on item list, it works fine on shop
- moving very fast while aiming bow
- Add more broken arrows to the world
- Add Gorth Greataxe since its used in promotion material
- Add oakshield back as ability
- Status Effect - cant cast spells or abilities
- Rename every chest in the game so we know where each item is.
- Remove Elven Ear from Arun Village
- During Molok fight he stopped taking damage
- Add mana to spells, check intelligence requirements
- Use Weapon Type to determine the bonus of poise damage for colossal weapons (Colossal could ignoire poise all together)
- Heavy Attack DAMAGE Bonus in effect?
- We should reset ai humanoid cuting distance to target on state exits on attack animatins (If we parry an enemy, he may continue to use cutting distance logic while stunned, happened with bird but I fixed it for generic creatures)
- Ring that makes orange juice drink faster
- Add category to received items popup
- BUG: Accessories can be equipped on other slots
- expliccar q aguenar abilidades da mais dano
- Add chest that uses another script for adding items, to avoid bugs with armors
- Weapon should have abilities for right trigger and right bumper and left trigger (this way, the amount of combos is dependant on the weapon itself)
- Add total game progress
- Backpack accessory - allow more items in th eback
- Healing Orb should cure dead companions
- Add focus to callback on scroll view exit buttons, which I forgot to do
- Thief should steal player weapon - ability
- Spells should spend mana
- Enemy Posture should increase every time its broke
- I think save game is resetting favorite managers
- UNtrack quest button not working
- Arena should give smithing shards as a reward

Low:

- Made wood sword upgradeable
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
- Elixir that reveals illusionary walls
- Confusion Status Effect
- If I kill Roberto with the wrong weapon and run 24 hours in the map, it triggers his dialogue again

BACKLOG:

- Setup Analytics for Unity
- Add camera damping options

DONE:

- Inventory weight, if too heavy, do heavy roll ( do not hinder jump )
- Blocking with weapon we are running on the speed
- Rework lady that gives attributes in West Bridge, we need to use functions in playerLevel Up

X Arun Village passage is buggy and I cant enter it
X Save Disk icon on game saved notification
X Maerimond Quest not being saved
X Tree Shooters need less chase state max distance
X Blacksmith Screen is showing Physical Attack labels (Solution: change blacksmith screen on generic trigger, it was using the legacy one)

Mountainpass:
X resting at bonfire makes wolves stuck in other state. check if its sleep or something else.
X Luzern has alcino cloak and shoulder pads
X mountainpass bandits also have alcino cloak
X Fix grindwheel
X Add fixed patrols to cows
X Soldado na taberna tem TALK como accçao

Maerimond Cavern:
X Maerimond Uses a very dangeoerus left attack, we should not have it (dont know if its due to the staff) - it was actually ising Right Attack C, which is the jump attack
X Maerimond not giving maerimond token as loot
X Light Trap should have different icon, ansd cast stronger light

Cecily Town (Lower):
X Brumilda dialogue on rumors missing character name
X Add Roberto to Skeleton character so we cna easily identify him in the map
X Remove General Sangria from alcino house
X GENERAL alcino shouldnt be in cecily town house during celebration i ntavern quest after orcs
X Add more speed to cmpanions (Alcino is too slow)

Cecily Town (Upper)
X Hide Levitation - changed Bubbles location to appear there
X Increase Collider for Faulty Knight
X Avelin sells twice resin, give her more ingredients
X Alcino talks twice when talking to player for the first time at his house, i think we should remove illusionary
walls dialogue

orc Cavern
X Remove steel key chest
X Remove steel key door
X Destroyable box with regeneration potion
X Flamberge lacking sound

drogo hideway
X cant travel in drogos hidewaybonfire
X Add more health to orc duo boss fight
X add more health to orcs

Sewers:
X Soldier shield no icon
X Hidden wall in sewrs with refined iron shard
X Box has stamina potion near thief trap
X Add Grischa quest in king castle
X Girshca has blood effect on her
X Return to pedro quest objective should have different sprite

West Bridge:
X Slime near magic shard not moving

King Castle:
X Remove genral sangria on 2nd floor
X Fix ladders outside, they are not properly working
X No reward given to player after completing sewers quest
X We should finish grischa quest after dialogue with pedro

Done:
X Wait 1 Hour is causing the clock to speed up even when changing scenes
X Check each quest location
X Check all bonfires to see if they are correctly setup and have locations assigned
X Two Handing does not produce bonus damage to elemental damage
X Max Health is different in level up screen and player hud
X level up max mana health and stamina are wrong.
X Fix overflow of items description in alchemy table
X reset event system in tutorial
X fix hugo combat actions
X if i have bow equipped and two hand, i cant block (equip bow and have iron sword, swithc to th)
X Physical Attack label is wrong on equipment view, it should read Phuysical Defense
X Molok Blade is repeated in dorrim shop, should be Molok Staff
X Flambe has 1287 burnt status ifnlicted. investigate
X Skills are not being saved in their position
X Accessories are not being saved in their position (at least pauldrons and cloaks)
X Consumables also dont look saved
X i cant travel from slepbone abandoned wharf bonfire
X Magic, Darkness and Water show 100 > 100 on view menu
X After levelling up, my iron sword +3 receives damage from 96 to 416. check what happens. i need to reload for this to be fixed.

cacildes home
X fix monster tally in cacildes home
X hide disciplines of ministeria
X hide note on table in living room
X the brawler - fix the input E
X After returning to mom after bear quest, add item is causing error.check

cecily fields-
X draw water from hell, fix generic trigger
X "push" when picking bow, its wrong label for generic trigger

mountainpass
X footstep receiver trigger bug when travelling from cecily fields to mountainpass
X I cant travel from bonfire

slepbone pathway
X lara is stuck

bear cavern
X shark is stuck
X petra is stuck when picking rosemary because of a tree i added
X bug with petra, she gives infinite health potions after accepting thelma quest
X Remove wooden bow chest near arena entrance
X tree arena variant has ambushed version below him
X chicken are labelled snakes
X poison ring on top of tower. is it configured correctly?
X coin of duplication, is it working?

thief cavern
X stamina potion still present in one of the thieves near the water and the alchemy table of thieves
X porta de madeira ao pe da fogueira de thieves cavern esta meio bloqueada na colisao, regenera
