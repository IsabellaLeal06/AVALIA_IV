@file:Suppress("FunctionName")

package br.unip.avaliamais.telas

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import br.unip.avaliamais.controles.ControleDesempenho
import br.unip.avaliamais.ui.BarraDesempenho
import br.unip.avaliamais.ui.Caixa
import br.unip.avaliamais.ui.CardIndicador
import br.unip.avaliamais.ui.Carregando
import br.unip.avaliamais.ui.Mensagem
import br.unip.avaliamais.ui.TituloPagina
import br.unip.avaliamais.ui.TituloSecao
import br.unip.avaliamais.ui.formatarDataHora
import br.unip.avaliamais.ui.formatarNumero
import br.unip.avaliamais.ui.formatarPercentual

/** Tela Form_Desempenho (diagrama UML): visualizarDesempenho → exibirDesempenho, por DISCIPLINA (RF06). */
@Composable
fun Form_Desempenho(controle: ControleDesempenho) {
    LaunchedEffect(Unit) { controle.visualizarDesempenho() }
    val d = controle.desempenho

    Column(
        Modifier
            .fillMaxSize()
            .verticalScroll(rememberScrollState())
            .padding(16.dp)
    ) {
        TituloPagina("MEU", "DESEMPENHO", "Acompanhe a sua evolução e analise os seus resultados.")
        Mensagem(controle.erro)
        Row(horizontalArrangement = Arrangement.spacedBy(12.dp)) {
            CardIndicador("Taxa média de acerto", formatarPercentual(d?.mediaAcertos), Modifier.weight(1f))
            CardIndicador("Simulados realizados", formatarNumero(d?.qtdSimulados ?: 0), Modifier.weight(1f))
        }
        CardIndicador("Último simulado", formatarDataHora(d?.ultimoSimulado), Modifier.fillMaxWidth())

        Caixa {
            Column {
                TituloSecao("Desempenho por Disciplina", "Aproveitamento acumulado em cada disciplina.")
                when {
                    controle.carregando && d == null -> Carregando()
                    d == null || d.porDisciplina.isEmpty() -> Text("Faça um simulado para ver o seu desempenho por disciplina.")
                    else -> d.porDisciplina.forEach {
                        BarraDesempenho(it.disciplina, it.percentual, "${it.totalAcertos} de ${it.totalRespostas} questões")
                    }
                }
            }
        }
    }
}
