EXTERNAL runEvent(eventId)
VAR received_potion = false

- Petra: The abandoned wharf… That’s where Thelma must be. I want nothing more than to follow you there, but—
<>She told me not to follow. *Made* me promise, even. But if she’s in trouble… promise me you won’t leave her out there alone.

-Still, I’ve been preparing what I can — potions, salves — just in case I *do* have to go after her.

{received_potion == false:
    ~ runEvent("GivePotionToPlayer")
    Petra: Here… I just finished this one. It may not seem like much, but it could keep you standing when it matters most.
    ~ received_potion = true
}

