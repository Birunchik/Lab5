namespace SweetBox.CandyClasses {
   public enum ChocolateType {
      Dark,
      Milk,
      White
   }

   public class ChocolateCandy : Candy {
      private ChocolateType _chocolateType;
      private bool _hasFilling;

      public ChocolateType ChocolateType {
         get => _chocolateType;
         set => _chocolateType = value;
      }

      public bool HasFilling {
         get => _hasFilling;
         set => _hasFilling = value;
      }

      public ChocolateCandy(string name, double weight, double sugarContent, CandySize size, ChocolateType chocolateType, bool hasFilling)
          : base(name, weight, sugarContent, size) {
         ChocolateType = chocolateType;
         HasFilling = hasFilling;
      }

      public override string GetInfo() {
         string fillingInfo = HasFilling ? "с начинкой" : "без начинки";
         return $"Название: '{Name}', Размер: {Size}, Вес: {Weight:F2}г, Сахар: {SugarContent:F2}%, Тип шоколада: {ChocolateType}, Начинка: {fillingInfo}";
      }

      public override string ToString() {
         return GetInfo();
      }
   }
}