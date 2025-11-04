EXTERNAL runEvent(eventId)
EXTERNAL hasPreservedStone()
EXTERNAL getPlayerName()

~ temp isStonePreserved = hasPreservedStone()
~ temp PLAYER_NAME = getPlayerName()

@wait_1

~ runEvent("Show Celebration Camera")
~ runEvent("Fenlora Talks")

{ isStonePreserved:
    Fenlora: ...{PLAYER_NAME}, you look rested.
    <> The trees have graced your dreams and restored your strength — as they always will for us elves, until the end of time.
- else:
    Fenlora: ...You look well, {PLAYER_NAME}.
    <> I wish I could say the same for my people. The stone... it’s gone. Shattered beyond repair.
}

{ isStonePreserved:
    Fenlora: Your grace in protecting the stone shall live forever in the hearts of the people of Arun. You have my deepest gratitude.
- else:
    Fenlora: Do not look sad. This was not your doing. We've faced hardships before.
    <> We will repair the damage done within our forest, and we will learn to adapt to our new fate.
}

~ runEvent("King Merlot Talks")

King Merlot: Truth be spoken. No one dared to go as far as you did, {PLAYER_NAME}. I have the right. No, the duty... to award you
<> this... will you please kneel?

* [Kneel to the king]

~ runEvent("Hide Celebration Camera")
~ runEvent("Kneel Towards King")

@wait_1

King Merlot: You bear the scars of battle and the heart of the brave. Rise, {PLAYER_NAME}.
<> Rise not merely as a soldier of the king... but as a Knight of Cecily Town.

** [Rise]

@wait_1

~ runEvent("Stand Up")
~ runEvent("Show Celebration Camera")

General Alcino: Well then, feel any different now that you're a knight?

Cacildes: Still the same, I guess.

King Merlot: Good. There's much work to be done, and we need your spirits high, {PLAYER_NAME}. Rest now, my friend. In time,
<> your adventure shall continue.

Fenlora: Rest well, Cacildes. You'll always have a place among the elves of Arun.

General Alcino: Can we grab a slice of meatroll at last? I'm starving...

~ runEvent("Play Laughter")

~ runEvent("Hide Celebration Camera")

@wait_1
