package br.unip.avaliamais.controles

import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import br.unip.avaliamais.dados.Perfil
import br.unip.avaliamais.dados.Repositorio
import br.unip.avaliamais.dados.Resposta
import kotlinx.coroutines.launch

/**
 * Controle ControlePontuacao (diagrama UML): visualizarPontuacao. A medalha é APURADA pela API,
 * comparando a pontuação com as faixas da tabela Medalha (RN22). Nada de faixa escrita no aplicativo.
 */
class ControlePontuacao(private val repositorio: Repositorio) : ViewModel() {

    var perfil by mutableStateOf<Perfil?>(null)
        private set
    var carregando by mutableStateOf(false)
        private set
    var erro by mutableStateOf<String?>(null)
        private set

    fun visualizarPontuacao() {
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

    fun urlImagem(url: String?) = repositorio.urlImagem(url)

    companion object {
        val Fabrica = fabrica { ControlePontuacao(it) }
    }
}
