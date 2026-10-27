namespace Avalia.Desktop.Telas
{
    partial class Form_Visualizar_perfil
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
            pnlConteudo = new Panel();
            pnlDados = new Panel();
            lblTituloDados = new Label();
            label_tituloNome = new Label();
            labelNome = new Label();
            label_tituloEmail = new Label();
            labelEmail = new Label();
            label_tituloTipoUsuario = new Label();
            labelTipoUsuario = new Label();
            label_tituloDataIngresso = new Label();
            labelDataIngresso = new Label();
            botaoAlterarSenha = new Button();
            botaoSair = new Button();
            btn_voltar = new Button();
            pnlSenha = new Panel();
            lblTituloSenha = new Label();
            pnlRecomendacao = new Panel();
            lblRecomendacao = new Label();
            label_senhaAtual = new Label();
            txt_senhaAtual = new TextBox();
            label_novaSenha = new Label();
            txt_novaSenha = new TextBox();
            label_confirmacao = new Label();
            txt_confirmacao = new TextBox();
            chkMostrarSenhas = new CheckBox();
            lblRegras = new Label();
            btn_salvarSenha = new Button();
            btnCancelarSenha = new Button();
            errorProvider = new ErrorProvider(components);
            pnlCabecalho.SuspendLayout();
            pnlConteudo.SuspendLayout();
            pnlDados.SuspendLayout();
            pnlSenha.SuspendLayout();
            pnlRecomendacao.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
            SuspendLayout();
            // 
            // pnlCabecalho
            // 
            pnlCabecalho.Controls.Add(label_titulo);
            pnlCabecalho.Controls.Add(lblSubtitulo);
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
            label_titulo.Text = "Meu perfil";
            label_titulo.Name = "label_titulo";
            // 
            // lblSubtitulo
            // 
            lblSubtitulo.ForeColor = Color.FromArgb(75, 85, 99);
            lblSubtitulo.Location = new Point(0, 42);
            lblSubtitulo.Size = new Size(980, 24);
            lblSubtitulo.TabIndex = 1;
            lblSubtitulo.Text = "Seus dados cadastrais. A senha é alterada somente por você (RN04).";
            lblSubtitulo.Name = "lblSubtitulo";
            // 
            // pnlConteudo
            // 
            pnlConteudo.Controls.Add(pnlDados);
            pnlConteudo.Controls.Add(pnlSenha);
            pnlConteudo.AutoScroll = true;
            pnlConteudo.Dock = DockStyle.Fill;
            pnlConteudo.Location = new Point(0, 76);
            pnlConteudo.Size = new Size(1000, 564);
            pnlConteudo.TabIndex = 1;
            pnlConteudo.Name = "pnlConteudo";
            // 
            // pnlDados
            // 
            pnlDados.Controls.Add(lblTituloDados);
            pnlDados.Controls.Add(label_tituloNome);
            pnlDados.Controls.Add(labelNome);
            pnlDados.Controls.Add(label_tituloEmail);
            pnlDados.Controls.Add(labelEmail);
            pnlDados.Controls.Add(label_tituloTipoUsuario);
            pnlDados.Controls.Add(labelTipoUsuario);
            pnlDados.Controls.Add(label_tituloDataIngresso);
            pnlDados.Controls.Add(labelDataIngresso);
            pnlDados.Controls.Add(botaoAlterarSenha);
            pnlDados.Controls.Add(botaoSair);
            pnlDados.Controls.Add(btn_voltar);
            pnlDados.BackColor = Color.White;
            pnlDados.Location = new Point(0, 8);
            pnlDados.Size = new Size(468, 300);
            pnlDados.TabIndex = 0;
            pnlDados.Name = "pnlDados";
            // 
            // lblTituloDados
            // 
            lblTituloDados.Font = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point);
            lblTituloDados.ForeColor = Color.FromArgb(37, 99, 235);
            lblTituloDados.Location = new Point(24, 16);
            lblTituloDados.Size = new Size(420, 36);
            lblTituloDados.TabIndex = 0;
            lblTituloDados.Text = "Dados do usuário";
            lblTituloDados.Name = "lblTituloDados";
            // 
            // label_tituloNome
            // 
            label_tituloNome.ForeColor = Color.FromArgb(75, 85, 99);
            label_tituloNome.Location = new Point(24, 62);
            label_tituloNome.Size = new Size(420, 20);
            label_tituloNome.TabIndex = 1;
            label_tituloNome.Text = "Nome";
            label_tituloNome.Name = "label_tituloNome";
            // 
            // labelNome
            // 
            labelNome.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
            labelNome.Location = new Point(24, 82);
            labelNome.Size = new Size(420, 24);
            labelNome.TabIndex = 2;
            labelNome.Text = "";
            labelNome.Name = "labelNome";
            // 
            // label_tituloEmail
            // 
            label_tituloEmail.ForeColor = Color.FromArgb(75, 85, 99);
            label_tituloEmail.Location = new Point(24, 114);
            label_tituloEmail.Size = new Size(420, 20);
            label_tituloEmail.TabIndex = 3;
            label_tituloEmail.Text = "E-mail";
            label_tituloEmail.Name = "label_tituloEmail";
            // 
            // labelEmail
            // 
            labelEmail.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            labelEmail.Location = new Point(24, 134);
            labelEmail.Size = new Size(420, 24);
            labelEmail.TabIndex = 4;
            labelEmail.Text = "";
            labelEmail.Name = "labelEmail";
            // 
            // label_tituloTipoUsuario
            // 
            label_tituloTipoUsuario.ForeColor = Color.FromArgb(75, 85, 99);
            label_tituloTipoUsuario.Location = new Point(24, 166);
            label_tituloTipoUsuario.Size = new Size(200, 20);
            label_tituloTipoUsuario.TabIndex = 5;
            label_tituloTipoUsuario.Text = "Tipo de usuário";
            label_tituloTipoUsuario.Name = "label_tituloTipoUsuario";
            // 
            // labelTipoUsuario
            // 
            labelTipoUsuario.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            labelTipoUsuario.Location = new Point(24, 186);
            labelTipoUsuario.Size = new Size(200, 24);
            labelTipoUsuario.TabIndex = 7;
            labelTipoUsuario.Text = "";
            labelTipoUsuario.Name = "labelTipoUsuario";
            // 
            // label_tituloDataIngresso
            // 
            label_tituloDataIngresso.ForeColor = Color.FromArgb(75, 85, 99);
            label_tituloDataIngresso.Location = new Point(240, 166);
            label_tituloDataIngresso.Size = new Size(204, 20);
            label_tituloDataIngresso.TabIndex = 6;
            label_tituloDataIngresso.Text = "Data de ingresso";
            label_tituloDataIngresso.Name = "label_tituloDataIngresso";
            // 
            // labelDataIngresso
            // 
            labelDataIngresso.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            labelDataIngresso.Location = new Point(240, 186);
            labelDataIngresso.Size = new Size(204, 24);
            labelDataIngresso.TabIndex = 8;
            labelDataIngresso.Text = "";
            labelDataIngresso.Name = "labelDataIngresso";
            // 
            // botaoAlterarSenha
            // 
            botaoAlterarSenha.Location = new Point(24, 238);
            botaoAlterarSenha.Size = new Size(170, 38);
            botaoAlterarSenha.TabIndex = 9;
            botaoAlterarSenha.Text = "&Alterar senha";
            botaoAlterarSenha.Name = "botaoAlterarSenha";
            botaoAlterarSenha.Click += botaoAlterarSenha_Click;
            // 
            // botaoSair
            // 
            botaoSair.Location = new Point(204, 238);
            botaoSair.Size = new Size(130, 38);
            botaoSair.TabIndex = 10;
            botaoSair.Text = "Sair da c&onta";
            botaoSair.Name = "botaoSair";
            botaoSair.Click += botaoSair_Click;
            // 
            // btn_voltar
            // 
            btn_voltar.Location = new Point(344, 238);
            btn_voltar.Size = new Size(100, 38);
            btn_voltar.TabIndex = 11;
            btn_voltar.Text = "&Voltar";
            btn_voltar.Name = "btn_voltar";
            btn_voltar.Click += btn_voltar_Click;
            // 
            // pnlSenha
            // 
            pnlSenha.Controls.Add(btnCancelarSenha);
            pnlSenha.Controls.Add(btn_salvarSenha);
            pnlSenha.Controls.Add(lblRegras);
            pnlSenha.Controls.Add(chkMostrarSenhas);
            pnlSenha.Controls.Add(txt_confirmacao);
            pnlSenha.Controls.Add(label_confirmacao);
            pnlSenha.Controls.Add(txt_novaSenha);
            pnlSenha.Controls.Add(label_novaSenha);
            pnlSenha.Controls.Add(txt_senhaAtual);
            pnlSenha.Controls.Add(label_senhaAtual);
            pnlSenha.Controls.Add(pnlRecomendacao);
            pnlSenha.Controls.Add(lblTituloSenha);
            pnlSenha.BackColor = Color.White;
            pnlSenha.Location = new Point(488, 8);
            pnlSenha.Size = new Size(468, 440);
            pnlSenha.TabIndex = 1;
            pnlSenha.Visible = false;
            pnlSenha.Name = "pnlSenha";
            // 
            // lblTituloSenha
            // 
            lblTituloSenha.Font = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point);
            lblTituloSenha.ForeColor = Color.FromArgb(37, 99, 235);
            lblTituloSenha.Location = new Point(24, 16);
            lblTituloSenha.Name = "lblTituloSenha";
            lblTituloSenha.Size = new Size(420, 36);
            lblTituloSenha.TabIndex = 0;
            lblTituloSenha.Text = "Alterar senha";
            // 
            // pnlRecomendacao
            // 
            pnlRecomendacao.Controls.Add(lblRecomendacao);
            pnlRecomendacao.BackColor = Color.FromArgb(254, 243, 199);
            pnlRecomendacao.Location = new Point(24, 58);
            pnlRecomendacao.Name = "pnlRecomendacao";
            pnlRecomendacao.Padding = new Padding(10, 6, 10, 6);
            pnlRecomendacao.Size = new Size(420, 56);
            pnlRecomendacao.TabIndex = 1;
            pnlRecomendacao.Visible = false;
            // 
            // lblRecomendacao
            // 
            lblRecomendacao.Dock = DockStyle.Fill;
            lblRecomendacao.ForeColor = Color.FromArgb(146, 64, 14);
            lblRecomendacao.Location = new Point(10, 6);
            lblRecomendacao.Name = "lblRecomendacao";
            lblRecomendacao.Size = new Size(400, 44);
            lblRecomendacao.TabIndex = 0;
            lblRecomendacao.Text = "Sua senha ainda é o seu CPF. Recomendamos trocá-la agora (a troca não é obrigatória).";
            lblRecomendacao.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label_senhaAtual
            // 
            label_senhaAtual.Location = new Point(24, 124);
            label_senhaAtual.Name = "label_senhaAtual";
            label_senhaAtual.Size = new Size(400, 22);
            label_senhaAtual.TabIndex = 2;
            label_senhaAtual.Text = "Senha &atual";
            // 
            // txt_senhaAtual
            // 
            txt_senhaAtual.AccessibleName = "Senha atual";
            txt_senhaAtual.Location = new Point(24, 148);
            txt_senhaAtual.MaxLength = 72;
            txt_senhaAtual.Name = "txt_senhaAtual";
            txt_senhaAtual.Size = new Size(396, 25);
            txt_senhaAtual.TabIndex = 3;
            txt_senhaAtual.UseSystemPasswordChar = true;
            // 
            // label_novaSenha
            // 
            label_novaSenha.Location = new Point(24, 184);
            label_novaSenha.Name = "label_novaSenha";
            label_novaSenha.Size = new Size(400, 22);
            label_novaSenha.TabIndex = 4;
            label_novaSenha.Text = "&Nova senha";
            // 
            // txt_novaSenha
            // 
            txt_novaSenha.AccessibleName = "Nova senha";
            txt_novaSenha.Location = new Point(24, 208);
            txt_novaSenha.MaxLength = 72;
            txt_novaSenha.Name = "txt_novaSenha";
            txt_novaSenha.Size = new Size(396, 25);
            txt_novaSenha.TabIndex = 5;
            txt_novaSenha.UseSystemPasswordChar = true;
            // 
            // label_confirmacao
            // 
            label_confirmacao.Location = new Point(24, 244);
            label_confirmacao.Name = "label_confirmacao";
            label_confirmacao.Size = new Size(400, 22);
            label_confirmacao.TabIndex = 6;
            label_confirmacao.Text = "&Confirme a nova senha";
            // 
            // txt_confirmacao
            // 
            txt_confirmacao.AccessibleName = "Confirmação da nova senha";
            txt_confirmacao.Location = new Point(24, 268);
            txt_confirmacao.MaxLength = 72;
            txt_confirmacao.Name = "txt_confirmacao";
            txt_confirmacao.Size = new Size(396, 25);
            txt_confirmacao.TabIndex = 7;
            txt_confirmacao.UseSystemPasswordChar = true;
            // 
            // chkMostrarSenhas
            // 
            chkMostrarSenhas.Location = new Point(24, 302);
            chkMostrarSenhas.Name = "chkMostrarSenhas";
            chkMostrarSenhas.Size = new Size(200, 26);
            chkMostrarSenhas.TabIndex = 8;
            chkMostrarSenhas.Text = "&Mostrar senhas";
            chkMostrarSenhas.UseVisualStyleBackColor = true;
            chkMostrarSenhas.CheckedChanged += chkMostrarSenhas_CheckedChanged;
            // 
            // lblRegras
            // 
            lblRegras.ForeColor = Color.FromArgb(75, 85, 99);
            lblRegras.Location = new Point(24, 332);
            lblRegras.Name = "lblRegras";
            lblRegras.Size = new Size(420, 40);
            lblRegras.TabIndex = 9;
            lblRegras.Text = "Regras";
            // 
            // btn_salvarSenha
            // 
            btn_salvarSenha.Location = new Point(204, 384);
            btn_salvarSenha.Name = "btn_salvarSenha";
            btn_salvarSenha.Size = new Size(120, 38);
            btn_salvarSenha.TabIndex = 10;
            btn_salvarSenha.Text = "&Salvar senha";
            btn_salvarSenha.UseVisualStyleBackColor = true;
            btn_salvarSenha.Click += btn_salvarSenha_Click;
            // 
            // btnCancelarSenha
            // 
            btnCancelarSenha.Location = new Point(334, 384);
            btnCancelarSenha.Name = "btnCancelarSenha";
            btnCancelarSenha.Size = new Size(110, 38);
            btnCancelarSenha.TabIndex = 11;
            btnCancelarSenha.Text = "Cancelar";
            btnCancelarSenha.UseVisualStyleBackColor = true;
            btnCancelarSenha.Click += btnCancelarSenha_Click;
            // 
            // errorProvider
            // 
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            errorProvider.ContainerControl = this;
            // 
            // Form_Visualizar_perfil
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(245, 246, 250);
            ClientSize = new Size(1000, 640);
            KeyPreview = true;
            Controls.Add(pnlConteudo);
            Controls.Add(pnlCabecalho);
            Name = "Form_Visualizar_perfil";
            pnlCabecalho.ResumeLayout(false);
            pnlCabecalho.PerformLayout();
            pnlConteudo.ResumeLayout(false);
            pnlConteudo.PerformLayout();
            pnlDados.ResumeLayout(false);
            pnlDados.PerformLayout();
            pnlSenha.ResumeLayout(false);
            pnlSenha.PerformLayout();
            pnlRecomendacao.ResumeLayout(false);
            pnlRecomendacao.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlCabecalho;
        private Label label_titulo;
        private Label lblSubtitulo;
        private Panel pnlConteudo;
        private Panel pnlDados;
        private Label lblTituloDados;
        private Label label_tituloNome;
        private Label labelNome;
        private Label label_tituloEmail;
        private Label labelEmail;
        private Label label_tituloTipoUsuario;
        private Label labelTipoUsuario;
        private Label label_tituloDataIngresso;
        private Label labelDataIngresso;
        private Button botaoAlterarSenha;
        private Button botaoSair;
        private Button btn_voltar;
        private Panel pnlSenha;
        private Label lblTituloSenha;
        private Panel pnlRecomendacao;
        private Label lblRecomendacao;
        private Label label_senhaAtual;
        private TextBox txt_senhaAtual;
        private Label label_novaSenha;
        private TextBox txt_novaSenha;
        private Label label_confirmacao;
        private TextBox txt_confirmacao;
        private CheckBox chkMostrarSenhas;
        private Label lblRegras;
        private Button btn_salvarSenha;
        private Button btnCancelarSenha;
        private ErrorProvider errorProvider;
    }
}
