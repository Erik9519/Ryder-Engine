
namespace Ryder_Engine.Forms
{
    partial class JSONViewer
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
            this.closeButton = new System.Windows.Forms.Button();
            this.jsonTextBox = new System.Windows.Forms.Label();
            this.jsonPanel = new System.Windows.Forms.Panel();
            this.jsonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(172, 652);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 1;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // jsonTextBox
            // 
            this.jsonTextBox.AutoSize = true;
            this.jsonTextBox.Location = new System.Drawing.Point(4, 3);
            this.jsonTextBox.Name = "jsonTextBox";
            this.jsonTextBox.Size = new System.Drawing.Size(30, 15);
            this.jsonTextBox.TabIndex = 2;
            this.jsonTextBox.Text = "Json";
            // 
            // jsonPanel
            // 
            this.jsonPanel.AutoScroll = true;
            this.jsonPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.jsonPanel.Controls.Add(this.jsonTextBox);
            this.jsonPanel.Location = new System.Drawing.Point(12, 12);
            this.jsonPanel.Name = "jsonPanel";
            this.jsonPanel.Size = new System.Drawing.Size(394, 634);
            this.jsonPanel.TabIndex = 3;
            // 
            // JSONViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 687);
            this.ControlBox = false;
            this.Controls.Add(this.jsonPanel);
            this.Controls.Add(this.closeButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "JSONViewer";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "JSONViewer";
            this.jsonPanel.ResumeLayout(false);
            this.jsonPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Label jsonTextBox;
        private System.Windows.Forms.Panel jsonPanel;
    }
}