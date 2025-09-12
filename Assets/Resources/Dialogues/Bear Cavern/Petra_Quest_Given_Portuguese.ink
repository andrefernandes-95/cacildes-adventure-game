EXTERNAL runEventOnce(eventId)
EXTERNAL hasRunEvent(eventId)

- Petra: Há uma escada antiga escavada na falésia dos Caminhos de Slepbone — coberta de musgo, mas ainda firme.  
Leva até à entrada do Porto Abandonado. Procura lá a minha irmã.

{hasRunEvent("Give Potion To Player") == false:
    Petra: Ah, toma... Acabei esta agora mesmo. Pode não parecer grande coisa, mas pode manter-te de pé quando mais precisares.
    ~ runEventOnce("Give Potion To Player")
}
