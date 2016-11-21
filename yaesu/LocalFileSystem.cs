using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace yaesu
{
    class LocalFileSystem
    {

        // コンストラクタ
        public LocalFileSystem()
        {
        }

        public ListView setListViewFromFiles(ListView listView, string filePath)
        {
            //"C:\test"以下のファイルをすべて取得する
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(@"C:\Users\Kenichi Takahashi\Desktop\yaesu");
            IEnumerable<System.IO.FileInfo> files =
                di.EnumerateFiles("*", System.IO.SearchOption.AllDirectories);


            //ファイルを列挙する
            foreach (System.IO.FileInfo f in files)
            {
                listView.Items.Add(f.Name);
            }


            return listView;
        }
    }
}
