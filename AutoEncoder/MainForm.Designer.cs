namespace AutoEncoder
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.ProcessDialogTextBox = new System.Windows.Forms.TextBox();
            this.ProcessStatusLabel = new System.Windows.Forms.Label();
            this.ProcessProgressBar = new System.Windows.Forms.ProgressBar();
            this.TotalProcessorTimeLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(619, 486);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 44);
            this.button1.TabIndex = 0;
            this.button1.Text = "encode";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ProcessDialogTextBox
            // 
            this.ProcessDialogTextBox.AcceptsReturn = true;
            this.ProcessDialogTextBox.AcceptsTab = true;
            this.ProcessDialogTextBox.Location = new System.Drawing.Point(53, 43);
            this.ProcessDialogTextBox.Multiline = true;
            this.ProcessDialogTextBox.Name = "ProcessDialogTextBox";
            this.ProcessDialogTextBox.ReadOnly = true;
            this.ProcessDialogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ProcessDialogTextBox.Size = new System.Drawing.Size(660, 352);
            this.ProcessDialogTextBox.TabIndex = 1;
            this.ProcessDialogTextBox.TextChanged += new System.EventHandler(this.ProcessDialogTextBox_TextChanged);
            // 
            // ProcessStatusLabel
            // 
            this.ProcessStatusLabel.AutoSize = true;
            this.ProcessStatusLabel.Location = new System.Drawing.Point(51, 486);
            this.ProcessStatusLabel.Name = "ProcessStatusLabel";
            this.ProcessStatusLabel.Size = new System.Drawing.Size(175, 12);
            this.ProcessStatusLabel.TabIndex = 2;
            this.ProcessStatusLabel.Text = "アプリケーションは起動されていません\r\n";
            // 
            // ProcessProgressBar
            // 
            this.ProcessProgressBar.Location = new System.Drawing.Point(53, 429);
            this.ProcessProgressBar.Name = "ProcessProgressBar";
            this.ProcessProgressBar.Size = new System.Drawing.Size(660, 30);
            this.ProcessProgressBar.TabIndex = 3;
            // 
            // TotalProcessorTimeLabel
            // 
            this.TotalProcessorTimeLabel.AutoSize = true;
            this.TotalProcessorTimeLabel.Location = new System.Drawing.Point(51, 518);
            this.TotalProcessorTimeLabel.Name = "TotalProcessorTimeLabel";
            this.TotalProcessorTimeLabel.Size = new System.Drawing.Size(123, 12);
            this.TotalProcessorTimeLabel.TabIndex = 4;
            this.TotalProcessorTimeLabel.Text = "合計起動時間：00:00:00";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(767, 556);
            this.Controls.Add(this.TotalProcessorTimeLabel);
            this.Controls.Add(this.ProcessProgressBar);
            this.Controls.Add(this.ProcessStatusLabel);
            this.Controls.Add(this.ProcessDialogTextBox);
            this.Controls.Add(this.button1);
            this.Name = "MainForm";
            this.Text = "AutoEncoder";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.TextBox ProcessDialogTextBox;
        public System.Windows.Forms.Label ProcessStatusLabel;
        public System.Windows.Forms.ProgressBar ProcessProgressBar;
        public System.Windows.Forms.Label TotalProcessorTimeLabel;
    }
}

