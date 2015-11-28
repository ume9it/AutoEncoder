using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;
using NUnit.Framework;
using System.Threading;
using static AutoEncoder.MyBaseClass;

namespace AutoEncoder
{
    class MyAutoEncode
    {
        #region フィールド

        private MainForm mainForm = MainForm.form;
        private SearchDirFiles tsFiles = new SearchDirFiles(PATH_DIR_RECORD, ".ts");
        private string strInputFilePath = null;
        private string strInputFilePathWithoutExtension = null;
        private string strOriginalFilePath = null;
        private string strOriginalFilePathWithoutExtension = null;

        Dictionary<string, Dictionary<string, ExternalAppSettings>> dicAllEncodeInfo = new Dictionary<string, Dictionary<string, ExternalAppSettings>>();

        #endregion 

        #region Public
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MyAutoEncode()
        {
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

            string str = null;
            str.Replace("", "fff");

            foreach (string file in tsFiles.FileNameWithoutExtension)
            {
                // 録画ディレクトリ内のTSファイルをそれぞれ処理する

                // グローバル変数に現在処理している.tsファイルを登録

                strInputFilePath = Path.Combine(PATH_DIR_WORK, "temp" + ".ts");
                strInputFilePathWithoutExtension = Path.Combine(PATH_DIR_WORK, "temp");

                strOriginalFilePath = Path.Combine(PATH_DIR_WORK, file + ".ts");
                strOriginalFilePathWithoutExtension = Path.Combine(PATH_DIR_WORK, file);

                Dictionary<string, ExternalAppSettings> dicSettings = new Dictionary<string, ExternalAppSettings>()
                {
                    {
                        // DGIndex.exeでTSをD2Vに
                        "DgIndex"
                        , new ExternalAppSettings(
                            "DgIndex"
                            , strInputFilePathWithoutExtension
                            , DeleteAAC
                            , strInputFilePathWithoutExtension)
                    },
                    {
                        // ts2aac.exeでtsファイルから壊れていないaacファイルを取得する
                        "Ts2Aac"
                        , new ExternalAppSettings(
                            "Ts2Aac"
                            , strInputFilePathWithoutExtension
                            , RenameAAC
                            , strInputFilePathWithoutExtension)
                    },
                    {
                        // ToWaveでts2aac.exeから再取得したaacファイルをwavファイルへ変換
                        "ToWave"
                        , new ExternalAppSettings(
                            "ToWave"
                            , strInputFilePathWithoutExtension
                            // avsファイルを作成する
                            , MakeAvs
                            , strInputFilePathWithoutExtension)
                    },
                    {
                        // chapter_exeでavsファイルを読み込み、無音空間のフレームを検出したテキストファイルを出力する
                        "ChapterExe"
                        , new ExternalAppSettings(
                            "ChapterExe"
                            , strInputFilePathWithoutExtension
                            , null
                            , strInputFilePathWithoutExtension)
                    },
                    {
                        // logoframe.exeでavsファイルを読み込み、ロゴが出現している区間のフレームを取得
                        "LogoFrame"
                        , new ExternalAppSettings(
                            "LogoFrame"
                            , strInputFilePathWithoutExtension
                            , null
                            , strInputFilePathWithoutExtension
                            , Path.Combine(
                                    Program.CurrentDirectory
                                    , XDOCUMENT_CONFIG_LIBRARY.ReadConfig("LogoData", CONFIG_LIBRARY_NODE_PATH)
                                    , GetLogoData(file)))
                    },
                    {
                        // join_logo_scp.exeで、chapter_exeとlogoframeが出力したテキストを読み込み、ロゴが出現/消滅した付近のフレームを取得
                        "JoinLogo"
                        , new ExternalAppSettings(
                            "JoinLogo"
                            , strInputFilePathWithoutExtension
                            , AddTrimDataToAVS
                            , strInputFilePathWithoutExtension
                            , strInputFilePathWithoutExtension
                            , Path.Combine(
                                Program.CurrentDirectory
                                , XDOCUMENT_CONFIG_LIBRARY.ReadConfig("JoinLogoConfig", CONFIG_LIBRARY_NODE_PATH)))
                    },
                    {
                        // AviUtlを起動し、2秒待つ（bat内のWaitがうまく作動しないため、起動処理のみ分離）
                        "AviUtl"
                        , new ExternalAppSettings(
                            "AviUtl"
                            , String.Empty
                            , WaitProcess(2)
                            , String.Empty
                        )
                    },
                    {
                        // AviutlでCMカット情報が記載されたavsをもとに動画をmp4へエンコードする
                        "Auc"
                        , new ExternalAppSettings(
                            "Auc"
                            , strInputFilePathWithoutExtension
                            // 処理の一番最後なので、成果物の移動や一時ファイルの移動などを行う
                            , AfterEncode(file)
                            , String.Empty
                            , strInputFilePathWithoutExtension
                        )
                    }
                };

                dicAllEncodeInfo.Add(file, dicSettings);
            }

            #region 外部アプリケーションの処理
            Task taskBefore = Task.Factory.StartNew(new Action(() => { }));

            foreach (KeyValuePair<string, Dictionary<string, ExternalAppSettings>> dicAllSettings in dicAllEncodeInfo)
            {
                taskBefore = taskBefore.ContinueWith(new Action<Task>((task) =>
                {
                    // 録画ディレクトリのTSファイルを作業ディレクトリへ移動する
                    File.Move(PATH_DIR_RECORD + dicAllSettings.Key + ".ts", strInputFilePath);
                }));
                foreach (KeyValuePair<string, ExternalAppSettings> settings in dicAllSettings.Value)
                {
                    taskBefore = TaskExtAppExecute(settings.Value, taskBefore);
                }
            }
            #endregion
        }
        #endregion

