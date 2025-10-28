EXTERNAL runEvent(eventId)

{shuffle:
    - Pooja: Namastê, querido(a)! Por favor, chega-te mais perto — vê estes tecidos das Areias de Sunspire! Brilham como estrelas na noite, não é?
    - Pooja: Salaam, amigo(a)! Os teus olhos são curiosos... Procuras algo mais bonito do que um tecido comum? Talvez algo com um pouco de magia?
}

* [Comprar equipamento]
    ~ runEvent("Buy from Pooja")

* [Quem és tu?]
    Pooja: Oh! Eu sou a Pooja, uma simples costureira. Cheguei a esta cidade há muitas luas, depois de algum drama familiar com a minha caravana. Sabes como é!
    <> Desde então, apaixonei-me pela arte do vestuário. Para aprender mais, viajei até à Vila de Arun e estudei com os elfos — o bordado deles é simplesmente deslumbrante!
    Pooja: Enquanto lá estava, o Rei Merlot pediu-me para criar um vestido para uma convidada real. Disse que o meu trabalho o ajudou a selar um tratado importante! Consegues acreditar?
    <> Como agradecimento, ofereceu-me esta oficina. Agora crio coisas bonitas para heróis, nobres e viajantes!

* [Ouviste algum rumor?]
    {shuffle:
        - Pooja: Tem cuidado na Estrada de Snailcliff, amigo(a) — há sempre lobos à espreita! Dizem que um mago dorme debaixo das rochas rodopiantes, preso por uma magia antiga.
        - Pooja: Depois da guerra com os partidos, muitos anões deixaram a fortaleza do Rei Thrommond. Agora há um ferreiro na Praia de Slepbone, a fazer coisas misteriosas em silêncio.
        - Pooja: Os andarilhos da floresta dizem que protegem os bosques. Eu sei porquê: algumas árvores têm uma resina especial que torna as armas super resistentes! Infelizmente, as pessoas cortam-nas por isso.
    }

* [Adeus]
