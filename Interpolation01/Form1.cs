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

namespace Interpolation01
{
    public partial class Form1 : Form
    {

        //массив цветов
        Color[] colors = new Color[]
        {
            Color.Teal, Color.Green, Color.Aqua, Color.Black,Color.BlueViolet,Color.LightSeaGreen
        };
        //счетчик кривых
        int counter = 0;
        //сохраняем линии-точки
        List<CurveItem> indexOfPoint = new List<CurveItem>();
        //сохраняем интерполяционные полиномы
        List<CurveItem> interLine = new List<CurveItem>();
        //точки, для отображения в дгв (dataGridViewer)
        BindingList<point> line = new BindingList<point>();

        public Form1()
        {
            InitializeComponent();

            zedGraphControl1.MouseClick += zedGraphControl1_MouseClick;

            #region dataGridViewer
            int width = dataGridView1.Width - dataGridView1.RowHeadersWidth - 2;
            dataGridView1.DataSource = line;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.Red;
            for (int x = 0; x < dataGridView1.Columns.Count; x++)
                dataGridView1.Columns[x].Width = width / 2;

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToOrderColumns = true;
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.RowHeadersWidthSizeMode =
                DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            #endregion;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            Draw();
        }

        void zedGraphControl1_MouseClick(object sender, MouseEventArgs e)
        {
            // Сюда будет сохранена кривая, рядом с которой был произведен клик
            CurveItem curve;
            // Номер точки кривой, ближайшей к точке клика
            int index;
            GraphPane pane = zedGraphControl1.GraphPane;
            // Максимальное расстояние от точки клика до кривой в пикселях, 
            // при котором еще считается, что клик попал в окрестность кривой.
            GraphPane.Default.NearestTol = 10;
            bool result = pane.FindNearestPoint(e.Location, out curve, out index);
            //if (result)
            //накладываем дополнительное условие, чтобы отмечать точки только на заданной функции
            if (result && pane.CurveList.IndexOf(curve) == 0)
            {
                // Максимально расстояние от точки клика до кривой не превысило NearestTol
                // Добавим точку на график, вблизи которой произошел клик
                PointPairList point = new PointPairList();
                point.Add(curve[index]);
                line.Add(new point(curve[index].X, curve[index].Y));
                // Кривая, состоящая из одной точки. Точка будет отмечена синим кругом
                LineItem curvePount = pane.AddCurve("",
                    new double[] { curve[index].X },
                    new double[] { curve[index].Y },
                    Color.Blue,
                    SymbolType.Circle);
                curvePount.Line.IsVisible = false;
                curvePount.Symbol.Fill.Color = Color.Red;
                curvePount.Symbol.Fill.Type = FillType.Solid;
                curvePount.Symbol.Size = 7;

                indexOfPoint.Add(curvePount);
            }

            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }

        private void Draw()
        {
            GraphPane pane = zedGraphControl1.GraphPane;
            PointPairList list = new PointPairList();
            double xmin = -Math.PI, xmax = Math.PI;
            //изменил точность, так график более точный и не будет проблем с "неотображениям" точек-кликов
            for (double x = xmin; x <= xmax; x += 0.01)
                list.Add(x, Math.Cos(x));

            LineItem myCurve = pane.AddCurve("Cos", list, Color.MediumVioletRed, SymbolType.None);
            myCurve.Line.IsAntiAlias = true;
            myCurve.Line.IsSmooth = true;

            zedGraphControl1.GraphPane.Title.Text = "Интерполяция полиномами Чебышева";
            zedGraphControl1.GraphPane.XAxis.Title.Text = "X";
            zedGraphControl1.GraphPane.YAxis.Title.Text = "Y";
            pane.XAxis.MajorGrid.IsVisible = true;
            pane.YAxis.MajorGrid.IsVisible = true;
            pane.Chart.Border.Color = Color.Red;
            pane.Fill.Type = FillType.Solid;
            pane.Fill.Color = Color.MediumSeaGreen;
            pane.Chart.Fill.Type = FillType.Solid;
            pane.Chart.Fill.Color = Color.WhiteSmoke;
            pane.XAxis.MajorGrid.IsZeroLine = true;
            pane.YAxis.MajorGrid.IsZeroLine = true;
            pane.XAxis.Color = Color.Black;
            pane.YAxis.Color = Color.Black;
            pane.XAxis.MajorGrid.IsVisible = true;
            pane.YAxis.MajorGrid.IsVisible = true;
            pane.XAxis.MajorGrid.Color = Color.Chocolate;
            pane.YAxis.MajorGrid.Color = Color.Chocolate;
            pane.XAxis.Title.FontSpec.FontColor = Color.Black;
            pane.YAxis.Title.FontSpec.FontColor = Color.Black;
            pane.XAxis.Scale.FontSpec.FontColor = Color.Black;
            pane.YAxis.Scale.FontSpec.FontColor = Color.Black;

            // Вызываем метод AxisChange (), чтобы обновить данные об осях.
            zedGraphControl1.AxisChange();
            // Обновляем график
            zedGraphControl1.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (line.Count > 0)
            {
                int index = dataGridView1.CurrentRow.Index;
                GraphPane pane = zedGraphControl1.GraphPane;
                //удаление точки 
                line.RemoveAt(index);
                pane.CurveList.Remove(indexOfPoint[index]);
                indexOfPoint.RemoveAt(index);

                zedGraphControl1.AxisChange();
                zedGraphControl1.Invalidate();
            }
        }

