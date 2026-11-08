namespace Avalia.Desktop.Telas
{
    partial class Form_Questao
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pnlCabecalho = new Panel();
            label_titulo = new Label();
            lblSubtitulo = new Label();
            pnlFiltros = new Panel();
            lblBusca = new Label();
            txtBusca = new TextBox();
            lblDisciplinaFiltro = new Label();
            cboDisciplinaFiltro = new ComboBox();
            lblSituacaoFiltro = new Label();
            cboSituacaoFiltro = new ComboBox();
            btnLimparFiltros = new Button();
            pnlAcoes = new Panel();
            btn_novo = new Button();
            btn_consultarPorId = new Button();
            btn_excluir = new Button();
            btn_reativar = new Button();
            btn_listar = new Button();
            label_totalRegistros = new Label();
            dgvQuestoes = new DataGridView();
            colId = new DataGridViewTextBoxColumn();
            colEnunciado = new DataGridViewTextBoxColumn();
            colDisciplina = new DataGridViewTextBoxColumn();
            colAutor = new DataGridViewTextBoxColumn();
            colUtilizada = new DataGridViewTextBoxColumn();
            colSituacao = new DataGridViewTextBoxColumn();
            tmrBusca = new System.Windows.Forms.Timer(components);
            label_modo = new Label();
            pnlLista = new Panel();
            pnlLegenda = new Panel();
            label_lista = new Label();
            label_id = new Label();
            button_id = new TextBox();
            pnlCadastro = new Panel();
            pnlCartao = new Panel();
            lblRegistro = new Label();
            pnlAvisoGabarito = new Panel();
            label_avisoGabarito = new Label();
            label_disciplina = new Label();
            button_disciplina = new ComboBox();
            label_autor = new Label();
            label_enunciado = new Label();
            button_enunciado = new TextBox();
            label_imagemURL = new Label();
            picImagem = new PictureBox();
            button_imagemURL = new TextBox();
            btn_selecionarImagem = new Button();
            btnRemoverImagem = new Button();
            grpAlternativas = new GroupBox();
            label_letraA = new Label();
            button_alternativaA = new TextBox();
            label_letraB = new Label();
            button_alternativaB = new TextBox();
            label_letraC = new Label();
            button_alternativaC = new TextBox();
            label_letraD = new Label();
            button_alternativaD = new TextBox();
            label_letraE = new Label();
            button_alternativaE = new TextBox();
            label_explicacaoResposta = new Label();
            button_explicacaoResposta = new TextBox();
            lblObrigatorios = new Label();
            btn_incluir = new Button();
            btn_voltar = new Button();
            errorProvider = new ErrorProvider(components);
            btn_editar = new Button();
            label_alternativaCorreta = new Label();
            button_alternativaCorreta = new ComboBox();
            label_ativo = new Label();
            button_ativo = new CheckBox();
            pnlCabecalho.SuspendLayout();
            pnlFiltros.SuspendLayout();
            pnlAcoes.SuspendLayout();
            pnlLista.SuspendLayout();
            pnlLegenda.SuspendLayout();
            pnlCadastro.SuspendLayout();
            pnlCartao.SuspendLayout();
            pnlAvisoGabarito.SuspendLayout();
            grpAlternativas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvQuestoes).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picImagem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
            SuspendLayout();
            // 
            // pnlCabecalho
            // 
            pnlCabecalho.Controls.Add(lblSubtitulo);
            pnlCabecalho.Controls.Add(label_titulo);
            pnlCabecalho.Controls.Add(label_modo);
            pnlCabecalho.Dock = DockStyle.Top;
            pnlCabecalho.Location = new Point(0, 0);
            pnlCabecalho.Name = "pnlCabecalho";
            pnlCabecalho.Size = new Size(1000, 76);
            pnlCabecalho.TabIndex = 0;
            // 
            // label_titulo
            // 
            label_titulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            label_titulo.ForeColor = Color.FromArgb(17, 24, 39);
            label_titulo.Location = new Point(0, 0);
            label_titulo.Name = "label_titulo";
            label_titulo.Size = new Size(740, 40);
            label_titulo.TabIndex = 0;
            label_titulo.Text = "Questões";
            // 
            // lblSubtitulo
            // 
            lblSubtitulo.ForeColor = Color.FromArgb(75, 85, 99);
            lblSubtitulo.Location = new Point(0, 42);
            lblSubtitulo.Name = "lblSubtitulo";
            lblSubtitulo.Size = new Size(980, 24);
            lblSubtitulo.TabIndex = 1;
            lblSubtitulo.Text = "Banco de questões de todas as disciplinas, com a disciplina e quem cadastrou cada questão.";
            // 
            // pnlFiltros
            // 
            pnlFiltros.Controls.Add(btnLimparFiltros);
            pnlFiltros.Controls.Add(cboSituacaoFiltro);
            pnlFiltros.Controls.Add(lblSituacaoFiltro);
            pnlFiltros.Controls.Add(cboDisciplinaFiltro);
            pnlFiltros.Controls.Add(lblDisciplinaFiltro);
            pnlFiltros.Controls.Add(txtBusca);
            pnlFiltros.Controls.Add(lblBusca);
            pnlFiltros.BackColor = Color.White;
            pnlFiltros.Dock = DockStyle.Top;
            pnlFiltros.Location = new Point(0, 76);
            pnlFiltros.Name = "pnlFiltros";
            pnlFiltros.Size = new Size(1000, 72);
            pnlFiltros.TabIndex = 1;
            // 
            // lblBusca
            // 
            lblBusca.Location = new Point(16, 8);
            lblBusca.Name = "lblBusca";
            lblBusca.Size = new Size(280, 22);
            lblBusca.TabIndex = 0;
            lblBusca.Text = "&Buscar no enunciado";
            // 
            // txtBusca
            // 
            txtBusca.AccessibleName = "Buscar no enunciado";
            txtBusca.Location = new Point(16, 32);
            txtBusca.Name = "txtBusca";
            txtBusca.Size = new Size(280, 25);
            txtBusca.TabIndex = 1;
            txtBusca.TextChanged += txtBusca_TextChanged;
            // 
            // lblDisciplinaFiltro
            // 
            lblDisciplinaFiltro.Location = new Point(312, 8);
            lblDisciplinaFiltro.Name = "lblDisciplinaFiltro";
            lblDisciplinaFiltro.Size = new Size(280, 22);
            lblDisciplinaFiltro.TabIndex = 2;
            lblDisciplinaFiltro.Text = "Dis&ciplina";
            // 
            // cboDisciplinaFiltro
            // 
            cboDisciplinaFiltro.AccessibleName = "Disciplina";
            cboDisciplinaFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDisciplinaFiltro.Location = new Point(312, 32);
            cboDisciplinaFiltro.Name = "cboDisciplinaFiltro";
            cboDisciplinaFiltro.Size = new Size(280, 25);
            cboDisciplinaFiltro.TabIndex = 3;
            cboDisciplinaFiltro.SelectedIndexChanged += Filtro_Alterado;
            // 
            // lblSituacaoFiltro
            // 
            lblSituacaoFiltro.Location = new Point(608, 8);
            lblSituacaoFiltro.Name = "lblSituacaoFiltro";
            lblSituacaoFiltro.Size = new Size(130, 22);
            lblSituacaoFiltro.TabIndex = 4;
            lblSituacaoFiltro.Text = "S&ituação";
            // 
            // cboSituacaoFiltro
            // 
            cboSituacaoFiltro.AccessibleName = "Situação";
            cboSituacaoFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSituacaoFiltro.Location = new Point(608, 32);
            cboSituacaoFiltro.Name = "cboSituacaoFiltro";
            cboSituacaoFiltro.Size = new Size(130, 25);
            cboSituacaoFiltro.TabIndex = 5;
            cboSituacaoFiltro.SelectedIndexChanged += Filtro_Alterado;
            // 
            // btnLimparFiltros
            // 
            btnLimparFiltros.AccessibleName = "Limpar filtros";
            btnLimparFiltros.Location = new Point(754, 30);
            btnLimparFiltros.Name = "btnLimparFiltros";
            btnLimparFiltros.Size = new Size(130, 30);
            btnLimparFiltros.TabIndex = 6;
            btnLimparFiltros.Text = "&Limpar filtros";
            btnLimparFiltros.Click += btnLimparFiltros_Click;
            // 
            // pnlAcoes
            // 
            pnlAcoes.Controls.Add(btn_listar);
            pnlAcoes.Controls.Add(btn_reativar);
            pnlAcoes.Controls.Add(btn_excluir);
            pnlAcoes.Controls.Add(btn_novo);
            pnlAcoes.Controls.Add(label_id);
            pnlAcoes.Controls.Add(button_id);
            pnlAcoes.Controls.Add(btn_consultarPorId);
            pnlAcoes.Dock = DockStyle.Top;
            pnlAcoes.Location = new Point(0, 148);
            pnlAcoes.Name = "pnlAcoes";
            pnlAcoes.Size = new Size(1000, 60);
            pnlAcoes.TabIndex = 2;
            // 
            // btn_novo
            // 
            btn_novo.AccessibleName = "Cadastrar novo registro";
            btn_novo.Location = new Point(0, 12);
            btn_novo.Name = "btn_novo";
            btn_novo.Size = new Size(110, 36);
            btn_novo.TabIndex = 0;
            btn_novo.Text = "&Nova";
            btn_novo.Click += btn_novo_Click;
            // 
            // btn_consultarPorId
            // 
            btn_consultarPorId.AccessibleName = "Consultar o ID informado ou o registro selecionado";
            btn_consultarPorId.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_consultarPorId.Location = new Point(860, 12);
            btn_consultarPorId.Name = "btn_consultarPorId";
            btn_consultarPorId.Size = new Size(140, 36);
            btn_consultarPorId.TabIndex = 7;
            btn_consultarPorId.Text = "&Consultar";
            btn_consultarPorId.Click += btn_consultarPorId_Click;
            // 
            // btn_excluir
            // 
            btn_excluir.AccessibleName = "Desativar registro selecionado";
            btn_excluir.Location = new Point(120, 12);
            btn_excluir.Name = "btn_excluir";
            btn_excluir.Size = new Size(130, 36);
            btn_excluir.TabIndex = 2;
            btn_excluir.Text = "De&sativar";
            btn_excluir.Click += btn_excluir_Click;
            // 
            // btn_reativar
            // 
            btn_reativar.AccessibleName = "Reativar registro selecionado";
            btn_reativar.Location = new Point(260, 12);
            btn_reativar.Name = "btn_reativar";
            btn_reativar.Size = new Size(130, 36);
            btn_reativar.TabIndex = 3;
            btn_reativar.Text = "&Reativar";
            btn_reativar.Click += btn_reativar_Click;
            // 
            // btn_listar
            // 
            btn_listar.AccessibleName = "Listar novamente";
            btn_listar.Location = new Point(400, 12);
            btn_listar.Name = "btn_listar";
            btn_listar.Size = new Size(130, 36);
            btn_listar.TabIndex = 4;
            btn_listar.Text = "&Listar (F5)";
            btn_listar.Click += btn_listar_Click;
            // 
            // label_totalRegistros
            // 
            label_totalRegistros.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label_totalRegistros.ForeColor = Color.FromArgb(75, 85, 99);
            label_totalRegistros.Location = new Point(700, 4);
            label_totalRegistros.Name = "label_totalRegistros";
            label_totalRegistros.Size = new Size(300, 22);
            label_totalRegistros.TabIndex = 5;
            label_totalRegistros.TextAlign = ContentAlignment.MiddleRight;
            // 
            // dgvQuestoes
            // 
            dgvQuestoes.AccessibleName = "Lista de questões";
            dgvQuestoes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvQuestoes.Columns.AddRange(new DataGridViewColumn[] { colId, colEnunciado, colDisciplina, colAutor, colUtilizada, colSituacao });
            dgvQuestoes.Dock = DockStyle.Fill;
            dgvQuestoes.Location = new Point(0, 208);
            dgvQuestoes.Name = "dgvQuestoes";
            dgvQuestoes.Size = new Size(1000, 432);
            dgvQuestoes.TabIndex = 3;
            dgvQuestoes.CellDoubleClick += dgvQuestoes_CellDoubleClick;
            dgvQuestoes.SelectionChanged += dgvQuestoes_SelectionChanged;
            dgvQuestoes.KeyDown += dgvQuestoes_KeyDown;
            // 
            // colId
            // 
            colId.DataPropertyName = "Id";
            colId.FillWeight = 5F;
            colId.HeaderText = "Nº";
            colId.Name = "colId";
            colId.ReadOnly = true;
            // 
            // colEnunciado
            // 
            colEnunciado.DataPropertyName = "EnunciadoResumido";
            colEnunciado.FillWeight = 45F;
            colEnunciado.HeaderText = "Enunciado";
            colEnunciado.Name = "colEnunciado";
            colEnunciado.ReadOnly = true;
            // 
            // colDisciplina
            // 
            colDisciplina.DataPropertyName = "DisciplinaDescricao";
            colDisciplina.FillWeight = 18F;
            colDisciplina.HeaderText = "Disciplina";
            colDisciplina.Name = "colDisciplina";
            colDisciplina.ReadOnly = true;
            // 
            // colAutor
            // 
            colAutor.DataPropertyName = "NomeUsuarioCadastro";
            colAutor.FillWeight = 16F;
            colAutor.HeaderText = "Cadastrada por";
            colAutor.Name = "colAutor";
            colAutor.ReadOnly = true;
            // 
            // colUtilizada
            // 
            colUtilizada.DataPropertyName = "UtilizadaDescricao";
            colUtilizada.FillWeight = 10F;
            colUtilizada.HeaderText = "Usada em simulado";
            colUtilizada.Name = "colUtilizada";
            colUtilizada.ReadOnly = true;
            // 
            // colSituacao
            // 
            colSituacao.DataPropertyName = "Situacao";
            colSituacao.FillWeight = 8F;
            colSituacao.HeaderText = "Situação";
            colSituacao.Name = "colSituacao";
            colSituacao.ReadOnly = true;
            // 
            // tmrBusca
            // 
            tmrBusca.Interval = 400;
            tmrBusca.Tick += tmrBusca_Tick;
            // 
            // label_modo
            // 
            label_modo.AccessibleName = "Modo da tela";
            label_modo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label_modo.BackColor = Color.FromArgb(239, 246, 255);
            label_modo.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            label_modo.ForeColor = Color.FromArgb(30, 58, 138);
            label_modo.Location = new Point(760, 8);
            label_modo.Size = new Size(240, 30);
            label_modo.TabIndex = 2;
            label_modo.Text = "Modo: Listagem";
            label_modo.TextAlign = ContentAlignment.MiddleCenter;
            label_modo.Name = "label_modo";
            // 
            // pnlLista
            // 
            pnlLista.Controls.Add(dgvQuestoes);
            pnlLista.Controls.Add(pnlLegenda);
            pnlLista.Controls.Add(pnlAcoes);
            pnlLista.Controls.Add(pnlFiltros);
            pnlLista.Dock = DockStyle.Fill;
            pnlLista.Location = new Point(0, 76);
            pnlLista.Size = new Size(1000, 564);
            pnlLista.TabIndex = 1;
            pnlLista.Name = "pnlLista";
            // 
            // pnlLegenda
            // 
            pnlLegenda.Controls.Add(label_lista);
            pnlLegenda.Controls.Add(label_totalRegistros);
            pnlLegenda.Dock = DockStyle.Top;
            pnlLegenda.Size = new Size(1000, 30);
            pnlLegenda.TabIndex = 3;
            pnlLegenda.Name = "pnlLegenda";
            // 
            // label_lista
            // 
            label_lista.ForeColor = Color.FromArgb(75, 85, 99);
            label_lista.Location = new Point(0, 4);
            label_lista.Size = new Size(680, 22);
            label_lista.TabIndex = 0;
            label_lista.Text = "Questões cadastradas. Dê duplo clique ou tecle Enter em uma questão para consultar e editar.";
            label_lista.Name = "label_lista";
            // 
            // label_id
            // 
            label_id.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label_id.Location = new Point(700, 12);
            label_id.Size = new Size(60, 36);
            label_id.TabIndex = 5;
            label_id.Text = "I&D";
            label_id.TextAlign = ContentAlignment.MiddleRight;
            label_id.Name = "label_id";
            // 
            // button_id
            // 
            button_id.AccessibleName = "ID do registro a consultar";
            button_id.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_id.Location = new Point(766, 18);
            button_id.MaxLength = 9;
            button_id.PlaceholderText = "nº";
            button_id.Size = new Size(84, 25);
            button_id.TabIndex = 6;
            button_id.Name = "button_id";
            // 
            // pnlCadastro
            // 
            pnlCadastro.Controls.Add(pnlCartao);
            pnlCadastro.AutoScroll = true;
            pnlCadastro.Dock = DockStyle.Fill;
            pnlCadastro.Location = new Point(0, 76);
            pnlCadastro.Size = new Size(1000, 564);
            pnlCadastro.TabIndex = 2;
            pnlCadastro.Visible = false;
            pnlCadastro.Name = "pnlCadastro";
            // 
            // pnlCartao
            // 
            pnlCartao.Controls.Add(btn_editar);
            pnlCartao.Controls.Add(btn_voltar);
            pnlCartao.Controls.Add(btn_incluir);
            pnlCartao.Controls.Add(lblObrigatorios);
            pnlCartao.Controls.Add(button_explicacaoResposta);
            pnlCartao.Controls.Add(label_explicacaoResposta);
            pnlCartao.Controls.Add(grpAlternativas);
            pnlCartao.Controls.Add(btnRemoverImagem);
            pnlCartao.Controls.Add(btn_selecionarImagem);
            pnlCartao.Controls.Add(button_imagemURL);
            pnlCartao.Controls.Add(picImagem);
            pnlCartao.Controls.Add(label_imagemURL);
            pnlCartao.Controls.Add(button_enunciado);
            pnlCartao.Controls.Add(label_enunciado);
            pnlCartao.Controls.Add(label_autor);
            pnlCartao.Controls.Add(button_disciplina);
            pnlCartao.Controls.Add(label_disciplina);
            pnlCartao.Controls.Add(pnlAvisoGabarito);
            pnlCartao.Controls.Add(lblRegistro);
            pnlCartao.Controls.Add(label_alternativaCorreta);
            pnlCartao.Controls.Add(button_alternativaCorreta);
            pnlCartao.Controls.Add(label_ativo);
            pnlCartao.Controls.Add(button_ativo);
            pnlCartao.BackColor = Color.White;
            pnlCartao.Location = new Point(0, 8);
            pnlCartao.Size = new Size(900, 788);
            pnlCartao.TabIndex = 0;
            pnlCartao.Name = "pnlCartao";
            // 
            // lblRegistro
            // 
            lblRegistro.Font = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point);
            lblRegistro.ForeColor = Color.FromArgb(37, 99, 235);
            lblRegistro.Location = new Point(24, 12);
            lblRegistro.Name = "lblRegistro";
            lblRegistro.Size = new Size(852, 36);
            lblRegistro.TabIndex = 0;
            lblRegistro.Text = "Nova questão";
            // 
            // pnlAvisoGabarito
            // 
            pnlAvisoGabarito.Controls.Add(label_avisoGabarito);
            pnlAvisoGabarito.AccessibleName = "Orientação";
            pnlAvisoGabarito.BackColor = Color.FromArgb(239, 246, 255);
            pnlAvisoGabarito.Location = new Point(24, 52);
            pnlAvisoGabarito.Name = "pnlAvisoGabarito";
            pnlAvisoGabarito.Padding = new Padding(12, 4, 12, 4);
            pnlAvisoGabarito.Size = new Size(852, 48);
            pnlAvisoGabarito.TabIndex = 1;
            // 
            // label_avisoGabarito
            // 
            label_avisoGabarito.Dock = DockStyle.Fill;
            label_avisoGabarito.ForeColor = Color.FromArgb(30, 58, 138);
            label_avisoGabarito.Location = new Point(12, 4);
            label_avisoGabarito.Name = "label_avisoGabarito";
            label_avisoGabarito.Size = new Size(828, 40);
            label_avisoGabarito.TabIndex = 0;
            label_avisoGabarito.Text = "";
            label_avisoGabarito.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label_disciplina
            // 
            label_disciplina.Location = new Point(24, 108);
            label_disciplina.Name = "label_disciplina";
            label_disciplina.Size = new Size(400, 22);
            label_disciplina.TabIndex = 2;
            label_disciplina.Text = "Disci&plina *";
            // 
            // button_disciplina
            // 
            button_disciplina.AccessibleName = "Disciplina, obrigatória";
            button_disciplina.DropDownStyle = ComboBoxStyle.DropDownList;
            button_disciplina.Location = new Point(24, 132);
            button_disciplina.Name = "button_disciplina";
            button_disciplina.Size = new Size(400, 25);
            button_disciplina.TabIndex = 3;
            // 
            // label_autor
            // 
            label_autor.ForeColor = Color.FromArgb(75, 85, 99);
            label_autor.Location = new Point(448, 135);
            label_autor.Name = "label_autor";
            label_autor.Size = new Size(428, 22);
            label_autor.TabIndex = 4;
            // 
            // label_enunciado
            // 
            label_enunciado.Location = new Point(24, 166);
            label_enunciado.Name = "label_enunciado";
            label_enunciado.Size = new Size(590, 22);
            label_enunciado.TabIndex = 5;
            label_enunciado.Text = "E&nunciado *";
            // 
            // button_enunciado
            // 
            button_enunciado.AcceptsReturn = true;
            button_enunciado.AccessibleName = "Enunciado, obrigatório";
            button_enunciado.Location = new Point(24, 190);
            button_enunciado.Multiline = true;
            button_enunciado.Name = "button_enunciado";
            button_enunciado.ScrollBars = ScrollBars.Vertical;
            button_enunciado.Size = new Size(590, 100);
            button_enunciado.TabIndex = 7;
            // 
            // label_imagemURL
            // 
            label_imagemURL.Location = new Point(640, 166);
            label_imagemURL.Name = "label_imagemURL";
            label_imagemURL.Size = new Size(236, 22);
            label_imagemURL.TabIndex = 6;
            label_imagemURL.Text = "Imagem (opcional): arraste o arquivo";
            // 
            // picImagem
            // 
            picImagem.AccessibleName = "Pré-visualização da imagem da questão";
            picImagem.AccessibleRole = AccessibleRole.Graphic;
            picImagem.BackColor = Color.FromArgb(248, 250, 252);
            picImagem.BorderStyle = BorderStyle.FixedSingle;
            picImagem.Location = new Point(640, 190);
            picImagem.Name = "picImagem";
            picImagem.Size = new Size(236, 100);
            picImagem.SizeMode = PictureBoxSizeMode.Zoom;
            picImagem.TabIndex = 8;
            picImagem.TabStop = false;
            // 
            // button_imagemURL
            // 
            button_imagemURL.AccessibleName = "Caminho do arquivo de imagem. Também é possível arrastar o arquivo para o quadro da imagem.";
            button_imagemURL.Location = new Point(640, 296);
            button_imagemURL.Name = "button_imagemURL";
            button_imagemURL.PlaceholderText = "ou cole o caminho do arquivo";
            button_imagemURL.Size = new Size(236, 25);
            button_imagemURL.TabIndex = 9;
            // 
            // btn_selecionarImagem
            // 
            btn_selecionarImagem.AccessibleName = "Usar a imagem do caminho informado";
            btn_selecionarImagem.Location = new Point(640, 328);
            btn_selecionarImagem.Name = "btn_selecionarImagem";
            btn_selecionarImagem.Size = new Size(116, 30);
            btn_selecionarImagem.TabIndex = 10;
            btn_selecionarImagem.Text = "Usar &imagem";
            btn_selecionarImagem.Click += btn_selecionarImagem_Click;
            // 
            // btnRemoverImagem
            // 
            btnRemoverImagem.AccessibleName = "Remover imagem da questão";
            btnRemoverImagem.Location = new Point(760, 328);
            btnRemoverImagem.Name = "btnRemoverImagem";
            btnRemoverImagem.Size = new Size(116, 30);
            btnRemoverImagem.TabIndex = 11;
            btnRemoverImagem.Text = "&Remover";
            btnRemoverImagem.Click += btnRemoverImagem_Click;
            // 
            // grpAlternativas
            // 
            grpAlternativas.Controls.Add(button_alternativaE);
            grpAlternativas.Controls.Add(label_letraE);
            grpAlternativas.Controls.Add(button_alternativaD);
            grpAlternativas.Controls.Add(label_letraD);
            grpAlternativas.Controls.Add(button_alternativaC);
            grpAlternativas.Controls.Add(label_letraC);
            grpAlternativas.Controls.Add(button_alternativaB);
            grpAlternativas.Controls.Add(label_letraB);
            grpAlternativas.Controls.Add(button_alternativaA);
            grpAlternativas.Controls.Add(label_letraA);
            grpAlternativas.AccessibleName = "Alternativas. Marque a alternativa correta.";
            grpAlternativas.Location = new Point(24, 366);
            grpAlternativas.Name = "grpAlternativas";
            grpAlternativas.Size = new Size(852, 222);
            grpAlternativas.TabIndex = 12;
            grpAlternativas.TabStop = false;
            grpAlternativas.Text = "Alternativas: escreva as cinco (A a E) *";
            // 
            // label_letraA
            // 
            label_letraA.AccessibleName = "Alternativa A";
            label_letraA.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            label_letraA.Location = new Point(16, 28);
            label_letraA.Name = "label_letraA";
            label_letraA.Size = new Size(52, 28);
            label_letraA.TabIndex = 0;
            label_letraA.Text = "A)";
            label_letraA.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // button_alternativaA
            // 
            button_alternativaA.AccessibleName = "Texto da alternativa A, obrigatório";
            button_alternativaA.Location = new Point(76, 29);
            button_alternativaA.Name = "button_alternativaA";
            button_alternativaA.Size = new Size(740, 25);
            button_alternativaA.TabIndex = 1;
            // 
            // label_letraB
            // 
            label_letraB.AccessibleName = "Alternativa B";
            label_letraB.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            label_letraB.Location = new Point(16, 66);
            label_letraB.Name = "label_letraB";
            label_letraB.Size = new Size(52, 28);
            label_letraB.TabIndex = 2;
            label_letraB.Text = "B)";
            label_letraB.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // button_alternativaB
            // 
            button_alternativaB.AccessibleName = "Texto da alternativa B, obrigatório";
            button_alternativaB.Location = new Point(76, 67);
            button_alternativaB.Name = "button_alternativaB";
            button_alternativaB.Size = new Size(740, 25);
            button_alternativaB.TabIndex = 3;
            // 
            // label_letraC
            // 
            label_letraC.AccessibleName = "Alternativa C";
            label_letraC.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            label_letraC.Location = new Point(16, 104);
            label_letraC.Name = "label_letraC";
            label_letraC.Size = new Size(52, 28);
            label_letraC.TabIndex = 4;
            label_letraC.Text = "C)";
            label_letraC.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // button_alternativaC
            // 
            button_alternativaC.AccessibleName = "Texto da alternativa C, obrigatório";
            button_alternativaC.Location = new Point(76, 105);
            button_alternativaC.Name = "button_alternativaC";
            button_alternativaC.Size = new Size(740, 25);
            button_alternativaC.TabIndex = 5;
            // 
            // label_letraD
            // 
            label_letraD.AccessibleName = "Alternativa D";
            label_letraD.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            label_letraD.Location = new Point(16, 142);
            label_letraD.Name = "label_letraD";
            label_letraD.Size = new Size(52, 28);
            label_letraD.TabIndex = 6;
            label_letraD.Text = "D)";
            label_letraD.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // button_alternativaD
            // 
            button_alternativaD.AccessibleName = "Texto da alternativa D, obrigatório";
            button_alternativaD.Location = new Point(76, 143);
            button_alternativaD.Name = "button_alternativaD";
            button_alternativaD.Size = new Size(740, 25);
            button_alternativaD.TabIndex = 7;
            // 
            // label_letraE
            // 
            label_letraE.AccessibleName = "Alternativa E";
            label_letraE.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            label_letraE.Location = new Point(16, 180);
            label_letraE.Name = "label_letraE";
            label_letraE.Size = new Size(52, 28);
            label_letraE.TabIndex = 8;
            label_letraE.Text = "E)";
            label_letraE.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // button_alternativaE
            // 
            button_alternativaE.AccessibleName = "Texto da alternativa E, obrigatório";
            button_alternativaE.Location = new Point(76, 181);
            button_alternativaE.Name = "button_alternativaE";
            button_alternativaE.Size = new Size(740, 25);
            button_alternativaE.TabIndex = 9;
            // 
            // label_explicacaoResposta
            // 
            label_explicacaoResposta.Location = new Point(24, 636);
            label_explicacaoResposta.Name = "label_explicacaoResposta";
            label_explicacaoResposta.Size = new Size(852, 22);
            label_explicacaoResposta.TabIndex = 15;
            label_explicacaoResposta.Text = "E&xplicação da resposta (opcional, exibida ao aluno na revisão)";
            // 
            // button_explicacaoResposta
            // 
            button_explicacaoResposta.AcceptsReturn = true;
            button_explicacaoResposta.AccessibleName = "Explicação da resposta, opcional";
            button_explicacaoResposta.Location = new Point(24, 660);
            button_explicacaoResposta.Multiline = true;
            button_explicacaoResposta.Name = "button_explicacaoResposta";
            button_explicacaoResposta.ScrollBars = ScrollBars.Vertical;
            button_explicacaoResposta.Size = new Size(852, 64);
            button_explicacaoResposta.TabIndex = 16;
            // 
            // lblObrigatorios
            // 
            lblObrigatorios.ForeColor = Color.FromArgb(75, 85, 99);
            lblObrigatorios.Location = new Point(240, 744);
            lblObrigatorios.Name = "lblObrigatorios";
            lblObrigatorios.Size = new Size(300, 22);
            lblObrigatorios.TabIndex = 22;
            lblObrigatorios.Text = "* campos obrigatórios";
            // 
            // btn_incluir
            // 
            btn_incluir.Location = new Point(636, 736);
            btn_incluir.Name = "btn_incluir";
            btn_incluir.Size = new Size(120, 38);
            btn_incluir.TabIndex = 19;
            btn_incluir.Text = "&Incluir";
            btn_incluir.Visible = false;
            btn_incluir.Click += btn_incluir_Click;
            // 
            // btn_voltar
            // 
            btn_voltar.Location = new Point(766, 736);
            btn_voltar.Name = "btn_voltar";
            btn_voltar.Size = new Size(110, 38);
            btn_voltar.TabIndex = 20;
            btn_voltar.Text = "&Voltar";
            btn_voltar.Click += btn_voltar_Click;
            // 
            // errorProvider
            // 
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            errorProvider.ContainerControl = this;
            // 
            // btn_editar
            // 
            btn_editar.Location = new Point(586, 736);
            btn_editar.Size = new Size(170, 38);
            btn_editar.TabIndex = 18;
            btn_editar.Text = "&Salvar alterações";
            btn_editar.Visible = false;
            btn_editar.Name = "btn_editar";
            btn_editar.Click += btn_editar_Click;
            // 
            // label_alternativaCorreta
            // 
            label_alternativaCorreta.Location = new Point(24, 602);
            label_alternativaCorreta.Size = new Size(190, 22);
            label_alternativaCorreta.TabIndex = 14;
            label_alternativaCorreta.Text = "Alternativa co&rreta *";
            label_alternativaCorreta.Name = "label_alternativaCorreta";
            // 
            // button_alternativaCorreta
            // 
            button_alternativaCorreta.AccessibleName = "Alternativa correta, obrigatória";
            button_alternativaCorreta.DropDownStyle = ComboBoxStyle.DropDownList;
            button_alternativaCorreta.Location = new Point(218, 599);
            button_alternativaCorreta.Size = new Size(160, 25);
            button_alternativaCorreta.TabIndex = 13;
            button_alternativaCorreta.Name = "button_alternativaCorreta";
            // 
            // label_ativo
            // 
            label_ativo.ForeColor = Color.FromArgb(75, 85, 99);
            label_ativo.Location = new Point(24, 744);
            label_ativo.Size = new Size(70, 22);
            label_ativo.TabIndex = 21;
            label_ativo.Text = "Situação:";
            label_ativo.Name = "label_ativo";
            // 
            // button_ativo
            // 
            button_ativo.AccessibleDescription = "A situação muda pelos botões Desativar e Reativar da lista";
            button_ativo.Enabled = false;
            button_ativo.Location = new Point(98, 742);
            button_ativo.Size = new Size(130, 26);
            button_ativo.TabIndex = 17;
            button_ativo.Text = "Ativa";
            button_ativo.Name = "button_ativo";
            // 
            // Form_Questao
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(245, 246, 250);
            ClientSize = new Size(1000, 640);
            KeyPreview = true;
            Controls.Add(pnlLista);
            Controls.Add(pnlCadastro);
            Controls.Add(pnlCabecalho);
            Name = "Form_Questao";
            pnlCabecalho.ResumeLayout(false);
            pnlCabecalho.PerformLayout();
            pnlFiltros.ResumeLayout(false);
            pnlFiltros.PerformLayout();
            pnlAcoes.ResumeLayout(false);
            pnlAcoes.PerformLayout();
            pnlLista.ResumeLayout(false);
            pnlLista.PerformLayout();
            pnlLegenda.ResumeLayout(false);
            pnlLegenda.PerformLayout();
            pnlCadastro.ResumeLayout(false);
            pnlCadastro.PerformLayout();
            pnlCartao.ResumeLayout(false);
            pnlCartao.PerformLayout();
            pnlAvisoGabarito.ResumeLayout(false);
            pnlAvisoGabarito.PerformLayout();
            grpAlternativas.ResumeLayout(false);
            grpAlternativas.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvQuestoes).EndInit();
            ((System.ComponentModel.ISupportInitialize)picImagem).EndInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlCabecalho;
        private Label label_titulo;
        private Label lblSubtitulo;
        private Panel pnlFiltros;
        private Label lblBusca;
        private TextBox txtBusca;
        private Label lblDisciplinaFiltro;
        private ComboBox cboDisciplinaFiltro;
        private Label lblSituacaoFiltro;
        private ComboBox cboSituacaoFiltro;
        private Button btnLimparFiltros;
        private Panel pnlAcoes;
        private Button btn_novo;
        private Button btn_consultarPorId;
        private Button btn_excluir;
        private Button btn_reativar;
        private Button btn_listar;
        private Label label_totalRegistros;
        private DataGridView dgvQuestoes;
        private DataGridViewTextBoxColumn colId;
        private DataGridViewTextBoxColumn colEnunciado;
        private DataGridViewTextBoxColumn colDisciplina;
        private DataGridViewTextBoxColumn colAutor;
        private DataGridViewTextBoxColumn colUtilizada;
        private DataGridViewTextBoxColumn colSituacao;
        private System.Windows.Forms.Timer tmrBusca;
        private Label label_modo;
        private Panel pnlLista;
        private Panel pnlLegenda;
        private Label label_lista;
        private Label label_id;
        private TextBox button_id;
        private Panel pnlCadastro;
        private Panel pnlCartao;
        private Label lblRegistro;
        private Panel pnlAvisoGabarito;
        private Label label_avisoGabarito;
        private Label label_disciplina;
        private ComboBox button_disciplina;
        private Label label_autor;
        private Label label_enunciado;
        private TextBox button_enunciado;
        private Label label_imagemURL;
        private PictureBox picImagem;
        private TextBox button_imagemURL;
        private Button btn_selecionarImagem;
        private Button btnRemoverImagem;
        private GroupBox grpAlternativas;
        private Label label_letraA;
        private TextBox button_alternativaA;
        private Label label_letraB;
        private TextBox button_alternativaB;
        private Label label_letraC;
        private TextBox button_alternativaC;
        private Label label_letraD;
        private TextBox button_alternativaD;
        private Label label_letraE;
        private TextBox button_alternativaE;
        private Label label_explicacaoResposta;
        private TextBox button_explicacaoResposta;
        private Label lblObrigatorios;
        private Button btn_incluir;
        private Button btn_voltar;
        private ErrorProvider errorProvider;
        private Button btn_editar;
        private Label label_alternativaCorreta;
        private ComboBox button_alternativaCorreta;
        private Label label_ativo;
        private CheckBox button_ativo;
    }
}
