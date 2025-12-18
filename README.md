# ECommerce API

Kısa açıklama
- .NET 8 ile geliştirilmiş, Entity Framework Core ile katmanlı mimari(CQRS) e‑ticaret API projesi.
- Katmanlar: `App.API` (web), `App.Infrastructure` (persistence), `App.Application`, `App.Domain`.

Önkoşullar
- .NET 8 SDK
- Docker Desktop (lokal container testi için, production için yönetilen DB (AZURE) önerilir)
- (Opsiyonel) `dotnet-ef` tool: migrations için

Hızlı başlat — yerel
1. Kodu çekin ve restore:
   - `dotnet restore`
2. EF araçları (yüklü değilse):
   - `dotnet tool install --global dotnet-ef`
3. Geliştirme config dosyalarını kontrol edin:
   - `App.API/appsettings.Development.json` geçerli JSON olmalı.
4. Migration oluşturma ve uygulama (çözüm kökünden):
   - `dotnet ef migrations add <Name> -p ./App.Infrastructure/App.Infrastructure.csproj -s ./App.API/App.API.csproj -o Migrations`
   - `dotnet ef database update -p ./App.Infrastructure/App.Infrastructure.csproj -s ./App.API/App.API.csproj`

Docker ile yerel (SQL Server container)
1. Kök dizinde `docker-compose.yml` ve `App.API/Dockerfile` bulunmalıdır.
2. Başlat:
   - `docker compose up --build`
3. Alternatif: manuel build/run
   - `docker build -t app-api -f App.API/Dockerfile .`
   - `docker run --rm -p 5106:80 -e "ConnectionStrings__DefaultConnection=Server=host.docker.internal,1433;Database=AppDb;User Id=sa;Password=Your_password123!;TrustServerCertificate=True;" app-api`

Migration çakışmaları (common)
- Hata: `There is already an object named 'Categories'`  
  Sebep: Veritabanında tablo var ama `__EFMigrationsHistory` kayıtlı değil; EF aynı tabloyu oluşturmaya çalışıyor.
  Çözümler:
  - Geliştirme ortamında veri kaybı uygunsa DB sil ve yeniden oluştur:
    - `dotnet ef database drop -p ./App.Infrastructure/App.Infrastructure.csproj -s ./App.API/App.API.csproj --force`
    - `dotnet ef database update -p ./App.Infrastructure/App.Infrastructure.csproj -s ./App.API/App.API.csproj`
  - Veri korumak için baseline migration:
    - `dotnet ef migrations add InitialBaseline -p ./App.Infrastructure/App.Infrastructure.csproj -s ./App.API/App.API.csproj --ignore-changes`
    - `dotnet ef database update -p ./App.Infrastructure/App.Infrastructure.csproj -s ./App.API/App.API.csproj`
  - Ya da idempotent SQL script üretip DBA ile uygulat:
    - `dotnet ef migrations script -p ./App.Infrastructure/App.Infrastructure.csproj -s ./App.API/App.API.csproj --idempotent -o migrate.sql`

Production önerileri
- Yönetilen veritabanı kullanın (Azure SQL / Azure Database for PostgreSQL).
- Secrets: connection string ve JWT anahtarlarını platform secret store/Key Vault kullanarak yönetin.
- Otomatik startup migration production için önerilmez; CI/CD içinde kontrollü migration adımı oluşturun.
- Backup, monitoring ve erişim kontrollerini konfigüre edin.

Troubleshooting kısa
- `docker` komutları çalışmıyorsa Docker Desktop çalıştırın: `docker version`, `docker info`.
- VS içinde Docker profili görünmüyorsa: `Add → Docker Support` veya `Properties/launchSettings.json` içine `Docker` profili ekleyin.
- Design-time DbContextFactory environment variable okumalı; aksi halde `dotnet ef` farklı connection string kullanabilir.
