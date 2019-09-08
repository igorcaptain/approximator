using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gauss
{
    class LinearSystem
    {
        public double[] Xvec;
        private double[][] Matr;
        private double[] BVec;
        private double[,] Matr_in;
        private double[] BVec_in;
        private int n, m; //n - rows, m - cols
        private double Eps;

        public LinearSystem(double[][] matr, double eps)
        {
            n = matr.Length;
            m = matr[0].Length - 1;


            //Matr = matr; Matr_in = (double[,])Matr.Clone();
            //BVec = bvec; BVec_in = (double[])BVec.Clone();
            Matr = matr;
            /*
            Matr = new double[n][];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    Matr[i][j] = matr[i][j];
                    */
            
            Xvec = new double[m];
            Eps = eps;
            Solve();
        } 

        public LinearSystem(double[][] matr)
            : this(matr, 0.0001)
        {

        }

        public double[] XVector
        {
            get { return Xvec; }
        }

        private void Swap(ref double a, ref double b)
        {
            double temp = a;
            a = b;
            b = temp;
        }

        private double Solve()
        {
            int[] where = new int[m];
            for (int i = 0; i < where.Length; i++) where[i] = -1;
            for (int col = 0, row = 0; col < m && row < n; ++col)
            {
                int sel = row;
                for (int i = row; i < n; ++i)
                    if (Math.Abs(Matr[i][col]) > Math.Abs(Matr[sel][col]))
                        sel = i;
                if (Math.Abs(Matr[sel][col]) < Eps)
                    continue;
                for (int i = col; i <= m; ++i)
                    Swap(ref Matr[sel][i], ref Matr[row][i]);
                where[col] = row;

                for (int i = 0; i < n; ++i)
                    if (i != row)
                    {
                        double c = Matr[i][col] / Matr[row][col];
                        for (int j = col; j <= m; ++j)
                            Matr[i][j] -= Matr[row][j] * c;
                    }
                ++row;
            }

            //ans.assign(m, 0);
            for (int i = 0; i < Xvec.Length; i++)
                Xvec[i] = 0;
            for (int i = 0; i < m; ++i)
                if (where[i] != -1)
                    Xvec[i] = Matr[where[i]][m] / Matr[where[i]][i];
            for (int i = 0; i < n; ++i)
            {
                double sum = 0;
                for (int j = 0; j < m; ++j)
                    sum += Xvec[j] * Matr[i][j];
                if (Math.Abs(sum - Matr[i][m]) > Eps)
                    return 0;
            }

            for (int i = 0; i < m; ++i)
                if (where[i] == -1)
                    return double.PositiveInfinity;
            return 1;
        }
    }
}
