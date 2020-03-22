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
   /// Třída reprezentující jednoho uživatele. 
   /// Uchovává veškeré informace o uživateli včetně kolekce záznamů patřící danému uživateli.
   /// </summary>
   public class Uzivatel
   {
      /// <summary>
      /// Jméno uživatele
      /// </summary>
      public string Jmeno { get; set; }

      /// <summary>
      /// Heslo uživatele
      /// </summary>
      public string Heslo { get; set; }

      /// <summary>
      /// Seznam záznamů uživatele
      /// </summary>
      public ObservableCollection<Zaznam> SeznamZaznamuUzivatele { get; set; }

      /// <summary>
      /// Text v poznámkovém bloku daného uživatele
      /// </summary>
      public string Poznamka { get; set; }

      /// <summary>
      /// Příznakový bit uchovávající informaci zda má být poznámkový blok pro uživatele zobrazen. 
      /// 0 - poznámky jsou uschovány, 1 - poznámky jsou zobrazeny
      /// </summary>
      public byte PoznamkaZobrazena { get; set; }


      /// <summary>
      /// Konstruktor třídy
      /// </summary>
      public Uzivatel(string Jmeno, string Heslo)
      {
         this.Jmeno = Jmeno;
         this.Heslo = Heslo;
         SpravceZaznamu spravceZaznamu = new SpravceZaznamu();
         SeznamZaznamuUzivatele = spravceZaznamu.SeznamTransakci;
         Poznamka = "";
         PoznamkaZobrazena = 0;
      }

      /// <summary>
      /// Bezparametrický konstruktor třídy pro možnost ukládání dat do souboru
      /// </summary>
      public Uzivatel()
      {

      }

   }
}
