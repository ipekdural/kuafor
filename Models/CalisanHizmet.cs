namespace kuafor.Models
{
    public class CalisanHizmet
    {
        public int CalisanId
        {
            get; set;
        } // FK: Çalışan
        public Calisan Calisan
        {
            get; set;
        }

        public int HizmetId
        {
            get; set;
        } // FK: Hizmet
        public Hizmet Hizmet
        {
            get; set;
        }
    }
}
