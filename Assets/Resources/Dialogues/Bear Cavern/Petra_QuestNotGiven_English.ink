EXTERNAL runEvent(eventId)

- Petra: Have you seen my sister, Thelma? She’s missing…  I’m starting to fear the worst.

* [Do you know where she might have gone?]
    Petra: She saw boats come in—no flags, no lanterns—slipping into the cove near the abandoned wharf. Like they didn’t want to be seen.
    <>She said something felt off. That they were here for her. She wouldn’t say why—just grabbed her bow and left before I could stop her.
    <>She told me not to follow. Said she could handle it alone. But that was last night. She should’ve been back by now.

    ** [I’ll go to the abandoned wharf and look for her.]
        Petra: You would? Thank you. The forest doesn’t forget those who stand with her.

    ** [That sounds dangerous. What do I get if I help?]
        ~ runEvent("Decrease Reputation by 1 point")
        Petra: You're bargaining while a life might be at stake? ...Fine. Help bring her back, and the tribe will make sure you’re rewarded. We don’t forget those who aid us.
        -> END
    ** [I’ll see what I can do. No promises.]
        Petra: Fair enough. Just… if you find her, guide her back to me. Please.

    - Cacildes: How do I get to the abandoned wharf?

    - Petra: If you came from the Slepbone Pathway, there’s a hidden ladder carved into the cliffs—covered in moss, but still holding strong. 
    <>It leads straight down to the wharf. Or look for road sign for help if the path twists on you.

    - Cacildes: Got it. I’ll see what I can do.

-> END
