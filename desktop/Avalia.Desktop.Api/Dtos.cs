namespace Avalia.Desktop.Api;

// Formatos JSON da API (camelCase). Campos que a API pode omitir ficam anuláveis.

internal sealed record IncluidoDto(int Id, string? Mensagem);

internal sealed record PainelDto(int Turmas, int Usuarios, int Disciplinas, int Questoes);

internal sealed record UsuarioDto(
    int Id, string Nome, string? Cpf, string Email, int IdPerfil, int? IdTurma, string? Turma,
    DateTime? DataIngresso, bool Ativo);

internal sealed record DisciplinaVinculoDto(int Id, string Nome, bool Ativo);

internal sealed record TurmaDto(
    int Id, string Nome, int AnoLetivo, bool Ativo, int QtdAlunosAtivos, int? QtdDisciplinas,
    List<DisciplinaVinculoDto>? Disciplinas);

internal sealed record DisciplinaDto(
    int Id, string Nome, string? Descricao, bool Ativo, int? IdResponsavel, string? NomeResponsavel,
    int QtdQuestoesAtivas, int QtdTurmas);

internal sealed record QuestaoResumoDto(
    int Id, string Enunciado, int IdDisciplina, string Disciplina, bool DisciplinaAtiva,
    int IdAutor, string Autor, bool TemImagem, bool Utilizada, bool Ativo);

internal sealed record AlternativaDto(string Letra, string Texto, bool Correta);

internal sealed record QuestaoDto(
    int Id, string Enunciado, string? ImagemUrl, string? Explicacao, bool Ativo, int IdDisciplina,
    string Disciplina, bool DisciplinaAtiva, int IdAutor, string Autor, List<AlternativaDto> Alternativas,
    string? Correta, bool Utilizada);

internal sealed record MedalhaDto(int Id, string Nome, int PontosMinimos, int? PontosProximaFaixa, string ImagemUrl, int? QtdAlunos);

internal sealed record ListaMedalhasDto(List<MedalhaDto> Faixas, int QtdAlunosSemMedalha);

internal sealed record UsuarioLoginDto(int Id, string Nome, string Email, int IdPerfil, string Perfil);

internal sealed record LoginDto(string Token, UsuarioLoginDto Usuario, bool SenhaInicial);

internal sealed record UploadDto(string Caminho, string Url);
