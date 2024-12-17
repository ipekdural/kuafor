namespace kuafor.Models
{
    public class Calisan
    {
        public int CalisanId
        {
            get; set;
        } // PK
        public string Ad
        {
            get; set;
        }
        public string Soyad
        {
            get; set;
        }
        public string UzmanlikAlanlari
        {
            get; set;
        } // Çalışanın uzmanlık alanları

        // Çalışanın sunduğu hizmetlerle ilişkili FK
        public ICollection<CalisanHizmet> CalisanHizmetler
        {
            get; set;
        }
    }
}
