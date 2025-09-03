EXTERNAL runEvent(eventId)

- Altaire: Salve, viajante... Sou Altaire. 
<> Vim a Ministeria de terras distantes, junto do meu irmão, numa missão sagrada. 

Altaire: Esta forja... jaz fria. A sua chama foi selada há séculos, acorrentada sob esta torre, adormecida em maldição e silêncio. 
<> Darás a tua força, para que volte a arder?

* [Hei de fazê-lo.]
    Altaire: Então escuta bem... Foi no coração desta caverna que Maerimond foi sepultado. 
    <> Mas o seu espírito inquieto persiste, acorrentado em pedra e ira. 
    <> Desperta-o da sua prisão. Que o seu bramido quebre as algemas, e o seu fogo corra até mim... para que eu reacenda esta forja.
    <> Vejo dúvida no teu semblante. Não temas — serás recompensado.
        ** [E como se desperta tal espírito?]
            Altaire: Com coragem. Com loucura. Com ambos.

            ~ runEvent("Show Maerimond Cavern")

            Altaire: Procura a entrada da caverna, oculta nas sombras. 
            <> Lá dentro, encontrarás Maerimond — preso, mas não vencido. 
            <> Perturba-o. Provoca-o. O seu fogo responderá. 
        
            ~ runEvent("Hide Maerimond Cavern")

            Altaire: Eu ficarei aqui, para moldar a chama quando esta se libertar.

            ~ runEvent("Start Maerimond Quest")
        ** [Certas coisas devem permanecer enterradas...]
            Altaire: Talvez. Mas pensa... quem decreta o que deve dormir e o que deve erguer-se? 
            <> Foi o seu silêncio fruto de justiça... ou de traição? 
            <> A resposta não está na prudência, mas na vontade.
            
* [Agora não.]
    Altaire: Assim seja. A forja espera, como eu espero. 
    <> Volta quando a tua determinação estiver firme.

-> END
