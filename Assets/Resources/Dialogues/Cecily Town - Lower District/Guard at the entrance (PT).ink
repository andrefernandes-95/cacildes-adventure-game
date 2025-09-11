EXTERNAL runEvent(eventId)

Town Guard (Gate): Saudações, cidadão. O que o traz pelo portão hoje?  

* [Perguntar direções para o castelo]  
    ~ runEvent("Show Upper District")  

    Town Guard (Gate): O castelo, é? Pois, posso indicar o caminho. Queres seguir para o Bairro Alto.  

    ~ runEvent("Show Thorgeir")  

    Town Guard (Gate): Segue em frente pelo mercado. Vais ouvir o Thorgeir na forja antes de o veres—o martelo dele nunca para.
    <> Bom lugar para melhorar o teu equipamento, mas não deixes que te convença a comprar algo a mais.  
    
    ~ runEvent("Show Tavern")  

    Town Guard (Gate): Depois vais passar pela taberna. O hidromel é bom, mas se parares, pode ser que não saias de lá antes de ficar com a carteira mais leve e a cabeça mais pesada.  
    
    ~ runEvent("Show Alchemist") 

    Town Guard (Gate): Depois está o alquimista—sempre com frascos a borbulhar, jura que tudo cura alguma coisa. Melhor continuares a caminho, a não ser que queiras cheirar a fumaça nos bolsos.  
    
    ~ runEvent("Show Library") 

    Town Guard (Gate): A biblioteca também fica por ali. Grande, cheia de livros empoeirados. Não é muito útil para quem tem pressa.  

    ~ runEvent("Show Castle")  

    Town Guard (Gate): Continua a subir e vais ver o portão do castelo bem à vista. Bate forte. Se ninguém responder, bate com mais força. Alguém aparecerá.  

    ~ runEvent("Hide Cameras")   

* [Ouvir rumores?]
    {shuffle:
        - Town Guard (Gate): Diz-se que um velho tem vivido nos esgotos, a fugir das suas dívidas. Estranha forma de viver, se me perguntas.
        - Town Guard (Gate): Difícil distinguir um elfo de um orc à noite… até que o vento leve o cheiro. Ervas doces para os elfos, humidade e podridão para os orcs.
        - Town Guard (Gate): Diz-se que o General Alcino escapou do inferno e abdicou do seu título de príncipe para viver livre entre os humanos. Difícil acreditar que um príncipe demónio encontraria paz entre o nosso povo. Mas depois olho para ele a beber e a devorar pernil na taberna… parece que encontrou o paraíso. Não sei.
    }

* [Adeus]  

-> END  
