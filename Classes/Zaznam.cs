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
   /// Třída reprezentující určitou finanční transakci (příjem/výdaj). 
   /// Uchovává v sobě potřebná data včetně seznamu položek.
   /// </summary>
   public class Zaznam
   {
      /// <summary>
      /// Datum vytvoření záznamu.
      /// </summary>
      public DateTime DatumZapisu { get; set; }

      /// <summary>
      /// Datum uskutečnění transakce v tomto záznamu. 
      /// Jedná se o datum koupě/prodeje produktů.
      /// </summary>
      public DateTime Datum { get; set; }

      /// <summary>
      /// Název vytvořeného záznamu
      /// </summary>
      public string Nazev { get; set; }

      /// <summary>
      /// Celková částka příjmu nebo výdaje v daném záznamu.
      /// </summary>
      public double Hodnota_PrijemVydaj { get; set; }
     
      /// <summary>
      /// Poznámka k záznamu
      /// </summary>
      public string Poznamka { get; set; }
            
      /// <summary>
      /// Kategorie do které záznam spadá.
      /// </summary>
      public Kategorie Kategorie { get; set; }

      /// <summary>
      /// Výčtový typ k rozdělení záznamů na příjmy a výdaje.
      /// </summary>
      public KategoriePrijemVydaj PrijemNeboVydaj { get; set; }
      
      /// <summary>
      /// Seznam položek v daném záznamu o změně financí
      /// </summary>
      public ObservableCollection<Polozka> SeznamPolozek { get; set; }


      /// <summary>
      /// Konstruktor třídy pro vytvoření nového záznamu s nastavením všech parametrů předaných v parametru.
      /// </summary>
      /// <param name="Nazev">Název záznamu</param>
      /// <param name="Datum">Datum záznamu</param>
      /// <param name="Hodnota">Celkový příjem/výdaj</param>
      /// <param name="PrijemNeboVydaj">Rozdělení záznamu na příjem a výdaj</param>
      /// <param name="Poznamka">Textová poznámka</param>
      /// <param name="kategorie">Kategorie záznamu</param>
      /// <param name="SeznamPolozek">Kolekce položek</param>
      public Zaznam(string Nazev, DateTime Datum, double Hodnota, KategoriePrijemVydaj PrijemNeboVydaj, string Poznamka, Kategorie kategorie, ObservableCollection<Polozka> SeznamPolozek)
      {      
         // Načtení hodnot z parametru do interních proměnných
         DatumZapisu = DateTime.Now;               // Datum zápisu je aktuální datum při vytvoření záznamu
         this.Nazev = Nazev;
         this.Poznamka = Poznamka;
         this.SeznamPolozek = SeznamPolozek;
         this.Hodnota_PrijemVydaj = Hodnota;
         this.PrijemNeboVydaj = PrijemNeboVydaj;
         this.Datum = Datum;
         this.Kategorie = kategorie;   
      }

      /// <summary>
      /// Bezparametrický konstruktor třídy pro možnost ukládání dat do souboru
      /// </summary>
      public Zaznam()
      {

      }


      /// <summary>
      /// Metoda pro vytvoření textového řetězce obsahující veškeré potřebné údaje o záznamu pro možnost výpisu.
      /// </summary>
      /// <returns>Textový řetězec obsahující potřebné údaje</returns>
      public override string ToString()
      {
         // Textový řetězec pro uložení parametrů záznamu
         string Zaznam = "";

         Zaznam += Nazev + "; ";
         Zaznam += "vytvořen " + Datum.ToString("dd.MM.yyyy");
         Zaznam += ". hodnota: " + Hodnota_PrijemVydaj+" Kč \n";

         // Vypsání všech položek do textového řetězce
         foreach (Polozka p in SeznamPolozek)
         {
            Zaznam += p + "; ";
         }
         return Zaznam;
      }
   }
}
