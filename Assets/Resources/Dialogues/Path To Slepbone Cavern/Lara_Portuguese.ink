VAR hasTastedMead = false 
EXTERNAL isDoingChickensQuest() 
EXTERNAL runEvent(eventId)  

- Lara: És da fazenda ali ao fundo da estrada, certo? Chamo-me Lara.<>  

~ temp doingChickensQuest = isDoingChickensQuest()  

{doingChickensQuest:     
    <> Não quero ser intrometida, mas estás com um ar preocupado. Está tudo bem? 
  - else:     
    <> Costumo recolher mel por estes lados... E tu, o que fazes? 
}      

    * {doingChickensQuest} [Estou à procura das minhas galinhas... não as viste por aí?]         
        Lara: Hm... não posso dizer que tenha visto. Mas olha, os animais costumam deixar pistas por onde passam.           
        <> Se as tuas galinhas passaram por aqui, talvez encontres umas penas espalhadas pelo caminho.           
        <> Lamento não poder ajudar mais — mas espero que as encontres em breve.      

    * [Recolhes mel das colmeias? As abelhas não te picam?]         
        Lara: Ah, as abelhas e eu somos velhas amigas. Claro que ajuda usar um pequeno talismã élfico que vibra com magia calmante. Mantém as meninas tranquilas quando estou por perto.         
        <> Também gostas de mel?          

        ** [Oh sim. Em abundância! Panquecas não são a mesma coisa sem ele.]             
            Lara: Ha! Uma alma gémea. Os verdadeiros apreciadores de comida reconhecem-se à distância.             
            <> Mel sobre panquecas quentinhas — a derreter como manteiga. É esse tipo de alquimia que me move.          

        ** [Se contares com o hidromel, então sim!]             
            Lara: Haha. Tens idade para isso? Estou a brincar. Eu cá não aguento álcool nenhum. Deixa-me a cabeça num nevoeiro e cheia de dores. Mas encontrei uma solução: uma bactéria cultivada que consome o álcool e deixa o sabor intacto. É assim que faço o meu próprio hidromel sem álcool. A natureza está cheia de atalhos engenhosos, se souberes onde procurar!          

        ** [Na verdade, não. As abelhas assustam-me, para ser sincero(a).]             
            Lara: Justo. O zumbido delas pode ser... intenso, se não estiveres habituado(a).               
            <> Mas não picam por prazer — só defendem o lar, tal como tu ou eu faríamos. É melhor deixá-las em paz... a picada tem veneno, por isso se estiveres sem antídotos, o melhor é não arriscar.          

        ** [Doces fazem-me doer os dentes. Nunca fui grande fã.]             
            Lara: Hah! Nem todos nascem com gosto por doces, pois claro. O mundo precisa de alguns como tu, para manter os gulosos como eu em equilíbrio.      

    *[Comprar Itens]         
        ~ runEvent("Buy from Lara")     

    *[Vender Itens]         
        ~ runEvent("Sell to Lara")     

    *[Adeus]  
    
-> END
