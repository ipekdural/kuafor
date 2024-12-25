namespace kuafor.Models
{
    public class Randevu
    {
        public int RandevuId
        {
            get; set;
        } 
        public DateTime RandevuTarihi
        {
            get; set;
        } 
        public int? HizmetId
        {
            get; set;
        } 
        public Hizmet Hizmet
        {
            get; set;
        }

        public int? CalisanId
        {
            get; set;
        } 
        public Calisan Calisan
        {
            get; set;
        }

        public int? KullaniciId
        {
            get; set;
        } 
        public Kullanici Kullanici
        {
            get; set;
        }
        public bool? OnayliMi
        {
            get; set;
        } 
        public bool SilindiMi { get; set; } = false;

    }
}
