using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tracker.Db.Models;

namespace Tracker.Db;

public class AppDbContext : IdentityDbContext<User, Role, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("asp_net_user_tokens");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("asp_net_user_logins");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("asp_net_user_claims");
        modelBuilder.Entity<Role>().ToTable("asp_net_roles").HasIndex(r => r.NormalizedName).HasDatabaseName("role_name_index");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("asp_net_user_roles");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("asp_net_role_claims");

        modelBuilder.Entity<User>(b =>
        {
            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.BossId);

            b.HasMany(e => e.Roles)
                .WithMany(e => e.Users)
                .UsingEntity<IdentityUserRole<string>>();

            b.HasOne(i => i.Boss)
                .WithMany(i => i.Children)
                .HasForeignKey(e => e.BossId);

            b.HasIndex(u => u.NormalizedUserName).HasDatabaseName("user_name_index").IsUnique();
            b.HasIndex(u => u.NormalizedEmail).HasDatabaseName("email_index");
            b.ToTable("asp_net_users");
        });
        SeedUsers(modelBuilder);
    }

    private static void SeedUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = "cd053e18-c8cc-49c6-858f-3ebea1a11214",
                UserName = "Овчаров",
                NormalizedUserName = "ОВЧАРОВ",
                Email = "ovcharov@parma.ru",
                NormalizedEmail = "OVCHAROV@PARMA.RU",
                PasswordHash = "AQAAAAEAACcQAAAAEFEnRRgs2cmkKDQfVcrbzbCSUSOcVIx/xk7ftlgQQmwBh+3WFNO4aEwmlXW05a0tLA==",
                SecurityStamp = "2KSHKNNQ42NT7L675Y6OVSM4Q6X4I3LD",
                ConcurrencyStamp = "5dbc2d6d-f206-4a35-b961-542276df93b7",
                LockoutEnabled = true
            },
            new User
            {
                Id = "38ecb3d6-5283-4f9e-96ce-95070f4336b9",
                BossId = "cd053e18-c8cc-49c6-858f-3ebea1a11214",
                UserName = "Забаев",
                NormalizedUserName = "ЗАБАЕВ",
                Email = "zabaev@parma.ru",
                NormalizedEmail = "ZABAEV@PARMA.RU",
                PasswordHash = "AQAAAAEAACcQAAAAEIaqLyLQtBbc+UK2GBj9ZqVOFsvVFxxiLZUKfyFhrB/oS949WKBmpGkfqILhn+3XgA==",
                SecurityStamp = "4TAUAMXMVQWQ67XRICFBPXTRQBCVTZG5",
                ConcurrencyStamp = "7e238c65-17d2-464c-9539-1a1086cc104e",
                LockoutEnabled = true
            },
            new User
            {
                Id = "b5632f6a-82bf-40c5-b866-9e68c2aea758",
                BossId = "cd053e18-c8cc-49c6-858f-3ebea1a11214",
                UserName = "Кайгородов",
                NormalizedUserName = "КАЙГОРОДОВ",
                Email = "kaigorodov@parma.ru",
                NormalizedEmail = "KAIGORODOV@PARMA.RU",
                PasswordHash = "AQAAAAEAACcQAAAAEND1bpz4qLHr+4PBxR3WaXJACxhflZ/rOFu27ajRZjgO3oInm2nAVEjwXRYPV59dAw==",
                SecurityStamp = "P4YC3AG3U6DMPHZOUQBT3CYPV3HP3DIN",
                ConcurrencyStamp = "7f2bb0c5-2fdc-412c-873a-99d17d96e5e7",
                LockoutEnabled = true
            },
            new User
            {
                Id = "55582f8a-9cad-4951-860d-97260f4a7ea5",
                BossId = "38ecb3d6-5283-4f9e-96ce-95070f4336b9",
                UserName = "Данилович",
                NormalizedUserName = "ДАНИЛОВИЧ",
                Email = "danilovich@parma.ru",
                NormalizedEmail = "DANILOVICH@PARMA.RU",
                PasswordHash = "AQAAAAEAACcQAAAAEEjH2QBjYMPiMZlsjNKyTjFB+lmA9adM6TTCp16Zz1ijZ2I8dP+tTeDgUKchArQjvA==",
                SecurityStamp = "CKECHTZA744R4OFXNSHCPZ5L2ZCCTEL2",
                ConcurrencyStamp = "dd512f73-6f64-4e36-be84-a42e8ce81e0a",
                LockoutEnabled = true
            },
            new User
            {
                Id = "aa962b1a-8efe-4b41-a405-4b14a8ba3132",
                BossId = "38ecb3d6-5283-4f9e-96ce-95070f4336b9",
                UserName = "Ульянич",
                NormalizedUserName = "УЛЬЯНИЧ",
                Email = "ulianich@parma.ru",
                NormalizedEmail = "ULIANICH@PARMA.RU",
                PasswordHash = "AQAAAAEAACcQAAAAELAn3ZHKO22jrcghVwJlhS0Lu46QzWeE3kxU5PjJdBhrhoY1vnhQ1ftSvM2kglIixA==",
                SecurityStamp = "IJBERLHWHJLGDXNGXKS4CDOU4D6LVS5H",
                ConcurrencyStamp = "6e39c11d-393e-482c-9d82-cd1943c6182e",
                LockoutEnabled = true
            },
            new User
            {
                Id = "0db07e67-f095-483e-92c6-df523ebf1216",
                BossId = "aa962b1a-8efe-4b41-a405-4b14a8ba3132",
                UserName = "Гафуров",
                NormalizedUserName = "ГАФУРОВ",
                Email = "gafurov@parma.ru",
                NormalizedEmail = "GAFUROV@PARMA.RU",
                PasswordHash = "AQAAAAEAACcQAAAAEHAjnCxisKj1qwrOQ2UneWkNMIU121CM2Gb81/vgrKt0tkJoYnGU86L5Amnyi9iU6A==",
                SecurityStamp = "PAAG4SBSIFANFDT75HSHCE2DCQUTLG2A",
                ConcurrencyStamp = "400e0f31-4c9c-4717-a868-447a9225035f",
                LockoutEnabled = true
            },
            new User
            {
                Id = "eddd1f09-68b1-4866-ab97-a5d8ec1dca29",
                BossId = "aa962b1a-8efe-4b41-a405-4b14a8ba3132",
                UserName = "Гнатенко",
                NormalizedUserName = "ГНАТЕНКО",
                Email = "gnatenko@parma.ru",
                NormalizedEmail = "GNATENKO@PARMA.RU",
                PasswordHash = "AQAAAAEAACcQAAAAEExRWYZPxxKRM5uc7Z71XWTJWazEF2QI0h7qjYg56CAqkYLb1LrHcRAXWSExXT7Hxw==",
                SecurityStamp = "LTFDNXABYK6S6GFSQ45Z3P53ZES37CYU",
                ConcurrencyStamp = "556b8e31-440c-4c02-88c2-423d51eac728",
                LockoutEnabled = true
            },
            new User
            {
                Id = "dd4e5a59-d500-4751-8fa3-41ecbc2c4f74",
                BossId = "eddd1f09-68b1-4866-ab97-a5d8ec1dca29",
                UserName = "Акулов",
                NormalizedUserName = "АКУЛОВ",
                Email = "akulov@parma.ru",
                NormalizedEmail = "AKULOV@PARMA.RU",
                PasswordHash = "AQAAAAEAACcQAAAAEB0Y5Qvc5qoXCPyXkl9qnqN4qAxK+s3c7BXFEYrz2sAme0aP+OiSGbJDoI0kqYp5Iw==",
                SecurityStamp = "7K3DKPHVEOLYWOGSG2DDQAIRYUWPXUKZ",
                ConcurrencyStamp = "a44ccabb-d0b0-4ed0-b21f-220df496faf8",
                LockoutEnabled = true
            },
            new User
            {
                Id = "7dd58573-7954-4e6f-8b0e-00420623038b",
                BossId = "eddd1f09-68b1-4866-ab97-a5d8ec1dca29",
                UserName = "Рябов",
                NormalizedUserName = "РЯБОВ",
                Email = "riabov@parma.ru",
                NormalizedEmail = "RIABOV@PARMA.RU",
                PasswordHash = "AQAAAAEAACcQAAAAEBrWLnyojv3+rSiOi8yjVOyLlUwGimzkTsm8bhpiwX2f0AehPadeh83MzFVmgfDlfg==",
                SecurityStamp = "5RYINNTABKGFBUNWXT4S34U4W76CSYUW",
                ConcurrencyStamp = "9d22ab6b-e6af-4bcd-84bd-6181f84d2339",
                LockoutEnabled = true
            },
            new User
            {
                Id = "d762b196-be23-43fb-8033-c69ac3bde570",
                BossId = "aa962b1a-8efe-4b41-a405-4b14a8ba3132",
                UserName = "Щелев",
                NormalizedUserName = "ЩЕЛЕВ",
                Email = "shelev@parma.ru",
                NormalizedEmail = "SHELEV@PARMA.RU",
                PasswordHash = "AQAAAAEAACcQAAAAEC5kctewaWAta9yCoMjIvr70JW6CylsjL7FUQsLUyhf689uQ7zENp+IN1r5r9OFuZg==",
                SecurityStamp = "2UDDP34D22GXRMCEJW7R6SSCSYNIPDFV",
                ConcurrencyStamp = "3154a3b7-14d2-45ea-9777-97b7e1b738dd",
                LockoutEnabled = true
            },
            new User
            {
                Id = "32f2b280-6fec-477c-8426-37bf5d723053",
                BossId = "d762b196-be23-43fb-8033-c69ac3bde570",
                UserName = "Ищенко",
                NormalizedUserName = "ИЩЕНКО",
                Email = "ishenko@parma.ru",
                NormalizedEmail = "ISHENKO@PARMA.RU",
                PasswordHash = "AQAAAAEAACcQAAAAENDw0Cqm9pFAemewVLecq+VLSUYLnqAyEGaxVVDiVS1rELeHEQkc3FXc6WmOmQYZSA==",
                SecurityStamp = "OF6L2TCA3KYZMQ2FEWJEMPSJU5FUIW5G",
                ConcurrencyStamp = "d9b116a5-6229-49a0-80ce-662a50b6a6c1",
                LockoutEnabled = true
            },
            new User
            {
                Id = "a37016ab-275d-43c9-ad27-25c83c0451a3",
                BossId = "d762b196-be23-43fb-8033-c69ac3bde570",
                UserName = "Мельников",
                NormalizedUserName = "МЕЛЬНИКОВ",
                Email = "melnikov@parma.ru",
                NormalizedEmail = "MELNIKOV@PARMA.RU",
                PasswordHash = "AQAAAAEAACcQAAAAEO6Pavg6HVeSipcFDD8cPxzD0eanrA0HhiIk+qWgl3RQxCm6X5nfmtDtPFeE73JHmA==",
                SecurityStamp = "4SFQU4ZZFNIM4UKHRVNLWNQ3RETBF4RQ",
                ConcurrencyStamp = "21279090-a23e-44ab-b414-311643a99442",
                LockoutEnabled = true
            },
            new User
            {
                Id = "d2b000e0-d352-4532-9b56-3ecce303a168",
                BossId = "a37016ab-275d-43c9-ad27-25c83c0451a3",
                UserName = "Веретенникова",
                NormalizedUserName = "ВЕРЕТЕННИКОВА",
                Email = "veretennikova@parma.ru",
                NormalizedEmail = "VERETENNIKOVA@PARMA.RU",
                PasswordHash = "AQAAAAEAACcQAAAAEDqroat9CObt+PE4EuSnzETkonj1NvylMVtiPYAMC1E9fkQkwJAH4RZYYyI8zTPxQg==",
                SecurityStamp = "PE7NWJHMCYMHI23RBE3X6LSLVDMOSA7M",
                ConcurrencyStamp = "491ed8a7-c70d-4325-b498-256c2dc957c1",
                LockoutEnabled = true
            },
            new User
            {
                Id = "aa8196f8-0237-4600-9cf2-dc91f08e6122",
                BossId = "a37016ab-275d-43c9-ad27-25c83c0451a3",
                UserName = "Корепанов",
                NormalizedUserName = "КОРЕПАНОВ",
                Email = "korepanov@parma.ru",
                NormalizedEmail = "KOREPANOV@PARMA.RU",
                PasswordHash = "AQAAAAEAACcQAAAAEBmReKZLUkZVkRT+JsQTJj/SXxKUtguETPSCN8BV2y6C1O/sZGm8/fn8Ec14ABkTSw==",
                SecurityStamp = "ZRIYT7IRMVPLH2NKL7EKW7SVJ26LWPML",
                ConcurrencyStamp = "84fd4cd2-b426-4abc-8125-cf622717c2b7",
                LockoutEnabled = true
            },
            new User
            {
                Id = "5ed8c7bc-0d09-4f95-81b4-2668110e255f",
                BossId = "aa8196f8-0237-4600-9cf2-dc91f08e6122",
                UserName = "Ватрубин",
                NormalizedUserName = "ВАТРУБИН",
                Email = "vatrubin@parma.ru",
                NormalizedEmail = "VATRUBIN@PARMA.RU",
                PasswordHash = "AQAAAAEAACcQAAAAENJZp1M7wFY9ATnuSwbo7hMb9GZFXOJbZwG+Wt6txo5NiOFwKfRO6nFGgslqRgBEhw==",
                SecurityStamp = "6RNC3NLA727K35JJVYREQP4U6ZJIT2SE",
                ConcurrencyStamp = "10619377-550f-49d4-81dc-09b38b2a870d",
                LockoutEnabled = true
            }
        );
    }
}
