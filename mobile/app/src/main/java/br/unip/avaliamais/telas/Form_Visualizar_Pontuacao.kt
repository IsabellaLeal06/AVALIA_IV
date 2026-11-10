@file:Suppress("FunctionName")

package br.unip.avaliamais.telas

import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.alpha
import androidx.compose.ui.graphics.ColorFilter
import androidx.compose.ui.graphics.ColorMatrix
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import br.unip.avaliamais.controles.ControlePontuacao
import br.unip.avaliamais.ui.Caixa
import br.unip.avaliamais.ui.Carregando
import br.unip.avaliamais.ui.Cores
import br.unip.avaliamais.ui.Mensagem
import br.unip.avaliamais.ui.TituloPagina
import br.unip.avaliamais.ui.formatarNumero
import coil.compose.AsyncImage

/**
 * Tela Form_Visualizar_Pontuação (diagrama UML): visualizarPontuacao → exibirPontuacao.
 * RN23: mostra a medalha corrente e quantos pontos faltam para a próxima. Quem ainda não
 * alcançou a primeira faixa vê a Bronze acinzentada. Sem barra de progresso.
 */
@Composable
fun Form_Visualizar_Pontuacao(controle: ControlePontuacao) {
    LaunchedEffect(Unit) { controle.visualizarPontuacao() }
    val perfil = controle.perfil
    val g = perfil?.gamificacao

    Column(
        Modifier
            .fillMaxSize()
            .verticalScroll(rememberScrollState())
            .padding(16.dp)
    ) {
        TituloPagina("MINHA", "PONTUAÇÃO", "Cada acerto vale 10 pontos. Os pontos nunca diminuem.")
        Mensagem(controle.erro)
        if (controle.carregando && perfil == null) Carregando()

        if (perfil != null && g != null) {
            Caixa {
                Column(Modifier.fillMaxWidth(), horizontalAlignment = Alignment.CenterHorizontally) {
                    val exibida = g.medalha ?: g.proxima
                    if (exibida != null) {
                        AsyncImage(
                            model = controle.urlImagem(exibida.imagemUrl),
                            contentDescription = if (g.medalha != null) "Medalha ${g.medalha.nome}" else "Medalha ${exibida.nome} ainda não conquistada",
                            colorFilter = if (g.semMedalha) ColorFilter.colorMatrix(ColorMatrix().apply { setToSaturation(0f) }) else null,
                            modifier = Modifier
                                .size(140.dp)
                                .alpha(if (g.semMedalha) 0.45f else 1f)
                        )
                    }
                    Text(
                        if (g.medalha != null) "Medalha ${g.medalha.nome}" else "Ainda sem medalha",
                        fontWeight = FontWeight.Bold,
                        fontSize = 20.sp,
                        modifier = Modifier.padding(top = 12.dp)
                    )
                    Text(
                        "${formatarNumero(perfil.pontuacaoTotal)} pontos",
                        color = Cores.Azul,
                        fontWeight = FontWeight.Bold,
                        fontSize = 32.sp,
                        modifier = Modifier.padding(vertical = 6.dp)
                    )
                    Text(g.mensagem, textAlign = TextAlign.Center, color = Cores.TextoSuave)
                }
            }
            g.proxima?.let {
                Mensagem("Próxima faixa: ${it.nome}, a partir de ${formatarNumero(it.pontosMinimos)} pontos.", br.unip.avaliamais.ui.TipoMensagem.INFO)
            }
        }
    }
}
