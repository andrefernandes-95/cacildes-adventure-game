EXTERNAL runEvent(eventId)

~ runEvent("Disable Player Control")

Cacildes: Ufa... estas escadas deixariam até uma cabra-montesa sem fôlego.

~ runEvent("Balbino Faces Player")

@wait_1

Balbino: Tu outra vez... Não desistes, pois não? Tenho de respeitar isso. És como eu — movido por algo maior do que ti próprio.

Balbino: Toda a minha vida ardi com o mesmo fogo que os meus irmãos. A minha coragem nunca foi menor, nem a minha fúria mais fraca. Mas o meu sangue... esse foi o meu peso.  
<> A minha mãe não era orca. E por isso, sempre fiquei nas sombras, afastado do calor da fogueira.

~ runEvent("Show Stone")

Balbino: Vês esta pedra? Dentro dela está o nosso deus, Molok, acorrentado no silêncio.  
<> Imagina ser aquele que o libertaria... Ser o campeão do nosso povo... Ter um lugar em todas as mesas, aquecido no coração da chama...

~ runEvent("Show Balbino and Player")

Balbino: E ainda assim, aqui estou... parado, a hesitar. Já podia tê-la destruído há muito tempo. Então, por que é que ainda não o fiz?

Cacildes: Tu já sabes porquê. Esta pedra está onde deve estar.

Drogo: Está, é? Será mesmo?

~ runEvent("Drogo Appears")

Balbino: Drogo... eu...

Drogo: Basta, meio-orc. Envergonhas-te outra vez. Sempre a correr atrás de valor como um cão esfomeado atrás de migalhas.  
<> Nunca carregarás a nossa chama. Volta para os teus humanos. Já não há lugar para ti entre nós.

@wait_0.5

~ runEvent("Drogo Kicks Balbino")

@wait_0.4

~ runEvent("Balbino Knocked Out")

@wait_0.5

~ runEvent("Show Drogo Boss Battle Camera")

Drogo: E tu... Lembro-me de ti em Anathar, a terra do inverno sem fim.  
<> Molok também lá estava, a sonhar em fogo.

Drogo: O teu destino e o nosso estão entrelaçados. E o primeiro nó aperta-se aqui... comigo.  
<> Avança, e que isto seja decidido.

~ runEvent("Hide Drogo Boss Battle Camera")

@wait_0.5

~ runEvent("Enable Player Control")

~ runEvent("Begin Boss Battle")

-> END
