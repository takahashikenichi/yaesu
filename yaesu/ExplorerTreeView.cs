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
    public partial class ExplorerTreeView : TreeView
    {
        private ShellNamespaceManager shellNamespaceManager;
        private SystemImageList systemImageList;

        private ExplorerListView linkedExplorerListView = null;

        public ExplorerListView LinkedExplorerListView
        {
            set
            {
                linkedExplorerListView = value;
            }
            get
            {
                return linkedExplorerListView;
            }
        }

        public ExplorerTreeView()
        {
            //InitializeComponent();
            Init();
        }

        public ExplorerTreeView(IContainer container)
        {
            container.Add(this);

            //InitializeComponent();
            Init();
        }

        private void Init()
        {
            shellNamespaceManager = new ShellNamespaceManager();
        }

        public void UIInit()
        {
            systemImageList = new SystemImageList(false, SystemImageList.SystemIconSize.Small);
            //this.ImageList = systemImageList;
            //systemImageList.TreeViewSetImageList(this);
            systemImageList.TreeViewSetImageList(this.Handle);

            LoadRootNodes();
        }

        /// <summary>
        /// Loads the root TreeView nodes.
        /// </summary>
        private void LoadRootNodes()
        {
            // Create the root shell item.
            //ShellItem m_shDesktop = new ShellItem();
            ShellItem m_shDesktop = shellNamespaceManager.GetDesktopShellItem();


            // Create the root node.
            TreeNode tvwRoot = new TreeNode();
            tvwRoot.Text = m_shDesktop.DisplayName;
            tvwRoot.ImageIndex = m_shDesktop.IconIndex;
            tvwRoot.SelectedImageIndex = m_shDesktop.IconIndex;
            tvwRoot.Tag = m_shDesktop;


            // Now we need to add any children to the root node.
            List<ShellItem> children = m_shDesktop.GetSubFolders(false);
            foreach (ShellItem shChild in children)
            {
                TreeNode tvwChild = new TreeNode();
                tvwChild.Text = shChild.DisplayName;
                tvwChild.ImageIndex = shChild.IconIndex;
                tvwChild.SelectedImageIndex = shChild.IconIndex;
                tvwChild.Tag = shChild;

                // If this is a folder item and has children then add a place holder node.
                if (shChild.IsFolder && shChild.HasSubFolder)
                    tvwChild.Nodes.Add("PH");
                tvwRoot.Nodes.Add(tvwChild);
            }


            // Add the root node to the tree.
            this.Nodes.Clear();
            this.Nodes.Add(tvwRoot);
            tvwRoot.Expand();

        }

        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            // Remove the placeholder node.
            e.Node.Nodes.Clear();

            // We stored the ShellItem object in the node's Tag property - hah!
            ShellItem shNode = (ShellItem)e.Node.Tag;
            List<ShellItem> sublist = shNode.GetSubFolders(true);
            foreach (ShellItem shChild in sublist)
            {
                if (shChild.IsZipFile == true || shChild.IsStream == true)
                {
                }
                else
                {
                    TreeNode tvwChild = new TreeNode();
                    tvwChild.Text = shChild.DisplayName;
                    tvwChild.ImageIndex = shChild.IconIndex;
                    tvwChild.SelectedImageIndex = shChild.IconIndex;
                    tvwChild.Tag = shChild;

                    // If this is a folder item and has children then add a place holder node.
                    if (shChild.IsFolder && shChild.HasSubFolder) tvwChild.Nodes.Add("PH");
                    e.Node.Nodes.Add(tvwChild);

                }
            }

            base.OnBeforeExpand(e);

        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            if (linkedExplorerListView != null)
            {
                ShellItem shNode = (ShellItem)e.Node.Tag;
                linkedExplorerListView.ChangeCurentDirectory(shNode);
            }
            base.OnAfterSelect(e);
        }

    }
}