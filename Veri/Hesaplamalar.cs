using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeriTabani;
using OxyPlot;
using OxyPlot.Utilities;
using OxyPlot.Series;
using OxyPlot.Legends;
using OxyPlot.Axes;
using OxyPlot.Annotations;


namespace Veri
{
    public class Hesaplamalar
    {
        VeriAV veriT;
        private int k { get; set; }
        private int[] randDizisi { get; set; }  //ilk merkez atamasi icin rastgele indisleri tutan dizi
        private float[,] merkezler { get; set; } //merkez noktalarini tutan dizi
        private int[,] tumVeri { get; set; } //final_data dosyasinin icerigi
        private float[,] kumeler { get; set; }  //her bir iterasyonda verilerin merkezlere uzakliginin toplamini tutan dizi
        private int[] kumeSayaci { get; set; }  //her kumede kac eleman oldugunu tutan dizi
        private List<List<float>> kumelenmisVeriler { get; set; } // verileri kumelere gore tutan liste
        private int[,] yerler { get; set; } //her verinin indisine gore kumesini tutan dizi

        private int veriSayisi;
       
        public Hesaplamalar(int k)
        {
            veriT = new VeriAV();
            this.k = k;
            veriSayisi = 249;
        }

        private void rand()
        {
            randDizisi = new int[k];
            for (int i = 0; i < k; i++)
            {
                Random r = new Random();
                int rastgele = r.Next(0, veriSayisi);
                randDizisi[i] = rastgele;
            }
        }

        private void tumVeriyiDiziyeCevir(VeriProp veriler)
        {
            tumVeri = new int[veriSayisi, 6];
            for (int i = 0; i < veriSayisi; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (j == 0)
                        tumVeri[i, j] = veriler.sports[i];
                    else if (j == 1)
                        tumVeri[i, j] = veriler.religious[i];
                    else if (j == 2)
                        tumVeri[i, j] = veriler.nature[i];
                    else if (j == 3)
                        tumVeri[i, j] = veriler.theatre[i];
                    else if (j == 4)
                        tumVeri[i, j] = veriler.shopping[i];
                    else if (j == 5)
                        tumVeri[i, j] = veriler.picnic[i];
                }
            }
        }

        public void merkezDegeri(VeriProp veriler)
        {
            rand();
            tumVeriyiDiziyeCevir(veriler);
            merkezler = new float[k, 6];

            for (int i = 0; i < k; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    merkezler[i, j] = tumVeri[randDizisi[i], j];  //randDizisi elemanlarina gore merkez belirler
                }
            }
            uzaklikHesapla();
        }

        private void uzaklikHesapla()
        {
            double uzaklik;
            double toplam;
            int min;

            List<float> a;
            yerler = new int[249, 2];
            kumelenmisVeriler = new List<List<float>>();
            kumeSayaci = new int[k];
            float[] secilenVeri = new float[6];
            float[,] merkezlerKopyasi = new float[k, 6]; //iterasyonu bitirecek kontrol icin merkezler dizisinin kopyasini tutan dizi

            float[] sayac = new float[k];  //Her verinin her bir merkeze uzakligini tutan dizi
            kumeler = new float[k, 6];


            merkezlerKopyasi = (float[,])merkezler.Clone(); //merkezlerKopyasini merkezler dizisine esitliyoruz

            for (int i = 0; i < veriSayisi; i++)
            {
                a = new List<float>();
                for (int j = 0; j < k; j++)
                {
                    double[] secilenDizi = { merkezler[j, 0], merkezler[j, 1], merkezler[j, 2], merkezler[j, 3], merkezler[j, 4], merkezler[j, 5] };  //her merkezi tek bir yerde tutan dizi

                    //oklid
                    toplam = Math.Pow(secilenDizi[0] - tumVeri[i, 0], 2) + Math.Pow(secilenDizi[1] - tumVeri[i, 1], 2) + Math.Pow(secilenDizi[2] - tumVeri[i, 2], 2) +
                        Math.Pow(secilenDizi[3] - tumVeri[i, 3], 2) + Math.Pow(secilenDizi[4] - tumVeri[i, 4], 2) + Math.Pow(secilenDizi[5] - tumVeri[i, 5], 2);
                    uzaklik = Math.Sqrt(toplam);  
                    //oklid
                    sayac[j] = (float)uzaklik;  //her verinin her bir kumeye uzakligi sayac dizisine atiliyor

                }
                min = Array.IndexOf(sayac, sayac.Min());
                kumeSayaci[min]++; //kumeSayaci dizisinin her indisi bir kumeye esit
                a.Add(min);
                
                for (int j = 0; j < 6; j++)
                {
                    a.Add(tumVeri[i, j]);
                    kumeler[min, j] += tumVeri[i, j]; //kumelerdeki verilerin kumulatif toplamlari kumeler dizisinde birikiyor
                }
                kumelenmisVeriler.Add(a);

                yerler[i, 0] = i;
                yerler[i, 1] = min;
            }
                        
            yeniMerkezBul(merkezlerKopyasi);

        }

        private void yeniMerkezBul(float[,] merkezlerKopyasi)
        {
            int a=0;

            for (int i = 0; i < k; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    merkezler[i, j] = kumeler[i, j] / kumeSayaci[i]; //yeni merkez bulmak icin kumeler dizisinin elemanlari kumedeki eleman sayisina bolunuyor
                    if (merkezlerKopyasi[i, j] == merkezler[i, j])  //merkezlerKopyasi dizisinin her bir elemani
                        a++;                                        //yeni merkezler dizisi ile karsilastiriliyor
                }                                                   
            }
            if(a!=k*6) //yeni merkezlerle bir onceki merkezler esit olana kadar dongu devam ediyor
                uzaklikHesapla();
            else
            {
                WCSS();
            }
        }

        public void WCSS()
        {
            int index;
            float toplam = 0;
            for (int i = 0; i < veriSayisi; i++)
            {
                index = yerler[i, 1];
                
                for (int j = 0; j < 6; j++)
                {
                    toplam += (float)Math.Pow((tumVeri[i, j] - merkezler[index,j]),2);
                }
            }
        }

        public List<int[]> ikiBoyutlu(int x, int y)
        {
            List<int> index = new List<int>();
            int[] d = new int[3];
            List<int[]> eksenler = new List<int[]>();

            for (int i = 0; i < veriSayisi; i++)
            {
                d = new int[3];
                index = kumelenmisVeriler[i].ConvertAll(x => (int)x);
                d[0] = index[0];
                d[1] = index[x];
                d[2] = index[y];
                eksenler.Add(d);
            }
            return eksenler;
        }
        public void yazdir()
        {
            veriT.yazdir(kumelenmisVeriler,kumeSayaci);
        }
    }
}
