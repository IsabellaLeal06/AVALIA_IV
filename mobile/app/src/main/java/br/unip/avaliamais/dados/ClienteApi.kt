package br.unip.avaliamais.dados

import kotlinx.serialization.json.Json
import okhttp3.MediaType.Companion.toMediaType
import okhttp3.OkHttpClient
import okhttp3.HttpUrl.Companion.toHttpUrlOrNull
import retrofit2.Retrofit
import retrofit2.converter.kotlinx.serialization.asConverterFactory
import java.util.concurrent.TimeUnit

/**
 * Cria o cliente HTTP da API. O token do usuário logado vai no cabeçalho Authorization
 * de toda requisição; o perfil nunca é informado pelo aplicativo (a API lê do token).
 */
class ClienteApi(private val sessao: Sessao) {

    val json = Json {
        ignoreUnknownKeys = true
        explicitNulls = true
        coerceInputValues = true
    }

    private val http = OkHttpClient.Builder()
        // O Azure SQL gratuito pode estar "acordando" da pausa automática: dá mais tempo
        .connectTimeout(20, TimeUnit.SECONDS)
        .readTimeout(70, TimeUnit.SECONDS)
        .addInterceptor { cadeia ->
            val token = sessao.token
            val requisicao = cadeia.request().newBuilder().apply {
                if (token != null) header("Authorization", "Bearer $token")
            }.build()
            cadeia.proceed(requisicao)
        }
        .build()

    private var urlAtual: String? = null
    private var apiAtual: ApiAvalia? = null

    /** API apontando para o endereço configurado (recriada se o endereço mudar). */
    @Synchronized
    fun api(): ApiAvalia {
        val url = sessao.urlApi.value
        if (apiAtual == null || url != urlAtual) {
            apiAtual = Retrofit.Builder()
                .baseUrl(url)
                .client(http)
                .addConverterFactory(json.asConverterFactory("application/json".toMediaType()))
                .build()
                .create(ApiAvalia::class.java)
            urlAtual = url
        }
        return apiAtual!!
    }

    /**
     * A API devolve o endereço completo das imagens montado com o endereço dela (ex.: http://localhost:3000/...).
     * No emulador ou no celular esse "localhost" não é o computador, então o caminho é mantido
     * e o começo é trocado pelo endereço configurado no aplicativo.
     */
    fun urlImagem(url: String?): String? {
        if (url.isNullOrBlank()) return null
        val base = sessao.urlApi.value.toHttpUrlOrNull() ?: return url
        val original = url.toHttpUrlOrNull() ?: return base.toString().trimEnd('/') + "/" + url.trimStart('/')
        return original.newBuilder().scheme(base.scheme).host(base.host).port(base.port).build().toString()
    }
}
