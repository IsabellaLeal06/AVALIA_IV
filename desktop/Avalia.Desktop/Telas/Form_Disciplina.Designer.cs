namespace Avalia.Desktop.Telas
{
    partial class Form_Disciplina
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
            lblSituacaoFiltro = new Label();
            cboSituacaoFiltro = new ComboBox();
            btnLimparFiltros = new Button();
            pnlAcoes = new Panel();
            btn_novo = new Button();
            btn_consultar = new Button();
            btn_excluir = new Button();
            btn_reativar = new Button();
            btn_listar = new Button();
            label_totalRegistros = new Label();
            dgvDisciplinas = new DataGridView();
            colNome = new DataGridViewTextBoxColumn();
            colResponsavel = new DataGridViewTextBoxColumn();
            colQuestoes = new DataGridViewTextBoxColumn();
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
            label_descricao = new Label();
            button_descricao = new TextBox();
            label_usuarioId = new Label();
            button_usuarioId = new ComboBox();
            label_avisoProfessor = new Label();
            label_ativo = new Label();
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
            ((System.ComponentModel.ISupportInitialize)dgvDisciplinas).BeginInit();
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
            label_titulo.Text = "Disciplinas";
            // 
            // lblSubtitulo
            // 
            lblSubtitulo.ForeColor = Color.FromArgb(75, 85, 99);
            lblSubtitulo.Location = new Point(0, 42);
            lblSubtitulo.Name = "lblSubtitulo";
            lblSubtitulo.Size = new Size(980, 24);
            lblSubtitulo.TabIndex = 1;
            lblSubtitulo.Text = "Disciplinas e professor responsável. Uma disciplina pode ficar temporariamente sem responsável.";
            // 
            // pnlFiltros
            // 
            pnlFiltros.Controls.Add(btnLimparFiltros);
            pnlFiltros.Controls.Add(cboSituacaoFiltro);
            pnlFiltros.Controls.Add(lblSituacaoFiltro);
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
            lblBusca.Size = new Size(300, 22);
            lblBusca.TabIndex = 0;
            lblBusca.Text = "&Buscar por nome";
            // 
            // txtBusca
            // 
            txtBusca.AccessibleName = "Buscar por nome";
            txtBusca.Location = new Point(16, 32);
            txtBusca.Name = "txtBusca";
            txtBusca.Size = new Size(300, 25);
            txtBusca.TabIndex = 1;
            txtBusca.TextChanged += txtBusca_TextChanged;
            // 
            // lblSituacaoFiltro
            // 
            lblSituacaoFiltro.Location = new Point(332, 8);
            lblSituacaoFiltro.Name = "lblSituacaoFiltro";
            lblSituacaoFiltro.Size = new Size(130, 22);
            lblSituacaoFiltro.TabIndex = 2;
            lblSituacaoFiltro.Text = "S&ituação";
            // 
            // cboSituacaoFiltro
            // 
            cboSituacaoFiltro.AccessibleName = "Situação";
            cboSituacaoFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSituacaoFiltro.Location = new Point(332, 32);
            cboSituacaoFiltro.Name = "cboSituacaoFiltro";
            cboSituacaoFiltro.Size = new Size(130, 25);
            cboSituacaoFiltro.TabIndex = 3;
            cboSituacaoFiltro.SelectedIndexChanged += Filtro_Alterado;
            // 
            // btnLimparFiltros
            // 
            btnLimparFiltros.AccessibleName = "Limpar filtros";
            btnLimparFiltros.Location = new Point(478, 30);
            btnLimparFiltros.Name = "btnLimparFiltros";
            btnLimparFiltros.Size = new Size(130, 30);
            btnLimparFiltros.TabIndex = 4;
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
            pnlAcoes.Controls.Add(btn_consultar);
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
            // btn_consultar
            // 
            btn_consultar.AccessibleName = "Consultar o ID informado ou o registro selecionado";
            btn_consultar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_consultar.Location = new Point(860, 12);
            btn_consultar.Name = "btn_consultar";
            btn_consultar.Size = new Size(140, 36);
            btn_consultar.TabIndex = 7;
            btn_consultar.Text = "&Consultar";
            btn_consultar.Click += btn_consultar_Click;
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
            // dgvDisciplinas
            // 
            dgvDisciplinas.AccessibleName = "Lista de disciplinas";
            dgvDisciplinas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDisciplinas.Columns.AddRange(new DataGridViewColumn[] { colNome, colResponsavel, colQuestoes, colSituacao });
            dgvDisciplinas.Dock = DockStyle.Fill;
            dgvDisciplinas.Location = new Point(0, 208);
            dgvDisciplinas.Name = "dgvDisciplinas";
            dgvDisciplinas.Size = new Size(1000, 432);
            dgvDisciplinas.TabIndex = 3;
            dgvDisciplinas.CellDoubleClick += dgvDisciplinas_CellDoubleClick;
            dgvDisciplinas.SelectionChanged += dgvDisciplinas_SelectionChanged;
            dgvDisciplinas.KeyDown += dgvDisciplinas_KeyDown;
            // 
            // colNome
            // 
            colNome.DataPropertyName = "Nome";
            colNome.FillWeight = 36F;
            colNome.HeaderText = "Disciplina";
            colNome.Name = "colNome";
            colNome.ReadOnly = true;
            // 
            // colResponsavel
            // 
            colResponsavel.DataPropertyName = "Responsavel";
            colResponsavel.FillWeight = 30F;
            colResponsavel.HeaderText = "Professor responsável";
            colResponsavel.Name = "colResponsavel";
            colResponsavel.ReadOnly = true;
            // 
            // colQuestoes
            // 
            colQuestoes.DataPropertyName = "QuantidadeQuestoesAtivas";
            colQuestoes.FillWeight = 14F;
            colQuestoes.HeaderText = "Questões ativas";
            colQuestoes.Name = "colQuestoes";
            colQuestoes.ReadOnly = true;
            // 
            // colSituacao
            // 
            colSituacao.DataPropertyName = "Situacao";
            colSituacao.FillWeight = 10F;
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
            pnlLista.Controls.Add(dgvDisciplinas);
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
            label_lista.Text = "Disciplinas cadastradas. Dê duplo clique ou tecle Enter em um registro para consultar e editar.";
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
            pnlCartao.Controls.Add(label_ativo);
            pnlCartao.Controls.Add(label_avisoProfessor);
            pnlCartao.Controls.Add(button_usuarioId);
            pnlCartao.Controls.Add(label_usuarioId);
            pnlCartao.Controls.Add(button_descricao);
            pnlCartao.Controls.Add(label_descricao);
            pnlCartao.Controls.Add(button_nome);
            pnlCartao.Controls.Add(label_nome);
            pnlCartao.Controls.Add(lblRegistro);
            pnlCartao.BackColor = Color.White;
            pnlCartao.Location = new Point(0, 8);
            pnlCartao.Size = new Size(544, 430);
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
            lblRegistro.Text = "Nova disciplina";
            // 
            // label_nome
            // 
            label_nome.Location = new Point(24, 66);
            label_nome.Name = "label_nome";
            label_nome.Size = new Size(486, 22);
            label_nome.TabIndex = 1;
            label_nome.Text = "&Nome da disciplina *";
            // 
            // button_nome
            // 
            button_nome.AccessibleName = "Nome da disciplina, obrigatório";
            button_nome.Location = new Point(24, 90);
            button_nome.MaxLength = 100;
            button_nome.Name = "button_nome";
            button_nome.Size = new Size(486, 25);
            button_nome.TabIndex = 2;
            // 
            // label_descricao
            // 
            label_descricao.Location = new Point(24, 128);
            label_descricao.Name = "label_descricao";
            label_descricao.Size = new Size(486, 22);
            label_descricao.TabIndex = 3;
            label_descricao.Text = "&Descrição (opcional)";
            // 
            // button_descricao
            // 
            button_descricao.AcceptsReturn = true;
            button_descricao.AccessibleName = "Descrição, opcional";
            button_descricao.Location = new Point(24, 152);
            button_descricao.MaxLength = 500;
            button_descricao.Multiline = true;
            button_descricao.Name = "button_descricao";
            button_descricao.ScrollBars = ScrollBars.Vertical;
            button_descricao.Size = new Size(486, 96);
            button_descricao.TabIndex = 4;
            // 
            // label_usuarioId
            // 
            label_usuarioId.Location = new Point(24, 262);
            label_usuarioId.Name = "label_usuarioId";
            label_usuarioId.Size = new Size(486, 22);
            label_usuarioId.TabIndex = 5;
            label_usuarioId.Text = "&Professor responsável";
            // 
            // button_usuarioId
            // 
            button_usuarioId.AccessibleName = "Professor responsável";
            button_usuarioId.DropDownStyle = ComboBoxStyle.DropDownList;
            button_usuarioId.Location = new Point(24, 286);
            button_usuarioId.Name = "button_usuarioId";
            button_usuarioId.Size = new Size(330, 25);
            button_usuarioId.TabIndex = 6;
            // 
            // label_avisoProfessor
            // 
            label_avisoProfessor.ForeColor = Color.FromArgb(75, 85, 99);
            label_avisoProfessor.Location = new Point(24, 318);
            label_avisoProfessor.Name = "label_avisoProfessor";
            label_avisoProfessor.Size = new Size(486, 40);
            label_avisoProfessor.TabIndex = 7;
            label_avisoProfessor.Text = "Somente professores ativos são listados. A disciplina pode ficar temporariamente sem responsável.";
            // 
            // label_ativo
            // 
            label_ativo.ForeColor = Color.FromArgb(75, 85, 99);
            label_ativo.Location = new Point(24, 380);
            label_ativo.Name = "label_ativo";
            label_ativo.Size = new Size(70, 22);
            label_ativo.TabIndex = 8;
            label_ativo.Text = "Situação:";
            // 
            // btn_incluir
            // 
            btn_incluir.Location = new Point(270, 372);
            btn_incluir.Name = "btn_incluir";
            btn_incluir.Size = new Size(120, 38);
            btn_incluir.TabIndex = 11;
            btn_incluir.Text = "&Incluir";
            btn_incluir.Visible = false;
            btn_incluir.Click += btn_incluir_Click;
            // 
            // btn_voltar
            // 
            btn_voltar.Location = new Point(400, 372);
            btn_voltar.Name = "btn_voltar";
            btn_voltar.Size = new Size(110, 38);
            btn_voltar.TabIndex = 12;
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
            btn_editar.Location = new Point(220, 372);
            btn_editar.Size = new Size(170, 38);
            btn_editar.TabIndex = 10;
            btn_editar.Text = "&Salvar alterações";
            btn_editar.Visible = false;
            btn_editar.Name = "btn_editar";
            btn_editar.Click += btn_editar_Click;
            // 
            // button_ativo
            // 
            button_ativo.AccessibleDescription = "A situação muda pelos botões Desativar e Reativar da lista";
            button_ativo.Enabled = false;
            button_ativo.Location = new Point(98, 378);
            button_ativo.Size = new Size(150, 26);
            button_ativo.TabIndex = 9;
            button_ativo.Text = "Ativo";
            button_ativo.Name = "button_ativo";
            // 
            // Form_Disciplina
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(245, 246, 250);
            ClientSize = new Size(1000, 640);
            KeyPreview = true;
            Controls.Add(pnlLista);
            Controls.Add(pnlCadastro);
            Controls.Add(pnlCabecalho);
            Name = "Form_Disciplina";
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
            ((System.ComponentModel.ISupportInitialize)dgvDisciplinas).EndInit();
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
        private Label lblSituacaoFiltro;
        private ComboBox cboSituacaoFiltro;
        private Button btnLimparFiltros;
        private Panel pnlAcoes;
        private Button btn_novo;
        private Button btn_consultar;
        private Button btn_excluir;
        private Button btn_reativar;
        private Button btn_listar;
        private Label label_totalRegistros;
        private DataGridView dgvDisciplinas;
        private DataGridViewTextBoxColumn colNome;
        private DataGridViewTextBoxColumn colResponsavel;
        private DataGridViewTextBoxColumn colQuestoes;
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
        private Label label_descricao;
        private TextBox button_descricao;
        private Label label_usuarioId;
        private ComboBox button_usuarioId;
        private Label label_avisoProfessor;
        private Label label_ativo;
        private Button btn_incluir;
        private Button btn_voltar;
        private ErrorProvider errorProvider;
        private Button btn_editar;
        private CheckBox button_ativo;
    }
}
