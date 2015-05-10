using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoEncoder
{
    static class Program
    {
        public static string strGLCurrentDirectory;

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Any())
            {
                // wine対応（wineではカレントディレクトリが正しく認識されないため、渡された引数をプログラムのカレントディレクトリとみなす）
                strGLCurrentDirectory = args.First();
            }
            else
            {
                strGLCurrentDirectory = System.Environment.CurrentDirectory;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
