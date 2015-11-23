using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AutoEncoder
{
    class ExternalAppSettings : MyBaseClass
    {
        public string AppRoot { get; private set; }
        public List<string> InputFile { get; private set; }
        public string OutputFile { get; private set; }
        public string AppName { get; private set; }
        public string Option { get; private set; }
        public string ArgumentString { get; private set; }
        public string TotalString { get; private set; }
        public Action AfterAction { get; private set; }

        public ExternalAppSettings(string strAppName, string strOutputFile, Action actAfter, params string[] strInputFile)
        {
            AppName = strAppName;
            AppRoot = XDOCUMENT_CONFIG_EXEPATH.ReadConfig(AppName, CONFIG_EXEPATH_NODE_PATH);
            InputFile = new List<string>(strInputFile);
            OutputFile = strOutputFile;
            Option = XDOCUMENT_CONFIG_EXEPATH.ReadConfig(AppName, CONFIG_EXEPATH_NODE_OPTION);
            ArgumentString = MakeAppArgs(AppName, InputFile, OutputFile);
            TotalString = AppRoot + ArgumentString;
            AfterAction = actAfter;
        }

        /// <summary>
        /// ExePath.configを読み込み、データを置換・結合し、コマンドライン引数を作成する
        /// </summary>
        /// <param name="strAppName">アプリケーション名</param>
        /// <param name="strInputFileName">入力ファイル名（拡張子を除いたファイルパスの配列）</param>
        /// <param name="strOutputFileName">出力ファイル名（パス、拡張子を除いたファイル名のみ）</param>
        /// <returns>各アプリケーションのコマンドライン引数</returns>
        private string MakeAppArgs(string strAppName, List<string> strInputFileName, string strOutputFileName)
        {
            IEnumerable<XElement> enumInputElements = XDOCUMENT_CONFIG_EXEPATH.GetConfigXElement(strAppName, CONFIG_EXEPATH_NODE_INPUT, CONFIG_EXEPATH_NODE_FILE_ARG);
            IEnumerable<XElement> enumOutputElements = XDOCUMENT_CONFIG_EXEPATH.GetConfigXElement(strAppName, CONFIG_EXEPATH_NODE_OUTPUT, CONFIG_EXEPATH_NODE_FILE_ARG);
            IEnumerable<XElement> enumOptionElements = XDOCUMENT_CONFIG_EXEPATH.GetConfigXElement(strAppName, CONFIG_EXEPATH_NODE_OPTION);

            StringBuilder sbArguments = new StringBuilder();

            foreach (var xInputItem in enumInputElements.Select((value, count) => new { value, count }))
            {
                sbArguments.Append(
                    xInputItem.value.Value
                        .Replace("{Input}", strInputFileName[xInputItem.count])
                        .Replace("{Ext}", xInputItem.value.Attribute(CONFIG_EXEPATH_ATTRIBUTE_FILE_EXT).Value)
                    );
            }

            foreach (XElement xOutputItem in enumOutputElements)
            {
                sbArguments.Append(
                    xOutputItem.Value
                        .Replace("{Output}", strOutputFileName)
                        .Replace("{Ext}", xOutputItem.Attribute(CONFIG_EXEPATH_ATTRIBUTE_FILE_EXT).Value)
                    );
            }

            foreach (XElement xOptionItem in enumOptionElements)
            {
                sbArguments.Append(xOptionItem.Value);
            }

            return sbArguments.ToString();
        }
    }
}
