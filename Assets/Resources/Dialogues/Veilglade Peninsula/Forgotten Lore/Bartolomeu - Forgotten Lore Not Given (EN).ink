EXTERNAL runEvent(eventId)  
EXTERNAL runEventOnce(eventId)  
EXTERNAL getPlayerName()  

~ temp PLAYER_NAME = getPlayerName()  

Bartolomeu: A new face, eh? Welcome to the Veilglades, traveler. The name’s Bartolomeu.  

Bartolomeu: Once, I was a scholar — before the Drowning. Had a bright future ahead, or so I thought.  
<> It’s a cruel twist of fate... in a thousand years of discovery, I had to live in the age when the Veilglade University itself was buried beneath the waves.  
<> But life rarely deals us a fair hand, friend. Anyway — you’ve got a name?  

Cacildes: Pleasure, Bartolomeu. I’m {PLAYER_NAME}. So... what exactly happened to your university?  

Bartolomeu: Ah, that tale... it’s little more than fragments now. There was a student — brilliant, but consumed by forbidden arts.  
<> One night, she unearthed a cursed tome, and before dawn broke, the university halls were awash in blood.  

Bartolomeu: Countless lives were lost. The headmaster chose to sink the entire university, hoping to seal the corruption beneath the waves.  
<> A desperate act... and ultimately a futile one. The taint had already spread across the island.  

@wait_0.5  

Bartolomeu: Grim tale, isn’t it? My apologies for the gloom. Hardly the welcome I meant to give.

~ runEvent("Show University Camera")

Bartolomeu: Still... if you look out toward the mists, you can still glimpse the ruins of the old university, half-swallowed by the sea.  

~ runEvent("Hide University Camera")

Bartolomeu: A pity, really... I’d give anything to walk those halls again. I sometimes wonder if Loras — the Helm — still lingers down there,  
<> rusting away in the depths.  

@wait_0.5

Cacildes: Loras?  

Bartolomeu: An old friend. He was there when it all sank. If you ever find yourself wandering the drowned halls...  
<> tell him Bartolomeu still remembers — and still owes him a drink.  
