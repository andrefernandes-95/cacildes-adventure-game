EXTERNAL runEvent(eventId)

- Mountainpass Barman: Tu! Deves 5 moedas à taberna da última vez que cá estiveste...
<>espera, tu és outro... não és aquele que começou a briga de ontem com os soldados? Não me lembro bem...

* [Deves estar a fazer confusão...]
    Mountainpass Barman: Talvez... mas e então, vais beber alguma coisa? 
        
        ** [Pedir algo]
            ~ runEvent("Buy from Mountainpass Barman")

        ** [Adeus]

* [Adeus]

-> END
