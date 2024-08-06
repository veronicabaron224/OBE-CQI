using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OBE_CQI
{
    public partial class GradeForm : Form
    {
        private Size formSize;

        public GradeForm()
        {
            InitializeComponent();
        }

        #region Event methods
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            formSize = this.ClientSize;
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                formSize = this.ClientSize;
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                this.Size = formSize;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
        #endregion

        #region Overridden methods
        protected override void WndProc(ref Message m)
        {
            const int WM_NCCALCSIZE = 0x0083;//Standar Title Bar - Snap Window
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MINIMIZE = 0xF020; //Minimize form (Before)
            const int SC_RESTORE = 0xF120; //Restore form (Before)
            const int WM_NCHITTEST = 0x0084;//Win32, Mouse Input Notification: Determine what part of the window corresponds to a point, allows to resize the form.
            const int resizeAreaSize = 10;

            #region Form Resize
            const int HTCLIENT = 1;
            const int HTLEFT = 10;
            const int HTRIGHT = 11;
            const int HTTOP = 12;  
            const int HTTOPLEFT = 13;
            const int HTTOPRIGHT = 14;
            const int HTBOTTOM = 15; 
            const int HTBOTTOMLEFT = 16;
            const int HTBOTTOMRIGHT = 17;

            if (m.Msg == WM_NCHITTEST)
            {
                base.WndProc(ref m);
                if (this.WindowState == FormWindowState.Normal)
                {
                    if ((int)m.Result == HTCLIENT)
                    {
                        Point screenPoint = new Point(m.LParam.ToInt32()); 
                        Point clientPoint = this.PointToClient(screenPoint);                    

                        if (clientPoint.Y <= resizeAreaSize)
                        {
                            if (clientPoint.X <= resizeAreaSize) 
                                m.Result = (IntPtr)HTTOPLEFT; 
                            else if (clientPoint.X < (this.Size.Width - resizeAreaSize))
                                m.Result = (IntPtr)HTTOP;
                            else 
                                m.Result = (IntPtr)HTTOPRIGHT;
                        }
                        else if (clientPoint.Y <= (this.Size.Height - resizeAreaSize)) 
                        {
                            if (clientPoint.X <= resizeAreaSize)
                                m.Result = (IntPtr)HTLEFT;
                            else if (clientPoint.X > (this.Width - resizeAreaSize))
                                m.Result = (IntPtr)HTRIGHT;
                        }
                        else
                        {
                            if (clientPoint.X <= resizeAreaSize)
                                m.Result = (IntPtr)HTBOTTOMLEFT;
                            else if (clientPoint.X < (this.Size.Width - resizeAreaSize)) 
                                m.Result = (IntPtr)HTBOTTOM;
                            else 
                                m.Result = (IntPtr)HTBOTTOMRIGHT;
                        }
                    }
                }
                return;
            }
            #endregion

            if (m.Msg == WM_NCCALCSIZE && m.WParam.ToInt32() == 1)
            {
                return;
            }

           
            if (m.Msg == WM_SYSCOMMAND)
            {
                int wParam = (m.WParam.ToInt32() & 0xFFF0);

                if (wParam == SC_MINIMIZE)
                    formSize = this.ClientSize;
                if (wParam == SC_RESTORE)
                    this.Size = formSize;
            }
            base.WndProc(ref m);
        }
        #endregion

        //Grade chart methods
        private void GradeForm_Load(object sender, EventArgs e)
        {
            chartDataBindingSource.DataSource = new List<ChartData>();
            cartesianChart1.AxisX.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Class Number",
                Labels = new[] {"1", "40" } //Array aka Data Structure
            });
            cartesianChart1.AxisY.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Grade",
                LabelFormatter = grade => grade.ToString()
            });
            cartesianChart1.LegendLocation = LiveCharts.LegendLocation.Right;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            cartesianChart1.Series.Clear();     
            SeriesCollection series = new SeriesCollection();
            var names = (from o in chartDataBindingSource.DataSource as List<ChartData>
                         select new { Name = o.Name }).Distinct();
            foreach (var name in names)
            {
                List<double> grades = new List<double>();
                for (int classnumber = 1; classnumber <= 10; classnumber++)
                {
                    double grade = 0;
                    var data = from o in chartDataBindingSource.DataSource as List<ChartData>
                               where o.Name.Equals(name.Name) && o.ClassNumber.Equals(classnumber)
                               orderby o.ClassNumber ascending //Sorting aka Data Structure
                               select new { o.Grade, o.ClassNumber };
                    if (data.SingleOrDefault() != null)
                        grade = data.SingleOrDefault().Grade;
                    grades.Add(grade);
                }
                series.Add(new LineSeries() { Title = name.Name.ToString(), Values = new ChartValues<double>(grades) });
            }
            cartesianChart1.Series = series;
        }
    }
}
