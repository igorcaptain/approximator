using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Approximator
{
    public partial class MainForm : Form
    {
        private List<Point> points = new List<Point>();
        private Point cursor;
        private Point mid;
        private int dragp, x, y;
        private bool isFive = false;
        private bool dragging = false;
        private int pointCount = 5;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            mid.X = pictureBox1.Width / 2;
            mid.Y = pictureBox1.Height / 2;

            mid.X = pictureBox1.Width / 2 - 350;
            mid.Y = pictureBox1.Height / 2 + 100;


            /*
            double[][] undvec = { new double[] { 0.0, 0.0, 0.0, 1.0, 60.0 },
                                  new double[] { 1000.0, 100.0, 10.0, 1.0, 90.0 },
                                  new double[] { 8000.0, 400.0, 20.0, 1.0, 80.0 },
                                  new double[] { 27000.0, 900.0, 30.0, 1.0, 20.0 },
                                  new double[] { 64000.0, 1600.0, 40.0, 1.0, 10.0 }
                                };
            double[] bvec = { 60.0, 90.0, 80.0, 20.0 };
            LinearSystem a = new LinearSystem(undvec);
            */
            
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
            for (int i = 1; i < 7; i++)
            {
                g.DrawLine(pen, mid.X + i * 75, mid.Y - 5, mid.X + i * 75, mid.Y + 5);
                g.DrawString(i.ToString(), font, brush, mid.X + i * 75 - 7, mid.Y + 10);
            }
            g.DrawLine(pen, pictureBox1.Width, mid.Y, pictureBox1.Width - 10, mid.Y - 5);
            g.DrawLine(pen, pictureBox1.Width, mid.Y, pictureBox1.Width - 10, mid.Y + 5);
            g.DrawLine(pen, mid.X, 0, mid.X + 5, 10);
            g.DrawLine(pen, mid.X, 0, mid.X - 5, 10);
        }

        private void Graph(PaintEventArgs e)
        {
            Pen pen = new Pen(Color.FromArgb(255, 0, 0), 1);
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
                //g.DrawEllipse(pen, p.X - 3, p.Y - 3, 6, 6);
                g.FillEllipse(brush, p.X - 10, p.Y - 10, 20, 20);
        }

        private double[] LeastSquares()
        {
            listBox1.Items.Clear();
            foreach (Point p in points)
                listBox1.Items.Add(p.X + ";" + p.Y);


            //double[,] undvec = new double[pointCount + 1, 5];
            //double[] bvec = new double[pointCount];
            /*
            double[][] undvec = new double[pointCount][];

            SparseMatrix matrix = new SparseMatrix(pointCount);
            double[] bvec = new double[4];
            for (int i = 0; i < pointCount; i++)
            {
                //undvec[i] = new double[5];
                for(int j = 0; j < 4; j++)
                {
                    //undvec[i][j] = Math.Pow(points[i].X, Math.Abs(j - 3));
                    matrix[i, j] = Math.Pow(points[i].X, Math.Abs(j - 3));
                }
                //undvec[i][4] = points[i].Y;
                bvec[i] = points[i].Y;

            }
            
            LinearSystem a = new LinearSystem(undvec);
            return a.XVector;
            */
            return null;
        }

        private void LeastSquaresDraw(PaintEventArgs e)
        {
            double[] coeffs = LeastSquares();
            var spline = LeastSquares();
            //string test = $"y = {coeffs[0]}x3 + {coeffs[1]}x2 + {coeffs[2]}x + {coeffs[3]}";
            //textBox1.Text = test;

            Graphics g = e.Graphics;
            Brush brush = Brushes.Red;
            for (int i = mid.X; i < mid.X+500; i++)
            {
                double iy = coeffs[0] * i * i * i + coeffs[1] * i * i + coeffs[2] * i + coeffs[3];
                g.FillEllipse(brush, i, (int)iy, 5, 5);
            }
            /*
            listBox1.Items.Clear();
            foreach (Point p in points)
            {
                g.FillEllipse(Brushes.Yellow, p.X, (int)(coeffs[0] * p.X * p.X * p.X + coeffs[1] * p.X * p.X + coeffs[2] * p.X + coeffs[3]), 10, 10);
                listBox1.Items.Add(p.X + ";" + p.Y);
            }
            */
        }

        private int Lagrange(int x, int[] xValues, int[] yValues, int size = 5)
        {
            int lagrangePol = 0;

            for (int i = 0; i < size; i++)
            {
                int basicsPol = 1;
                for (int j = 0; j < size; j++)
                {
                    if (j != i)
                    {
                        basicsPol *= (x - xValues[j]) / (xValues[i] - xValues[j]);
                    }
                }
                lagrangePol += basicsPol * yValues[i];
            }

            return lagrangePol;
        }

        private void LagrangeDraw(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush brush = Brushes.Yellow;
            int[] xs = new int[5], ys = new int[5];
            int iter = 0;
            foreach(Point p in points)
            {
                xs[iter] = p.X;
                ys[iter] = p.Y;
            }
            for (int i = mid.X; i < mid.X + 500; i++)
                g.FillEllipse(brush, i, Lagrange(i, xs, ys), 5, 5);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            SquareCoord(e);
            DrawPoint(e);
            Graph(e);
            if (isFive)
            {
                LeastSquaresDraw(e);
                //LagrangeDraw(e);
            }
                
            //LagrangeDraw(e);
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if(points.Count == pointCount - 1)
                isFive = true;
            if (points.Count == pointCount)
            {
                (sender as Control).Invalidate();
                return;
            }
            points.Add(new Point(mid.X + (points.Count + 1) * 75, e.Y));
            (sender as Control).Invalidate();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            label1.Text = e.X.ToString();
            label2.Text = e.Y.ToString();

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
                    Text = $"{e.X};{e.Y};{dragp}";
                    dragging = true;
                    break;
                }
            }
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
    }
}
