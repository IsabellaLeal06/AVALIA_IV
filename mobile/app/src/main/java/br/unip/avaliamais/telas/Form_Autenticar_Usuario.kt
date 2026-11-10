@file:Suppress("FunctionName")

package br.unip.avaliamais.telas

import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.imePadding
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.AlertDialog
import androidx.compose.material3.Checkbox
import androidx.compose.material3.Text
import androidx.compose.material3.TextButton
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.semantics.heading
import androidx.compose.ui.semantics.semantics
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import br.unip.avaliamais.R
import br.unip.avaliamais.controles.ControleAutenticarUsuario
import br.unip.avaliamais.controles.EtapaRecuperacao
import br.unip.avaliamais.ui.BotaoPrincipal
import br.unip.avaliamais.ui.BotaoSecundario
import br.unip.avaliamais.ui.CampoTexto
import br.unip.avaliamais.ui.Cores
import br.unip.avaliamais.ui.Mensagem
import br.unip.avaliamais.ui.TipoMensagem

/** Layout comum das telas abertas: logo, chamada e o formulário. */
@Composable
private fun TelaAberta(conteudo: @Composable () -> Unit) {
    Column(
        Modifier
            .fillMaxSize()
            .background(Cores.Fundo)
            .verticalScroll(rememberScrollState())
            .imePadding()
            .padding(horizontal = 24.dp, vertical = 32.dp)
    ) {
        Image(
            painter = painterResource(R.drawable.logo_avalia),
            contentDescription = "Avalia+",
            modifier = Modifier.width(150.dp)
        )
        Spacer(Modifier.height(28.dp))
        Text("Simulados inteligentes.", fontSize = 26.sp, fontWeight = FontWeight.Bold)
        Text("Resultados reais.", fontSize = 26.sp, fontWeight = FontWeight.Bold, color = Cores.Roxo)
        Text(
            "Pratique com as questões do acervo da sua instituição e acompanhe o seu desempenho por disciplina.",
            color = Cores.TextoFraco,
            modifier = Modifier.padding(top = 8.dp, bottom = 28.dp)
        )
        conteudo()
    }
}

/**
 * Tela Form_Autenticar_Usuario (diagrama UML). A mensagem de falha é a mesma para qualquer erro.
 * Não existe autocadastro (RN01) nem bloqueio por plataforma (RN26).
 */
@Composable
fun Form_Autenticar_Usuario(
    controle: ControleAutenticarUsuario,
    aoEntrar: (idPerfil: Int) -> Unit,
    aoRecuperarSenha: () -> Unit
) {
    var mostrarSenha by remember { mutableStateOf(false) }
    var editarServidor by remember { mutableStateOf(false) }

    TelaAberta {
        Text(
            "Bem-vindo(a) de volta",
            fontSize = 22.sp,
            fontWeight = FontWeight.Bold,
            color = Cores.Azul,
            modifier = Modifier.semantics { heading() }
        )
        Text("Entre com o seu e-mail e a sua senha.", color = Cores.TextoFraco, modifier = Modifier.padding(bottom = 16.dp))

        Mensagem(controle.erro)
        Mensagem(controle.informacao, TipoMensagem.INFO)

        CampoTexto(controle.email, { controle.email = it }, "E-mail", teclado = KeyboardType.Email)
        CampoTexto(controle.senha, { controle.senha = it }, "Senha", senha = !mostrarSenha)
        Row(verticalAlignment = Alignment.CenterVertically) {
            Checkbox(checked = mostrarSenha, onCheckedChange = { mostrarSenha = it })
            Text("Mostrar senha")
        }

        Spacer(Modifier.height(12.dp))
        BotaoPrincipal("Entrar", { controle.autenticarUsuario(aoEntrar) }, carregando = controle.carregando)

        Row(Modifier.fillMaxWidth().padding(top = 10.dp), horizontalArrangement = Arrangement.SpaceBetween) {
            TextButton(onClick = { controle.limparMensagens(); aoRecuperarSenha() }) { Text("Esqueceu a sua senha?") }
            TextButton(onClick = { editarServidor = true }) { Text("Servidor da API") }
        }
    }

    if (editarServidor) {
        DialogoServidor(
            urlAtual = controle.urlApi,
            aoSalvar = {
                controle.definirServidor(it)
                editarServidor = false
            },
            aoCancelar = { editarServidor = false }
        )
    }
}

/**
 * Endereço da API: no emulador, http://10.0.2.2:3000/ (padrão);
 * no celular, http://IP-do-computador:3000/ (os dois na mesma rede Wi-Fi).
 */
