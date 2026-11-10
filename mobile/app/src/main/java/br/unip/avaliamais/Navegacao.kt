package br.unip.avaliamais

import androidx.compose.foundation.Image
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.width
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.List
import androidx.compose.material.icons.filled.CheckCircle
import androidx.compose.material.icons.filled.Home
import androidx.compose.material.icons.filled.Person
import androidx.compose.material.icons.filled.Star
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.Icon
import androidx.compose.material3.NavigationBar
import androidx.compose.material3.NavigationBarItem
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.material3.TopAppBar
import androidx.compose.material3.TopAppBarDefaults
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.vector.ImageVector
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.unit.dp
import androidx.lifecycle.viewmodel.compose.viewModel
import androidx.navigation.NavHostController
import androidx.navigation.NavType
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.currentBackStackEntryAsState
import androidx.navigation.compose.rememberNavController
import androidx.navigation.navArgument
import br.unip.avaliamais.controles.ControleAutenticarUsuario
import br.unip.avaliamais.controles.ControleDesempenho
import br.unip.avaliamais.controles.ControlePontuacao
import br.unip.avaliamais.controles.ControleSimulado
import br.unip.avaliamais.controles.Visualizar_perfil
import br.unip.avaliamais.telas.Form_Autenticar_Usuario
import br.unip.avaliamais.telas.Form_Consultar_Historico
import br.unip.avaliamais.telas.Form_Desempenho
import br.unip.avaliamais.telas.Form_Inicio
import br.unip.avaliamais.telas.Form_Perfil_Sem_Telas
import br.unip.avaliamais.telas.Form_Realizar_Simulado
import br.unip.avaliamais.telas.Form_Recuperar_Senha
import br.unip.avaliamais.telas.Form_Resposta
import br.unip.avaliamais.telas.Form_Revisar_Simulados
import br.unip.avaliamais.telas.Form_Visualizar_Pontuacao
import br.unip.avaliamais.telas.Form_Visualizar_perfil
import br.unip.avaliamais.ui.Cores

/** Rotas do aplicativo. */
object Rotas {
    const val LOGIN = "login"
    const val RECUPERAR = "recuperar"
    const val SEM_TELAS = "sem_telas"
    const val INICIO = "inicio"
    const val HISTORICO = "historico"
    const val DESEMPENHO = "desempenho"
    const val PONTUACAO = "pontuacao"
    const val PERFIL = "perfil"
    const val REALIZAR = "realizar"
    const val RESPOSTA = "resposta/{id}"
    const val REVISAO = "revisao/{id}"
    fun resposta(id: Int) = "resposta/$id"
    fun revisao(id: Int) = "revisao/$id"
}

private data class Aba(val rota: String, val rotulo: String, val icone: ImageVector)

private val ABAS = listOf(
    Aba(Rotas.INICIO, "Início", Icons.Filled.Home),
    Aba(Rotas.HISTORICO, "Simulados", Icons.AutoMirrored.Filled.List),
    Aba(Rotas.DESEMPENHO, "Desempenho", Icons.Filled.CheckCircle),
    Aba(Rotas.PONTUACAO, "Pontuação", Icons.Filled.Star),
    Aba(Rotas.PERFIL, "Perfil", Icons.Filled.Person)
)

private const val ALUNO = 1

/** Volta para o login limpando todas as telas anteriores. */
private fun NavHostController.irParaLogin() = navigate(Rotas.LOGIN) { popUpTo(0) { inclusive = true } }

