package br.unip.avaliamais.controles

import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import br.unip.avaliamais.dados.DesempenhoAluno
import br.unip.avaliamais.dados.Repositorio
import br.unip.avaliamais.dados.Resposta
import kotlinx.coroutines.launch

/** Controle ControleDesempenho (diagrama UML): visualizarDesempenho do próprio aluno (RF06, RN24). */
class ControleDesempenho(private val repositorio: Repositorio) : ViewModel() {

    var desempenho by mutableStateOf<DesempenhoAluno?>(null)
        private set
    var carregando by mutableStateOf(false)
        private set
    var erro by mutableStateOf<String?>(null)
        private set

    fun visualizarDesempenho() {
        viewModelScope.launch {
            carregando = true
            erro = null
            when (val r = repositorio.desempenhoAluno()) {
                is Resposta.Sucesso -> desempenho = r.valor
                is Resposta.Falha -> erro = r.erro.message
            }
            carregando = false
        }
    }

    companion object {
        val Fabrica = fabrica { ControleDesempenho(it) }
    }
}
