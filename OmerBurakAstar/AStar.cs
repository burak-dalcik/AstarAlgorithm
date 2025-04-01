using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OmerBurakAstar
{
    public class AStar
    {
        private Durum baslangicDurum;
        private Durum hedefDurum;
        private List<DurumMaliyet> acikListe;
        private HashSet<string> kapaliListe;

        public AStar()
        {
            acikListe = new List<DurumMaliyet>();
            kapaliListe = new HashSet<string>();
        }

       
        private class DurumMaliyet
        {
            public Durum Durum { get; set; }
            public int Maliyet { get; set; }

            public DurumMaliyet(Durum durum, int maliyet)
            {
                Durum = durum;
                Maliyet = maliyet;
            }
        }

        public void Coz(Durum baslangic, Durum hedef)
        {
            this.baslangicDurum = baslangic;
            this.hedefDurum = hedef;
            acikListe.Clear();
            kapaliListe.Clear();

            acikListe.Add(new DurumMaliyet(baslangicDurum, HesaplaF(baslangicDurum)));

            while (acikListe.Count > 0)
            {
                int enDusukIndex = 0;
                for (int i = 1; i < acikListe.Count; i++)
                {
                    if (acikListe[i].Maliyet < acikListe[enDusukIndex].Maliyet)
                        enDusukIndex = i;
                }

                Durum mevcutDurum = acikListe[enDusukIndex].Durum;
                acikListe.RemoveAt(enDusukIndex);

                if (DurumlarEsit(mevcutDurum, hedefDurum))
                {
                    CozumYolunuGoster(mevcutDurum);
                    return;
                }

                string durumStr = DurumToString(mevcutDurum);
                if (kapaliListe.Contains(durumStr))
                    continue;

                kapaliListe.Add(durumStr);

                foreach (var yeniDurum in UretHamleler(mevcutDurum))
                {
                    string yeniDurumStr = DurumToString(yeniDurum);
                    if (!kapaliListe.Contains(yeniDurumStr))
                    {
                        yeniDurum.Onceki = mevcutDurum;
                        acikListe.Add(new DurumMaliyet(yeniDurum, HesaplaF(yeniDurum)));
                    }
                }
            }

            MessageBox.Show("Çözüm bulunamadı!");
        }

        private int HesaplaF(Durum durum)
        {
            return HesaplaG(durum) + HesaplaH(durum);
        }

        private int HesaplaG(Durum durum)
        {
            // G değeri: Başlangıçtan mevcut duruma kadar olan adım sayısı
            int adimSayisi = 0;
            Durum temp = durum;
            while (temp.Onceki != null)
            {
                adimSayisi++;
                temp = temp.Onceki;
            }
            return adimSayisi;
        }

        private int HesaplaH(Durum durum)
        {
            // H değeri: Manhattan mesafesi
            int mesafe = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (durum.Tahta[i, j] != 0)
                    {
                        // Hedef konumu bul
                        FindHedefKonum(durum.Tahta[i, j], out int hedefI, out int hedefJ);
                        mesafe += Math.Abs(i - hedefI) + Math.Abs(j - hedefJ);
                    }
                }
            }
            return mesafe;
        }

        private void FindHedefKonum(int sayi, out int i, out int j)
        {
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (hedefDurum.Tahta[row, col] == sayi)
                    {
                        i = row;
                        j = col;
                        return;
                    }
                }
            }
            i = j = -1;
        }

        private List<Durum> UretHamleler(Durum durum)
        {
            List<Durum> hamleler = new List<Durum>();
            int[] dx = { -1, 1, 0, 0 }; // Yukarı, Aşağı
            int[] dy = { 0, 0, -1, 1 }; // Sol, Sağ

            for (int k = 0; k < 4; k++)
            {
                int yeniX = durum.BosX + dx[k];
                int yeniY = durum.BosY + dy[k];

                if (yeniX >= 0 && yeniX < 3 && yeniY >= 0 && yeniY < 3)
                {
                    Durum yeniDurum = durum.KopyaOlustur();
                    
                    // Taşları değiştir
                    int temp = yeniDurum.Tahta[yeniX, yeniY];
                    yeniDurum.Tahta[yeniX, yeniY] = 0;
                    yeniDurum.Tahta[durum.BosX, durum.BosY] = temp;
                    
                    // Yeni boş hücre konumunu güncelle
                    yeniDurum.BosX = yeniX;
                    yeniDurum.BosY = yeniY;
                    
                    hamleler.Add(yeniDurum);
                }
            }

            return hamleler;
        }

        private string DurumToString(Durum durum)
        {
            string sonuc = "";
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    sonuc += durum.Tahta[i, j].ToString();
            return sonuc;
        }

        private bool DurumlarEsit(Durum durum1, Durum durum2)
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (durum1.Tahta[i, j] != durum2.Tahta[i, j])
                        return false;
            return true;
        }

        /*
        private void CozumYolunuGoster(Durum sonDurum)
        {
            List<Durum> cozumYolu = new List<Durum>();
            Durum temp = sonDurum;
            while (temp != null)
            {
                cozumYolu.Add(temp);
                temp = temp.Onceki;
            }
            cozumYolu.Reverse();

            // Çözüm yolunu göster
            string mesaj = $"Çözüm {cozumYolu.Count - 1} adımda bulundu!\n";
            for (int i = 0; i < cozumYolu.Count; i++)
            {
                mesaj += $"\nAdım {i}:\n";
                mesaj += DurumuYazdir(cozumYolu[i]);
            }
            MessageBox.Show(mesaj);
        }
        */
        private void CozumYolunuGoster(Durum sonDurum)
        {
            List<Durum> cozumYolu = new List<Durum>();
            Durum temp = sonDurum;
            while (temp != null)
            {
                cozumYolu.Add(temp);
                temp = temp.Onceki;
            }
            cozumYolu.Reverse();

            StringBuilder mesaj = new StringBuilder();
            mesaj.AppendLine($"Çözüm {cozumYolu.Count - 1} adımda bulundu!\n");

            // Her adımı 3x3 matris formatında göster
            for (int i = 0; i < cozumYolu.Count; i++)
            {
                mesaj.AppendLine($"Adım {i}:");
                for (int row = 0; row < 3; row++)
                {
                    for (int col = 0; col < 3; col++)
                    {
                        int sayi = cozumYolu[i].Tahta[row, col];
                        mesaj.Append(sayi == 0 ? "_" : sayi.ToString());
                        mesaj.Append(" ");
                    }
                    mesaj.AppendLine();
                }
                mesaj.AppendLine();
            }

            // Toplam maliyet hesabı
            int toplamHamle = cozumYolu.Count - 1;
            int toplamMaliyet = toplamHamle * 50;
            mesaj.AppendLine($"Toplam hamle sayısı: {toplamHamle}");
            mesaj.AppendLine($"Toplam maliyet: {toplamMaliyet} birim (Her hamle 50 birim)");

            using (Form form = new Form())
            {
                form.Text = "Çözüm Adımları ve Maliyet";
                form.Size = new Size(400, 600);
                TextBox textBox = new TextBox
                {
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    Size = new Size(350, 500),
                    Location = new Point(10, 10),
                    ReadOnly = true,
                    Text = mesaj.ToString(),
                    Font = new Font("Consolas", 12),
                    WordWrap = false
                };
                Button okButton = new Button
                {
                    Text = "Tamam",
                    DialogResult = DialogResult.OK,
                    Location = new Point(150, 520)
                };
                form.Controls.Add(textBox);
                form.Controls.Add(okButton);
                form.ShowDialog();
            }
        }

    }
}
