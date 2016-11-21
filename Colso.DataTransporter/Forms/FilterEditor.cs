using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Colso.DataTransporter.Forms
{
    public partial class FilterEditor : Form
    {
        private static Color HC_NODE = Color.Firebrick;
        private static Color HC_STRING = Color.Blue;
        private static Color HC_ATTRIBUTE = Color.Red;
        private static Color HC_COMMENT = Color.GreenYellow;
        private static Color HC_INNERTEXT = Color.Black;

        public string Filter
        {
            get {
                return txtFilter.Text.Trim();
            }
            set
            {
                txtFilter.Text = value;
                HighlightRTF(txtFilter);
            }
        }

        public FilterEditor(string currentfilter)
        {
            InitializeComponent();
            this.Filter = currentfilter;
        }

        private void BtnCloseClick(object sender, EventArgs e)
        {
            Close();
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            HighlightRTF(txtFilter);
        }

        private void HighlightRTF(RichTextBox rtb)
        {
            var selStart = rtb.SelectionStart;
            var selLength = rtb.SelectionLength;

            int k = 0;

            string str = rtb.Text;

            int st, en;
            int lasten = -1;
            while (k < str.Length)
            {
                st = str.IndexOf('<', k);

                if (st < 0)
                    break;

                if (lasten > 0)
                {
                    rtb.Select(lasten + 1, st - lasten - 1);
                    rtb.SelectionColor = HC_INNERTEXT;
                }

                en = str.IndexOf('>', st + 1);
                if (en < 0)
                    break;

                k = en + 1;
                lasten = en;

                if (str[st + 1] == '!')
                {
                    rtb.Select(st + 1, en - st - 1);
                    rtb.SelectionColor = HC_COMMENT;
                    continue;

                }
                String nodeText = str.Substring(st + 1, en - st - 1);

                bool inString = false;

                int lastSt = -1;
                int state = 0;
                /* 0 = before node name
                 * 1 = in node name
                   2 = after node name
                   3 = in attribute
                   4 = in string
                   */
                int startNodeName = 0, startAtt = 0;
                for (int i = 0; i < nodeText.Length; ++i)
                {
                    if (nodeText[i] == '"')
                        inString = !inString;

                    if (inString && nodeText[i] == '"')
                        lastSt = i;
                    else
                        if (nodeText[i] == '"')
                    {
                        rtb.Select(lastSt + st + 2, i - lastSt - 1);
                        rtb.SelectionColor = HC_STRING;
                    }

                    switch (state)
                    {
                        case 0:
                            if (!Char.IsWhiteSpace(nodeText, i))
                            {
                                startNodeName = i;
                                state = 1;
                            }
                            break;
                        case 1:
                            if (Char.IsWhiteSpace(nodeText, i))
                            {
                                rtb.Select(startNodeName + st, i - startNodeName + 1);
                                rtb.SelectionColor = HC_NODE;
                                state = 2;
                            }
                            break;
                        case 2:
                            if (!Char.IsWhiteSpace(nodeText, i))
                            {
                                startAtt = i;
                                state = 3;
                            }
                            break;

                        case 3:
                            if (Char.IsWhiteSpace(nodeText, i) || nodeText[i] == '=')
                            {
                                rtb.Select(startAtt + st, i - startAtt + 1);
                                rtb.SelectionColor = HC_ATTRIBUTE;
                                state = 4;
                            }
                            break;
                        case 4:
                            if (nodeText[i] == '"' && !inString)
                                state = 2;
                            break;
                    }
                }

                if (state == 1)
                {
                    rtb.Select(st + 1, nodeText.Length);
                    rtb.SelectionColor = HC_NODE;
                }
            }

            // reset selection
            rtb.Select(selStart, selLength);
        }
    }
}