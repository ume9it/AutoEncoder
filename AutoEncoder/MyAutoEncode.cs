using System;
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
        MyExtApplication myExtApplication = null;

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
            myAutoEncode = this;
        }



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
            // DGIndexの処理終了　→　DGIndexが出力したAACを削除
            // (放送局がTOKYO MXのファイルのAACファイルは壊れているので別途ts2aac.exeにて取得する)
            // 一つのファイルに二つの放送波が混在しているため？
            taskAfterExecute dlgAfterTsToD2V = new taskAfterExecute(DeleteAAC);

            taskAfterExecute dlgAfterTsToAacAC = new taskAfterExecute(RenameAAC);

            // デバッグ用
            //tsToD2V();
            //tsToAAC();

            foreach (string tsFilePath in lstGLTsFiles)
            {
                // 録画ディレクトリ内のTSファイルをそれぞれ処理する

                // グローバル変数に現在処理している.tsファイルを登録
                strGLFileName = tsFilePath;

                // 録画ディレクトリのTSファイルを作業ディレクトリへ移動する
                File.Move(strGLRecDir + tsFilePath + ".ts", strGLWorkDir + tsFilePath + ".ts");

                // DGIndex.exeでTSをD2Vに
                Task taskTsToD2V = TaskExtAppExecute(EP_APP_DG_INDEX, tsFilePath, tsFilePath, dlgAfterTsToD2V);

                // ts2aac.exeでtsファイルから壊れていないaacファイルを取得する
                Task taskTsToAAC = TaskExtAppExecute(EP_APP_TS2AAC, tsFilePath, tsFilePath, taskTsToD2V, dlgAfterTsToAacAC);

                // ToWaveでts2aac.exeから再取得したaacファイルをwavファイルへ変換
                Task taskToWave = TaskExtAppExecute(EP_APP_TO_WAVE, tsFilePath, tsFilePath, taskTsToAAC, null);
            }
        }
        #endregion

        #region Private
        /// <summary>
        /// 非同期処理にてタスクを開始する（一番最初の非同期処理）
        /// タスク内容：各外部アプリケーションにファイルと引数を渡し、ファイルを出力させる
        /// 外部アプリケーションの出力したメッセージはイベントハンドラにて捕捉、フォームに出力させる
        /// </summary>
        /// <param name="strAppName">外部アプリケーション名（パスと拡張子を除いたファイルの名前、%~nと同義）</param>
        /// <param name="strInputFileName">入力ファイル名（パスと拡張子を除いたファイルの名前、%~nと同義）</param>
        /// <param name="strOutputFileName">出力ファイル名（パスと拡張子を除いたファイルの名前、%~nと同義）</param>
        /// <param name="dlgTaskAfter">外部アプリケーションの処理終了を待ってから行う処理</param>
        /// <returns>この非同期処理のタスクのオブジェクト</returns>
        private Task TaskExtAppExecute(string strAppName, string strInputFileName, string strOutputFileName, taskAfterExecute dlgTaskAfter)
        {
            // 非同期処理１（処理待ちせずに実行）
            Action actFirst = SetActionFirstExecuteApp(strAppName, strInputFileName, strOutputFileName);

            // 非同期処理２（非同期処理１の終了を待ってから実行する）
            Action<Task> actNext = SetActionAfterExecuteApp(strAppName, dlgTaskAfter);

            // 複数の非同期処理の同期処理
            Task taskDgIndex = 
                Task.Factory
                .StartNew(actFirst)
                .ContinueWith(actNext);

            // TODO
            // コマンドライン引数が間違っていて、アプリケーションが起動はしたが処理を開始しない場合にどうエラー判定するか
            // コマンドライン引数のValidation

            return taskDgIndex;
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
        private Task TaskExtAppExecute(string strAppName, string strInputFileName, string strOutputFileName, Task taskBefore, taskAfterExecute dlgTaskAfter)
        {
            // 非同期処理１（前に起動された非同期処理を待った後に行う）
            Action<Task> actFirst = SetActionNextExecuteApp(strAppName, strInputFileName, strOutputFileName);

            // 非同期処理２（非同期処理１の終了を待ってから実行する）
            Action<Task> actNext = SetActionAfterExecuteApp(strAppName, dlgTaskAfter);

            // 複数の非同期処理の同期処理
            Task taskDgIndex = 
                taskBefore
                .ContinueWith(actFirst)
                .ContinueWith(actNext);

            // TODO
            // コマンドライン引数が間違っていて、アプリケーションが起動はしたが処理を開始しない場合にどうエラー判定するか
            // コマンドライン引数のValidation

            return taskDgIndex;
        }

        /// <summary>
        /// 一番最初の外部アプリケーションの起動
        /// </summary>
        /// <param name="strAppName">外部アプリケーション名（パスと拡張子を除いたファイルの名前、%~nと同義）</param>
        /// <param name="strInputFileName">入力ファイル名（パスと拡張子を除いたファイルの名前、%~nと同義）</param>
        /// <param name="strOutputFileName">出力ファイル名（パスと拡張子を除いたファイルの名前、%~nと同義）</param>
        /// <returns>Actionオブジェクト</returns>
        private Action SetActionFirstExecuteApp(string strAppName, string strInputFileName, string strOutputFileName)
        {
            Action actFirst = (Action)delegate
            {
                // 設定したコマンドライン引数を渡して外部アプリケーションを起動

                myExtApplication.runExternalApp(strAppName, strInputFileName, strOutputFileName);
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
        private Action<Task> SetActionNextExecuteApp(string strAppName, string strInputFileName, string strOutputFileName)
        {
            Action<Task> actFirst = (Action<Task>)delegate
            {
                // 設定したコマンドライン引数を渡して外部アプリケーションを起動

                myExtApplication.runExternalApp(strAppName, strInputFileName, strOutputFileName);
            };

            return actFirst;
        }

        /// <summary>
        /// 外部アプリケーションの処理終了を待ってから行う処理
        /// </summary>
        /// <param name="strAppName">外部アプリケーション名（パスと拡張子を除いたファイルの名前、%~nと同義）</param>
        /// <param name="dlgTaskAfter">外部アプリケーションの処理終了を待ってから行う処理</param>
        /// <returns>Action＜Task＞オブジェクト</returns>
        private Action<Task> SetActionAfterExecuteApp (string strAppName, taskAfterExecute dlgTaskAfter)
        {
            Action<Task> actNext = (Action<Task>)delegate
            {
                MyErrorHandling.showInfoMessage(strAppName + "の処理が終了しました");

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
            Regex regex = new Regex(@" PID 0x110 DELAY " + @".+ms\.aac");

            // 正規表現にマッチするaacファイルを検索
            string strRenameAac = Directory
                .GetFiles(strGLWorkDir)
                .Where(dirFile => dirFile.Split('.').Last() == "aac")
                .Where(aacFile => regex.IsMatch(aacFile))
                .Select(item => item)
                .First();

            string strNewAacName = Path.Combine(strGLWorkDir, strGLFileName);

            // ファイル名の余計な文字列を削除
            File.Move(strRenameAac, strNewAacName);
        }
        #endregion
    }
}
