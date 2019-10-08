using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calculator
{
    public partial class Notepad : Form
    {
        private string path = "";       //path가 ""인 경우 새로 만들어지거나 처음인 경우
        private enum State { StandBy, Writing };
        private State state;
        private bool isChanged = false;
        private bool isLoaded = false;

        public Notepad()
        {
            InitializeComponent();
            state = State.Writing;
            Text = "제목 없음";
            path = "";
            isChanged = false;
            isLoaded = false;
        }

        private void SaveCheck(object sender, EventArgs e)
        {
            if(isChanged)
            {
                var result = MessageBox.Show("변경된 내용을 저장하시겠습니까?", "알림", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (path == "")//파일이 열린 경우가 아니라면
                        SaveAs_Click(sender, e);
                    else
                        Save_Click(sender, e);
                }
            }
        }

        private void NewFile_Click(object sender, EventArgs e)
        {
            SaveCheck(sender, e);

            Text = "제목 없음";
            TextBox.Text = "";
            path = "";

            state = State.Writing;
            isChanged = false;

            isLoaded = true;
        }

        private void FileOpen_Click(object sender, EventArgs e)
        {
            SaveCheck(sender, e);

            try
            {
                using (OpenFileDialog fileDialog = new OpenFileDialog()
                {
                    Title = "열기",
                    Filter = "텍스트 파일 (*.txt)|*.txt|모든 파일 (*.*)|*.*"
                })
                {
                    fileDialog.ShowDialog();
                    path = fileDialog.FileName;
                    Text = (System.IO.Path.GetFileName(path) == "") ? Text : System.IO.Path.GetFileName(path);
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                throw;
            }

            if (path != "")
            {
                using (var reader = new System.IO.StreamReader(path))
                {
                    var file = reader.ReadToEnd();
                    TextBox.Text = file;
                }

                state = State.StandBy;
                isChanged = false;
            }
        }


        private void Save_Click(object sender, EventArgs e)
        {
            if (!isChanged) return;

            if (path == "")
            {
                SaveAs_Click(sender, e);
                return;
            }

            using (var writer = new System.IO.StreamWriter(path))
            {
                writer.Write(TextBox.Text);
            }

            state = State.Writing;
            Text = Text.Substring(0, Text.Length - 1);
            isChanged = false;
        }

        private void SaveAs_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog fileDialog = new SaveFileDialog()
            {
                Title = "다른 이름으로 저장",
                Filter = "텍스트 파일 (*.txt)|*.txt|모든 파일 (*.*)|*.*"
            })
            {
                fileDialog.ShowDialog();
                path = fileDialog.FileName;
            }

            if (path != "")
            {
                Text = System.IO.Path.GetFileName(path);

                using (var writer = new System.IO.StreamWriter(path))
                {
                    writer.Write(TextBox.Text);
                }

                if (state == State.Writing)
                {
                    state = State.StandBy;
                    isChanged = false;
                }
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            SaveCheck(sender, e);
            Application.Exit();
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if(!isLoaded)
            {
                state = State.Writing;
                if (state == State.Writing)
                {
                    if (!isChanged)
                    {
                        Text += "*";
                        isChanged = true;
                    }
                }
            }

            isLoaded = false;
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox.SelectedText);
            TextBox.Select(TextBox.TextLength, 1);
        }

        private void Cut_Click(object sender, EventArgs e)
        {
            string strCopy = TextBox.Text;
            int startIndex = TextBox.SelectionStart;
            int length = TextBox.SelectedText.Length;

            Clipboard.SetText(TextBox.SelectedText);
            TextBox.Text = strCopy.Substring(0, startIndex);
            TextBox.Text += strCopy.Substring(startIndex + length);
            TextBox.Select(TextBox.TextLength, 1);
        }

        private void Paste_Click(object sender, EventArgs e)
        {
            TextBox.Text = TextBox.Text.Insert(TextBox.SelectionStart, Clipboard.GetText());
            TextBox.Select(TextBox.TextLength, 1);
        }

        private void FontSetting_Click(object sender, EventArgs e)
        {
            using (FontDialog fontDialog = new FontDialog())
            {
                if (DialogResult.OK == fontDialog.ShowDialog())
                {
                    TextBox.Font = fontDialog.Font;
                }
            }
        }

        private void Notepad_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveCheck(sender, e);
        }

        private void Infomation_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Kiheyunkim@gmail.com\n제작자:김기현", "정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
