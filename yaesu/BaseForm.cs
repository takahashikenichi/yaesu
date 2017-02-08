using mshtml;
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
        public static readonly String SEARCH_WATERMARK = "検索";
        private LocalFileSystem localFileSystem;
        private FileInfo fileInfo; // ローカルファイルシステム
        Point scrollpos;
        Boolean isOpenNewFile = false;
        Boolean isGitHubMarkdown = false;

        public BaseForm()
        {
            InitializeComponent();

            // SearchTextBoxのWatermark
            searchTextBox.ForeColor = SystemColors.GrayText;
            searchTextBox.Text = SEARCH_WATERMARK;
            this.searchTextBox.Leave += new System.EventHandler(this.searchTextBox_Leave);
            this.searchTextBox.Enter += new System.EventHandler(this.searchTextBox_Enter);

            // editRichTextBoxの行間を詰めて、フォントを合わせる
            editRichTextBox.LanguageOption = RichTextBoxLanguageOptions.UIFonts;

        }

        private void BaseForm_Load(object sender, EventArgs e)
        {
            // ListViewコントロールの設定
            var column = new ColumnHeader();
            column.Text = "ファイル";
            column.Width = -1;
            fileListView.Columns.Add(column);

            // ListBoxにファイル一覧を入れるためにローカルファイルシステムを作成
            if (Properties.Settings.Default.DefaultFolderPath != "")
            {
                localFileSystem = new LocalFileSystem(Properties.Settings.Default.DefaultFolderPath);

                fileListView = localFileSystem.setListViewFromFiles(fileListView);

                if (Properties.Settings.Default.OpendFilePath != "")
                {
                    // ファイル情報を取得する
                    fileInfo = localFileSystem.getFileInfoFromListView(fileListView, Properties.Settings.Default.OpendFilePath);
                }
            }

            // サイズを読み込みリサイズ
            this.ClientSize = Properties.Settings.Default.MyClientSize;

            explorerTreeView.UIInit();

            // トラックバーの設定
            mbTrackBar.Minimum = 0;
            mbTrackBar.Maximum = 100;

            mbTrackBar.Value = 50;
            mbTrackBar.TickFrequency = 10;
            mbTrackBar.SmallChange = 10;

        }


        private void setFileToRichTextBox(FileInfo fileInfo)
        {
            // ファイル情報からファイルを開き、全文を読み込む
            isOpenNewFile = true;
            StreamReader sr = fileInfo.OpenText();
            editRichTextBox.Text = sr.ReadToEnd();

            sr.Close();

            Label_Created.Text ="Modified: " + fileInfo.LastWriteTime.ToString();
            updateIndicaterLavel.Visible = false;
            FileNameLabel.Text = fileInfo.Name;

        }

        private void 開くOToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void markdownTextBox_TextChanged(object sender, EventArgs e)
        {
        }

        private void markdownBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            markdownBrowser.Document.Window.ScrollTo(scrollpos);
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
            // 現在のスクロール位置を取得
            if(markdownBrowser.Document != null && !isOpenNewFile)
            {
                IHTMLDocument3 doc3 = (IHTMLDocument3)markdownBrowser.Document.DomDocument;
                IHTMLElement2 elm = (IHTMLElement2)doc3.documentElement;
                scrollpos = new Point(elm.scrollLeft, elm.scrollTop);
            } else
            {
                scrollpos = new Point(0, 0);
            }
            isOpenNewFile = false;

            //指定されたマニフェストリソースを読み込む
            if(isGitHubMarkdown)
            {
                markdownBrowser.DocumentText = Properties.Resources.htmlHeder + CommonMark.CommonMarkConverter.Convert(editRichTextBox.Text) + Properties.Resources.htmlFooter;
            }
            else
            {
                markdownBrowser.DocumentText = Properties.Resources.htmlHeder_nonGit + CommonMark.CommonMarkConverter.Convert(editRichTextBox.Text) + Properties.Resources.htmlFooter;
            }

            updateIndicaterLavel.Visible = true;

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
                if (updateIndicaterLavel.Visible == true)
                {
                    // ファイルに変更がある
                    // メッセージボックスを表示する
                    DialogResult result = MessageBox.Show("ファイルを上書きしますか？",
                        "質問",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button2);

                    //何が選択されたか調べる
                    if (result == DialogResult.Yes)
                    {
                        //「はい」が選択された時
                        StreamWriter sw = fileInfo.CreateText();

                        // ファイルに書き込む
                        sw.Write(editRichTextBox.Text);
                        sw.Close();

                        updateIndicaterLavel.Visible = false;

                        // ファイル名を取得する
                        fileName = fileListView.SelectedItems[0].Text;

                        // ファイル情報を取得する
                        fileInfo = localFileSystem.getFileInfoFromListView(fileListView, fileName);

                        setFileToRichTextBox(fileInfo);
                    }

                    else if (result == DialogResult.No)
                    {
                        //「いいえ」が選択された時
                        if (fileListView.SelectedItems.Count > 0)
                        {
                            // ファイル名を取得する
                            fileName = fileListView.SelectedItems[0].Text;

                           // ファイル情報を取得する
                            fileInfo = localFileSystem.getFileInfoFromListView(fileListView, fileName);

                            setFileToRichTextBox(fileInfo);
                        }
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        //「キャンセル」が選択された時 なにもしない
                    }
                }
                else
                {
                    // ファイルに変更がない場合
                    // ファイル名を取得する
                    fileName = fileListView.SelectedItems[0].Text;

                    // ファイル情報を取得する
                    fileInfo = localFileSystem.getFileInfoFromListView(fileListView, fileName);

                    setFileToRichTextBox(fileInfo);
                }
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

                updateIndicaterLavel.Visible = false;
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

            String fileName = "";
            if (updateIndicaterLavel.Visible == true)
            {
                // ファイルに変更がある
                // メッセージボックスを表示する
                DialogResult result = MessageBox.Show("ファイルを上書きしますか？",
                    "質問",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button2);

                //何が選択されたか調べる
                if (result == DialogResult.Yes)
                {
                    //「はい」が選択された時
                    StreamWriter sw = fileInfo.CreateText();

                    // ファイルに書き込む
                    sw.Write(editRichTextBox.Text);
                    sw.Close();

                    updateIndicaterLavel.Visible = false;

                    // ファイル名を取得する
                    fileName = fileListView.SelectedItems[0].Text;

                    // ファイル情報を取得する
                    fileInfo = localFileSystem.getFileInfoFromListView(fileListView, fileName);

                    setFileToRichTextBox(fileInfo);
                }

                else if (result == DialogResult.No)
                {
                    //「いいえ」が選択された時
                    if (fileListView.SelectedItems.Count > 0)
                    {
                        // ファイル名を取得する
                        fileName = fileListView.SelectedItems[0].Text;

                        // ファイル情報を取得する
                        fileInfo = localFileSystem.getFileInfoFromListView(fileListView, fileName);

                        setFileToRichTextBox(fileInfo);
                    }
                }
                else if (result == DialogResult.Cancel)
                {
                    //「キャンセル」が選択された時 なにもしない
                }
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer1.Panel1Collapsed = false;
            splitContainer1.Panel2Collapsed = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer1.Panel1Collapsed = false;
            splitContainer1.Panel2Collapsed = false;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer1.Panel1Collapsed = true;
            splitContainer1.Panel2Collapsed = false;
        }

        private void explorerTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            MessageBox.Show(explorerTreeView.SelectedNode.FullPath);
            /*
                        // ListBoxにファイル一覧を入れるためにローカルファイルシステムを作成
                        localFileSystem = new LocalFileSystem(explorerTreeView.SelectedNode.FullPath);
                        fileListView.Clear();
                        fileListView = localFileSystem.setListViewFromFiles(fileListView);*/
        }


        private void newNoteButton_Click(object sender, EventArgs e)
        {
            //SaveFileDialogクラスのインスタンスを作成
            SaveFileDialog sfd = new SaveFileDialog();

            //はじめのファイル名を指定する
            //はじめに「ファイル名」で表示される文字列を指定する
            // 現在の日付を取得する
            DateTime dtToday = DateTime.Today;
            // 取得した日付を表示する
            

            sfd.FileName = DateTime.Now.ToString("yyyyMMdd") + ".md";
            //はじめに表示されるフォルダを指定する            
            sfd.InitialDirectory = localFileSystem.getFolderPath();
            //[ファイルの種類]に表示される選択肢を指定する
            //指定しない（空の文字列）の時は、現在のディレクトリが表示される
            sfd.Filter = "MDファイル(*.md)|*.md|すべてのファイル(*.*)|*.*";
            //[ファイルの種類]ではじめに選択されるものを指定する
            //2番目の「すべてのファイル」が選択されているようにする
            sfd.FilterIndex = 1;
            //タイトルを設定する
            sfd.Title = "保存先のファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            sfd.RestoreDirectory = true;
            //既に存在するファイル名を指定したとき警告する
            //デフォルトでTrueなので指定する必要はない
            sfd.OverwritePrompt = true;
            //存在しないパスが指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            sfd.CheckPathExists = true;

            //ダイアログを表示する
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                // OKボタンがクリックされたとき
                using (FileStream fs = File.Create(sfd.FileName))
                {
                    // ファイルストリームを閉じて、変更を確定させる
                    // 呼ばなくても using を抜けた時点で Dispose メソッドが呼び出される
                    fs.Close();
                }
                // ファイルを列挙し直す
                fileListView = localFileSystem.setListViewFromFiles(fileListView);

                // ファイル情報を取得する
                fileInfo = localFileSystem.getFileInfoFromListView(fileListView, System.IO.Path.GetFileName(sfd.FileName));

                // 新規作成したファイルを読み込む
                setFileToRichTextBox(fileInfo);
            }
        }

        private void editRichTextBox_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void mbTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if(markdownBrowser != null && markdownBrowser.Document != null)
            {
                // ズームを取得
                var w = (mshtml.IHTMLWindow2)markdownBrowser.Document.Window.DomWindow;
                var s = (mshtml.IHTMLScreen2)w.screen;
                int zoom = s.deviceXDPI * 100 / 96;

                // ズーム率を設定
                if(mbTrackBar.Value < 50)
                {
                    // 縮小
                    zoom = 100 - (50 - mbTrackBar.Value);
                } else
                {
                    zoom = mbTrackBar.Value * 2;
                }
                ((SHDocVw.WebBrowser)markdownBrowser.ActiveXInstance).ExecWB(SHDocVw.OLECMDID.OLECMDID_OPTICAL_ZOOM, SHDocVw.OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER, zoom, IntPtr.Zero);
            }
        }

        private void nonGitCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            isGitHubMarkdown = ghmdCheckBox.Checked;

            // イケてないが、別のところのコピペ
            // 現在のスクロール位置を取得
            if (markdownBrowser.Document != null && !isOpenNewFile)
            {
                IHTMLDocument3 doc3 = (IHTMLDocument3)markdownBrowser.Document.DomDocument;
                IHTMLElement2 elm = (IHTMLElement2)doc3.documentElement;
                scrollpos = new Point(elm.scrollLeft, elm.scrollTop);
            }
            else
            {
                scrollpos = new Point(0, 0);
            }
            //指定されたマニフェストリソースを読み込む
            if (isGitHubMarkdown)
            {
                markdownBrowser.DocumentText = Properties.Resources.htmlHeder + CommonMark.CommonMarkConverter.Convert(editRichTextBox.Text) + Properties.Resources.htmlFooter;
            }
            else
            {
                markdownBrowser.DocumentText = Properties.Resources.htmlHeder_nonGit + CommonMark.CommonMarkConverter.Convert(editRichTextBox.Text) + Properties.Resources.htmlFooter;
            }

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
