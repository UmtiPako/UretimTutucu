namespace UretimTutucu.Models
{
    public class StandartDurus
    {
        public TimeOnly durusBaslangic { get; set; }
        public TimeOnly durusBitis { get; set; }
        public String durusNedeni { get; set; }

        public StandartDurus(TimeOnly durusBaslangic, 
            TimeOnly durusBitis, String durusNedeni)
        {
            this.durusBaslangic = durusBaslangic;
            this.durusBitis = durusBitis;
            this.durusNedeni = durusNedeni;
        }
    }
}
