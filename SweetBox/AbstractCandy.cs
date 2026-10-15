namespace SweetBox {
   public enum CandySize {
      Small,
      Medium,
      Large
   }

   public abstract class Candy {
      private string _name;
      private double _weight;
      private double _sugarContent;
      private CandySize _size;

      public string Name {
         get => _name;
         set {
            if (string.IsNullOrWhiteSpace(value)) {
               throw new System.ArgumentException("Название не может быть пустым.", nameof(value));
            }
            _name = value;
         }
      }

      public CandySize Size {
         get => _size;
         set {
            _size = value;
            // Принудительно проверяем текущий вес при смене размера, 
            // чтобы он не выходил за допустимые рамки нового размера
            Weight = _weight;
         }
      }

      public double Weight {
         get => _weight;
         set {
            ValidateWeight(value, _size);
            _weight = value;
         }
      }

      public double SugarContent {
         get => _sugarContent;
         set {
            if (value < 0) {
               throw new System.ArgumentOutOfRangeException(nameof(value), "Содержание сахара не может быть отрицательным.");
            }
            _sugarContent = value;
         }
      }

      protected Candy(string name, double weight, double sugarContent, CandySize size) {
         _size = size;
         Name = name;
         Weight = weight;
         SugarContent = sugarContent;
      }

      private void ValidateWeight(double weight, CandySize size) {
         (double min, double max) range = size switch{
            CandySize.Small => (1.0, 15.0),
            CandySize.Medium => (15.01, 50.0),
            CandySize.Large => (50.01, 150.0),
            _ => (0.1, 1000.0)
         };

         if (weight < range.min || weight > range.max) {
            throw new System.ArgumentOutOfRangeException(
                nameof(weight),
                $"Недопустимый вес ({weight}г) для размера {size}. Допустимый диапазон: от {range.min}г до {range.max}г."
            );
         }
      }

      public abstract string GetInfo();

      public override string ToString() {
         return $"Конфета: '{Name}', Размер: {Size}, Вес: {Weight}г, Сахар: {SugarContent}%";
      }
   }
}