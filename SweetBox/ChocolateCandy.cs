namespace SweetBox {
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
         return $"Шоколадная конфета: '{Name}', Тип шоколада: {ChocolateType}, Начинка: {fillingInfo}, Размер: {Size}, Вес: {Weight}г, Сахар: {SugarContent}%";
      }

      public override string ToString() {
         return GetInfo();
      }
   }
}