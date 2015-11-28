using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoEncoder
{
    static class Program
    {
        public static string CurrentDirectory;

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                if (args.Any())
                {
                    // wine対応（wineではカレントディレクトリが正しく認識されないため、渡された引数をプログラムのカレントディレクトリとみなす）
                    CurrentDirectory = args.First();
                }
                else
                {
                    CurrentDirectory = Environment.CurrentDirectory;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            catch(Exception ex)
            {
                // キャッチしきれなかった例外はFatalログとして出力する。
                ex.Fatal();
            }
        }
    }
}
