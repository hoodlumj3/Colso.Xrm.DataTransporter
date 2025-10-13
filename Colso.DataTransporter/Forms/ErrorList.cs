using Colso.DataTransporter.AppCode;
using Colso.Xrm.DataTransporter.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static Colso.Xrm.DataTransporter.AppCode.Enumerations;

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

            toolTipErrorList.SetToolTip(btnCopy, "Copy all items in the list to the clipboard (as CSV), optionally seperating all GUIDs out to seperate columns");
            toolTipErrorList.SetToolTip(btnClose, "Close this error dialog window");
            toolTipErrorList.SetToolTip(ckbExtractIDs, "Include extra columns in copy for all discovered GUIDs");

            ckbExtractIDs.Checked = settings.errorExtractIDs;

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

        private void btnCopy_Click(object sender, EventArgs e)
        {

            //
            // Dump to clipboard
            //

            int _enCopyOptions = 0;
            
            _enCopyOptions |= (int)enCopyOptions.CSV;
            if (settings.errorExtractIDs) _enCopyOptions |= (int)enCopyOptions.ExtractIDs;

            Clipboard.SetText(listviewToOutputFormat(lvErrors, _enCopyOptions));

        }

        private void ckbExtractIDs_CheckStateChanged(object sender, EventArgs e)
        {
            settings.errorExtractIDs = ckbExtractIDs.Checked;
        }
    }
}