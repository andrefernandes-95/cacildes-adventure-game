EXTERNAL hasDefeatedHawkBand()  
EXTERNAL runEvent(eventId)  

~ temp defeatedHawkBand = hasDefeatedHawkBand()  

{defeatedHawkBand:  
    Khalea: So, you’ve defeated the Hawk Band? That must’ve given them something to think about.  
    <> Do you think they’ll reach out to you to join that grand army they’ve been trying to build? Who can say.  
- else:  
    Khalea: Are you here for the Hawk Band? Best of luck.  
    <> Many fine knights have come seeking their bounty, and just as many have awakened beside their last bonfire.  
}  

* [Buy items]  
~ runEvent("Buy from Khalea")  

* [The Hawk Band?]  
Khalea: They’re the scoundrels around here. Word is, they were once formidable knights from a distant land,  
<> who came to Slepbone to rally an army and reclaim their homeland from a tyrant’s rule.  
<> But time passed, and no one worthy rose to join them. They’ve grown tired of waiting and weary from disappointment.  
<> Perhaps you’re the one they’ve been looking for.  

* [Do you live here?]  
Khalea: Sometimes. Other times, I wander through Slepbone on my own.  
<> When I get bored, I head to the Golden Beach and take a boat ride with the golden samurai.  
<> He takes me on little adventures — we find treasure on forgotten islands, sometimes even a fight or two.  
<> It gets dull staying here all day, mourning the past. We’re free now; no need to keep living in old shadows.  

* [Goodbye]  
