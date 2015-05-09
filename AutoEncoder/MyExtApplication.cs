using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

namespace AutoEncoder
{
    class MyExtApplication : MyBaseClass
    {
        public MyExtApplication(Form1 f)
        {
            form1 = f;
        }

        /// <summary>
        /// 外部アプリケーションを起動するためのプロセスを作成し、コマンドライン引数を渡した状態で起動する
        /// （カレントディレクトリ内のアプリケーション）
        /// </summary>
        /// <param name="strAppName">アプリケーション名</param>
        /// <param name="strAppArg">コマンドライン引数</param>
        /// <returns>外部アプリケーションの標準出力</returns>
        public void processStart(string strAppName, string strAppArg = "")
        {
            processStart(strAppName, System.Environment.CurrentDirectory, strAppArg);
        }

        /// <summary>
        /// 外部アプリケーションを起動するためのプロセスを作成し、コマンドライン引数を渡した状態で起動する
        /// （アプリケーションのディレクトリを指定して起動）
        /// </summary>
        /// <param name="strAppName">アプリケーション名</param>
        /// <param name="strAppDir">アプリケーションの格納されているディレクトリ</param>
        /// <param name="strAppArg">コマンドライン引数</param>
        /// <returns>外部アプリケーションの標準出力</returns>
        public void processStart(string strAppName, string strAppDir, string strAppArg = "")
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
                process.OutputDataReceived += form1.process_OutputDataReceived;

                process.Start();
                
                process.BeginOutputReadLine();

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
        public void runExternalApp(string strAppName, string strInputFileName, string strOutputFileName)
        {
            // 外部アプリケーションのパス
            string strExtAppPath = MyReadConfig.readConfig(strGLConfigExePath, strAppName, EP_NODE_PATH);

            // 外部アプリケーション実行時のオプション（入力、出力以外の引数オプション）
            string strExtAppStartOption = MyReadConfig.readConfig(strGLConfigExePath, strAppName, EP_NODE_OPTION);

            // 外部アプリケーションに入力するファイルの拡張子形式
            string strInputExt = MyReadConfig.readConfig(strGLConfigExePath, strAppName, EP_NODE_INPUT_EXT);

            // 外部アプリケーションに入力するファイルのフルパス
            string strInputPath = Path.Combine(strGLWorkDir, strInputFileName + strInputExt);

            // 外部アプリケーションが生成するファイルの出力先の拡張子を除いたパス
            string strOutputPath = Path.Combine(strGLWorkDir, strOutputFileName);

            // 外部アプリケーションを起動するためのコマンドライン引数を設定
            string strArguments = MyReadConfig.readConfig(strGLConfigExePath, strAppName, EP_NODE_OPTION);

            // 引数内の変数を置換
            strArguments = strArguments
                .Replace("{Input}", strInputPath)
                .Replace("{Output}", strOutputPath);

            // 設定したコマンドライン引数を渡して外部アプリケーションを起動
            processStart(strExtAppPath, strArguments);
        }
    }
}
