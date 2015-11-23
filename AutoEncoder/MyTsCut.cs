using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoEncoder
{
    class MyTsCut
    {
        private const double dblGLFrameRate = 29.97;

        public MyTsCut()
        {
            MessageBox.Show(calcFrame(17135));
        }

        /// <summary>
        /// <para>フレーム数から動画時間を算出する</para>
        /// <para>数ミリ秒単位でAviUtlの秒数と誤差が出る場合があるが、フレームに直すと1フレーム程度の誤差なので無視</para>
        /// <para>フレームレートはAviUtl準拠の29.97fpsで計算（正確なテレビ番組のフレームレートは29.968fps?）</para>
        /// </summary>
        /// <param name="dblFrameRate">フレームレート。デフォルトはAviUtl準拠で29.97fpsで計算</param>
        /// <param name="intFrameCount">フレーム数</param>
        /// <returns></returns>
        private string calcFrame(int intFrameCount)
        {
            double dblTotalSecond = intFrameCount / dblGLFrameRate;

            TimeSpan ts = TimeSpan.FromSeconds(dblTotalSecond);

            return ts.ToString(@"hh\:mm\:ss\.ff");
        }
    }
}
