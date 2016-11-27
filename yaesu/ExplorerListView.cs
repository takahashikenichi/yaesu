﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShellNamespace
{
    public partial class ExplorerListView : ListView
    {
        private ShellNamespaceManager shellNamespaceManager;
        private SystemImageList systemImageList_Normal;
        private SystemImageList systemImageList_Small;

        public ExplorerListView()
        {
            //InitializeComponent();
            init();
        }

        public ExplorerListView(IContainer container)
        {
            container.Add(this);

            //InitializeComponent();
            init();
        }

        private void init()
        {
            //Items.Clear();
            shellNamespaceManager = new ShellNamespaceManager();
            CreateDetailsColumn();

        }

        public void UIInit()
        {
            systemImageList_Normal = new SystemImageList(false, SystemImageList.SystemIconSize.ExtraLarge);
            systemImageList_Normal.ListViewSetImageList(this.Handle, SystemImageList.ListViewIconSetMode.Normal);

            systemImageList_Small = new SystemImageList(false, SystemImageList.SystemIconSize.Small);
            systemImageList_Small.ListViewSetImageList(this.Handle, SystemImageList.ListViewIconSetMode.Small);

            if (DesignMode == false)
            {
                LoadDesktopFolder();
            }
        }

        private void LoadDesktopFolder()
        {
            ShellItem m_shDesktop = shellNamespaceManager.GetDesktopShellItem();

            List<ShellItem> itemList = m_shDesktop.GetSubItems(true);

            this.Items.Clear();

            foreach (ShellItem si in itemList)
            {
                ListViewItem lvItem = new ListViewItem();
                lvItem.Text = si.DisplayName;
                lvItem.ImageIndex = si.IconIndex;
                //lvItem.SelectedImageIndex = m_shDesktop.IconIndex;
                lvItem.Tag = si;


                m_shDesktop.GetData(si);

                if ((si.IsStream == true && si.IsFileSystem == true) || (si.IsFolder == true && si.IsFileSystem == true))
                {
                    if (si.LastAccessTime == DateTime.MinValue)
                    {
                        lvItem.SubItems.Add("");
                    }
                    else
                    {
                        lvItem.SubItems.Add(si.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));
                    }
                }
                else
                {
                    lvItem.SubItems.Add("");
                }



                //lvItem.SubItems.Add(si.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));
                lvItem.SubItems.Add(si.TypeName);
                lvItem.SubItems.Add(FileSizeToString(si.FileSize));

                //lvItem.SubItems.Add(si.);
                this.Items.Add(lvItem);
            }
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);

            if (SelectedItems.Count > 0)
            {
                ChangeCurentDirectory((ShellItem)SelectedItems[0].Tag);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Enter)
            {
                if (SelectedItems.Count > 0)
                {
                    ChangeCurentDirectory((ShellItem)SelectedItems[0].Tag);
                }
            }
        }

        public void ChangeCurentDirectory(ShellItem ssi)
        {
            if (ssi.IsFolder == true)
            {
                List<ShellItem> itemList;
                try
                {
                    itemList = ssi.GetSubItems(false);
                }
                catch (System.IO.FileNotFoundException exc)
                {
                    System.Windows.Forms.MessageBox.Show(exc.Message);
                    return;
                }
                catch (Exception exc)
                {
                    return;
                }

                FillItem(itemList, ssi);
            }
        }

        private void FillItem(List<ShellItem> itemList, ShellItem parentShellItem)
        {
            this.Items.Clear();

            foreach (ShellItem si in itemList)
            {
                ListViewItem lvItem = new ListViewItem();
                lvItem.Text = si.DisplayName;
                lvItem.ImageIndex = si.IconIndex;
                //lvItem.SelectedImageIndex = m_shDesktop.IconIndex;
                lvItem.Tag = si;


                //WindowsAPI.WIN32_FIND_DATA findData = parentShellItem.GetData(si.PIDL_Relative);
                //System.Diagnostics.Debug.WriteLine(findData.ftCreationTime);
                //System.Runtime.InteropServices.FILETIME

                //DateTime dt = DateTime.FromFileTime(findData.ftCreationTime);
                parentShellItem.GetData(si);

                if (si.IsStream == true || si.IsFolder == true)
                {
                    if (si.LastAccessTime == DateTime.MinValue)
                    {
                        lvItem.SubItems.Add("");
                    }
                    else
                    {
                        lvItem.SubItems.Add(si.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));
                    }
                }
                else
                {
                    lvItem.SubItems.Add("");
                }

                lvItem.SubItems.Add(si.TypeName);
                lvItem.SubItems.Add(FileSizeToString(si.FileSize));

                this.Items.Add(lvItem);

            }

        }

        protected virtual void CreateDetailsColumn()
        {
            Columns.Add("名前");
            Columns.Add("更新日時");
            Columns.Add("種類");
            Columns.Add("サイズ");

        }

        private string FileSizeToString(long FileSize)
        {
            long TB = 1000L * 1000L * 1000L * 1000L;
            long GB = 1000L * 1000L * 1000L;
            long MB = 1000L * 1000L;
            long KB = 1000L;

            if (FileSize <= 0)
            {
                return "";
            }
            else if (TB <= FileSize)
            {
                return string.Format("{0:#,#} TB", FileSize / TB);
            }
            else if (GB <= FileSize)
            {
                return string.Format("{0:#,#} GB", FileSize / GB);
            }
            else if (MB <= FileSize)
            {
                return string.Format("{0:#,#} MB", FileSize / MB);
            }
            else if (KB <= FileSize)
            {
                return string.Format("{0:#,#} KB", FileSize / KB);
            }
            else
            {
                return string.Format("{0:#,#} Byte", FileSize);
            }
        }

    }
}

