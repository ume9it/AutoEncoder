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
        protected MyExtApplication myExtApplication;
        protected MyMakeConfig myMakeConfig;

        #region 定数

        #region ExePath.config

        // 第一階層
        protected const string EP_APP_DG_INDEX = "DgIndex";
        protected const string EP_APP_TO_WAVE = "ToWave";
        protected const string EP_APP_TS2AAC = "Ts2Aac";
        protected const string EP_APP_CHAPTER_EXE = "ChapterExe";
        protected const string EP_APP_LOGO_FRAME = "LogoFrame";
        protected const string EP_APP_JOIN_LOGO = "JoinLogo";

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
        protected const string LIB_JOIN_LOGO_CONIFG = "JoinLogoConfig";

        // 第二階層
        protected const string LIB_NODE_PATH = "Path";
        protected const string LIB_NODE_FILE = "File";

        // 属性
        protected const string LIB_ATTRIBUTE_FILE_EXT = "Ext";

        #endregion

        #endregion

        #region グローバル変数の定義

        // 成否を管理するフラグ
        protected bool bolGLResultFlag = true;
        protected bool isProcessEnd = false;

        // ExePath.configのファイルパス
        protected static string strGLConfigExePath =
            Path.Combine(Program.CurrentDirectory, "ExePath.config");

        protected static string strGLConfigLibraryPath =
            Path.Combine(Program.CurrentDirectory, "Library.config");

        protected static string strGLConfigAutoEncodePath =
            Path.Combine(Program.CurrentDirectory, "AutoEncode.config");

        // 録画ファイルを保存しているディレクトリのパス
        protected static string strGLRecDir =
            Path.Combine(Program.CurrentDirectory
                , MyReadConfig.ReadConfig(strGLConfigAutoEncodePath, AE_DIR, AE_NODE_REC)
                );

        // 作業用一時ファイルを置くディレクトリのパス
        protected static string strGLWorkDir =
            Path.Combine(Program.CurrentDirectory
                , MyReadConfig.ReadConfig(strGLConfigAutoEncodePath, AE_DIR, AE_NODE_WORK)
                );

        #endregion

        protected SearchDirFiles tsFiles = new SearchDirFiles(strGLRecDir, ".ts");
        protected string strInputFile = null;
        protected string strInputFileWithoutExtension = null;
    }
}
