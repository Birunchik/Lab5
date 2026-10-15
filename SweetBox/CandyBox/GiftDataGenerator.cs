using Bogus;
using SweetBox.CandyClasses;
using System.Text;
using System.Text.Json;

namespace SweetBox.CandyBox {
   public static class GiftDataGenerator {
      private static readonly List<string> JellyNames = ParseNamesFromFile("CandyBox/settings/jelly_names.txt", ["Мишка", "Червячки", "Фруктовый сад"]);
      private static readonly List<string> ChocolateNames = ParseNamesFromFile("CandyBox/settings/chocolate_names.txt", ["Трюфель", "Аленка", "Белочка"]);
      private static readonly List<string> LollipopNames = ParseNamesFromFile("CandyBox/settings/lollipop_names.txt", ["Петушок", "Монпансье", "Фруктовая палочка"]);
      private static readonly CandyGenerationSettings Settings = LoadSettings("CandyBox/settings/candy_config.json");

      public static CandyGenerationSettings LoadSettings(string filePath) {
         if (!File.Exists(filePath)) {
            return GetDefaultSettings();
         }
         JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

         try {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var settings = JsonSerializer.Deserialize<CandyGenerationSettings>(fileStream, JsonOptions);
            return settings ?? GetDefaultSettings();
         }
         catch (JsonException) {
            return GetDefaultSettings();
         }
         catch (IOException) {
            return GetDefaultSettings();
         }
      }

      private static CandyGenerationSettings GetDefaultSettings() {
         Console.WriteLine("ДЕФОЛТНЫЕ");
         return new CandyGenerationSettings
         {
            WeightRanges = new Dictionary<CandySize, RangeConfig>
            {
                { CandySize.Small, new RangeConfig { Min = 5.0, Max = 15.0 } },
                { CandySize.Medium, new RangeConfig { Min = 15.1, Max = 50.0 } },
                { CandySize.Large, new RangeConfig { Min = 50.1, Max = 150.0 } }
            },
            JellyConfig = new JellyConfig { MinSugar = 10.0, MaxSugar = 75.0 },
            ChocolateConfig = new ChocolateConfig
            {
               MinSugar = 20.0,
               MaxSugar = 90.0,
               AvailableTypes = new() { "Dark", "Milk", "White" }
            },
            LollipopConfig = new LollipopConfig
            {
               MinSugar = 80.0,
               MaxSugar = 99.9,
               Flavors = new() { "Клубника", "Апельсин", "Кола", "Мята" }
            }
         };
      }

      private static List<string> ParseNamesFromFile(string filePath, string[] fallbackNames) {
         var names = new List<string>();

         if (!File.Exists(filePath)) {
            Console.WriteLine("ДЕФОЛТНЫЕ");
            return fallbackNames.ToList();
         }

         try {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fileStream, Encoding.UTF8)) {
               string line;
               while ((line = reader.ReadLine()) != null) {
                  string trimmedLine = line.Trim();
                  if (!string.IsNullOrEmpty(trimmedLine)) {
                     names.Add(trimmedLine);
                  }
               }
            }
         }
         catch (IOException) {
            Console.WriteLine("ДЕФОЛТНЫЕ");
            return fallbackNames.ToList();
         }

         return names.Count > 0 ? names : fallbackNames.ToList();
      }

      public static Candy GenerateRandomCandy() {
         var faker = new Faker();
         int candyType = faker.Random.Int(0, 2); // 0 — JellyCandy, 1 — ChocolateCandy, 2 — Lollipop

         return candyType switch
         {
            0 => GenerateJellyCandy(faker),
            1 => GenerateChocolateCandy(faker),
            2 => GenerateLollipop(faker),
            _ => throw new InvalidOperationException("Неизвестный тип конфеты.")
         };
      }

      private static JellyCandy GenerateJellyCandy(Faker faker) {
         var config = Settings.JellyConfig;

         return new Faker<JellyCandy>()
            .CustomInstantiator(f => {
               var size = f.PickRandom<CandySize>();
               double weight = GetWeightBySize(size, f);

               return new JellyCandy(
                  name: f.PickRandom(JellyNames),
                  size: size,
                  weight: weight,
                  sugarContent: f.Random.Double(config.MinSugar, config.MaxSugar),
                  isHalal: f.Random.Bool()
               );
            })
            .Generate();
      }

      private static ChocolateCandy GenerateChocolateCandy(Faker faker) {
         var config = Settings.ChocolateConfig;

         return new Faker<ChocolateCandy>()
            .CustomInstantiator(f => {
               var size = f.PickRandom<CandySize>();
               double weight = GetWeightBySize(size, f);

               return new ChocolateCandy(
                  name: f.PickRandom(ChocolateNames),
                  size: size,
                  weight: weight,
                  sugarContent: f.Random.Double(config.MinSugar, config.MaxSugar),
                  chocolateType: Enum.Parse<ChocolateType>(f.PickRandom(config.AvailableTypes)),
                  hasFilling: f.Random.Bool()
               );
            })
            .Generate();
      }

      private static Lollipop GenerateLollipop(Faker faker) {
         var config = Settings.LollipopConfig;

         return new Faker<Lollipop>()
            .CustomInstantiator(f => {
               var size = f.PickRandom<CandySize>();
               double weight = GetWeightBySize(size, f);

               return new Lollipop(
                  name: f.PickRandom(LollipopNames),
                  size: size,
                  weight: weight,
                  sugarContent: f.Random.Double(config.MinSugar, config.MaxSugar),
                  flavor: f.PickRandom(config.Flavors),
                  isLollipopOnStick: f.Random.Bool()
               );
            })
            .Generate();
      }

      private static double GetWeightBySize(CandySize size, Faker f) {
         if (Settings.WeightRanges.TryGetValue(size, out var range)) {
            return f.Random.Double(range.Min, range.Max);
         }
         return f.Random.Double(5.0, 50.0);
      }

      public static Gift GenerateGiftByCount(int count) {
         if (count <= 0) {
            throw new ArgumentOutOfRangeException(nameof(count));
         }

         var candies = new List<Candy>(count);
         for (int i = 0; i < count; i++) {
            candies.Add(GenerateRandomCandy());
         }

         return new Gift(candies);
      }

      public static Gift GenerateGiftByTargetWeight(double targetWeight) {
         var gift = new Gift(targetWeight);

         while (gift.RemainingWeight >= 5.0) {
            var candidate = GenerateRandomCandy();
            if (candidate.Weight <= gift.RemainingWeight) {
               gift.Add(candidate);
            }
         }

         return gift;
      }
   }
}