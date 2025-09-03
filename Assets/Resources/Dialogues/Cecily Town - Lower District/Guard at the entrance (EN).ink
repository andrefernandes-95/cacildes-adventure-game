EXTERNAL runEvent(eventId)

Town Guard (Gate): Hail, citizen. What brings you through the gate today?  

* [Ask directions to the castle]  
    ~ runEvent("Show Upper District")  

    Town Guard (Gate): The castle, eh? Aye, I can point you the way. You’ll want the Upper District.  

    ~ runEvent("Show Thorgeir")  

    Town Guard (Gate): Head straight on through the market. You’ll hear Thorgeir at the forge before you see him—hammer never stops.
    <> Good place to fix your gear, but don’t let him rope you into a sales pitch.  
    
    ~ runEvent("Show Tavern")  

    Town Guard (Gate): You’ll pass the tavern next. Mead’s fine, but if you stop, you may not leave ‘til your purse is lighter and your head heavier.  
    
    ~ runEvent("Show Alchemist") 

    Town Guard (Gate): Then there’s the alchemist—always got bottles bubbling, swears everything cures something. Best keep moving unless you fancy smoke in your pockets.  
    
    ~ runEvent("Show Library") 

    Town Guard (Gate): Library’s up that way too. Big, dusty place full of old books. Not much use to a traveler in a hurry.  

    ~ runEvent("Show Castle")  

    Town Guard (Gate): Keep climbing the road and you’ll see the castle gates clear as day. Give a good knock. If no one answers, knock harder. Someone’ll show.  

    ~ runEvent("Hide Cameras")   

* [Heard any rumours?]
    Town Guard (Gate): {shuffle:
        - Folks say an old man’s been living in the sewers, hiding from his debts. Strange way to live, if you ask me.
        - Hard to tell an elf from an orc at night… ‘til the wind carries the scent. Sweet herbs for elves, damp rot for orcs.
        - Legend says General Alcino escaped hell and gave up his prince status to live free among humans. I find it hard to believe a demon prince would find solace among our kin. But then I look at him drinking and eating pork shoulder at the tavern… and it’s like he’s found heaven. I don’t know.
    }

* [Goodbye]  

-> END  
