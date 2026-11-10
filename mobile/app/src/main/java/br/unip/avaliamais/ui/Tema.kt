package br.unip.avaliamais.ui

import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.lightColorScheme
import androidx.compose.runtime.Composable
import androidx.compose.ui.graphics.Brush
import androidx.compose.ui.graphics.Color

// Mesmas cores da versão web (web/css/base.css)
object Cores {
    val Azul = Color(0xFF2563EB)
    val Roxo = Color(0xFF7C3AED)
    val Fundo = Color(0xFFF5F6FA)
    val Superficie = Color.White
    val SuperficieSuave = Color(0xFFF8F9FC)
    val Texto = Color(0xFF111827)
    val TextoSuave = Color(0xFF4B5563)
    val TextoFraco = Color(0xFF6B7280)
    val Borda = Color(0xFFE5E7EB)
    val Sucesso = Color(0xFF15803D)
    val SucessoFundo = Color(0xFFDCFCE7)
    val Erro = Color(0xFFB91C1C)
    val ErroFundo = Color(0xFFFEE2E2)
    val Alerta = Color(0xFF92400E)
    val AlertaFundo = Color(0xFFFEF3C7)
    val InfoFundo = Color(0xFFEEF2FF)
    val Gradiente = Brush.horizontalGradient(listOf(Azul, Roxo))
}

private val esquema = lightColorScheme(
    primary = Cores.Azul,
    onPrimary = Color.White,
    secondary = Cores.Roxo,
    onSecondary = Color.White,
    background = Cores.Fundo,
    onBackground = Cores.Texto,
    surface = Cores.Superficie,
    onSurface = Cores.Texto,
    surfaceVariant = Cores.SuperficieSuave,
    onSurfaceVariant = Cores.TextoSuave,
    outline = Cores.Borda,
    error = Cores.Erro
)

@Composable
fun TemaAvalia(conteudo: @Composable () -> Unit) {
    MaterialTheme(colorScheme = esquema, content = conteudo)
}
