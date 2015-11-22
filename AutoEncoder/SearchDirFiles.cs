using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AutoEncoder
{
    public class SearchDirFiles
    {
        public List<string> FullPath { get; set; }
        public List<string> FileName { get; set; }
        public List<string> FileNameWithoutExtension { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="strDirectoryPath">検索するディレクトリのパス</param>
        /// <param name="strExtension">検索する拡張子</param>
        public SearchDirFiles(string strDirectoryPath, string strExtension)
        {
            FullPath = 
                Directory.GetFiles(strDirectoryPath)
                    .Where(file => Path.GetExtension(file) == strExtension)
                    .ToList();

            FileName = new List<string>();
            FileNameWithoutExtension = new List<string>();

            foreach (string strFile in FullPath)
            {
                FileName.Add(Path.GetFileName(strFile));
                FileNameWithoutExtension.Add(Path.GetFileNameWithoutExtension(strFile));
            }
        }
    }
}
