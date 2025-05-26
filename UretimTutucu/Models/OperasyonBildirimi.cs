using System.Data;

namespace UretimTutucu.Models
{
    public enum STATU { URETIM, DURUS }

    public class OperasyonBildirimi
    {
        public int kayitNo { get; set; }
        public DateTime operasyonBaslangic { get; set; }

        public DateTime operasyonBitis { get; set; }

        public TimeSpan toplamSure_saat { get; set; }
        public STATU statu { get; set; }

        public String? durusNedeni { get; set; }

        public OperasyonBildirimi(DateTime operasyonBaslangic, 
            DateTime operasyonBitis, STATU statu, String durusNedeni)
        {
            this.operasyonBaslangic = operasyonBaslangic;
            this.operasyonBitis = operasyonBitis;
            this.toplamSure_saat = operasyonBitis - operasyonBaslangic;
            this.statu = statu;
            this.durusNedeni = statu == STATU.URETIM ? " " : durusNedeni;
        }

        public OperasyonBildirimi(int kayitNo,DateTime operasyonBaslangic,
        DateTime operasyonBitis, STATU statu, String durusNedeni)
        {
            this.kayitNo = kayitNo;
            this.operasyonBaslangic = operasyonBaslangic;
            this.operasyonBitis = operasyonBitis;
            this.toplamSure_saat = operasyonBitis - operasyonBaslangic;
            this.statu = statu;
            this.durusNedeni = statu == STATU.URETIM ? " " : durusNedeni;
        }
    }
}
