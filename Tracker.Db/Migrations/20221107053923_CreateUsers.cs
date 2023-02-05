using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tracker.Db.Migrations
{
    public partial class CreateUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "asp_net_users",
                columns: new[] { "id", "access_failed_count", "boss_id", "concurrency_stamp", "email", "email_confirmed", "lockout_enabled", "lockout_end", "normalized_email", "normalized_user_name", "password_hash", "phone_number", "phone_number_confirmed", "refresh_token", "refresh_token_expiry_time", "security_stamp", "two_factor_enabled", "user_name" },
                values: new object[] { "cd053e18-c8cc-49c6-858f-3ebea1a11214", 0, null, "5dbc2d6d-f206-4a35-b961-542276df93b7", "ovcharov@parma.ru", false, true, null, "OVCHAROV@PARMA.RU", "ОВЧАРОВ", "AQAAAAEAACcQAAAAEFEnRRgs2cmkKDQfVcrbzbCSUSOcVIx/xk7ftlgQQmwBh+3WFNO4aEwmlXW05a0tLA==", null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "2KSHKNNQ42NT7L675Y6OVSM4Q6X4I3LD", false, "Овчаров" });

            migrationBuilder.InsertData(
                table: "asp_net_users",
                columns: new[] { "id", "access_failed_count", "boss_id", "concurrency_stamp", "email", "email_confirmed", "lockout_enabled", "lockout_end", "normalized_email", "normalized_user_name", "password_hash", "phone_number", "phone_number_confirmed", "refresh_token", "refresh_token_expiry_time", "security_stamp", "two_factor_enabled", "user_name" },
                values: new object[,]
                {
                    { "38ecb3d6-5283-4f9e-96ce-95070f4336b9", 0, "cd053e18-c8cc-49c6-858f-3ebea1a11214", "7e238c65-17d2-464c-9539-1a1086cc104e", "zabaev@parma.ru", false, true, null, "ZABAEV@PARMA.RU", "ЗАБАЕВ", "AQAAAAEAACcQAAAAEIaqLyLQtBbc+UK2GBj9ZqVOFsvVFxxiLZUKfyFhrB/oS949WKBmpGkfqILhn+3XgA==", null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "4TAUAMXMVQWQ67XRICFBPXTRQBCVTZG5", false, "Забаев" },
                    { "b5632f6a-82bf-40c5-b866-9e68c2aea758", 0, "cd053e18-c8cc-49c6-858f-3ebea1a11214", "7f2bb0c5-2fdc-412c-873a-99d17d96e5e7", "kaigorodov@parma.ru", false, true, null, "KAIGORODOV@PARMA.RU", "КАЙГОРОДОВ", "AQAAAAEAACcQAAAAEND1bpz4qLHr+4PBxR3WaXJACxhflZ/rOFu27ajRZjgO3oInm2nAVEjwXRYPV59dAw==", null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "P4YC3AG3U6DMPHZOUQBT3CYPV3HP3DIN", false, "Кайгородов" }
                });

            migrationBuilder.InsertData(
                table: "asp_net_users",
                columns: new[] { "id", "access_failed_count", "boss_id", "concurrency_stamp", "email", "email_confirmed", "lockout_enabled", "lockout_end", "normalized_email", "normalized_user_name", "password_hash", "phone_number", "phone_number_confirmed", "refresh_token", "refresh_token_expiry_time", "security_stamp", "two_factor_enabled", "user_name" },
                values: new object[,]
                {
                    { "55582f8a-9cad-4951-860d-97260f4a7ea5", 0, "38ecb3d6-5283-4f9e-96ce-95070f4336b9", "dd512f73-6f64-4e36-be84-a42e8ce81e0a", "danilovich@parma.ru", false, true, null, "DANILOVICH@PARMA.RU", "ДАНИЛОВИЧ", "AQAAAAEAACcQAAAAEEjH2QBjYMPiMZlsjNKyTjFB+lmA9adM6TTCp16Zz1ijZ2I8dP+tTeDgUKchArQjvA==", null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CKECHTZA744R4OFXNSHCPZ5L2ZCCTEL2", false, "Данилович" },
                    { "aa962b1a-8efe-4b41-a405-4b14a8ba3132", 0, "38ecb3d6-5283-4f9e-96ce-95070f4336b9", "6e39c11d-393e-482c-9d82-cd1943c6182e", "ulianich@parma.ru", false, true, null, "ULIANICH@PARMA.RU", "УЛЬЯНИЧ", "AQAAAAEAACcQAAAAELAn3ZHKO22jrcghVwJlhS0Lu46QzWeE3kxU5PjJdBhrhoY1vnhQ1ftSvM2kglIixA==", null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "IJBERLHWHJLGDXNGXKS4CDOU4D6LVS5H", false, "Ульянич" }
                });

            migrationBuilder.InsertData(
                table: "asp_net_users",
                columns: new[] { "id", "access_failed_count", "boss_id", "concurrency_stamp", "email", "email_confirmed", "lockout_enabled", "lockout_end", "normalized_email", "normalized_user_name", "password_hash", "phone_number", "phone_number_confirmed", "refresh_token", "refresh_token_expiry_time", "security_stamp", "two_factor_enabled", "user_name" },
                values: new object[,]
                {
                    { "0db07e67-f095-483e-92c6-df523ebf1216", 0, "aa962b1a-8efe-4b41-a405-4b14a8ba3132", "400e0f31-4c9c-4717-a868-447a9225035f", "gafurov@parma.ru", false, true, null, "GAFUROV@PARMA.RU", "ГАФУРОВ", "AQAAAAEAACcQAAAAEHAjnCxisKj1qwrOQ2UneWkNMIU121CM2Gb81/vgrKt0tkJoYnGU86L5Amnyi9iU6A==", null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "PAAG4SBSIFANFDT75HSHCE2DCQUTLG2A", false, "Гафуров" },
                    { "d762b196-be23-43fb-8033-c69ac3bde570", 0, "aa962b1a-8efe-4b41-a405-4b14a8ba3132", "3154a3b7-14d2-45ea-9777-97b7e1b738dd", "shelev@parma.ru", false, true, null, "SHELEV@PARMA.RU", "ЩЕЛЕВ", "AQAAAAEAACcQAAAAEC5kctewaWAta9yCoMjIvr70JW6CylsjL7FUQsLUyhf689uQ7zENp+IN1r5r9OFuZg==", null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "2UDDP34D22GXRMCEJW7R6SSCSYNIPDFV", false, "Щелев" },
                    { "eddd1f09-68b1-4866-ab97-a5d8ec1dca29", 0, "aa962b1a-8efe-4b41-a405-4b14a8ba3132", "556b8e31-440c-4c02-88c2-423d51eac728", "gnatenko@parma.ru", false, true, null, "GNATENKO@PARMA.RU", "ГНАТЕНКО", "AQAAAAEAACcQAAAAEExRWYZPxxKRM5uc7Z71XWTJWazEF2QI0h7qjYg56CAqkYLb1LrHcRAXWSExXT7Hxw==", null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "LTFDNXABYK6S6GFSQ45Z3P53ZES37CYU", false, "Гнатенко" }
                });

            migrationBuilder.InsertData(
                table: "asp_net_users",
                columns: new[] { "id", "access_failed_count", "boss_id", "concurrency_stamp", "email", "email_confirmed", "lockout_enabled", "lockout_end", "normalized_email", "normalized_user_name", "password_hash", "phone_number", "phone_number_confirmed", "refresh_token", "refresh_token_expiry_time", "security_stamp", "two_factor_enabled", "user_name" },
                values: new object[,]
                {
                    { "32f2b280-6fec-477c-8426-37bf5d723053", 0, "d762b196-be23-43fb-8033-c69ac3bde570", "d9b116a5-6229-49a0-80ce-662a50b6a6c1", "ishenko@parma.ru", false, true, null, "ISHENKO@PARMA.RU", "ИЩЕНКО", "AQAAAAEAACcQAAAAENDw0Cqm9pFAemewVLecq+VLSUYLnqAyEGaxVVDiVS1rELeHEQkc3FXc6WmOmQYZSA==", null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "OF6L2TCA3KYZMQ2FEWJEMPSJU5FUIW5G", false, "Ищенко" },
                    { "7dd58573-7954-4e6f-8b0e-00420623038b", 0, "eddd1f09-68b1-4866-ab97-a5d8ec1dca29", "9d22ab6b-e6af-4bcd-84bd-6181f84d2339", "riabov@parma.ru", false, true, null, "RIABOV@PARMA.RU", "РЯБОВ", "AQAAAAEAACcQAAAAEBrWLnyojv3+rSiOi8yjVOyLlUwGimzkTsm8bhpiwX2f0AehPadeh83MzFVmgfDlfg==", null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "5RYINNTABKGFBUNWXT4S34U4W76CSYUW", false, "Рябов" },
                    { "a37016ab-275d-43c9-ad27-25c83c0451a3", 0, "d762b196-be23-43fb-8033-c69ac3bde570", "21279090-a23e-44ab-b414-311643a99442", "melnikov@parma.ru", false, true, null, "MELNIKOV@PARMA.RU", "МЕЛЬНИКОВ", "AQAAAAEAACcQAAAAEO6Pavg6HVeSipcFDD8cPxzD0eanrA0HhiIk+qWgl3RQxCm6X5nfmtDtPFeE73JHmA==", null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "4SFQU4ZZFNIM4UKHRVNLWNQ3RETBF4RQ", false, "Мельников" },
                    { "dd4e5a59-d500-4751-8fa3-41ecbc2c4f74", 0, "eddd1f09-68b1-4866-ab97-a5d8ec1dca29", "a44ccabb-d0b0-4ed0-b21f-220df496faf8", "akulov@parma.ru", false, true, null, "AKULOV@PARMA.RU", "АКУЛОВ", "AQAAAAEAACcQAAAAEB0Y5Qvc5qoXCPyXkl9qnqN4qAxK+s3c7BXFEYrz2sAme0aP+OiSGbJDoI0kqYp5Iw==", null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "7K3DKPHVEOLYWOGSG2DDQAIRYUWPXUKZ", false, "Акулов" }
                });

            migrationBuilder.InsertData(
                table: "asp_net_users",
                columns: new[] { "id", "access_failed_count", "boss_id", "concurrency_stamp", "email", "email_confirmed", "lockout_enabled", "lockout_end", "normalized_email", "normalized_user_name", "password_hash", "phone_number", "phone_number_confirmed", "refresh_token", "refresh_token_expiry_time", "security_stamp", "two_factor_enabled", "user_name" },
                values: new object[,]
                {
                    { "aa8196f8-0237-4600-9cf2-dc91f08e6122", 0, "a37016ab-275d-43c9-ad27-25c83c0451a3", "84fd4cd2-b426-4abc-8125-cf622717c2b7", "korepanov@parma.ru", false, true, null, "KOREPANOV@PARMA.RU", "КОРЕПАНОВ", "AQAAAAEAACcQAAAAEBmReKZLUkZVkRT+JsQTJj/SXxKUtguETPSCN8BV2y6C1O/sZGm8/fn8Ec14ABkTSw==", null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ZRIYT7IRMVPLH2NKL7EKW7SVJ26LWPML", false, "Корепанов" },
                    { "d2b000e0-d352-4532-9b56-3ecce303a168", 0, "a37016ab-275d-43c9-ad27-25c83c0451a3", "491ed8a7-c70d-4325-b498-256c2dc957c1", "veretennikova@parma.ru", false, true, null, "VERETENNIKOVA@PARMA.RU", "ВЕРЕТЕННИКОВА", "AQAAAAEAACcQAAAAEDqroat9CObt+PE4EuSnzETkonj1NvylMVtiPYAMC1E9fkQkwJAH4RZYYyI8zTPxQg==", null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "PE7NWJHMCYMHI23RBE3X6LSLVDMOSA7M", false, "Веретенникова" }
                });

            migrationBuilder.InsertData(
                table: "asp_net_users",
                columns: new[] { "id", "access_failed_count", "boss_id", "concurrency_stamp", "email", "email_confirmed", "lockout_enabled", "lockout_end", "normalized_email", "normalized_user_name", "password_hash", "phone_number", "phone_number_confirmed", "refresh_token", "refresh_token_expiry_time", "security_stamp", "two_factor_enabled", "user_name" },
                values: new object[] { "5ed8c7bc-0d09-4f95-81b4-2668110e255f", 0, "aa8196f8-0237-4600-9cf2-dc91f08e6122", "10619377-550f-49d4-81dc-09b38b2a870d", "vatrubin@parma.ru", false, true, null, "VATRUBIN@PARMA.RU", "ВАТРУБИН", "AQAAAAEAACcQAAAAENJZp1M7wFY9ATnuSwbo7hMb9GZFXOJbZwG+Wt6txo5NiOFwKfRO6nFGgslqRgBEhw==", null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "6RNC3NLA727K35JJVYREQP4U6ZJIT2SE", false, "Ватрубин" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "asp_net_users",
                keyColumn: "id",
                keyValue: "0db07e67-f095-483e-92c6-df523ebf1216");

            migrationBuilder.DeleteData(
                table: "asp_net_users",
                keyColumn: "id",
                keyValue: "32f2b280-6fec-477c-8426-37bf5d723053");

            migrationBuilder.DeleteData(
                table: "asp_net_users",
                keyColumn: "id",
                keyValue: "55582f8a-9cad-4951-860d-97260f4a7ea5");

            migrationBuilder.DeleteData(
                table: "asp_net_users",
                keyColumn: "id",
                keyValue: "5ed8c7bc-0d09-4f95-81b4-2668110e255f");

            migrationBuilder.DeleteData(
                table: "asp_net_users",
                keyColumn: "id",
                keyValue: "7dd58573-7954-4e6f-8b0e-00420623038b");

            migrationBuilder.DeleteData(
                table: "asp_net_users",
                keyColumn: "id",
                keyValue: "b5632f6a-82bf-40c5-b866-9e68c2aea758");

            migrationBuilder.DeleteData(
                table: "asp_net_users",
                keyColumn: "id",
                keyValue: "d2b000e0-d352-4532-9b56-3ecce303a168");

            migrationBuilder.DeleteData(
                table: "asp_net_users",
                keyColumn: "id",
                keyValue: "dd4e5a59-d500-4751-8fa3-41ecbc2c4f74");

            migrationBuilder.DeleteData(
                table: "asp_net_users",
                keyColumn: "id",
                keyValue: "aa8196f8-0237-4600-9cf2-dc91f08e6122");

            migrationBuilder.DeleteData(
                table: "asp_net_users",
                keyColumn: "id",
                keyValue: "eddd1f09-68b1-4866-ab97-a5d8ec1dca29");

            migrationBuilder.DeleteData(
                table: "asp_net_users",
                keyColumn: "id",
                keyValue: "a37016ab-275d-43c9-ad27-25c83c0451a3");

            migrationBuilder.DeleteData(
                table: "asp_net_users",
                keyColumn: "id",
                keyValue: "d762b196-be23-43fb-8033-c69ac3bde570");

            migrationBuilder.DeleteData(
                table: "asp_net_users",
                keyColumn: "id",
                keyValue: "aa962b1a-8efe-4b41-a405-4b14a8ba3132");

            migrationBuilder.DeleteData(
                table: "asp_net_users",
                keyColumn: "id",
                keyValue: "38ecb3d6-5283-4f9e-96ce-95070f4336b9");

            migrationBuilder.DeleteData(
                table: "asp_net_users",
                keyColumn: "id",
                keyValue: "cd053e18-c8cc-49c6-858f-3ebea1a11214");
        }
    }
}
