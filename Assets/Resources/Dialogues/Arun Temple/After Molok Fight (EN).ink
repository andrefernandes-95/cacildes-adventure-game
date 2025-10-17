EXTERNAL runEvent(eventId)
EXTERNAL hasPreservedStone()
EXTERNAL getPlayerName()

~ temp isStonePreserved = hasPreservedStone()
~ temp PLAYER_NAME = getPlayerName()

@wait_1

~ runEvent("Show Drogo Cage Camera")

Drogo: We're back at the temple? Did you...

~ runEvent("Hide Drogo Cage Camera")

~ runEvent("Show Stone Camera")

{ isStonePreserved:
    Drogo: No! The stone... remains still... Molok... my visions have failed me... my people are doomed...
- else:
    Drogo: Yes... the Cold Fire Stone has been destroyed... Molok is free, at last... my people will not forget your deed... {PLAYER_NAME} - thank you.
}

~ runEvent("Hide Stone Camera")

~ runEvent("Show King Camera")

@wait_1

King Merlot: It is finished, Drogo.  
<> By my decree you shall be conveyed to Ministeria’s highest keep, there to atone for the ruin you have wrought.  

{ isStonePreserved:
    King Merlot: Thanks to our dear {PLAYER_NAME}, this stone has been preserved, and the elves shall live on.  
    <> Let it stand as a reminder that order endures when humans and elves are joined together for a greater cause.
- else:
    King Merlot: We have failed to protect the elves. I must live with this burden... I can only hope their suffering  
    <> is eased by the knowledge that justice will be served.
}

@wait_1

~ runEvent("Hide King Camera")

~ runEvent("Show Cacildes Camera")

~ runEvent("Player And King Face Each Other")

@wait_.5

{ isStonePreserved:
    King Merlot: You have done us a great service, my friend. Words cannot begin to express how much we owe you...
- else:
    King Merlot: You did your best, I am certain, my friend... I am proud of you, no matter the outcome.
}

Cacildes: Thank you, your highness... though I wouldn't mind... a sip of orange juice right now...

@wait_.5

~ runEvent("Play Player Faint")

@wait_.5

-> END
