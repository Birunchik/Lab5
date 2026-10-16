namespace SweetBox.CandyClasses {
   public class JellyCandy : Candy {
      private bool _isHalal;

      public bool IsHalal {
         get => _isHalal;
         set => _isHalal = value;
      }

      public JellyCandy(string name, double weight, double sugarContent, CandySize size, bool isHalal)
          : base(name, weight, sugarContent, size) {
         IsHalal = isHalal;
      }

      public override string GetInfo() {
         string halalInfo = IsHalal ? "Халяль" : "Не халяль";
         return $"Название: '{Name}', Размер: {Size}, Вес: {Weight:F2}г, Сахар: {SugarContent:F2}%, Статус: {halalInfo}";
      }

      public override string ToString() {
         return GetInfo();
      }
   }
}