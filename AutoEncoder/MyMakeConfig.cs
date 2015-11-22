using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AutoEncoder
{
    class MyMakeConfig : MyBaseClass
    {
        public MyMakeConfig()
        {
        }

        public void MakeAvs (string strFileName)
        {
            string strAvsName = Path.Combine(strGLWorkDir, strFileName + ".avs");
            Dictionary<string, object> dicAvsContents = new Dictionary<string,object>();

            // DGDecode.dllのパス
            string strDgDecodePath = Path.Combine(Program.CurrentDirectory, MyReadConfig.ReadConfig(strGLConfigLibraryPath, "DgDecode", "Path"));

            dicAvsContents.Add("LoadPlugin", strDgDecodePath);
            dicAvsContents.Add("SetMemoryMax", 256);
            dicAvsContents.Add("DGDecode_MPEG2Source", strFileName + ".d2v");
            dicAvsContents.Add("WavSource", strFileName + ".wav");
            dicAvsContents.Add("interlaced", "true");

            using (StreamWriter swAvs = new StreamWriter(new FileStream(strAvsName, FileMode.Append), Encoding.GetEncoding(932)))
            {
                // AVSファイルは文字コードがCP932でないとアプリケーションが読み込めない。

                swAvs.WriteLine("SetMemoryMax(" + dicAvsContents["SetMemoryMax"] + ")");
                swAvs.WriteLine("LoadPlugin(" + "\"" + dicAvsContents["LoadPlugin"] + "\"" + ")");
                swAvs.WriteLine("vSrc = DGDecode_MPEG2Source(" + "\"" + dicAvsContents["DGDecode_MPEG2Source"] + "\")");
                swAvs.WriteLine("vSrc = KillAudio(vSrc)");
                swAvs.WriteLine("aSrc = WavSource(" + "\"" + dicAvsContents["WavSource"] + "\")");
                swAvs.WriteLine("aSrc = KillVideo(aSrc)");
                swAvs.WriteLine("AudioDubEx(vSrc,aSrc)");
                swAvs.WriteLine("ConvertToYUY2(interlaced = " + dicAvsContents["interlaced"] + ")");
            }

            MyErrorHandling.showInfoMessage("avsの作成が完了しました");
        }
    }
}