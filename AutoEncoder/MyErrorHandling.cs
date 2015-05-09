using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoEncoder
{
    class MyErrorHandling
    {
        /// <summary>
        /// エラーメッセージ出力用メソッド
        /// </summary>
        /// <param name="strMessge">エラーメッセージの文言</param>
        public static void showErrorMessage(string strMessge)
        {
            MessageBox.Show(strMessge, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// エラーメッセージ出力用メソッド（詳細表示）
        /// </summary>
        /// <param name="strMessge">エラーメッセージの文言</param>
        public static void showErrorMessage(string strMessge, Exception ex)
        {
            MessageBox.Show(strMessge + "\r\n" + ex.TargetSite, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 警告メッセージ出力用メソッド（エラーではないが、エラーにつながる事象が起こった時に使う）
        /// </summary>
        /// <param name="strMessge">警告メッセージの文言</param>
        public static void showWarnMessage(string strMessge)
        {
            MessageBox.Show(strMessge, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 通常メッセージ出力用メソッド
        /// </summary>
        /// <param name="strMessage">メッセージ</param>
        public static void showInfoMessage(string strMessage)
        {
            MessageBox.Show(strMessage, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
