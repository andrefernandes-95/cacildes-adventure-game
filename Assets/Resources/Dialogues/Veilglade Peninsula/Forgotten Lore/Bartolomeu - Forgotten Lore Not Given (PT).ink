EXTERNAL runEvent(eventId)  
EXTERNAL runEventOnce(eventId)  
EXTERNAL getPlayerName()  

~ temp PLAYER_NAME = getPlayerName()  

Bartolomeu: Uma cara nova, hein? Bem-vindo às Veilglades, viajante. O meu nome é Bartolomeu.  

Bartolomeu: Em tempos fui um erudito — antes do Afundamento. Tinha um futuro promissor pela frente... ou assim pensei.  
<> É uma reviravolta cruel do destino... em mil anos de descobertas, calhou-me viver na era em que a própria Universidade das Veilglades foi engolida pelo mar.  
<> Mas a vida raramente nos dá um jogo justo, amigo. Então — e tu, como te chamas?  

Cacildes: Prazer, Bartolomeu. Sou {PLAYER_NAME}. Então... o que aconteceu exatamente à tua universidade?  

Bartolomeu: Ah, essa história... já não passa de fragmentos na memória. Havia uma estudante — brilhante, mas consumida pelas artes proibidas.  
<> Numa noite, encontrou um tomo amaldiçoado e, antes que o sol nascesse, os corredores da universidade estavam cobertos de sangue.  

Bartolomeu: Perderam-se incontáveis vidas. O reitor decidiu afundar toda a universidade, na esperança de selar a corrupção sob as ondas.  
<> Um ato desesperado... e, no fim, inútil. A mancha já se tinha espalhado por toda a ilha.  

@wait_0.5  

Bartolomeu: História sombria, não é? As minhas desculpas pelo tom lúgubre. Não era assim que queria dar-te as boas-vindas.  

~ runEvent("Show University Camera")  

Bartolomeu: Ainda assim... se olhares para o nevoeiro, ainda consegues vislumbrar as ruínas da velha universidade, meio engolidas pelo mar.  

~ runEvent("Hide University Camera")  

Bartolomeu: Uma pena, de facto... Dava tudo para voltar a percorrer aqueles corredores. Às vezes pergunto-me se o Loras — o Elmo — ainda vagueia lá em baixo,  
<> enferrujado nas profundezas.  

@wait_0.5  

Cacildes: Loras?  

Bartolomeu: Um velho amigo. Estava lá quando tudo se afundou. Se algum dia te aventurares pelos corredores submersos...  
<> diz-lhe que o Bartolomeu ainda se lembra — e que ainda lhe deve uma bebida.  
