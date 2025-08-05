EXTERNAL runEvent(eventId)
VAR received_potion = false

- Petra: There's an old ladder carved into the cliffside of Slepbone Pathway — overgrown with moss, but still solid. It
leads to the entrance of the Abandoned Wharf. Look for my sister there.

{received_potion == false:
    ~ runEvent("Give Potion To Player")
    Petra: Oh, here… I just finished this one. It may not seem like much, but it could keep you standing when it matters most.
    ~ received_potion = true
}
