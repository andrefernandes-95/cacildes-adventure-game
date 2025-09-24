EXTERNAL runEvent(eventId)
EXTERNAL runEventOnce(eventId)  
EXTERNAL hasFinishedSewersAndIsReadyForReward()  
EXTERNAL hasStartedSewersQuest()  

~ temp finishedSewersAndIsReadyForReward = hasFinishedSewersAndIsReadyForReward()
~ temp startedSewersQuest = hasStartedSewersQuest()

- Pedro: Fortune favors the bold, citizen. The king’s castle welcomes all… though some deserve it more than others.

* {!startedSewersQuest} [I'm looking for work.] 
    Pedro: Ha! That’s what I like to hear. Not many come knocking on Pedro’s door for *more trouble* on purpose.  

    Pedro: You’ve heard the whispers, I’m sure—Grischa and her band of cutthroats nesting down in our sewers like rats.  

    Pedro: My own soldiers quake at her name. *"She eats flesh off the bone,"* they mutter. Or, *"She fights with two sickles, faster than a hawk’s talons."* Bah—cowards, the lot of them.  

    Pedro: You? You don’t flinch. I like that. Maybe you’ve got the steel to gut her, and the sense not to get gutted yourself.  

    Pedro: The sewer entrance lies near the blacksmith’s forge. Have a word with Thorgeir if you need to arm yourself—though mind you, Grischa’s no tavern drunk to spar with. She’ll make you sweat for every swing.  

    Pedro: Do this for me, and I’ll make sure you’re rewarded handsomely. I never leave debts unpaid—ask *any* of my old flames.  

* {finishedSewersAndIsReadyForReward} [I've dealt with Grischa.] 
    Pedro: Ha! So the stories end with you still standing and her rotting in the muck. That’s the sort of tale I can drink to.  

    Pedro: A shame we’ll never know her grand design, but in my experience? Villains who brood too much usually die before they can finish a sentence.  

    Pedro: You’ve done the king, the city, and me a great service. Here—your reward, as promised.  

    ~ runEvent("Reward player for Sewers Quest")

    Pedro: Don’t stray too far. Men like me always find more work… and who knows, you might even enjoy it.  

* [Who are you?]
    Pedro: Hah! A fair question. I once wore steel and scars for King Merlot's father under General Alcino’s banner.  

    Pedro: Now? I’m retired. The blade is lighter when you trade it for council scrolls. I tend the city’s wounds while the king tends the kingdom’s. And, well… let’s say I’ve found other hobbies.  

    Pedro: Slepbone is not a vast land, but it’s never short on quarrels: elves with their honeyed words, and the dwarves of Sunkenland—long gone, but not forgotten.  

    ** [What happened to the dwarves of Sunkenland?]
        Pedro: A tragedy. The dwarves, proud and blind, enslaved the sundered for generations. You don’t cage fire without getting burned.  

        Pedro: One day, a slave rose and sparked a rebellion. By the time it was done, the Obsidian Keep was nothing but shadows and silence. A fitting grave for a king too mad to listen.  

    ** [The elves bring conflict to the king?]
        Pedro: Conflict? No, no. They’re gentle folk—too gentle. Our fields thrive thanks to their lush land, and their sacred waters bless every river that touches us.  

        Pedro: But make no mistake: they can’t hold a wall against raiders, demons, or worse. Someone must shield them. That someone is always us. And me? I’ve never been shy about protecting those who can’t protect themselves. Especially when the wine flows freely.  

* [Heard any rumours?]
    {shuffle:
        - Pedro: The Faulty Knight brags about his trophy chest after a few meads. If a thief ever took him seriously, he’d wake up naked with his own boots stolen. Supposedly, his gear’s worth more than a small farm.  
        - Pedro: They say General Alcino was cast out of hell for being too soft. Imagine that! A demon too tender-hearted. Lucky for us—better a temperate demon than one with fire still in his belly.  
        - Pedro: Keep an ash in your pocket, traveler. One spark brings you back to the last bonfire you rested at. Saved my hide once, though that story ends in less glory and more shame. Another time, perhaps.  
    }

* [Goodbye]  

-> END  
