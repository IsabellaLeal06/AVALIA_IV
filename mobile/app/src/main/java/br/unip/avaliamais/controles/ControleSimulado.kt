package br.unip.avaliamais.controles

import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import br.unip.avaliamais.dados.DisciplinaAtiva
import br.unip.avaliamais.dados.Historico
import br.unip.avaliamais.dados.Repositorio
import br.unip.avaliamais.dados.RespostaEnvio
import br.unip.avaliamais.dados.Resposta
import br.unip.avaliamais.dados.Resultado
import br.unip.avaliamais.dados.Revisao
import br.unip.avaliamais.dados.SimuladoRealizacao
import kotlinx.coroutines.launch

/**
 * Controle ControleSimulado (diagrama UML): iniciarSimulado, exibirQuestao, selecionarResposta,
 * salvarResposta, finalizarSimulado, calcularResultado, consultarResultado, revisarSimulado
 * e consultarHistorico. Sorteio e correção acontecem na API (RNF07).
 */
class ControleSimulado(private val repositorio: Repositorio) : ViewModel() {

    var carregando by mutableStateOf(false)
        private set
    var erro by mutableStateOf<String?>(null)
        private set

    // ------------------------------------------------------------------ montagem (RN13, RN14)
    var disciplinas by mutableStateOf<List<DisciplinaAtiva>>(emptyList())
        private set
    var selecionadas by mutableStateOf<Set<Int>>(emptySet())
        private set
    var quantidade by mutableStateOf<Int?>(null)
        private set

    val disponiveis: Int get() = disciplinas.filter { it.id in selecionadas }.sumOf { it.qtdQuestoes }
    val podeIniciar: Boolean
        get() {
            val qtd = quantidade ?: return false
            return selecionadas.size >= MINIMO_DISCIPLINAS && disponiveis >= qtd
        }

    fun listarDisciplinasAtivas() {
        viewModelScope.launch {
            carregando = true
            erro = null
            when (val r = repositorio.disciplinasAtivas()) {
                is Resposta.Sucesso -> disciplinas = r.valor
                is Resposta.Falha -> erro = r.erro.message
            }
            carregando = false
        }
    }

    fun alternarDisciplina(id: Int) {
        selecionadas = if (id in selecionadas) selecionadas - id else selecionadas + id
    }

    fun escolherQuantidade(valor: Int) {
        quantidade = valor
    }

    /** iniciarSimulado(disciplinas, quantidade): o servidor confere as regras de novo e sorteia. */
    fun iniciarSimulado(aoCriar: (idSimulado: Int) -> Unit) {
        val qtd = quantidade ?: return
        viewModelScope.launch {
            carregando = true
            erro = null
            when (val r = repositorio.gerarSimulado(selecionadas.toList(), qtd)) {
                is Resposta.Sucesso -> aoCriar(r.valor.id)
                is Resposta.Falha -> erro = r.erro.message
            }
            carregando = false
        }
    }

    // ------------------------------------------------------------------ realização (RN16, RN17, RN18)
    var simulado by mutableStateOf<SimuladoRealizacao?>(null)
        private set
    var indice by mutableStateOf(0)
        private set
    /** Marcações provisórias (idQuestao → letra), só na memória até a finalização (RN18). */
    var respostas by mutableStateOf<Map<Int, String>>(emptyMap())
        private set
    var resultado by mutableStateOf<Resultado?>(null)
        private set
    var finalizando by mutableStateOf(false)
        private set

    val emBranco: Int get() = simulado?.questoes?.count { it.id !in respostas } ?: 0

    /** Carrega as questões SEM gabarito. Se já foi finalizado, mostra o resultado (tentativa única). */
    fun consultarSimulado(idSimulado: Int) {
        if (simulado?.id == idSimulado || resultado?.idSimulado == idSimulado) return
        viewModelScope.launch {
            carregando = true
            erro = null
            when (val r = repositorio.consultarSimulado(idSimulado)) {
                is Resposta.Sucesso ->
                    if (r.valor.finalizado) consultarResultado(idSimulado) else simulado = r.valor
                is Resposta.Falha -> erro = r.erro.message
            }
            carregando = false
        }
    }

