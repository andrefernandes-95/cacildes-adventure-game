EXTERNAL runEvent(eventId)

~ runEvent("King Talks")

King Merlot: Do you have an appointment?
<> Is this about the court jester role? It's taken. Yes.
<> Well, no. We've ousted the court jester. I've been in no mood for laughs and giggles lately.

    * [Show the king's admission letter...]

    ~ runEvent("King Reads Letter")

    King Merlot: Ah yes. You're Gertrude's offspring.
    <> My apologies for that oversight; this orc business has been occupying my thoughts.
    <> Our army needs all the help it can get, friend.

    ~ runEvent("King Hides Letter")

    King Merlot: Tell me… do you know your way around a sword and shield?

        ** [Sword, axes, fists, if it smashes, I excel at it]
        King Merlot: Perfect! That’s exactly the kind of reckless courage we need.
        -> How_Much_You_Know

        ** [I prefer spells and enchantations]
        King Merlot: Ah… a wizard! Clever. Have you met Cael, our resident mage? Or should I say… tower of temperamental brilliance? I digress...
        -> How_Much_You_Know

        ** [I'm a seasoned fighter. Fought many slimes on my way here...]
        King Merlot: Haha. Slimes might bore you, but we’ll be throwing wolves, spiders, and even worse at you soon enough.
        -> How_Much_You_Know


== How_Much_You_Know ==

    ~ runEvent("King Talks")

    King Merlot: How much do you know about my previous chat with General Alcino?

    *** [Nothing, I swear.]
    King Merlot: Ah… polite silence, or poor hearing? Either way, you are forgiven.
    -> Ending

    *** [Something about a stone and orcs near our doorstep.]
    King Merlot: Then you have listened well.
    -> Ending

== Ending ==

King Merlot: The orcs of Thorum, led by that zealot Drogo, have made camp near the West Bridge—just outside Cecily Town.

~ runEvent("King Talks")

King Merlot: They seek a precious stone—the Cold Fire Stone—said to be guarded by our elven allies in Arun Village. Drogo believes it real enough to march an army toward it.

King Merlot: That should bring you up to speed. I've learned through long years of rule that wisdom lies in measured revelations.

~ runEvent("King Talks")

King Merlot: Anyway... you should meet my stubborn general, Alcino. He went to his shack near the church, down in the Lower District.

King Merlot: Let him size you up and hand you your first task. Meanwhile, I’ll disappear to my chambers and dig through some tomes for inspiration.

~ runEvent("King Talks")

King Merlot: And by all means, explore the castle. Open chests, poke around, see what secrets you can uncover. You have my permission… just don’t burn anything down.

-> END
