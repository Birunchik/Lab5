namespace SweetBox.CandyClasses {
   public class Lollipop : Candy {
      private bool _isLollipopOnStick;
      private string _flavor;

      public bool IsLollipopOnStick {
         get => _isLollipopOnStick;
         set => _isLollipopOnStick = value;
      }

      public string Flavor {
         get => _flavor;
         set {
            if (string.IsNullOrWhiteSpace(value)) {
               throw new System.ArgumentException("Вкус не может быть пустым.", nameof(value));
            }
            _flavor = value;
         }
      }

      public Lollipop(string name, double weight, double sugarContent, CandySize size, bool isLollipopOnStick, string flavor)
          : base(name, weight, sugarContent, size) {
         IsLollipopOnStick = isLollipopOnStick;
         Flavor = flavor;
      }

      public override string GetInfo() {
         string stickInfo = IsLollipopOnStick ? "на палочке" : "без палочки";
         return $"Название: '{Name}', Размер: {Size}, Вес: {Weight:F2}г, Сахар: {SugarContent:F2}%, Вкус: {Flavor}, Исполнение: {stickInfo}";
      }

      public override string ToString() {
         return GetInfo();
      }
   }
}