package br.unip.avaliamais

import android.app.Application
import br.unip.avaliamais.dados.ClienteApi
import br.unip.avaliamais.dados.Repositorio
import br.unip.avaliamais.dados.Sessao

/** Cria, uma única vez, a sessão e o acesso à API usados por todos os controles. */
class AvaliaApp : Application() {

    lateinit var repositorio: Repositorio
        private set

    override fun onCreate() {
        super.onCreate()
        val sessao = Sessao(this)
        repositorio = Repositorio(sessao, ClienteApi(sessao))
    }
}
