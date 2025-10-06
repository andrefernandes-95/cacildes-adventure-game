EXTERNAL runEvent(eventId)

~ runEvent("Disable Player Control")

Cacildes: Whew... those stairs could tire a mountain goat.

~ runEvent("Balbino Faces Player")

@wait_1

Balbino: You again... You never give up, do you? I can respect that. You're like me—pushed forward by something greater than yourself.

Balbino: My whole life, I burned with the same fire as my kin. My courage was no less, my fury no weaker. But my blood... that was my curse. 
<> My mother was not of the orcs. And so I was always left in the shadows, shunned at the edges of the flame.

~ runEvent("Show Stone")

Balbino: See this stone? Within it lies our god, Molok, chained in silence. 
<> Imagine being the one to free him... To be named champion of our people... To claim a place at every table, warmed at the very heart of the fire...

~ runEvent("Show Balbino and Player")

Balbino: And yet here I remain... staring, waiting. I could have shattered it long ago. So why do I still hesitate?

Cacildes: You already know why. This stone belongs where it stands.

Drogo: Belongs? Does it?

~ runEvent("Drogo Appears")

Balbino: Drogo... I...

Drogo: Enough, half-blood. You shame yourself again. Always clawing after worth like a starving dog after scraps. 
<> You’ll never bear our flame. Crawl back to your humans. You have no place among us.

@wait_0.5

~ runEvent("Drogo Kicks Balbino")

@wait_0.4

~ runEvent("Balbino Knocked Out")

@wait_0.5

~ runEvent("Show Drogo Boss Battle Camera")

Drogo: And you... I saw you in Anathar, the land of endless winter. 
<> Molok was there too, dreaming in fire.

Drogo: Your fate and ours are bound together. And the first knot is tied here... with me. 
<> Step forward, and let this be decided.

~ runEvent("Hide Drogo Boss Battle Camera")

@wait_0.5

~ runEvent("Enable Player Control")

~ runEvent("Begin Boss Battle")

-> END
