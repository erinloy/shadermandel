using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShaderMandelbrot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        bool mmd = false;
        bool smd = false;
        bool switchMode = false;
        double mrg = 20;
        Point lmp;
        Point smp;

        private void MandelRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double p = MandelRoot.ActualHeight / MandelRoot.ActualWidth;
            m.Size = new Point(m.Size.Y * p, m.Size.Y);
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            Point mp = e.GetPosition(MandelRoot);
            if (switchMode)
            {
                double x = mp.X / MandelRoot.ActualWidth / m.Size.X + m.Offset.X;
                double y = mp.Y / MandelRoot.ActualHeight / m.Size.Y + m.Offset.Y;
                j.seedR.Value = x;
                j.seedI.Value = y;
                j.m.Seed = new Point(x, y);
            }
            else
            {
                if (mmd)
                {
                    double px = lmp.X - mp.X;
                    double py = lmp.Y - mp.Y;
                    px = px / MandelRoot.ActualWidth / m.Size.X;
                    py = py / MandelRoot.ActualHeight / m.Size.Y;
                    m.Offset = new Point(m.Offset.X + px, m.Offset.Y + py);
                }
                lmp = mp;
                if (mmd)
                {
                    if (mp.X >= MandelRoot.ActualWidth - mrg || mp.Y >= MandelRoot.ActualHeight - mrg || mp.X <= mrg || mp.Y <= mrg)
                    {
                        double X = 0;
                        double Y = 0;
                        if (mp.X >= MandelRoot.ActualWidth - mrg)
                            X -= (int)MandelRoot.ActualWidth - mrg * 2;
                        else if (mp.X <= mrg)
                            X += (int)MandelRoot.ActualWidth - mrg * 2;
                        if (mp.Y >= MandelRoot.ActualHeight - mrg)
                            Y -= (int)MandelRoot.ActualHeight - mrg * 2;
                        else if (mp.Y <= mrg)
                            Y += (int)MandelRoot.ActualHeight - mrg * 2;
                        lmp = new Point(lmp.X + X, lmp.Y + Y);
                        System.Windows.Forms.Cursor.Position = new System.Drawing.Point(System.Windows.Forms.Cursor.Position.X + (int)X, System.Windows.Forms.Cursor.Position.Y + (int)Y);
                    }
                }
                if (smd)
                {
                    darker.Width = (lmp.X - smp.X < 0 ? 0 : lmp.X - smp.X);
                    darker.Height = darker.Width * (MandelRoot.ActualHeight / MandelRoot.ActualWidth);
                }
            }
        }

        private void maxIter_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m.MaxIter = e.NewValue;
            if (switchMode)
                j.m.MaxIter = e.NewValue;
        }

        private void powerR_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m.Power = new Point(e.NewValue, m.Power.Y);
            if (switchMode)
                j.m.Power = new Point(e.NewValue, m.Power.Y);
        }

        private void powerI_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m.Power = new Point(m.Power.X, e.NewValue);
            if (switchMode)
                j.m.Power = new Point(m.Power.X, e.NewValue);
        }

        private void bailout_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m.Bailout = e.NewValue;
            if (switchMode)
                j.m.Bailout = e.NewValue;
        }

        private void MandelRoot_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!switchMode)
            {
                lmp = e.GetPosition(MandelRoot);
                smp = e.GetPosition(MandelRoot);
                if (e.ChangedButton == MouseButton.Middle)
                {
                    mmd = true;
                }
                else if (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Right)
                {
                    smd = true;
                    darker.Margin = new Thickness(smp.X, smp.Y, 0, 0);
                    darker.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        private void MandelRoot_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Point mp = e.GetPosition(MandelRoot);
            if (switchMode)
            {
                switchMode = false;
                MandelRoot.Cursor = Cursors.Arrow;
                double x = mp.X / MandelRoot.ActualWidth / m.Size.X + m.Offset.X;
                double y = mp.Y / MandelRoot.ActualHeight / m.Size.Y + m.Offset.Y;
                j.seedR.Value = x;
                j.seedI.Value = y;
                j.m.Seed = new Point(x, y);
                j.WindowState = System.Windows.WindowState.Maximized;
                j.Title = "ShaderJulii";
                j.options.Visibility = System.Windows.Visibility.Visible;
                j.Focus();
            }
            else
            {
                if (e.ChangedButton == MouseButton.Middle)
                {
                    mmd = false;
                }
                else if (e.ChangedButton == MouseButton.Left)
                {
                    darker.Visibility = System.Windows.Visibility.Collapsed;
                    smd = false;
                    Point emp = e.GetPosition(MandelRoot);
                    if (emp.X - smp.X > 10)
                    {
                        double d = (emp.X - smp.X) / MandelRoot.ActualWidth;
                        m.Offset = new Point(m.Offset.X + smp.X / MandelRoot.ActualWidth / m.Size.X, m.Offset.Y + smp.Y / MandelRoot.ActualHeight / m.Size.Y);
                        m.Size = new Point(m.Size.X / d, m.Size.Y / d);
                    }
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    darker.Visibility = System.Windows.Visibility.Collapsed;
                    smd = false;
                    Point emp = e.GetPosition(MandelRoot);
                    if (emp.X - smp.X > 10)
                    {
                        double d = (emp.X - smp.X) / MandelRoot.ActualWidth;
                        m.Size = new Point(m.Size.X * d, m.Size.Y * d);
                        m.Offset = new Point(m.Offset.X - smp.X / MandelRoot.ActualWidth / m.Size.X, m.Offset.Y - smp.Y / MandelRoot.ActualHeight / m.Size.Y);
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            maxIter.Value = 32;
            powerR.Value = 2;
            powerI.Value = 0;
            bailout.Value = 4;
            m.MaxIter = 32;
            m.Power = new Point(2, 0);
            m.Bailout = 4;
            double p = MandelRoot.ActualHeight / MandelRoot.ActualWidth;
            m.Size = new Point(m.Size.Y * p, m.Size.Y);
            m.Offset = new Point(-3, -2);
        }

        JuliiWindow j;
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            switchMode = !switchMode;
            if (switchMode)
            {
                MessageBox.Show("Select point on Mandelbrot.");
                MandelRoot.Cursor = Cursors.Cross;
                j = new JuliiWindow();
                j.ShowActivated = false;
                j.Left = 0;
                j.Top = 0;
                j.Title = "Preview";
                j.maxIter.Value = this.maxIter.Value;
                j.powerR.Value = this.powerR.Value;
                j.powerI.Value = this.powerI.Value;
                j.bailout.Value = this.bailout.Value;
                j.Closing += new System.ComponentModel.CancelEventHandler(j_Closing);
                j.Show();
            }
            else
            {
                MandelRoot.Cursor = Cursors.Arrow;
                j.Close();
            }
        }

        void j_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            switchMode = false;
            MandelRoot.Cursor = Cursors.Arrow;
        }
    }
}
