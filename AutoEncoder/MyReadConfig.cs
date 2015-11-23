using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Xml.Linq;

namespace AutoEncoder
{
    public static class MyReadConfig
    {
        /// <summary>
        /// XML形式で記述されたファイルを読み込み、データを抽出する
        /// </summary>
        /// <param name="strXmlName">読み込むXMLの名前</param>
        /// <param name="strElementName">1階層目の要素名</param>
        /// <param name="strNodeName">strElementNameに属する2階層目の要素名</param>
        /// <returns>抽出したデータ</returns>
        public static string ReadConfig(this XDocument xdoc, string strElementName, string strNodeName)
        {
            string strXmlPath = String.Empty;

            try
            {
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
                MyErrorHandling.showErrorMessage(ex.Message + "\r\n\r\nXML：" + xdoc + "\r\nElement：" + strElementName + "\r\nNode：" + strNodeName);
            }

            return strXmlPath;
        }

        /// <summary>
        /// xmlの第二階層の要素の列挙体を取得する
        /// </summary>
        /// <param name="strXmlName">XMLのファイルパス</param>
        /// <param name="strFirstElement">第一階層の要素名</param>
        /// <param name="strSecondElement">第二階層の要素名</param>
        /// <returns>第二階層の要素の列挙体</returns>
        public static IEnumerable<XElement> GetConfigXElement(this XDocument xdoc, string strFirstElement, string strSecondElement)
        {
            // 戻り値の列挙体を定義
            IEnumerable<XElement> enumXElement = null;

            // XMLから要素を取得する
            enumXElement = xdoc
                .Root
                .Elements(strFirstElement)
                .Descendants(strSecondElement);

            return enumXElement;
        }

        /// <summary>
        /// xmlの第三階層の要素の列挙体を取得する
        /// </summary>
        /// <param name="strXmlName">XMLのファイルパス</param>
        /// <param name="strFirstElement">第一階層の要素名</param>
        /// <param name="strSecondElement">第二階層の要素名</param>
        /// <param name="strThirdElement">第三階層の要素名</param>
        /// <returns>第三階層の要素の列挙体</returns>
        public static IEnumerable<XElement> GetConfigXElement(this XDocument xdoc, string strFirstElement, string strSecondElement, string strThirdElement)
        {
            // 戻り値の列挙体を定義
            IEnumerable<XElement> enumXElement = null;

            // XMLから要素を取得する
            enumXElement = xdoc
                .Root
                .Elements(strFirstElement)
                .Elements(strSecondElement)
                .Descendants(strThirdElement);

            return enumXElement;
        }
    }
}