        private void zedGraphControl1_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //очищаем 
            GraphPane pane = zedGraphControl1.GraphPane;
            zedGraphControl1.GraphPane.CurveList.Clear();
            line.Clear();
            interLine.Clear();
            comboBox1.Items.Clear();
            indexOfPoint.Clear();
            Draw();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                int index = comboBox1.SelectedIndex;
                GraphPane pane = zedGraphControl1.GraphPane;
                //удаление интерполяционного полинома
                pane.CurveList.Remove(interLine[index]);
                interLine.RemoveAt(index);
                comboBox1.Items.RemoveAt(index);

                zedGraphControl1.AxisChange();
                zedGraphControl1.Invalidate();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (line.Count > 1)
            {
                GraphPane pane = zedGraphControl1.GraphPane;
                PointPairList list = new PointPairList();
                double xmin = -Math.PI, xmax = Math.PI;
                Interpolation one = new Interpolation(line.OrderBy(q => q.X).ToList());
                for (double x = xmin; x <= xmax + 0.01; x += 0.01)
                    list.Add(x, one.Ch(x));

                LineItem myCurve = pane.AddCurve("Interpolation(" + (line.Count).ToString() + ")", list, colors[(counter++) % colors.Length], SymbolType.None);
                //"сглаживаем" отображение
                interLine.Add(myCurve);
                comboBox1.Items.Add("Interpolation(" + (line.Count).ToString() + ")");
                myCurve.Line.IsAntiAlias = true;
                myCurve.Line.IsSmooth = true;
                pane.XAxis.MajorGrid.IsVisible = true;
                pane.YAxis.MajorGrid.IsVisible = true;
                // Вызываем метод AxisChange (), чтобы обновить данные об осях.
                zedGraphControl1.AxisChange();
                // Обновляем график
                zedGraphControl1.Invalidate();
            }
        }


    }

    public class point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public point(double x, double y)
        {
            X = x;
            Y = y;
        }
        public point()
            : this(0, 0)
        {
        }
    }

    public class Interpolation
    {
        public List<point> line { get; private set; }
        public Interpolation(List<point> a)
        {
            line = a;
        }

        public double Ch(double y)
        {
            List<double[]> tb = new List<double[]>();

            List<double> a = new List<double>() { 0, line.Average(w => w.X) };
            List<double> b = new List<double>() { 0, 0 };
            List<double> c = new List<double>() { };
            Func<double, int, double> temp = null;
            temp = new Func<double, int, double>((z, ind) =>
            {
                if (ind == 0)
                    return 1;
                if (ind == 1)
                    return z - a[ind];
                return (z - a[ind]) * temp(z, ind - 1) - b[ind] * temp(z, ind - 2);
            });

            c.Add(line.Sum(f => f.Y * temp(f.X, 0))
                / line.Sum(f => temp(f.X, 0) * temp(f.X, 0)));

            c.Add(line.Sum(f => f.Y * temp(f.X, 1))
                 / line.Sum(f => temp(f.X, 1) * temp(f.X, 1)));

            for (int k = 2; k < line.Count; k++)
            {
                a.Add(line.Sum(f => f.X * temp(f.X, k - 1) * temp(f.X, k - 1))
                        / line.Sum(h => temp(h.X, k - 1) * temp(h.X, k - 1)));
                b.Add(line.Sum(f => f.X * temp(f.X, k - 1) * temp(f.X, k - 2)) /
                        line.Sum(g => temp(g.X, k - 2) * temp(g.X, k - 2)));
                c.Add(line.Sum(f => f.Y * temp(f.X, k))
                         / line.Sum(f => temp(f.X, k) * temp(f.X, k)));
                tb.Add(line.Select(s => temp(s.X, k)).ToArray());
            }

            double sum = 0;
            for (int i = 0; i < line.Count; i++)
                sum += c[i] * temp(y, i);
            return sum;
        }
    }
}
