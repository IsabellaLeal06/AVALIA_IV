namespace Avalia.Desktop.Telas
{
    partial class Form_Medalha
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
            pnlLista = new Panel();
            pnlCadastro = new Panel();
            pnlCabecalho = new Panel();
            label_titulo = new Label();
            lblSubtitulo = new Label();
            label_modo = new Label();
            dgvMedalhas = new DataGridView();
            colNome = new DataGridViewTextBoxColumn();
            colFaixa = new DataGridViewTextBoxColumn();
            colPontos = new DataGridViewTextBoxColumn();
            colAlunos = new DataGridViewTextBoxColumn();
            colImagem = new DataGridViewTextBoxColumn();
            pnlLegenda = new Panel();
            label_lista = new Label();
            label_totalRegistros = new Label();
            pnlAcoes = new Panel();
            btn_novo = new Button();
            btn_excluir = new Button();
            btn_listar = new Button();
            label_id = new Label();
            button_id = new TextBox();
            btn_consultarPorId = new Button();
            pnlResumo = new Panel();
            label_qtdAlunos = new Label();
            pnlCartao = new Panel();
            lblRegistro = new Label();
            label_nome = new Label();
            button_nome = new TextBox();
            label_pontosMinimos = new Label();
            button_pontosMinimos = new NumericUpDown();
            label_imagemURL = new Label();
            button_imagemURL = new TextBox();
            btn_selecionarImagem = new Button();
            picImagem = new PictureBox();
            label_dicaImagem = new Label();
            lblFaixa = new Label();
            lblObrigatorios = new Label();
            btn_incluir = new Button();
            btn_editar = new Button();
            btn_voltar = new Button();
            errorProvider = new ErrorProvider(components);
            pnlLista.SuspendLayout();
            pnlCadastro.SuspendLayout();
            pnlCabecalho.SuspendLayout();
            pnlLegenda.SuspendLayout();
            pnlAcoes.SuspendLayout();
            pnlResumo.SuspendLayout();
            pnlCartao.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMedalhas).BeginInit();
            ((System.ComponentModel.ISupportInitialize)button_pontosMinimos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picImagem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
            SuspendLayout();
            // 
            // pnlLista
            // 
            pnlLista.Controls.Add(dgvMedalhas);
            pnlLista.Controls.Add(pnlLegenda);
            pnlLista.Controls.Add(pnlAcoes);
            pnlLista.Controls.Add(pnlResumo);
            pnlLista.Dock = DockStyle.Fill;
            pnlLista.Location = new Point(0, 76);
            pnlLista.Size = new Size(1000, 564);
            pnlLista.TabIndex = 1;
            pnlLista.Name = "pnlLista";
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
            // pnlCabecalho
            // 
            pnlCabecalho.Controls.Add(label_titulo);
            pnlCabecalho.Controls.Add(lblSubtitulo);
            pnlCabecalho.Controls.Add(label_modo);
            pnlCabecalho.Dock = DockStyle.Top;
            pnlCabecalho.Size = new Size(1000, 76);
            pnlCabecalho.TabIndex = 0;
            pnlCabecalho.Name = "pnlCabecalho";
            // 
            // label_titulo
            // 
            label_titulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            label_titulo.ForeColor = Color.FromArgb(17, 24, 39);
            label_titulo.Location = new Point(0, 0);
            label_titulo.Size = new Size(740, 40);
            label_titulo.TabIndex = 0;
            label_titulo.Text = "Medalhas";
            label_titulo.Name = "label_titulo";
            // 
            // lblSubtitulo
            // 
            lblSubtitulo.ForeColor = Color.FromArgb(75, 85, 99);
            lblSubtitulo.Location = new Point(0, 42);
            lblSubtitulo.Size = new Size(980, 24);
            lblSubtitulo.TabIndex = 1;
            lblSubtitulo.Text = "Faixas de pontuação da gamificação (RN22). A medalha do aluno é apurada pela pontuação, sem alterar as aplicações.";
            lblSubtitulo.Name = "lblSubtitulo";
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
            // dgvMedalhas
            // 
            dgvMedalhas.AccessibleName = "Lista de medalhas";
            dgvMedalhas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMedalhas.Columns.AddRange(new DataGridViewColumn[] { colNome, colFaixa, colPontos, colAlunos, colImagem });
            dgvMedalhas.Dock = DockStyle.Fill;
            dgvMedalhas.TabIndex = 3;
            dgvMedalhas.Name = "dgvMedalhas";
            dgvMedalhas.CellDoubleClick += dgvMedalhas_CellDoubleClick;
            dgvMedalhas.SelectionChanged += dgvMedalhas_SelectionChanged;
            dgvMedalhas.KeyDown += dgvMedalhas_KeyDown;
            // 
            // colNome
            // 
            colNome.DataPropertyName = "Nome";
            colNome.FillWeight = 18F;
            colNome.HeaderText = "Medalha";
            colNome.ReadOnly = true;
            colNome.Name = "colNome";
            // 
            // colFaixa
            // 
            colFaixa.DataPropertyName = "Faixa";
            colFaixa.FillWeight = 26F;
            colFaixa.HeaderText = "Faixa de pontuação";
            colFaixa.ReadOnly = true;
            colFaixa.Name = "colFaixa";
            // 
            // colPontos
            // 
            colPontos.DataPropertyName = "PontosMinimos";
            colPontos.FillWeight = 14F;
            colPontos.HeaderText = "Pontos mínimos";
            colPontos.ReadOnly = true;
            colPontos.Name = "colPontos";
            // 
            // colAlunos
            // 
            colAlunos.DataPropertyName = "QtdAlunos";
            colAlunos.FillWeight = 14F;
            colAlunos.HeaderText = "Alunos na faixa";
            colAlunos.ReadOnly = true;
            colAlunos.Name = "colAlunos";
            // 
            // colImagem
            // 
            colImagem.DataPropertyName = "ImagemUrl";
            colImagem.FillWeight = 28F;
            colImagem.HeaderText = "Imagem";
            colImagem.ReadOnly = true;
            colImagem.Name = "colImagem";
            // 
            // pnlLegenda
            // 
            pnlLegenda.Controls.Add(label_lista);
            pnlLegenda.Controls.Add(label_totalRegistros);
            pnlLegenda.Dock = DockStyle.Top;
            pnlLegenda.Size = new Size(1000, 30);
            pnlLegenda.TabIndex = 2;
            pnlLegenda.Name = "pnlLegenda";
            // 
            // label_lista
            // 
            label_lista.ForeColor = Color.FromArgb(75, 85, 99);
            label_lista.Location = new Point(0, 4);
            label_lista.Size = new Size(680, 22);
            label_lista.TabIndex = 0;
            label_lista.Text = "Faixas cadastradas, da menor para a maior. Dê duplo clique ou tecle Enter para consultar e editar.";
            label_lista.Name = "label_lista";
            // 
            // label_totalRegistros
            // 
            label_totalRegistros.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label_totalRegistros.ForeColor = Color.FromArgb(75, 85, 99);
            label_totalRegistros.Location = new Point(700, 4);
            label_totalRegistros.Size = new Size(300, 22);
            label_totalRegistros.TabIndex = 1;
            label_totalRegistros.TextAlign = ContentAlignment.MiddleRight;
            label_totalRegistros.Name = "label_totalRegistros";
            // 
            // pnlAcoes
            // 
            pnlAcoes.Controls.Add(btn_novo);
            pnlAcoes.Controls.Add(btn_excluir);
            pnlAcoes.Controls.Add(btn_listar);
            pnlAcoes.Controls.Add(label_id);
            pnlAcoes.Controls.Add(button_id);
            pnlAcoes.Controls.Add(btn_consultarPorId);
            pnlAcoes.Dock = DockStyle.Top;
            pnlAcoes.Size = new Size(1000, 60);
            pnlAcoes.TabIndex = 1;
            pnlAcoes.Name = "pnlAcoes";
            // 
            // btn_novo
            // 
            btn_novo.AccessibleName = "Cadastrar nova medalha";
            btn_novo.Location = new Point(0, 12);
            btn_novo.Size = new Size(110, 36);
            btn_novo.TabIndex = 0;
            btn_novo.Text = "&Nova";
            btn_novo.Name = "btn_novo";
            btn_novo.Click += btn_novo_Click;
            // 
            // btn_excluir
            // 
            btn_excluir.AccessibleName = "Excluir medalha selecionada";
            btn_excluir.Location = new Point(120, 12);
            btn_excluir.Size = new Size(130, 36);
            btn_excluir.TabIndex = 1;
            btn_excluir.Text = "E&xcluir";
            btn_excluir.Name = "btn_excluir";
            btn_excluir.Click += btn_excluir_Click;
            // 
            // btn_listar
            // 
            btn_listar.AccessibleName = "Listar novamente";
            btn_listar.Location = new Point(260, 12);
            btn_listar.Size = new Size(130, 36);
            btn_listar.TabIndex = 2;
            btn_listar.Text = "&Listar (F5)";
            btn_listar.Name = "btn_listar";
            btn_listar.Click += btn_listar_Click;
            // 
            // label_id
            // 
            label_id.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label_id.Location = new Point(700, 12);
            label_id.Size = new Size(60, 36);
            label_id.TabIndex = 3;
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
            button_id.TabIndex = 4;
            button_id.Name = "button_id";
            // 
            // btn_consultarPorId
            // 
            btn_consultarPorId.AccessibleName = "Consultar o ID informado ou o registro selecionado";
            btn_consultarPorId.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_consultarPorId.Location = new Point(860, 12);
            btn_consultarPorId.Size = new Size(140, 36);
            btn_consultarPorId.TabIndex = 5;
            btn_consultarPorId.Text = "&Consultar";
            btn_consultarPorId.Name = "btn_consultarPorId";
            btn_consultarPorId.Click += btn_consultarPorId_Click;
            // 
            // pnlResumo
            // 
            pnlResumo.Controls.Add(label_qtdAlunos);
            pnlResumo.BackColor = Color.White;
            pnlResumo.Dock = DockStyle.Top;
            pnlResumo.Padding = new Padding(16, 6, 16, 6);
            pnlResumo.Size = new Size(1000, 52);
            pnlResumo.TabIndex = 0;
            pnlResumo.Name = "pnlResumo";
            // 
            // label_qtdAlunos
            // 
            label_qtdAlunos.AccessibleName = "Alunos sem medalha";
            label_qtdAlunos.Dock = DockStyle.Fill;
            label_qtdAlunos.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            label_qtdAlunos.ForeColor = Color.FromArgb(30, 58, 138);
            label_qtdAlunos.TabIndex = 0;
            label_qtdAlunos.TextAlign = ContentAlignment.MiddleLeft;
            label_qtdAlunos.Name = "label_qtdAlunos";
            // 
            // pnlCartao
            // 
            pnlCartao.Controls.Add(btn_editar);
            pnlCartao.Controls.Add(lblRegistro);
            pnlCartao.Controls.Add(label_nome);
            pnlCartao.Controls.Add(button_nome);
            pnlCartao.Controls.Add(label_pontosMinimos);
            pnlCartao.Controls.Add(button_pontosMinimos);
            pnlCartao.Controls.Add(label_imagemURL);
            pnlCartao.Controls.Add(button_imagemURL);
            pnlCartao.Controls.Add(btn_selecionarImagem);
            pnlCartao.Controls.Add(picImagem);
            pnlCartao.Controls.Add(label_dicaImagem);
            pnlCartao.Controls.Add(lblFaixa);
            pnlCartao.Controls.Add(lblObrigatorios);
            pnlCartao.Controls.Add(btn_incluir);
            pnlCartao.Controls.Add(btn_voltar);
            pnlCartao.BackColor = Color.White;
            pnlCartao.Location = new Point(0, 8);
            pnlCartao.Size = new Size(640, 430);
            pnlCartao.TabIndex = 0;
            pnlCartao.Name = "pnlCartao";
            // 
            // lblRegistro
            // 
            lblRegistro.Font = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point);
            lblRegistro.ForeColor = Color.FromArgb(37, 99, 235);
            lblRegistro.Location = new Point(24, 16);
            lblRegistro.Size = new Size(590, 36);
            lblRegistro.TabIndex = 0;
            lblRegistro.Text = "Nova medalha";
            lblRegistro.Name = "lblRegistro";
            // 
            // label_nome
            // 
            label_nome.Location = new Point(24, 66);
            label_nome.Size = new Size(300, 22);
            label_nome.TabIndex = 1;
            label_nome.Text = "&Nome da medalha *";
            label_nome.Name = "label_nome";
            // 
            // button_nome
            // 
            button_nome.AccessibleName = "Nome da medalha, obrigatório";
            button_nome.Location = new Point(24, 90);
            button_nome.MaxLength = 50;
            button_nome.PlaceholderText = "ex.: Bronze";
            button_nome.Size = new Size(300, 25);
            button_nome.TabIndex = 3;
            button_nome.Name = "button_nome";
            // 
            // label_pontosMinimos
            // 
            label_pontosMinimos.Location = new Point(348, 66);
            label_pontosMinimos.Size = new Size(266, 22);
            label_pontosMinimos.TabIndex = 2;
            label_pontosMinimos.Text = "&Pontuação mínima *";
            label_pontosMinimos.Name = "label_pontosMinimos";
            // 
            // button_pontosMinimos
            // 
            button_pontosMinimos.AccessibleName = "Pontuação mínima da faixa, obrigatória";
            button_pontosMinimos.Location = new Point(348, 90);
            button_pontosMinimos.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            button_pontosMinimos.Size = new Size(140, 25);
            button_pontosMinimos.TabIndex = 4;
            button_pontosMinimos.ThousandsSeparator = true;
            button_pontosMinimos.Name = "button_pontosMinimos";
            // 
            // label_imagemURL
            // 
            label_imagemURL.Location = new Point(24, 128);
            label_imagemURL.Size = new Size(450, 22);
            label_imagemURL.TabIndex = 5;
            label_imagemURL.Text = "&Imagem da medalha *";
            label_imagemURL.Name = "label_imagemURL";
            // 
            // button_imagemURL
            // 
            button_imagemURL.AccessibleName = "Imagem da medalha: caminho do arquivo ou da versão Web, obrigatório";
            button_imagemURL.Location = new Point(24, 152);
            button_imagemURL.MaxLength = 500;
            button_imagemURL.PlaceholderText = "assets/medalhas/bronze.png ou cole o caminho do arquivo";
            button_imagemURL.Size = new Size(450, 25);
            button_imagemURL.TabIndex = 6;
            button_imagemURL.Name = "button_imagemURL";
            // 
            // btn_selecionarImagem
            // 
            btn_selecionarImagem.Location = new Point(484, 149);
            btn_selecionarImagem.Size = new Size(130, 30);
            btn_selecionarImagem.TabIndex = 7;
            btn_selecionarImagem.Text = "Usar &imagem";
            btn_selecionarImagem.Name = "btn_selecionarImagem";
            btn_selecionarImagem.Click += btn_selecionarImagem_Click;
            // 
            // picImagem
            // 
            picImagem.AccessibleName = "Prévia da imagem da medalha";
            picImagem.BackColor = Color.FromArgb(245, 246, 250);
            picImagem.BorderStyle = BorderStyle.FixedSingle;
            picImagem.Location = new Point(24, 192);
            picImagem.Size = new Size(110, 110);
            picImagem.SizeMode = PictureBoxSizeMode.Zoom;
            picImagem.TabIndex = 8;
            picImagem.TabStop = false;
            picImagem.Name = "picImagem";
            // 
            // label_dicaImagem
            // 
            label_dicaImagem.ForeColor = Color.FromArgb(75, 85, 99);
            label_dicaImagem.Location = new Point(146, 192);
            label_dicaImagem.Size = new Size(468, 110);
            label_dicaImagem.TabIndex = 9;
            label_dicaImagem.Text = "Arraste a imagem (PNG, JPG, GIF ou BMP, até 2 MB) para o quadro ao lado, ou cole o caminho do arquivo e clique em Usar imagem: ela será copiada ao salvar. Também pode ser informado o caminho usado pela versão Web, como assets/medalhas/bronze.png.";
            label_dicaImagem.Name = "label_dicaImagem";
            // 
            // lblFaixa
            // 
            lblFaixa.ForeColor = Color.FromArgb(30, 58, 138);
            lblFaixa.Location = new Point(24, 316);
            lblFaixa.Size = new Size(590, 40);
            lblFaixa.TabIndex = 10;
            lblFaixa.Text = "";
            lblFaixa.Name = "lblFaixa";
            // 
            // lblObrigatorios
            // 
            lblObrigatorios.ForeColor = Color.FromArgb(75, 85, 99);
            lblObrigatorios.Location = new Point(24, 378);
            lblObrigatorios.Size = new Size(200, 22);
            lblObrigatorios.TabIndex = 14;
            lblObrigatorios.Text = "* campos obrigatórios";
            lblObrigatorios.Name = "lblObrigatorios";
            // 
            // btn_incluir
            // 
            btn_incluir.Location = new Point(374, 370);
            btn_incluir.Size = new Size(110, 38);
            btn_incluir.TabIndex = 12;
            btn_incluir.Text = "&Incluir";
            btn_incluir.Visible = false;
            btn_incluir.Name = "btn_incluir";
            btn_incluir.Click += btn_incluir_Click;
            // 
            // btn_editar
            // 
            btn_editar.Location = new Point(314, 370);
            btn_editar.Size = new Size(170, 38);
            btn_editar.TabIndex = 11;
            btn_editar.Text = "&Salvar alterações";
            btn_editar.Visible = false;
            btn_editar.Name = "btn_editar";
            btn_editar.Click += btn_editar_Click;
            // 
            // btn_voltar
            // 
            btn_voltar.Location = new Point(494, 370);
            btn_voltar.Size = new Size(120, 38);
            btn_voltar.TabIndex = 13;
            btn_voltar.Text = "&Voltar";
            btn_voltar.Name = "btn_voltar";
            btn_voltar.Click += btn_voltar_Click;
            // 
            // errorProvider
            // 
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            errorProvider.ContainerControl = this;
            // 
            // Form_Medalha
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(245, 246, 250);
            ClientSize = new Size(1000, 640);
            KeyPreview = true;
            Controls.Add(pnlLista);
            Controls.Add(pnlCadastro);
            Controls.Add(pnlCabecalho);
            Name = "Form_Medalha";
            pnlLista.ResumeLayout(false);
            pnlLista.PerformLayout();
            pnlCadastro.ResumeLayout(false);
            pnlCadastro.PerformLayout();
            pnlCabecalho.ResumeLayout(false);
            pnlCabecalho.PerformLayout();
            pnlLegenda.ResumeLayout(false);
            pnlLegenda.PerformLayout();
            pnlAcoes.ResumeLayout(false);
            pnlAcoes.PerformLayout();
            pnlResumo.ResumeLayout(false);
            pnlResumo.PerformLayout();
            pnlCartao.ResumeLayout(false);
            pnlCartao.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMedalhas).EndInit();
            ((System.ComponentModel.ISupportInitialize)button_pontosMinimos).EndInit();
            ((System.ComponentModel.ISupportInitialize)picImagem).EndInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlLista;
        private Panel pnlCadastro;
        private Panel pnlCabecalho;
        private Label label_titulo;
        private Label lblSubtitulo;
        private Label label_modo;
        private DataGridView dgvMedalhas;
        private DataGridViewTextBoxColumn colNome;
        private DataGridViewTextBoxColumn colFaixa;
        private DataGridViewTextBoxColumn colPontos;
        private DataGridViewTextBoxColumn colAlunos;
        private DataGridViewTextBoxColumn colImagem;
        private Panel pnlLegenda;
        private Label label_lista;
        private Label label_totalRegistros;
        private Panel pnlAcoes;
        private Button btn_novo;
        private Button btn_excluir;
        private Button btn_listar;
        private Label label_id;
        private TextBox button_id;
        private Button btn_consultarPorId;
        private Panel pnlResumo;
        private Label label_qtdAlunos;
        private Panel pnlCartao;
        private Label lblRegistro;
        private Label label_nome;
        private TextBox button_nome;
        private Label label_pontosMinimos;
        private NumericUpDown button_pontosMinimos;
        private Label label_imagemURL;
        private TextBox button_imagemURL;
        private Button btn_selecionarImagem;
        private PictureBox picImagem;
        private Label label_dicaImagem;
        private Label lblFaixa;
        private Label lblObrigatorios;
        private Button btn_incluir;
        private Button btn_editar;
        private Button btn_voltar;
        private ErrorProvider errorProvider;
    }
}
