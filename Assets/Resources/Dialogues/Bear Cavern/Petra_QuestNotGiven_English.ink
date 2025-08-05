EXTERNAL runEvent(eventId)

- Petra: Have you seen my sister? She looks a lot like me... just younger.  

* [Do you have any idea where she went?]
    Petra: She took her bow and headed toward the Abandoned Wharf. Said she saw ships coming in. That place hasn’t seen a crew in years, so I understood her curiosity...
    <>But she’s been gone far too long. I’m starting to worry.

    ** [I’ll go look for her. Where exactly is the wharf?]
        ~ runEvent("Increase Reputation By 1 Point")
        ~ runEvent("Start Petra Quest")
        Petra: Thank you. If you came by the Slepbone Pathway, there's an old ladder carved into the cliffside — overgrown with moss, but still solid.  
        <>It’ll take you straight down to the wharf. And if the path confuses you, look for a weathered signpost. It still points true.

    ** [I have a younger brother. I get it. I’ll see what I can do.]
        ~ runEvent("Increase Reputation By 1 Point")
        ~ runEvent("Start Petra Quest")
        Petra: Then you understand. Thank you. If you came by the Slepbone Pathway, there's a hidden ladder carved into the cliffs — mossy, but steady.  
        <>It leads straight to the wharf. Watch for an old sign if the trail gets tricky.

    ** [Ships... an abandoned port? Sounds risky. What's in it for me?]
        ~ runEvent("Decrease Reputation by 1 point")
        Petra: Right. Should’ve guessed you'd only lift a finger for coin. Forget I asked.
        -> END

    ** [I’ll see what I can do — no promises.]
        ~ runEvent("Start Petra Quest")
        Petra: Fair enough. Just... if you find her, make sure she's safe. Please.

* [Goodbye]

-> END
