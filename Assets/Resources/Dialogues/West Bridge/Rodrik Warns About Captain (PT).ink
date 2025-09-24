EXTERNAL runEvent(eventId)
EXTERNAL runEventOnce(eventId)

Rodrik: Como é? Procuras o Capitão Pleias?...  

Rodrik: A verdade é que bebemos demais ontem à noite. Fui eu o idiota que o desafiou a ir ter com aquele orc, o Drogo — disse-lhe que o podia conquistar com palavras e bebida. *“És diplomata,”* disse eu. *“Acredito que até um orc cairia no teu jeito de falar.”*  

Rodrik: Quando dei por mim, já ele cambaleava em direcção à caverna. Tentei segui-lo, mas a bebida levou a melhor. Esta manhã, acordei ao lado de um rasto de garrafas... e sem capitão. Deuses, o que foi que eu fiz?  

Rodrik: ...  

* [Vou procurar o capitão]
    Rodrik: Farias isso? Óptimo. Eu fico aqui, a garantir que nenhum orc passa. Terás o meu apoio.
    -> RUN_QUEST

* [Deixaste-o ir sozinho? Grande soldado és tu.]
    ~ runEventOnce("Decrease reputation for belittling soldier")
    Rodrik: Duro, mas justo. Devia tê-lo impedido — bêbedo ou não. Espero que o capitão ainda esteja vivo... o rei há-de rebaixar-me a bobo outra vez, de certeza.
    -> RUN_QUEST

== RUN_QUEST ==

~ runEventOnce("Update Orcs At Our Door Quest")

-> END
