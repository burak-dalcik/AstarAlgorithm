using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OmerBurakAstar
{
    public partial class Form1 : Form
    {
        private Button[,] butonlar;
        private int[,] baslangicDurum;
        private int[,] hedefDurum;
        private List<Durum> cozumYolu;
        private int adimSayisi = 0;
        private Label adimLabel;
        private Label durumLabel;
        private TextBox[,] baslangicKutuları;
        private TextBox[,] hedefKutuları;
        private Button baslatButton;
        private Button temizleButton;
        private HashSet<int> kullanılanSayilarBaslangic;
        private HashSet<int> kullanılanSayilarHedef;
        private Label[,] baslangicLabels;
        private Label[,] hedefLabels;

        public Form1()
        {
            InitializeComponent();
            this.Size = new Size(300, 300);
            this.Text = "8-Taş Bulmacası - A* Algoritması";
            kullanılanSayilarBaslangic = new HashSet<int>();
            kullanılanSayilarHedef = new HashSet<int>();
            KutuOlustur();
        }

        private void KutuOlustur()
        {
            // Başlangıç durumu için kutular
            baslangicKutuları = new TextBox[3, 3];
            Label baslangicLabel = new Label
            {
                Text = "Başlangıç Durumu:",
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(baslangicLabel);

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    baslangicKutuları[i, j] = new TextBox
                    {
                        Location = new Point(20 + (j * 30), 50 + (i * 30)),
                        Size = new Size(25, 25),
                        MaxLength = 1
                    };
                    this.Controls.Add(baslangicKutuları[i, j]);
                }
            }

            // Hedef durumu için kutular
            hedefKutuları = new TextBox[3, 3];
            Label hedefLabel = new Label
            {
                Text = "Hedef Durumu:",
                Location = new Point(150, 20),
                AutoSize = true
            };
            this.Controls.Add(hedefLabel);

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    hedefKutuları[i, j] = new TextBox
                    {
                        Location = new Point(150 + (j * 30), 50 + (i * 30)),
                        Size = new Size(25, 25),
                        MaxLength = 1
                    };
                    this.Controls.Add(hedefKutuları[i, j]);
                }
            }

            // Çöz butonu
            baslatButton = new Button
            {
                Text = "Çöz",
                Location = new Point(20, 150),
                Size = new Size(80, 30),
                BackColor = Color.LightGreen
            };
            baslatButton.Click += BaslatButton_Click;
            this.Controls.Add(baslatButton);

            // Temizle butonu - Çöz butonunun yanına
            temizleButton = new Button
            {
                Text = "Temizle",
                Location = new Point(110, 150), // Çöz butonunun hemen yanında
                Size = new Size(80, 30),
                BackColor = Color.LightBlue
            };
            temizleButton.Click += TemizleButton_Click;
            this.Controls.Add(temizleButton);

            // Adım Label
            adimLabel = new Label
            {
                Text = "Adım: 0",
                Location = new Point(20, 190),
                AutoSize = true
            };
            this.Controls.Add(adimLabel);

            // Durum Label
            durumLabel = new Label
            {
                Text = "Durum: Hazır",
                Location = new Point(20, 210),
                AutoSize = true
            };
            this.Controls.Add(durumLabel);
        }

        private void BaslatButton_Click(object sender, EventArgs e)
        {
            try
            {
                int[,] baslangicTahta = new int[3, 3];
                int[,] hedefTahta = new int[3, 3];

                // Başlangıç durumunu oku
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        string deger = baslangicKutuları[i, j].Text.Trim();
                        baslangicTahta[i, j] = string.IsNullOrEmpty(deger) ? 0 : int.Parse(deger);
                    }
                }

                // Hedef durumunu oku
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        string deger = hedefKutuları[i, j].Text.Trim();
                        hedefTahta[i, j] = string.IsNullOrEmpty(deger) ? 0 : int.Parse(deger);
                    }
                }

                // Durum nesnelerini oluştur
                Durum baslangicDurum = new Durum(baslangicTahta);
                Durum hedefDurum = new Durum(hedefTahta);

                // A* algoritmasını başlat
                AStar astar = new AStar();
                astar.Coz(baslangicDurum, hedefDurum);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TemizleButton_Click(object sender, EventArgs e)
        {
            // Tüm kutuları temizle
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    baslangicKutuları[i, j].Text = "";
                    hedefKutuları[i, j].Text = "";
                }
            }

            // Adım ve durum labellarını sıfırla
            adimLabel.Text = "Adım: 0";
            durumLabel.Text = "Durum: Hazır";

            // Kullanılan sayıları temizle
            if (kullanılanSayilarBaslangic != null)
                kullanılanSayilarBaslangic.Clear();
            if (kullanılanSayilarHedef != null)
                kullanılanSayilarHedef.Clear();

            // İlk kutuya focus
            baslangicKutuları[0, 0].Focus();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (adimSayisi < cozumYolu.Count)
            {
                TahtayiGuncelle(cozumYolu[adimSayisi].Tahta);
                adimLabel.Text = $"Adım: {adimSayisi + 1}/{cozumYolu.Count}";
                adimSayisi++;
            }
            else
            {
                ((Timer)sender).Stop();
                adimSayisi = 0;
                durumLabel.Text = "Durum: Tamamlandı!";
            }
        }

        private void TahtayiGuncelle(int[,] tahta)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    butonlar[i, j].Text = tahta[i, j] == 0 ? "" : tahta[i, j].ToString();
                    butonlar[i, j].BackColor = tahta[i, j] == 0 ? Color.LightGray : Color.White;
                }
            }
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Sadece rakamları ve backspace'i kabul et
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
                return;
            }

            // 0 girişini engelle
            if (e.KeyChar == '0')
            {
                e.Handled = true;
                return;
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string gridType = textBox.Tag.ToString();
            HashSet<int> kullanılanSayilar = gridType == "baslangic" ? kullanılanSayilarBaslangic : kullanılanSayilarHedef;

            if (string.IsNullOrEmpty(textBox.Text))
                return;

            int sayi;
            if (int.TryParse(textBox.Text, out sayi))
            {
                if (sayi < 1 || sayi > 8)
                {
                    MessageBox.Show("Lütfen 1-8 arası bir sayı girin.", "Uyarı");
                    textBox.Text = "";
                    return;
                }

                // Eğer sayı daha önce kullanıldıysa
                if (kullanılanSayilar.Contains(sayi))
                {
                    MessageBox.Show($"{sayi} sayısı zaten kullanılmış.", "Uyarı");
                    textBox.Text = "";
                    return;
                }

                // Önceki değeri kaldır (eğer varsa)
                if (!string.IsNullOrEmpty(textBox.Text))
                {
                    int eskiSayi;
                    if (int.TryParse(textBox.Text, out eskiSayi))
                    {
                        kullanılanSayilar.Remove(eskiSayi);
                    }
                }

                // Yeni sayıyı ekle
                kullanılanSayilar.Add(sayi);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
