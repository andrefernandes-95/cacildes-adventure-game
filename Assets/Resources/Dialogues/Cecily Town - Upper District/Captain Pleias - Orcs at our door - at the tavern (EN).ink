EXTERNAL runEvent(eventId)
EXTERNAL runEventOnce(eventId)

Captain Pleias: Ah... there you are.

~ runEvent("Show king and alcino in the tavern")

Captain Pleias: The king wishes to speak with you.

~ runEvent("Hide king and alcino in the tavern")

Captain Pleias: This is my last... *official* act... (hic)... hail the king... hail to you!

* [How are you?]
    Captain Pleias: I owe you my life. If not for you, the orcs would’ve finished me.  
    <> The king wasn’t too pleased with my... escapade. He gave me two choices: sober up and keep the post, or pack up and serve elsewhere.

    ** [That’s fair. But... is that mead in your hand?]
        Captain Pleias: (hic) ...Aye. I chose the easier road. The bottle, not the crown.

        *** [You nearly died because of it. Don’t you think it’s time to let go?]

            ~ runEventOnce("Award reputation for trying to convince Pleias to stop drinking")

            Captain Pleias: You’re not wrong. But without mead... I lose my spark. My courage, my wit—it all flows from the cup.  

            **** [Then it isn’t truly you. Try standing on your own, without it.]  
                Captain Pleias: Hmph... perhaps you’re right. (hic) Tomorrow. Yes. Tomorrow I’ll try.  

            *** [That’s your choice. I won’t press the matter.]  
                Captain Pleias: (smirks) At least you’re no nag. I’ll drink to that...  

* [Goodbye]
