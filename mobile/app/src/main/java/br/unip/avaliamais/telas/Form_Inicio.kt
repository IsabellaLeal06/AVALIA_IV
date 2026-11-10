@file:Suppress("FunctionName")

package br.unip.avaliamais.telas

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.TextButton
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import br.unip.avaliamais.controles.ControleDesempenho
import br.unip.avaliamais.controles.ControlePontuacao
import br.unip.avaliamais.ui.BarraDesempenho
import br.unip.avaliamais.ui.BotaoPrincipal
import br.unip.avaliamais.ui.Caixa
import br.unip.avaliamais.ui.CardIndicador
import br.unip.avaliamais.ui.Carregando
import br.unip.avaliamais.ui.Mensagem
import br.unip.avaliamais.ui.TipoMensagem
import br.unip.avaliamais.ui.TituloPagina
import br.unip.avaliamais.ui.TituloSecao
import br.unip.avaliamais.ui.formatarData
import br.unip.avaliamais.ui.formatarNumero
import br.unip.avaliamais.ui.formatarPercentual

/**
 * Início do aluno: resumo do desempenho, pontuação e atalho para criar simulado.
 * Tudo vem da API (nenhum dado fixo na tela).
 */
@Composable
fun Form_Inicio(
    nome: String,
    recomendarTrocaSenha: Boolean,
    desempenho: ControleDesempenho,
    pontuacao: ControlePontuacao,
    aoCriarSimulado: () -> Unit,
    aoTrocarSenha: () -> Unit
) {
    LaunchedEffect(Unit) {
        desempenho.visualizarDesempenho()
        pontuacao.visualizarPontuacao()
    }

    Column(
        Modifier
            .fillMaxSize()
            .verticalScroll(rememberScrollState())
            .padding(16.dp)
    ) {
        TituloPagina("Olá,", nome.substringBefore(' ').uppercase(), "Acompanhe o seu desempenho e a sua evolução.")

        // RN02: a troca da senha inicial é recomendada, não obrigatória
        if (recomendarTrocaSenha) {
            Mensagem("Você ainda usa a senha inicial (o seu CPF). Recomendamos trocá-la.", TipoMensagem.ALERTA)
            TextButton(onClick = aoTrocarSenha) { Text("Trocar senha agora") }
        }

        BotaoPrincipal("Criar simulado", aoCriarSimulado, modifier = Modifier.padding(vertical = 10.dp))

        Mensagem(desempenho.erro ?: pontuacao.erro)
        val d = desempenho.desempenho
        val p = pontuacao.perfil

        Row(horizontalArrangement = Arrangement.spacedBy(12.dp)) {
            CardIndicador("Taxa média", formatarPercentual(d?.mediaAcertos), Modifier.weight(1f))
            CardIndicador("Simulados", formatarNumero(d?.qtdSimulados ?: 0), Modifier.weight(1f))
        }
        Row(horizontalArrangement = Arrangement.spacedBy(12.dp)) {
            CardIndicador("Pontos", formatarNumero(p?.pontuacaoTotal ?: 0), Modifier.weight(1f))
            CardIndicador("Último simulado", formatarData(d?.ultimoSimulado), Modifier.weight(1f))
        }
        p?.gamificacao?.mensagem?.let { Mensagem(it, TipoMensagem.INFO) }

        Caixa {
            Column {
                TituloSecao("Desempenho por Disciplina", "Aproveitamento acumulado em todos os simulados.")
                when {
                    desempenho.carregando && d == null -> Carregando()
                    d == null || d.porDisciplina.isEmpty() ->
                        Text("Você ainda não fez nenhum simulado. Que tal começar agora?")
                    else -> d.porDisciplina.forEach {
                        BarraDesempenho(it.disciplina, it.percentual, "${it.totalAcertos} de ${it.totalRespostas}")
                    }
                }
            }
        }
    }
}
