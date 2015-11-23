using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AutoEncoder
{
    class MyExtApplication : MyBaseClass
    {
        public MyExtApplication()
        {
        }

        /// <summary>
        /// 外部アプリケーションを起動するためのプロセスを作成し、コマンドライン引数を渡した状態で起動する
        /// （アプリケーションのディレクトリを指定して起動）
        /// </summary>
        /// <param name="strAppName">アプリケーション名</param>
        /// <param name="strAppDir">アプリケーションの格納されているディレクトリ</param>
        /// <param name="strAppArg">コマンドライン引数</param>
        /// <returns>外部アプリケーションの標準出力</returns>
        public static Process ProcessStart(ExternalAppSettings setting)
        {
            // プロセスのオブジェクトを作成
            Process process = new Process();

            try
            {
                // 外部アプリケーションをプロセスのオブジェクトに関連付ける
                process.StartInfo.FileName = setting.AppRoot;

                // プロセスに渡す引数を設定
                process.StartInfo.Arguments = setting.ArgumentString;

                // 標準出力を受け取る設定
                process.StartInfo.RedirectStandardOutput = true;

                // 実行時にシェルを使うかの設定（ウインドウを隠すときはfalseにしなければならない）
                process.StartInfo.UseShellExecute = false;

                // ウインドウを隠すかどうかの設定
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                // 外部アプリケーションの画面を非表示（GUIがない場合）
                process.StartInfo.CreateNoWindow = true;

                // 起動失敗時にエラーダイアログを表示
                process.StartInfo.ErrorDialog = true;

                //// 標準出力を取得するイベントハンドラ
                process.OutputDataReceived += MainForm.form.process_OutputDataReceived;
                process.Start();
                process.BeginOutputReadLine();
            }
            catch(Exception ex)
            {
                MyErrorHandling.showErrorMessage(ex.Message, ex);
            }

            return process;
        }
    }
}
