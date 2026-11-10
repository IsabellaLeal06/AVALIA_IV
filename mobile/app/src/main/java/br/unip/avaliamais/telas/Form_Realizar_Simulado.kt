@file:Suppress("FunctionName")

package br.unip.avaliamais.telas

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.selection.selectable
import androidx.compose.foundation.selection.selectableGroup
import androidx.compose.foundation.selection.toggleable
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.AlertDialog
import androidx.compose.material3.Checkbox
import androidx.compose.material3.RadioButton
import androidx.compose.material3.Text
import androidx.compose.material3.TextButton
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.semantics.Role
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import br.unip.avaliamais.controles.ControleSimulado
import br.unip.avaliamais.ui.BotaoPrincipal
import br.unip.avaliamais.ui.Caixa
import br.unip.avaliamais.ui.Carregando
import br.unip.avaliamais.ui.Cores
import br.unip.avaliamais.ui.Mensagem
import br.unip.avaliamais.ui.TipoMensagem
import br.unip.avaliamais.ui.TituloPagina
import br.unip.avaliamais.ui.TituloSecao

/**
 * Tela Form_Realizar_Simulado (diagrama UML): o aluno escolhe duas ou mais disciplinas (RN13)
 * e 10, 15 ou 20 questões (RN14); depois lê as instruções e chama iniciarSimulado.
 * As disciplinas vêm da API (só ativas). O servidor confere tudo de novo antes de sortear.
 */
@Composable
fun Form_Realizar_Simulado(controle: ControleSimulado, aoIniciar: (idSimulado: Int) -> Unit) {
    LaunchedEffect(Unit) { controle.listarDisciplinasAtivas() }
    var mostrarInstrucoes by remember { mutableStateOf(false) }

    Column(
        Modifier
            .fillMaxSize()
            .verticalScroll(rememberScrollState())
            .padding(16.dp)
    ) {
        TituloPagina("CRIAR", "SIMULADO", "Selecione as disciplinas e a quantidade de questões.")
        Mensagem(controle.erro)

        Caixa {
            Column {
                TituloSecao("Selecione duas ou mais disciplinas")
                if (controle.carregando && controle.disciplinas.isEmpty()) Carregando()
                controle.disciplinas.forEach { d ->
                    val marcada = d.id in controle.selecionadas
                    val semQuestoes = d.qtdQuestoes == 0
                    Row(
                        Modifier
                            .fillMaxWidth()
                            .clip(RoundedCornerShape(10.dp))
                            .toggleable(
                                value = marcada,
                                enabled = !semQuestoes,
                                role = Role.Checkbox,
                                onValueChange = { controle.alternarDisciplina(d.id) }
                            )
                            .padding(vertical = 4.dp),
                        verticalAlignment = Alignment.CenterVertically
                    ) {
                        // O Row inteiro é o alvo do toque (acessível); a caixa só mostra o estado
                        Checkbox(checked = marcada, onCheckedChange = null, enabled = !semQuestoes)
                        Column(Modifier.weight(1f).padding(start = 12.dp)) {
                            Text(d.nome, color = if (semQuestoes) Cores.TextoFraco else Cores.Texto)
                            Text("${d.qtdQuestoes} questões", fontSize = 12.sp, color = Cores.TextoFraco)
                        }
                    }
                }
            }
        }

        Caixa {
            Column {
                TituloSecao("Quantidade de questões")
                Row(Modifier.selectableGroup(), horizontalArrangement = Arrangement.spacedBy(10.dp)) {
                    ControleSimulado.QUANTIDADES.forEach { q ->
                        Row(
                            Modifier
                                .weight(1f)
                                .clip(RoundedCornerShape(10.dp))
                                .background(Cores.SuperficieSuave)
                                .selectable(
                                    selected = controle.quantidade == q,
                                    role = Role.RadioButton,
                                    onClick = { controle.escolherQuantidade(q) }
                                )
                                .padding(8.dp),
                            verticalAlignment = Alignment.CenterVertically
                        ) {
                            RadioButton(selected = controle.quantidade == q, onClick = null)
                            Text("$q", fontWeight = FontWeight.Bold, modifier = Modifier.padding(start = 8.dp))
                        }
                    }
                }
            }
        }

        val qtd = controle.quantidade
        val aviso = when {
            controle.selecionadas.size < ControleSimulado.MINIMO_DISCIPLINAS ->
                "Selecione pelo menos duas disciplinas (${controle.selecionadas.size} selecionada(s))."
            qtd == null -> "Escolha a quantidade de questões."
            controle.disponiveis < qtd ->
                "As disciplinas escolhidas têm ${controle.disponiveis} questões. Escolha mais disciplinas ou menos questões."
            else -> "Pronto: $qtd questões sorteadas entre ${controle.selecionadas.size} disciplinas."
        }
        Mensagem(aviso, if (controle.podeIniciar) TipoMensagem.SUCESSO else TipoMensagem.INFO)
        Text(
            "As questões são sorteadas livremente entre as disciplinas escolhidas, sem divisão fixa.",
            fontSize = 12.sp,
            color = Cores.TextoFraco,
            modifier = Modifier.padding(bottom = 12.dp)
        )
        BotaoPrincipal(
            "Próximo",
            { mostrarInstrucoes = true },
            habilitado = controle.podeIniciar,
            carregando = controle.carregando && controle.disciplinas.isNotEmpty()
        )
    }

    if (mostrarInstrucoes) {
        AlertDialog(
            onDismissRequest = { mostrarInstrucoes = false },
            title = { Text("Instruções do simulado") },
            text = {
                Text(
                    "• Tentativa única: sair da tela do simulado ENCERRA a tentativa, e não é possível retomá-la.\n" +
                        "• Você pode voltar e avançar entre as questões e mudar as respostas até finalizar.\n" +
                        "• Questões em branco contam como erro.\n" +
                        "• O tempo é exibido, sem limite, e não é registrado.\n" +
                        "• Cada acerto vale 10 pontos."
                )
            },
            confirmButton = {
                TextButton(onClick = {
                    mostrarInstrucoes = false
                    controle.iniciarSimulado(aoIniciar)
                }) { Text("Iniciar simulado") }
            },
            dismissButton = { TextButton(onClick = { mostrarInstrucoes = false }) { Text("Voltar") } }
        )
    }
}
