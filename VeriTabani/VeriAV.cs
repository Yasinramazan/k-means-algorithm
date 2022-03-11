using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VeriTabani
{
    public class VeriAV
    {
        
        private VeriProp props;
        public VeriAV()
        {
            kontrol();
        }

        private void kontrol()
        {
            if (!Directory.Exists("C:\\VeriOdevi"))
                Directory.CreateDirectory("C:\\VeriOdevi");
            if (!File.Exists("C:\\VeriOdevi\\sonuc.txt"))
                File.Create("C:\\VeriOdevi\\sonuc.txt");
            
        }

        private bool dosyaKontrol()
        {
            bool a = true; ;
            if (!File.Exists("C:\\Final-data.txt")) //Final-data'nin bu yolda olmasi gerekmektedir
                a = false;
            return a;
        }

        public VeriProp gelenVeri()
        {
            bool a = dosyaKontrol();
            string dosya;
            string[] veri;
            props = new VeriProp();
            props.sports = new List<int>();
            props.religious = new List<int>();
            props.theatre = new List<int>();
            props.picnic = new List<int>();
            props.nature = new List<int>();
            props.shopping = new List<int>();
            if (a) //a, false ise props nesnesi ici bos olarak geri donecek
            {
                StreamReader sr = new StreamReader("C:\\Final-data.txt");
                sr.ReadLine();
                while ((dosya = sr.ReadLine()) != null)
                {

                    veri = dosya.Split(",");
                    props.sports.Add(Convert.ToInt32(veri[0]));
                    props.religious.Add(Convert.ToInt32(veri[1]));
                    props.nature.Add(Convert.ToInt32(veri[2]));
                    props.theatre.Add(Convert.ToInt32(veri[3]));
                    props.shopping.Add(Convert.ToInt32(veri[4]));
                    props.picnic.Add(Convert.ToInt32(veri[5]));

                }

            }
           
            return props;
        }

        public void yazdir(List<List<float>> sonuc, int[] sayac)
        {
            StreamWriter sw = new StreamWriter("C:\\VeriOdevi\\sonuc.txt");
            int i = 0;
            string value;
            foreach (var item in sonuc)
            {
                value = "Veri " + i + ":    Küme:" + item[0];
                sw.WriteLine(value);
                i++;
            }
            i = 0;
            sw.WriteLine();
            foreach (var item in sayac)
            {
                value = "Küme "+i+":   "+item+" Kayıt";
                sw.WriteLine(value);
                i++;
            }
            sw.Close();
        }
    }
}
