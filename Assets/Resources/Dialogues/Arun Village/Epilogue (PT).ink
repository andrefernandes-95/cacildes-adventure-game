EXTERNAL runEvent(eventId)
EXTERNAL hasPreservedStone()
EXTERNAL getPlayerName()

~ temp isStonePreserved = hasPreservedStone()
~ temp PLAYER_NAME = getPlayerName()

@wait_1

~ runEvent("Show Celebration Camera")
~ runEvent("Fenlora Talks")

{ isStonePreserved:
    Fenlora: ...{PLAYER_NAME}, pareces descansado.
    <> As árvores agraciaram os teus sonhos e restauraram a tua força — como sempre o farão para nós, elfos, até ao fim dos tempos.
- else:
    Fenlora: ...Pareces bem, {PLAYER_NAME}.
    <> Queria poder dizer o mesmo do meu povo. A pedra... está perdida. Partida para sempre.
}

{ isStonePreserved:
    Fenlora: A tua graça em proteger a pedra viverá para sempre nos corações do povo de Arun. Tens a minha mais profunda gratidão.
- else:
    Fenlora: Não fiques triste. Isto não foi culpa tua. Já enfrentámos dificuldades antes.
    <> Repararemos os danos feitos na nossa floresta e aprenderemos a adaptar-nos ao nosso novo destino.
}

~ runEvent("King Merlot Talks")

King Merlot: A verdade seja dita. Ninguém ousou ir tão longe quanto tu, {PLAYER_NAME}. Tenho o direito. Não, o dever... de te conceder
<> isto... por favor, ajoelha-te?

* [Ajoelhar perante o rei]

~ runEvent("Hide Celebration Camera")
~ runEvent("Kneel Towards King")

@wait_1

King Merlot: Carregas as cicatrizes da batalha e o coração de um bravo. Levanta-te, {PLAYER_NAME}.
<> Levanta-te não apenas como soldado do rei... mas como Cavaleiro da Cidade de Cecily.

** [Levantar]

@wait_1

~ runEvent("Stand Up")
~ runEvent("Show Celebration Camera")

General Alcino: E então, sentes-te diferente agora que és cavaleiro?

Cacildes: Continua igual, acho eu.

King Merlot: Bom. Há muito trabalho a fazer, e precisamos do teu ânimo, {PLAYER_NAME}. Descansa agora, meu amigo. Com o tempo,
<> a tua aventura continuará.

Fenlora: Descansa bem, Cacildes. Terás sempre um lugar entre os elfos de Arun.

General Alcino: Podemos finalmente comer uma fatia de rolo de carne? Estou morto de fome...

~ runEvent("Play Laughter")

~ runEvent("Hide Celebration Camera")

@wait_1
