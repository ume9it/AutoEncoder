﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace AutoEncoder
{
    class MyAutoEncode : MyBaseClass
    {
        #region フィールド

        public static MyAutoEncode myAutoEncode = null;
        MainForm mainForm;

        // 非同期タスクの処理終了後に実行するメソッドを呼び出すためのデリゲート
        delegate void taskAfterExecute();

        #endregion 

        #region Public
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MyAutoEncode()
        {
            myExtApplication = new MyExtApplication();
            myMakeConfig = new MyMakeConfig();

            myAutoEncode = this;
        }

        /// <summary>
        /// 自身のインスタンスを取得
        /// </summary>
        /// <returns></returns>
        public static MyAutoEncode GetInstance()
        {
            return myAutoEncode;
        }

        /// <summary>
        /// 録画ディレクトリ内のすべてのTSファイルを順次アプリケーションに引き渡していく
        /// </summary>
        /// <returns>実行成否</returns>
        public void EncodeMovie()
        {
            mainForm = MainForm.GetInstance();

            // DGIndexの処理終了　→　DGIndexが出力したAACを削除
            // (放送局がTOKYO MXのファイルのAACファイルは壊れているので別途ts2aac.exeにて取得する)
            // 一つのファイルに二つの放送波が混在しているため？
            taskAfterExecute dlgAfterTsToD2V = new taskAfterExecute(DeleteAAC);

            taskAfterExecute dlgAfterTsToAAC = new taskAfterExecute(RenameAAC);

            // デバッグ用
            //tsToD2V();
            //tsToAAC();

            foreach (string tsFilePath in lstGLTsFiles)
            {
                // 録画ディレクトリ内のTSファイルをそれぞれ処理する

                // グローバル変数に現在処理している.tsファイルを登録
                strGLFileName = tsFilePath;

                // 録画ディレクトリのTSファイルを作業ディレクトリへ移動する
                File.Move(strGLRecDir + tsFilePath + ".ts", strGLWorkDir + strGLFileName + ".ts");

                // DGIndex.exeでTSをD2Vに
                Task taskTsToD2V = TaskExtAppExecute(EP_APP_DG_INDEX, new string[] {Path.Combine(strGLWorkDir, strGLFileName)}, strGLFileName, dlgAfterTsToD2V);

                // ts2aac.exeでtsファイルから壊れていないaacファイルを取得する
                Task taskTsToAAC = TaskExtAppExecute(EP_APP_TS2AAC, new string[] { Path.Combine(strGLWorkDir, strGLFileName) }, strGLFileName, taskTsToD2V, dlgAfterTsToAAC);

                // ToWaveでts2aac.exeから再取得したaacファイルをwavファイルへ変換
                Task taskToWave = TaskExtAppExecute(EP_APP_TO_WAVE, new string[] { Path.Combine(strGLWorkDir, strGLFileName) }, strGLFileName, taskTsToAAC, null);

                // avsファイルを作成する
                Action<Task> action = (Action<Task>)delegate{ myMakeConfig.makeAvs(strGLFileName); };
                Task taskMakeAAC = taskToWave.ContinueWith(action);

                // chapter_exeでavsファイルを読み込み、無音空間のフレームを検出したテキストファイルを出力する
                Task taskChapterExe = TaskExtAppExecute(EP_APP_CHAPTER_EXE, new string[] { Path.Combine(strGLWorkDir, strGLFileName) }, strGLFileName, taskMakeAAC, null);

                // logoframe.exeでavsファイルを読み込み、ロゴが出現している区間のフレームを取得
                Task taskLogoFrame = TaskExtAppExecute(
                    EP_APP_LOGO_FRAME
                    , new string[] { Path.Combine(strGLWorkDir, strGLFileName), Path.Combine(Program.strGLCurrentDirectory, MyReadConfig.ReadConfig(strGLConfigLibraryPath, LIB_LOGO_DATA, LIB_NODE_PATH), "TOKYO_MX") }
                    , strGLFileName
                    , taskChapterExe, null);

                //// join_logo_scp.exeで、chapter_exeとlogoframeが出力したテキストを読み込み、
                //// ロゴが出現/消滅した付近のフレームを取得
                //Task taskJoinLogo = TaskExtAppExecute()

            }   
        }
        #endregion

        #region Private
        /// <summary>
        /// <para>非同期処理にてタスクを開始する（一番最初の非同期処理）</para>
        /// <para>タスク内容：各外部アプリケーションにファイルと引数を渡し、ファイルを出力させる</para>
        /// <para>外部アプリケーションの出力したメッセージはイベントハンドラにて捕捉、フォームに出力させる</para>
        /// </summary>
        /// <param name="strAppName">外部アプリケーション名（パスと拡張子を除いたファイルの名前、%~nと同義）</param>
        /// <param name="strInputFileName">入力ファイル名（パスと拡張子を除いたファイルの名前、%~nと同義）</param>
        /// <param name="strOutputFileName">出力ファイル名（パスと拡張子を除いたファイルの名前、%~nと同義）</param>
        /// <param name="dlgTaskAfter">外部アプリケーションの処理終了を待ってから行う処理</param>
        /// <returns>この非同期処理のタスクのオブジェクト</returns>
        private Task TaskExtAppExecute(string strAppName, string[] strInputFileName, string strOutputFileName, taskAfterExecute dlgTaskAfter)
        {
            // 非同期処理１（処理待ちせずに実行）
            Action actFirst = SetActionFirstExecuteApp(strAppName, strInputFileName, strOutputFileName);

            // 非同期処理２（非同期処理１の終了を待ってから実行する）
            Action<Task> actNext = SetActionAfterExecuteApp(strAppName, dlgTaskAfter);

            // 複数の非同期処理の同期処理
            Task taskAsync = 
                Task.Factory
                .StartNew(actFirst)
                .ContinueWith(actNext);

            // TODO
            // コマンドライン引数が間違っていて、アプリケーションが起動はしたが処理を開始しない場合にどうエラー判定するか
            // コマンドライン引数のValidation

            return taskAsync;
        }

        /// <summary>
        /// 非同期処理にてタスクを開始する(前に実行された非同期タスクが存在する場合)
        /// タスク内容：各外部アプリケーションにファイルと引数を渡し、ファイルを出力させる
        /// 外部アプリケーションの出力したメッセージはイベントハンドラにて捕捉、フォームに出力させる
        /// </summary>
        /// <param name="strAppName">外部アプリケーション名（パスと拡張子を除いたファイルの名前、%~nと同義）</param>
        /// <param name="strInputFileName">入力ファイル名（パスと拡張子を除いたファイルの名前、%~nと同義）</param>
        /// <param name="strOutputFileName">出力ファイル名（パスと拡張子を除いたファイルの名前、%~nと同義）</param>
        /// <param name="taskBefore">この処理の前に行う非同期処理のタスク</param>
        /// <param name="dlgTaskAfter">外部アプリケーションの処理終了を待ってから行う処理</param>
        /// <returns>この非同期処理のタスクのオブジェクト</returns>
        private Task TaskExtAppExecute(string strAppName, string[] strInputFileName, string strOutputFileName, Task taskBefore, taskAfterExecute dlgTaskAfter)
        {
            // 非同期処理１（前に起動された非同期処理を待った後に行う）
            Action<Task> actFirst = SetActionNextExecuteApp(strAppName, strInputFileName, strOutputFileName);

            // 非同期処理２（非同期処理１の終了を待ってから実行する）
            Action<Task> actNext = SetActionAfterExecuteApp(strAppName, dlgTaskAfter);

            // 複数の非同期処理の同期処理
            Task taskAsync =
                taskBefore
                .ContinueWith(actFirst)
                .ContinueWith(actNext);

            // TODO
            // コマンドライン引数が間違っていて、アプリケーションが起動はしたが処理を開始しない場合にどうエラー判定するか
            // コマンドライン引数のValidation

            return taskAsync;
        }

        /// <summary>
        /// 一番最初の外部アプリケーションの起動
        /// </summary>
        /// <param name="strAppName">外部アプリケーション名（パスと拡張子を除いたファイルの名前、%~nと同義）</param>
        /// <param name="strInputFileName">入力ファイル名（パスと拡張子を除いたファイルの名前、%~nと同義）</param>
        /// <param name="strOutputFileName">出力ファイル名（パスと拡張子を除いたファイルの名前、%~nと同義）</param>
        /// <returns>Actionオブジェクト</returns>
        private Action SetActionFirstExecuteApp(string strAppName, string[] strInputFileName, string strOutputFileName)
        {
            Action actFirst = (Action)delegate
            {
                mainForm.InvokeAddText(
                    (Action<string, TextBox>)mainForm.AddText
                    , strAppName + "の処理を開始します。"
                    , mainForm.ProcessDialogTextBox
                    );

                // 設定したコマンドライン引数を渡して外部アプリケーションを起動
                System.Diagnostics.Process process = myExtApplication.RunExternalApp(strAppName, strInputFileName, strOutputFileName);

                while(!process.HasExited)
                {
                    System.Threading.Thread.Sleep(1000);
                    mainForm.InvokeAddText(
                        (Action<string, Label>)mainForm.UpdateLabel
                        , strAppName + "の処理が実行中です。\r\n\r\n合計起動時間：" + process.TotalProcessorTime.ToString(@"hh\:mm\:ss")
                        , mainForm.ProcessStatusLabel
                        );
                }

                mainForm.InvokeAddProgress(
                    (Action<ProgressBar>)mainForm.AddProgressBar
                    , mainForm.ProcessProgressBar
                    );

                mainForm.InvokeAddText(
                    (Action<string, Label>)mainForm.UpdateLabel
                    , strAppName + "の処理が終了しました。\r\n\r\n合計起動時間：" + process.TotalProcessorTime.ToString(@"hh\:mm\:ss")
                    , mainForm.ProcessStatusLabel
                    );

                process.Dispose();
            };

            return actFirst;
        }

        /// <summary>
        /// 2番目以降の外部アプリケーションの起動
        /// （前に起動した外部アプリケーションの終了を待ってから起動処理をするため、戻り値が違う）
        /// </summary>
        /// <param name="strAppName">外部アプリケーション名（パスと拡張子を除いたファイルの名前、%~nと同義）</param>
        /// <param name="strInputFileName">入力ファイル名（パスと拡張子を除いたファイルの名前、%~nと同義）</param>
        /// <param name="strOutputFileName">出力ファイル名（パスと拡張子を除いたファイルの名前、%~nと同義）</param>
        /// <returns>Action＜Task＞オブジェクト</returns>
        private Action<Task> SetActionNextExecuteApp(string strAppName, string[] strInputFileName, string strOutputFileName)
        {
            Action<Task> actFirst = (Action<Task>)delegate
            {
                // テキストボックスに処理のログを表示
                mainForm.InvokeAddText(
                    (Action<string, TextBox>)mainForm.AddText
                    , strAppName + "の処理を開始します。"
                    , mainForm.ProcessDialogTextBox
                    );

                // 設定したコマンドライン引数を渡して外部アプリケーションを起動
                System.Diagnostics.Process process = myExtApplication.RunExternalApp(strAppName, strInputFileName, strOutputFileName);

                while (!process.HasExited)
                {
                    System.Threading.Thread.Sleep(1000);

                    // ラベルにプロセスの待ち時間を表示
                    mainForm.InvokeAddText(
                        (Action<string, Label>)mainForm.UpdateLabel
                        , strAppName + "の処理が実行中です。\r\n\r\n合計起動時間：" + process.TotalProcessorTime.ToString(@"hh\:mm\:ss")
                        , mainForm.ProcessStatusLabel
                        );
                }

                mainForm.InvokeAddProgress(
                    (Action<ProgressBar>)mainForm.AddProgressBar
                    , mainForm.ProcessProgressBar
                    );

                mainForm.InvokeAddText(
                    (Action<string, Label>)mainForm.UpdateLabel
                    , strAppName + "の処理が終了しました。\r\n\r\n合計起動時間：" + process.TotalProcessorTime.ToString(@"hh\:mm\:ss")
                    , mainForm.ProcessStatusLabel
                    );

                process.Dispose();
            };

            return actFirst;
        }

        /// <summary>
        /// <para>外部アプリケーションの処理終了を待ってから行う処理。</para>
        /// <para>第二引数がnullの場合はアプリケーション実行完了メッセージのみを出力</para>
        /// </summary>
        /// <param name="strAppName">外部アプリケーション名（パスと拡張子を除いたファイルの名前、%~nと同義）</param>
        /// <param name="dlgTaskAfter">外部アプリケーションの処理終了を待ってから行う処理</param>
        /// <returns>Action＜Task＞オブジェクト</returns>
        private Action<Task> SetActionAfterExecuteApp (string strAppName, taskAfterExecute dlgTaskAfter)
        {
            Action<Task> actNext = (Action<Task>)delegate
            {
                mainForm.InvokeAddText(
                    (Action<string, TextBox>)mainForm.AddText
                    , strAppName + "の処理が終了しました。"
                    , mainForm.ProcessDialogTextBox
                     );

                if (dlgTaskAfter != null)
                {
                    // デリゲートが入力されている場合はそのメソッドを実行する
                    dlgTaskAfter();
                }
            };

            return actNext;
        }

        /// <summary>
        /// DGIndex.exeが生成したAACを削除する
        /// </summary>
        private void DeleteAAC()
        {
            IEnumerable<string> strCollapseAAC = Directory.GetFiles(strGLWorkDir, "*.aac");

            // aacファイルを削除する（取得したaacは壊れているケースが多いため不使用）
            if (strCollapseAAC.Any())
            {
                File.Delete(strCollapseAAC.First());
            }
        }

        /// <summary>
        /// ts2aac.exeが生成したaacファイルの名前を変更
        /// </summary>
        /// <param name="strGLFileName">もともとのファイル名</param>
        private void RenameAAC()
        {
            // 正規表現
            Regex regex = new Regex(@" PID 0x112 DELAY " + @".+ms\.aac");

            var aaa = Directory.GetFiles(strGLWorkDir);
            var ttt = aaa[0].Split('.');

            // 正規表現にマッチするaacファイルを検索
            IEnumerable<string> strRenameAac = Directory
                .GetFiles(strGLWorkDir)
                .Where(dirFile => dirFile.Split('.').Last() == "aac")
                .Where(aacFile => regex.IsMatch(aacFile))
                .Select(item => item);

            var bbb = strRenameAac.First();

            string strNewAacName = Path.Combine(strGLWorkDir, strGLFileName + ".aac");

            // ファイル名の余計な文字列を削除
            File.Move(bbb, strNewAacName);
        }
        #endregion
    }
}
