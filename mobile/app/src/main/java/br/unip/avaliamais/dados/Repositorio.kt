package br.unip.avaliamais.dados

import kotlinx.coroutines.flow.MutableSharedFlow
import kotlinx.coroutines.flow.SharedFlow
import kotlinx.coroutines.flow.asSharedFlow
import kotlinx.serialization.SerializationException
import retrofit2.HttpException
import java.io.IOException

/** Erro mostrado na tela, com a mensagem da API e, quando houver, o campo a corrigir. */
class ErroApi(val status: Int, override val message: String, val campos: Map<String, String> = emptyMap()) : Exception(message)

/** Resultado de uma chamada: sucesso com o valor ou falha com a mensagem. */
sealed interface Resposta<out T> {
    data class Sucesso<T>(val valor: T) : Resposta<T>
    data class Falha(val erro: ErroApi) : Resposta<Nothing>
}

/**
 * Ponto único de acesso à API no aplicativo. Converte falhas de rede e o erro padrão
 * da API ({ "erro": { "mensagem", "campos" } }) em mensagens para a tela.
 */
class Repositorio(val sessao: Sessao, val cliente: ClienteApi) {

    private val _sessaoExpirada = MutableSharedFlow<Unit>(extraBufferCapacity = 1)

    /** Avisa a navegação quando a API recusa o token (prazo vencido ou conta desativada). */
    val sessaoExpirada: SharedFlow<Unit> = _sessaoExpirada.asSharedFlow()

    private suspend fun <T> chamar(protegida: Boolean = true, bloco: suspend ApiAvalia.() -> T): Resposta<T> =
        try {
            Resposta.Sucesso(cliente.api().bloco())
        } catch (e: HttpException) {
            val corpo = runCatching { e.response()?.errorBody()?.string() }.getOrNull()
            val detalhe = corpo?.let { runCatching { cliente.json.decodeFromString(CorpoErro.serializer(), it).erro }.getOrNull() }
            if (e.code() == 401 && protegida) {
                sessao.limpar()
                _sessaoExpirada.tryEmit(Unit)
            }
            Resposta.Falha(
                ErroApi(
                    e.code(),
                    detalhe?.mensagem?.takeIf { it.isNotBlank() } ?: "Não foi possível concluir a operação (erro ${e.code()}).",
                    detalhe?.campos.orEmpty()
                )
            )
        } catch (e: IOException) {
            Resposta.Falha(
                ErroApi(
                    0,
                    "Não foi possível falar com o servidor. Confira a internet, se a API está rodando " +
                        "e o endereço em \"Servidor da API\" (${sessao.urlApi.value})."
                )
            )
        } catch (e: SerializationException) {
            Resposta.Falha(ErroApi(0, "A API respondeu num formato inesperado. Confira se o endereço aponta para a API do Avalia+."))
        } catch (e: IllegalArgumentException) {
            Resposta.Falha(ErroApi(0, "Endereço da API inválido: ${sessao.urlApi.value}"))
        }

    // ------------------------------------------------------------------ autenticação e senha
    suspend fun login(email: String, senha: String) = chamar(protegida = false) { login(LoginCorpo(email, senha)) }
    suspend fun recuperarSenha(email: String) = chamar(protegida = false) { recuperarSenha(RecuperarSenhaCorpo(email)) }
    suspend fun validarCodigo(email: String, codigo: String) = chamar(protegida = false) { validarCodigo(email, codigo) }
    suspend fun novaSenha(corpo: NovaSenhaCorpo) = chamar(protegida = false) { novaSenha(corpo) }
    suspend fun alterarSenha(corpo: AlterarSenhaCorpo) = chamar { alterarSenha(corpo) }

    // ------------------------------------------------------------------ aluno
    suspend fun perfil() = chamar { perfil() }
    suspend fun disciplinasAtivas() = chamar { disciplinasAtivas() }
    suspend fun gerarSimulado(disciplinas: List<Int>, quantidade: Int) = chamar { gerarSimulado(GerarSimuladoCorpo(disciplinas, quantidade)) }
    suspend fun consultarSimulado(id: Int) = chamar { consultarSimulado(id) }
    suspend fun finalizarSimulado(id: Int, respostas: List<RespostaEnvio>) = chamar { finalizarSimulado(id, FinalizarCorpo(respostas)) }
    suspend fun resultado(id: Int) = chamar { resultado(id) }
    suspend fun revisao(id: Int) = chamar { revisao(id) }
    suspend fun historico(pagina: Int, tamanho: Int = 10) = chamar { historico(pagina, tamanho) }
    suspend fun desempenhoAluno() = chamar { desempenhoAluno() }

    fun urlImagem(url: String?) = cliente.urlImagem(url)
}
