using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace yaesu
{
    class LocalFileSystem
    {
        System.IO.DirectoryInfo di;
        IEnumerable<FileInfo> files;
        string _hiddenFileName;

        // コンストラクタ
        public LocalFileSystem(string filePath, string hiddenFileName)
        {
            di = new DirectoryInfo(filePath);
            _hiddenFileName = hiddenFileName;

        }

        public ListView setListViewFromFiles(ListView listView)
        {
            listView.Clear();

            //"C:\test"以下のファイルをすべて取得する
//            di = new DirectoryInfo(@"C:\Users\Kenichi Takahashi\Desktop\yaesu");
            files = di.EnumerateFiles("*", System.IO.SearchOption.AllDirectories);


            //ファイルを列挙する
            foreach (FileInfo f in files)
            {
                if(0 != f.Name.CompareTo(_hiddenFileName))
                {
                    listView.Items.Add(f.Name);
                }
            }


            return listView;
        }

        public FileInfo getFileInfoFromListView (ListView listView, string FileName)
        {
            files = di.EnumerateFiles("*", System.IO.SearchOption.AllDirectories);
            
            foreach (FileInfo f in files)
            {
                if(f.Name == FileName)
                {
                    return f;
                }
            }

            return null;
        }

        public String getFolderPath()
        {
            return di.ToString();
        }
    }
}