@Composable
private fun DialogoServidor(urlAtual: String, aoSalvar: (String) -> Unit, aoCancelar: () -> Unit) {
    var url by remember { mutableStateOf(urlAtual) }
    AlertDialog(
        onDismissRequest = aoCancelar,
        title = { Text("Servidor da API") },
        text = {
            Column {
                Text(
                    "Emulador do Android Studio: http://10.0.2.2:3000/\n" +
                        "Celular: http://IP-do-computador:3000/ (mesma rede Wi-Fi). O IP aparece no comando ipconfig do Windows.",
                    fontSize = 13.sp,
                    color = Cores.TextoSuave,
                    modifier = Modifier.padding(bottom = 12.dp)
                )
                CampoTexto(url, { url = it }, "Endereço da API", teclado = KeyboardType.Uri)
            }
        },
        confirmButton = { TextButton(onClick = { aoSalvar(url) }) { Text("Salvar") } },
        dismissButton = { TextButton(onClick = aoCancelar) { Text("Cancelar") } }
    )
}

/**
 * Recuperação de senha (RN03): e-mail → código recebido por e-mail → nova senha.
 * Os campos da nova senha só aparecem depois que a API confere o código.
 */
@Composable
fun Form_Recuperar_Senha(controle: ControleAutenticarUsuario, aoVoltar: () -> Unit) {
    TelaAberta {
        Text(
            "Recuperação de senha",
            fontSize = 22.sp,
            fontWeight = FontWeight.Bold,
            color = Cores.Azul,
            modifier = Modifier.semantics { heading() }
        )
        Text(
            when (controle.etapa) {
                EtapaRecuperacao.EMAIL -> "Informe o e-mail cadastrado. Você vai receber um código de 6 dígitos."
                EtapaRecuperacao.CODIGO -> "Digite o código que chegou no seu e-mail."
                EtapaRecuperacao.NOVA_SENHA -> "Crie a nova senha."
            },
            color = Cores.TextoFraco,
            modifier = Modifier.padding(bottom = 16.dp)
        )

        Mensagem(controle.erro)
        Mensagem(controle.informacao, TipoMensagem.SUCESSO)

        CampoTexto(
            controle.email,
            { controle.email = it },
            "E-mail",
            teclado = KeyboardType.Email,
            erro = controle.errosCampos["email"],
            somenteLeitura = controle.etapa != EtapaRecuperacao.EMAIL
        )

        when (controle.etapa) {
            EtapaRecuperacao.EMAIL ->
                BotaoPrincipal("Enviar código", { controle.recuperarSenha() }, carregando = controle.carregando)

            EtapaRecuperacao.CODIGO -> {
                CampoTexto(
                    controle.codigo,
                    { valor -> controle.codigo = valor.filter { it.isDigit() }.take(6) },
                    "Código de 6 dígitos",
                    teclado = KeyboardType.NumberPassword
                )
                BotaoPrincipal("Conferir código", { controle.validarCodigo() }, carregando = controle.carregando)
                TextButton(onClick = { controle.recuperarSenha() }) { Text("Pedir um novo código") }
            }

            EtapaRecuperacao.NOVA_SENHA -> {
                CampoTexto(
                    controle.novaSenha,
                    { controle.novaSenha = it },
                    "Nova senha",
                    senha = true,
                    erro = controle.errosCampos["novaSenha"],
                    ajuda = "Mínimo de 8 caracteres, com pelo menos uma letra e um número."
                )
                CampoTexto(
                    controle.confirmacao,
                    { controle.confirmacao = it },
                    "Confirmar nova senha",
                    senha = true,
                    erro = controle.errosCampos["confirmacao"]
                )
                BotaoPrincipal("Salvar nova senha", { controle.salvarNovaSenha(aoVoltar) }, carregando = controle.carregando)
            }
        }

        Spacer(Modifier.height(10.dp))
        BotaoSecundario(
            "Voltar para o login",
            { controle.reiniciarRecuperacao(); controle.limparMensagens(); aoVoltar() },
            modifier = Modifier.fillMaxWidth()
        )
    }
}

/**
 * O aplicativo tem as telas do Aluno (RN26). Professor e Administrador conseguem entrar
 * (não há bloqueio por plataforma), mas as telas deles estão na Web.
 */
@Composable
fun Form_Perfil_Sem_Telas(nome: String, perfil: String, aoSair: () -> Unit) {
    TelaAberta {
        Text("Olá, ${nome.substringBefore(' ')}!", fontSize = 22.sp, fontWeight = FontWeight.Bold, modifier = Modifier.semantics { heading() })
        Mensagem(
            "O aplicativo traz as telas do Aluno. As telas do perfil $perfil estão na versão Web do Avalia+.",
            TipoMensagem.INFO
        )
        Spacer(Modifier.height(12.dp))
        BotaoSecundario("Sair", aoSair, modifier = Modifier.fillMaxWidth())
    }
}
