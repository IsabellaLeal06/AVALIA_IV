package br.unip.avaliamais.dados

import android.content.Context
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.core.booleanPreferencesKey
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.core.intPreferencesKey
import androidx.datastore.preferences.core.stringPreferencesKey
import androidx.datastore.preferences.preferencesDataStore
import br.unip.avaliamais.BuildConfig
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.first

private val Context.armazenamento by preferencesDataStore(name = "sessao")

/** Dados do usuário logado guardados no aparelho. */
data class DadosSessao(
    val token: String,
    val idUsuario: Int,
    val nome: String,
    val email: String,
    val idPerfil: Int,
    val perfil: String,
    val senhaInicial: Boolean
)

/**
 * Sessão do aplicativo: token, usuário e endereço da API.
 * O token vale 7 dias (prazo da API): o aluno não precisa entrar de novo a cada abertura.
 * Uma cópia fica em memória para o interceptador do OkHttp ler o token sem acessar o disco.
 */
class Sessao(private val context: Context) {

    private object Chaves {
        val TOKEN = stringPreferencesKey("token")
        val ID = intPreferencesKey("id")
        val NOME = stringPreferencesKey("nome")
        val EMAIL = stringPreferencesKey("email")
        val ID_PERFIL = intPreferencesKey("id_perfil")
        val PERFIL = stringPreferencesKey("perfil")
        val SENHA_INICIAL = booleanPreferencesKey("senha_inicial")
        val URL_API = stringPreferencesKey("url_api")
    }

    private val _atual = MutableStateFlow<DadosSessao?>(null)
    val atual: StateFlow<DadosSessao?> = _atual.asStateFlow()

    private val _urlApi = MutableStateFlow(BuildConfig.API_URL_PADRAO)
    val urlApi: StateFlow<String> = _urlApi.asStateFlow()

    val token: String? get() = _atual.value?.token

    /** Lê o que ficou gravado (chamado uma vez, na abertura do aplicativo). */
    suspend fun carregar() {
        val p = context.armazenamento.data.first()
        _urlApi.value = p[Chaves.URL_API] ?: BuildConfig.API_URL_PADRAO
        _atual.value = lerSessao(p)
    }

    private fun lerSessao(p: Preferences): DadosSessao? {
        val token = p[Chaves.TOKEN] ?: return null
        return DadosSessao(
            token = token,
            idUsuario = p[Chaves.ID] ?: 0,
            nome = p[Chaves.NOME].orEmpty(),
            email = p[Chaves.EMAIL].orEmpty(),
            idPerfil = p[Chaves.ID_PERFIL] ?: 0,
            perfil = p[Chaves.PERFIL].orEmpty(),
            senhaInicial = p[Chaves.SENHA_INICIAL] ?: false
        )
    }

    suspend fun salvar(resposta: LoginResposta) {
        context.armazenamento.edit { p ->
            p[Chaves.TOKEN] = resposta.token
            p[Chaves.ID] = resposta.usuario.id
            p[Chaves.NOME] = resposta.usuario.nome
            p[Chaves.EMAIL] = resposta.usuario.email
            p[Chaves.ID_PERFIL] = resposta.usuario.idPerfil
            p[Chaves.PERFIL] = resposta.usuario.perfil
            p[Chaves.SENHA_INICIAL] = resposta.senhaInicial
        }
        _atual.value = lerSessao(context.armazenamento.data.first())
    }

    suspend fun senhaTrocada() {
        context.armazenamento.edit { it[Chaves.SENHA_INICIAL] = false }
        _atual.value = _atual.value?.copy(senhaInicial = false)
    }

    suspend fun limpar() {
        // O endereço da API continua gravado
        context.armazenamento.edit { p ->
            p.remove(Chaves.TOKEN)
            p.remove(Chaves.ID)
            p.remove(Chaves.NOME)
            p.remove(Chaves.EMAIL)
            p.remove(Chaves.ID_PERFIL)
            p.remove(Chaves.PERFIL)
            p.remove(Chaves.SENHA_INICIAL)
        }
        _atual.value = null
    }

    /** Endereço da API (emulador: http://10.0.2.2:3000/ ; celular: http://IP-do-computador:3000/). */
    suspend fun definirUrlApi(url: String) {
        val normalizada = normalizarUrl(url)
        context.armazenamento.edit { it[Chaves.URL_API] = normalizada }
        _urlApi.value = normalizada
    }

    companion object {
        fun normalizarUrl(url: String): String {
            var u = url.trim()
            if (u.isEmpty()) return BuildConfig.API_URL_PADRAO
            if (!u.startsWith("http://") && !u.startsWith("https://")) u = "http://$u"
            return if (u.endsWith("/")) u else "$u/"
        }
    }
}
