using Colso.DataTransporter.AppCode;
using Colso.Xrm.DataTransporter.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static Colso.Xrm.DataTransporter.AppCode.Enumerations;
using static System.Windows.Forms.ListViewItem;

namespace Colso.DataTransporter.Forms
{

    public partial class ErrorList : Form
    {
        private List<Item<string, string>> errors;
        private Settings settings;

        public ErrorList(List<Item<string, string>> errors, Settings settings)
        {
            this.errors = errors;
            this.settings = settings;
            InitializeComponent();

            toolTipErrorList.SetToolTip(btnCopyAll, "Copy all items in the list to the clipboard (using selected Copy format) and optionally extracting all GUIDs");
            toolTipErrorList.SetToolTip(btnClose, "Close this error dialog window");

        }

        private void BtnCloseClick(object sender, EventArgs e)
        {
            Close();
        }

        private void ErrorListLoad(object sender, EventArgs e)
        {
            foreach (var error in errors)
            {
                var item = new ListViewItem(error.Key);
                item.SubItems.Add(error.Value);

                lvErrors.Items.Add(item);
            }
        }

        private void btnCopyAll_Click(object sender, EventArgs e)
        {

            int _enCopyOptions = settings.CopyFormatType;
            if (settings.IncludeExtractGUIDs) _enCopyOptions |= (int)enCopyOptions.ExtractIDs;

            //
            // Dump to clipboard
            //
            Clipboard.SetText(listviewToOutputFormat(lvErrors, _enCopyOptions));

        }

        private void lvErrors_KeyUp(object sender, KeyEventArgs e)
        {

            handleListViewKeyUp(sender, e, settings);

        }


    }

}