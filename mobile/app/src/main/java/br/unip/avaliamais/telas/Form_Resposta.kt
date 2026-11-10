@file:Suppress("FunctionName")

package br.unip.avaliamais.telas

import androidx.activity.compose.BackHandler
import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.ExperimentalLayoutApi
import androidx.compose.foundation.layout.FlowRow
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.heightIn
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.selection.selectable
import androidx.compose.foundation.selection.selectableGroup
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.AlertDialog
import androidx.compose.material3.LinearProgressIndicator
import androidx.compose.material3.Text
import androidx.compose.material3.TextButton
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableLongStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.semantics.Role
import androidx.compose.ui.semantics.contentDescription
import androidx.compose.ui.semantics.heading
import androidx.compose.ui.semantics.semantics
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import br.unip.avaliamais.controles.ControleSimulado
import br.unip.avaliamais.dados.Resultado
import br.unip.avaliamais.ui.BarraDesempenho
import br.unip.avaliamais.ui.BotaoPrincipal
import br.unip.avaliamais.ui.BotaoSecundario
import br.unip.avaliamais.ui.Caixa
import br.unip.avaliamais.ui.CardIndicador
import br.unip.avaliamais.ui.Carregando
import br.unip.avaliamais.ui.Cores
import br.unip.avaliamais.ui.Mensagem
import br.unip.avaliamais.ui.TituloSecao
import br.unip.avaliamais.ui.formatarDataHora
import br.unip.avaliamais.ui.formatarNumero
import br.unip.avaliamais.ui.formatarPercentual
import coil.compose.AsyncImage
import kotlinx.coroutines.delay

/**
 * Tela Form_Resposta (diagrama UML): exibirQuestao, selecionarResposta, finalizarSimulado
 * e, depois da correção (calcularResultado), exibirResultado.
 *  - Questões SEM gabarito; "Anterior", "Próximo" e navegação numerada; avanço em branco (RN18).
 *  - Cronômetro só exibido: o tempo não é enviado (RN17).
 *  - Voltar/sair durante a prova pede confirmação e ENCERRA a tentativa (RN16).
 */
@Composable
fun Form_Resposta(
    controle: ControleSimulado,
    idSimulado: Int,
    aoInicio: () -> Unit,
    aoRevisar: (Int) -> Unit,
    aoNovoSimulado: () -> Unit
) {
    LaunchedEffect(idSimulado) { controle.consultarSimulado(idSimulado) }
    var confirmarFinalizacao by remember { mutableStateOf(false) }
    var confirmarSaida by remember { mutableStateOf(false) }

    val resultado = controle.resultado
    val simulado = controle.simulado

    // RN16: o botão Voltar do celular não sai sem confirmação
    BackHandler(enabled = simulado != null) { confirmarSaida = true }

    when {
        resultado != null -> ExibirResultado(resultado, aoInicio, { aoRevisar(resultado.idSimulado) }, aoNovoSimulado)
        simulado == null -> Column(Modifier.padding(16.dp)) {
            Mensagem(controle.erro)
            if (controle.carregando || controle.erro == null) Carregando("Carregando o simulado...")
            else BotaoSecundario("Voltar ao início", aoInicio, Modifier.fillMaxWidth())
        }
        else -> ExibirQuestao(controle, aoPedirFinalizacao = { confirmarFinalizacao = true }, aoPedirSaida = { confirmarSaida = true })
    }

    if (confirmarFinalizacao) {
        val brancas = controle.emBranco
        AlertDialog(
            onDismissRequest = { confirmarFinalizacao = false },
            title = { Text("Finalizar simulado") },
            text = {
                Text(
                    if (brancas > 0) "Você deixou $brancas questão(ões) em branco. Questões em branco contam como erro.\n\nDeseja finalizar?"
                    else "Todas as questões foram respondidas. Deseja finalizar?"
                )
            },
            confirmButton = { TextButton(onClick = { confirmarFinalizacao = false; controle.finalizarSimulado() }) { Text("Finalizar") } },
            dismissButton = { TextButton(onClick = { confirmarFinalizacao = false }) { Text("Continuar respondendo") } }
        )
    }

    if (confirmarSaida) {
        AlertDialog(
            onDismissRequest = { confirmarSaida = false },
            title = { Text("Sair do simulado") },
            text = {
                Text(
                    "Sair desta tela ENCERRA a tentativa: o simulado será finalizado com as respostas marcadas até agora, " +
                        "e não será possível retomá-lo."
                )
            },
            confirmButton = {
                TextButton(onClick = { confirmarSaida = false; controle.finalizarSimulado() }) { Text("Sair e finalizar", color = Cores.Erro) }
            },
            dismissButton = { TextButton(onClick = { confirmarSaida = false }) { Text("Continuar") } }
        )
    }
}

