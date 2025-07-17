VAR hasTastedMead = false
EXTERNAL isDoingChickensQuest()
EXTERNAL runEvent(eventId)

- Lara: You're from the farm down the road, right? Name's Lara.<>

~ temp doingChickensQuest = isDoingChickensQuest()

{ doingChickensQuest:
    <> I don't mean to pry, but you look a little on edge. Everything alright?
- else:
    <> I collect honey around these parts... What about you?
}

    * {doingChickensQuest} [I'm looking for my chickens... have you seen them?]
        Lara: Hm... can't say I have. But you know, animals usually leave little clues behind wherever they go.  
        <> If your chickens passed through here, you might spot some feathers scattered along the path.  
        <> Sorry I can't be more help — but I hope you find them soon.

    * [You collect honey from the beehives? Don’t the bees sting you?]
        Lara: Oh, the bees and I are old friends. Of course, it doesn’t hurt to wear a little elven trinket that hums with calming magic. Keeps the girls mellow when I’m near.
        <> You're a curious one. Do you have a sweet tooth for honey?

        ** [Oh yes. Honey in abundance! Pancakes aren't the same without it.]
            Lara: Ha! A kindred spirit. Real food lovers can spot each other from a mile away.
            <> Honey on warm pancakes — just enough to melt into the butter. That’s the kind of alchemy I live for.

        ** [Whatever’s left of it when I'm drinking my mead, of course]
            Lara: Haha. Are you old enough to drink? Just kidding. I can't handle alcohol at all. Makes my head too foggy and achy. But I did come up with a workaround: a cultured bacteria that eats the alcohol and leaves the flavor untouched. That's how I prepare my own dealcoholized mead. Nature's full of clever loopholes, if you know where to look!

        ** [Not really. Bees scare me, to be honest.]
            Lara: Fair enough. Their buzzing can be a bit... intense if you’re not used to it.  
            <> But they don’t sting for sport — they only defend their home, just like you or I would. They're definitely best left alone... their sting carries venom, so if you're short on antidotes, don't take any chances around them.

        ** [Sweet things make my teeth ache. Never been a fan.]
            Lara: Hah! I guess not everyone’s born with a sweet tooth. The world needs a few of you to keep us sugar-fiends in balance.

    *[Buy Items]
        ~ runEvent("Buy from Lara")
    *[Sell Items]
        ~ runEvent("Sell to Lara")
    *[Goodbye]

-> END
