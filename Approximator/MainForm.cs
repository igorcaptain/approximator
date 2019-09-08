using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Gauss;

namespace Approximator
{
    public partial class MainForm : Form
    {
        private List<Point> points = new List<Point>();
        private Point mid;
        private int dragp;
        private bool isEnd = false;
        private bool dragging = false;
        private int pointCount = 5;
        private int scaler = 75;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Width = this.Width - 16;
            pictureBox1.Height = this.Height;

            mid.X = pictureBox1.Width / 2 - 250;
            mid.Y = pictureBox1.Height / 2 + 100;
        }

        private void SquareCoord(PaintEventArgs e)
        {
            pictureBox1.BackColor = Color.FromArgb(255, 255, 255);
            Pen pen = new Pen(Color.FromArgb(0, 0, 0), 1);
            Brush brush = Brushes.Black;
            Font font = new Font("Arial", 12);
            Graphics g = e.Graphics;
            g.DrawLine(pen, mid.X, 0, mid.X, pictureBox1.Height);
            g.DrawLine(pen, 0, mid.Y, pictureBox1.Width, mid.Y);
            g.DrawString("0", font, brush, mid.X, mid.Y + 5);
            for (int i = 1; i <= pointCount + 1; i++)
            {
                g.DrawLine(pen, mid.X + i * scaler, mid.Y - 5, mid.X + i * scaler, mid.Y + 5);
                g.DrawString(i.ToString(), font, brush, mid.X + i * scaler - 7, mid.Y + 10);
            }
            g.DrawLine(pen, pictureBox1.Width, mid.Y, pictureBox1.Width - 10, mid.Y - 5);
            g.DrawLine(pen, pictureBox1.Width, mid.Y, pictureBox1.Width - 10, mid.Y + 5);
            g.DrawLine(pen, mid.X, 0, mid.X + 5, 10);
            g.DrawLine(pen, mid.X, 0, mid.X - 5, 10);
        }

        private void LegendText(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Font font = new Font("Arial", 16);
            g.DrawLine(new Pen(Color.Red, 4), Width - 250, 30, Width - 220, 30);
            g.DrawLine(new Pen(Color.Yellow, 4), Width - 250, 60, Width - 220, 60);
            g.DrawString("МНК", font, Brushes.Red, Width - 220, 20);
            g.DrawString("Поліном Лагранжа", font, Brushes.Yellow, Width - 220, 50);
        }

        private void Graph(PaintEventArgs e)
        {
            Pen pen = new Pen(Color.FromArgb(0, 0, 255), 1);
            Graphics g = e.Graphics;
            for (int i = 0; i < points.Count - 1; i++)
                g.DrawLine(pen, points[i], points[i + 1]);
        }

        private void DrawPoint(PaintEventArgs e)
        {
            Pen pen = new Pen(Color.FromArgb(255, 0, 0), 1);
            Brush brush = Brushes.Purple;
            Graphics g = e.Graphics;
            foreach(Point p in points)
                g.FillEllipse(brush, p.X - 10, p.Y - 10, 20, 20);
        }

        private double[] Fi(double x)
        {
            return new double[] { 1, x, x * x, x * x * x };
        }

        private double LeastSquares(double x)
        {
            //
            int k = 4;

            double[][] FiX = new double[pointCount][];
            int iter = 0;
            foreach(Point p in points)
                FiX[iter++] = Fi(p.X);
            double[][] C = new double[k][];
            for(int j = 0; j < k; j++)
            {
                C[j] = new double[k];
                for(int m = 0; m < k; m++)
                {
                    double S = 0.0;
                    for (int i = 0; i < pointCount; i++)
                        S = S + FiX[i][j] * FiX[i][m];
                    C[j][m] = S;
                }
            }
            double[] d = new double[k];
            for(int j = 0; j < k; j++)
            {
                double S = 0.0;
                for (int i = 0; i < pointCount; i++)
                    S = S + points[i].Y * FiX[i][j];
                d[j] = S;
            }
            double[][] matr = new double[k][];
            for(int i = 0; i < k; i++)
            {
                matr[i] = new double[k + 1];
                for (int j = 0; j < k; j++)
                    matr[i][j] = C[i][j];
                matr[i][k] = d[i];
            }
            LinearSystem a = new LinearSystem(matr);
            //return a.XVector[2]*x*x + a.XVector[1]*x + a.XVector[0];
            return a.XVector[3] * x * x * x + a.XVector[2] * x * x + a.XVector[1] * x + a.XVector[0];
        }

        private double Lagrange(double x)
        {
            double lagrangePol = 0;
            double[] XVal = new double[pointCount];
            double[] YVal = new double[pointCount];
            int iter = 0;
            foreach(Point p in points)
            {
                XVal[iter] = p.X; YVal[iter] = p.Y;
                iter++;
            }

            for (int i = 0; i < pointCount; i++)
            {
                double basicsPol = 1;
                for (int j = 0; j < pointCount; j++)
                {
                    if (j != i)
                    {
                        basicsPol *= (x - XVal[j]) / (XVal[i] - XVal[j]);
                    }
                }
                lagrangePol += basicsPol * YVal[i];
            }

            return lagrangePol;
        }

        private void DrawInterpolation(Func<double, double> func, Brush brush, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            for (int i = mid.X; i < mid.X + (pointCount+1)*scaler; i++)
                g.FillEllipse(brush, i, (int)func(i), 5, 5);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            SquareCoord(e);
            DrawPoint(e);
            Graph(e);
            if (isEnd)
            {
                DrawInterpolation(LeastSquares, Brushes.Red, e);
                DrawInterpolation(Lagrange, Brushes.Yellow, e);
                LegendText(e);
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if(points.Count == pointCount - 1)
                isEnd = true;
            if (points.Count == pointCount)
            {
                (sender as Control).Invalidate();
                return;
            }
            points.Add(new Point(mid.X + (points.Count + 1) * scaler, e.Y));
            (sender as Control).Invalidate();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                points[dragp - 1] = new Point(points[dragp - 1].X, e.Y);
                (sender as Control).Invalidate();
            }
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            //
            dragp = 0;
            foreach (Point p in points)
            {
                dragp++;
                float dot_cirDiff = (float)Math.Sqrt(Math.Pow(e.X - p.X, 2) + Math.Pow(e.Y - p.Y, 2));
                if (dot_cirDiff < 10)
                {
                    //D&D activate!
                    dragging = true;
                    break;
                }
            }
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Width = this.Width - 16;
            pictureBox1.Height = this.Height;
        }
    }
}
