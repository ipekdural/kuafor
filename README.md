Proje Adı: Kuaför & Güzellik Salonu Randevu Sistemi
Bu proje, ASP.NET MVC kullanılarak geliştirilen basit bir randevu yönetim sistemidir. Kullanıcılar; ad, hizmet türü, çalışandan hizmet alma tercihi, tarih ve saat gibi bilgileri girerek randevu oluşturabilir.
Kurulum
Projeyi İndirin veya Kopyalayın
![image](https://github.com/user-attachments/assets/f288dc9d-dc3b-4382-8ba0-7fde8a3c88bf)

Kaynak kodları GitHub’dan veya projenin paylaşıldığı konumdan bilgisayarınıza indirin.
Gerekli Bağımlılıkları Yükleyin

Projede .NET 6 veya üzeri (veya sizin kullandığınız sürüm) yüklü olmalıdır.
Gerekli NuGet paketlerini indirip kurmak için Visual Studio’daki Solution Explorer üzerinden projeye sağ tıklayarak “Restore NuGet Packages” seçeneğini kullanabilirsiniz.
Veritabanı Ayarları (Varsa)

Eğer bir veritabanı kullanıyorsanız, appsettings.json ya da ConnectionStrings ayarlarınızı güncelleyin. (Bu basit örnekte veritabanı yapısına ihtiyaç duyulmayabilir.)
Kullanım
Proje Dosyalarını Açın

Visual Studio (ya da Rider, VS Code gibi) IDE kullanıyorsanız, .sln dosyasına çift tıklayarak projeyi açabilirsiniz.
Uygulamayı Çalıştırın

Visual Studio içinde F5 veya Ctrl+F5 tuşlarına basarak projeyi başlatın.
Tarayıcınız açılınca projenin varsayılan sayfası yüklenecektir.
Randevu Oluşturma

Menüden veya ana sayfadan “Randevu Oluştur” sayfasına gidin (örneğin, /Appointment/Create rotası).
Formda:
Adınız kısmına kendi isminizi girin.
Hizmet Seçin listesinden bir hizmeti seçin (Saç Kesimi, Saç Boyama, vb.).
Hizmet Almak İstediğiniz Çalışan listesinden istediğiniz çalışanı seçin.
Tarih ve Saat alanında randevu zamanını seçin.
Randevu Al butonuna tıklayın.
Basit bir örnek olması adına, bu formun verileri varsayılan olarak veritabanına ya da bir listeye kaydedilebilir.
Gerçek proje senaryosunda, bu formun gönderimi AppointmentController (örnek) içerisinde yazılacak mantık ile randevu bilgilerini kaydedebilir.
