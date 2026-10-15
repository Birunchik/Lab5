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
         string halalInfo = IsHalal ? "халяль" : "не халяль";
         return $"Мармеладная конфета: '{Name}', Статус: {halalInfo}, Размер: {Size}, Вес: {Weight}г, Сахар: {SugarContent}%";
      }

      public override string ToString() {
         return GetInfo();
      }
   }
}