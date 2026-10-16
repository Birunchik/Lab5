using SweetBox;
using SweetBox.CandyBox;
using SweetBox.CandyClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

class Program {
   private static bool _isRunning = true;
   private static Gift? _lastGeneratedGift = null;

   static void Main(string[] args) {
      while (_isRunning) {
         RenderMenu();
         ProcessUserChoice();
      }
   }

   private static void RenderMenu() {
      Console.Clear();
      Console.WriteLine("==================================================");
      Console.WriteLine("          СИСТЕМА УПРАВЛЕНИЯ SWEETBOX             ");
      Console.WriteLine("==================================================");
      Console.WriteLine("1. Простая генерация одной конфетки");
      Console.WriteLine("2. Генерация подарка с определенным количеством конфет");
      Console.WriteLine("3. Генерация подарка с заданным весом");
      Console.WriteLine("4. Проверить, был ли последний набор халяльным");
      Console.WriteLine("5. Найти конфеты в последнем наборе по диапазону сахара");
      Console.WriteLine("6. Отсортировать последний набор конфет");
      Console.WriteLine("7. Выход");
      Console.WriteLine("--------------------------------------------------");
      Console.Write("Выберите пункт меню (1-7): ");
   }

   private static void ProcessUserChoice() {
      string? choice = Console.ReadLine();
      Console.WriteLine();

      try {
         switch (choice) {
            case "1":
               ExecuteSingleCandyGeneration();
               break;
            case "2":
               ExecuteGiftByCountGeneration();
               break;
            case "3":
               ExecuteGiftByWeightGeneration();
               break;
            case "4":
               ExecuteHalalCheck();
               break;
            case "5":
               ExecuteFindBySugarContent();
               break;
            case "6":
               ExecuteSortingMenu();
               break;
            case "7":
               _isRunning = false;
               Console.WriteLine("Завершение работы программы.");
               break;
            default:
               Console.WriteLine("[Ошибка]: Неверный выбор. Введите число от 1 до 7.");
               break;
         }
      }
      catch (Exception ex) {
         Console.ForegroundColor = ConsoleColor.Red;
         Console.WriteLine($"[Критическая ошибка]: {ex.Message}");
         Console.ResetColor();
      }

      if (_isRunning) {
         Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
         Console.ReadKey();
      }
   }
   private static void ExecuteSingleCandyGeneration() {
      Console.WriteLine("--- Генерация единичной конфеты ---");
      Candy candy = GiftDataGenerator.GenerateRandomCandy();
      PrintCandyDetails(candy);
   }
   private static void ExecuteSortingMenu() {
      if (_lastGeneratedGift == null || !_lastGeneratedGift.Items.Any()) {
         Console.WriteLine("[Информация]: Сначала сгенерируйте подарочный набор (пункты 2 или 3).");
         return;
      }

      bool inSortingMenu = true;
      while (inSortingMenu) {
         Console.Clear();
         Console.WriteLine("==================================================");
         Console.WriteLine("            МЕНЮ СОРТИРОВКИ КОНФЕТ                ");
         Console.WriteLine("==================================================");
         Console.WriteLine("1. Сортировать по названию (Name)");
         Console.WriteLine("2. Сортировать по весу (Weight)");
         Console.WriteLine("3. Сортировать по содержанию сахара (SugarContent)");
         Console.WriteLine("4. Сортировать по размеру (Size)");
         Console.WriteLine("5. Назад в главное меню");
         Console.WriteLine("--------------------------------------------------");
         Console.Write("Выберите критерий сортировки (1-5): ");

         string? sortChoice = Console.ReadLine();
         Console.WriteLine();

         try {
            SortCriterion? criterion = sortChoice switch
            {
               "1" => SortCriterion.Name,
               "2" => SortCriterion.Weight,
               "3" => SortCriterion.SugarContent,
               "4" => SortCriterion.Size,
               "5" => null,
               _ => throw new ArgumentException("Неверный выбор пункта сортировки.")
            };

            if (criterion == null) {
               inSortingMenu = false;
               break;
            }
            _lastGeneratedGift.SortBy(criterion.Value);
            SaveAndReportBox("last_sorted_gift_report.txt");

            Console.WriteLine($"\n[Успех]: Набор успешно отсортирован по критерию: {criterion.Value}!");
            inSortingMenu = false;
         }
         catch (Exception ex) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Ошибка]: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine("\nНажмите любую клавишу, чтобы продолжить...");
            Console.ReadKey();
         }
      }
   }

   private static void ExecuteGiftByCountGeneration() {
      Console.WriteLine("--- Генерация подарка по количеству конфет ---");
      Console.Write("Введите количество конфет: ");

      if (!int.TryParse(Console.ReadLine(), out int count) || count <= 0) {
         Console.WriteLine("[Ошибка]: Некорректный ввод. Установлено значение по умолчанию: 10.");
         count = 10;
      }

      var tempCandies = new List<Candy>();
      for (int i = 0; i < count; i++) {
         tempCandies.Add(GiftDataGenerator.GenerateRandomCandy());
      }
      _lastGeneratedGift = new Gift(tempCandies);
      SaveAndReportBox("gift_by_count_report.txt");
   }

   private static void ExecuteGiftByWeightGeneration() {
      Console.WriteLine("--- Генерация подарка по целевому весу ---");
      Console.Write("Введите желаемый вес подарка (в граммах): ");

      if (!double.TryParse(Console.ReadLine(), out double targetWeight) || targetWeight <= 50) {
         Console.WriteLine("[Ошибка]: Некорректный вес. Установлено значение по умолчанию: 300 г.");
         targetWeight = 300.0;
      }

      var gift = new Gift(targetWeight);

      while (true) {
         var candy = GiftDataGenerator.GenerateRandomCandy();
         if (candy.Weight > gift.RemainingWeight) { break; }

         gift.Add(candy);

         if (gift.RemainingWeight <= 0) { break; }
      }

      _lastGeneratedGift = gift;
      SaveAndReportBox("gift_by_weight_report.txt");
   }

   private static void ExecuteHalalCheck() {
      Console.WriteLine("--- Проверка последнего набора на халяльность ---");

      if (_lastGeneratedGift == null || !_lastGeneratedGift.Items.Any()) {
         Console.WriteLine("[Информация]: Сначала сгенерируйте подарочный набор (пункты 2 или 3).");
         return;
      }

      bool isHalal = true;
      foreach (var candy in _lastGeneratedGift) {
         if (candy is JellyCandy jelly && !jelly.IsHalal) {
            isHalal = false;
            break;
         }
      }

      Console.WriteLine($"Количество конфет в последнем наборе: {_lastGeneratedGift.Items.Count}");
      if (isHalal) {
         Console.ForegroundColor = ConsoleColor.Green;
         Console.WriteLine("[Результат]: Последний набор СООТВЕТСТВУЕТ стандартам халяль.");
         Console.ResetColor();
      }
      else {
         Console.ForegroundColor = ConsoleColor.Red;
         Console.WriteLine("[Результат]: !!!!!!Последний набор НЕ СООТВЕТСТВУЕТ стандартам халяль!!!!!!!");
         Console.ResetColor();
      }
   }

   private static void ExecuteFindBySugarContent() {
      Console.WriteLine("--- Поиск конфет в последнем наборе по диапазону сахара ---");

      if (_lastGeneratedGift == null || !_lastGeneratedGift.Items.Any()) {
         Console.WriteLine("[Информация]: Сначала сгенерируйте подарочный набор (пункты 2 или 3).");
         return;
      }

      Console.Write("Введите минимальный процент сахара (например, 5.0): ");
      if (!double.TryParse(Console.ReadLine(), out double minSugar)) {
         Console.WriteLine("[Ошибка]: Некорректный ввод минимального значения.");
         return;
      }
      double maxSugar;
      do {
         Console.Write("Введите максимальный процент сахара (например, 25.0): ");
         if (!double.TryParse(Console.ReadLine(), out maxSugar )) {
            Console.WriteLine("[Ошибка]: Некорректный ввод максимального значения.");
            return;
         }
      } while (maxSugar < minSugar);

      try {
         var foundCandies = _lastGeneratedGift.FindBySugarContent(minSugar, maxSugar).ToList();

         Console.WriteLine($"\nРезультаты поиска в диапазоне от {minSugar:F1}% до {maxSugar:F1}% (найдено: {foundCandies.Count}):");
         if (foundCandies.Count == 0) {
            Console.WriteLine("  Конфет, удовлетворяющих условию, не найдено.");
         }
         else {
            for (int i = 0; i < foundCandies.Count; i++) {
               Console.WriteLine($"  [{i + 1,2}] {foundCandies[i].GetInfo()}");
            }
         }
      }
      catch (ArgumentException ex) {
         Console.ForegroundColor = ConsoleColor.Red;
         Console.WriteLine($"[Ошибка валидации]: {ex.Message}");
         Console.ResetColor();
      }
   }

   private static void SaveAndReportBox(string fileName) {
      if (_lastGeneratedGift == null)
         return;

      double totalSugar = 0;
      var sb = new StringBuilder();

      sb.AppendLine("==================================================");
      sb.AppendLine("               ОТЧЕТ ПОДАРКА SWEETBOX             ");
      sb.AppendLine("==================================================");
      sb.AppendLine($"Всего конфет: {_lastGeneratedGift.Items.Count}");
      sb.AppendLine("--------------------------------------------------");

      int index = 1;
      foreach (var candy in _lastGeneratedGift) {
         totalSugar += candy.SugarContent;
         sb.AppendLine($"[{index++,2}] {candy.GetInfo()}");
      }

      sb.AppendLine("--------------------------------------------------");
      sb.AppendLine($"Общий вес подарка: {_lastGeneratedGift.CalculateTotalWeight():F2} г");
      sb.AppendLine($"Среднее содержание сахара: {(_lastGeneratedGift.Items.Count > 0 ? totalSugar / _lastGeneratedGift.Items.Count : 0):F1}%");
      sb.AppendLine("==================================================");

      File.WriteAllText(fileName, sb.ToString());

      Console.WriteLine($"\n[Успех]: Набор успешно сформирован и записан в файл: {fileName}");
      Console.WriteLine($"Сгенерировано конфет: {_lastGeneratedGift.Items.Count}");
      Console.WriteLine($"Общий вес: {_lastGeneratedGift.CalculateTotalWeight():F2} г");
   }

   private static void PrintCandyDetails(Candy candy) {
      Console.WriteLine($" {candy.GetInfo()}");
   }
}