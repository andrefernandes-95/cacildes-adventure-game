EXTERNAL runEvent(eventId)
EXTERNAL runEventOnce(eventId)  
EXTERNAL hasFinishedSewersAndIsReadyForReward()  
EXTERNAL hasStartedSewersQuest()  

~ temp finishedSewersAndIsReadyForReward = hasFinishedSewersAndIsReadyForReward()
~ temp startedSewersQuest = hasStartedSewersQuest()

- Pedro: A sorte favorece os audazes, cidadão. O castelo do rei acolhe todos… embora alguns mereçam mais do que outros.  

* {!startedSewersQuest} [Procuro trabalho.] 
    Pedro: Ha! Isso é o que gosto de ouvir. Poucos batem à porta de Pedro à procura de *mais sarilhos* de propósito.  

    Pedro: Já ouviste os sussurros, de certeza — Grischa e o seu bando de facínoras, escondidos nos esgotos da cidade como ratazanas.  

    Pedro: Os meus soldados tremem só de ouvir o nome dela. *“Come carne arrancada do osso”*, murmuram. Ou, *“Luta com duas foices, mais rápidas que as garras de um falcão.”* Bah — cobardes, todos eles.  

    Pedro: Tu? Nem pestanejas. Gosto disso. Talvez tenhas o aço para a derrubar, e a cabeça para não acabares tu no chão.  

    Pedro: A entrada dos esgotos fica junto à forja do ferreiro. Fala com Thorgeir se precisares de uma boa arma — mas lembra-te, Grischa não é um bêbedo de taverna para treinar golpes. Vais suar por cada investida.  

    Pedro: Faz isto por mim e garanto que serás bem recompensado. Nunca deixo dívidas por saldar.  

    ~ runEventOnce("Begin Sewers Quest")

* {finishedSewersAndIsReadyForReward} [Tratei da Grischa.] 
    Pedro: Ha! Então a história acaba contigo de pé e ela a apodrecer na lama. Isso sim é um feito digno de vinho.  

    Pedro: É pena nunca sabermos qual era o grande plano dela, mas na minha experiência? Vilões que pensam demais costumam morrer antes de acabar a frase.  

    Pedro: Prestaste um grande serviço ao rei, à cidade e a mim. Aqui está a tua recompensa, como prometido.  

    ~ runEventOnce("Reward player for Sewers Quest")

    @wait_0.5

    Pedro: Não te afastes demasiado. Homens como eu arranjam sempre mais trabalho… e quem sabe se até não lhe tomas o gosto.  

    ~ runEvent("Finish Sewers Quest")

* [Quem és tu?]
    Pedro: Hah! Pergunta justa. Em tempos, servi como general. Combati sob o comando do General Alcino, ao serviço do pai do Rei Merlot.  

    Pedro: Agora? Estou reformado. A espada pesa menos quando a trocamos por pergaminhos de conselho. Cuido das feridas da cidade, enquanto o rei carrega o peso do reino.  

    Pedro: A idade ensina-nos isto: a força não se mede apenas nas batalhas vencidas, mas também na sabedoria de guiar outros. E Slepbone, pequena como é, precisa mais de sabedoria do que de aço.  

    ** [O que aconteceu aos anões de Sunkenland?]
        Pedro: Orgulho foi a sua queda. Os anões, cegos de soberba, escravizaram os cindidos durante gerações. Não se acorrenta o fogo sem esperar ser queimado.  

        Pedro: Um dia, um escravo ergueu-se e iniciou uma rebelião. Quando as chamas se apagaram, a Fortaleza de Obsidiana estava vazia.
        <> Apenas paredes de pedra e sombras — foi esse o túmulo de um rei louco.  

    ** [Os elfos trazem conflitos ao rei?]
        Pedro: Conflitos? Não. Os elfos são paz em forma de gente — talvez até demasiado.
        <> A sua terra enriquece os nossos campos, e as suas águas sagradas abençoam os nossos rios.  

        Pedro: Mas a paz sem força é frágil. Eles não podem erguer muralhas contra saqueadores, demónios ou pior.
        <> Por isso, somos nós a fazê-lo. Esse é o dever de soldados e de homens: proteger aqueles que não se conseguem proteger.  

* [Ouvistes algum rumor?]
    {shuffle:
        - Pedro: O Cavaleiro Desajeitado gaba-se de um baú em sua casa cheio de troféus sempre que bebe demais.
            <> Se algum ladrão lhe der ouvidos, vai acordar nu e sem as botas.
            <> Dizem que o seu equipamento vale mais do que uma quinta inteira.  
        - Pedro: Diz-se que o General Alcino foi expulso do inferno por ser demasiado brando.
            <> Imagina! Um demónio com coração brando. Sorte a nossa — melhor um demónio temperado do que um em fúria.  
        - Pedro: Leva sempre uma cinza contigo, viajante.
        <> Um só punhado leva-te de volta à última fogueira onde descansaste.
        <> Já me salvou a pele uma vez — embora a história acabe com menos glória do que vergonha. Outra altura conto-te.  
    }

* [Adeus]  

-> END  
