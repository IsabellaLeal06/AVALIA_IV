namespace Avalia.Desktop.Telas
{
    partial class Form_Usuario
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
            lblPerfilFiltro = new Label();
            cboPerfilFiltro = new ComboBox();
            lblTurmaFiltro = new Label();
            cboTurmaFiltro = new ComboBox();
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
            dgvUsuarios = new DataGridView();
            colNome = new DataGridViewTextBoxColumn();
            colEmail = new DataGridViewTextBoxColumn();
            colPerfil = new DataGridViewTextBoxColumn();
            colTurma = new DataGridViewTextBoxColumn();
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
            label_nome = new Label();
            button_nome = new TextBox();
            label_CPF = new Label();
            button_CPF = new MaskedTextBox();
            label_dataIngresso = new Label();
            button_dataIngresso = new DateTimePicker();
            label_email = new Label();
            button_email = new TextBox();
            label_perfilId = new Label();
            button_perfilId = new ComboBox();
            label_turmaId = new Label();
            button_turmaId = new ComboBox();
            label_ativo = new Label();
            pnlSenha = new Panel();
            label_senha = new Label();
            lblObrigatorios = new Label();
            btn_incluir = new Button();
            btn_voltar = new Button();
            errorProvider = new ErrorProvider(components);
            btn_editar = new Button();
            button_ativo = new CheckBox();
            pnlCabecalho.SuspendLayout();
            pnlFiltros.SuspendLayout();
            pnlAcoes.SuspendLayout();
            pnlLista.SuspendLayout();
            pnlLegenda.SuspendLayout();
            pnlCadastro.SuspendLayout();
            pnlCartao.SuspendLayout();
            pnlSenha.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvUsuarios).BeginInit();
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
            label_titulo.Text = "Usuários";
            // 
            // lblSubtitulo
            // 
            lblSubtitulo.ForeColor = Color.FromArgb(75, 85, 99);
            lblSubtitulo.Location = new Point(0, 42);
            lblSubtitulo.Name = "lblSubtitulo";
            lblSubtitulo.Size = new Size(980, 24);
            lblSubtitulo.TabIndex = 1;
            lblSubtitulo.Text = "Contas de alunos, professores e administradores. O CPF não é exibido na listagem.";
            // 
            // pnlFiltros
            // 
            pnlFiltros.Controls.Add(btnLimparFiltros);
            pnlFiltros.Controls.Add(cboSituacaoFiltro);
            pnlFiltros.Controls.Add(lblSituacaoFiltro);
            pnlFiltros.Controls.Add(cboTurmaFiltro);
            pnlFiltros.Controls.Add(lblTurmaFiltro);
            pnlFiltros.Controls.Add(cboPerfilFiltro);
            pnlFiltros.Controls.Add(lblPerfilFiltro);
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
            lblBusca.Size = new Size(260, 22);
            lblBusca.TabIndex = 0;
            lblBusca.Text = "&Buscar por nome ou e-mail";
            // 
            // txtBusca
            // 
            txtBusca.AccessibleName = "Buscar por nome ou e-mail";
            txtBusca.Location = new Point(16, 32);
            txtBusca.Name = "txtBusca";
            txtBusca.Size = new Size(260, 25);
            txtBusca.TabIndex = 1;
            txtBusca.TextChanged += txtBusca_TextChanged;
            // 
            // lblPerfilFiltro
            // 
            lblPerfilFiltro.Location = new Point(292, 8);
            lblPerfilFiltro.Name = "lblPerfilFiltro";
            lblPerfilFiltro.Size = new Size(160, 22);
            lblPerfilFiltro.TabIndex = 2;
            lblPerfilFiltro.Text = "Per&fil";
            // 
            // cboPerfilFiltro
            // 
            cboPerfilFiltro.AccessibleName = "Perfil";
            cboPerfilFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPerfilFiltro.Location = new Point(292, 32);
            cboPerfilFiltro.Name = "cboPerfilFiltro";
            cboPerfilFiltro.Size = new Size(160, 25);
            cboPerfilFiltro.TabIndex = 3;
            cboPerfilFiltro.SelectedIndexChanged += Filtro_Alterado;
            // 
            // lblTurmaFiltro
            // 
            lblTurmaFiltro.Location = new Point(468, 8);
            lblTurmaFiltro.Name = "lblTurmaFiltro";
            lblTurmaFiltro.Size = new Size(240, 22);
            lblTurmaFiltro.TabIndex = 4;
            lblTurmaFiltro.Text = "Tur&ma";
            // 
            // cboTurmaFiltro
            // 
            cboTurmaFiltro.AccessibleName = "Turma";
            cboTurmaFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTurmaFiltro.Location = new Point(468, 32);
            cboTurmaFiltro.Name = "cboTurmaFiltro";
            cboTurmaFiltro.Size = new Size(240, 25);
            cboTurmaFiltro.TabIndex = 5;
            cboTurmaFiltro.SelectedIndexChanged += Filtro_Alterado;
            // 
            // lblSituacaoFiltro
            // 
            lblSituacaoFiltro.Location = new Point(724, 8);
            lblSituacaoFiltro.Name = "lblSituacaoFiltro";
            lblSituacaoFiltro.Size = new Size(130, 22);
            lblSituacaoFiltro.TabIndex = 6;
            lblSituacaoFiltro.Text = "S&ituação";
            // 
            // cboSituacaoFiltro
            // 
            cboSituacaoFiltro.AccessibleName = "Situação";
            cboSituacaoFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSituacaoFiltro.Location = new Point(724, 32);
            cboSituacaoFiltro.Name = "cboSituacaoFiltro";
            cboSituacaoFiltro.Size = new Size(130, 25);
            cboSituacaoFiltro.TabIndex = 7;
            cboSituacaoFiltro.SelectedIndexChanged += Filtro_Alterado;
            // 
            // btnLimparFiltros
            // 
            btnLimparFiltros.AccessibleName = "Limpar filtros";
            btnLimparFiltros.Location = new Point(870, 30);
            btnLimparFiltros.Name = "btnLimparFiltros";
            btnLimparFiltros.Size = new Size(130, 30);
            btnLimparFiltros.TabIndex = 8;
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
            btn_novo.Text = "&Novo";
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
            // dgvUsuarios
            // 
            dgvUsuarios.AccessibleName = "Lista de usuários";
            dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsuarios.Columns.AddRange(new DataGridViewColumn[] { colNome, colEmail, colPerfil, colTurma, colSituacao });
            dgvUsuarios.Dock = DockStyle.Fill;
            dgvUsuarios.Location = new Point(0, 208);
            dgvUsuarios.Name = "dgvUsuarios";
            dgvUsuarios.Size = new Size(1000, 432);
            dgvUsuarios.TabIndex = 3;
            dgvUsuarios.CellDoubleClick += dgvUsuarios_CellDoubleClick;
            dgvUsuarios.SelectionChanged += dgvUsuarios_SelectionChanged;
            dgvUsuarios.KeyDown += dgvUsuarios_KeyDown;
            // 
            // colNome
            // 
            colNome.DataPropertyName = "Nome";
            colNome.FillWeight = 30F;
            colNome.HeaderText = "Nome";
            colNome.Name = "colNome";
            colNome.ReadOnly = true;
            // 
            // colEmail
            // 
            colEmail.DataPropertyName = "Email";
            colEmail.FillWeight = 30F;
            colEmail.HeaderText = "E-mail";
            colEmail.Name = "colEmail";
            colEmail.ReadOnly = true;
            // 
            // colPerfil
            // 
            colPerfil.DataPropertyName = "PerfilDescricao";
            colPerfil.FillWeight = 12F;
            colPerfil.HeaderText = "Perfil";
            colPerfil.Name = "colPerfil";
            colPerfil.ReadOnly = true;
            // 
            // colTurma
            // 
            colTurma.DataPropertyName = "TurmaDescricao";
            colTurma.FillWeight = 20F;
            colTurma.HeaderText = "Turma";
            colTurma.Name = "colTurma";
            colTurma.ReadOnly = true;
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
            pnlLista.Controls.Add(dgvUsuarios);
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
            label_lista.Text = "Usuários cadastrados. Dê duplo clique ou tecle Enter em um registro para consultar e editar.";
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
            pnlCartao.Controls.Add(button_ativo);
            pnlCartao.Controls.Add(btn_editar);
            pnlCartao.Controls.Add(btn_voltar);
            pnlCartao.Controls.Add(btn_incluir);
            pnlCartao.Controls.Add(lblObrigatorios);
            pnlCartao.Controls.Add(pnlSenha);
            pnlCartao.Controls.Add(label_ativo);
            pnlCartao.Controls.Add(button_turmaId);
            pnlCartao.Controls.Add(label_turmaId);
            pnlCartao.Controls.Add(button_perfilId);
            pnlCartao.Controls.Add(label_perfilId);
            pnlCartao.Controls.Add(button_email);
            pnlCartao.Controls.Add(label_email);
            pnlCartao.Controls.Add(button_dataIngresso);
            pnlCartao.Controls.Add(label_dataIngresso);
            pnlCartao.Controls.Add(button_CPF);
            pnlCartao.Controls.Add(label_CPF);
            pnlCartao.Controls.Add(button_nome);
            pnlCartao.Controls.Add(label_nome);
            pnlCartao.Controls.Add(lblRegistro);
            pnlCartao.BackColor = Color.White;
            pnlCartao.Location = new Point(0, 8);
            pnlCartao.Size = new Size(544, 472);
            pnlCartao.TabIndex = 0;
            pnlCartao.Name = "pnlCartao";
            // 
            // lblRegistro
            // 
            lblRegistro.Font = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point);
            lblRegistro.ForeColor = Color.FromArgb(37, 99, 235);
            lblRegistro.Location = new Point(24, 16);
            lblRegistro.Name = "lblRegistro";
            lblRegistro.Size = new Size(510, 36);
            lblRegistro.TabIndex = 0;
            lblRegistro.Text = "Novo usuário";
            // 
            // label_nome
            // 
            label_nome.Location = new Point(24, 66);
            label_nome.Name = "label_nome";
            label_nome.Size = new Size(510, 22);
            label_nome.TabIndex = 1;
            label_nome.Text = "&Nome completo *";
            // 
            // button_nome
            // 
            button_nome.AccessibleName = "Nome completo, obrigatório";
            button_nome.Location = new Point(24, 90);
            button_nome.MaxLength = 150;
            button_nome.Name = "button_nome";
            button_nome.Size = new Size(486, 25);
            button_nome.TabIndex = 2;
            // 
            // label_CPF
            // 
            label_CPF.Location = new Point(24, 128);
            label_CPF.Name = "label_CPF";
            label_CPF.Size = new Size(220, 22);
            label_CPF.TabIndex = 3;
            label_CPF.Text = "&CPF *";
            // 
            // button_CPF
            // 
            button_CPF.AccessibleName = "CPF, obrigatório";
            button_CPF.Location = new Point(24, 152);
            button_CPF.Mask = "000\\.000\\.000-00";
            button_CPF.Name = "button_CPF";
            button_CPF.Size = new Size(200, 25);
            button_CPF.TabIndex = 5;
            button_CPF.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            // 
            // label_dataIngresso
            // 
            label_dataIngresso.Location = new Point(264, 128);
            label_dataIngresso.Name = "label_dataIngresso";
            label_dataIngresso.Size = new Size(246, 22);
            label_dataIngresso.TabIndex = 4;
            label_dataIngresso.Text = "Data de &ingresso *";
            // 
            // button_dataIngresso
            // 
            button_dataIngresso.AccessibleName = "Data de ingresso, obrigatória";
            button_dataIngresso.CustomFormat = "dd/MM/yyyy";
            button_dataIngresso.Format = DateTimePickerFormat.Custom;
            button_dataIngresso.Location = new Point(264, 152);
            button_dataIngresso.Name = "button_dataIngresso";
            button_dataIngresso.Size = new Size(200, 25);
            button_dataIngresso.TabIndex = 6;
            // 
            // label_email
            // 
            label_email.Location = new Point(24, 190);
            label_email.Name = "label_email";
            label_email.Size = new Size(510, 22);
            label_email.TabIndex = 7;
            label_email.Text = "&E-mail *";
            // 
            // button_email
            // 
            button_email.AccessibleName = "E-mail, obrigatório";
            button_email.Location = new Point(24, 214);
            button_email.MaxLength = 150;
            button_email.Name = "button_email";
            button_email.Size = new Size(486, 25);
            button_email.TabIndex = 8;
            // 
            // label_perfilId
            // 
            label_perfilId.Location = new Point(24, 252);
            label_perfilId.Name = "label_perfilId";
            label_perfilId.Size = new Size(220, 22);
            label_perfilId.TabIndex = 9;
            label_perfilId.Text = "&Perfil *";
            // 
            // button_perfilId
            // 
            button_perfilId.AccessibleName = "Perfil, obrigatório";
            button_perfilId.DropDownStyle = ComboBoxStyle.DropDownList;
            button_perfilId.Location = new Point(24, 276);
            button_perfilId.Name = "button_perfilId";
            button_perfilId.Size = new Size(200, 25);
            button_perfilId.TabIndex = 11;
            button_perfilId.SelectedIndexChanged += button_perfilId_SelectedIndexChanged;
            // 
            // label_turmaId
            // 
            label_turmaId.Location = new Point(264, 252);
            label_turmaId.Name = "label_turmaId";
            label_turmaId.Size = new Size(246, 22);
            label_turmaId.TabIndex = 10;
            label_turmaId.Text = "&Turma * (somente aluno)";
            // 
            // button_turmaId
            // 
            button_turmaId.AccessibleName = "Turma, obrigatória para aluno";
            button_turmaId.DropDownStyle = ComboBoxStyle.DropDownList;
            button_turmaId.Location = new Point(264, 276);
            button_turmaId.Name = "button_turmaId";
            button_turmaId.Size = new Size(246, 25);
            button_turmaId.TabIndex = 12;
            // 
            // label_ativo
            // 
            label_ativo.ForeColor = Color.FromArgb(75, 85, 99);
            label_ativo.Location = new Point(24, 314);
            label_ativo.Name = "label_ativo";
            label_ativo.Size = new Size(70, 22);
            label_ativo.TabIndex = 13;
            label_ativo.Text = "Situação:";
            // 
            // pnlSenha
            // 
            pnlSenha.Controls.Add(label_senha);
            pnlSenha.BackColor = Color.FromArgb(239, 246, 255);
            pnlSenha.Location = new Point(24, 342);
            pnlSenha.Name = "pnlSenha";
            pnlSenha.Padding = new Padding(10, 6, 10, 6);
            pnlSenha.Size = new Size(486, 56);
            pnlSenha.TabIndex = 15;
            // 
            // label_senha
            // 
            label_senha.Dock = DockStyle.Fill;
            label_senha.ForeColor = Color.FromArgb(30, 58, 138);
            label_senha.Location = new Point(10, 6);
            label_senha.Name = "label_senha";
            label_senha.Size = new Size(466, 44);
            label_senha.TabIndex = 0;
            label_senha.Text = "A senha inicial será o CPF (somente números). O usuário poderá alterá-la depois do primeiro acesso.";
            label_senha.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblObrigatorios
            // 
            lblObrigatorios.ForeColor = Color.FromArgb(75, 85, 99);
            lblObrigatorios.Location = new Point(24, 422);
            lblObrigatorios.Name = "lblObrigatorios";
            lblObrigatorios.Size = new Size(220, 22);
            lblObrigatorios.TabIndex = 19;
            lblObrigatorios.Text = "* campos obrigatórios";
            // 
            // btn_incluir
            // 
            btn_incluir.Location = new Point(270, 414);
            btn_incluir.Name = "btn_incluir";
            btn_incluir.Size = new Size(120, 38);
            btn_incluir.TabIndex = 17;
            btn_incluir.Text = "&Incluir";
            btn_incluir.Visible = false;
            btn_incluir.Click += btn_incluir_Click;
            // 
            // btn_voltar
            // 
            btn_voltar.Location = new Point(400, 414);
            btn_voltar.Name = "btn_voltar";
            btn_voltar.Size = new Size(110, 38);
            btn_voltar.TabIndex = 18;
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
            btn_editar.Location = new Point(220, 414);
            btn_editar.Size = new Size(170, 38);
            btn_editar.TabIndex = 16;
            btn_editar.Text = "&Salvar alterações";
            btn_editar.Visible = false;
            btn_editar.Name = "btn_editar";
            btn_editar.Click += btn_editar_Click;
            // 
            // button_ativo
            // 
            button_ativo.AccessibleDescription = "A situação muda pelos botões Desativar e Reativar da lista";
            button_ativo.Enabled = false;
            button_ativo.Location = new Point(98, 312);
            button_ativo.Size = new Size(150, 26);
            button_ativo.TabIndex = 14;
            button_ativo.Text = "Ativo";
            button_ativo.Name = "button_ativo";
            // 
            // Form_Usuario
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(245, 246, 250);
            ClientSize = new Size(1000, 640);
            KeyPreview = true;
            Controls.Add(pnlLista);
            Controls.Add(pnlCadastro);
            Controls.Add(pnlCabecalho);
            Name = "Form_Usuario";
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
            pnlSenha.ResumeLayout(false);
            pnlSenha.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvUsuarios).EndInit();
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
        private Label lblPerfilFiltro;
        private ComboBox cboPerfilFiltro;
        private Label lblTurmaFiltro;
        private ComboBox cboTurmaFiltro;
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
        private DataGridView dgvUsuarios;
        private DataGridViewTextBoxColumn colNome;
        private DataGridViewTextBoxColumn colEmail;
        private DataGridViewTextBoxColumn colPerfil;
        private DataGridViewTextBoxColumn colTurma;
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
        private Label label_nome;
        private TextBox button_nome;
        private Label label_CPF;
        private MaskedTextBox button_CPF;
        private Label label_dataIngresso;
        private DateTimePicker button_dataIngresso;
        private Label label_email;
        private TextBox button_email;
        private Label label_perfilId;
        private ComboBox button_perfilId;
        private Label label_turmaId;
        private ComboBox button_turmaId;
        private Label label_ativo;
        private Panel pnlSenha;
        private Label label_senha;
        private Label lblObrigatorios;
        private Button btn_incluir;
        private Button btn_voltar;
        private ErrorProvider errorProvider;
        private Button btn_editar;
        private CheckBox button_ativo;
    }
}
