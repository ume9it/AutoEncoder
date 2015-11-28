using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NUnit.Framework;
using NLog;

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
            ILogger logger = LogManager.GetCurrentClassLogger();
            logger.Debug("test");
            logger.Error("test");
            logger.Warn("test");
            logger.Info("test");
            logger.Fatal("test");
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
    }
}