@OptIn(ExperimentalLayoutApi::class)
@Composable
private fun ExibirQuestao(controle: ControleSimulado, aoPedirFinalizacao: () -> Unit, aoPedirSaida: () -> Unit) {
    val simulado = controle.simulado ?: return
    val questoes = simulado.questoes
    val questao = questoes[controle.indice]
    val respondidas = questoes.size - controle.emBranco
    val ultima = controle.indice == questoes.lastIndex
    val rolagem = rememberScrollState()

    // Ao trocar de questão, volta para o topo
    LaunchedEffect(controle.indice) { rolagem.scrollTo(0) }

    Column(
        Modifier
            .fillMaxSize()
            .verticalScroll(rolagem)
            .padding(16.dp)
    ) {
        Row(Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween, verticalAlignment = Alignment.CenterVertically) {
            Cronometro()
            TextButton(onClick = aoPedirSaida) { Text("Sair", color = Cores.Erro) }
        }
        Text("$respondidas de ${questoes.size} respondidas", fontSize = 13.sp, color = Cores.TextoFraco)
        LinearProgressIndicator(
            progress = { respondidas.toFloat() / questoes.size },
            modifier = Modifier.fillMaxWidth().padding(vertical = 8.dp).clip(RoundedCornerShape(50)),
            color = Cores.Azul,
            trackColor = Color(0xFFECEEF3)
        )

        // Navegação numerada: azul = respondida, branco = em branco
        FlowRow(
            Modifier.fillMaxWidth().padding(bottom = 10.dp),
            horizontalArrangement = Arrangement.spacedBy(6.dp),
            verticalArrangement = Arrangement.spacedBy(6.dp)
        ) {
            questoes.forEachIndexed { i, q ->
                val marcada = controle.respostas[q.id]
                NumeroQuestao(
                    numero = i + 1,
                    respondida = marcada != null,
                    atual = i == controle.indice,
                    descricao = "Questão ${i + 1}, ${if (marcada != null) "marcada $marcada" else "em branco"}"
                ) { controle.exibirQuestao(i) }
            }
        }

        Caixa {
            Column {
                Text(
                    "QUESTÃO ${controle.indice + 1}",
                    fontSize = 24.sp,
                    fontWeight = FontWeight.Bold,
                    color = Color(0xFF4D5BE7),
                    modifier = Modifier.semantics { heading() }
                )
                Text("Questão ${controle.indice + 1} de ${questoes.size} · ${questao.disciplina}", fontSize = 13.sp, color = Cores.TextoFraco)
                Text(questao.enunciado, fontSize = 16.sp, lineHeight = 24.sp, modifier = Modifier.padding(vertical = 14.dp))
                questao.imagemUrl?.let {
                    AsyncImage(
                        model = controle.urlImagem(it),
                        contentDescription = "Imagem da questão ${controle.indice + 1}",
                        modifier = Modifier.fillMaxWidth().heightIn(max = 260.dp).padding(bottom = 12.dp)
                    )
                }

                Column(Modifier.selectableGroup(), verticalArrangement = Arrangement.spacedBy(10.dp)) {
                    questao.alternativas.forEach { a ->
                        val selecionada = controle.respostas[questao.id] == a.letra
                        Row(
                            Modifier
                                .fillMaxWidth()
                                .clip(RoundedCornerShape(12.dp))
                                .background(if (selecionada) Cores.InfoFundo else Cores.SuperficieSuave)
                                .border(1.dp, if (selecionada) Cores.Azul else Cores.Borda, RoundedCornerShape(12.dp))
                                .selectable(selected = selecionada, role = Role.RadioButton) { controle.selecionarResposta(a.letra) }
                                .padding(12.dp),
                            verticalAlignment = Alignment.Top
                        ) {
                            Box(
                                Modifier
                                    .size(30.dp)
                                    .clip(CircleShape)
                                    .background(if (selecionada) Cores.Azul else Cores.Superficie)
                                    .border(1.dp, Cores.Borda, CircleShape),
                                contentAlignment = Alignment.Center
                            ) {
                                Text(a.letra, fontWeight = FontWeight.Bold, color = if (selecionada) Color.White else Cores.Texto)
                            }
                            Text(a.texto, modifier = Modifier.padding(start = 12.dp, top = 4.dp))
                        }
                    }
                }
            }
        }

        Mensagem(controle.erro)
        Row(horizontalArrangement = Arrangement.spacedBy(10.dp)) {
            BotaoSecundario("← Anterior", { controle.exibirQuestao(controle.indice - 1) }, Modifier.weight(1f), habilitado = controle.indice > 0)
            BotaoSecundario(
                "Deixar em branco",
                { controle.salvarResposta(questao.id, null) },
                Modifier.weight(1f),
                habilitado = controle.respostas[questao.id] != null
            )
        }
        if (ultima) {
            BotaoPrincipal("Finalizar simulado", aoPedirFinalizacao, Modifier.padding(top = 10.dp), carregando = controle.finalizando)
        } else {
            BotaoPrincipal("Próximo →", { controle.exibirQuestao(controle.indice + 1) }, Modifier.padding(top = 10.dp))
        }
    }
}

