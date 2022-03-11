using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VeriTabani;
using OxyPlot.WindowsForms;
using OxyPlot.Series;
using OxyPlot;


namespace Veri
{
    public partial class Form1 : Form
    {
        VeriAV veriT;
        VeriProp veriler;
        
        public Form1()
        {
            InitializeComponent();
            veriT = new VeriAV();
        }

        

        private void Form1_Load(object sender, EventArgs e)
        {
            veriler = veriT.gelenVeri();
            if (veriler.nature.Count == 0) //dosya yolu hatalari icin kontrol
            {
                MessageBox.Show("Final_data.txt'nin C:'de olduğundan emin olunuz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }

            comboX.DropDownStyle = ComboBoxStyle.DropDownList;
            comboX.Items.Add("sports");
            comboX.Items.Add("religious");
            comboX.Items.Add("nature");
            comboX.Items.Add("theatre");
            comboX.Items.Add("shopping");
            comboX.Items.Add("picnic");

            comboY.DropDownStyle = ComboBoxStyle.DropDownList;
            comboY.Items.Add("sports");
            comboY.Items.Add("religious");
            comboY.Items.Add("nature");
            comboY.Items.Add("theatre");
            comboY.Items.Add("shopping");
            comboY.Items.Add("picnic");
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(kDegeri.Text))
                MessageBox.Show("k değeri giriniz.", "Dikkat", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else if (Convert.ToInt32(kDegeri.Text) < 249 && Convert.ToInt32(kDegeri.Text)>1)
                groupBox1.Enabled = true;
            else
                MessageBox.Show("k, veri sayısından küçük, 1'den büyük olmalıdır", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int k = Convert.ToInt32(kDegeri.Text);
            int[] renkler = new int[k];
            int[] renkler2 = new int[k];
            int[] renkler3 = new int[k];
            List<int[]> eksenler = new List<int[]>(); //Secilen iki sutunun degerlerini ve ait olduklari kumeleri tutar
            Random r = new Random();
            
            Hesaplamalar hesap = new Hesaplamalar(Convert.ToInt32(kDegeri.Text));
            
            if (comboX.SelectedIndex == -1 || comboY.SelectedIndex == -1)
                MessageBox.Show("X ve Y eksenlerini seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                hesap.merkezDegeri(veriler); //k-means algoritmasi baslar
                hesap.yazdir();  //sonuc.txt'ye yazdirir
                eksenler = hesap.ikiBoyutlu(comboX.SelectedIndex + 1, comboY.SelectedIndex + 1); //Comboboxtan secilen sutunları dondurur
            }


            //Gorsellestirme 
            //renkler rastgele belirlendigi icin bazen cok yakin renkler gelebilir
            for (int i = 0; i < k; i++)
            {
                renkler[i] = r.Next(1, 255);
                renkler2[i] = r.Next(1, 255);
                renkler3[i] = r.Next(1, 255);
            }

            PlotView pw = new PlotView();
            pw.Location = new Point(200, 50);
            pw.Size = new Size(500, 300);
            groupBox1.Controls.Add(pw);

            pw.Model = new OxyPlot.PlotModel { Title = "Veri" };

            ScatterSeries sc = new ScatterSeries()
            {
                MarkerSize = 3f,
                MarkerType = MarkerType.Diamond,
            };

            foreach (var item in eksenler)
            {
                
                sc.Points.Add(new OxyPlot.Series.ScatterPoint(item[1], item[2]));
                sc.MarkerFill = OxyColor.FromRgb((byte)renkler[item[0]],(byte)renkler2[item[0]],(byte)renkler3[item[0]]);
                pw.Model.Series.Add(sc); 
                sc = new ScatterSeries()
                {
                    MarkerSize = 3f,
                    MarkerType = MarkerType.Diamond
                };
            }
            
        }
    }
}
