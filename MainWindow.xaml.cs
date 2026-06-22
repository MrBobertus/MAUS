using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;


namespace maus
{
    public partial class MainWindow : Window
    {

        // =========================
        // Variables
        // =========================

        private DispatcherTimer clickTimer;
        private bool isRunning = false;
        private Key selectedHotkey;

        // =========================
        // Konstruktor
        // =========================

        public MainWindow()
        {
            InitializeComponent();

            MouseButtonBox.Items.Add(
                new MouseClickType()
                {
                    Name = "Left Button",
                    Down = 0x0002,
                    Up = 0x0004
                }
            );

            MouseButtonBox.Items.Add(
                new MouseClickType()
                {
                    Name = "Middle Button",
                    Down = 0x0020,
                    Up = 0x0040
                }
            );

            MouseButtonBox.Items.Add(
                new MouseClickType()
                {
                    Name = "Right Button",
                    Down = 0x0008,
                    Up = 0x0010
                }
            );

            clickTimer = new DispatcherTimer();

            clickTimer.Tick += ClickTimer_Tick;
        }



        // =========================
        // Button Events
        // =========================

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            int hours = 0;
            int minutes = 0;
            int seconds = 0;
            int milliseconds = 0;


            int.TryParse(HoursIntervalBox.Text, out hours);
            int.TryParse(MinutesIntervalBox.Text, out minutes);
            int.TryParse(SecondsIntervalBox.Text, out seconds);
            int.TryParse(MillisecondsIntervalBox.Text, out milliseconds);


            int interval =
                (hours * 60 * 60 * 1000)
                + (minutes * 60 * 1000)
                + (seconds * 1000)
                + milliseconds;

            clickTimer.Interval = TimeSpan.FromMilliseconds(interval);

            Start_Clicker();
        }


        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Stop_Clicker();
        }



        // =========================
        // Eigene Funktionen
        // =========================

        private void SimulateMouse_Click()
        {
            int clickCount = 1;
            GetCursorPos(out POINT point);
            MouseClickType selected = (MouseClickType)MouseButtonBox.SelectedItem;

            if (ClickTypeBox.SelectedIndex == 0)
            {
                clickCount = 1;
            }
            else if (ClickTypeBox.SelectedIndex == 1)
            {
                clickCount = 2;
            }

            for (int i = 0; i < clickCount; i++)
            {
                mouse_event(selected.Down, point.X, point.Y, 0, 0);
                mouse_event(selected.Up, point.X, point.Y, 0, 0);
            }
        }

        // unused keypboard simulation for future feature
        private void SimulateKey_Press()
        {
            keybd_event(0x41, 0, 0, 0);
            keybd_event(0x41, 0, 2, 0);
        }

        private void ClickTimer_Tick(object sender, EventArgs e)
        {
            SimulateMouse_Click();
        }

        private void IntervalBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == selectedHotkey)
            {
                if (isRunning)
                {
                    Stop_Clicker();
                }
                else
                {
                    Start_Clicker();
                }
            }
        }

        private void Start_Clicker()
        {
            StatusText.Text = "Status: On";
            clickTimer.Start();
            isRunning = true;
        }

        private void Stop_Clicker()
        {
            StatusText.Text = "Status: Off";
            clickTimer.Stop();
            isRunning = false;
        }

        private void HotkeyBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            selectedHotkey = e.Key;
            HotkeyBox.Text = e.Key.ToString();
            e.Handled = true;
        }

        // =========================
        // Windows API
        // =========================


        [DllImport("user32.dll")]
        public static extern void mouse_event(
            int dwFlags,
            int dx,
            int dy,
            int cButtons,
            int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern void keybd_event(
            byte bVk,
            byte bScan,
            uint dwFlags,
            int dwExtraInfo);


        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out POINT lpPoint);


        // =========================
        // Hilfsdaten
        // =========================


        public struct POINT
        {
            public int X;
            public int Y;
        }

    }

    public class MouseClickType
    {
        public string Name { get; set; }
        public int Down { get; set; }
        public int Up { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}