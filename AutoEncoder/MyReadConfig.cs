using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Xml.Linq;

namespace AutoEncoder
{
    class MyReadConfig
    {
        /// <summary>
        /// XML形式で記述されたファイルを読み込み、データを抽出する
        /// </summary>
        /// <param name="strXmlName">読み込むXMLの名前</param>
        /// <param name="strElementName">1階層目の要素名</param>
        /// <param name="strNodeName">strElementNameに属する2階層目の要素名</param>
        /// <returns>抽出したデータ</returns>
        public static string readConfig(string strXmlName, string strElementName, string strNodeName)
        {
            string strXmlPath = String.Empty;

            try
            {
                // XML形式の文書を読み込む
                XDocument xdoc = XDocument.Load(strXmlName);

                // 読み込んだXML形式の文書から目的の情報を抽出する
                strXmlPath = xdoc.Root.Elements()
                    .Where(elements => elements.Name == strElementName)
                    .Select(nodes => nodes.Descendants(strNodeName)
                        .Select(node => node.Value)
                        .First())
                    .First();
            }
            catch (Exception ex)
            {
                MyErrorHandling.showErrorMessage(ex.Message + "\r\n\r\nXML：" + strXmlName + "\r\nElement：" + strElementName + "\r\nNode：" + strNodeName);
            }

            return strXmlPath;
        }
    }
}
