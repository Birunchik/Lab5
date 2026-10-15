using SweetBox.CandyBox;
using SweetBox.CandyClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

class Program {
   private static bool _isRunning = true;
   private static List<Candy> _lastGeneratedBox = new();

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
      Console.WriteLine("5. Выход");
      Console.WriteLine("--------------------------------------------------");
      Console.Write("Выберите пункт меню (1-5): ");
   }

   private static void ProcessUserChoice() {
      string choice = Console.ReadLine();
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
               _isRunning = false;
               Console.WriteLine("Завершение работы программы.");
               break;
            default:
               Console.WriteLine("[Ошибка]: Неверный выбор. Введите число от 1 до 5.");
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

   private static void ExecuteGiftByCountGeneration() {
      Console.WriteLine("--- Генерация подарка по количеству конфет ---");
      Console.Write("Введите количество конфет: ");

      if (!int.TryParse(Console.ReadLine(), out int count) || count <= 0) {
         Console.WriteLine("[Ошибка]: Некорректный ввод. Установлено значение по умолчанию: 10.");
         count = 10;
      }

      _lastGeneratedBox.Clear();
      for (int i = 0; i < count; i++) {
         _lastGeneratedBox.Add(GiftDataGenerator.GenerateRandomCandy());
      }

      SaveAndReportBox("gift_by_count_report.txt");
   }

   private static void ExecuteGiftByWeightGeneration() {
      Console.WriteLine("--- Генерация подарка по целевому весу ---");
      Console.Write("Введите желаемый вес подарка (в граммах): ");

      if (!double.TryParse(Console.ReadLine(), out double targetWeight) || targetWeight <= 50) {
         Console.WriteLine("[Ошибка]: Некорректный вес. Установлено значение по умолчанию: 300 г.");
         targetWeight = 300.0;
      }

      _lastGeneratedBox.Clear();
      double currentWeight = 0;

      // Генерируем конфеты до тех пор, пока суммарный вес не приблизится или не превысит целевой
      while (currentWeight < targetWeight) {
         var candy = GiftDataGenerator.GenerateRandomCandy();
         _lastGeneratedBox.Add(candy);
         currentWeight += candy.Weight;
      }

      SaveAndReportBox("gift_by_weight_report.txt");
   }

   private static void ExecuteHalalCheck() {
      Console.WriteLine("--- Проверка последнего набора на халяльность ---");

      if (_lastGeneratedBox.Count == 0) {
         Console.WriteLine("[Информация]: Сначала сгенерируйте подарочный набор (пункты 2 или 3).");
         return;
      }
      bool isHalal = true; 
      foreach (var candy in _lastGeneratedBox) {
         if (candy is JellyCandy jelly && !jelly.IsHalal) {
            isHalal = false;
         }
      }

      

      Console.WriteLine($"Количество конфет в последнем наборе: {_lastGeneratedBox.Count}");
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

   private static void SaveAndReportBox(string fileName) {
      double totalWeight = 0;
      double totalSugar = 0;

      var sb = new StringBuilder();
      sb.AppendLine("==================================================");
      sb.AppendLine("               ОТЧЕТ ПОДАРКА SWEETBOX             ");
      sb.AppendLine("==================================================");
      sb.AppendLine($"Всего конфет: {_lastGeneratedBox.Count}");
      sb.AppendLine("--------------------------------------------------");

      for (int i = 0; i < _lastGeneratedBox.Count; i++) {
         var candy = _lastGeneratedBox[i];
         totalWeight += candy.Weight;
         totalSugar += candy.SugarContent;

         string details = candy switch
         {
            ChocolateCandy choco => $"Шоколад | Тип: {choco.ChocolateType} | Начинка: {(choco.HasFilling ? "Да" : "Нет")}",
            Lollipop l => $"Леденец | Вкус: {l.Flavor}",
            JellyCandy j => $"Мармелад | Халяль: {(j.IsHalal ? "Да" : "Нет")}",
            _ => "Конфета"
         };

         sb.AppendLine($"[{i + 1,2}] Название: {candy.Name,-20} | Размер: {candy.Size,-6} | Вес: {candy.Weight,5:F1}г | Сахар: {candy.SugarContent,4:F1}% | {details}");
      }

      sb.AppendLine("--------------------------------------------------");
      sb.AppendLine($"Общий вес подарка: {totalWeight:F2} г");
      sb.AppendLine($"Среднее содержание сахара: {(totalSugar / _lastGeneratedBox.Count):F1}%");
      sb.AppendLine("==================================================");
      
      File.WriteAllText(fileName, sb.ToString());

      
      Console.WriteLine($"\n[Успех]: Набор успешно сформирован и записан в файл: {fileName}");
      Console.WriteLine($"Сгенерировано конфет: {_lastGeneratedBox.Count}");
      Console.WriteLine($"Общий вес: {totalWeight:F2} г");
   }

   private static void PrintCandyDetails(Candy candy) {
      string typeInfo = candy switch
      {
         JellyCandy jelly => $"Категория: Мармелад | Халяль: {(jelly.IsHalal ? "Да" : "Нет")}",
         ChocolateCandy choco => $"Категория: Шоколад | Вид: {choco.ChocolateType} | Начинка: {(choco.HasFilling ? "Да" : "Нет")}",
         Lollipop lollipop => $"Категория: Леденец | Вкус: {lollipop.Flavor}",
         _ => "Категория: Неизвестно"
      };

      Console.WriteLine($" Название: \"{candy.Name}\"");
      Console.WriteLine($" Размер:   {candy.Size}");
      Console.WriteLine($" Вес:      {candy.Weight:F2} г");
      Console.WriteLine($" Сахар:    {candy.SugarContent:F1}%");
      Console.WriteLine($" {typeInfo}");
   }
}