namespace kuafor.Models
{
    public class Calisan
    {
        public int CalisanId { get; set; } // PK
        public string Ad { get; set; }
        public string Soyad { get; set; }

        // Uzmanlık Alanı, sunduğu hizmetlerden biri olmalı
        public int? UzmanlikHizmetId { get; set; }
        public Hizmet? UzmanlikHizmet { get; set; } // Navigation property

        // Çalışanın sunduğu hizmetlerle ilişkili FK
        public ICollection<CalisanHizmet>? CalisanHizmetler { get; set; }
        public ICollection<Randevu>? Randevular
        { get;set;
        }
    }
}
