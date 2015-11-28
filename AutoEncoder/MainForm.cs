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
    public partial class MainForm : Form
    {
        public static MainForm form;
        public static TextBox processDialog;
        public static Label processStatus;
        public static ProgressBar processProgress;

        MyAutoEncode myAutoEncode;
        MyExtApplication myExt;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            form = this;
        }

        /// <summary>
        /// フォームロード時の初期設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            myExt = new MyExtApplication();

            processDialog = this.ProcessDialogTextBox;
            processStatus = this.ProcessStatusLabel;
            processProgress = this.ProcessProgressBar;
            
            // プログレスバーを初期値に設定
            processProgress.Maximum = 8;
            processProgress.Minimum = 0;
        }

        /// <summary>
        /// エンコード開始ボタンをクリック後の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            myAutoEncode = new MyAutoEncode();
            myAutoEncode.EncodeMovie();
        }

        /// <summary>
        /// テキストボックスに文字を追加
        /// </summary>
        /// <param name="text"></param>
        /// <param name="textBox"></param>
        public void AddText(string text, TextBox textBox)
        {
            textBox.AppendText(text + "\r\n");
        }

        /// <summary>
        /// ラベルに文字を表示
        /// </summary>
        /// <param name="text"></param>
        /// <param name="textBox"></param>
        public void UpdateLabel(string text, Label label)
        {
            label.Text = text;
        }

        /// <summary>
        /// プログレスバーを進める
        /// </summary>
        /// <param name="progress">プログレスバー</param>
        public void AddProgressBar(int intProgress, ProgressBar progress)
        {
            progress.Value += intProgress;
        }

        /// <summary>
        /// 外部メソッドからテキストボックスを操作する（文字列表示）
        /// </summary>
        /// <param name="strMessage">表示するメッセージの内容</param>
        public void InvokeAddText(Action<string, TextBox> controlModifier, string strMessage, TextBox textBox)
        {
            InvokeIfRequired(
                new Action(()=>
                {
                    controlModifier(strMessage, textBox);
                }), true);
        }

        /// <summary>
        /// 外部メソッドからラベルを操作する（文字列表示）
        /// </summary>
        /// <param name="strMessage">表示するメッセージの内容</param>
        public void InvokeUpdateLabel(Action<string, Label> controlModifier, string strMessage, Label label)
        {
            InvokeIfRequired(
                new Action(()=>
                {
                    controlModifier(strMessage, label);
                }), true);
        }

        /// <summary>
        /// 外部メソッドからプログレスバーを動かす
        /// </summary>
        /// <param name="controlModifier"></param>
        /// <param name="intProgress"></param>
        /// <param name="progress"></param>
        public void InvokeAddProgress (Action<int, ProgressBar> controlModifier, int intProgress, ProgressBar progress)
        {
            InvokeIfRequired(
                new Action(()=>
                {
                    controlModifier(intProgress, progress);
                }), true);
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
                // 文字の出力が終わっていない場合

                InvokeIfRequired(
                    (Action)delegate ()
                    {
                        AddText(e.Data, processDialog);
                    }, true);
            }
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
