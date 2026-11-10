package br.unip.avaliamais.controles

import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import br.unip.avaliamais.dados.AlterarSenhaCorpo
import br.unip.avaliamais.dados.Perfil
import br.unip.avaliamais.dados.Repositorio
import br.unip.avaliamais.dados.Resposta
import kotlinx.coroutines.launch

/**
 * Controle Visualizar_perfil (diagrama UML, mesmo nome usado na Desktop):
 * visualizarPerfil, alterarSenha (RN04) e sairConta.
 */
@Suppress("ClassName")
class Visualizar_perfil(private val repositorio: Repositorio) : ViewModel() {

    var perfil by mutableStateOf<Perfil?>(null)
        private set
    var carregando by mutableStateOf(false)
        private set
    var erro by mutableStateOf<String?>(null)
        private set
    var sucesso by mutableStateOf<String?>(null)
        private set

    var senhaAtual by mutableStateOf("")
    var novaSenha by mutableStateOf("")
    var confirmacao by mutableStateOf("")
    var errosCampos by mutableStateOf<Map<String, String>>(emptyMap())
        private set

    val recomendarTrocaSenha: Boolean get() = repositorio.sessao.atual.value?.senhaInicial == true

    fun visualizarPerfil() {
        viewModelScope.launch {
            carregando = true
            erro = null
            when (val r = repositorio.perfil()) {
                is Resposta.Sucesso -> perfil = r.valor
                is Resposta.Falha -> erro = r.erro.message
            }
            carregando = false
        }
    }

    /** alterarSenha(senhaAtual, novaSenha, confirmacao): sempre do usuário do token (RN04). */
    fun alterarSenha() {
        viewModelScope.launch {
            carregando = true
            erro = null
            sucesso = null
            errosCampos = emptyMap()
            when (val r = repositorio.alterarSenha(AlterarSenhaCorpo(senhaAtual, novaSenha, confirmacao))) {
                is Resposta.Sucesso -> {
                    repositorio.sessao.senhaTrocada()
                    senhaAtual = ""
                    novaSenha = ""
                    confirmacao = ""
                    sucesso = r.valor.mensagem
                }
                is Resposta.Falha -> {
                    erro = r.erro.message
                    errosCampos = r.erro.campos
                }
            }
            carregando = false
        }
    }

    fun sairConta(aoSair: () -> Unit) {
        viewModelScope.launch {
            repositorio.sessao.limpar()
            aoSair()
        }
    }

    companion object {
        val Fabrica = fabrica { Visualizar_perfil(it) }
    }
}
