package br.unip.avaliamais.controles

import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.ViewModelProvider.AndroidViewModelFactory.Companion.APPLICATION_KEY
import androidx.lifecycle.viewmodel.CreationExtras
import br.unip.avaliamais.AvaliaApp
import br.unip.avaliamais.dados.Repositorio

/**
 * Cria um controle (ViewModel) entregando a ele o acesso à API.
 * Não usa viewModelFactory { initializer { } }, que exige o tipo do controle conhecido na compilação.
 */
fun <C : ViewModel> fabrica(criar: (Repositorio) -> C): ViewModelProvider.Factory =
    object : ViewModelProvider.Factory {
        @Suppress("UNCHECKED_CAST")
        override fun <T : ViewModel> create(modelClass: Class<T>, extras: CreationExtras): T {
            val aplicacao = extras[APPLICATION_KEY] as AvaliaApp
            return criar(aplicacao.repositorio) as T
        }
    }