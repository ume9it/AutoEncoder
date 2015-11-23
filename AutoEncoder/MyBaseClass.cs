using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;

namespace AutoEncoder
{
    /// <summary>
    /// 定数の定義を行う。
    /// </summary>
    class MyBaseClass 
    {
        #region Config内要素

        #region ExePath.config

        // 要素
        protected static readonly string CONFIG_EXEPATH_NODE_PATH = "Path";
        protected static readonly string CONFIG_EXEPATH_NODE_OPTION = "Option";
        protected static readonly string CONFIG_EXEPATH_NODE_INPUT = "Input";
        protected static readonly string CONFIG_EXEPATH_NODE_OUTPUT = "Output";
        protected static readonly string CONFIG_EXEPATH_NODE_FILE_ARG = "FileArg";

        // 属性
        protected static readonly string CONFIG_EXEPATH_ATTRIBUTE_FILE_EXT = "Ext";

        #endregion

        #region AutoEncode.config

        // 要素
        protected static readonly string CONFIG_AUTOENCODE_DIR = "Directory";
        protected static readonly string CONFIG_AUTOENCODE_NODE_REC = "Rec";
        protected static readonly string CONFIG_AUTOENCODE_NODE_WORK = "Work";
        protected static readonly string CONFIG_AUTOENCODE_NODE_OUTPUT_MP4 = "OutputMp4";
        protected static readonly string CONFIG_AUTOENCODE_NODE_OUTPUT_TS = "OutputTs";

        #endregion

        #region Library.config

        // 要素
        protected static readonly string CONFIG_LIBRARY_NODE_PATH = "Path";
        protected static readonly string CONFIG_LIBRARY_NODE_FILE = "File";

        // 属性
        protected static readonly string CONFIG_LIBRARY_ATTRIBUTE_FILE_EXT = "Ext";

        #endregion

        #endregion

        #region ファイルパス

        /// <summary>
        /// ExePath.configのファイルパス
        /// </summary>
        protected static readonly string PATH_CONFIG_EXEPATH =
            Path.Combine(Program.CurrentDirectory, "ExePath.config");

        /// <summary>
        /// Library.configのファイルパス
        /// </summary>
        protected static readonly string PATH_CONFIG_LIBRARY =
            Path.Combine(Program.CurrentDirectory, "Library.config");

        /// <summary>
        /// AutoEncode.configのファイルパス
        /// </summary>
        protected static readonly string PATH_CONFIG_AUTOENCODE =
            Path.Combine(Program.CurrentDirectory, "AutoEncode.config");

        /// <summary>
        /// ExePath.configをパースしたXDocument
        /// </summary>
        protected static readonly XDocument XDOCUMENT_CONFIG_EXEPATH = XDocument.Load(PATH_CONFIG_EXEPATH);

        /// <summary>
        /// Library.configをパースしたXDocument
        /// </summary>
        protected static readonly XDocument XDOCUMENT_CONFIG_LIBRARY = XDocument.Load(PATH_CONFIG_LIBRARY);

        /// <summary>
        /// AutoEncode.configをパースしたXDocument
        /// </summary>
        protected static readonly XDocument XDOCUMENT_CONFIG_AUTOENCODE = XDocument.Load(PATH_CONFIG_AUTOENCODE);

        /// <summary>
        /// 録画ファイルを保存しているディレクトリのパス
        /// </summary>
        protected static readonly string PATH_DIR_RECORD =
            Path.Combine(Program.CurrentDirectory
                , XDOCUMENT_CONFIG_AUTOENCODE.ReadConfig(CONFIG_AUTOENCODE_DIR, CONFIG_AUTOENCODE_NODE_REC)
                );

        /// <summary>
        /// 作業用一時ファイルを置くディレクトリのパス
        /// </summary>
        protected static readonly string PATH_DIR_WORK =
            Path.Combine(Program.CurrentDirectory
                , XDOCUMENT_CONFIG_AUTOENCODE.ReadConfig(CONFIG_AUTOENCODE_DIR, CONFIG_AUTOENCODE_NODE_WORK)
                );

        /// <summary>
        /// ロゴデータを置くディレクトリのパス
        /// </summary>
        protected static readonly string PATH_DIR_LOGODATA =
            XDOCUMENT_CONFIG_LIBRARY.ReadConfig("LogoData", "Path");

        /// <summary>
        /// 各ロゴデータの配置パス
        /// </summary>
        protected static readonly Dictionary<string, string> DICTIONARY_PATH_FILE_LOGODATA =
            XDOCUMENT_CONFIG_LIBRARY.GetConfigXElement("LogoData", "File")
            .Select(item =>
            {
                string strLogoWithExtension = Path.ChangeExtension(item.Value, item.FirstAttribute.Value);
                return new KeyValuePair<string, string>(
                    item.Value
                    , Path.Combine(PATH_DIR_LOGODATA, strLogoWithExtension));
            })
            .ToDictionary(key => key.Key, value => value.Value);
        
        #endregion
    }
}
