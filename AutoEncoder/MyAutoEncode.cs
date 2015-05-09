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
        MyExtApplication MyExtApplication;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MyAutoEncode(Form1 f)
        {
            MyExtApplication = new MyExtApplication(f);
        }

        /// <summary>
        /// 録画ディレクトリ内のすべてのTSファイルを順次アプリケーションに引き渡していく
        /// </summary>
        /// <returns>実行成否</returns>
        public void encodeMovie()
        {
            // DGIndexでTSをD2Vに

            tsToD2V();

            lstGLTsFiles.ForEach(tsFile => 
            {
                strGLFileName = tsFile;

                // 録画ディレクトリから作業ディレクトリへ名前を変更し、TSファイルを移動する
                //（ファイル名に記号などが含まれている場合、外部アプリケーションがエラーを起こす可能性がある）
                File.Move(strGLRecDir + tsFile + ".ts", strGLWorkDir + strGLTempName + ".ts");

                // DGIndexでTSをD2Vに
                tsToD2V();
            });
        }

        /// <summary>
        /// DGIndex.exeを使用し、TSをd2vファイルとaacファイルに分離
        /// （録画ファイルの放送局がTOKYOMXの場合、ここで得たaacは壊れているので、ts2aacから取得する）
        /// </summary>
        /// <param name="strFileName">入力ファイル名(拡張子・パスを除いたファイル名のみ)</param>
        /// <returns>実行成否</returns>
        public void tsToD2V()
        {
            // 設定したコマンドライン引数を渡してDGIndex.exeを起動

            tokenSource = new System.Threading.CancellationTokenSource();
            token = tokenSource.Token;

            Task.Factory.StartNew(() =>
            {

                //MyExtApplication.runExternalApp(EP_APP_DG_INDEX, strGLTempName, strGLTempName);
                MyExtApplication.processStart(System.Environment.CurrentDirectory + "\\Library\\" + EP_APP_DG_INDEX + ".exe");

                IEnumerable<string> strCollapseAAC = Directory.GetFiles(strGLWorkDir, "*.aac");

                // aacファイルを削除する（取得したaacは壊れているケースが多いため不使用）
                if (strCollapseAAC.Any())
                {
                    File.Delete(strCollapseAAC.First());
                }

            }, token).ContinueWith(task =>
            {
                //while (token.IsCancellationRequested == false)
                //{
                //    System.Threading.Thread.Sleep(100);
                //    Console.WriteLine(DateTime.Now.ToString("yyyyMMdd:mmhhss"));
                //}
                MyErrorHandling.showInfoMessage("おわり");
            });

            // TODO
            // コマンドライン引数が間違っていて、DGIndexが起動はしたが処理を開始しない場合にどうエラー判定するか
            // コマンドライン引数のValidation
        }

        /// <summary>
        /// ts2aac.exeを使用し、TSからaacファイルを抽出する
        /// </summary>
        /// <param name="strFileName">入力ファイル名(拡張子・パスを除いたファイル名のみ)</param>
        /// <returns>実行成否</returns>
        public void tsToAAC()
        {
            // 設定したコマンドライン引数を渡してts2aac.exeを起動
            MyExtApplication.runExternalApp(EP_APP_TS2AAC, strGLTempName, strGLTempName);

            //form1.ProcessDialogTextBox.Text = strOutput;
        }
    }
}
