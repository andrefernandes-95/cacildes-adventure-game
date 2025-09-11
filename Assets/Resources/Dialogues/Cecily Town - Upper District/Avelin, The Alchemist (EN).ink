EXTERNAL runEvent(eventId)

{shuffle:
    - Avelin: Ah, a stranger! Don’t touch that jar—it screams when you open it. Welcome, welcome.

    - Avelin: If you’re here to buy, buy. If you’re here to steal, let me know, so I can poison the right shelf.
}

* [Buy items]
~ runEvent("Buy items from Avelin")

* [Are you the town alchemist?]

Avelin: Depends who you ask. Marcel, the priest, calls me a witch, the beggar calls me a saint, and the tavern-wench calls me a mad old crow.  
<> Me? I call myself hungry for answers. Alchemy’s just the prettiest word for obsession.  

** [How did you become an alchemist?]

Avelin: I apprenticed under the Mithriel Sisters, a coven obsessed with the breath of life and the fragility of it all. Charming ladies… if you enjoy tea that smells faintly of grave dust.  

Avelin: I left to study on my own... and truth be told... anyone can toss herbs in a cauldron and hope for a miracle. That’s cooking with consequences. 
<> Only when you’ve named a recipe your own, after hours of failure, can you truly claim the title of an alchemist.  

Avelin: Hah… I sound elitist, don’t I? Don’t let me scare you off. All are welcome here — dabble, experiment.  
<> If you need ingredients or guidance, my shelves — and my mortar — are yours. Just… try not to explode anything in the process.  

* [Thoughts on alchemy...]

{shuffle: 
    - Avelin: They’ll tell you alchemy is about balance—life and death, fire and water.
    <> All wrong. It’s about obsession. Mix enough powders and herbs and you’ll either cure a fever
    <> or blow your roof off. Only the mad can afford to take the chances at this type of trade.

    - Avelin: Most alchemists I knew who tried to turn copper into gold failed spectacularly…  
    <> One of them came back rich, though. Perhaps he did find the right formula…

    - Avelin: Once I brewed a potion to bring back my cat. Thought I had the recipe right.  
    <> It laughed my pet’s meows for three days before it dissolved. I keep the vial. A reminder that some things aren’t meant to return.
}

* [Rumours]

{shuffle:  
    - Avelin: Townsfolk whisper that I speak to the dead when the shop shutters close.  
    <> Ridiculous. The only spirits I talk to are the ones that float up from my cauldrons and scald the air.  

    - Avelin: Some say my potions are too strong… that they stir strange dreams.  
    <> Perhaps. But it’s often the dreams, not the draught, that heal. Or haunt. Depends on your constitution.  

    - Avelin: They call me a witch… until their children burn with fever or their men bleed in the mud.  
    <> Then it’s my door they pound on. Hypocrisy, after all, is the oldest cure in town.  
}

* [Goodbye]
