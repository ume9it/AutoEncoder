using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;

namespace AutoEncoder
{
    class MyAutoEncode : MyBaseClass
    {
        #region フィールド
        MyExtApplication myExtApplication = null;
        public static MyAutoEncode myAutoEncode = null;

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
        public void encodeMovie()
        {
            // DGIndexでTSをD2Vに
            
            // デバッグ用
            //tsToD2V();
            //tsToAAC();

            foreach (string tsFilePath in lstGLTsFiles)
            {
                // 録画ディレクトリから作業ディレクトリへ名前を変更し、TSファイルを移動する
                //（ファイル名に記号などが含まれている場合、外部アプリケーションがエラーを起こす可能性がある）
                File.Move(strGLRecDir + tsFilePath + ".ts", strGLWorkDir + strGLTempName + ".ts");

                // DGIndexでTSをD2Vに

                Task taskTsToD2V = tsToD2V();
                Task taskTsToAAC = tsToAAC(taskTsToD2V);
            }
        }
        #endregion

        #region Private
        /// <summary>
        /// DGIndex.exeを使用し、TSをd2vファイルとaacファイルに分離
        /// （録画ファイルの放送局がTOKYOMXの場合、ここで得たaacは壊れているので、ts2aacから取得する）
        /// </summary>
        /// <param name="strFileName">入力ファイル名(拡張子・パスを除いたファイル名のみ)</param>
        /// <returns>実行成否</returns>
        private Task tsToD2V()
        {
            // 設定したコマンドライン引数を渡してDGIndex.exeを起動

            Task taskDgIndex = Task.Factory.StartNew(() =>
            {
                myExtApplication.runExternalApp(EP_APP_DG_INDEX, strGLTempName, strGLTempName);
                //myExtApplication.processStart(System.Environment.CurrentDirectory + "\\Library\\" + EP_APP_DG_INDEX + ".exe");

            }).ContinueWith(task =>
            {
                MyErrorHandling.showInfoMessage(EP_APP_DG_INDEX + "の処理が終了しました");
                deleteAAC();
            });

            // TODO
            // コマンドライン引数が間違っていて、DGIndexが起動はしたが処理を開始しない場合にどうエラー判定するか
            // コマンドライン引数のValidation

            return taskDgIndex;
        }

        /// <summary>
        /// ts2aac.exeを使用し、TSからaacファイルを抽出する
        /// </summary>
        /// <param name="strFileName">入力ファイル名(拡張子・パスを除いたファイル名のみ)</param>
        /// <returns>実行成否</returns>
        private Task tsToAAC(Task taskBefore)
        {
            Task taskTsToAAC = taskBefore.ContinueWith(task =>
                {
                    // 設定したコマンドライン引数を渡してts2aac.exeを起動
                    myExtApplication.runExternalApp(EP_APP_TS2AAC, strGLTempName, strGLTempName);
                    //myExtApplication.processStart(System.Environment.CurrentDirectory + "\\Library\\" + EP_APP_TS2AAC + ".exe");

                }).ContinueWith(taskAfter =>
                    {
                        MyErrorHandling.showInfoMessage(EP_APP_TS2AAC + "の処理が終了しました");
                    });

            return taskTsToAAC;
        }

        /// <summary>
        /// DGIndexが生成したAACを削除する
        /// </summary>
        private void deleteAAC()
        {
            IEnumerable<string> strCollapseAAC = Directory.GetFiles(strGLWorkDir, "*.aac");

            // aacファイルを削除する（取得したaacは壊れているケースが多いため不使用）
            if (strCollapseAAC.Any())
            {
                File.Delete(strCollapseAAC.First());
            }
        }
        #endregion
    }
}
