package br.unip.avaliamais.dados

import kotlinx.serialization.Serializable

// Formatos de entrada e saída da API (mesmos campos da API usada pela web).
// Campos que a API pode omitir têm valor padrão.

// ------------------------------------------------------------------ autenticação e senha
@Serializable
data class LoginCorpo(val email: String, val senha: String)

@Serializable
data class UsuarioLogado(
    val id: Int,
    val nome: String,
    val email: String,
    val idPerfil: Int,
    val perfil: String
)

@Serializable
data class LoginResposta(
    val token: String,
    val usuario: UsuarioLogado,
    val senhaInicial: Boolean = false
)

@Serializable
data class Mensagem(val mensagem: String = "")

@Serializable
data class RecuperarSenhaCorpo(val email: String)

@Serializable
data class RecuperarSenhaResposta(val mensagem: String = "", val validadeMinutos: Int = 15)

@Serializable
data class NovaSenhaCorpo(val email: String, val codigo: String, val novaSenha: String, val confirmacao: String)

@Serializable
data class AlterarSenhaCorpo(val senhaAtual: String, val novaSenha: String, val confirmacao: String)

// ------------------------------------------------------------------ perfil e gamificação
@Serializable
data class Medalha(
    val id: Int,
    val nome: String,
    val pontosMinimos: Int,
    val imagemUrl: String? = null,
    val faltam: Int? = null
)

@Serializable
data class Gamificacao(
    val medalha: Medalha? = null,
    val semMedalha: Boolean = true,
    val proxima: Medalha? = null,
    val nivelMaximo: Boolean = false,
    val mensagem: String = ""
)

@Serializable
data class Perfil(
    val id: Int,
    val nome: String,
    val email: String,
    val idPerfil: Int,
    val perfil: String,
    val dataIngresso: String? = null,
    val turma: String? = null,
    val pontuacaoTotal: Int = 0,
    val gamificacao: Gamificacao? = null
)

// ------------------------------------------------------------------ simulado
@Serializable
data class DisciplinaAtiva(
    val id: Int,
    val nome: String,
    val descricao: String? = null,
    val qtdQuestoes: Int = 0
)

@Serializable
data class GerarSimuladoCorpo(val disciplinas: List<Int>, val quantidade: Int)

@Serializable
data class SimuladoCriado(val id: Int, val titulo: String, val quantidade: Int)

@Serializable
data class Alternativa(val letra: String, val texto: String)

/** Questão para a realização: a API não envia gabarito nem explicação. */
@Serializable
data class QuestaoRealizacao(
    val ordem: Int,
    val id: Int,
    val enunciado: String,
    val imagemUrl: String? = null,
    val disciplina: String,
    val alternativas: List<Alternativa>
)

@Serializable
data class SimuladoRealizacao(
    val id: Int,
    val titulo: String,
    val quantidade: Int,
    val finalizado: Boolean = false,
    val questoes: List<QuestaoRealizacao> = emptyList()
)

@Serializable
data class RespostaEnvio(val idQuestao: Int, val alternativa: String?)

@Serializable
data class FinalizarCorpo(val respostas: List<RespostaEnvio>)

@Serializable
data class ResultadoDisciplina(
    val idDisciplina: Int,
    val disciplina: String,
    val totalQuestoes: Int,
    val totalAcertos: Int,
    val percentual: Double
)

@Serializable
data class Resultado(
    val idSimulado: Int,
    val titulo: String,
    val totalQuestoes: Int,
    val totalAcertos: Int,
    val emBranco: Int,
    val totalErros: Int,
    val percentual: Double,
    val pontosObtidos: Int,
    val pontuacaoTotal: Int,
    val dataRealizacao: String? = null,
    val porDisciplina: List<ResultadoDisciplina> = emptyList(),
    val jaFinalizado: Boolean = false
)

@Serializable
data class QuestaoRevisao(
    val ordem: Int,
    val id: Int,
    val enunciado: String,
    val imagemUrl: String? = null,
    val disciplina: String,
    val alternativas: List<Alternativa>,
    val marcada: String? = null,
    val emBranco: Boolean = false,
    val correta: String? = null,
    val acertou: Boolean = false,
    val explicacao: String? = null
)

@Serializable
data class Revisao(
    val id: Int,
    val titulo: String,
    val dataRealizacao: String? = null,
    val totalQuestoes: Int,
    val totalAcertos: Int,
    val percentual: Double,
    val questoes: List<QuestaoRevisao>
)

@Serializable
data class ItemHistorico(
    val id: Int,
    val titulo: String,
    val totalQuestoes: Int,
    val totalAcertos: Int,
    val percentual: Double,
    val pontosObtidos: Int,
    val dataRealizacao: String? = null
)

@Serializable
data class Historico(
    val pagina: Int,
    val tamanho: Int,
    val total: Int,
    val totalPaginas: Int,
    val itens: List<ItemHistorico>
)

// ------------------------------------------------------------------ desempenho
@Serializable
data class DesempenhoDisciplina(
    val idDisciplina: Int,
    val disciplina: String,
    val totalRespostas: Int,
    val totalAcertos: Int,
    val percentual: Double? = null
)

@Serializable
data class DesempenhoAluno(
    val qtdSimulados: Int = 0,
    val mediaAcertos: Double? = null,
    val ultimoSimulado: String? = null,
    val porDisciplina: List<DesempenhoDisciplina> = emptyList()
)

// ------------------------------------------------------------------ erro padrão da API
@Serializable
data class CorpoErro(val erro: DetalheErro? = null)

@Serializable
data class DetalheErro(val mensagem: String = "", val campos: Map<String, String>? = null)
