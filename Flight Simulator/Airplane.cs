using System;
using System.Collections.Generic;

namespace Flight_Simulator
{
	// В приложении должен быть реализован класс «Самолет»
	class Airplane
	{
		private List<Dispatcher> dispatchers;// Список текущих диспетчеров
		private int currentSpeed;// Текущая скорость
		private int currentHeight;// Текущая Высота
		private int totalPenalty;// Общая сумма штрафных очков
		private bool IsSpeedGained;// Показывает, набрана ли максимальная скорость
		private bool IsFlyBegin;// Показывает, начался ли полет

		private delegate void ChangeDelegate(int speed, int height);
		private event ChangeDelegate ChangeEvent;

		public Airplane()
		{
			dispatchers = new List<Dispatcher>();
			currentSpeed = 0;
			currentHeight = 0;
			totalPenalty = 0;
			IsSpeedGained = false;
			IsFlyBegin = false;
		}
		// Функция для добавления диспечтера
		public void AddDispatcher(string name)
		{
			Dispatcher d = new Dispatcher(name);
			ChangeEvent += d.RecomendedHight;// Подписка на событие
			dispatchers.Add(d);// Добавение в список
			Console.WriteLine($"Диспетчер {name} добавлен!\a");
		}
		// Функция для удаление диспетчера
		public void DeleteDispatcher(int position)
		{
			if (dispatchers.Count == 0)// Если в списке пусто
			{
				Console.WriteLine("Сначала добавьте диспетчера!");
				Console.Beep();
			}
			else if (position == -1)
			{
				Console.WriteLine("Отмена.\a");
				return;
			}
			else if (position >= 0 && position <= dispatchers.Count - 1)
			{
				ChangeEvent -= dispatchers[position].RecomendedHight;// Отписка от события
				Console.WriteLine($"Диспетчер {dispatchers[position].Name} удален!\a");
				totalPenalty += dispatchers[position].Penalty;// Сохранение штрафных очков, полученных от удаляемого диспетчера
				dispatchers.RemoveAt(position);// Удаление из списка
			}
			else
			{
				Console.WriteLine("Такого диспетчера не существует!");
				Console.Beep();
			}
		}
		// Функция для вывода всех диспетчеровна экран
		public void PrintDispatchers()
		{
			Console.WriteLine();
			Console.WriteLine("0. Отмена");
			foreach (Dispatcher i in dispatchers)
				Console.WriteLine($"{dispatchers.IndexOf(i) + 1}. {i.Name}");
		}

		public void Fly()
		{
			Console.WriteLine("Управление:\n+(плюс) - добавить нового диспетчера,\n-(минус) - удалить выбраного диспетчера,\n\nRightArrow - увеличить скорость самолета на 50,\nLeftArrow - уменьшить скорость самолета на 50,\nShift + RightArrow - увеличить скорость самолета на 150,\nShift + LeftArrow - уменьшить скорость самолета на 150,\n\nUpArrow - увеличить высоту самолета на 250,\nDownArrow - уменьшить высоту самолета на 250,\nShift + UpArrow - увеличить высоту самолета на 500,\nShift + DownArrow - уменьшить высоту самолета на 500.\n");
			Console.WriteLine("Задача пилота — взлететь на самолете, набрать максимальную (1000 км/ч.) скорость, а затем посадить самолет.");

			ConsoleKeyInfo key;

			while (true)
			{
				key = Console.ReadKey();

				if ((key.Modifiers & ConsoleModifiers.Shift) != 0)
				{
					if (key.Key == ConsoleKey.RightArrow) currentSpeed += 150;
					else if (key.Key == ConsoleKey.LeftArrow) currentSpeed -= 150;
					else if (key.Key == ConsoleKey.UpArrow) currentHeight += 500;
					else if (key.Key == ConsoleKey.DownArrow) currentHeight -= 500;
				}
				else
				{
					if (key.Key == ConsoleKey.RightArrow) currentSpeed += 50;
					else if (key.Key == ConsoleKey.LeftArrow) currentSpeed -= 50;
					else if (key.Key == ConsoleKey.UpArrow) currentHeight += 250;
					else if (key.Key == ConsoleKey.DownArrow) currentHeight -= 250;
					else if (key.Key == ConsoleKey.OemPlus || key.Key == ConsoleKey.Add)
					{
						Console.Write($"\nВведите имя диспетчера: ");
						AddDispatcher(Console.ReadLine());
					}
					else if (key.Key == ConsoleKey.OemMinus || key.Key == ConsoleKey.Subtract)
					{
						PrintDispatchers();
						Console.Write($"Введите номер диспетчера, которого хотите удалить: ");
						DeleteDispatcher(Convert.ToInt32(Console.ReadLine()) - 1);
					}
				}
				if (dispatchers.Count >= 2 && currentSpeed >= 50)// Управление самолетом диспетчерами начинается
				{
					Console.WriteLine();
					if (!IsFlyBegin)// Оповещение о начале полета
						Console.WriteLine("Полет начался!\a");
					IsFlyBegin = true;

					// В процессе полета самолет автоматически сообщает
					// всем диспетчерам все изменения в скорости
					// и высоте полета с помощью делегатов
					ChangeEvent(currentSpeed, currentHeight);
					if (currentSpeed == 1000)
					{
						IsSpeedGained = true;
						Console.WriteLine("\nВы набрали максимальную скорость. Ваша задача - посадить самолет!\a");
					}
					else if (IsSpeedGained && currentSpeed <= 50)// Управление самолетом диспетчерами прекращается
					{
						Console.WriteLine("\nПолет закончился!\a");
						// Перебор всех диспетчеров в коллекции и суммирование 
						// всех штрафныех очков в общую сумму
						foreach (Dispatcher i in dispatchers)
						{
							totalPenalty += i.Penalty;
							Console.WriteLine($"{i.Name}: {i.Penalty}");
						}

						Console.WriteLine($"Сумарное число штрафных очков: {totalPenalty}\a");

						break;// Выход из цикла
					}
				}
				Console.WriteLine($"Скорость: {currentSpeed} км/ч Высота: {currentHeight} м");
			}
		}
	}
}