@Composable
private fun NumeroQuestao(numero: Int, respondida: Boolean, atual: Boolean, descricao: String, aoTocar: () -> Unit) {
    Box(
        Modifier
            .size(44.dp)
            .clip(RoundedCornerShape(8.dp))
            .background(if (respondida) Cores.InfoFundo else Cores.Superficie)
            .border(if (atual) 3.dp else 1.dp, if (atual) Cores.Roxo else if (respondida) Cores.Azul else Cores.Borda, RoundedCornerShape(8.dp))
            .clickable(role = Role.Button, onClick = aoTocar)
            .semantics { contentDescription = descricao },
        contentAlignment = Alignment.Center
    ) {
        Text("$numero", fontWeight = FontWeight.Bold, color = if (respondida) Cores.Azul else Cores.Texto)
    }
}

/** Tempo decorrido, só na tela (RN17: não é registrado). */
@Composable
private fun Cronometro() {
    var segundos by remember { mutableLongStateOf(0L) }
    LaunchedEffect(Unit) {
        while (true) {
            delay(1000)
            segundos++
        }
    }
    val texto = "%02d:%02d:%02d".format(segundos / 3600, (segundos % 3600) / 60, segundos % 60)
    Text(
        texto,
        fontSize = 22.sp,
        fontWeight = FontWeight.Bold,
        color = Cores.Azul,
        modifier = Modifier.semantics { contentDescription = "Tempo decorrido $texto (não é registrado)" }
    )
}

/**
 * exibirResultado(): apurado pelo servidor. Sem tempo (RN17). Saudação conforme o aproveitamento,
 * com tom encorajador nas faixas baixas.
 */
@Composable
private fun ExibirResultado(r: Resultado, aoInicio: () -> Unit, aoRevisar: () -> Unit, aoNovoSimulado: () -> Unit) {
    val (titulo, apoio) = when {
        r.percentual >= 80 -> "EXCELENTE!" to "Resultado de destaque. Continue assim!"
        r.percentual >= 60 -> "PARABÉNS!" to "Bom resultado. Revise as questões que errou para ir ainda mais longe."
        r.percentual >= 40 -> "BOM ESFORÇO!" to "Você está no caminho. A revisão comentada ajuda a fixar o conteúdo."
        else -> "CONTINUE PRATICANDO!" to "Cada simulado é um passo. Revise as questões e tente de novo."
    }
    Column(
        Modifier
            .fillMaxSize()
            .verticalScroll(rememberScrollState())
            .padding(16.dp)
    ) {
        Text(titulo, fontSize = 26.sp, fontWeight = FontWeight.Bold, color = Cores.Roxo, modifier = Modifier.semantics { heading() })
        Text(apoio, color = Cores.TextoSuave, modifier = Modifier.padding(top = 4.dp))
        Text("Realizado em ${formatarDataHora(r.dataRealizacao)}", fontSize = 13.sp, color = Cores.TextoFraco, modifier = Modifier.padding(vertical = 10.dp))

        CardIndicador("Taxa de acerto", formatarPercentual(r.percentual), Modifier.fillMaxWidth())
        Row(horizontalArrangement = Arrangement.spacedBy(12.dp)) {
            CardIndicador("Acertos", "${r.totalAcertos}/${r.totalQuestoes}", Modifier.weight(1f), "${r.totalErros} erro(s), ${r.emBranco} em branco")
            CardIndicador("Pontos", "+${r.pontosObtidos}", Modifier.weight(1f), "Total: ${formatarNumero(r.pontuacaoTotal)}")
        }

        Caixa {
            Column {
                TituloSecao("Desempenho por Disciplina", "Como você foi em cada disciplina deste simulado.")
                r.porDisciplina.forEach { BarraDesempenho(it.disciplina, it.percentual, "${it.totalAcertos} de ${it.totalQuestoes}") }
            }
        }

        BotaoPrincipal("Revisar questões", aoRevisar)
        Row(Modifier.padding(top = 10.dp), horizontalArrangement = Arrangement.spacedBy(10.dp)) {
            BotaoSecundario("Início", aoInicio, Modifier.weight(1f))
            BotaoSecundario("Novo simulado", aoNovoSimulado, Modifier.weight(1f))
        }
    }
}
