namespace Avalia.Desktop.Telas
{
    partial class Form_Conf_Desativação
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
            label_titulo = new Label();
            label_mensagemConfirmacao = new Label();
            button_confirmarDesativacao = new Button();
            button_cancelarDesativacao = new Button();
            SuspendLayout();
            // 
            // label_titulo
            // 
            label_titulo.Font = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point);
            label_titulo.ForeColor = Color.FromArgb(17, 24, 39);
            label_titulo.Location = new Point(24, 16);
            label_titulo.Name = "label_titulo";
            label_titulo.Size = new Size(600, 36);
            label_titulo.TabIndex = 0;
            label_titulo.Text = "Confirmar desativação";
            // 
            // label_mensagemConfirmacao
            // 
            label_mensagemConfirmacao.AccessibleRole = AccessibleRole.StaticText;
            label_mensagemConfirmacao.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
            label_mensagemConfirmacao.ForeColor = Color.FromArgb(17, 24, 39);
            label_mensagemConfirmacao.Location = new Point(24, 64);
            label_mensagemConfirmacao.Name = "label_mensagemConfirmacao";
            label_mensagemConfirmacao.Size = new Size(600, 170);
            label_mensagemConfirmacao.TabIndex = 1;
            // 
            // button_confirmarDesativacao
            // 
            button_confirmarDesativacao.Location = new Point(244, 250);
            button_confirmarDesativacao.Name = "button_confirmarDesativacao";
            button_confirmarDesativacao.Size = new Size(230, 40);
            button_confirmarDesativacao.TabIndex = 3;
            button_confirmarDesativacao.Text = "&Confirmar";
            button_confirmarDesativacao.Click += button_confirmarDesativacao_Click;
            // 
            // button_cancelarDesativacao
            // 
            button_cancelarDesativacao.AccessibleName = "Cancelar e voltar";
            button_cancelarDesativacao.Location = new Point(484, 250);
            button_cancelarDesativacao.Name = "button_cancelarDesativacao";
            button_cancelarDesativacao.Size = new Size(140, 40);
            button_cancelarDesativacao.TabIndex = 2;
            button_cancelarDesativacao.Text = "Cancelar";
            button_cancelarDesativacao.Click += button_cancelarDesativacao_Click;
            // 
            // Form_Conf_Desativação
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.White;
            CancelButton = button_cancelarDesativacao;
            ClientSize = new Size(648, 310);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Controls.Add(button_cancelarDesativacao);
            Controls.Add(button_confirmarDesativacao);
            Controls.Add(label_mensagemConfirmacao);
            Controls.Add(label_titulo);
            Name = "Form_Conf_Desativação";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label_titulo;
        private Label label_mensagemConfirmacao;
        private Button button_confirmarDesativacao;
        private Button button_cancelarDesativacao;
    }
}
