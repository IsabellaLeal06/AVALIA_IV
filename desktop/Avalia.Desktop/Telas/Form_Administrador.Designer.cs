namespace Avalia.Desktop.Telas
{
    partial class Form_Administrador
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
            pnlMenu = new Panel();
            btn_sairConta = new Button();
            btn_visualizarPerfil = new Button();
            btn_gerenciarQuestoes = new Button();
            btn_gerenciarDisciplina = new Button();
            btn_gerenciarTurma = new Button();
            btn_gerenciarUsuario = new Button();
            btn_visualizarPainel = new Button();
            label_usuarioLogado = new Label();
            picLogo = new PictureBox();
            pnlDireita = new Panel();
            pnlConteudo = new Panel();
            barraMensagem = new Componentes.BarraMensagem();
            stsRodape = new StatusStrip();
            lblStatusBanco = new ToolStripStatusLabel();
            lblStatusEspaco = new ToolStripStatusLabel();
            lblStatusVersao = new ToolStripStatusLabel();
            pnlPainel = new Panel();
            pnlCabecalhoPainel = new Panel();
            btnAtualizarPainel = new Button();
            label_painelDescricao = new Label();
            label_painel = new Label();
            pnlRecomendacao = new Panel();
            lblRecomendacao = new Label();
            btnAlterarSenhaAgora = new Button();
            tblCartoes = new TableLayoutPanel();
            pnlCardTurmas = new Panel();
            label_qtdTurmas = new Label();
            label_cardTurmas = new Label();
            lblDetalheTurmas = new Label();
            pnlCardUsuarios = new Panel();
            label_qtdUsuarios = new Label();
            label_cardUsuarios = new Label();
            lblDetalheUsuarios = new Label();
            pnlCardDisciplinas = new Panel();
            label_qtdDisciplinas = new Label();
            label_cardDisciplinas = new Label();
            lblDetalheDisciplinas = new Label();
            pnlCardQuestoes = new Panel();
            label_qtdQuestoes = new Label();
            label_cardQuestoes = new Label();
            lblDetalheQuestoes = new Label();
            lblAtualizadoEm = new Label();
            lblDicas = new Label();
            btn_gerenciarMedalha = new Button();
            label_titulo = new Label();
            pnlMenu.SuspendLayout();
            pnlDireita.SuspendLayout();
            pnlConteudo.SuspendLayout();
            stsRodape.SuspendLayout();
            pnlPainel.SuspendLayout();
            pnlCabecalhoPainel.SuspendLayout();
            pnlRecomendacao.SuspendLayout();
            tblCartoes.SuspendLayout();
            pnlCardTurmas.SuspendLayout();
            pnlCardUsuarios.SuspendLayout();
            pnlCardDisciplinas.SuspendLayout();
            pnlCardQuestoes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            SuspendLayout();
            // 
            // pnlMenu
            // 
            pnlMenu.Controls.Add(btn_sairConta);
            pnlMenu.Controls.Add(btn_visualizarPerfil);
            pnlMenu.Controls.Add(btn_gerenciarQuestoes);
            pnlMenu.Controls.Add(btn_gerenciarDisciplina);
            pnlMenu.Controls.Add(btn_gerenciarTurma);
            pnlMenu.Controls.Add(btn_gerenciarUsuario);
            pnlMenu.Controls.Add(btn_visualizarPainel);
            pnlMenu.Controls.Add(label_usuarioLogado);
            pnlMenu.Controls.Add(picLogo);
            pnlMenu.Controls.Add(btn_gerenciarMedalha);
            pnlMenu.Controls.Add(label_titulo);
            pnlMenu.BackColor = Color.FromArgb(30, 58, 138);
            pnlMenu.Dock = DockStyle.Left;
            pnlMenu.Location = new Point(0, 0);
            pnlMenu.Name = "pnlMenu";
            pnlMenu.Size = new Size(240, 738);
            pnlMenu.TabIndex = 0;
            // 
            // btn_sairConta
            // 
            btn_sairConta.AccessibleName = "Sair da conta";
            btn_sairConta.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btn_sairConta.Location = new Point(0, 682);
            btn_sairConta.Name = "btn_sairConta";
            btn_sairConta.Size = new Size(240, 44);
            btn_sairConta.TabIndex = 8;
            btn_sairConta.Text = "Sair da c&onta";
            btn_sairConta.Click += btn_sairConta_Click;
            // 
            // btn_visualizarPerfil
            // 
            btn_visualizarPerfil.AccessibleName = "Meu perfil";
            btn_visualizarPerfil.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btn_visualizarPerfil.Location = new Point(0, 636);
            btn_visualizarPerfil.Name = "btn_visualizarPerfil";
            btn_visualizarPerfil.Size = new Size(240, 44);
            btn_visualizarPerfil.TabIndex = 7;
            btn_visualizarPerfil.Text = "Meu pe&rfil";
            btn_visualizarPerfil.Click += btn_visualizarPerfil_Click;
            // 
            // btn_gerenciarQuestoes
            // 
            btn_gerenciarQuestoes.AccessibleName = "Questões";
            btn_gerenciarQuestoes.Location = new Point(0, 330);
            btn_gerenciarQuestoes.Name = "btn_gerenciarQuestoes";
            btn_gerenciarQuestoes.Size = new Size(240, 46);
            btn_gerenciarQuestoes.TabIndex = 6;
            btn_gerenciarQuestoes.Text = "&Questões";
            btn_gerenciarQuestoes.Click += btn_gerenciarQuestoes_Click;
            // 
            // btn_gerenciarDisciplina
            // 
            btn_gerenciarDisciplina.AccessibleName = "Disciplinas";
            btn_gerenciarDisciplina.Location = new Point(0, 284);
            btn_gerenciarDisciplina.Name = "btn_gerenciarDisciplina";
            btn_gerenciarDisciplina.Size = new Size(240, 46);
            btn_gerenciarDisciplina.TabIndex = 5;
            btn_gerenciarDisciplina.Text = "&Disciplinas";
            btn_gerenciarDisciplina.Click += btn_gerenciarDisciplina_Click;
            // 
            // btn_gerenciarTurma
            // 
            btn_gerenciarTurma.AccessibleName = "Turmas";
            btn_gerenciarTurma.Location = new Point(0, 238);
            btn_gerenciarTurma.Name = "btn_gerenciarTurma";
            btn_gerenciarTurma.Size = new Size(240, 46);
            btn_gerenciarTurma.TabIndex = 4;
            btn_gerenciarTurma.Text = "&Turmas";
            btn_gerenciarTurma.Click += btn_gerenciarTurma_Click;
            // 
            // btn_gerenciarUsuario
            // 
            btn_gerenciarUsuario.AccessibleName = "Usuários";
            btn_gerenciarUsuario.Location = new Point(0, 192);
            btn_gerenciarUsuario.Name = "btn_gerenciarUsuario";
            btn_gerenciarUsuario.Size = new Size(240, 46);
            btn_gerenciarUsuario.TabIndex = 3;
            btn_gerenciarUsuario.Text = "&Usuários";
            btn_gerenciarUsuario.Click += btn_gerenciarUsuario_Click;
            // 
            // btn_visualizarPainel
            // 
            btn_visualizarPainel.AccessibleName = "Painel";
            btn_visualizarPainel.Location = new Point(0, 146);
            btn_visualizarPainel.Name = "btn_visualizarPainel";
            btn_visualizarPainel.Size = new Size(240, 46);
            btn_visualizarPainel.TabIndex = 2;
            btn_visualizarPainel.Text = "&Painel";
            btn_visualizarPainel.Click += btn_visualizarPainel_Click;
            // 
            // label_usuarioLogado
            // 
            label_usuarioLogado.ForeColor = Color.FromArgb(219, 234, 254);
            label_usuarioLogado.Location = new Point(20, 86);
            label_usuarioLogado.Name = "label_usuarioLogado";
            label_usuarioLogado.Size = new Size(204, 50);
            label_usuarioLogado.TabIndex = 1;
            label_usuarioLogado.Text = "Olá!";
            // 
            // picLogo
            // 
            picLogo.AccessibleName = "Logo do Avalia+";
            picLogo.AccessibleRole = AccessibleRole.Graphic;
            picLogo.Location = new Point(20, 26);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(190, 32);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.TabIndex = 0;
            picLogo.TabStop = false;
            // 
            // pnlDireita
            // 
            pnlDireita.Controls.Add(pnlConteudo);
            pnlDireita.Controls.Add(barraMensagem);
            pnlDireita.BackColor = Color.FromArgb(245, 246, 250);
            pnlDireita.Dock = DockStyle.Fill;
            pnlDireita.Location = new Point(240, 0);
            pnlDireita.Name = "pnlDireita";
            pnlDireita.Size = new Size(1040, 738);
            pnlDireita.TabIndex = 1;
            // 
            // pnlConteudo
            // 
            pnlConteudo.Controls.Add(pnlPainel);
            pnlConteudo.AutoScroll = true;
            pnlConteudo.BackColor = Color.FromArgb(245, 246, 250);
            pnlConteudo.Dock = DockStyle.Fill;
            pnlConteudo.Location = new Point(0, 44);
            pnlConteudo.Name = "pnlConteudo";
            pnlConteudo.Padding = new Padding(24, 20, 24, 16);
            pnlConteudo.Size = new Size(1040, 694);
            pnlConteudo.TabIndex = 1;
            // 
            // barraMensagem
            // 
            barraMensagem.Dock = DockStyle.Top;
            barraMensagem.Location = new Point(0, 0);
            barraMensagem.Name = "barraMensagem";
            barraMensagem.Size = new Size(1040, 44);
            barraMensagem.TabIndex = 0;
            barraMensagem.Visible = false;
            // 
            // stsRodape
            // 
            stsRodape.BackColor = Color.White;
            stsRodape.Items.AddRange(new ToolStripItem[] { lblStatusBanco, lblStatusEspaco, lblStatusVersao });
            stsRodape.Location = new Point(0, 738);
            stsRodape.Name = "stsRodape";
            stsRodape.Size = new Size(1280, 22);
            stsRodape.TabIndex = 2;
            // 
            // lblStatusBanco
            // 
            lblStatusBanco.Name = "lblStatusBanco";
            lblStatusBanco.Size = new Size(48, 17);
            lblStatusBanco.Text = "Banco: ";
            // 
            // lblStatusEspaco
            // 
            lblStatusEspaco.Name = "lblStatusEspaco";
            lblStatusEspaco.Size = new Size(1100, 17);
            lblStatusEspaco.Spring = true;
            // 
            // lblStatusVersao
            // 
            lblStatusVersao.Name = "lblStatusVersao";
            lblStatusVersao.Size = new Size(117, 17);
            lblStatusVersao.Text = "Avalia+ Desktop";
            // 
            // pnlPainel
            // 
            pnlPainel.Controls.Add(lblDicas);
            pnlPainel.Controls.Add(lblAtualizadoEm);
            pnlPainel.Controls.Add(tblCartoes);
            pnlPainel.Controls.Add(pnlRecomendacao);
            pnlPainel.Controls.Add(pnlCabecalhoPainel);
            pnlPainel.AutoScroll = true;
            pnlPainel.BackColor = Color.FromArgb(245, 246, 250);
            pnlPainel.Dock = DockStyle.Fill;
            pnlPainel.Location = new Point(0, 0);
            pnlPainel.Size = new Size(1040, 694);
            pnlPainel.TabIndex = 0;
            pnlPainel.Name = "pnlPainel";
            // 
            // pnlCabecalhoPainel
            // 
            pnlCabecalhoPainel.Controls.Add(btnAtualizarPainel);
            pnlCabecalhoPainel.Controls.Add(label_painelDescricao);
            pnlCabecalhoPainel.Controls.Add(label_painel);
            pnlCabecalhoPainel.Dock = DockStyle.Top;
            pnlCabecalhoPainel.Location = new Point(0, 0);
            pnlCabecalhoPainel.Name = "pnlCabecalhoPainel";
            pnlCabecalhoPainel.Size = new Size(1000, 84);
            pnlCabecalhoPainel.TabIndex = 0;
            // 
            // btnAtualizarPainel
            // 
            btnAtualizarPainel.AccessibleName = "Atualizar quantitativos";
            btnAtualizarPainel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAtualizarPainel.Location = new Point(840, 8);
            btnAtualizarPainel.Name = "btnAtualizarPainel";
            btnAtualizarPainel.Size = new Size(160, 36);
            btnAtualizarPainel.TabIndex = 2;
            btnAtualizarPainel.Text = "Atuali&zar (F5)";
            btnAtualizarPainel.Click += btnAtualizarPainel_Click;
            // 
            // label_painelDescricao
            // 
            label_painelDescricao.ForeColor = Color.FromArgb(75, 85, 99);
            label_painelDescricao.Location = new Point(0, 44);
            label_painelDescricao.Name = "label_painelDescricao";
            label_painelDescricao.Size = new Size(820, 24);
            label_painelDescricao.TabIndex = 1;
            label_painelDescricao.Text = "Quantitativos de cadastros ativos. O perfil Administrador não possui indicadores de desempenho acadêmico.";
            // 
            // label_painel
            // 
            label_painel.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            label_painel.ForeColor = Color.FromArgb(17, 24, 39);
            label_painel.Location = new Point(0, 0);
            label_painel.Name = "label_painel";
            label_painel.Size = new Size(600, 40);
            label_painel.TabIndex = 0;
            label_painel.Text = "Painel";
            // 
            // pnlRecomendacao
            // 
            pnlRecomendacao.Controls.Add(lblRecomendacao);
            pnlRecomendacao.Controls.Add(btnAlterarSenhaAgora);
            pnlRecomendacao.AccessibleName = "Recomendação de troca de senha";
            pnlRecomendacao.BackColor = Color.FromArgb(254, 243, 199);
            pnlRecomendacao.Dock = DockStyle.Top;
            pnlRecomendacao.Location = new Point(0, 84);
            pnlRecomendacao.Margin = new Padding(0);
            pnlRecomendacao.Name = "pnlRecomendacao";
            pnlRecomendacao.Padding = new Padding(16, 10, 16, 10);
            pnlRecomendacao.Size = new Size(1000, 60);
            pnlRecomendacao.TabIndex = 1;
            pnlRecomendacao.Visible = false;
            // 
            // lblRecomendacao
            // 
            lblRecomendacao.Dock = DockStyle.Fill;
            lblRecomendacao.ForeColor = Color.FromArgb(146, 64, 14);
            lblRecomendacao.Name = "lblRecomendacao";
            lblRecomendacao.TabIndex = 0;
            lblRecomendacao.Text = "Sua senha ainda é o seu CPF (senha inicial). Recomendamos alterá-la. A troca não é obrigatória.";
            lblRecomendacao.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnAlterarSenhaAgora
            // 
            btnAlterarSenhaAgora.Dock = DockStyle.Right;
            btnAlterarSenhaAgora.Name = "btnAlterarSenhaAgora";
            btnAlterarSenhaAgora.Size = new Size(190, 40);
            btnAlterarSenhaAgora.TabIndex = 1;
            btnAlterarSenhaAgora.Text = "Alterar a senha &agora";
            btnAlterarSenhaAgora.Click += btnAlterarSenhaAgora_Click;
            // 
            // tblCartoes
            // 
            tblCartoes.ColumnCount = 4;
            tblCartoes.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tblCartoes.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tblCartoes.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tblCartoes.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tblCartoes.Controls.Add(pnlCardTurmas, 0, 0);
            tblCartoes.Controls.Add(pnlCardUsuarios, 1, 0);
            tblCartoes.Controls.Add(pnlCardDisciplinas, 2, 0);
            tblCartoes.Controls.Add(pnlCardQuestoes, 3, 0);
            tblCartoes.Dock = DockStyle.Top;
            tblCartoes.Location = new Point(0, 84);
            tblCartoes.Name = "tblCartoes";
            tblCartoes.RowCount = 1;
            tblCartoes.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tblCartoes.Size = new Size(1000, 160);
            tblCartoes.TabIndex = 1;
            // 
            // pnlCardTurmas
            // 
            pnlCardTurmas.Controls.Add(lblDetalheTurmas);
            pnlCardTurmas.Controls.Add(label_cardTurmas);
            pnlCardTurmas.Controls.Add(label_qtdTurmas);
            pnlCardTurmas.AccessibleName = "Turmas ativas";
            pnlCardTurmas.AccessibleRole = AccessibleRole.Grouping;
            pnlCardTurmas.BackColor = Color.White;
            pnlCardTurmas.Dock = DockStyle.Fill;
            pnlCardTurmas.Margin = new Padding(0, 0, 16, 0);
            pnlCardTurmas.Name = "pnlCardTurmas";
            pnlCardTurmas.Padding = new Padding(20, 16, 16, 12);
            pnlCardTurmas.TabIndex = 0;
            // 
            // label_qtdTurmas
            // 
            label_qtdTurmas.Dock = DockStyle.Top;
            label_qtdTurmas.Font = new Font("Segoe UI", 28F, FontStyle.Bold, GraphicsUnit.Point);
            label_qtdTurmas.ForeColor = Color.FromArgb(37, 99, 235);
            label_qtdTurmas.Name = "label_qtdTurmas";
            label_qtdTurmas.Size = new Size(200, 56);
            label_qtdTurmas.TabIndex = 0;
            label_qtdTurmas.Text = "–";
            // 
            // label_cardTurmas
            // 
            label_cardTurmas.Dock = DockStyle.Top;
            label_cardTurmas.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
            label_cardTurmas.ForeColor = Color.FromArgb(17, 24, 39);
            label_cardTurmas.Name = "label_cardTurmas";
            label_cardTurmas.Size = new Size(200, 26);
            label_cardTurmas.TabIndex = 1;
            label_cardTurmas.Text = "Turmas ativas";
            // 
            // lblDetalheTurmas
            // 
            lblDetalheTurmas.Dock = DockStyle.Fill;
            lblDetalheTurmas.ForeColor = Color.FromArgb(75, 85, 99);
            lblDetalheTurmas.Name = "lblDetalheTurmas";
            lblDetalheTurmas.TabIndex = 2;
            lblDetalheTurmas.Text = "";
            // 
            // pnlCardUsuarios
            // 
            pnlCardUsuarios.Controls.Add(lblDetalheUsuarios);
            pnlCardUsuarios.Controls.Add(label_cardUsuarios);
            pnlCardUsuarios.Controls.Add(label_qtdUsuarios);
            pnlCardUsuarios.AccessibleName = "Usuários ativos";
            pnlCardUsuarios.AccessibleRole = AccessibleRole.Grouping;
            pnlCardUsuarios.BackColor = Color.White;
            pnlCardUsuarios.Dock = DockStyle.Fill;
            pnlCardUsuarios.Margin = new Padding(0, 0, 16, 0);
            pnlCardUsuarios.Name = "pnlCardUsuarios";
            pnlCardUsuarios.Padding = new Padding(20, 16, 16, 12);
            pnlCardUsuarios.TabIndex = 1;
            // 
            // label_qtdUsuarios
            // 
            label_qtdUsuarios.Dock = DockStyle.Top;
            label_qtdUsuarios.Font = new Font("Segoe UI", 28F, FontStyle.Bold, GraphicsUnit.Point);
            label_qtdUsuarios.ForeColor = Color.FromArgb(37, 99, 235);
            label_qtdUsuarios.Name = "label_qtdUsuarios";
            label_qtdUsuarios.Size = new Size(200, 56);
            label_qtdUsuarios.TabIndex = 0;
            label_qtdUsuarios.Text = "–";
            // 
            // label_cardUsuarios
            // 
            label_cardUsuarios.Dock = DockStyle.Top;
            label_cardUsuarios.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
            label_cardUsuarios.ForeColor = Color.FromArgb(17, 24, 39);
            label_cardUsuarios.Name = "label_cardUsuarios";
            label_cardUsuarios.Size = new Size(200, 26);
            label_cardUsuarios.TabIndex = 1;
            label_cardUsuarios.Text = "Usuários ativos";
            // 
            // lblDetalheUsuarios
            // 
            lblDetalheUsuarios.Dock = DockStyle.Fill;
            lblDetalheUsuarios.ForeColor = Color.FromArgb(75, 85, 99);
            lblDetalheUsuarios.Name = "lblDetalheUsuarios";
            lblDetalheUsuarios.TabIndex = 2;
            lblDetalheUsuarios.Text = "";
            // 
            // pnlCardDisciplinas
            // 
            pnlCardDisciplinas.Controls.Add(lblDetalheDisciplinas);
            pnlCardDisciplinas.Controls.Add(label_cardDisciplinas);
            pnlCardDisciplinas.Controls.Add(label_qtdDisciplinas);
            pnlCardDisciplinas.AccessibleName = "Disciplinas ativas";
            pnlCardDisciplinas.AccessibleRole = AccessibleRole.Grouping;
            pnlCardDisciplinas.BackColor = Color.White;
            pnlCardDisciplinas.Dock = DockStyle.Fill;
            pnlCardDisciplinas.Margin = new Padding(0, 0, 16, 0);
            pnlCardDisciplinas.Name = "pnlCardDisciplinas";
            pnlCardDisciplinas.Padding = new Padding(20, 16, 16, 12);
            pnlCardDisciplinas.TabIndex = 2;
            // 
            // label_qtdDisciplinas
            // 
            label_qtdDisciplinas.Dock = DockStyle.Top;
            label_qtdDisciplinas.Font = new Font("Segoe UI", 28F, FontStyle.Bold, GraphicsUnit.Point);
            label_qtdDisciplinas.ForeColor = Color.FromArgb(37, 99, 235);
            label_qtdDisciplinas.Name = "label_qtdDisciplinas";
            label_qtdDisciplinas.Size = new Size(200, 56);
            label_qtdDisciplinas.TabIndex = 0;
            label_qtdDisciplinas.Text = "–";
            // 
            // label_cardDisciplinas
            // 
            label_cardDisciplinas.Dock = DockStyle.Top;
            label_cardDisciplinas.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
            label_cardDisciplinas.ForeColor = Color.FromArgb(17, 24, 39);
            label_cardDisciplinas.Name = "label_cardDisciplinas";
            label_cardDisciplinas.Size = new Size(200, 26);
            label_cardDisciplinas.TabIndex = 1;
            label_cardDisciplinas.Text = "Disciplinas ativas";
            // 
            // lblDetalheDisciplinas
            // 
            lblDetalheDisciplinas.Dock = DockStyle.Fill;
            lblDetalheDisciplinas.ForeColor = Color.FromArgb(75, 85, 99);
            lblDetalheDisciplinas.Name = "lblDetalheDisciplinas";
            lblDetalheDisciplinas.TabIndex = 2;
            lblDetalheDisciplinas.Text = "";
            // 
            // pnlCardQuestoes
            // 
            pnlCardQuestoes.Controls.Add(lblDetalheQuestoes);
            pnlCardQuestoes.Controls.Add(label_cardQuestoes);
            pnlCardQuestoes.Controls.Add(label_qtdQuestoes);
            pnlCardQuestoes.AccessibleName = "Questões ativas";
            pnlCardQuestoes.AccessibleRole = AccessibleRole.Grouping;
            pnlCardQuestoes.BackColor = Color.White;
            pnlCardQuestoes.Dock = DockStyle.Fill;
            pnlCardQuestoes.Margin = new Padding(0, 0, 16, 0);
            pnlCardQuestoes.Name = "pnlCardQuestoes";
            pnlCardQuestoes.Padding = new Padding(20, 16, 16, 12);
            pnlCardQuestoes.TabIndex = 3;
            // 
            // label_qtdQuestoes
            // 
            label_qtdQuestoes.Dock = DockStyle.Top;
            label_qtdQuestoes.Font = new Font("Segoe UI", 28F, FontStyle.Bold, GraphicsUnit.Point);
            label_qtdQuestoes.ForeColor = Color.FromArgb(37, 99, 235);
            label_qtdQuestoes.Name = "label_qtdQuestoes";
            label_qtdQuestoes.Size = new Size(200, 56);
            label_qtdQuestoes.TabIndex = 0;
            label_qtdQuestoes.Text = "–";
            // 
            // label_cardQuestoes
            // 
            label_cardQuestoes.Dock = DockStyle.Top;
            label_cardQuestoes.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
            label_cardQuestoes.ForeColor = Color.FromArgb(17, 24, 39);
            label_cardQuestoes.Name = "label_cardQuestoes";
            label_cardQuestoes.Size = new Size(200, 26);
            label_cardQuestoes.TabIndex = 1;
            label_cardQuestoes.Text = "Questões ativas";
            // 
            // lblDetalheQuestoes
            // 
            lblDetalheQuestoes.Dock = DockStyle.Fill;
            lblDetalheQuestoes.ForeColor = Color.FromArgb(75, 85, 99);
            lblDetalheQuestoes.Name = "lblDetalheQuestoes";
            lblDetalheQuestoes.TabIndex = 2;
            lblDetalheQuestoes.Text = "";
            // 
            // lblAtualizadoEm
            // 
            lblAtualizadoEm.Dock = DockStyle.Top;
            lblAtualizadoEm.ForeColor = Color.FromArgb(75, 85, 99);
            lblAtualizadoEm.Location = new Point(0, 244);
            lblAtualizadoEm.Name = "lblAtualizadoEm";
            lblAtualizadoEm.Padding = new Padding(0, 10, 0, 0);
            lblAtualizadoEm.Size = new Size(1000, 36);
            lblAtualizadoEm.TabIndex = 2;
            // 
            // lblDicas
            // 
            lblDicas.BackColor = Color.White;
            lblDicas.Dock = DockStyle.Top;
            lblDicas.ForeColor = Color.FromArgb(17, 24, 39);
            lblDicas.Location = new Point(0, 280);
            lblDicas.Name = "lblDicas";
            lblDicas.Padding = new Padding(16, 12, 16, 12);
            lblDicas.Size = new Size(1000, 120);
            lblDicas.TabIndex = 3;
            lblDicas.Text = "Atalhos de teclado\r\n• Alt + letra sublinhada: abre o menu ou aciona o botão (ex.: Alt+U abre Usuários).\r\n• Nas listagens: Ctrl+N cadastra, Enter edita, F5 atualiza e Tab percorre os campos.\r\n• Nas telas de cadastro: Enter salva e Esc volta para a tela anterior.";
            // 
            // btn_gerenciarMedalha
            // 
            btn_gerenciarMedalha.AccessibleName = "Medalhas";
            btn_gerenciarMedalha.Location = new Point(0, 376);
            btn_gerenciarMedalha.Size = new Size(240, 46);
            btn_gerenciarMedalha.TabIndex = 7;
            btn_gerenciarMedalha.Text = "&Medalhas";
            btn_gerenciarMedalha.Name = "btn_gerenciarMedalha";
            btn_gerenciarMedalha.Click += btn_gerenciarMedalha_Click;
            // 
            // label_titulo
            // 
            label_titulo.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label_titulo.ForeColor = Color.FromArgb(191, 219, 254);
            label_titulo.Location = new Point(20, 62);
            label_titulo.Size = new Size(204, 20);
            label_titulo.Text = "Área do Administrador";
            label_titulo.Name = "label_titulo";
            // 
            // Form_Administrador
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1280, 760);
            KeyPreview = true;
            MinimumSize = new Size(1100, 680);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Avalia+ | Administrador";
            WindowState = FormWindowState.Maximized;
            Controls.Add(pnlDireita);
            Controls.Add(pnlMenu);
            Controls.Add(stsRodape);
            Name = "Form_Administrador";
            pnlMenu.ResumeLayout(false);
            pnlMenu.PerformLayout();
            pnlDireita.ResumeLayout(false);
            pnlDireita.PerformLayout();
            pnlConteudo.ResumeLayout(false);
            pnlConteudo.PerformLayout();
            stsRodape.ResumeLayout(false);
            pnlPainel.ResumeLayout(false);
            pnlPainel.PerformLayout();
            pnlCabecalhoPainel.ResumeLayout(false);
            pnlCabecalhoPainel.PerformLayout();
            pnlRecomendacao.ResumeLayout(false);
            pnlRecomendacao.PerformLayout();
            tblCartoes.ResumeLayout(false);
            pnlCardTurmas.ResumeLayout(false);
            pnlCardTurmas.PerformLayout();
            pnlCardUsuarios.ResumeLayout(false);
            pnlCardUsuarios.PerformLayout();
            pnlCardDisciplinas.ResumeLayout(false);
            pnlCardDisciplinas.PerformLayout();
            pnlCardQuestoes.ResumeLayout(false);
            pnlCardQuestoes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlMenu;
        private Button btn_sairConta;
        private Button btn_visualizarPerfil;
        private Button btn_gerenciarQuestoes;
        private Button btn_gerenciarDisciplina;
        private Button btn_gerenciarTurma;
        private Button btn_gerenciarUsuario;
        private Button btn_visualizarPainel;
        private Label label_usuarioLogado;
        private PictureBox picLogo;
        private Panel pnlDireita;
        private Panel pnlConteudo;
        private Componentes.BarraMensagem barraMensagem;
        private StatusStrip stsRodape;
        private ToolStripStatusLabel lblStatusBanco;
        private ToolStripStatusLabel lblStatusEspaco;
        private ToolStripStatusLabel lblStatusVersao;
        private Panel pnlPainel;
        private Panel pnlCabecalhoPainel;
        private Button btnAtualizarPainel;
        private Label label_painelDescricao;
        private Label label_painel;
        private Panel pnlRecomendacao;
        private Label lblRecomendacao;
        private Button btnAlterarSenhaAgora;
        private TableLayoutPanel tblCartoes;
        private Panel pnlCardTurmas;
        private Label label_qtdTurmas;
        private Label label_cardTurmas;
        private Label lblDetalheTurmas;
        private Panel pnlCardUsuarios;
        private Label label_qtdUsuarios;
        private Label label_cardUsuarios;
        private Label lblDetalheUsuarios;
        private Panel pnlCardDisciplinas;
        private Label label_qtdDisciplinas;
        private Label label_cardDisciplinas;
        private Label lblDetalheDisciplinas;
        private Panel pnlCardQuestoes;
        private Label label_qtdQuestoes;
        private Label label_cardQuestoes;
        private Label lblDetalheQuestoes;
        private Label lblAtualizadoEm;
        private Label lblDicas;
        private Button btn_gerenciarMedalha;
        private Label label_titulo;
    }
}
