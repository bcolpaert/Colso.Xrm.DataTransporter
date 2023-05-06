using System;
using System.Drawing;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace Colso.DataTransporter.Forms
{
    public partial class FilterEditor : Form
    {
        private static Color HC_NODE = Color.Firebrick;
        private static Color HC_STRING = Color.Blue;
        private static Color HC_ATTRIBUTE = Color.Red;
        private static Color HC_COMMENT = Color.GreenYellow;
        private static Color HC_INNERTEXT = Color.Black;

        private bool _isHighlightBusy = false;
        private readonly DataTransporter _parentForm = null;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern int GetScrollPos(IntPtr hWnd, int nBar);
        public enum Message : uint
        {
            WM_VSCROLL = 0x0115,
            WM_SETREDRAW = 0x0b
        }

        public enum ScrollBarType : uint
        {
            SbHorz = 0,
            SbVert = 1,
            SbCtl = 2,
            SbBoth = 3
        }

        public enum ScrollBarCommands : uint
        {
            SB_THUMBPOSITION = 4
        }

        public string Filter
        {
            get
            {
                return txtFilter.Text.Trim();
            }
            set
            {
                txtFilter.Text = value;
                HighlightRTF(txtFilter);
            }
        }

        public FilterEditor()
        {
            InitializeComponent();
        }

        public FilterEditor(string currentfilter, DataTransporter parentForm)
        {
            InitializeComponent();
            this.Filter = currentfilter;
            _parentForm = parentForm;
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            tmrTextChanged.Stop();
            if (!_isHighlightBusy) tmrTextChanged.Start();
            txtFilter.Invalidate();
        }

        private void tmrTextChanged_Tick(object sender, EventArgs e)
        {
            HighlightRTF(txtFilter);
            txtFilter.Invalidate();
        }

        private void HighlightRTF(RichTextBox rtb)
        {
            tmrTextChanged.Stop();
            _isHighlightBusy = true;

            // BeginUpdate
            SendMessage(rtb.Handle, (int)Message.WM_SETREDRAW, (IntPtr)0, IntPtr.Zero);

            var selStart = rtb.SelectionStart;
            var selLength = rtb.SelectionLength;

            var scrollPos = GetScrollPos(rtb.Handle, (int)ScrollBarType.SbVert);
            scrollPos <<= 16;

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
            rtb.SelectionColor = HC_INNERTEXT;
            // reset scroll position
            uint wParam = (uint)ScrollBarCommands.SB_THUMBPOSITION | (uint)scrollPos;
            SendMessage(rtb.Handle, (int)Message.WM_VSCROLL, new IntPtr(wParam), IntPtr.Zero);

            //EndUpdate
            SendMessage(rtb.Handle, (int)Message.WM_SETREDRAW, (IntPtr)1, IntPtr.Zero);

            _isHighlightBusy = false;
        }

        private void btnFxb_Click(object sender, EventArgs e)
        {
            _parentForm.OpenFxb(txtFilter.Text);
        }
    }
}