    /** exibirQuestao(numeroQuestao): navega para qualquer questão, mesmo deixando a atual em branco. */
    fun exibirQuestao(numeroQuestao: Int) {
        val total = simulado?.questoes?.size ?: return
        indice = numeroQuestao.coerceIn(0, total - 1)
    }

    /** selecionarResposta(alternativa) → salvarResposta(idQuestao, alternativa). */
    fun selecionarResposta(alternativa: String) {
        val questao = simulado?.questoes?.getOrNull(indice) ?: return
        salvarResposta(questao.id, alternativa)
    }

    fun salvarResposta(idQuestao: Int, alternativa: String?) {
        respostas = if (alternativa == null) respostas - idQuestao else respostas + (idQuestao to alternativa)
    }

    /** finalizarSimulado(): envia as respostas; a correção é do servidor (calcularResultado). */
    fun finalizarSimulado() {
        val atual = simulado ?: return
        calcularResultado(atual.id)
    }

    /**
     * calcularResultado(idSimulado). Se a rede falhar, as marcações continuam aqui e o aluno pode
     * tentar de novo: a API devolve o resultado já apurado se a primeira tentativa tiver chegado (RN16).
     */
    fun calcularResultado(idSimulado: Int) {
        val atual = simulado ?: return
        if (finalizando) return
        viewModelScope.launch {
            finalizando = true
            erro = null
            val envio = atual.questoes.map { RespostaEnvio(it.id, respostas[it.id]) }
            when (val r = repositorio.finalizarSimulado(idSimulado, envio)) {
                is Resposta.Sucesso -> {
                    resultado = r.valor
                    simulado = null
                    respostas = emptyMap()
                }
                is Resposta.Falha -> erro = r.erro.message
            }
            finalizando = false
        }
    }

    fun consultarResultado(idSimulado: Int) {
        viewModelScope.launch {
            carregando = true
            when (val r = repositorio.resultado(idSimulado)) {
                is Resposta.Sucesso -> {
                    resultado = r.valor
                    simulado = null
                }
                is Resposta.Falha -> erro = r.erro.message
            }
            carregando = false
        }
    }

    // ------------------------------------------------------------------ revisão comentada (RF05)
    var revisao by mutableStateOf<Revisao?>(null)
        private set
    var indiceRevisao by mutableStateOf(0)
        private set

    /** revisarSimulado(idSimulado): marcada, correta e explicação, só depois de finalizado. */
    fun revisarSimulado(idSimulado: Int) {
        viewModelScope.launch {
            carregando = true
            erro = null
            when (val r = repositorio.revisao(idSimulado)) {
                is Resposta.Sucesso -> revisao = r.valor
                is Resposta.Falha -> erro = r.erro.message
            }
            carregando = false
        }
    }

    fun exibirQuestaoRevisao(numero: Int) {
        val total = revisao?.questoes?.size ?: return
        indiceRevisao = numero.coerceIn(0, total - 1)
    }

    // ------------------------------------------------------------------ histórico (RN20)
    var historico by mutableStateOf<Historico?>(null)
        private set

    /** consultarHistorico(): simulados finalizados do aluno do token, paginados. */
    fun consultarHistorico(pagina: Int = 1) {
        viewModelScope.launch {
            carregando = true
            erro = null
            when (val r = repositorio.historico(pagina)) {
                is Resposta.Sucesso -> historico = r.valor
                is Resposta.Falha -> erro = r.erro.message
            }
            carregando = false
        }
    }

    fun urlImagem(url: String?) = repositorio.urlImagem(url)

    companion object {
        const val MINIMO_DISCIPLINAS = 2
        val QUANTIDADES = listOf(10, 15, 20)
        val Fabrica = fabrica { ControleSimulado(it) }
    }
}
