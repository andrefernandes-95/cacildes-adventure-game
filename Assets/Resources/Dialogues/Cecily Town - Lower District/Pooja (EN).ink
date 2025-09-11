EXTERNAL runEvent(eventId)

{shuffle:
    - Pooja: Namaste, dear! Please, come closer — see these silks from the Sunspire Sands! They sparkle like stars in the night, hai na?
    - Pooja: Salaam, dost! Your eyes are curious... Are you looking for something more beautiful than ordinary cloth? Maybe something with a little magic?
}

* [Buy gear]
    ~ runEvent("Buy from Pooja")

* [Who are you?]
    Pooja: Oh! I'm Pooja, a humble clothier. I arrived in this town many moons ago, after a bit of family drama with my caravan. You know how it goes!
    <> Since then, I've fallen in love with the art of clothing. To learn more, I journeyed all the way to Arun Village and studied with the elves — their embroidery is simply breathtaking!
    Pooja: While I was there, King Merlot himself asked me to craft a dress for a royal guest. He said my work helped him seal an important treaty! Can you believe it?
    <> As a thank you, he gifted me this workshop. Now I create beautiful things for heroes, nobles, and wanderers alike!

* [Heard any rumours?]
    {shuffle:
        - Pooja: Careful on Snailcliff Road, yaar — wolves are always lurking! People say a mage is sleeping under the swirling rocks, trapped by some old magic.
        - Pooja: After the war with the sundered, many dwarves left King Thorgeir's keep. There's a blacksmith on Slepbone Beach now, quietly making mysterious things.
        - Pooja: The forest wanderers say they protect the woods. I know why: some trees have a special resin that makes weapons super strong! People cut them down for it, sadly.
    }

* [Goodbye]
