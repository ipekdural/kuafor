namespace kuafor.Models
{
    public class Calisan
    {
        public int CalisanId { get; set; } 
        public string Ad { get; set; }
        public string Soyad { get; set; }

        public int? UzmanlikHizmetId { get; set; }
        public Hizmet? UzmanlikHizmet { get; set; } 

        public ICollection<CalisanHizmet>? CalisanHizmetler { get; set; }
        public ICollection<Randevu>? Randevular
        { get;set;
        }
    }
}
