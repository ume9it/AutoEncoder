using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Xml.Linq;

namespace AutoEncoder
{
    class MyExtApplication : MyBaseClass
    {
        public MyExtApplication()
        {
        }

        /// <summary>
        /// 外部アプリケーションを起動するためのプロセスを作成し、コマンドライン引数を渡した状態で起動する
        /// （カレントディレクトリ内のアプリケーション）
        /// </summary>
        /// <param name="strAppName">アプリケーション名</param>
        /// <param name="strAppArg">コマンドライン引数</param>
        /// <returns>外部アプリケーションの標準出力</returns>
        public void ProcessStart(string strAppName, string strAppArg = "")
        {
            ProcessStart(strAppName, Program.strGLCurrentDirectory, strAppArg);
        }

        /// <summary>
        /// 外部アプリケーションを起動するためのプロセスを作成し、コマンドライン引数を渡した状態で起動する
        /// （アプリケーションのディレクトリを指定して起動）
        /// </summary>
        /// <param name="strAppName">アプリケーション名</param>
        /// <param name="strAppDir">アプリケーションの格納されているディレクトリ</param>
        /// <param name="strAppArg">コマンドライン引数</param>
        /// <returns>外部アプリケーションの標準出力</returns>
        public void ProcessStart(string strAppName, string strAppDir, string strAppArg = "")
        {
            try
            {
                // プロセスのオブジェクトを作成
                System.Diagnostics.Process process = new System.Diagnostics.Process();

                // 外部アプリケーションをプロセスのオブジェクトに関連付ける
                process.StartInfo.FileName = Path.Combine(strAppDir, strAppName);

                // プロセスに渡す引数を設定
                process.StartInfo.Arguments = strAppArg;

                // 標準出力を受け取る設定
                process.StartInfo.RedirectStandardOutput = true;

                // 実行時にシェルを使うかの設定（ウインドウを隠すときはfalseにしなければならない）
                process.StartInfo.UseShellExecute = false;

                // ウインドウを隠すかどうかの設定
                process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

                // 外部アプリケーションの画面を非表示（GUIがない場合）
                process.StartInfo.CreateNoWindow = true;

                // 起動失敗時にエラーダイアログを表示
                process.StartInfo.ErrorDialog = true;

                // 標準出力を取得するイベントハンドラ
                process.OutputDataReceived += MainForm.GetInstance().process_OutputDataReceived;

                process.Start();
                
                process.BeginOutputReadLine();

                process.WaitForExit();

            }
            catch(Exception ex)
            {
                MyErrorHandling.showErrorMessage(ex.Message, ex);
            }
        }

        /// <summary>
        /// 外部アプリケーションを起動するための引数やファイルパスなどを設定し、プロセスの作成メソッドへ値を渡す
        /// </summary>
        /// <param name="strAppName">外部アプリケーション名</param>
        /// <param name="strInputFileName">入力ファイル名（ファイル名のみ）</param>
        /// <param name="strOutputFileName">出力ファイル名（ファイル名のみ）</param>
        /// <param name="strArguments">外部アプリケーションに渡す引数</param>
        /// <returns>アプリケーションの出力したメッセージ</returns>
        public void RunExternalApp(string strAppName, string strInputFileName, string strOutputFileName)
        {
            // 外部アプリケーションのパス
            string strExtAppPath = MyReadConfig.ReadConfig(strGLConfigExePath, strAppName, EP_NODE_PATH);

            // 外部アプリケーション実行時のオプション（入力、出力以外の引数オプション）
            string strExtAppStartOption = MyReadConfig.ReadConfig(strGLConfigExePath, strAppName, EP_NODE_OPTION);

            // コマンドライン引数を作成
            string strArguments = MakeAppArgs(strAppName, strInputFileName, strOutputFileName);

            // 設定したコマンドライン引数を渡して外部アプリケーションを起動
            ProcessStart(strExtAppPath, strArguments);
        }

        /// <summary>
        /// ExePath.configを読み込み、データを置換・結合し、コマンドライン引数を作成する
        /// </summary>
        /// <param name="strAppName">アプリケーション名</param>
        /// <param name="strInputFileName">入力ファイル名（パス、拡張子を除いたファイル名のみ）</param>
        /// <param name="strOutputFileName">出力ファイル名（パス、拡張子を除いたファイル名のみ）</param>
        /// <returns>各アプリケーションのコマンドライン引数</returns>
        public string MakeAppArgs(string strAppName, string strInputFileName, string strOutputFileName)
        {
            IEnumerable<XElement> enumInputElements = MyReadConfig.GetConfigXElement(strGLConfigExePath, strAppName, EP_NODE_INPUT, EP_NODE_FILE_ARG);
            IEnumerable<XElement> enumOutputElements = MyReadConfig.GetConfigXElement(strGLConfigExePath, strAppName, EP_NODE_OUTPUT, EP_NODE_FILE_ARG);
            IEnumerable<XElement> enumOptionElements = MyReadConfig.GetConfigXElement(strGLConfigExePath, strAppName, EP_NODE_OPTION);

            string strInputArgs = String.Empty;
            string strOutputArgs = String.Empty;
            string strOptionArgs = String.Empty;

            foreach(XElement xInputItem in enumInputElements)
            {
                strInputArgs += xInputItem.Value
                    .Replace("{Input}", strGLWorkDir + strInputFileName)
                    .Replace("{Ext}", xInputItem.Attribute(EP_ATTRIBUTE_FILE_EXT).Value);
            }

            foreach(XElement xOutputItem in enumOutputElements)
            {
                strOutputArgs += xOutputItem.Value
                    .Replace("{Output}", strGLWorkDir + strOutputFileName)
                    .Replace("{Ext}", xOutputItem.Attribute(EP_ATTRIBUTE_FILE_EXT).Value);
            }

            foreach (XElement xOptionItem in enumOptionElements)
            {
                strOptionArgs += xOptionItem.Value;
            }

            string strUnionArgument = strInputArgs + strOutputArgs + strOptionArgs;

            return strUnionArgument;
        }
    }
}
