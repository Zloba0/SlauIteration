using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SlauIteration
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = $"4 2 1 5{Environment.NewLine}1 -5 2 4{Environment.NewLine}2 1 6 -2";
            textBox2.Text = "3";
            textBox5.Text = "1";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
            textBox4.Text = "";
            int n = 0;
            try
            {
                n = Convert.ToInt32(textBox2.Text);
            }
            catch
            {
                MessageBox.Show("Input dimension", "Worning");
            }
            double eps = 0;
            try
            {
                eps = Convert.ToDouble(textBox5.Text);
            }
            catch
            {
                MessageBox.Show("Input rejection", "Worning");
            }
            double[,] matr = new double[n, n+1];
            try
            {
                string[] line;
                line = textBox1.Lines;
                for (int i = 0; i < n; i++)
                {
                    int j = 0;
                    string subLine = "";
                    for (int g = 0; g < line[i].Length; g++)
                    {
                        if (line[i][g] != ' ')
                        {
                            subLine += line[i][g];
                        }
                        else
                        {
                            matr[i, j] = Convert.ToDouble(subLine);
                            j++;
                            subLine = "";
                        }
                    }
                    matr[i, j] = Convert.ToDouble(subLine);
                    j++;
                    if (j != n+1)
                    {
                        throw new Exception();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Wrong input of extended matrix", "Worning");
            }
            double[,] matrClone = new double[n, n+1];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n+1; j++)
                {
                    matrClone[i, j] = matr[i, j];
                }
            }
            if (!IsConvergent(matr, n))
            {
                MessageBox.Show("Matrix is not convergent", "Worning");
                return;
            }
            double[] xi = Jacoby(matr, n, eps);
            for (int i = 0; i < n; i++)
            {
                textBox3.Text += $"x{i+1} = {xi[i]}";
                textBox3.AppendText(Environment.NewLine);
            }
        }
        public double[] Zeydel(double[,] matr, int n, double eps)
        {
            //double[,] b = CreateB(matr, n);
            double[] f = CreateF(matr, n);
            double[] xFirst = new double[n];
            double[] xNext = new double[n];
            bool flag;
            int IterationNumber = 0;
            GraphPane pane = zedGraph.GraphPane;
            pane.CurveList.Clear();

            PointPairList list = new PointPairList();
            do
            {
                xNext.CopyTo(xFirst, 0);
                for (int i = 0; i < n; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < n; j++)
                    {
                        if (j == i)
                        {
                            continue;
                        }
                        sum += -matr[i, j] * xNext[i];
                    }
                    xNext[i] = (f[i] + sum) / matr[i, i];
                }
                double tmp = Norma(VecSub(xNext, xFirst));
                flag = tmp < eps;
                list.Add(Convert.ToDouble(IterationNumber), Norma(R(matr, xNext, n)));
                IterationNumber++;
            } while (!flag);
            LineItem myCurve = pane.AddCurve(" ", list, Color.Blue, SymbolType.None);
            zedGraph.AxisChange();
            zedGraph.Invalidate();
            double[] r = R(matr, xNext, n);
            for (int i = 0; i < r.Length; i++)
            {
                textBox4.Text += $"r{i+1} = {r[i]}";
                textBox4.AppendText(Environment.NewLine);
            }
            return xNext;
        }
        public double[] Jacoby(double[,] matr, int n, double eps)
        {
            //double[,] b = CreateB(matr, n);
            double[] f = CreateF(matr, n);
            double[] xFirst = new double[n];
            double[] xNext = new double[n];
            bool flag;
            int IterationNumber = 0;
            GraphPane pane = zedGraph.GraphPane;
            pane.CurveList.Clear();

            PointPairList list = new PointPairList();
            do
            {
                xNext.CopyTo(xFirst, 0);
                for (int i = 0; i < n; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < n; j++)
                    {
                        if(j == i)
                        {
                            continue;
                        }
                        sum += -matr[i,j] * xFirst[i];
                    }
                    xNext[i] = (f[i] + sum) / matr[i,i];
                }
                double tmp = Norma(VecSub(xNext, xFirst));
                flag = tmp < eps;
                list.Add(Convert.ToDouble(IterationNumber), Norma(R(matr, xNext, n)));
                IterationNumber++;
            } while (!flag);
            LineItem myCurve = pane.AddCurve(" ", list, Color.Blue, SymbolType.None);
            zedGraph.AxisChange();
            zedGraph.Invalidate();
            double[] r = R(matr, xNext, n);
            for(int i = 0; i < r.Length; i++)
            {
                textBox4.Text += $"r{i+1} = {r[i]}";
                textBox4.AppendText(Environment.NewLine);
            }
            return xNext;
        }
        public double[] CreateF(double[,] matr, int n)
        {
            double[] f = new double[n];
            for (int i = 0; i < n; i++)
            {
                f[i] = matr[i, n];
            }
            return f;
        }
        //public double[,] CreateB(double[,] matr, int n)
        //{
        //    double[,] b = new double[n, n];
        //    for (int i = 0; i < n; i++)
        //    {
        //        for (int j = 0; j < n; j++)
        //        {
        //            if (i == j)
        //            {
        //                b[i, j] = matr[i, j] + 1;
        //            }
        //            else
        //            {
        //                b[i, j] = matr[i, j];
        //            }
        //        }
        //    }
        //    return b;
        //}
        public bool IsConvergent(double[,] matr, int n)
        {
            for (int i = 0; i < n; i++)
            {
                double sum = 0;
                for (int j = 0; j < n; j++)
                {
                    if (j == i)
                    {
                        continue;
                    }
                    sum += Math.Abs(matr[i, j]);
                }
                if (Math.Abs(matr[i, i]) <= sum)
                {
                    return false;
                }
            }
            return true;

        }
        public double Norma(double[] x)
        {
            double res = 0;
            for (int i = 0; i < x.Length; i++)
            {
                res += x[i] * x[i];
            }
            return Math.Sqrt(res);
        }
        public double[] VecSub(double[] x, double[] y)
        {
            double[] vec = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                vec[i] = x[i] - y[i];
            }
            return vec;
        }
        public double[] R(double[,] matr, double[] xi, int n)
        {
            double[] res = new double[n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    res[i] += matr[i, j]*xi[j];
                }
                res[i] -= matr[i, n];
            }
            return res;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
            textBox4.Text = "";
            int n = 0;
            try
            {
                n = Convert.ToInt32(textBox2.Text);
            }
            catch
            {
                MessageBox.Show("Input dimension", "Worning");
            }
            double eps = 0;
            try
            {
                eps = Convert.ToDouble(textBox5.Text);
            }
            catch
            {
                MessageBox.Show("Input rejection", "Worning");
            }
            double[,] matr = new double[n, n+1];
            try
            {
                string[] line;
                line = textBox1.Lines;
                for (int i = 0; i < n; i++)
                {
                    int j = 0;
                    string subLine = "";
                    for (int g = 0; g < line[i].Length; g++)
                    {
                        if (line[i][g] != ' ')
                        {
                            subLine += line[i][g];
                        }
                        else
                        {
                            matr[i, j] = Convert.ToDouble(subLine);
                            j++;
                            subLine = "";
                        }
                    }
                    matr[i, j] = Convert.ToDouble(subLine);
                    j++;
                    if (j != n+1)
                    {
                        throw new Exception();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Wrong input of extended matrix", "Worning");
            }
            double[,] matrClone = new double[n, n+1];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n+1; j++)
                {
                    matrClone[i, j] = matr[i, j];
                }
            }
            if (!IsConvergent(matr, n))
            {
                MessageBox.Show("Matrix is not convergent", "Worning");
                return;
            }
            double[] xi = Jacoby(matr, n, eps);
            for (int i = 0; i < n; i++)
            {
                textBox3.Text += $"x{i+1} = {xi[i]}";
                textBox3.AppendText(Environment.NewLine);
            }
        }
    }
}
