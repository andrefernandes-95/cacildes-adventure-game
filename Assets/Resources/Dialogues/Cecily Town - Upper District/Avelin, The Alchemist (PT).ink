EXTERNAL runEvent(eventId)

{shuffle:
    - Avelin: Ah, um estranho! Não toques nesse frasco—ele grita quando o abres. Bem-vindo, bem-vindo.  

    - Avelin: Se vieste comprar, compra. Se vieste roubar, avisa-me, para eu envenenar a prateleira certa.  
}

* [Comprar itens]  
~ runEvent("Buy items from Avelin")  

* [És a alquimista da vila?]  

Avelin: Depende de quem perguntas. Marcel, o padre, chama-me bruxa, o mendigo chama-me santa, e a taberneira chama-me corvo louco.  
<> Eu? Chamo-me faminta por respostas. Alquimia é apenas a palavra mais bonita para obsessão.  

** [Como te tornaste alquimista?]  

Avelin: Fui aprendiz das Irmãs Mithriel, um convento obcecado pelo sopro da vida e pela sua fragilidade. Senhoras encantadoras… se gostas de chá que cheira a pó de túmulo.  

Avelin: Parti para estudar sozinha… e, para ser sincera… qualquer um pode lançar ervas numa caldeira e esperar por um milagre. Isso é cozinhar com consequências.  
<> Só quando deres o teu nome a uma receita, depois de horas de fracassos, podes realmente reivindicar o título de alquimista.  

Avelin: Hah… soarei elitista, não é? Não deixes que te assuste. Todos são bem-vindos aqui — experimenta, arrisca-te.  
<> Se precisares de ingredientes ou orientação, as minhas prateleiras — e o meu almofariz — são teus. Só… tenta não explodir nada no processo.  

* [Opiniões sobre alquimia...]  

{shuffle:  
    - Avelin: Dir-te-ão que alquimia é sobre equilíbrio — vida e morte, fogo e água.  
    <> Errado. É sobre obsessão. Mistura pó e ervas suficientes e ou curas uma febre…  
    <> ou rebentas o telhado da tua casa. Só os loucos se podem dar ao luxo de arriscar neste ofício.  

    - Avelin: A maioria dos alquimistas que conheci e tentaram transformar cobre em ouro falharam de forma espectacular…  
    <> Um deles voltou rico, contudo. Talvez tenha encontrado a fórmula certa…  

    - Avelin: Uma vez tentei criar uma poção para trazer o meu gato de volta. Pensei que tinha a receita correcta.  
    <> A garrafa riu os miados do meu animal durante três dias antes de se dissolver. Ainda a guardo. Um lembrete de que algumas coisas não estão feitas para regressar.  
}

* [Rumores]  

{shuffle:  
    - Avelin: Os aldeões murmuram que falo com os mortos quando a loja fecha.  
    <> Ridículo. Os únicos espíritos com quem falo são os que sobem das minhas caldeiras e queimam o ar.  

    - Avelin: Alguns dizem que as minhas poções são fortes demais… que provocam sonhos estranhos.  
    <> Talvez. Mas muitas vezes são os sonhos, e não o preparado, que curam. Ou assombram. Depende da tua constituição.  

    - Avelin: Chamam-me bruxa… até que os filhos deles ardem em febre ou os seus homens sangram no campo.  
    <> Aí, batem à minha porta. Hipocrisia, afinal, é o remédio mais antigo da aldeia.  
}

* [Adeus]  
