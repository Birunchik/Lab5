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
         return $"Леденец: '{Name}', Вкус: {Flavor}, Исполнение: {stickInfo}, Размер: {Size}, Вес: {Weight}г, Сахар: {SugarContent}%";
      }

      public override string ToString() {
         return GetInfo();
      }
   }
}