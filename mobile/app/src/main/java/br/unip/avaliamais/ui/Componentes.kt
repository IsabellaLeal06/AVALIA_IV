package br.unip.avaliamais.ui

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.heightIn
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.LinearProgressIndicator
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.OutlinedButton
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.SolidColor
import androidx.compose.ui.semantics.contentDescription
import androidx.compose.ui.semantics.heading
import androidx.compose.ui.semantics.liveRegion
import androidx.compose.ui.semantics.LiveRegionMode
import androidx.compose.ui.semantics.semantics
import androidx.compose.ui.text.SpanStyle
import androidx.compose.ui.text.buildAnnotatedString
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.text.input.PasswordVisualTransformation
import androidx.compose.ui.text.input.VisualTransformation
import androidx.compose.ui.text.withStyle
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import java.text.NumberFormat
import java.util.Locale

// ------------------------------------------------------------------ formatação (pt-BR)
private val ptBr = Locale.forLanguageTag("pt-BR")

/** "2026-10-14T19:30:00" → "14/10/2026" */
fun formatarData(valor: String?): String {
    if (valor.isNullOrBlank() || valor.length < 10) return "—"
    val (ano, mes, dia) = valor.substring(0, 10).split("-")
    return "$dia/$mes/$ano"
}

/** "2026-10-14T19:30:00" → "14/10/2026 às 19:30" */
fun formatarDataHora(valor: String?): String {
    if (valor.isNullOrBlank() || valor.length < 16) return formatarData(valor)
    return "${formatarData(valor)} às ${valor.substring(11, 16)}"
}

fun formatarPercentual(valor: Double?): String {
    if (valor == null) return "—"
    val formato = NumberFormat.getNumberInstance(ptBr).apply { maximumFractionDigits = 1 }
    return "${formato.format(valor)}%"
}

fun formatarNumero(valor: Int): String = NumberFormat.getIntegerInstance(ptBr).format(valor)

// ------------------------------------------------------------------ títulos e textos
/** Título no estilo da web: primeira parte escura e destaque colorido. */
@Composable
fun TituloPagina(inicio: String, destaque: String, subtitulo: String? = null) {
    Column(Modifier.padding(bottom = 16.dp)) {
        Text(
            text = buildAnnotatedString {
                append("$inicio ")
                withStyle(SpanStyle(color = Cores.Roxo)) { append(destaque) }
            },
            fontSize = 26.sp,
            fontWeight = FontWeight.Bold,
            color = Cores.Texto,
            modifier = Modifier.semantics { heading() }
        )
        if (subtitulo != null) {
            Text(subtitulo, color = Cores.TextoFraco, fontSize = 14.sp, modifier = Modifier.padding(top = 4.dp))
        }
    }
}

@Composable
fun TituloSecao(texto: String, ajuda: String? = null) {
    Column(Modifier.padding(bottom = 10.dp)) {
        Text(texto, fontWeight = FontWeight.Bold, fontSize = 17.sp, modifier = Modifier.semantics { heading() })
        if (ajuda != null) Text(ajuda, color = Cores.TextoFraco, fontSize = 13.sp)
    }
}

enum class TipoMensagem(val fundo: Color, val texto: Color) {
    ERRO(Cores.ErroFundo, Cores.Erro),
    SUCESSO(Cores.SucessoFundo, Cores.Sucesso),
    ALERTA(Cores.AlertaFundo, Cores.Alerta),
    INFO(Cores.InfoFundo, Color(0xFF3730A3))
}

/** Mensagem na tela (o leitor de tela anuncia quando ela aparece). */
@Composable
fun Mensagem(texto: String?, tipo: TipoMensagem = TipoMensagem.ERRO) {
    if (texto.isNullOrBlank()) return
    Text(
        text = texto,
        color = tipo.texto,
        fontSize = 14.sp,
        modifier = Modifier
            .fillMaxWidth()
            .padding(vertical = 6.dp)
            .clip(RoundedCornerShape(10.dp))
            .background(tipo.fundo)
            .padding(12.dp)
            .semantics { liveRegion = LiveRegionMode.Polite }
    )
}

@Composable
fun Carregando(texto: String = "Carregando...") {
    Row(
        Modifier
            .fillMaxWidth()
            .padding(24.dp),
        horizontalArrangement = Arrangement.Center,
        verticalAlignment = Alignment.CenterVertically
    ) {
        CircularProgressIndicator(Modifier.height(24.dp).width(24.dp), strokeWidth = 3.dp)
        Spacer(Modifier.width(12.dp))
        Text(texto, color = Cores.TextoFraco)
    }
}

// ------------------------------------------------------------------ cartões e barras
@Composable
fun Caixa(modifier: Modifier = Modifier, conteudo: @Composable () -> Unit) {
    Box(
        modifier
            .fillMaxWidth()
            .padding(bottom = 14.dp)
            .clip(RoundedCornerShape(14.dp))
            .background(Cores.Superficie)
            .padding(18.dp)
    ) { conteudo() }
}

/** Card de indicador com a borda roxa à esquerda, como na web. */
@Composable
fun CardIndicador(rotulo: String, valor: String, modifier: Modifier = Modifier, detalhe: String? = null) {
    Row(
        modifier
            .padding(bottom = 12.dp)
            .clip(RoundedCornerShape(14.dp))
            .background(Cores.Superficie)
            .semantics(mergeDescendants = true) {}
    ) {
        Box(Modifier.width(4.dp).heightIn(min = 84.dp).background(Cores.Roxo))
        Column(Modifier.padding(14.dp)) {
            Text(rotulo.uppercase(), fontSize = 11.sp, color = Cores.TextoFraco)
            Text(valor, fontSize = 26.sp, fontWeight = FontWeight.Bold, color = Cores.Azul, modifier = Modifier.padding(top = 6.dp))
            if (detalhe != null) Text(detalhe, fontSize = 12.sp, color = Cores.TextoFraco)
        }
    }
}

