EXTERNAL runEvent(eventId)

- Altaire: Hail, wanderer... I am Altaire. 
<> I came to Ministeria from distant soil, alongside my brother, upon a sacred errand. 

Altaire: This forge... it lies cold. Its flame was sealed centuries past, bound beneath this tower, slumbering in curse and silence. 
<> Will you lend your strength, and see it blaze once more?

* [I shall see this done.]
    Altaire: Then listen well... It was within the cavern’s heart that Maerimond was entombed. 
    <> His restless spirit lingers, chained in stone and wrath. 
    <> Stir him from his prison. Let his roar break the shackles, and his fire surge unto me... that I may awaken this forge.
    <> I see doubt upon your face. Fear not — I shall see you rewarded.
        ** [And how does one rouse such a spirit?]
            Altaire: With courage. With folly. With both.

            ~ runEvent("Show Maerimond Cavern")

            Altaire: Seek the cavern entrance, hidden in shadow. 
            <> Within, you will find Maerimond — bound, yet not broken. 
            <> Disturb him. Provoke him. His fire shall answer. 
        
            ~ runEvent("Hide Maerimond Cavern")

            Altaire: I will remain here, to shape the flame once it is loosed.

            ~ runEvent("Start Maerimond Quest")
        ** [Some things are best left buried...]
            Altaire: Perhaps. Yet consider... who decrees what must slumber, and what must waken? 
            <> Was his silence born of justice... or of treachery? 
            <> The answer lies not in caution, but in will.
            
* [Not now.]
    Altaire: Then so be it. The forge waits as I wait. 
    <> Return when your resolve is steadied.

-> END
