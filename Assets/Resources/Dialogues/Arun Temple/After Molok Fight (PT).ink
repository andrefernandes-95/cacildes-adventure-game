EXTERNAL runEvent(eventId)
EXTERNAL hasPreservedStone()
EXTERNAL getPlayerName()

~ temp isStonePreserved = hasPreservedStone()
~ temp PLAYER_NAME = getPlayerName()

@wait_1

~ runEvent("Show Drogo Cage Camera")

Drogo: Estamos de volta ao templo? Conseguiste...?

~ runEvent("Hide Drogo Cage Camera")

~ runEvent("Show Stone Camera")

{ isStonePreserved:
    Drogo: Não! A pedra... permanece intacta... Molok... as minhas visões falharam-me... o meu povo está condenado...
- else:
    Drogo: Sim... a Pedra do Fogo Frio foi destruída... Molok está livre, finalmente... o meu povo não esquecerá a tua ação... {PLAYER_NAME} - obrigado.
}

~ runEvent("Hide Stone Camera")

~ runEvent("Show King Camera")

@wait_1

King Merlot: Está terminado, Drogo.  
<> Pelo meu decreto, serás conduzido à mais alta fortaleza de Ministeria, para expiar a ruína que causaste.  

{ isStonePreserved:
    King Merlot: Graças a ti, {PLAYER_NAME}, esta pedra foi preservada, e os elfos irão sobreviver.  
    <> Que sirva como lembrança de que a ordem perdura quando humanos e elfos se unem por uma causa maior.
- else:
    King Merlot: Falhámos em proteger os elfos. Terei de viver com este fardo... Só posso esperar que o seu sofrimento  
    <> seja aliviado pelo conhecimento de que a justiça será feita.
}

@wait_1

~ runEvent("Hide King Camera")

~ runEvent("Show Cacildes Camera")

~ runEvent("Player And King Face Each Other")

@wait_.5

{ isStonePreserved:
    King Merlot: Prestaste-nos um grande serviço. Palavras não conseguem expressar o quanto te devemos...
- else:
    King Merlot: Fizeste o teu melhor, disso estou certo... Estou orgulhoso de ti, independentemente do resultado.
}

Cacildes: Obrigado, vossa alteza... embora não me importasse de beber... um gole de sumo de laranja agora...

@wait_.5

~ runEvent("Play Player Faint")

@wait_.5

-> END
