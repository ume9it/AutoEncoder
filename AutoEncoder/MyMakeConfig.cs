using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace AutoEncoder
{
    class MyMakeConfig : MyBaseClass
    {
        bool isGLRenewDocument = false;

        public void makeAvs ()
        {
            string strAvsName = Path.Combine(strGLWorkDir, strGLFileName + ".avs");
            Dictionary<string, object> dicAvsContents = new Dictionary<string,object>();

            dicAvsContents.Add("LoadPlugin", 2048);
            dicAvsContents.Add("SetMemoryMax", 2048);
            dicAvsContents.Add("DGDecode_MPEG2Source", strGLFileName + ".d2v");
            dicAvsContents.Add("WavSource", 2048);
            dicAvsContents.Add("interlaced", true);

            string strAvsContents = 
                "SetMemoryMax(" + dicAvsContents["SetMemoryMax"] + ")" + "\r\n"
                + "LoadPlugin(" + "\"" + dicAvsContents["LoadPlugin"] + "\"" + ")" + "\r\n"
                + "vSrc = DGDecode_MPEG2Source(" + "\"" + dicAvsContents["DGDecode_MPEG2Source"] + "\")" + "\r\n"
                + "vSrc = KillAudio(vSrc)" + "\r\n"
                + "aSrc = WavSource(" + "\"" + dicAvsContents["WavSource"] + "\")" + "\r\n"
                + "aSrc = KillVideo(aSrc)" + "\r\n"
                + "AudioDubEx(vSrc,aSrc)" + "\r\n"
                + "ConvertToYUY2(interlaced = " + dicAvsContents["interlaced"] + ")";

            StreamWriter swAvs = new StreamWriter(strAvsName, isGLRenewDocument);

            swAvs.WriteLine("SetMemoryMax(" + dicAvsContents["SetMemoryMax"] + ")");
            swAvs.WriteLine("LoadPlugin(" + "\"" + dicAvsContents["LoadPlugin"] + "\"" + ")");
        }
    }
}