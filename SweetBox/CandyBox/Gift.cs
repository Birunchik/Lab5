using SweetBox.CandyClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SweetBox {
   public enum SortCriterion {
      Name,
      Weight,
      SugarContent,
      Size
   }

   public class Gift : IEnumerable<Candy> {
      private readonly List<Candy> _items;
      private readonly double _targetWeight;
      private double _remainingWeight;

      public IReadOnlyCollection<Candy> Items => _items.AsReadOnly();

      public double TargetWeight => _targetWeight;

      public double RemainingWeight => _remainingWeight;

      public Gift(double targetWeight) {
         if (targetWeight <= 0) {
            throw new ArgumentOutOfRangeException(nameof(targetWeight));
         }

         _targetWeight = targetWeight;
         _remainingWeight = targetWeight;
         _items = new List<Candy>();
      }

      public Gift(IEnumerable<Candy> candies) {
         if (candies == null) {
            throw new ArgumentNullException(nameof(candies));
         }

         _items = new List<Candy>(candies);

         if (_items.Count == 0) {
            throw new ArgumentException("Коллекция конфет не может быть пустой.", nameof(candies));
         }

         _targetWeight = _items.Sum(c => c.Weight);
         _remainingWeight = 0;
      }

      public void Add(Candy candy) {
         if (candy == null) {
            throw new ArgumentNullException(nameof(candy));
         }

         if (candy.Weight > _remainingWeight) {
            throw new InvalidOperationException($"Нельзя добавить конфету. Ее вес ({candy.Weight}г) превышает оставшийся вес подарка ({_remainingWeight}г).");
         }

         _items.Add(candy);
         _remainingWeight -= candy.Weight;
      }

      public bool Remove(Candy candy) {
         if (candy == null) {
            return false;
         }

         bool removed = _items.Remove(candy);
         if (removed) {
            _remainingWeight += candy.Weight;
         }
         return removed;
      }

      public double CalculateTotalWeight() {
         return _targetWeight - _remainingWeight;
      }

      public IEnumerable<Candy> FindBySugarContent(double minSugar, double maxSugar) {
         if (minSugar < 0 || maxSugar < 0 || minSugar > maxSugar) {
            throw new ArgumentException("Некорректно задан диапазон содержания сахара.");
         }

         return _items.Where(c => c.SugarContent >= minSugar && c.SugarContent <= maxSugar);
      }

      public void SortBy(SortCriterion criterion) {
         _items.Sort((x, y) => criterion switch
         {
            SortCriterion.Name => string.Compare(x.Name, y.Name, StringComparison.Ordinal),
            SortCriterion.Weight => x.Weight.CompareTo(y.Weight),
            SortCriterion.SugarContent => x.SugarContent.CompareTo(y.SugarContent),
            SortCriterion.Size => x.Size.CompareTo(y.Size),
            _ => throw new ArgumentOutOfRangeException(nameof(criterion), criterion, "Неизвестный критерий сортировки.")
         });
      }

      public IEnumerator<Candy> GetEnumerator() {
         return _items.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator() {
         return GetEnumerator();
      }

      public override string ToString() {
         if (_items.Count == 0) {
            return $"Подарок пуст. Целевой вес: {_targetWeight}г, Оставшийся вес: {_remainingWeight}г.";
         }

         var info = $"Новогодний подарок (всего сладостей: {_items.Count}, текущий вес: {CalculateTotalWeight()}г из {_targetWeight}г, остаток: {_remainingWeight}г):\n";
         foreach (var candy in _items) {
            info += $" - {candy.GetInfo()}\n";
         }
         return info;
      }
   }
}