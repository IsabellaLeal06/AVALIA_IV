@file:Suppress("FunctionName")

package br.unip.avaliamais.telas

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
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
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
import br.unip.avaliamais.dados.QuestaoRevisao
import br.unip.avaliamais.ui.BotaoPrincipal
import br.unip.avaliamais.ui.BotaoSecundario
import br.unip.avaliamais.ui.Caixa
import br.unip.avaliamais.ui.Carregando
import br.unip.avaliamais.ui.Cores
import br.unip.avaliamais.ui.Mensagem
import br.unip.avaliamais.ui.TituloPagina
import br.unip.avaliamais.ui.formatarDataHora
import br.unip.avaliamais.ui.formatarPercentual
import coil.compose.AsyncImage

/**
 * Tela Form_Revisar_Simulados (diagrama UML): revisarSimulado → exibirRevisao,
 * exibirRespostaMarcada e exibirRespostaCorreta, com a explicação quando houver (RF05).
 * Questões em branco aparecem como "Questão deixada em branco".
 */
@OptIn(ExperimentalLayoutApi::class)
@Composable
fun Form_Revisar_Simulados(controle: ControleSimulado, idSimulado: Int) {
    LaunchedEffect(idSimulado) { controle.revisarSimulado(idSimulado) }
    val revisao = controle.revisao

    Column(
        Modifier
            .fillMaxSize()
            .verticalScroll(rememberScrollState())
            .padding(16.dp)
    ) {
        Mensagem(controle.erro)
        if (revisao == null) {
            if (controle.carregando) Carregando()
            return@Column
        }
        TituloPagina(
            "REVISÃO",
            "",
            "Realizado em ${formatarDataHora(revisao.dataRealizacao)} · ${revisao.totalAcertos} de ${revisao.totalQuestoes} " +
                "acertos (${formatarPercentual(revisao.percentual)})"
        )

        FlowRow(
            Modifier.fillMaxWidth().padding(bottom = 12.dp),
            horizontalArrangement = Arrangement.spacedBy(6.dp),
            verticalArrangement = Arrangement.spacedBy(6.dp)
        ) {
            revisao.questoes.forEachIndexed { i, q -> NumeroRevisao(i + 1, q, i == controle.indiceRevisao) { controle.exibirQuestaoRevisao(i) } }
        }

        val q = revisao.questoes[controle.indiceRevisao]
        Caixa {
            Column {
                Text(
                    "QUESTÃO ${controle.indiceRevisao + 1}",
                    fontSize = 24.sp,
                    fontWeight = FontWeight.Bold,
                    color = Color(0xFF4D5BE7),
                    modifier = Modifier.semantics { heading() }
                )
                Text("Questão ${controle.indiceRevisao + 1} de ${revisao.questoes.size} · ${q.disciplina}", fontSize = 13.sp, color = Cores.TextoFraco)
                Text(q.enunciado, fontSize = 16.sp, lineHeight = 24.sp, modifier = Modifier.padding(vertical = 14.dp))
                q.imagemUrl?.let {
                    AsyncImage(
                        model = controle.urlImagem(it),
                        contentDescription = "Imagem da questão ${controle.indiceRevisao + 1}",
                        modifier = Modifier.fillMaxWidth().heightIn(max = 260.dp).padding(bottom = 12.dp)
                    )
                }

                // exibirRespostaMarcada
                when {
                    q.emBranco -> Resposta("QUESTÃO DEIXADA EM BRANCO", "Questão deixada em branco: conta como erro.", Color(0xFFF3F4F6), Cores.TextoFraco)
                    q.acertou -> Resposta("✔ SUA RESPOSTA (CORRETA)", textoAlternativa(q, q.marcada), Cores.SucessoFundo, Cores.Sucesso)
                    else -> Resposta("✖ ALTERNATIVA MARCADA", textoAlternativa(q, q.marcada), Cores.ErroFundo, Cores.Erro)
                }
                // exibirRespostaCorreta
                Resposta("✔ RESPOSTA CORRETA", textoAlternativa(q, q.correta), Cores.SucessoFundo, Cores.Sucesso)
                q.explicacao?.takeIf { it.isNotBlank() }?.let { Resposta("EXPLICAÇÃO", it, Cores.InfoFundo, Cores.Azul) }
            }
        }

        Row(horizontalArrangement = Arrangement.spacedBy(10.dp)) {
            BotaoSecundario(
                "← Anterior",
                { controle.exibirQuestaoRevisao(controle.indiceRevisao - 1) },
                Modifier.weight(1f),
                habilitado = controle.indiceRevisao > 0
            )
            Box(Modifier.weight(1f)) {
                BotaoPrincipal(
                    "Próxima →",
                    { controle.exibirQuestaoRevisao(controle.indiceRevisao + 1) },
                    habilitado = controle.indiceRevisao < revisao.questoes.lastIndex
                )
            }
        }
    }
}

private fun textoAlternativa(q: QuestaoRevisao, letra: String?): String {
    if (letra == null) return "—"
    val alternativa = q.alternativas.firstOrNull { it.letra == letra }
    return if (alternativa != null) "$letra) ${alternativa.texto}" else letra
}

@Composable
private fun Resposta(rotulo: String, texto: String, fundo: Color, cor: Color) {
    Column(
        Modifier
            .fillMaxWidth()
            .padding(top = 10.dp)
            .clip(RoundedCornerShape(12.dp))
            .border(1.dp, Cores.Borda, RoundedCornerShape(12.dp))
            .padding(14.dp)
            .semantics(mergeDescendants = true) {}
    ) {
        Text(
            rotulo,
            fontSize = 12.sp,
            fontWeight = FontWeight.Bold,
            color = cor,
            modifier = Modifier.clip(RoundedCornerShape(50)).background(fundo).padding(horizontal = 10.dp, vertical = 4.dp)
        )
        Text(texto, modifier = Modifier.padding(top = 8.dp))
    }
}

@Composable
private fun NumeroRevisao(numero: Int, q: QuestaoRevisao, atual: Boolean, aoTocar: () -> Unit) {
    val (fundo, cor, situacao) = when {
        q.emBranco -> Triple(Color(0xFFF3F4F6), Cores.TextoFraco, "em branco")
        q.acertou -> Triple(Cores.SucessoFundo, Cores.Sucesso, "acertou")
        else -> Triple(Cores.ErroFundo, Cores.Erro, "errou")
    }
    Box(
        Modifier
            .size(44.dp)
            .clip(RoundedCornerShape(8.dp))
            .background(fundo)
            .border(if (atual) 3.dp else 1.dp, if (atual) Cores.Roxo else cor, RoundedCornerShape(8.dp))
            .clickable(role = Role.Button, onClick = aoTocar)
            .semantics { contentDescription = "Questão $numero, $situacao" },
        contentAlignment = Alignment.Center
    ) {
        Text("$numero", fontWeight = FontWeight.Bold, color = cor)
    }
}
