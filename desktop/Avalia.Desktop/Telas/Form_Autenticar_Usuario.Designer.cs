namespace Avalia.Desktop.Telas
{
    partial class Form_Autenticar_Usuario
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
            pnlLateral = new Panel();
            lblRodapeLateral = new Label();
            lblSlogan = new Label();
            picLogo = new PictureBox();
            label_titulo = new Label();
            label_subtitulo = new Label();
            label_email = new Label();
            button_email = new TextBox();
            label_senha = new Label();
            button_senha = new TextBox();
            chkMostrarSenha = new CheckBox();
            label_erro = new Label();
            btn_autenticarUsuario = new Button();
            lnkEsqueciSenha = new LinkLabel();
            btn_sairSistema = new Button();
            pnlLateral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            SuspendLayout();
            // 
            // pnlLateral
            // 
            pnlLateral.Controls.Add(lblRodapeLateral);
            pnlLateral.Controls.Add(lblSlogan);
            pnlLateral.Controls.Add(picLogo);
            pnlLateral.BackColor = Color.FromArgb(37, 99, 235);
            pnlLateral.Dock = DockStyle.Left;
            pnlLateral.Location = new Point(0, 0);
            pnlLateral.Name = "pnlLateral";
            pnlLateral.Size = new Size(380, 560);
            pnlLateral.TabIndex = 0;
            pnlLateral.Paint += pnlLateral_Paint;
            // 
            // lblRodapeLateral
            // 
            lblRodapeLateral.BackColor = Color.Transparent;
            lblRodapeLateral.ForeColor = Color.FromArgb(219, 234, 254);
            lblRodapeLateral.Location = new Point(40, 492);
            lblRodapeLateral.Name = "lblRodapeLateral";
            lblRodapeLateral.Size = new Size(300, 44);
            lblRodapeLateral.TabIndex = 2;
            lblRodapeLateral.Text = "Aplicação Desktop do Administrador\r\nPIM IV · ADS · UNIP";
            // 
            // lblSlogan
            // 
            lblSlogan.BackColor = Color.Transparent;
            lblSlogan.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lblSlogan.ForeColor = Color.White;
            lblSlogan.Location = new Point(40, 270);
            lblSlogan.Name = "lblSlogan";
            lblSlogan.Size = new Size(300, 72);
            lblSlogan.TabIndex = 1;
            lblSlogan.Text = "Plataforma de simulados e acompanhamento de desempenho acadêmico";
            // 
            // picLogo
            // 
            picLogo.AccessibleName = "Logo do Avalia+";
            picLogo.AccessibleRole = AccessibleRole.Graphic;
            picLogo.BackColor = Color.Transparent;
            picLogo.Location = new Point(40, 210);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(300, 48);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.TabIndex = 0;
            picLogo.TabStop = false;
            // 
            // label_titulo
            // 
            label_titulo.Font = new Font("Segoe UI", 20F, FontStyle.Bold, GraphicsUnit.Point);
            label_titulo.ForeColor = Color.FromArgb(37, 99, 235);
            label_titulo.Location = new Point(440, 60);
            label_titulo.Name = "label_titulo";
            label_titulo.Size = new Size(380, 46);
            label_titulo.TabIndex = 1;
            label_titulo.Text = "Entrar";
            // 
            // label_subtitulo
            // 
            label_subtitulo.ForeColor = Color.FromArgb(75, 85, 99);
            label_subtitulo.Location = new Point(440, 108);
            label_subtitulo.Name = "label_subtitulo";
            label_subtitulo.Size = new Size(380, 24);
            label_subtitulo.TabIndex = 2;
            label_subtitulo.Text = "Use o seu e-mail e a sua senha do Avalia+.";
            // 
            // label_email
            // 
            label_email.Location = new Point(440, 156);
            label_email.Name = "label_email";
            label_email.Size = new Size(380, 22);
            label_email.TabIndex = 3;
            label_email.Text = "&E-mail";
            // 
            // button_email
            // 
            button_email.AccessibleName = "E-mail";
            button_email.Location = new Point(440, 180);
            button_email.MaxLength = 150;
            button_email.Name = "button_email";
            button_email.Size = new Size(380, 25);
            button_email.TabIndex = 4;
            // 
            // label_senha
            // 
            label_senha.Location = new Point(440, 222);
            label_senha.Name = "label_senha";
            label_senha.Size = new Size(380, 22);
            label_senha.TabIndex = 5;
            label_senha.Text = "&Senha";
            // 
            // button_senha
            // 
            button_senha.AccessibleName = "Senha";
            button_senha.Location = new Point(440, 246);
            button_senha.MaxLength = 72;
            button_senha.Name = "button_senha";
            button_senha.Size = new Size(380, 25);
            button_senha.TabIndex = 6;
            button_senha.UseSystemPasswordChar = true;
            // 
            // chkMostrarSenha
            // 
            chkMostrarSenha.AccessibleName = "Mostrar senha";
            chkMostrarSenha.Location = new Point(440, 280);
            chkMostrarSenha.Name = "chkMostrarSenha";
            chkMostrarSenha.Size = new Size(200, 26);
            chkMostrarSenha.TabIndex = 7;
            chkMostrarSenha.Text = "&Mostrar senha";
            chkMostrarSenha.UseVisualStyleBackColor = true;
            chkMostrarSenha.CheckedChanged += chkMostrarSenha_CheckedChanged;
            // 
            // label_erro
            // 
            label_erro.AccessibleName = "Mensagem";
            label_erro.AccessibleRole = AccessibleRole.Alert;
            label_erro.ForeColor = Color.FromArgb(185, 28, 28);
            label_erro.Location = new Point(440, 312);
            label_erro.Name = "label_erro";
            label_erro.Size = new Size(380, 84);
            label_erro.TabIndex = 8;
            // 
            // btn_autenticarUsuario
            // 
            btn_autenticarUsuario.AccessibleName = "Entrar";
            btn_autenticarUsuario.Location = new Point(440, 400);
            btn_autenticarUsuario.Name = "btn_autenticarUsuario";
            btn_autenticarUsuario.Size = new Size(380, 42);
            btn_autenticarUsuario.TabIndex = 9;
            btn_autenticarUsuario.Text = "E&ntrar";
            btn_autenticarUsuario.UseVisualStyleBackColor = true;
            btn_autenticarUsuario.Click += btn_autenticarUsuario_Click;
            // 
            // lnkEsqueciSenha
            // 
            lnkEsqueciSenha.AccessibleName = "Esqueci minha senha";
            lnkEsqueciSenha.LinkColor = Color.FromArgb(37, 99, 235);
            lnkEsqueciSenha.Location = new Point(440, 452);
            lnkEsqueciSenha.Name = "lnkEsqueciSenha";
            lnkEsqueciSenha.Size = new Size(380, 24);
            lnkEsqueciSenha.TabIndex = 10;
            lnkEsqueciSenha.TabStop = true;
            lnkEsqueciSenha.Text = "Esqueci minha senha";
            lnkEsqueciSenha.TextAlign = ContentAlignment.MiddleCenter;
            lnkEsqueciSenha.LinkClicked += lnkEsqueciSenha_LinkClicked;
            // 
            // btn_sairSistema
            // 
            btn_sairSistema.AccessibleName = "Sair do Avalia+";
            btn_sairSistema.Location = new Point(440, 492);
            btn_sairSistema.Name = "btn_sairSistema";
            btn_sairSistema.Size = new Size(380, 36);
            btn_sairSistema.TabIndex = 11;
            btn_sairSistema.Text = "Sai&r";
            btn_sairSistema.UseVisualStyleBackColor = true;
            btn_sairSistema.Click += btn_sairSistema_Click;
            // 
            // Form_Autenticar_Usuario
            // 
            AcceptButton = btn_autenticarUsuario;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.White;
            CancelButton = btn_sairSistema;
            ClientSize = new Size(880, 560);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Avalia+ | Entrar";
            Controls.Add(btn_sairSistema);
            Controls.Add(lnkEsqueciSenha);
            Controls.Add(btn_autenticarUsuario);
            Controls.Add(label_erro);
            Controls.Add(chkMostrarSenha);
            Controls.Add(button_senha);
            Controls.Add(label_senha);
            Controls.Add(button_email);
            Controls.Add(label_email);
            Controls.Add(label_subtitulo);
            Controls.Add(label_titulo);
            Controls.Add(pnlLateral);
            Name = "Form_Autenticar_Usuario";
            pnlLateral.ResumeLayout(false);
            pnlLateral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlLateral;
        private Label lblRodapeLateral;
        private Label lblSlogan;
        private PictureBox picLogo;
        private Label label_titulo;
        private Label label_subtitulo;
        private Label label_email;
        private TextBox button_email;
        private Label label_senha;
        private TextBox button_senha;
        private CheckBox chkMostrarSenha;
        private Label label_erro;
        private Button btn_autenticarUsuario;
        private LinkLabel lnkEsqueciSenha;
        private Button btn_sairSistema;
    }
}
