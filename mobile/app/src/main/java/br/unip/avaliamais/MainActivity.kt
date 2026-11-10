package br.unip.avaliamais

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import br.unip.avaliamais.ui.TemaAvalia

/** Única Activity do aplicativo: lê a sessão gravada e abre a navegação. */
class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        val sessao = (application as AvaliaApp).repositorio.sessao
        setContent {
            TemaAvalia {
                var inicial by remember { mutableStateOf<String?>(null) }
                LaunchedEffect(Unit) {
                    sessao.carregar()
                    // O token gravado vale 7 dias; se tiver vencido, a primeira chamada à API leva ao login
                    inicial = rotaInicial(sessao.atual.value?.idPerfil)
                }
                val rota = inicial
                if (rota == null) Abertura() else NavegacaoAvalia(rota)
            }
        }
    }
}
