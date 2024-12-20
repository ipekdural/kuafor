namespace kuafor.Models
{
    public class Randevu
    {
        public int RandevuId
        {
            get; set;
        } // PK
        public DateTime RandevuTarihi
        {
            get; set;
        } // Randevu tarihi
        public int? HizmetId
        {
            get; set;
        } // FK: Alınan hizmet
        public Hizmet Hizmet
        {
            get; set;
        }

        public int? CalisanId
        {
            get; set;
        } // FK: Hizmetin yapılacağı çalışan
        public Calisan Calisan
        {
            get; set;
        }

        public int? KullaniciId
        {
            get; set;
        } // FK: Randevuyu alan kullanıcı
        public Kullanici Kullanici
        {
            get; set;
        }

        public bool OnayliMi
        {
            get; set;
        } // Randevunun onaylanıp onaylanmadığını belirten flag
    }
}
