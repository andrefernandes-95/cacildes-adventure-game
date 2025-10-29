EXTERNAL hasDefeatedHawkBand()  
EXTERNAL runEvent(eventId)  

~ temp defeatedHawkBand = hasDefeatedHawkBand()  

{defeatedHawkBand:  
    Khalea: Então, derrotaste o Bando do Falcão? Isso deve tê-los feito pensar duas vezes.  
    <> Achas que vão contactar-te para te juntares ao grande exército que andam a tentar formar? Quem sabe.  
- else:  
    Khalea: Vieste por causa do Bando do Falcão? Boa sorte.  
    <> Muitos cavaleiros valorosos vieram à procura da sua recompensa, e muitos desses acabaram por despertar junto à sua última fogueira.  
}  

* [Comprar itens]  
~ runEvent("Buy from Khalea")  

* [O Bando do Falcão?]  
Khalea: São os patifes que andam por estas terras. Diz-se que foram, em tempos, cavaleiros temíveis de uma terra distante,  
<> que vieram a Slepbone para reunir um exército e reconquistar a sua pátria das mãos de um tirano.  
<> Mas o tempo passou, e ninguém digno surgiu para se juntar a eles. Cansaram-se de esperar e perderam a esperança.  
<> Talvez sejas tu aquele que eles andam à procura.  

* [Vives aqui?]  
Khalea: Às vezes. Outras vezes gosto de vaguear por Slepbone sozinha.  
<> Quando me aborreço, vou até à Praia Dourada e apanho um barco com o samurai dourado.  
<> Leva-me em pequenas aventuras — encontramos tesouros em ilhas esquecidas, e por vezes até umas boas lutas.  
<> Fica-se cansada de estar aqui o dia todo, a lamentar o passado. Somos livres agora; não há razão para continuar a viver nas sombras antigas.  

* [Adeus]  
