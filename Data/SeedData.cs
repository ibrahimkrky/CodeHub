using CodeHub.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CodeHub.Data
{
    // Demo için örnek veri. Program.cs'te uygulama açılışında çağrılır.
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await db.Database.MigrateAsync();

            // Zaten veri varsa tekrar ekleme
            if (await db.Snippets.AnyAsync()) return;

            // --- Kullanıcılar ---
            var users = new List<(string email, string name, int score)>
            {
                ("ayse@codehub.dev", "Ayşe Yılmaz", 0),
                ("mehmet@codehub.dev", "Mehmet Demir", 0),
                ("zeynep@codehub.dev", "Zeynep Kaya", 0),
            };

            var createdUsers = new List<ApplicationUser>();
            foreach (var (email, name, score) in users)
            {
                var u = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    DisplayName = name,
                    Score = score
                };
                await userManager.CreateAsync(u, "Parola123!");
                createdUsers.Add(u);
            }

            // --- Tag'ler ---
            var tagNames = new[] { "C#", "JavaScript", "SQL", "CSS", "Python", "HTML", "React", "ASP.NET", "LINQ", "Bootstrap" };
            var tags = tagNames.Select(t => new Tag { Name = t }).ToList();
            db.Tags.AddRange(tags);
            await db.SaveChangesAsync();

            Tag T(string n) => tags.First(x => x.Name == n);

            // --- Snippet'ler ---
            var snippets = new List<Snippet>
            {
                new() { Title = "LINQ ile listeyi gruplama", Language = "C#", AuthorId = createdUsers[0].Id,
                    Description = "Bir listeyi belirli bir alana göre gruplar.", LikeCount = 12, ViewCount = 140,
                    Code = "var grouped = people.GroupBy(p => p.City)\n    .Select(g => new { City = g.Key, Count = g.Count() });" },
                new() { Title = "Debounce fonksiyonu", Language = "JavaScript", AuthorId = createdUsers[1].Id,
                    Description = "Arama kutuları için klasik debounce.", LikeCount = 8, ViewCount = 95,
                    Code = "function debounce(fn, delay) {\n  let t;\n  return (...args) => {\n    clearTimeout(t);\n    t = setTimeout(() => fn(...args), delay);\n  };\n}" },
                new() { Title = "SQL'de satır numarası", Language = "SQL", AuthorId = createdUsers[2].Id,
                    Description = "ROW_NUMBER ile sıralama.", LikeCount = 15, ViewCount = 210,
                    Code = "SELECT *, ROW_NUMBER() OVER (ORDER BY CreatedAt DESC) AS rn\nFROM Snippets;" },
                new() { Title = "CSS ile ortalama (flex)", Language = "CSS", AuthorId = createdUsers[0].Id,
                    Description = "Dikey ve yatay ortalama.", LikeCount = 20, ViewCount = 300,
                    Code = ".center {\n  display: flex;\n  align-items: center;\n  justify-content: center;\n}" },
                new() { Title = "Python liste kavraması", Language = "Python", AuthorId = createdUsers[1].Id,
                    Description = "Tek satırda filtreleme.", LikeCount = 6, ViewCount = 70,
                    Code = "evens = [x for x in range(20) if x % 2 == 0]" },
                new() { Title = "Fetch ile POST isteği", Language = "JavaScript", AuthorId = createdUsers[2].Id,
                    Description = "JSON gövdeli POST.", LikeCount = 9, ViewCount = 120,
                    Code = "await fetch('/api/data', {\n  method: 'POST',\n  headers: { 'Content-Type': 'application/json' },\n  body: JSON.stringify(payload)\n});" },
                new() { Title = "Async metot örneği", Language = "C#", AuthorId = createdUsers[0].Id,
                    Description = "Basit async/await.", LikeCount = 11, ViewCount = 130,
                    Code = "public async Task<int> GetCountAsync()\n{\n    return await _db.Snippets.CountAsync();\n}" },
                new() { Title = "HTML form doğrulama", Language = "HTML", AuthorId = createdUsers[1].Id,
                    Description = "required attribute kullanımı.", LikeCount = 4, ViewCount = 55,
                    Code = "<input type=\"email\" required placeholder=\"E-posta\" />" },
            };
            db.Snippets.AddRange(snippets);
            await db.SaveChangesAsync();

            // --- SnippetTag eşleşmeleri ---
            db.SnippetTags.AddRange(
                new SnippetTag { SnippetId = snippets[0].Id, TagId = T("C#").Id },
                new SnippetTag { SnippetId = snippets[0].Id, TagId = T("LINQ").Id },
                new SnippetTag { SnippetId = snippets[1].Id, TagId = T("JavaScript").Id },
                new SnippetTag { SnippetId = snippets[2].Id, TagId = T("SQL").Id },
                new SnippetTag { SnippetId = snippets[3].Id, TagId = T("CSS").Id },
                new SnippetTag { SnippetId = snippets[4].Id, TagId = T("Python").Id },
                new SnippetTag { SnippetId = snippets[5].Id, TagId = T("JavaScript").Id },
                new SnippetTag { SnippetId = snippets[6].Id, TagId = T("C#").Id },
                new SnippetTag { SnippetId = snippets[6].Id, TagId = T("ASP.NET").Id },
                new SnippetTag { SnippetId = snippets[7].Id, TagId = T("HTML").Id }
            );

            // --- Koleksiyonlar ---
            var col1 = new Collection { Name = "Frontend Araçları", OwnerId = createdUsers[0].Id };
            var col2 = new Collection { Name = "Veri Tabanı Kodları", OwnerId = createdUsers[0].Id };
            db.Collections.AddRange(col1, col2);
            await db.SaveChangesAsync();

            db.CollectionSnippets.AddRange(
                new CollectionSnippet { CollectionId = col1.Id, SnippetId = snippets[3].Id },
                new CollectionSnippet { CollectionId = col1.Id, SnippetId = snippets[1].Id },
                new CollectionSnippet { CollectionId = col2.Id, SnippetId = snippets[2].Id }
            );

            // --- Review + yorumlar ---
            var review = new Review
            {
                Title = "Bu döngü neden yavaş çalışıyor?",
                Language = "C#",
                Problem = "Büyük listede çok yavaş, nasıl optimize ederim?",
                AuthorId = createdUsers[1].Id,
                Code = "foreach (var item in list)\n{\n    if (other.Contains(item)) result.Add(item);\n}"
            };
            db.Reviews.Add(review);
            await db.SaveChangesAsync();

            db.Comments.AddRange(
                new Comment { ReviewId = review.Id, AuthorId = createdUsers[0].Id, IsSolution = true,
                    Body = "other'ı HashSet'e çevir, Contains O(1) olur." },
                new Comment { ReviewId = review.Id, AuthorId = createdUsers[2].Id,
                    Body = "LINQ Intersect de kullanabilirsin." }
            );

            // --- Beğeniler ---
            db.Stars.AddRange(
                new Star { UserId = createdUsers[1].Id, SnippetId = snippets[0].Id },
                new Star { UserId = createdUsers[2].Id, SnippetId = snippets[0].Id },
                new Star { UserId = createdUsers[0].Id, SnippetId = snippets[3].Id }
            );

            await db.SaveChangesAsync();

            // --- Skorları güncelle (snippet sayısı*5 + toplam beğeni) ---
            foreach (var u in createdUsers)
            {
                int snippetCount = await db.Snippets.CountAsync(s => s.AuthorId == u.Id);
                int likes = await db.Snippets.Where(s => s.AuthorId == u.Id).SumAsync(s => s.LikeCount);
                u.Score = snippetCount * 5 + likes;
                await userManager.UpdateAsync(u);
            }
        }
    }
}
