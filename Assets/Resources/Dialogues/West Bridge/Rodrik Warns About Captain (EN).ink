EXTERNAL runEvent(eventId)

Rodrik: What’s that? Looking for Captain Pleias?...  

Rodrik: Truth is, we’d had too much ale last night. I might’ve been the fool who dared him to seek out that orc, Drogo—told him he could win him over with words and drink. *“You’re a diplomat,”* I said. *“You could charm an orc if you tried.”*  

Rodrik: Next thing I knew, he was staggering down toward the cavern. I tried to follow, but the drink put me under. When I woke this morning, all I found was a trail of bottles... and no captain. Gods, what have I done?  

Rodrik: ...  

* [I’ll look for the captain]
    Rodrik: You’d do that? Good. I’ll hold here and make sure no orcs slip past. You’ll have my support.
    -> RUN_QUEST

* [You let him go alone? Some soldier you are.]
    ~ runEventOnce("Decrease reputation for belittling soldier")
    Rodrik: Harsh, but fair. I should’ve stopped him—drunk or not. I hope the captain is alive... the king will surely demote me to jester again after this.
    -> RUN_QUEST

== RUN_QUEST ==

~ runEvent("Update Orcs At Our Door Quest")

-> END
