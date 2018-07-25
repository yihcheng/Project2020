using System;
using System.Collections.Generic;
using System.Threading;
using CommonContracts;

namespace WindowsComputer
{
    public class WindowsKeyboard : IKeyboard
    {
        private readonly IMouse _mouse;

        public WindowsKeyboard(IMouse mouse)
        {
            _mouse = mouse;
        }

        public void SendKeys(string keyString)
        {
            DoKeyboardWork(keyString);
        }

        public void SendKeys(int x, int y, string keyString)
        {
            if (_mouse != null)
            {
                _mouse.Click(x, y);
                Thread.Sleep(500);
            }

            DoKeyboardWork(keyString);
        }

        private void DoKeyboardWork(string keyString)
        {
            List<string> keyStreams = DivideKeyString(keyString);

            foreach (string key in keyStreams)
            {
                string convertedKey = ConvertKey(key);

                if (!string.IsNullOrEmpty(convertedKey))
                {
                    System.Windows.Forms.SendKeys.SendWait(convertedKey);
                    Thread.Sleep(500);
                }
            }
        }

        internal List<string> DivideKeyString(string keyString)
        {
            List<string> keyStreams = new List<string>();
            int index1 = 0;
            int index2 = index1;
            int strLength = keyString.Length;
            string keys;

            while (index1 < strLength && index2 < strLength)
            {
                if (keyString.Substring(index2, 1).Equals("]"))
                {
                    if (index1 != index2)
                    {
                        keys = keyString.Substring(index1 + 1, index2 - index1 - 1);

                        if (!String.IsNullOrEmpty(keys))
                        {
                            keyStreams.Add(keys);
                        }

                        index1 = index2 + 1;
                        index2 = index1;
                    }
                    else
                    {
                        index2++;
                    }
                }
                else if (keyString.Substring(index2, 1).Equals("["))
                {
                    if (index1 == index2)
                    {
                        index2++;
                    }
                    else
                    {
                        keys = keyString.Substring(index1, index2 - index1);
                        keyStreams.Add(keys);
                        index1 = index2;
                        index2++;
                    }
                }
                else
                {
                    index2++;
                }
            }

            if (index1 < strLength)
            {
                keyStreams.Add(keyString.Substring(index1));
            }

            return keyStreams;
        }

        internal string ConvertKey(string key)
        {
            const string CtrlCode = "^";
            const string AltCode = "%";
            const string ShiftCode = "+";

            string[] s = key.Split('+');

            if (s.Length == 1)
            {
                return s[0];
            }

            if (string.Equals(s[0], "ctrl", StringComparison.OrdinalIgnoreCase))
            {
                return s[1].Length > 1 ? CtrlCode + "(" + s[1] + ")" : CtrlCode + s[1];
            }
            else if (string.Equals(s[0], "alt", StringComparison.OrdinalIgnoreCase))
            {
                return s[1].Length > 1 ? AltCode + "(" + s[1] + ")" : AltCode + s[1];
            }
            else if (string.Equals(s[0], "shift", StringComparison.OrdinalIgnoreCase))
            {
                return s[1].Length > 1 ? ShiftCode + "(" + s[1] + ")" : ShiftCode + s[1];
            }

            return "";
        }
    }
}
