@file:Suppress("FunctionName")

package br.unip.avaliamais.telas

import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.imePadding
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.AlertDialog
import androidx.compose.material3.Text
import androidx.compose.material3.TextButton
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.semantics.semantics
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import br.unip.avaliamais.controles.Visualizar_perfil
import br.unip.avaliamais.ui.Avatar
import br.unip.avaliamais.ui.BotaoPrincipal
import br.unip.avaliamais.ui.BotaoSecundario
import br.unip.avaliamais.ui.Caixa
import br.unip.avaliamais.ui.CampoTexto
import br.unip.avaliamais.ui.Carregando
import br.unip.avaliamais.ui.Cores
import br.unip.avaliamais.ui.Mensagem
import br.unip.avaliamais.ui.TipoMensagem
import br.unip.avaliamais.ui.TituloPagina
import br.unip.avaliamais.ui.TituloSecao
import br.unip.avaliamais.ui.formatarData

/**
 * Tela Form_Visualizar_perfil (diagrama UML): visualizarPerfil, exibirAvatar, alterarSenha e sairConta.
 * Os dados cadastrais são só para leitura: quem altera é o Administrador (RN04).
 */
@Composable
fun Form_Visualizar_perfil(controle: Visualizar_perfil, aoSair: () -> Unit) {
    LaunchedEffect(Unit) { controle.visualizarPerfil() }
    var confirmarSaida by remember { mutableStateOf(false) }
    val perfil = controle.perfil

    Column(
        Modifier
            .fillMaxSize()
            .verticalScroll(rememberScrollState())
            .imePadding()
            .padding(16.dp)
    ) {
        TituloPagina("MEU", "PERFIL", "As suas informações.")
        if (controle.carregando && perfil == null) Carregando()

        if (perfil != null) {
            Caixa {
                Column(Modifier.fillMaxWidth(), horizontalAlignment = Alignment.CenterHorizontally) {
                    Avatar(perfil.nome)
                    Spacer(Modifier.height(16.dp))
                    Column(Modifier.fillMaxWidth()) {
                        Dado("Nome completo", perfil.nome)
                        Dado("E-mail", perfil.email)
                        Dado("Tipo de usuário", perfil.perfil)
                        Dado("Data de ingresso", formatarData(perfil.dataIngresso))
                        perfil.turma?.let { Dado("Turma", it) }
                    }
                }
            }
        }

        Caixa {
            Column {
                TituloSecao("Alterar senha", "Mínimo de 8 caracteres, com pelo menos uma letra e um número.")
                if (controle.recomendarTrocaSenha) {
                    Mensagem("Você ainda usa a senha inicial (o seu CPF). Recomendamos trocá-la.", TipoMensagem.ALERTA)
                }
                Mensagem(controle.erro)
                Mensagem(controle.sucesso, TipoMensagem.SUCESSO)
                CampoTexto(controle.senhaAtual, { controle.senhaAtual = it }, "Senha atual", senha = true, erro = controle.errosCampos["senhaAtual"])
                CampoTexto(controle.novaSenha, { controle.novaSenha = it }, "Nova senha", senha = true, erro = controle.errosCampos["novaSenha"])
                CampoTexto(controle.confirmacao, { controle.confirmacao = it }, "Confirmar nova senha", senha = true, erro = controle.errosCampos["confirmacao"])
                BotaoPrincipal("Alterar senha", { controle.alterarSenha() }, carregando = controle.carregando && perfil != null)
            }
        }

        BotaoSecundario("Sair da conta", { confirmarSaida = true }, modifier = Modifier.fillMaxWidth(), perigo = true)
    }

    if (confirmarSaida) {
        AlertDialog(
            onDismissRequest = { confirmarSaida = false },
            title = { Text("Sair da conta") },
            text = { Text("Deseja mesmo sair?") },
            confirmButton = { TextButton(onClick = { confirmarSaida = false; controle.sairConta(aoSair) }) { Text("Sair") } },
            dismissButton = { TextButton(onClick = { confirmarSaida = false }) { Text("Cancelar") } }
        )
    }
}

@Composable
private fun Dado(rotulo: String, valor: String) {
    Column(Modifier.padding(vertical = 6.dp).semantics(mergeDescendants = true) {}) {
        Text(rotulo.uppercase(), fontSize = 11.sp, fontWeight = FontWeight.Bold, color = Cores.TextoFraco)
        Text(valor, color = Cores.TextoSuave, fontSize = 15.sp)
    }
}
