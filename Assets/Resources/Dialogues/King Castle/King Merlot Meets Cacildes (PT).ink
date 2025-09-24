EXTERNAL runEvent(eventId)

~ runEvent("King Talks")

King Merlot: Tens marcação?
<> Isto é sobre o cargo de bobo da corte? Já está ocupado. Sim.
<> Bem, não. Despedimos o bobo da corte. Não tenho estado com disposição para risos e gargalhadas ultimamente.

* [Mostrar a carta de admissão do rei...]

~ runEvent("King Reads Letter")

    King Merlot: Ah sim. És filho da Gertrude.
    <> As minhas desculpas por esse lapso; este assunto dos orcs tem-me ocupado os pensamentos.
    <> O nosso exército precisa de toda a ajuda possível, amigo.

    ~ runEvent("King Hides Letter")

    King Merlot: Diz-me... sabes como te desenrascas com espada e escudo?

        ** [Espada, machados, punhos, se esmaga, sou excelente nisso]
            King Merlot: Perfeito! É exactamente esse tipo de coragem imprudente que precisamos.
            -> How_Much_You_Know

        ** [Prefiro feitiços e encantamentos]
            King Merlot: Ah... um feiticeiro! Esperto. Já conheceste o Cael, o nosso mago residente? Ou devo dizer... torre de brilhantismo temperamental? Mas divago...
            -> How_Much_You_Know

        ** [Sou um lutador experiente. Lutei contra muitas lesmas no caminho para cá...]
            King Merlot: Haha. As lesmas podem aborrecer-te, mas em breve estaremos a atirar-te lobos, aranhas e ainda pior.
            -> How_Much_You_Know


== How_Much_You_Know ==

    King Merlot: Quanto sabes sobre a minha conversa anterior com o General Alcino?

    *** [Nada, juro.]
    King Merlot: Ah... silêncio educado, ou má audição? De qualquer forma, estás perdoado.
    -> Ending

    *** [Algo sobre uma pedra e orcs à nossa porta.]
    King Merlot: Então escutaste bem.
    -> Ending

== Ending ==

King Merlot: Os orcs de Thorum, liderados pelo fanático Drogo, acamparam perto da Ponte Oeste — na orla de Cecily Town.

~ runEvent("King Talks")

King Merlot: Procuram uma pedra preciosa—a Pedra do Fogo Frio—que se diz estar guardada pelos nossos aliados elfos na Vila de Arun.
<> O Drogo acredita que é suficientemente real para marchar um exército em sua direcção.

King Merlot: Isso deve pôr-te a par. Aprendi através de longos anos de reinado que a sabedoria reside em revelações medidas.

~ runEvent("King Talks")

King Merlot: Entretanto, procura conhecer o General Alcino. Foi para a sua cabana perto da igreja, na baixa da cidade.

King Merlot: Deixa-o avaliar-te e dar-te a tua primeira tarefa. Quanto a mim, irei ausentar-me para os meus aposentos e procurar alguns tomos em busca de inspiração.

~ runEvent("King Talks")

King Merlot: Já agora, podes explorar o castelo à vontade. Abre baús, remexe por aí, vê que segredos consegues descobrir.
<> Tens a minha permissão... apenas não incendeies nada.

-> END