/** Barra de aproveitamento (0 a 100). */
@Composable
fun BarraDesempenho(rotulo: String, percentual: Double?, detalhe: String? = null) {
    val textoValor = if (percentual == null) "sem respostas" else formatarPercentual(percentual)
    Column(
        Modifier
            .padding(vertical = 6.dp)
            .semantics(mergeDescendants = true) {
                contentDescription = "$rotulo: $textoValor${detalhe?.let { ", $it" } ?: ""}"
            }
    ) {
        Row(Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
            Text(rotulo, fontSize = 14.sp, modifier = Modifier.weight(1f))
            Text(listOfNotNull(textoValor, detalhe).joinToString(" · "), fontSize = 13.sp, color = Cores.TextoFraco)
        }
        LinearProgressIndicator(
            progress = { ((percentual ?: 0.0) / 100.0).toFloat().coerceIn(0f, 1f) },
            modifier = Modifier
                .fillMaxWidth()
                .padding(top = 6.dp)
                .height(8.dp)
                .clip(RoundedCornerShape(50)),
            color = Cores.Azul,
            trackColor = Color(0xFFECEEF3)
        )
    }
}

// ------------------------------------------------------------------ botões e campos
/** Botão principal com o degradê azul e roxo da web. */
@Composable
fun BotaoPrincipal(texto: String, aoClicar: () -> Unit, modifier: Modifier = Modifier, habilitado: Boolean = true, carregando: Boolean = false) {
    val ativo = habilitado && !carregando
    Button(
        onClick = aoClicar,
        enabled = ativo,
        modifier = modifier
            .fillMaxWidth()
            .heightIn(min = 50.dp)
            .clip(RoundedCornerShape(12.dp))
            .background(if (ativo) Cores.Gradiente else SolidColor(Cores.Borda)),
        colors = ButtonDefaults.buttonColors(
            containerColor = Color.Transparent,
            disabledContainerColor = Color.Transparent,
            contentColor = Color.White,
            disabledContentColor = Cores.TextoFraco
        ),
        shape = RoundedCornerShape(12.dp)
    ) {
        if (carregando) {
            CircularProgressIndicator(Modifier.height(18.dp).width(18.dp), color = Color.White, strokeWidth = 2.dp)
            Spacer(Modifier.width(10.dp))
        }
        Text(texto, fontWeight = FontWeight.Bold)
    }
}

@Composable
fun BotaoSecundario(texto: String, aoClicar: () -> Unit, modifier: Modifier = Modifier, perigo: Boolean = false, habilitado: Boolean = true) {
    OutlinedButton(
        onClick = aoClicar,
        enabled = habilitado,
        modifier = modifier.heightIn(min = 48.dp),
        shape = RoundedCornerShape(12.dp),
        colors = ButtonDefaults.outlinedButtonColors(contentColor = if (perigo) Cores.Erro else Cores.Azul)
    ) { Text(texto, fontWeight = FontWeight.Bold) }
}

@Composable
fun CampoTexto(
    valor: String,
    aoMudar: (String) -> Unit,
    rotulo: String,
    modifier: Modifier = Modifier,
    erro: String? = null,
    teclado: KeyboardType = KeyboardType.Text,
    senha: Boolean = false,
    ajuda: String? = null,
    somenteLeitura: Boolean = false
) {
    OutlinedTextField(
        value = valor,
        onValueChange = aoMudar,
        label = { Text(rotulo) },
        isError = erro != null,
        readOnly = somenteLeitura,
        singleLine = true,
        visualTransformation = if (senha) PasswordVisualTransformation() else VisualTransformation.None,
        keyboardOptions = KeyboardOptions(keyboardType = if (senha) KeyboardType.Password else teclado),
        supportingText = (erro ?: ajuda)?.let { texto -> @Composable { Text(texto) } },
        shape = RoundedCornerShape(12.dp),
        modifier = modifier
            .fillMaxWidth()
            .padding(bottom = 6.dp)
    )
}

/** Círculo com as iniciais do nome (avatar), como no cabeçalho da web. */
@Composable
fun Avatar(nome: String, tamanho: Int = 72) {
    val partes = nome.trim().split(Regex("\\s+"))
    val iniciais = ((partes.firstOrNull()?.firstOrNull()?.toString() ?: "") +
        (if (partes.size > 1) partes.last().first().toString() else "")).uppercase()
    Box(
        Modifier
            .width(tamanho.dp)
            .height(tamanho.dp)
            .clip(RoundedCornerShape(20.dp))
            .background(Cores.Gradiente),
        contentAlignment = Alignment.Center
    ) {
        Text(iniciais, color = Color.White, fontSize = (tamanho / 2.6).sp, fontWeight = FontWeight.Bold)
    }
}

/** Etiqueta pequena (ex.: situação da questão na revisão). */
@Composable
fun Etiqueta(texto: String, fundo: Color, cor: Color) {
    Text(
        texto,
        color = cor,
        fontSize = 12.sp,
        fontWeight = FontWeight.Bold,
        modifier = Modifier
            .clip(RoundedCornerShape(50))
            .background(fundo)
            .padding(horizontal = 10.dp, vertical = 4.dp)
    )
}
