using Colso.DataTransporter.AppCode;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using XrmToolBox;
using static System.Windows.Forms.ListViewItem;

namespace Colso.Xrm.DataTransporter.AppCode
{
    public static class Enumerations
    {
        public enum TransferMode
        {
            None = 0,
            Preview = 1,
            Create = 2,
            Update = 4,
            Delete = 8
        }

        public enum enCopyOptions // 1,2,4,8,16,32,64 etc...
        {
            CSV = 1,
            TAB = 2,
            //Other3 = 4,
            //Other4 = 8,
            //Other5 = 16,
            //Other6 = 32,
            //Other7 = 64,
            //Other8 = 128,
            //Other9 = 256,
            ExtractIDs = 512,
            SelectedLinesOnly = 1024

        };

        public static Dictionary<int, string> clipFormatItems = new Dictionary<int, string>()
        {
            {(int) enCopyOptions.CSV, "CSV Format"},
            {(int) enCopyOptions.TAB, "TAB Delimited"},
        };

        private static string regexGUID = "[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}";

        /// <summary>
        /// Turn a string into a CSV cell output
        /// </summary>
        /// <param name="str">String to output</param>
        /// <returns>The CSV cell formatted string</returns>
        /// https://stackoverflow.com/questions/6377454/escaping-tricky-string-to-csv-format
        public static string stringToCSVCell(string str)
        {
            bool mustQuote = (str.Contains(",") || str.Contains("\"") || str.Contains("\r") || str.Contains("\n"));
            if (mustQuote) {
                StringBuilder sb = new StringBuilder();
                sb.Append("\"");
                foreach (char nextChar in str) {
                    sb.Append(nextChar);
                    if (nextChar == '"')
                        sb.Append("\"");
                }
                sb.Append("\"");
                return sb.ToString();
            }

            return str;
        }



        public static void handleListViewKeyUp(object sender, KeyEventArgs e, Settings settings)
        {

            ListView thisListView = (ListView)sender;

            if (thisListView.GetType().Name != "ListView") return;

            if (e.Control && e.KeyCode == Keys.C) {

                int _enCopyOptions = 0;

                _enCopyOptions |= (int)settings.CopyFormatType;
                if (settings.IncludeExtractGUIDs) _enCopyOptions |= (int)enCopyOptions.ExtractIDs;

                //
                // copy only selected listview items
                //
                _enCopyOptions |= (int)enCopyOptions.SelectedLinesOnly;

                Clipboard.SetText(listviewToOutputFormat(thisListView, _enCopyOptions));

            }
            else
            if (e.Control && e.KeyCode == Keys.A) {
                thisListView.BeginUpdate();
                foreach (ListViewItem lvi in thisListView.Items) {
                    lvi.Selected = true;
                }
                thisListView.EndUpdate();

            }

        }


        /// <summary>
        /// Iterate through a listview and turn it's data into a (currently) CSV string
        /// </summary>
        /// <param name="_listView">Listview to get data from</param>
        /// <param name="_copyOptions">Output options</param>
        /// <returns>A CSV data formatted string</returns>
        public static string listviewToOutputFormat(ListView _listView, int _copyOptions)
        {
            StringBuilder result = new StringBuilder();
            StringBuilder header = new StringBuilder();
            var isFirstItem = true;
            var maxColumns = 0;
            var currentColumn = 0;
            var checkValidFormat = _copyOptions & ((int)enCopyOptions.CSV | (int)enCopyOptions.TAB);
            bool copySelectedOnly = (_copyOptions & (int)enCopyOptions.SelectedLinesOnly) > 0;

            if (checkValidFormat != 0) {
                //
                // iterate through each listview item and produce a valid line and dump it in the stringbuilder result bucket
                //
                foreach (ListViewItem item in _listView.Items) {
                    StringBuilder fullLine = new StringBuilder();
                    isFirstItem = true;
                    currentColumn = 0;

                    if (copySelectedOnly == true && item.Selected == false ) continue;

                    foreach (ListViewSubItem subitem in item.SubItems) {

                        if (!isFirstItem) fullLine.Append(getSeperatorBasedOnCopyOptions(_copyOptions)); else isFirstItem = false;

                        //
                        // escape the escapees
                        //
                        fullLine.Append(stringToCSVCell(subitem.Text));
                        currentColumn++;
                    }

                    //
                    // check request to add extra(cted) GUIDs to line ends, GUIDs don't require escaping
                    //
                    if ((_copyOptions & (int)enCopyOptions.ExtractIDs) != 0) {
                        foreach (Match m in Regex.Matches(fullLine.ToString(), regexGUID, RegexOptions.IgnoreCase)) {
                            fullLine.Append(getSeperatorBasedOnCopyOptions(_copyOptions));
                            fullLine.Append(m.Value);
                            currentColumn++;
                        }
                    }

                    maxColumns = Math.Max(maxColumns, currentColumn);

                    result.Append(fullLine);

                    //
                    // CRLF
                    //
                    result.AppendLine();
                }

            }


            //
            // now build the header (based on the format requirement)
            //
            if (checkValidFormat != 0) {
                isFirstItem = true;

                for (int i = 0; i < maxColumns; i++) {
                    string columnText = "";

                    if (!isFirstItem) header.Append(getSeperatorBasedOnCopyOptions(_copyOptions)); else isFirstItem = false;

                    //
                    // do actual & fake columns
                    //
                    columnText = i < _listView.Columns.Count ? _listView.Columns[i].Text : columnText = $"GUID{(i)}"; ;
                    header.Append(stringToCSVCell(columnText));

                }

                //
                // CRLF
                //
                header.AppendLine();

                result.Insert(0, header);
            }

            return result.ToString();
        }

        public static string getSeperatorBasedOnCopyOptions(int _copyOptions)
        {
            var checkValidFormat = _copyOptions & ((int)enCopyOptions.CSV | (int)enCopyOptions.TAB);
            var seperator = "";
            switch (checkValidFormat) {
                case (int)enCopyOptions.CSV:
                    seperator += ",";
                    break;
                case (int)enCopyOptions.TAB:
                    seperator += "\t";
                    break;
            }
            return seperator;
        }

        public static int _calcProgress(int item, int items)
        {
            return (int)(((float)item / items) * 100.0f);
        }
    }


}

