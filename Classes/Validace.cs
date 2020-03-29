using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows;

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
   /// <summary>
   /// Třída slouží pro validaci vstupních dat. 
   /// Metody kontrolují správnost zadaných údajů od uživatele a převádí vstupní data do potřebného formátu za účelem snadného zpracování programem.
   /// </summary>
   public class Validace
   {
      /// <summary>
      /// Převedení čísla zadaného uživatelem do potřebného formátu pro možnost následného zpracování.
      /// </summary>
      /// <param name="ZadanyText">Textová reprezentace čísla</param>
      /// <returns>Číslo v potřebném formátu</returns>
      public double NactiCislo(string ZadanyText)
      {
         // Interní proměnná pro uložení načteného čísla
         double Cislo;

         // Kontrola zda bylo zadáno číslo
         while (!double.TryParse(ZadanyText, out Cislo))
            throw new ArgumentException("Zadali jste číslo ve špatném formátu!");

         // Návratová hodnota
         return Cislo;
      }

      /// <summary>
      /// Metoda pro načtení data zadaného jako vstupní data od uživatele do potřebného formátu pro následné zpracování.
      /// </summary>
      /// <param name="Datum">Datum zadané od uživatele</param>
      /// <returns>Datum v potřebném formátu</returns>
      public DateTime NactiDatum(DateTime? Datum)
      {
         // Kontrola zda bylo zadání datum
         if (Datum == null)
            throw new ArgumentException("Nebylo zadáno datum!");

         // Kontrola zda se nejedná o budoucí datum
         if (Datum.Value.Date > DateTime.Now)
            MessageBox.Show("Zadáváte budoucí datum", "Upozornění", MessageBoxButton.OK, MessageBoxImage.Information);

         // Navrácení data bez složky uchovávající informace o čase
         return Datum.Value.Date;
      }

      /// <summary>
      /// Metoda pro rozdělení předaného textového řetězce dle předaných separátorů. 
      /// Oddělené části textu jsou uloženy do pole textových řetězců.
      /// </summary>
      /// <param name="Text">Textový řetězec pro rozdělení</param>
      /// <param name="OddelovaciZnaky">Pole znaků sloužící pro oddělení částí textu</param>
      /// <returns>Pole textových řetězců oddělených zadanými separátory</returns>
      public string[] RozdelText(string Text, char[] OddelovaciZnaky)
      {
         string[] PoleOddelenychSlov = Text.Split(OddelovaciZnaky);
         return PoleOddelenychSlov;
      }

      /// <summary>
      /// Metoda pro složení slov do věty. 
      /// V parametru je předáno pole textových řetězců, které se postupně vkládají do jediného textového řetězce s mezerou jako oddělením jednotlivých slov.
      /// </summary>
      /// <param name="Slova">Pole textových řetězců</param>
      /// <returns>Textový řetězec obsahující všechny řetězce předané v parametru</returns>
      public string SlozSlovaDoTextu(string[] Slova)
      {
         return string.Join(" ", Slova);
      }

      /// <summary>
      /// Smazání posledního znaku v řetězci
      /// </summary>
      /// <param name="Zprava">Text ke zpracování</param>
      /// <returns>Upravený text</returns>
      public string SmazPosledniZnak(string Zprava)
      {
         string UpravenaZprava = Zprava;
         UpravenaZprava = UpravenaZprava.Remove(UpravenaZprava.Length - 1);
         return UpravenaZprava;
      }

   }
}
