using System;
using System.Collections;
using System.Data;

namespace Gauss {
    public class GaussSolutionNotFound : Exception {
        public GaussSolutionNotFound(string msg)
            : base("������� �� ����� ���� �������: \r\n" + msg) {
        }
    }

    public class LinearSystem {
        private double[,] initial_a_matrix;
        private double[,] a_matrix;  // ������� A
        private double[] x_vector;   // ������ ����������� x
        private double[] initial_b_vector;
        private double[] b_vector;   // ������ b
        private double[] u_vector;   // ������ ������� U
        private double eps;          // ������� �������� ��� ��������� ������������ ����� 
        private int size;            // ����������� ������


        public LinearSystem(double[,] a_matrix, double[] b_vector)
            : this(a_matrix, b_vector, 0.0001) {
        }
        public LinearSystem(double[,] a_matrix, double[] b_vector, double eps) {
            if (a_matrix == null || b_vector == null)
                throw new ArgumentNullException("���� �� ���������� ����� null.");

            int b_length = b_vector.Length;
            int a_length = a_matrix.Length;
            if (a_length != b_length * b_length)
                throw new ArgumentException(@"���������� ����� � �������� � ������� A ������ ��������� � ����������� ���������� � ������� B.");

            this.initial_a_matrix = a_matrix;  // ���������� �������� �������
            this.a_matrix = (double[,])a_matrix.Clone(); // � � ������ ����� ����������� ����������
            this.initial_b_vector = b_vector;  // ���������� �������� ������
            this.b_vector = (double[])b_vector.Clone();  // � ��� ������ ����� ����������� ����������
            this.x_vector = new double[b_length];
            this.u_vector = new double[b_length];
            this.size = b_length;
            this.eps = eps;

            GaussSolve();
        }

        public double[] XVector {
            get {
                return x_vector;
            }
        }

        public double[] UVector {
            get {
                return u_vector;
            }
        }

        // ������������� ������� �������� ��������
        private int[] InitIndex() {
            int[] index = new int[size];
            for (int i = 0; i < index.Length; ++i)
                index[i] = i;
            return index;
        }

        // ����� �������� �������� � �������
        private double FindR(int row, int[] index) {
            int max_index = row;
            double max = a_matrix[row, index[max_index]];
            double max_abs = Math.Abs(max);
            //if(row < size - 1)
            for (int cur_index = row + 1; cur_index < size; ++cur_index) {
                double cur = a_matrix[row, index[cur_index]];
                double cur_abs = Math.Abs(cur);
                if (cur_abs > max_abs) {
                    max_index = cur_index;
                    max = cur;
                    max_abs = cur_abs;
                }
            }

            if (max_abs < eps) {
                if (Math.Abs(b_vector[row]) > eps)
                    throw new GaussSolutionNotFound("������� ��������� �����������.");
                else
                    throw new GaussSolutionNotFound("������� ��������� ����� ��������� �������.");
            }

            // ������ ������� ������� ��������
            int temp = index[row];
            index[row] = index[max_index];
            index[max_index] = temp;

            return max;
        }

        // ���������� ������� ��� ������� ������
        private void GaussSolve() {
            int[] index = InitIndex();
            GaussForwardStroke(index);
            GaussBackwardStroke(index);
            GaussDiscrepancy();
        }

        // ������ ��� ������ ������
        private void GaussForwardStroke(int[] index) {
            // ������������ �� ������ ������ ������ ����
            for (int i = 0; i < size; ++i) {
                // 1) ����� �������� ��������
                double r = FindR(i, index);

                // 2) �������������� ������� ������ ������� A
                for (int j = 0; j < size; ++j)
                    a_matrix[i, j] /= r;

                // 3) �������������� i-�� �������� ������� b
                b_vector[i] /= r;

                // 4) ��������� ������� ������ �� ���� ����������������� �����
                for (int k = i + 1; k < size; ++k) {
                    double p = a_matrix[k, index[i]];
                    for (int j = i; j < size; ++j)
                        a_matrix[k, index[j]] -= a_matrix[i, index[j]] * p;
                    b_vector[k] -= b_vector[i] * p;
                    a_matrix[k, index[i]] = 0.0;
                }
            }
        }

        // �������� ��� ������ ������
        private void GaussBackwardStroke(int[] index) {
            // ������������ �� ������ ������ ����� �����
            for (int i = size - 1; i >= 0; --i) {
                // 1) ������� ��������� �������� �������� x
                double x_i = b_vector[i];

                // 2) ������������� ����� ��������
                for (int j = i + 1; j < size; ++j)
                    x_i -= x_vector[index[j]] * a_matrix[i, index[j]];
                x_vector[index[i]] = x_i;
            }
        }

        // ���������� ������� �������
        // U = b - x * A
        // x - ������� ���������, ���������� ������� ������
        private void GaussDiscrepancy() {
            for (int i = 0; i < size; ++i) {
                double actual_b_i = 0.0;   // ��������� ������������ i-������ 
                // �������� ������� �� ������ x
                for (int j = 0; j < size; ++j)
                    actual_b_i += initial_a_matrix[i, j] * x_vector[j];
                // i-� ������� ������� �������
                u_vector[i] = initial_b_vector[i] - actual_b_i;
            }
        }
    }
}
