EXTERNAL runEvent(eventId)
EXTERNAL getReputation()
EXTERNAL hasBegunRobertoQuest()
EXTERNAL hasKilledRobertoAndIsWaitingForAReward()
EXTERNAL hasCompletedRobertoQuest()

~ temp hasBegunRoberto = hasBegunRobertoQuest()
~ temp killedRobertoAndIsWaitingForAReward = hasKilledRobertoAndIsWaitingForAReward()
~ temp robertoQuestComplete = hasCompletedRobertoQuest()
~ temp reputation = getReputation()

- Marcel: A paz esteja contigo, viajante.
<> {reputation < 0: Mas sinto sombras a seguir os teus passos.}
<> {reputation > 0: Os teus feitos precedem-te — uma luz bem-vinda nestes tempos conturbados.}

<> O que te traz ao nosso humilde santuário?

* {!hasBegunRoberto} [Para que servem esses regadores?]

Marcel: Ah… sim. Estes não são regadores comuns. Tenho usado água benta no cemitério, a tentar convencer o pobre Roberto a… ficar morto.  
<> Todas as noites ele arranha o caminho de volta, atormentando o povoado. Nada do que tentei até agora o detém. É um verdadeiro sarilho.

    ** [Como é que ele morreu?]

    Marcel: Um acidente trágico, na verdade. Afogou-se no rio, mesmo aqui junto à igreja. Estava a discutir consigo próprio, bêbado como um barril, a brandir a sua amada maça…  
    <> O peso traiu-o, e acabou por tombar no rio. Tal devoção a uma arma pode ser fatal, ao que parece.

    Cacildes: A sua maça… chegou a ser recuperada?

    Marcel: Não. Mas… talvez se alguém a encontrasse e a usasse para o enfrentar com alguma firmeza… pudesse finalmente descansar.

    Cacildes: Vou arriscar. Vale a tentativa.

    ~ runEvent("Begin Roberto Quest")

* {killedRobertoAndIsWaitingForAReward} [O Roberto já não voltará a incomodar.]

    Marcel: Dizes a verdade? Pelos deuses...  
    <> Já não se ouvirão os seus gritos atormentados a atravessar a meia-noite. Já não acordarão as crianças a gritar dos seus sonhos.  
    <> O mais sagrado de tudo — a alma de Roberto caminha agora livre, rumo ao renascimento que lhe está destinado.

    Marcel: As palavras sabem a pouco, mas fica certo disto: a vila deve-te uma grande dívida.  
    <> Aceita isto — moeda abençoada no altar, tocada pela chama sagrada.  

    ~ runEvent("Reward player with church gold")
    
    Marcel: Os meus cuidados, a minha água benta, o meu conselho — tudo teu, apenas pelo custo dos materiais.  
    <> E se alguma vez precisares de refúgio, estas portas abrir-se-ão para ti sem hesitar.  
    
    Marcel: O nome de Roberto será agora lembrado nas orações da tarde — não em medo, mas em gratidão,  
    <> pela paz que finalmente encontrou. O teu feito ecoa no reino divino, amigo.  

    ~ runEvent("Complete Roberto Quest")

* {robertoQuestComplete} [Comprar artigos]
    ~ runEvent("Buy church items")

* [És o padre da vila?]

Marcel: Cuido deste rebanho como o meu pai cuidava das suas vinhas, e o pai dele antes dele.  
<> Mas encontrei a fé através da perda — a minha mãe morreu ao dar-me à luz.  
<> Algumas noites pergunto-me se servir o divino me permitirá encontrá-la para além do véu.

** [Acreditas mesmo que isso é possível?]

Marcel: Ter fé é acreditar no que não se vê, em possibilidades sem prova.  
<> Os deuses falam através do vento, da chama, da raiz e da maré.  
<> Se se manifestam na natureza, porque não também numa reunião para lá da morte?

** [O teu pai era vinhateiro?]

Marcel: O melhor em Slepbone. Os seus vinhos curavam corações partidos… ou assim dizia ele.  
<> Herdei a sua paciência, mas não o seu paladar.  
<> Alguns dons saltam gerações; outros… transformam-se.

* [Quem são os deuses?]

Marcel: Antes de os lordes mortais governarem, foram os Quatro Primarcas a moldar o mundo.  
<> Vael’Noor forjou o sol a partir de brasas cósmicas e esculpiu as montanhas.  
<> Mithriel soprou vida na terra — cada semente que brota carrega a sua bênção.

Marcel: Korvak comanda as águas inquietas, desde os riachos serenos às tempestades furiosas.  
<> O sopro gelado de Anathar põe tudo à prova, enfraquecendo os fracos e fortalecendo os fortes.

Marcel: Depois vieram os lordes. Lorde Celes reina sobre as terras geladas de Anathar,  
<> enquanto Lorde Arun governa os vales férteis aqui em Slepbone.  
<> Erguem-se e caem como as estações, mas os Primarcas permanecem eternos.

Marcel: A nossa igreja não celebra rituais, mas serviço. Cuidamos dos cidadãos, curamos os doentes. O patrocínio é voluntário. O credo é pessoal, pois os deuses já se fazem sentir na própria natureza.

** [E como é que os veneram?]

Marcel: Através de atos, não de cerimónias.  
<> Cada nascer do sol, cada colheita, cada ferida curada é uma oração.  
<> Servimos o povo — isso é adoração suficiente.

** [E o inferno?]

Marcel: Conheces o General Alcino? Já governou lá.  
<> O inferno existe para as almas que desrespeitam a natureza e afrontam os deuses — mas não é eterno.  
<> Lá, refletem, arrependem-se e regressam purificadas, prontas para o próximo ciclo da vida. Pensa nisso como… um castigo divino temporário.

* [Rumores]

{shuffle:
    - Marcel: Bandidos têm comprado água benta antes de assaltos. Tolice, pensarem que isso apaga o pecado. O perdão divino exige arrependimento verdadeiro. Vendo-lha na mesma — talvez um dia aprendam.

    - Marcel: Os piratas andam de olho na nossa vila. As defesas são… otimistas. Felizmente, reside aqui um certo príncipe-demónio. Até o mal teme males maiores.

    - Marcel: A peixeira por vezes vende salmão já passado. Aprendi a cheirar duas vezes antes de comprar. O peixe fresco não deve cheirar a peixe.

    - Marcel: Uma estranha névoa assombra a estrada abandonada pela Floresta dos Druidas, depois da Ponte Oeste. Há coisas que é melhor ficarem intocadas, enterradas com as eras.
}

* [Adeus]
