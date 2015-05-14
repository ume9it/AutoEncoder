using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AutoEncoder
{
    class MyBaseClass
    {
        #region 定数

        #region ExePath.config

        // 第一階層
        protected const string EP_APP_DG_INDEX = "DgIndex";
        protected const string EP_APP_TO_WAVE = "ToWave";
        protected const string EP_APP_TS2AAC = "Ts2Aac";
        protected const string EP_APP_CHAPTER_EXE = "ChapterExe";

        // 第二階層
        protected const string EP_NODE_PATH = "Path";
        protected const string EP_NODE_OPTION = "Option";
        protected const string EP_NODE_INPUT = "Input";
        protected const string EP_NODE_OUTPUT = "Output";

        // 第三階層
        protected const string EP_NODE_FILE_ARG = "FileArg";

        // 属性
        protected const string EP_ATTRIBUTE_FILE_EXT = "Ext";

        #endregion

        #region AutoEncode.config

        // 第一階層
        protected const string AE_DIR = "Directory";

        // 第二階層
        protected const string AE_NODE_REC = "Rec";
        protected const string AE_NODE_WORK = "Work";
        protected const string AE_NODE_OUTPUT_MP4 = "OutputMp4";
        protected const string AE_NODE_OUTPUT_TS = "OutputTs";

        #endregion

        #region Library.config

        // 第一階層
        protected const string LIB_DG_DECODE = "DgDecode";
        protected const string LIB_LOGO_DATA = "LogoData";

        // 第二階層
        protected const string LIB_NODE_PATH = "Path";

        #endregion

        #endregion

        #region グローバル変数の定義

        // 成否を管理するフラグ
        protected bool bolGLResultFlag = true;
        protected bool isProcessEnd = false; 

        protected string strGLFileName;
        protected string strGLConfigExePath;
        protected string strGLConfigLibraryPath;
        protected string strGLConfigAutoEncodePath;
        protected string strGLRecDir;
        protected string strGLWorkDir;
        protected List<string> lstGLFileFullPath;
        protected List<string> lstGLTsFiles;

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MyBaseClass()
        {
            #region グローバル変数に値を設定

            // ExePath.configのファイルパス
            strGLConfigExePath = Program.strGLCurrentDirectory + @"\ExePath.config";

            // Library.configのファイルパス
            strGLConfigLibraryPath = Program.strGLCurrentDirectory + @"\Library.config";

            // AutoEncode.configのファイルパス
            strGLConfigAutoEncodePath = Program.strGLCurrentDirectory + @"\AutoEncode.config";

            // 録画ファイルを保存しているディレクトリのパス
            strGLRecDir = Path.Combine(Program.strGLCurrentDirectory
                , MyReadConfig.ReadConfig(strGLConfigAutoEncodePath, AE_DIR, AE_NODE_REC)
                );

            // 作業用一時ファイルを置くディレクトリのパス
            strGLWorkDir = Path.Combine(Program.strGLCurrentDirectory
                , MyReadConfig.ReadConfig(strGLConfigAutoEncodePath, AE_DIR, AE_NODE_WORK)
                );

            // 録画ディレクトリの中の.tsファイルをすべて配列に登録
            lstGLFileFullPath = Directory
                .GetFiles(strGLRecDir)
                .Where(dirFile => dirFile.Split('.').Last() == "ts")
                .Select(tsFile => tsFile)
                .ToList();

            // 登録した.tsファイルの拡張子・パスを除いて名前だけにする
            lstGLTsFiles = lstGLFileFullPath
                .Select(tsFile => tsFile
                    .Split('\\').Last()
                    .Split('.').First()
                )
                .ToList();

            #endregion
        }
    }
}
