using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmerBurakAstar
{
    public class Durum
    {
        public int[,] Tahta { get; private set; }
        public int BosX { get; set; }
        public int BosY { get; set; }
        public int G { get; set; }
        public int H { get; set; }
        public int F => G + H;
        public Durum Onceki { get; set; }

        public Durum(int[,] tahta)
        {
            // Yeni bir 3x3 tahta oluştur
            Tahta = new int[3, 3];
            
            // Gelen tahtayı kopyala
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Tahta[i, j] = tahta[i, j];
                    // Boş hücrenin konumunu bul (0 değeri olan hücre)
                    if (tahta[i, j] == 0)
                    {
                        BosX = i;
                        BosY = j;
                    }
                }
            }
        }

        // Tahtanın derin kopyasını oluştur
        public Durum KopyaOlustur()
        {
            int[,] yeniTahta = new int[3, 3];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    yeniTahta[i, j] = Tahta[i, j];

            return new Durum(yeniTahta);
        }

        // İki durumun eşit olup olmadığını kontrol et
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Durum other = (Durum)obj;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (Tahta[i, j] != other.Tahta[i, j])
                        return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    hash = hash * 31 + Tahta[i, j];
                }
            }
            return hash;
        }

        public int ManhattanUzakligiHesapla(int[,] hedefTahta)
        {
            int uzaklik = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (Tahta[i, j] != 0)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            for (int l = 0; l < 3; l++)
                            {
                                if (hedefTahta[k, l] == Tahta[i, j])
                                {
                                    uzaklik += Math.Abs(i - k) + Math.Abs(j - l);
                                }
                            }
                        }
                    }
                }
            }
            return uzaklik;
        }

        public Durum TasHareketEt(int yeniX, int yeniY)
        {
            int[,] yeniTahta = new int[3, 3];
            Array.Copy(Tahta, yeniTahta, Tahta.Length);

            yeniTahta[BosX, BosY] = yeniTahta[yeniX, yeniY];
            yeniTahta[yeniX, yeniY] = 0;

            return new Durum(yeniTahta);
        }
    }
}
