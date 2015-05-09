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
    public partial class AutoEncoder : Form
    {
        public static AutoEncoder autoEnc;

        TextBox processDialog;
        MyAutoEncode myAutoEncode;
        MyExtApplication myExt;


        public AutoEncoder()
        {
            InitializeComponent();

            myAutoEncode = new MyAutoEncode();
            myExt = new MyExtApplication();
            autoEnc = this;

            processDialog = this.ProcessDialogTextBox;
        }

        public static AutoEncoder GetInstance()
        {
            return autoEnc;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                myAutoEncode.encodeMovie();

                // task終了時の処理を登録
                //task.ContinueWith(OnTaskCompleted);
            }
            catch(Exception ex)
            {
                MyErrorHandling.showErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// テキストボックスに文字を追加
        /// </summary>
        /// <param name="text"></param>
        /// <param name="textBox"></param>
        private void AddText(string text, TextBox textBox)
        {
            textBox.AppendText(text + "\r\n");
        }

        /// <summary>
        /// 文字が出力されたことを検知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">e.Dataで出力された文字を検知</param>
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
