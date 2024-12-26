namespace kuafor.Models
{
    public class Hizmet
    {
        public int HizmetId
        {
            get; set;
        } // PK
        public string HizmetAdi
        {
            get; set;
        } // Hizmet adı (örneğin, saç kesimi)
        public TimeSpan Sure
        {
            get; set;
        } // Hizmet süresi
        public decimal Ucret
        {
            get; set;
        } // Hizmet ücreti
    }
}
