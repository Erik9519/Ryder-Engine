
namespace Ryder_Engine.Forms
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.msiCheckBox = new System.Windows.Forms.CheckBox();
            this.foregroundMonitorCheckBox = new System.Windows.Forms.CheckBox();
            this.networkMonitorCheckBox = new System.Windows.Forms.CheckBox();
            this.storageMonitorCheckBox = new System.Windows.Forms.CheckBox();
            this.comPortMonitorCheckBox = new System.Windows.Forms.CheckBox();
            this.applyButton = new System.Windows.Forms.Button();
            this.comPort_comboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pswTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // msiCheckBox
            // 
            this.msiCheckBox.AutoSize = true;
            this.msiCheckBox.Location = new System.Drawing.Point(12, 12);
            this.msiCheckBox.Name = "msiCheckBox";
            this.msiCheckBox.Size = new System.Drawing.Size(90, 19);
            this.msiCheckBox.TabIndex = 0;
            this.msiCheckBox.Text = "MSI module";
            this.msiCheckBox.UseVisualStyleBackColor = true;
            // 
            // foregroundMonitorCheckBox
            // 
            this.foregroundMonitorCheckBox.AutoSize = true;
            this.foregroundMonitorCheckBox.Location = new System.Drawing.Point(12, 37);
            this.foregroundMonitorCheckBox.Name = "foregroundMonitorCheckBox";
            this.foregroundMonitorCheckBox.Size = new System.Drawing.Size(203, 19);
            this.foregroundMonitorCheckBox.TabIndex = 1;
            this.foregroundMonitorCheckBox.Text = "Foreground App Monitor module";
            this.foregroundMonitorCheckBox.UseVisualStyleBackColor = true;
            // 
            // networkMonitorCheckBox
            // 
            this.networkMonitorCheckBox.AutoSize = true;
            this.networkMonitorCheckBox.Location = new System.Drawing.Point(12, 62);
            this.networkMonitorCheckBox.Name = "networkMonitorCheckBox";
            this.networkMonitorCheckBox.Size = new System.Drawing.Size(161, 19);
            this.networkMonitorCheckBox.TabIndex = 2;
            this.networkMonitorCheckBox.Text = "Network Monitor module";
            this.networkMonitorCheckBox.UseVisualStyleBackColor = true;
            // 
            // storageMonitorCheckBox
            // 
            this.storageMonitorCheckBox.AutoSize = true;
            this.storageMonitorCheckBox.Location = new System.Drawing.Point(12, 87);
            this.storageMonitorCheckBox.Name = "storageMonitorCheckBox";
            this.storageMonitorCheckBox.Size = new System.Drawing.Size(156, 19);
            this.storageMonitorCheckBox.TabIndex = 3;
            this.storageMonitorCheckBox.Text = "Storage Monitor module";
            this.storageMonitorCheckBox.UseVisualStyleBackColor = true;
            // 
            // comPortMonitorCheckBox
            // 
            this.comPortMonitorCheckBox.AutoSize = true;
            this.comPortMonitorCheckBox.Location = new System.Drawing.Point(12, 112);
            this.comPortMonitorCheckBox.Name = "comPortMonitorCheckBox";
            this.comPortMonitorCheckBox.Size = new System.Drawing.Size(125, 19);
            this.comPortMonitorCheckBox.TabIndex = 4;
            this.comPortMonitorCheckBox.Text = "COM Port Monitor";
            this.comPortMonitorCheckBox.UseVisualStyleBackColor = true;
            // 
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(84, 175);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(75, 23);
            this.applyButton.TabIndex = 5;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // comPort_comboBox
            // 
            this.comPort_comboBox.FormattingEnabled = true;
            this.comPort_comboBox.Location = new System.Drawing.Point(159, 110);
            this.comPort_comboBox.Name = "comPort_comboBox";
            this.comPort_comboBox.Size = new System.Drawing.Size(72, 23);
            this.comPort_comboBox.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 143);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = "Password";
            // 
            // pswTextBox
            // 
            this.pswTextBox.Location = new System.Drawing.Point(73, 139);
            this.pswTextBox.MaxLength = 15;
            this.pswTextBox.Name = "pswTextBox";
            this.pswTextBox.Size = new System.Drawing.Size(158, 23);
            this.pswTextBox.TabIndex = 8;
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(243, 208);
            this.Controls.Add(this.pswTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comPort_comboBox);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.comPortMonitorCheckBox);
            this.Controls.Add(this.storageMonitorCheckBox);
            this.Controls.Add(this.networkMonitorCheckBox);
            this.Controls.Add(this.foregroundMonitorCheckBox);
            this.Controls.Add(this.msiCheckBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox msiCheckBox;
        private System.Windows.Forms.CheckBox foregroundMonitorCheckBox;
        private System.Windows.Forms.CheckBox networkMonitorCheckBox;
        private System.Windows.Forms.CheckBox storageMonitorCheckBox;
        private System.Windows.Forms.CheckBox comPortMonitorCheckBox;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.ComboBox comPort_comboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox pswTextBox;
    }
}