/** Troca de aba sem empilhar telas: o Voltar do celular sempre leva ao Início. */
private fun NavHostController.irParaAba(rota: String) = navigate(rota) {
    popUpTo(Rotas.INICIO) { saveState = true }
    launchSingleTop = true
    restoreState = true
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun NavegacaoAvalia(inicial: String) {
    val nav = rememberNavController()
    val repositorio = (LocalContext.current.applicationContext as AvaliaApp).repositorio
    val sessao by repositorio.sessao.atual.collectAsState()
    val autenticacao: ControleAutenticarUsuario = viewModel(factory = ControleAutenticarUsuario.Fabrica)

    // Token vencido ou conta desativada: a API recusa e o aplicativo volta para o login
    LaunchedEffect(Unit) {
        repositorio.sessaoExpirada.collect {
            autenticacao.limparMensagens()
            nav.irParaLogin()
        }
    }

    val rotaAtual = nav.currentBackStackEntryAsState().value?.destination?.route
    val mostrarAbas = ABAS.any { it.rota == rotaAtual }
    val logado = rotaAtual !in listOf(Rotas.LOGIN, Rotas.RECUPERAR, Rotas.SEM_TELAS, null)

    Scaffold(
        containerColor = Cores.Fundo,
        topBar = {
            if (logado) {
                TopAppBar(
                    title = {
                        Image(painterResource(R.drawable.logo_avalia), contentDescription = "Avalia+", modifier = Modifier.width(110.dp))
                    },
                    colors = TopAppBarDefaults.topAppBarColors(containerColor = Cores.Superficie)
                )
            }
        },
        bottomBar = {
            if (mostrarAbas) {
                NavigationBar(containerColor = Cores.Superficie) {
                    ABAS.forEach { aba ->
                        NavigationBarItem(
                            selected = rotaAtual == aba.rota,
                            onClick = { nav.irParaAba(aba.rota) },
                            icon = { Icon(aba.icone, contentDescription = null) },
                            label = { Text(aba.rotulo) }
                        )
                    }
                }
            }
        }
    ) { espacos ->
        NavHost(nav, startDestination = inicial, modifier = Modifier.padding(espacos)) {
            composable(Rotas.LOGIN) {
                Form_Autenticar_Usuario(
                    controle = autenticacao,
                    aoEntrar = { idPerfil ->
                        val destino = if (idPerfil == ALUNO) Rotas.INICIO else Rotas.SEM_TELAS
                        nav.navigate(destino) { popUpTo(Rotas.LOGIN) { inclusive = true } }
                    },
                    aoRecuperarSenha = { nav.navigate(Rotas.RECUPERAR) }
                )
            }
            composable(Rotas.RECUPERAR) {
                Form_Recuperar_Senha(autenticacao, aoVoltar = { nav.popBackStack() })
            }
            composable(Rotas.SEM_TELAS) {
                Form_Perfil_Sem_Telas(
                    nome = sessao?.nome.orEmpty(),
                    perfil = sessao?.perfil.orEmpty(),
                    aoSair = { autenticacao.sairSistema { nav.irParaLogin() } }
                )
            }

            // Abas do aluno
            composable(Rotas.INICIO) {
                val perfil: Visualizar_perfil = viewModel(factory = Visualizar_perfil.Fabrica)
                Form_Inicio(
                    nome = sessao?.nome.orEmpty(),
                    recomendarTrocaSenha = perfil.recomendarTrocaSenha,
                    desempenho = viewModel(factory = ControleDesempenho.Fabrica),
                    pontuacao = viewModel(factory = ControlePontuacao.Fabrica),
                    aoCriarSimulado = { nav.navigate(Rotas.REALIZAR) },
                    aoTrocarSenha = { nav.irParaAba(Rotas.PERFIL) }
                )
            }
            composable(Rotas.HISTORICO) {
                Form_Consultar_Historico(viewModel(factory = ControleSimulado.Fabrica)) { id -> nav.navigate(Rotas.revisao(id)) }
            }
            composable(Rotas.DESEMPENHO) {
                Form_Desempenho(viewModel(factory = ControleDesempenho.Fabrica))
            }
            composable(Rotas.PONTUACAO) {
                Form_Visualizar_Pontuacao(viewModel(factory = ControlePontuacao.Fabrica))
            }
            composable(Rotas.PERFIL) {
                Form_Visualizar_perfil(viewModel(factory = Visualizar_perfil.Fabrica)) { nav.irParaLogin() }
            }

            // Ciclo do simulado
            composable(Rotas.REALIZAR) {
                Form_Realizar_Simulado(viewModel(factory = ControleSimulado.Fabrica)) { id ->
                    // Sem voltar para a montagem: o botão Voltar não gera outro simulado
                    nav.navigate(Rotas.resposta(id)) { popUpTo(Rotas.INICIO) }
                }
            }
            composable(Rotas.RESPOSTA, arguments = listOf(navArgument("id") { type = NavType.IntType })) { entrada ->
                val id = entrada.arguments?.getInt("id") ?: 0
                Form_Resposta(
                    controle = viewModel(factory = ControleSimulado.Fabrica),
                    idSimulado = id,
                    aoInicio = { nav.irParaAba(Rotas.INICIO) },
                    aoRevisar = { nav.navigate(Rotas.revisao(it)) { popUpTo(Rotas.INICIO) } },
                    aoNovoSimulado = { nav.navigate(Rotas.REALIZAR) { popUpTo(Rotas.INICIO) } }
                )
            }
            composable(Rotas.REVISAO, arguments = listOf(navArgument("id") { type = NavType.IntType })) { entrada ->
                Form_Revisar_Simulados(viewModel(factory = ControleSimulado.Fabrica), entrada.arguments?.getInt("id") ?: 0)
            }
        }
    }
}

/** Tela de abertura enquanto a sessão gravada é lida. */
@Composable
fun Abertura() {
    Box(Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
        Image(painterResource(R.drawable.logo_avalia), contentDescription = "Avalia+", modifier = Modifier.width(180.dp))
    }
}

/** Primeira tela: login, ou o início do perfil se o token gravado ainda estiver na sessão. */
fun rotaInicial(idPerfil: Int?): String = when (idPerfil) {
    null -> Rotas.LOGIN
    ALUNO -> Rotas.INICIO
    else -> Rotas.SEM_TELAS
}
