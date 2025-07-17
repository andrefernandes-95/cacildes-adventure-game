VAR hasTastedMead = false
EXTERNAL isDoingChickensQuest()

- Lara: You're from the farm down the road, right? Name's Lara.<>

~ temp doingChickensQuest = isDoingChickensQuest()

{ doingChickensQuest:
    <> I don't mean to pry, but you look a little on edge. Everything alright?
- else:
    <> I collect honey around these parts... Did you need something?
}

    * {doingChickensQuest} [I'm looking for my chickens... have you seen them?]
        Lara: Hm... can't say I have. But you know, animals usually leave little clues behind wherever they go.  
        <> If your chickens passed through here, you might spot some feathers caught in the brambles or scattered along the path.  
        <> Sorry I can't be more help — but I hope you find them soon.

    * [You collect honey from the bee hives? Don’t the bees sting you?]
        Lara: Oh, the bees and I are old friends. We understand each other.
        <> Of course... it doesn’t hurt to wear a little elven trinket that hums with calming magic. Keeps the girls mellow when I’m near.
        <> You're a curious one, aren't you? Tell me — do you have a sweet tooth, or just a passing interest in golden nectar?

        ** [Oh yes. In abundance, please. Pancakes aren't the same without it.]
            Lara: Ha! A kindred spirit. Real food lovers can spot each other from a mile away.
            <> Honey on warm pancakes — just enough to melt into the butter. That’s the kind of alchemy I live for.
            <> You and I? We'd eat our way through half the continent given half a chance.
            <> And the bees? They're the tiny golden chefs behind it all.

        ** [If you count mead, then the answer is yes!]
            Lara: I had a feeling. You strike me as the kind of adventurer who enjoys that extra kick mead gives your swing.  
            <> Sadly, I can’t handle alcohol myself — it makes my head all foggy and strange.  
            <> So I came up with a workaround: a cultured little bacteria that eats the alcohol but leaves the flavor untouched.  
            <> That’s how I enjoy my mead — no headaches, no hangovers, just the good stuff.  
            <> Nature’s full of clever loopholes... you just have to know where to look.

        ** [Not really. Bees scare me, to be honest.]
            Lara: Fair enough. Their buzzing can be a bit... intense if you’re not used to it.  
            <> But they don’t sting for sport — they only defend their home, just like you or me.  
            <> I’ve found that if you move slow, breathe steady, and avoid surprising them, they’ll usually leave you be.

        ** [Sweet things make my teeth ache. Never been a fan.]
            Lara: Hah! I guess not everyone’s born with a sweet tooth. The world needs a few of you to keep us sugar-fiends in balance.

    *[Buy Items]
    *[Sell Items]
    *[Goodbye]

-> END