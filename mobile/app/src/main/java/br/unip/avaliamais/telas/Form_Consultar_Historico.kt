@file:Suppress("FunctionName")

package br.unip.avaliamais.telas

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.semantics.Role
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import br.unip.avaliamais.controles.ControleSimulado
import br.unip.avaliamais.dados.ItemHistorico
import br.unip.avaliamais.ui.BotaoSecundario
import br.unip.avaliamais.ui.Carregando
import br.unip.avaliamais.ui.Cores
import br.unip.avaliamais.ui.Mensagem
import br.unip.avaliamais.ui.TituloPagina
import br.unip.avaliamais.ui.formatarDataHora
import br.unip.avaliamais.ui.formatarPercentual

/**
 * Tela Form_Consultar_Historico (diagrama UML): consultarHistorico → exibirHistorico.
 * Só simulados finalizados, vindos da API e paginados (RN20).
 */
@Composable
fun Form_Consultar_Historico(controle: ControleSimulado, aoAbrirSimulado: (Int) -> Unit) {
    LaunchedEffect(Unit) { controle.consultarHistorico(1) }
    val historico = controle.historico

    LazyColumn(
        Modifier.fillMaxSize(),
        contentPadding = androidx.compose.foundation.layout.PaddingValues(16.dp),
        verticalArrangement = Arrangement.spacedBy(10.dp)
    ) {
        item { TituloPagina("MEUS", "SIMULADOS", "Toque em um simulado para revisar as questões.") }
        item { Mensagem(controle.erro) }
        when {
            controle.carregando && historico == null -> item { Carregando() }
            historico == null || historico.itens.isEmpty() -> item { Text("Nenhum simulado realizado ainda.", color = Cores.TextoFraco) }
            else -> {
                items(historico.itens, key = { it.id }) { simulado -> ItemSimulado(simulado) { aoAbrirSimulado(simulado.id) } }
                item {
                    Row(
                        Modifier.fillMaxWidth().padding(top = 6.dp),
                        horizontalArrangement = Arrangement.SpaceBetween,
                        verticalAlignment = Alignment.CenterVertically
                    ) {
                        BotaoSecundario("← Anterior", { controle.consultarHistorico(historico.pagina - 1) }, habilitado = historico.pagina > 1)
                        Text("Página ${historico.pagina} de ${historico.totalPaginas}", color = Cores.TextoFraco, fontSize = 13.sp)
                        BotaoSecundario(
                            "Próxima →",
                            { controle.consultarHistorico(historico.pagina + 1) },
                            habilitado = historico.pagina < historico.totalPaginas
                        )
                    }
                }
            }
        }
    }
}

@Composable
private fun ItemSimulado(simulado: ItemHistorico, aoTocar: () -> Unit) {
    Column(
        Modifier
            .fillMaxWidth()
            .clip(RoundedCornerShape(12.dp))
            .background(Cores.Superficie)
            .clickable(role = Role.Button, onClickLabel = "Revisar o simulado", onClick = aoTocar)
            .padding(16.dp)
    ) {
        Text(simulado.titulo, fontWeight = FontWeight.Bold)
        Row(Modifier.fillMaxWidth().padding(top = 6.dp), horizontalArrangement = Arrangement.SpaceBetween) {
            Text("${simulado.totalAcertos}/${simulado.totalQuestoes} acertos · ${formatarPercentual(simulado.percentual)}", color = Cores.TextoSuave)
            Text("+${simulado.pontosObtidos} pts", color = Cores.Azul, fontWeight = FontWeight.Bold)
        }
        Text(formatarDataHora(simulado.dataRealizacao), color = Cores.TextoFraco, fontSize = 12.sp, modifier = Modifier.padding(top = 4.dp))
    }
}
