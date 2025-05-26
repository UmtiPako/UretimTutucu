namespace UretimTutucu.Models
{
    public class FabrikaKayitlari
    {
        public List<OperasyonBildirimi> operasyonKayitlari { get; }
        public List<StandartDurus> standartDuruslar { get; }
        public List<OperasyonBildirimi> hesaplananKayitlar { get; }

        public FabrikaKayitlari()
        {
            operasyonKayitlari = new List<OperasyonBildirimi>
        {
            new OperasyonBildirimi(1,
                new DateTime(2020, 5, 23, 7, 30, 0),
                new DateTime(2020, 5, 23, 8, 30, 0),
                STATU.URETIM,
                null),
             new OperasyonBildirimi(2,
                new DateTime(2020, 5, 23, 8, 30, 0),
                new DateTime(2020, 5, 23, 12, 00, 0),
                STATU.URETIM,
                null),
            new OperasyonBildirimi(3,
                new DateTime(2020, 5, 23, 12, 00, 0),
                new DateTime(2020, 5, 23, 13, 0, 0),
                STATU.URETIM,
                null),
             new OperasyonBildirimi(4,
                new DateTime(2020, 5, 23, 13, 0, 0),
                new DateTime(2020, 5, 23, 13, 45, 0),
                STATU.DURUS,
                "ARIZA"),
            new OperasyonBildirimi(1,
                new DateTime(2020, 5, 23, 13, 45, 0),
                new DateTime(2020, 5, 23, 17, 30, 0),
                STATU.URETIM,
                null),
        };

            standartDuruslar = new List<StandartDurus>
            {
            new StandartDurus(new TimeOnly(10, 0), new TimeOnly(10, 15), "Çay Molası"),
            new StandartDurus(new TimeOnly(12, 0), new TimeOnly(12, 30), "Yemek Molası"),
            new StandartDurus(new TimeOnly(15, 0), new TimeOnly(15, 15), "Çay Molası")
            };

            hesaplananKayitlar = KayitlariHesapla();
        }

        private List<OperasyonBildirimi> KayitlariHesapla()
        {
            var hesaplananKayitlar = new List<OperasyonBildirimi>(); // Yeni tabloya koyulacak hesaplanan kayıtları koymak için listemizi oluşturuyoruz
            var durusAraliklari = new HashSet<(DateTime, DateTime)>(); // Duruş aralıklarını kontrol edeceğiz, kayıtlar üzerinden sorgulamak daha yorucu olacağından hashset oluşturuyoruz

            foreach (var kayit in operasyonKayitlari)
            {
                if (kayit.statu == STATU.DURUS && kayit.durusNedeni.Equals("ARIZA"))
                {
                    // Eğer şu anki kayıtta bi' arıza bulunmaktaysa, o arıza kaydı yeni tabloya direkt ekleniyor ve
                    // standart duruşları es geçiyor
                    hesaplananKayitlar.Add(kayit);
                    durusAraliklari.Add((kayit.operasyonBaslangic, kayit.operasyonBitis));
                    continue;
                }

                var ilgiliDuruslar = standartDuruslar
                    .Where(durus =>
                        kayit.operasyonBaslangic.TimeOfDay < durus.durusBitis.ToTimeSpan() &&
                        kayit.operasyonBitis.TimeOfDay > durus.durusBaslangic.ToTimeSpan())
                    .ToList(); // Bulunduğumuz kaydın zaman aralığına düşen standart duruşları filtreliyoruz


                foreach (var durus in ilgiliDuruslar)
                {
                    // Her bir filtrelenen duruş için (örn: Çay molası, yemek molası vs.) hesaplanan kayıtlara
                    // ekleme yapıyoruz. Aynı zamanda sonraki kontroller için duruş aralıklarımızı da set olarak ekliyoruz
                    var yeniHesaplananKayit = new OperasyonBildirimi(
                        new DateTime(kayit.operasyonBaslangic.Year, kayit.operasyonBaslangic.Month, kayit.operasyonBaslangic.Day,
                                     durus.durusBaslangic.Hour, durus.durusBaslangic.Minute, 0),
                        new DateTime(kayit.operasyonBaslangic.Year, kayit.operasyonBaslangic.Month, kayit.operasyonBaslangic.Day,
                                     durus.durusBitis.Hour, durus.durusBitis.Minute, 0),
                        STATU.DURUS,
                        durus.durusNedeni);
                    hesaplananKayitlar.Add(yeniHesaplananKayit);
                    durusAraliklari.Add((yeniHesaplananKayit.operasyonBaslangic, yeniHesaplananKayit.operasyonBitis));
                }

                // Bulunduğumuz kaydın son durumu tabloya eklenirkenki tüm zaman aralıklarını
                // kullanma adına zamanları ekliyoruz.
                var zamanlar = new List<DateTime>();
                zamanlar.Add(kayit.operasyonBaslangic);
                zamanlar.AddRange(ilgiliDuruslar.Select(durus => new DateTime(
                    kayit.operasyonBaslangic.Year, kayit.operasyonBaslangic.Month, kayit.operasyonBaslangic.Day,
                    durus.durusBaslangic.Hour, durus.durusBaslangic.Minute, 0)));
                zamanlar.AddRange(ilgiliDuruslar.Select(durus => new DateTime(
                    kayit.operasyonBaslangic.Year, kayit.operasyonBaslangic.Month, kayit.operasyonBaslangic.Day,
                    durus.durusBitis.Hour, durus.durusBitis.Minute, 0)));
                zamanlar.Add(kayit.operasyonBitis);

                zamanlar = zamanlar.OrderBy(t => t).ToList();

                for (int i = 0; i < zamanlar.Count - 1; i++)
                {
                    // Zaman aralığı duruş olarak eklenmediyse üretim olarak kayıtlara ekliyoruz
                    DateTime bas = zamanlar[i];
                    DateTime bit = zamanlar[i + 1];

                    if (!durusAraliklari.Contains((bas,bit)) && bas != bit)
                    {
                        hesaplananKayitlar.Add(new OperasyonBildirimi(bas, bit, STATU.URETIM, null));
                    }

                }
            }

            // kayıtları zamana göre sıralıyoruz
            hesaplananKayitlar = hesaplananKayitlar
            .OrderBy(r => r.operasyonBaslangic)
            .ToList();

            for (int i = 0; i < hesaplananKayitlar.Count; i++)
                hesaplananKayitlar[i].kayitNo = i + 1;

            return hesaplananKayitlar.ToList();
        }


    }
    }

