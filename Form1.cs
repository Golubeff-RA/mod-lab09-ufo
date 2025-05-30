using ScottPlot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace UFO_flying
{
    public partial class Form1 : Form
    {
        private int radius = 10;
        private int step = 5;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        private int Factorial(int n) => n == 0 ? 1 : n * Factorial(n - 1);

        private float MyCos(double x, int n, int round)
        {
            double answer = 0;
            for (int i = 0; i < n; ++i)
                answer += Math.Pow(-1, i) * Math.Pow(x, i * 2) / Factorial(i * 2);

            return (float)Math.Round(answer, round);
        }

        private float MySin(double x, int n, int round)
        {
            double answer = 0;
            for (int i = 0; i < n; ++i)
                answer += Math.Pow(-1, i) * Math.Pow(x, i * 2 + 1) / Factorial(i * 2 + 1);

            return (float)Math.Round(answer, round);
        }

        private float MyAtan(double x, int n, int round)
        {
            if (Math.Abs(x) > 1)
                return (float)(Math.Sign(x) * Math.PI / 2 - MyAtan(1 / x, n, round));

            double answer = 0;
            for (int i = 0; i < n; ++i)
                answer += Math.Pow(-1, i) * Math.Pow(x, i * 2 + 1) / (i * 2 + 1);

            return (float)Math.Round(answer, round);
        }

        private void DrawCircle(Point center, int raduis, Pen pen, Graphics g)
        {
            g.DrawEllipse(pen, center.X - raduis, center.Y - raduis, raduis * 2, raduis * 2);
        }

        private void startButton_Click(object sender, EventArgs e)
        {

            using (Graphics g = this.CreateGraphics())
            {
                Point start = new Point(5, this.Height - 50 - 2 * radius);
                Point end = new Point(this.Width - radius * 3, radius);
                g.Clear(this.BackColor);
                DrawCircle(start, 2, Pens.Green, g);
                DrawCircle(end, radius, Pens.Red, g);

                List<double> radiuses = new List<double>();
                List<double> n_s = new List<double>();

                StreamWriter w = new StreamWriter("../../result/data.txt", false);
                for (float r = 0; r < 30; r += 0.5f)
                {
                    g.Clear(this.BackColor);
                    DrawCircle(start, 2, Pens.Green, g);
                    DrawCircle(end, (int)r, Pens.Red, g);

                    bool collision = false;
                    int n = 0;
                    do
                    {
                        ++n;
                        collision = DrawLine(g, end, start, n, 5, r);
                    } while (!collision && n < 12);

                    w.WriteLine("{0} {1}", r, n);
                    radiuses.Add(r);
                    n_s.Add(n);

                    var plot = new Plot();
                    plot.Add.Scatter(radiuses, n_s);
                    plot.Title("Зависимость числа членов в ряде Тейлора от радиуса");
                    plot.XLabel("Радиус");
                    plot.YLabel("Число членов ряда");
                    plot.SavePng("../../result/plot.png", 800, 600);
                }

                w.Close();
            }
        }

        private double Distance(PointF left, PointF right)
        {
            return Math.Sqrt(Math.Pow(left.X - right.X, 2) + Math.Pow(left.Y - right.Y, 2));
        }

        private bool DrawLine(Graphics g, Point end, Point start, int n, int round, double rad)
        {
            float angle = MyAtan(((double)Math.Abs(end.Y - start.Y) / (double)Math.Abs(end.X - start.X)), n, round);
            bool collision = false;
            PointF p = start;
            while (Distance(p, end) > rad && p.X < this.Width && p.Y > 0)
            {
                p.X += step * MyCos(angle, n, round);
                p.Y -= step * MySin(angle, n, round);
                g.FillEllipse(Brushes.Black, p.X, p.Y, 2, 2);
            }

            if (Distance(p, end) <= rad)
                collision = true;

            return collision;
        }


    }
}
