using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace yaesu
{

    public partial class BaseForm : Form
    {
        public static readonly String SEARCH_WATERMARK = "マークダウンノートを検索";
        private LocalFileSystem localFileSystem;
        private FileInfo fileInfo; // ローカルファイルシステム

        public BaseForm()
        {
            InitializeComponent();

            // SearchTextBoxのWatermark
            searchTextBox.ForeColor = SystemColors.GrayText;
            searchTextBox.Text = SEARCH_WATERMARK;
            this.searchTextBox.Leave += new System.EventHandler(this.searchTextBox_Leave);
            this.searchTextBox.Enter += new System.EventHandler(this.searchTextBox_Enter);
        }

        private void BaseForm_Load(object sender, EventArgs e)
        {
            // 

            // ListViewコントロールの設定
            var column = new ColumnHeader();
            column.Text = "ファイル";
            column.Width = -1;
            fileListView.Columns.Add(column);

            /*
            // リスト項目の設定
            string[] row_1 = { "123" };
            string[] row_2 = { "234" };
            string[] row_3 = { "567" };
            string[] row_4 = { "890" };

            fileListView.Items.Add(new ListViewItem(row_1));
            fileListView.Items.Add(new ListViewItem(row_2));
            fileListView.Items.Add(new ListViewItem(row_3));
            fileListView.Items.Add(new ListViewItem(row_4));
            */

            // ListBoxにファイル一覧を入れるためにローカルファイルシステムを作成
            if (Properties.Settings.Default.DefaultFolderPath != "")
            {
                localFileSystem = new LocalFileSystem(Properties.Settings.Default.DefaultFolderPath);

                fileListView = localFileSystem.setListViewFromFiles(fileListView);

                if (Properties.Settings.Default.OpendFilePath != "")
                {
                    // ファイル情報を取得する
                    fileInfo = localFileSystem.getFileInfoFromListView(fileListView, Properties.Settings.Default.OpendFilePath);

                    // ファイル情報からファイルを開き、全文を読み込む
                    StreamReader sr = fileInfo.OpenText();
                    editRichTextBox.Text = sr.ReadToEnd();

                    sr.Close();
                }

            }

            // サイズを読み込みリサイズ
            this.ClientSize = Properties.Settings.Default.MyClientSize;
        }

        private void 開くOToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void markdownTextBox_TextChanged(object sender, EventArgs e)
        {
        }

        private void markdownBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void fileListView_ColumnClick(object columnClick)
        {
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void editRichTextBox_TextChanged(object sender, EventArgs e)
        {
            markdownBrowser.DocumentText = CommonMark.CommonMarkConverter.Convert(editRichTextBox.Text);
        }

        private void searchTextBox_Leave(object sender, EventArgs e)
        {
            if (searchTextBox.Text.Length == 0)
            {
                searchTextBox.Text = SEARCH_WATERMARK;
                searchTextBox.ForeColor = SystemColors.GrayText;
            }
        }

        private void searchTextBox_Enter(object sender, EventArgs e)
        {
            if (searchTextBox.Text == SEARCH_WATERMARK)
            {
                searchTextBox.Text = "";
                searchTextBox.ForeColor = SystemColors.WindowText;
            }
        }

        private void ColumnClick(object o, ColumnClickEventArgs e)
        {
            this.fileListView.ListViewItemSorter = new ListViewItemComparer(e.Column);
        }

        private void 終了XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void fileListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = 0;
            String fileName = "";
            if (fileListView.SelectedItems.Count > 0)
            {
                // ファイル名を取得する
                fileName = fileListView.SelectedItems[0].Text;

                // ファイル情報を取得する
                fileInfo = localFileSystem.getFileInfoFromListView(fileListView, fileName);

                // ファイル情報からファイルを開き、全文を読み込む
                StreamReader sr = fileInfo.OpenText();
                editRichTextBox.Text = sr.ReadToEnd();

                /*
                MessageBox.Show(
                    fileName,
                    // (idx + 1).ToString() + "番目が選択されました。",
                    "選択されたインデックス",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.None
                );
                */
                sr.Close();

                Properties.Settings.Default.OpendFilePath = fileName;
            }

        }

        private void 保存SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(fileInfo != null)
            {
                StreamWriter sw = fileInfo.CreateText();

                // ファイルに書き込む
                sw.Write(editRichTextBox.Text);
                sw.Close();
            }
        }

        private void オプションOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //FolderBrowserDialogクラスのインスタンスを作成
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            //上部に表示する説明テキストを指定する
            fbd.Description = "フォルダを指定してください。";
            //ルートフォルダを指定する
            //デフォルトでDesktop
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            //最初に選択するフォルダを指定する
            //RootFolder以下にあるフォルダである必要がある

            fbd.SelectedPath = Properties.Settings.Default.DefaultFolderPath;
            //ユーザーが新しいフォルダを作成できるようにする
            //デフォルトでTrue
            fbd.ShowNewFolderButton = true;

            //ダイアログを表示する
            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                //選択されたフォルダを表示する
                Console.WriteLine(fbd.SelectedPath);
                Properties.Settings.Default.DefaultFolderPath = fbd.SelectedPath;

                // ListBoxにファイル一覧を入れるためにローカルファイルシステムを作成
                localFileSystem = new LocalFileSystem(fbd.SelectedPath);
                fileListView.Clear();
                fileListView = localFileSystem.setListViewFromFiles(fileListView);

            }
        }

        private void BaseForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.MyClientSize = this.ClientSize;
            Properties.Settings.Default.Save();
        }
    }

    class ListViewItemComparer : IComparer
    {
        private int col;
        public ListViewItemComparer()
        {
            col = 0;
        }
        public ListViewItemComparer(int column)
        {
            col = column;
        }
        public int Compare(object x, object y)
        {
            return String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
        }
    }

}
