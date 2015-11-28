using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace AutoEncoder
{
    /// <summary>
    /// ログ出力
    /// </summary>
    public static class MyLogger
    {
        /// <summary>
        /// Loggerの作成
        /// </summary>
        /// <returns></returns>
        public static ILogger Logger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// Infoログの出力
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">ログに表示するメッセージ、または例外オブジェクト</param>
        public static void Info<T>(this T message)
        {
            Logger().Info<T>(message);
        }

        /// <summary>
        /// Infoログの出力
        /// </summary>
        /// <param name="message">ログに表示するメッセージ</param>
        /// <param name="args">String.Formatに対応する引数</param>
        public static void Info(this string message, params object[] args)
        {
            Logger().Info(string.Format(message, args));
        }

        /// <summary>
        /// Warnログの出力
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">ログに表示するメッセージ、または例外オブジェクト</param>
        public static void Warn<T>(this T message)
        {
            Logger().Warn<T>(message);
        }

        /// <summary>
        /// Warnログの出力
        /// </summary>
        /// <param name="message">ログに表示するメッセージ</param>
        /// <param name="args">String.Formatに対応する引数</param>
        public static void Warn(this string message, params object[] args)
        {
            Logger().Warn(string.Format(message, args));
        }

        /// <summary>
        /// Errorログの出力
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">ログに表示するメッセージ、または例外オブジェクト</param>
        public static void Error<T>(this T message)
        {
            Logger().Error<T>(message);
        }

        /// <summary>
        /// Errorログの出力
        /// </summary>
        /// <param name="message">ログに表示するメッセージ</param>
        /// <param name="args">String.Formatに対応する引数</param>
        public static void Error(this string message, params object[] args)
        {
            Logger().Error(string.Format(message, args));
        }

        /// <summary>
        /// Fatalログの出力
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">ログに表示するメッセージ、または例外オブジェクト</param>
        public static void Fatal<T>(this T message)
        {
            Logger().Fatal<T>(message);
        }

        /// <summary>
        /// Fatalログの出力
        /// </summary>
        /// <param name="message">ログに表示するメッセージ</param>
        /// <param name="args">String.Formatに対応する引数</param>
        public static void Fatal(this string message, params object[] args)
        {
            Logger().Fatal(string.Format(message, args));
        }

        /// <summary>
        /// Debugログの出力
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">ログに表示するメッセージ、または例外オブジェクト</param>
        public static void Debug<T>(this T message)
        {
            Logger().Debug<T>(message);
        }

        /// <summary>
        /// Debugログの出力
        /// </summary>
        /// <param name="message">ログに表示するメッセージ</param>
        /// <param name="args">String.Formatに対応する引数</param>
        public static void Debug(this string message, params object[] args)
        {
            Logger().Debug(string.Format(message, args));
        }
    }
}
