package br.unip.avaliamais.controles

import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import br.unip.avaliamais.dados.NovaSenhaCorpo
import br.unip.avaliamais.dados.Repositorio
import br.unip.avaliamais.dados.Resposta
import kotlinx.coroutines.launch

/** Etapas da recuperação de senha (RN03): e-mail → código → nova senha. */
enum class EtapaRecuperacao { EMAIL, CODIGO, NOVA_SENHA }

/**
 * Controle ControleAutenticarUsuario (diagrama UML): autenticarUsuario e sairSistema,
 * mais a recuperação de senha por código (RN03) e o endereço da API.
 */
class ControleAutenticarUsuario(private val repositorio: Repositorio) : ViewModel() {

    var email by mutableStateOf("")
    var senha by mutableStateOf("")
    var carregando by mutableStateOf(false)
        private set
    var erro by mutableStateOf<String?>(null)
        private set
    var informacao by mutableStateOf<String?>(null)
        private set

    // Recuperação de senha
    var etapa by mutableStateOf(EtapaRecuperacao.EMAIL)
        private set
    var codigo by mutableStateOf("")
    var novaSenha by mutableStateOf("")
    var confirmacao by mutableStateOf("")
    var errosCampos by mutableStateOf<Map<String, String>>(emptyMap())
        private set

    val urlApi get() = repositorio.sessao.urlApi.value

    /**
     * autenticarUsuario(email, senha). O perfil vem da resposta da API, nunca do aplicativo.
     * A mensagem de falha é sempre a mesma da API ("E-mail ou senha inválidos.").
     */
    fun autenticarUsuario(aoEntrar: (idPerfil: Int) -> Unit) {
        if (email.isBlank() || senha.isEmpty()) {
            erro = "Informe o e-mail e a senha."
            return
        }
        viewModelScope.launch {
            carregando = true
            erro = null
            when (val r = repositorio.login(email.trim(), senha)) {
                is Resposta.Sucesso -> {
                    repositorio.sessao.salvar(r.valor)
                    senha = ""
                    aoEntrar(r.valor.usuario.idPerfil)
                }
                is Resposta.Falha -> {
                    erro = r.erro.message
                    senha = ""
                }
            }
            carregando = false
        }
    }

    /** sairSistema(): apaga o token do aparelho. */
    fun sairSistema(aoSair: () -> Unit) {
        viewModelScope.launch {
            repositorio.sessao.limpar()
            aoSair()
        }
    }

    fun definirServidor(url: String) {
        viewModelScope.launch {
            repositorio.sessao.definirUrlApi(url)
            informacao = "Servidor da API: ${repositorio.sessao.urlApi.value}"
        }
    }

    fun limparMensagens() {
        erro = null
        informacao = null
        errosCampos = emptyMap()
    }

    // ------------------------------------------------------------------ recuperação de senha (RN03)
    fun recuperarSenha() {
        if (email.isBlank()) {
            errosCampos = mapOf("email" to "Informe o e-mail cadastrado.")
            return
        }
        viewModelScope.launch {
            carregando = true
            limparMensagens()
            when (val r = repositorio.recuperarSenha(email.trim())) {
                is Resposta.Sucesso -> {
                    informacao = "${r.valor.mensagem}\nO código vale por ${r.valor.validadeMinutos} minutos."
                    etapa = EtapaRecuperacao.CODIGO
                }
                is Resposta.Falha -> erro = r.erro.message
            }
            carregando = false
        }
    }

    /** Confere o código ANTES de liberar os campos da nova senha. */
    fun validarCodigo() {
        viewModelScope.launch {
            carregando = true
            limparMensagens()
            when (val r = repositorio.validarCodigo(email.trim(), codigo.trim())) {
                is Resposta.Sucesso -> {
                    informacao = "Código confirmado. Agora crie a nova senha."
                    etapa = EtapaRecuperacao.NOVA_SENHA
                }
                is Resposta.Falha -> erro = r.erro.message
            }
            carregando = false
        }
    }

    fun salvarNovaSenha(aoConcluir: () -> Unit) {
        viewModelScope.launch {
            carregando = true
            limparMensagens()
            when (val r = repositorio.novaSenha(NovaSenhaCorpo(email.trim(), codigo.trim(), novaSenha, confirmacao))) {
                is Resposta.Sucesso -> {
                    reiniciarRecuperacao()
                    informacao = "Senha alterada. Entre com a nova senha."
                    aoConcluir()
                }
                is Resposta.Falha -> {
                    erro = r.erro.message
                    errosCampos = r.erro.campos
                }
            }
            carregando = false
        }
    }

    fun reiniciarRecuperacao() {
        etapa = EtapaRecuperacao.EMAIL
        codigo = ""
        novaSenha = ""
        confirmacao = ""
        errosCampos = emptyMap()
    }

    companion object {
        val Fabrica = fabrica { ControleAutenticarUsuario(it) }
    }
}
