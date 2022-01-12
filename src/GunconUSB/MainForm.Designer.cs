
namespace GunconUSB
{
    partial class MainForm
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
            this.checkBoxA = new System.Windows.Forms.CheckBox();
            this.checkBoxB = new System.Windows.Forms.CheckBox();
            this.checkBoxC = new System.Windows.Forms.CheckBox();
            this.checkBoxTrigger = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxRunning = new System.Windows.Forms.CheckBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.checkBoxSelect = new System.Windows.Forms.CheckBox();
            this.checkBoxStart = new System.Windows.Forms.CheckBox();
            this.textBoxPointerY = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxPointerX = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownPadX = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownPadY = new System.Windows.Forms.NumericUpDown();
            this.textBoxMinY = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxMinX = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxMaxY = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxMaxX = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxVjoyInfo = new System.Windows.Forms.TextBox();
            this.rbMoveNone = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbMoveMouse = new System.Windows.Forms.RadioButton();
            this.rbMoveJoy = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnCalibrate = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPadX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPadY)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxA
            // 
            this.checkBoxA.AutoSize = true;
            this.checkBoxA.Enabled = false;
            this.checkBoxA.Location = new System.Drawing.Point(11, 19);
            this.checkBoxA.Name = "checkBoxA";
            this.checkBoxA.Size = new System.Drawing.Size(33, 17);
            this.checkBoxA.TabIndex = 0;
            this.checkBoxA.Text = "A";
            this.checkBoxA.UseVisualStyleBackColor = true;
            // 
            // checkBoxB
            // 
            this.checkBoxB.AutoSize = true;
            this.checkBoxB.Enabled = false;
            this.checkBoxB.Location = new System.Drawing.Point(89, 19);
            this.checkBoxB.Name = "checkBoxB";
            this.checkBoxB.Size = new System.Drawing.Size(33, 17);
            this.checkBoxB.TabIndex = 1;
            this.checkBoxB.Text = "B";
            this.checkBoxB.UseVisualStyleBackColor = true;
            // 
            // checkBoxC
            // 
            this.checkBoxC.AutoSize = true;
            this.checkBoxC.Enabled = false;
            this.checkBoxC.Location = new System.Drawing.Point(50, 19);
            this.checkBoxC.Name = "checkBoxC";
            this.checkBoxC.Size = new System.Drawing.Size(33, 17);
            this.checkBoxC.TabIndex = 2;
            this.checkBoxC.Text = "C";
            this.checkBoxC.UseVisualStyleBackColor = true;
            // 
            // checkBoxTrigger
            // 
            this.checkBoxTrigger.AutoSize = true;
            this.checkBoxTrigger.Enabled = false;
            this.checkBoxTrigger.Location = new System.Drawing.Point(128, 19);
            this.checkBoxTrigger.Name = "checkBoxTrigger";
            this.checkBoxTrigger.Size = new System.Drawing.Size(59, 17);
            this.checkBoxTrigger.TabIndex = 3;
            this.checkBoxTrigger.Text = "Trigger";
            this.checkBoxTrigger.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "PadX";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "PadY";
            // 
            // checkBoxRunning
            // 
            this.checkBoxRunning.AutoSize = true;
            this.checkBoxRunning.Enabled = false;
            this.checkBoxRunning.Location = new System.Drawing.Point(175, 16);
            this.checkBoxRunning.Name = "checkBoxRunning";
            this.checkBoxRunning.Size = new System.Drawing.Size(66, 17);
            this.checkBoxRunning.TabIndex = 8;
            this.checkBoxRunning.Text = "Running";
            this.checkBoxRunning.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(13, 12);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 9;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(94, 12);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 10;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // checkBoxSelect
            // 
            this.checkBoxSelect.AutoSize = true;
            this.checkBoxSelect.Enabled = false;
            this.checkBoxSelect.Location = new System.Drawing.Point(247, 19);
            this.checkBoxSelect.Name = "checkBoxSelect";
            this.checkBoxSelect.Size = new System.Drawing.Size(56, 17);
            this.checkBoxSelect.TabIndex = 11;
            this.checkBoxSelect.Text = "Select";
            this.checkBoxSelect.UseVisualStyleBackColor = true;
            // 
            // checkBoxStart
            // 
            this.checkBoxStart.AutoSize = true;
            this.checkBoxStart.Enabled = false;
            this.checkBoxStart.Location = new System.Drawing.Point(193, 19);
            this.checkBoxStart.Name = "checkBoxStart";
            this.checkBoxStart.Size = new System.Drawing.Size(48, 17);
            this.checkBoxStart.TabIndex = 12;
            this.checkBoxStart.Text = "Start";
            this.checkBoxStart.UseVisualStyleBackColor = true;
            // 
            // textBoxPointerY
            // 
            this.textBoxPointerY.Location = new System.Drawing.Point(170, 71);
            this.textBoxPointerY.Name = "textBoxPointerY";
            this.textBoxPointerY.Size = new System.Drawing.Size(100, 20);
            this.textBoxPointerY.TabIndex = 16;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(114, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Pointer Y";
            // 
            // textBoxPointerX
            // 
            this.textBoxPointerX.Location = new System.Drawing.Point(170, 45);
            this.textBoxPointerX.Name = "textBoxPointerX";
            this.textBoxPointerX.Size = new System.Drawing.Size(100, 20);
            this.textBoxPointerX.TabIndex = 14;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(114, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Pointer X";
            // 
            // numericUpDownPadX
            // 
            this.numericUpDownPadX.Enabled = false;
            this.numericUpDownPadX.Location = new System.Drawing.Point(49, 45);
            this.numericUpDownPadX.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownPadX.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numericUpDownPadX.Name = "numericUpDownPadX";
            this.numericUpDownPadX.Size = new System.Drawing.Size(43, 20);
            this.numericUpDownPadX.TabIndex = 17;
            // 
            // numericUpDownPadY
            // 
            this.numericUpDownPadY.Enabled = false;
            this.numericUpDownPadY.Location = new System.Drawing.Point(49, 71);
            this.numericUpDownPadY.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownPadY.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numericUpDownPadY.Name = "numericUpDownPadY";
            this.numericUpDownPadY.Size = new System.Drawing.Size(43, 20);
            this.numericUpDownPadY.TabIndex = 18;
            // 
            // textBoxMinY
            // 
            this.textBoxMinY.Location = new System.Drawing.Point(47, 137);
            this.textBoxMinY.Name = "textBoxMinY";
            this.textBoxMinY.Size = new System.Drawing.Size(100, 20);
            this.textBoxMinY.TabIndex = 23;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 140);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "Min Y";
            // 
            // textBoxMinX
            // 
            this.textBoxMinX.Location = new System.Drawing.Point(47, 111);
            this.textBoxMinX.Name = "textBoxMinX";
            this.textBoxMinX.Size = new System.Drawing.Size(100, 20);
            this.textBoxMinX.TabIndex = 21;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 114);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "Min X";
            // 
            // textBoxMaxY
            // 
            this.textBoxMaxY.Location = new System.Drawing.Point(211, 137);
            this.textBoxMaxY.Name = "textBoxMaxY";
            this.textBoxMaxY.Size = new System.Drawing.Size(100, 20);
            this.textBoxMaxY.TabIndex = 27;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(171, 140);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 13);
            this.label7.TabIndex = 26;
            this.label7.Text = "Max Y";
            // 
            // textBoxMaxX
            // 
            this.textBoxMaxX.Location = new System.Drawing.Point(211, 111);
            this.textBoxMaxX.Name = "textBoxMaxX";
            this.textBoxMaxX.Size = new System.Drawing.Size(100, 20);
            this.textBoxMaxX.TabIndex = 25;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(171, 114);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 13);
            this.label8.TabIndex = 24;
            this.label8.Text = "Max X";
            // 
            // textBoxVjoyInfo
            // 
            this.textBoxVjoyInfo.Location = new System.Drawing.Point(10, 163);
            this.textBoxVjoyInfo.Multiline = true;
            this.textBoxVjoyInfo.Name = "textBoxVjoyInfo";
            this.textBoxVjoyInfo.ReadOnly = true;
            this.textBoxVjoyInfo.Size = new System.Drawing.Size(375, 55);
            this.textBoxVjoyInfo.TabIndex = 28;
            // 
            // rbMoveNone
            // 
            this.rbMoveNone.AutoSize = true;
            this.rbMoveNone.Checked = true;
            this.rbMoveNone.Location = new System.Drawing.Point(8, 19);
            this.rbMoveNone.Name = "rbMoveNone";
            this.rbMoveNone.Size = new System.Drawing.Size(66, 17);
            this.rbMoveNone.TabIndex = 29;
            this.rbMoveNone.TabStop = true;
            this.rbMoveNone.Text = "Disabled";
            this.rbMoveNone.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbMoveMouse);
            this.groupBox1.Controls.Add(this.rbMoveJoy);
            this.groupBox1.Controls.Add(this.rbMoveNone);
            this.groupBox1.Location = new System.Drawing.Point(14, 41);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(227, 49);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Movement";
            // 
            // rbMoveMouse
            // 
            this.rbMoveMouse.AutoSize = true;
            this.rbMoveMouse.Location = new System.Drawing.Point(161, 19);
            this.rbMoveMouse.Name = "rbMoveMouse";
            this.rbMoveMouse.Size = new System.Drawing.Size(57, 17);
            this.rbMoveMouse.TabIndex = 31;
            this.rbMoveMouse.Text = "Mouse";
            this.rbMoveMouse.UseVisualStyleBackColor = true;
            // 
            // rbMoveJoy
            // 
            this.rbMoveJoy.AutoSize = true;
            this.rbMoveJoy.Location = new System.Drawing.Point(86, 19);
            this.rbMoveJoy.Name = "rbMoveJoy";
            this.rbMoveJoy.Size = new System.Drawing.Size(63, 17);
            this.rbMoveJoy.TabIndex = 30;
            this.rbMoveJoy.Text = "Joystick";
            this.rbMoveJoy.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.checkBoxStart);
            this.groupBox2.Controls.Add(this.textBoxVjoyInfo);
            this.groupBox2.Controls.Add(this.checkBoxSelect);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.textBoxMaxY);
            this.groupBox2.Controls.Add(this.textBoxPointerX);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.checkBoxTrigger);
            this.groupBox2.Controls.Add(this.checkBoxC);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.checkBoxB);
            this.groupBox2.Controls.Add(this.textBoxMaxX);
            this.groupBox2.Controls.Add(this.checkBoxA);
            this.groupBox2.Controls.Add(this.textBoxPointerY);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.numericUpDownPadX);
            this.groupBox2.Controls.Add(this.textBoxMinY);
            this.groupBox2.Controls.Add(this.numericUpDownPadY);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.textBoxMinX);
            this.groupBox2.Location = new System.Drawing.Point(12, 96);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(399, 233);
            this.groupBox2.TabIndex = 31;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Debug";
            // 
            // btnCalibrate
            // 
            this.btnCalibrate.Location = new System.Drawing.Point(248, 12);
            this.btnCalibrate.Name = "btnCalibrate";
            this.btnCalibrate.Size = new System.Drawing.Size(75, 23);
            this.btnCalibrate.TabIndex = 32;
            this.btnCalibrate.Text = "Calibrate";
            this.btnCalibrate.UseVisualStyleBackColor = true;
            this.btnCalibrate.Click += new System.EventHandler(this.btnCalibrate_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 338);
            this.Controls.Add(this.btnCalibrate);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.checkBoxRunning);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Guncon2";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPadX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPadY)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxA;
        private System.Windows.Forms.CheckBox checkBoxB;
        private System.Windows.Forms.CheckBox checkBoxC;
        private System.Windows.Forms.CheckBox checkBoxTrigger;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxRunning;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.CheckBox checkBoxSelect;
        private System.Windows.Forms.CheckBox checkBoxStart;
        private System.Windows.Forms.TextBox textBoxPointerY;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxPointerX;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericUpDownPadX;
        private System.Windows.Forms.NumericUpDown numericUpDownPadY;
        private System.Windows.Forms.TextBox textBoxMinY;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxMinX;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxMaxY;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxMaxX;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxVjoyInfo;
        private System.Windows.Forms.RadioButton rbMoveNone;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbMoveMouse;
        private System.Windows.Forms.RadioButton rbMoveJoy;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnCalibrate;
    }
}

