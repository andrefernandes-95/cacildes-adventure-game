EXTERNAL runEvent(eventId)
EXTERNAL getReputation()
EXTERNAL hasBegunRobertoQuest()
EXTERNAL hasKilledRobertoAndIsWaitingForAReward()
EXTERNAL hasCompletedRobertoQuest()

~ temp hasBegunRoberto = hasBegunRobertoQuest()
~ temp killedRobertoAndIsWaitingForAReward = hasKilledRobertoAndIsWaitingForAReward()
~ temp robertoQuestComplete = hasCompletedRobertoQuest()
~ temp reputation = getReputation()

- Marcel: Peace be with you, traveler.
<> {reputation < 0: Though I sense shadows trailing your steps.}
<> {reputation > 0: Your deeds precede you—a welcome light in these troubled times.}

<> What brings you to our humble sanctuary?

* {!hasBegunRoberto} [What are those watering cans for?]

Marcel: Ah… yes. These are no ordinary watering cans. I’ve been sprinkling holy water across the graveyard, trying to persuade poor Roberto to… stay dead.
<> Every night he claws his way out to harass the townsfolk. Nothing I’ve tried so far keeps him down. Quite the pickle, truly.

    ** [How did he die?]

    Marcel: A tragic accident, really. Drowned in the river just outside the church. He argued with himself, drunk as a barrel, swinging his beloved morning star…
    <> Its weight betrayed him, and he tumbled into the river. Such devotion to a mace can be fatal, it seems.

    Cacildes: His morning star… was it ever recovered?

    Marcel: No. But… perhaps if someone were to reunite him with it and… persuade him with a little firmness… it might finally lay him to rest.

    Cacildes: I’ll take my chances. Sounds worth a try.

    ~ runEvent("Begin Roberto Quest")

* {killedRobertoAndIsWaitingForAReward} [Roberto won't be a problem anymore.]
     Marcel: You speak the truth? By the gods...  
    <> No longer will his tortured cries pierce the midnight air. No longer will children wake screaming from their dreams.  
    <> Most sacred of all—Roberto’s soul at last walks free, bound for its destined rebirth.

    Marcel: Words feel too small, yet know this: the town owes you a great debt.  
    <> Accept this—coin blessed upon the altar, touched by sacred flame.  

    ~ runEvent("Reward player with church gold")
    
    Marcel: My healing, my blessed water, my counsel—all yours, for no more than the cost of materials.  
    <> And should you ever need sanctuary, these doors will open to you without question.  
    
    Marcel: Roberto’s name will be spoken in our evening prayers—not in fear, but in gratitude,  
    <> for the peace he has finally found. Your deed echoes in the divine realm, friend.  

    ~ runEvent("Complete Roberto Quest")

* {robertoQuestComplete} [Buy items]
    ~ runEvent("Buy church items")

* [You’re the town priest?]

Marcel: I tend this flock as my father tended his vineyards, and his father before him.
<> Though I came to faith through loss—my mother died bringing me into this world.
<> Some nights, I wonder if serving the divine might allow me to meet her beyond the veil.

** [Do you truly believe that’s possible?]

Marcel: Faith is believing in things unseen, in possibilities without proof.
<> The gods speak through wind, flame, root, and tide.
<> If they can manifest in nature, why not in reunion beyond death?

** [Your father was a vintner?]

Marcel: The finest in Slepbone. His wines could soothe heartbreak… or so he claimed.
<> I inherited his patience, though not his palate.
<> Some gifts skip generations; others… transform entirely.

* [Who are the gods?]

Marcel: Before mortal lords ruled, the Four Primarchs shaped the realm.
<> Vael’Noor hammered the sun from cosmic embers and sculpted mountains.
<> Mithriel breathed life into soil—every sprouting seed carries her blessing.

Marcel: Korvak commands restless waters, from tranquil streams to raging storms.
<> Anathar’s icy breath tests all things, tempering the weak and strengthening the strong.

Marcel: Then came the lords. Lord Celes rules the frozen Anathar territories,
<> while Lord Arun governs our fertile valleys here in Slepbone.
<> They rise and fall like seasons, yet the Primarchs endure, eternal.

Marcel: Our church celebrates not rituals, but service. We tend to our citizens, heal the sick. Patronage is voluntary. Creed is personal, for the gods are felt in nature itself.

** [How do you worship them?]

Marcel: Through action, not ceremony.
<> Every sunrise, every harvest, every healed wound is a prayer.
<> We serve the people—that is worship enough.

** [What of hell?]

Marcel: You know General Alcino? He once ruled there.
<> Hell exists for mischievous souls who denigrate nature and defy the gods—but it’s not eternal.
<> There, they reflect, repent, and return cleansed, ready for the next cycle of life. Think of it as… divine time-out.

* [Rumours]

{shuffle:
    - Marcel: Bandits have recently purchased holy water before raids. Foolish, thinking it erases sin. Divine forgiveness demands true repentance. I sell it anyway—perhaps one day they’ll learn.

    - Marcel: Pirates have set their eyes on our town. Defenses are… optimistic. Thankfully, a certain demon prince resides here. Even evil fears greater evil.

    - Marcel: The fisherwoman sometimes sells salmon past its prime. I’ve learned to sniff twice before buying. Fish shouldn’t smell like fish if it’s fresh.

    - Marcel: A strange mist haunts the abandoned road through the Druid Forest past West Bridge. Some things are better left undisturbed, buried with the ages.
}

* [Goodbye]
