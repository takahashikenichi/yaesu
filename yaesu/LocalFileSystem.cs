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

        // コンストラクタ
        public LocalFileSystem(string filePath)
        {
            di = new DirectoryInfo(filePath);
        }

        public ListView setListViewFromFiles(ListView listView)
        {
            //"C:\test"以下のファイルをすべて取得する
//            di = new DirectoryInfo(@"C:\Users\Kenichi Takahashi\Desktop\yaesu");
            files = di.EnumerateFiles("*", System.IO.SearchOption.AllDirectories);


            //ファイルを列挙する
            foreach (FileInfo f in files)
            {
                listView.Items.Add(f.Name);
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
    }
}