        #region Private
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
        private Task TaskExtAppExecute(ExternalAppSettings setting, Task taskBefore)
        {
            if (taskBefore == null)
            {
                // 前処理が設定されていない場合、空のタスクを作成する
                taskBefore = Task.Factory.StartNew(new Action(() => { }));
            }
            
            // 非同期処理１（前に起動された非同期処理を待った後に行う）
            Action<Task> actFirst = new Action<Task>((task) =>
            {
                // テキストボックスに処理のログを表示
                mainForm.InvokeUpdateLabel(
                    (Action<string, Label>)mainForm.UpdateLabel
                    , setting.AppName + "の処理を実行中です。"
                    , mainForm.ProcessStatusLabel
                     );

                // 設定したコマンドライン引数を渡して外部アプリケーションを起動
                Process process = MyExtApplication.ProcessStart(setting);

                // プロセスが終了するまで待機する処理＆待機中に行う処理
                process = ProcessWait(process);

                // プロセスが終了した後に行う処理
                ProcessEnd();
            });

            // 非同期処理２（非同期処理１の終了を待ってから実行する）
            Action<Task> actNext = new Action<Task>((task) =>
            {
                mainForm.InvokeUpdateLabel(
                    (Action<string, Label>)mainForm.UpdateLabel
                    , setting.AppName + "の処理が終了しました。"
                    , mainForm.ProcessStatusLabel
                     );

                if (setting.AfterAction != null)
                {
                    // デリゲートが入力されている場合はそのメソッドを実行する
                    setting.AfterAction();
                }
            });

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
        /// プロセスが処理を終了するまで永続的に行う処理
        /// </summary>
        /// <param name="processWaitFor">処理を待つプロセスのオブジェクト</param>
        /// <returns>プロセスのオブジェクト</returns>
        private Process ProcessWait(Process processWaitFor)
        {
            while (!processWaitFor.HasExited)
            {
                // 1秒待つ
                System.Threading.Thread.Sleep(1000);

                // メインフォームのラベルに実行中メッセージと、プロセスの合計起動時間（待ち時間）を表示
                mainForm.InvokeUpdateLabel(
                    (Action<string, Label>)mainForm.UpdateLabel
                    , "合計起動時間：" + processWaitFor.TotalProcessorTime.ToString(@"hh\:mm\:ss")
                    , mainForm.TotalProcessorTimeLabel
                    );
            }

            return processWaitFor;
        }

        /// <summary>
        /// プロセスが処理を終了した後の処理
        /// </summary>
        private void ProcessEnd()
        {
            // プログレスバーを１進める
            mainForm.InvokeAddProgress(
                (Action<int, ProgressBar>)mainForm.AddProgressBar
                , 1
                , mainForm.ProcessProgressBar
                );
        }

        /// <summary>
        /// DGIndex.exeが生成したAACを削除する
        /// </summary>
        private void DeleteAAC()
        {
            IEnumerable<string> strCollapseAAC = Directory.GetFiles(PATH_DIR_WORK, "*.aac");

            // aacファイルを削除する（取得したaacは壊れているケースが多いため不使用）
            if (strCollapseAAC.Any())
            {
                File.Delete(strCollapseAAC.First());
            }
        }

        /// <summary>
        /// ts2aac.exeが生成したaacファイルの名前を変更
        /// </summary>
        private void RenameAAC()
        {
            // 正規表現
            Regex regex = new Regex(@" PID [0-9]x[0-9]{3} DELAY .*[0-9]+ms\.aac");

            // 正規表現にマッチするaacファイルを検索
            IEnumerable<string> strRenameAac = Directory
                .GetFiles(PATH_DIR_WORK)
                .Where(dirFile => dirFile.Split('.').Last() == "aac")
                .Where(aacFile => regex.IsMatch(aacFile))
                .Select(item => item);

            if (strRenameAac.Any())
            {
                string strOldAAC = strRenameAac.First();

                string strNewAAC = strInputFilePathWithoutExtension + ".aac";

                // ファイル名の余計な文字列を削除
                File.Move(strOldAAC, strNewAAC);
            }
        }

        /// <summary>
        /// 放送局の判定
        /// </summary>
        /// <param name="strFileName"></param>
        /// <returns></returns>
        private string GetLogoData(string strFileName)
        {
            foreach(string key in DICTIONARY_PATH_FILE_LOGODATA.Keys)
            {
                if (new Regex("^" + key).IsMatch(strFileName))
                {
                    return key;
                }
            }
            return "";
        }

        /// <summary>
        /// 最初のavsファイルの作成
        /// </summary>
        private void MakeAvs()
        {
            string strAvsName = Path.Combine(strInputFilePathWithoutExtension + ".avs");
            Dictionary<string, object> dicAvsContents = new Dictionary<string, object>();

            // DGDecode.dllのパス
            string strDgDecodePath = Path.Combine(Program.CurrentDirectory, XDOCUMENT_CONFIG_LIBRARY.ReadConfig("DgDecode", "Path"));

            dicAvsContents.Add("LoadPlugin", strDgDecodePath);
            dicAvsContents.Add("SetMemoryMax", 256);
            dicAvsContents.Add("DGDecode_MPEG2Source", strInputFilePathWithoutExtension + ".d2v");
            dicAvsContents.Add("WavSource", strInputFilePathWithoutExtension + ".wav");
            dicAvsContents.Add("interlaced", "true");

            using (StreamWriter swAvs = new StreamWriter(new FileStream(strAvsName, FileMode.Append), Encoding.GetEncoding(932)))
            {
                // AVSファイルは文字コードがCP932でないとアプリケーションが読み込めない。

                swAvs.WriteLine("SetMemoryMax(" + dicAvsContents["SetMemoryMax"] + ")");
                swAvs.WriteLine("LoadPlugin(" + "\"" + dicAvsContents["LoadPlugin"] + "\"" + ")");
                swAvs.WriteLine("vSrc = DGDecode_MPEG2Source(" + "\"" + dicAvsContents["DGDecode_MPEG2Source"] + "\")");
                swAvs.WriteLine("vSrc = KillAudio(vSrc)");
                swAvs.WriteLine("aSrc = WavSource(" + "\"" + dicAvsContents["WavSource"] + "\")");
                swAvs.WriteLine("aSrc = KillVideo(aSrc)");
                swAvs.WriteLine("AudioDubEx(vSrc,aSrc)");
                swAvs.WriteLine("ConvertToYUY2(interlaced = " + dicAvsContents["interlaced"] + ")");
            }
        }

        /// <summary>
        /// CMカット結果をavsファイルに追記する
        /// </summary>
        private void AddTrimDataToAVS()
        {
            string strAVS = (strInputFilePathWithoutExtension + ".avs");
            string strTrimData =  strInputFilePathWithoutExtension + "_Trim.txt";

            using (StreamReader reader = new StreamReader(strTrimData))
            {
                using (StreamWriter writer = new StreamWriter(new FileStream(strAVS, FileMode.Append), Encoding.GetEncoding(932)))
                {
                    writer.Write(reader.ReadToEnd());
                }
            }
        }

        /// <summary>
        /// TSファイルの名前を元に戻す
        /// </summary>
        private void RenameTS()
        {
            File.Move(strInputFilePath, strOriginalFilePath);
        }

        /// <summary>
        /// 指定した秒数待つ
        /// </summary>
        private Action WaitProcess(int intSeconds)
        {
            return new Action(() => Thread.Sleep(intSeconds * 1000));
        }

        /// <summary>
        /// すべての外部アプリケーションの処理が終了した後の処理
        /// </summary>
        private Action AfterEncode(string strOriginalFileName)
        {
            #region 外部アプリケーションの処理終了後の処理

            return new Action(() =>
            {
                // TODO ffmpegでtsのCMカットを行う（無圧縮、動画の切り抜き＋結合をするだけ）

                // 出力した.ts、.mp4ファイルを指定フォルダへ保存＆リネーム（指定フォルダが存在しない場合は作成）
                File.Move(strInputFilePath, Path.Combine(PATH_DIR_OUTPUT_TS, strOriginalFileName + ".ts"));
                File.Move(Path.ChangeExtension(strInputFilePath, ".mp4"), Path.Combine(PATH_DIR_OUTPUT_MP4, strOriginalFileName + ".mp4"));

                // 作成された一時ファイルを移動
                Directory.GetFiles(PATH_DIR_RECORD)
                    .Select(item =>
                    {
                        string strFileName = Path.GetFileName(item);
                        File.Move(Path.Combine(PATH_DIR_RECORD, strFileName), Path.Combine(PATH_DIR_OUTPUT_TEMP, strFileName.Replace("temp", strOriginalFileName)));
                        return item;
                    });
            });

            #endregion
        }
        #endregion
    }
}
