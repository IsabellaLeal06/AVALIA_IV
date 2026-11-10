package br.unip.avaliamais.dados

import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Path
import retrofit2.http.Query

/**
 * Rotas da API usadas pelo aplicativo do Aluno (RN26).
 * As regras (sorteio, correção, pontuação e medalha) ficam todas na API (RNF07):
 * o aplicativo só envia e exibe.
 */
interface ApiAvalia {

    // RF01 e RN03: rotas abertas
    @POST("auth/login")
    suspend fun login(@Body corpo: LoginCorpo): LoginResposta

    @POST("auth/recuperar-senha")
    suspend fun recuperarSenha(@Body corpo: RecuperarSenhaCorpo): RecuperarSenhaResposta

    @GET("auth/validar-codigo")
    suspend fun validarCodigo(@Query("email") email: String, @Query("codigo") codigo: String): Map<String, Boolean>

    @POST("auth/nova-senha")
    suspend fun novaSenha(@Body corpo: NovaSenhaCorpo): Mensagem

    // Rotas com token
    @POST("auth/alterar-senha")
    suspend fun alterarSenha(@Body corpo: AlterarSenhaCorpo): Mensagem

    @GET("perfil")
    suspend fun perfil(): Perfil

    @GET("disciplinas/ativas")
    suspend fun disciplinasAtivas(): List<DisciplinaAtiva>

    @POST("simulados")
    suspend fun gerarSimulado(@Body corpo: GerarSimuladoCorpo): SimuladoCriado

    @GET("simulados/{id}")
    suspend fun consultarSimulado(@Path("id") id: Int): SimuladoRealizacao

    @POST("simulados/{id}/finalizar")
    suspend fun finalizarSimulado(@Path("id") id: Int, @Body corpo: FinalizarCorpo): Resultado

    @GET("simulados/{id}/resultado")
    suspend fun resultado(@Path("id") id: Int): Resultado

    @GET("simulados/{id}/revisao")
    suspend fun revisao(@Path("id") id: Int): Revisao

    @GET("simulados")
    suspend fun historico(@Query("pagina") pagina: Int, @Query("tamanho") tamanho: Int): Historico

    @GET("desempenho/aluno")
    suspend fun desempenhoAluno(): DesempenhoAluno
}
