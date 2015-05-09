using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoEncoder
{
    public partial class Form1 : Form
    {
        TextBox processDialog;
        MyAutoEncode autoEncode;
        MyExtApplication myExt;


        public Form1()
        {
            InitializeComponent();

            autoEncode = new MyAutoEncode(this);
            myExt = new MyExtApplication(this);

            processDialog = this.ProcessDialogTextBox;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                autoEncode.encodeMovie();

                // task終了時の処理を登録
                //task.ContinueWith(OnTaskCompleted);
            }
            catch(Exception ex)
            {
                MyErrorHandling.showErrorMessage(ex.Message);
            }
        }

        private void AddText(string text, TextBox textBox)
        {
            textBox.AppendText(text + "\r\n");
        }

        public void process_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);

            if (e.Data != null)
            {
                InvokeIfRequired(
                    (Action)delegate()
                    {
                        AddText(e.Data, processDialog);
                    }, true);
            }
            else
            {
                // タスクをキャンセル（無限ループを脱出するため）
                autoEncode.tokenSource.Cancel(true);
                autoEncode.tokenSource.Dispose();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void ProcessDialogTextBox_TextChanged(object sender, EventArgs e)
        {
        }


        /// <summary>
        /// エンコードが終了した際の処理
        /// </summary>
        /// <param name="task">結果</param>
        private void OnTaskCompleted(Task<bool> task)
        {
            if(task.Status == TaskStatus.RanToCompletion)
            {
                if (task.Result == false)
                {
                    throw new Exception("エンコードが失敗しました");
                }
                else
                {
                    MyErrorHandling.showInfoMessage("エンコードが終了しました");
                }
            }
        }

        /// <summary>
        /// メインスレッド外からUIにアクセスする場合はInvokeする
        /// </summary>
        /// <param name="action">処理</param>
        /// <param name="isAsync">非同期かどうか true:非同期, false:動機</param>
        private void InvokeIfRequired(Action action, bool isAsync)
        {
            if(this.InvokeRequired == true)
            {
                if (isAsync == true)
                {
                    BeginInvoke(action);
                }
                else
                {
                    Invoke(action);
                }
            }
            else
            {
                action();
            }
        }
    }
}
