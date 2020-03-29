using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Aplikace pro správu financí určena pouze pro osobní užití.
/// Aplikace pracuje vždy pouze s daty patřící přihlášenému uživateli, 
/// avšak do souboru ukládá a při spuštění načítá veškerá data pro zpracování, tedy data všech registrovaných uživatelů.
/// V hlavním okně aplikace je zobrazen stručný přehled a je zde uživatelské rozhraní pro správu aplikace i pro možnost využití dalších funkcí aplikace pracujících v samostatných oknech.
/// 
/// Autor projektu: bc. David Halas
/// Link uložení projektu: https://github.com/dejv147/Finance-Management
/// </summary>
namespace Sprava_financi
{
   /* Tato třída byla upravena. Původní autor: bc. Miroslav Hájek. */
   /// <summary>
   /// Umožňuje získat počáteční a konečné dny všech týdnů v roce.
   /// </summary>
   public static class Tydny
   {
      /// <summary>
      /// Metoda pro výpočet prvního a posledního dne v každém týdnu v roce.
      /// </summary>
      /// <param name="month">Měsíc, pro který se stanoví roky</param>
      /// <param name="year">Rok ve kterém se daný měsíc nachází</param>
      /// <returns>Kolekci obsahující první den týdne [0,2,4,..] a poslední den týdne[1,3,5,..]</returns>
      public static List<DateTime> GetWeek(int month, int year)
      {
         // Kolekce do které se uloží první a následně poslední den týde. Takto pro celý měsíc
         List<DateTime> startEndDayWeeks = new List<DateTime>();

         // Prochází se všechny dny měsíce 
         for (int i = 1; i <= DateTime.DaysInMonth(year, month); i++)
         {
            // Zkoumaný den
            DateTime day = new DateTime(year, month, i);

            // Ošetření prvního týdne v měsíci, který většinou nezačíná 1.
            if (day.DayOfWeek != DayOfWeek.Monday && day.Day == 1)
            {
               int previousMonthDays = 0;
               int yearPrevious = year;
               int monthPrevious = month - 1;

               // Pokud je měsíc odlišný od Ledna
               if (month != 1)
                  previousMonthDays = DateTime.DaysInMonth(year, month - 1);

               // Pokud je měsícem leden, spočítá se zbytek týdne z minulého roku a prosince
               else
               {
                  previousMonthDays = DateTime.DaysInMonth(year - 1, 12);
                  yearPrevious = year - 1;
                  monthPrevious = 12;
               }

               // Procházím prosinec minulého roku a získám tak pondělí pro první týden   
               for (int j = previousMonthDays; j > 0; j--)
               {
                  DateTime dayPrevious = new DateTime(yearPrevious, monthPrevious, j);
                  if (dayPrevious.DayOfWeek == DayOfWeek.Monday)
                  {
                     startEndDayWeeks.Add(dayPrevious);
                     break;
                  }
               }

            }

            // Pokud je den pondělí, přidá se do kolekce jako začátek nového týdne
            if (day.DayOfWeek == DayOfWeek.Monday && startEndDayWeeks.Count < 10)
               startEndDayWeeks.Add(day);

            // Pokud je den neděle, přidá se do kolekce jako konec tohoto týdne    
            else if (day.DayOfWeek == DayOfWeek.Sunday && startEndDayWeeks.Count < 10)
               startEndDayWeeks.Add(day);

            // Ošetření posledního týdne v měsíci. Jelikož měsíc nekončí v neděli, a proto týden pokračuje do nového měsíce
            if (day.DayOfWeek != DayOfWeek.Sunday && day.Day == DateTime.DaysInMonth(year, month) && startEndDayWeeks.Count < 10)
            {
               int nextMonthDays = 0;
               int yearNext = year;
               int monthNext = month + 1;
               if (month != 12)
                  nextMonthDays = DateTime.DaysInMonth(year, monthNext);
               else
               {
                  nextMonthDays = DateTime.DaysInMonth(year + 1, 1);
                  yearNext = year + 1;
                  monthNext = 1;
               }

               for (int j = 1; j <= nextMonthDays; j++)
               {
                  DateTime dayNext = new DateTime(yearNext, monthNext, j);
                  if (dayNext.DayOfWeek == DayOfWeek.Sunday)
                  {
                     startEndDayWeeks.Add(dayNext);
                     break;
                  }
               }
            }

         }

         return startEndDayWeeks;
      }
   }
}
