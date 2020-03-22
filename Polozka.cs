using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
   /// Třída uchovávající informace o konkrétní položce.
   /// </summary>
   public class Polozka
   {
      /// <summary>
      /// Název vytvořené položky
      /// </summary>
      public string Nazev { get; set; }

      /// <summary>
      /// Hodnota vytvořené položky
      /// </summary>
      public double Cena { get; set; }

      /// <summary>
      /// Textový popis vytvořené položky
      /// </summary>
      public string Popis { get; set; }

      /// <summary>
      /// Kategorie do které položka spadá.
      /// </summary>
      public Kategorie KategoriePolozky { get; set; }



      /// <summary>
      /// Konstruktor třídy pro vytvoření nové položky s nastavením všech parametrů předaných v konstruktoru.
      /// </summary>
      /// <param name="Nazev">Název položky</param>
      /// <param name="Cena">Hodnota položky</param>
      /// <param name="kategorie">Kategorie položky</param>
      /// <param name="popis">Textový popis položky</param>
      public Polozka(string Nazev, double Cena, Kategorie kategorie, string popis)
      {
         this.Nazev = Nazev;
         this.Cena = Cena;
         this.KategoriePolozky = kategorie;
         this.Popis = popis;
      }


      /// <summary>
      /// Bezparametrický konstruktor třídy pro možnost ukládání dat do souboru
      /// </summary>
      public Polozka()
      {

      }


      /// <summary>
      /// Vrátí textový řetězec s názvem položky a její cenou.
      /// </summary>
      /// <returns>Textový řetězec</returns>
      public override string ToString()
      {
         if (Popis.Length > 0) 
            return String.Format("{0} ({1}): {2} Kč", Nazev, Popis, Cena);
         else
            return String.Format("{0}: {1} Kč", Nazev, Cena);
      }

   }
}
