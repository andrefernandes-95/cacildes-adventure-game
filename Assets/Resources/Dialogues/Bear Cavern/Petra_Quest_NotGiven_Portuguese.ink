EXTERNAL runEvent(eventId)

- Petra: Viste por aí a minha irmã? É parecida comigo... só que mais nova.

* [Sabes para onde é que ela foi?]
    Petra: Levou o arco e seguiu para o Porto Abandonado. Disse que avistou uns navios a chegar. Ninguém usa aquele porto há anos, por isso percebo a curiosidade...  
    <>Mas já passou demasiado tempo. Começo a ficar preocupada.

    ** [Vou procurá-la. Onde fica esse cais?]
        ~ runEvent("Increase Reputation By 1 Point")
        Petra: A sério? Obrigada. Se vieste pelos Caminhos de Slepbone, há uma escada esculpida na falésia — está coberta de musgo, mas ainda aguenta bem.  
        <>Leva-te directamente ao cais. E se o trilho te baralhar, procura um velho poste de sinalização. Ainda aponta na direcção certa.
        ~ runEvent("Start Petra Quest")

    ** [Tenho um irmão mais novo. Claro que vou ver como ela está.]
        ~ runEvent("Increase Reputation by 1 Point")
        Petra: Tens? Então sabes bem como ficamos aflitos. Se vieste pelos Caminhos de Slepbone, há uma escada antiga cravada na falésia — coberta de musgo, mas firme.  
        <>Desce até ao cais. Se o trilho se complicar, segue um velho sinal de madeira — ainda deve lá estar.
        ~ runEvent("Start Petra Quest")

    ** [Navios num porto abandonado...? Parece perigoso. E o que ganho com isso?]
        ~ runEvent("Decrease Reputation by 1 point")
        Petra: Pois. Devia ter percebido que só te mexias por interesse. Desculpa o incómodo.
        -> END

    ** [Vou ver o que posso fazer... mas não prometo nada.]
        Petra: Justo. Só te peço uma coisa — se a encontrares, certifica-te de que está bem. Por favor.
        ~ runEvent("Start Petra Quest")

* [Adeus]

-> END
