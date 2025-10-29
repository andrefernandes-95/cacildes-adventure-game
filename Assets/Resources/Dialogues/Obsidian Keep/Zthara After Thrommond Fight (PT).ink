EXTERNAL runEvent(eventId)  
EXTERNAL runEventOnce(eventId)  

Zthara: Um momento, amigo. Aqui, nesta fortaleza amaldiçoada que assombrou os meus antepassados durante gerações, triunfaste sobre a sombra da montanha.  
<> Por isso, tens a gratidão do meu povo. Ainda assim, atrevo-me a pedir-te um último favor.  

* [Claro. O que precisas?]  
    
    ~ runEventOnce("Gain Reputation On Zthara Conversation")  
    
    -> Progress  

* [Não trabalho de graça. Paga primeiro, depois falamos.]  
    
    ~ runEventOnce("Lose Reputation On Zthara Conversation")  
    
    Zthara: Compreendo. Os teus feitos já trouxeram grande prestígio ao nosso povo. Deveria ter mostrado mais autossuficiência.  
    
    -> Ending  

* [Talvez mais tarde.]  
    
    Zthara: Muito bem. Encontrar-me-ás aqui se mudares de ideias.  
    
    -> Ending

== Progress ==  

    Zthara: Sob esta fortaleza existe uma prisão. Nela, o herói do meu povo permanece acorrentado em eterno desassossego, preso numa forma amaldiçoada.  
    <> Peço-te apenas que lhe concedas uma morte sagrada — libertando-o desta existência miserável.  

    ** [Depois de dragões e reis anões, um herói morto-vivo não deve ser grande problema.]  

    Zthara: Foi um grande herói no seu tempo. Canções sobre o favor da princesa por ele ecoavam secretamente por estes corredores.  
    <> Merece este último ato de misericórdia — pela sua honra, e pelo nosso povo.  

    @wait_.5  

    Zthara: Aqui...  

    ~ runEvent("Give Key To Jail")  

    @wait_.5  

    Zthara: Esta é a chave para a prisão abaixo. Tem cuidado com Danuris, o carcereiro. Pode oferecer-te mais do que apenas uma luta,  
    <> pois a sua fome é insaciável.  

    @wait_1  

    Zthara: Duvido que os nossos caminhos se voltem a cruzar... mas obrigado. Deixaste a tua marca entre nós, e farei com que o teu nome seja gravado em tinta — para sempre inscrito na nossa história.  

    ~ runEvent("End Zthara Interaction")  

    -> Ending

== Ending ==
    -> END  
