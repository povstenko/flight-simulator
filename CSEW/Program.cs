//
//НА ДАНЫЙ МОМЕНТ Я НЕ ЗНАЮ КАК СДЕЛАТЬ УПРАВЛЕНИЕ С ПОМОЩЬЮ Shift, ПОЭТОМУ ВРЕМЕННО СДЕЛАЛ УПРАВЛЕНИЕ СТРЕЛКАМИ И WASD
//
//
//
//
using System;
using System.Collections.Generic;

namespace CSEW
{
	class Dispatcher
	{
		private string name;
		public string Name { get { return name; } }
		
		private int adjustment;// корректировка погодных условий
		private int penalty;// штрафные очки
		public int Penalty { get { return penalty; } }

		private static Random r;

		public Dispatcher(string name)
		{
			this.name = name;
			r = new Random ();
			adjustment = r.Next(-200, 200);
			penalty = 0;
		}

		public void RecomendedHight(int speed, int height)
		{
			int recomended = 7*speed - adjustment;
			int difference = height - recomended;
			Console.WriteLine($"Диспетчер {name}: Рекомендуемая высота полета: {recomended} м.");

			if(speed > 1000)
			{
				penalty += 100;
				Console.WriteLine($"Диспетчер {name}: Немедленно снизьте скорость!");
			}

			if(difference >= 300 && difference < 600) penalty += 25;
			else if(difference >= 600 && difference < 1000) penalty += 50;
			else if(difference >= 1000 || (speed <= 0 && height <= 0))
				throw new Exception("Самолет разбился");

			if(penalty >= 1000)
				throw new Exception("Непригоден к полетам");
		}
	}

	class Airplane
	{
		private List<Dispatcher> dispatchers;
		private int currentSpeed;
		private int currentHeight;
		private bool IsMissionComplete;

		private delegate void ChangeDelegate(int speed, int height);
		private event ChangeDelegate changeEvent;

		public Airplane()
		{
			dispatchers = new List<Dispatcher> ();
			currentSpeed = 0;
			currentHeight = 0;
			IsMissionComplete = false;
		}

		public void AddDispatcher(string name)
		{
			Dispatcher d = new Dispatcher(name);
			changeEvent += d.RecomendedHight;
			dispatchers.Add(d);
			Console.WriteLine ($"Диспетчер {name} добавлен!");
		}
		public void DeleteDispatcher(int position)
		{
			if(dispatchers.Count == 0)
			{
				Console.WriteLine ("Сначала добавьте диспетчера!");
			}
			else if (position >= 0 && position <= dispatchers.Count-1)
			{
				changeEvent -= dispatchers[position].RecomendedHight;
				Console.WriteLine ($"Диспетчер {dispatchers[position].Name} удален!");
				dispatchers.RemoveAt (position);
			}
			else
				Console.WriteLine ("Такого диспетчера не существует!");
		}
		public void PrintDispatchers()
		{
			Console.WriteLine ();
			foreach (Dispatcher i in dispatchers) {
				Console.WriteLine (dispatchers.IndexOf(i) + ". " + i.Name);
			}
		}

		public void Fly()
		{
			//Console.WriteLine ("Управление:\nInsert - добавить нового диспетчера,\nDelete - удалить выбраного диспетчера,\n\nRightArrow - увеличить скорость самолета на 50,\nLeftArrow - уменьшить скорость самолета на 50,\nShift + RightArrow - увеличить скорость самолета на 150,\nShift + LeftArrow - уменьшить скорость самолета на 150,\n\nUpArrow - увеличить высоту самолета на 250,\nDownArrow - уменьшить высоту самолета на 250,\nShift + UpArrow - увеличить высоту самолета на 500,\nShift + DownArrow - уменьшить высоту самолета на 500.");
			
			ConsoleKeyInfo key;

			while(true)
			{
				key = Console.ReadKey();

				if(key.Key == ConsoleKey.RightArrow) currentSpeed += 50;
				else if(key.Key == ConsoleKey.LeftArrow) currentSpeed -= 50;
				else if(key.Key == ConsoleKey.D) currentSpeed += 150;
				else if(key.Key == ConsoleKey.A) currentSpeed -= 150;
				else if(key.Key == ConsoleKey.UpArrow) currentHeight += 250;
				else if(key.Key == ConsoleKey.DownArrow) currentHeight -= 250;
				else if(key.Key == ConsoleKey.W) currentHeight += 500;
				else if(key.Key == ConsoleKey.S) currentHeight -= 500;
				
				if(key.Key == ConsoleKey.Insert)
				{
					Console.Write ($"\nВведите имя диспетчера: ");
					AddDispatcher(Console.ReadLine());
				}
				else if(key.Key == ConsoleKey.Delete)
				{
					PrintDispatchers();
					Console.Write ($"Введите номер диспетчера, которого хотите удалить: ");
					DeleteDispatcher(Convert.ToInt32(Console.ReadLine()));
				}
				else
				{
					if (dispatchers.Count >= 2 && currentSpeed >= 50)// fly begin
					{
						Console.WriteLine ();
						if(currentSpeed == 50)Console.WriteLine ("Полет начался!");

						changeEvent (currentSpeed, currentHeight);
						Console.WriteLine ($"Скорость: {currentSpeed} км/ч Высота: {currentHeight} м");
						if (currentSpeed == 1000)
						{
							IsMissionComplete = true;
							Console.WriteLine ("\nВы набрали максимальную скорость. Ваша задача - посадить самолет!");
						}
						if (IsMissionComplete && currentSpeed <= 50)//fly end
						{
							Console.WriteLine ("\nПолет закончился!");
							int totalPenalty = 0;
							foreach (Dispatcher i in dispatchers) {
								totalPenalty += i.Penalty;
							}
							Console.WriteLine ($"Сумарное число штрафных очков: {totalPenalty}");
						}
					}
					else
					{
						Console.WriteLine ($"Скорость: {currentSpeed} км/ч Высота: {currentHeight} м");
					}
				}
			}
		}
	}

	class MainClass
	{
		public static void Main (string[] args)
		{
			try
			{
				Airplane plane = new Airplane();
				plane.Fly();
			}
			catch(Exception ex)
			{
				Console.WriteLine (ex.Message);
			}
		}
	}
}
// CSharpExamWorkByVitalyPovstenko(16.06.2018);