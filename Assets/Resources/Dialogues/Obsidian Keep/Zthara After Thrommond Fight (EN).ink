EXTERNAL runEvent(eventId)  
EXTERNAL runEventOnce(eventId)  

Zthara: A moment, friend. Here, in this accursed keep that has haunted my ancestors for generations, you’ve triumphed over the mountain’s shadow.  
<> For this, you have my people’s gratitude. Yet, I would dare to request one last favor.  

* [Sure. What do you need?]  

    ~ runEventOnce("Gain Reputation On Zthara Conversation")  

    -> Progress  

* [I don’t work for free. Pay up, then we’ll talk.]  
    ~ runEventOnce("Lose Reputation On Zthara Conversation")  

    Zthara: I understand. Your deeds have already brought great favor upon us. I should have shown more self-reliance.  

    -> Ending  

* [Maybe later.]  
    Zthara: Very well. You’ll find me here if you change your mind.  

    -> Ending  

== Progress ==  

Zthara: Below this keep lies a jail. Within it, the hero of my people remains shackled in eternal unrest, bound in an accursed form.  
<> I ask only that you grant him a holy death — to free him from this wretched existence.  

** [After dragons and dwarven kings, an undead hero shouldn’t be much trouble.]  

Zthara: He was a great hero in his time. Songs of the princess’s favor for him once echoed secretly through these halls.  
<> He deserves this final act of mercy — for his honor, and for our people.  

@wait_.5

Zthara: Here...

~ runEvent("Give Key To Jail")  

@wait_.5

Zthara: This is the key to the jail below. Beware of Danuris, the jailkeeper. He may offer you more than just a fight,  
<> for his hunger is insatiable.  

@wait_1  

Zthara: I doubt our paths will cross again... but thank you. You’ve left your mark upon us, and I’ll see your name carved in ink — forever etched into our history.  

~ runEvent("End Zthara Interaction")  

-> Ending

== Ending ==
    -> END  
