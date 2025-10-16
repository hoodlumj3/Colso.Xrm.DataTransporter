using Colso.DataTransporter.AppCode;
using Colso.Xrm.DataTransporter.Models;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using static System.Windows.Forms.ListViewItem;
using static Colso.Xrm.DataTransporter.AppCode.Enumerations;
using System.Activities.Validation;

namespace Colso.DataTransporter.Forms
{
    public partial class Preview : Form
    {
        private List<ListViewItem> items;
        private Settings settings;

        public Preview(List<ListViewItem> items, Settings settings)
        {
            this.items = items;
            this.settings = settings;
            InitializeComponent();

            toolTipPreview.SetToolTip(btnCopyAll, "Copy all items in the list to the clipboard (using selected Copy format) and optionally extracting all GUIDs");
            toolTipPreview.SetToolTip(btnClose, "Close this preview dialog window.");



        }

        private void BtnCloseClick(object sender, EventArgs e)
        {
            Close();
        }

        private void ListLoad(object sender, EventArgs e)
        {
            lvItems.Columns.Clear();
            lvItems.Items.Clear();

            // Add columns
            lvItems.Columns.Add("Action", 80, HorizontalAlignment.Left);
            lvItems.Columns.Add("Id", 225, HorizontalAlignment.Left);
            lvItems.Columns.Add("Name", -2, HorizontalAlignment.Left);

            // Add items
            foreach (var item in items)
                lvItems.Items.Add(item);
        }

        private void SetListViewSorting(ListView listview, int column)
        {
            if (listview.Sorting == SortOrder.Ascending)
                listview.Sorting = SortOrder.Descending;
            else
                listview.Sorting = SortOrder.Ascending;

            listview.ListViewItemSorter = new ListViewItemComparer(column, listview.Sorting);
        }

        private void lvItems_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            SetListViewSorting(lvItems, e.Column);
        }

        private void btnCopyAll_Click(object sender, EventArgs e)
        {

            int _enCopyOptions = settings.CopyFormatType;
            if (settings.IncludeExtractGUIDs) _enCopyOptions |= (int)enCopyOptions.ExtractIDs;

            //
            // Dump to clipboard
            //
            Clipboard.SetText(listviewToOutputFormat(lvItems, _enCopyOptions));

        }

        private void lvItems_KeyUp(object sender, KeyEventArgs e)
        {

            handleListViewKeyUp(sender, e, settings);

        }

    }

}