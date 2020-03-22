using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

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
   /// Třída pro vyhledávání konkrétních záznamů dle požadovaných filtračních parametrů. 
   /// Kolekce ve které se záznamy hledají je předávána vždy v parametru příslušné funkce.
   /// Pro vyhledávání jsou používány LINQ dotazy a návratová kolekce je vždy přetypována na původní datový typ, tedy Zaznam.
   /// </summary>
   class Vyhledavani
   {
      /// <summary>
      /// Kolekce všech záznamů, ve které se vyhledávají potřebná data.
      /// </summary>
      public ObservableCollection<Zaznam> VsechnyZaznamy { get; private set; }


      /// <summary>
      /// Konstruktor třídy
      /// </summary>
      public Vyhledavani()
      {
         VsechnyZaznamy = new ObservableCollection<Zaznam>();
      }




      /// <summary>
      /// Výběr záznamů aktuálního měsíce z celkové kolekce záznamů
      /// </summary>
      /// <param name="KolekceZazanamu">Celková kolekce záznamů</param>
      /// <returns>Kolekce vybraných záznamů</returns>
      public ObservableCollection<Zaznam> VratZaznamyAktualnihoMesice(ObservableCollection<Zaznam> KolekceZazanamu)
      {
         // LINQ dotaz pro výběr záznamů pouze z aktuálního měsíce. Záznamy jsou seřazeny sestupně dle data 
         var VybraneZaznamy = from zaznam in KolekceZazanamu
                              where (DateTime.Now.Month == zaznam.Datum.Month)
                              orderby zaznam.Datum descending
                              select zaznam;

         // Vytvoření kolekce pro návratovou hodnotu za účelem převedení výstupu z LINQ dotazu na potřebný datový typ
         ObservableCollection<Zaznam> Kolekce = new ObservableCollection<Zaznam>();
         foreach(var prvek in VybraneZaznamy)
         {
            Kolekce.Add((Zaznam)prvek);
         }

         // Návratová hodnota vrací vybraná data jako kolekci záznamů
         return Kolekce;
      }

      /// <summary>
      /// Výběr všech záznamů v kategorii příjmy.
      /// </summary>
      /// <param name="KolekceZazanamu">Celková kolekce záznamů</param>
      /// <returns>Kolekce vybraných záznamů</returns>
      public ObservableCollection<Zaznam> VratPrijmy(ObservableCollection<Zaznam> KolekceZazanamu)
      {
            // LINQ dotaz pro výběr všech záznamů spadajících do kategorie Příjmy
            var Prijmy = from zaznam in KolekceZazanamu
                         where (zaznam.PrijemNeboVydaj == KategoriePrijemVydaj.Prijem)
                         orderby zaznam.Datum descending
                         select zaznam;

         // Vytvoření kolekce pro návratovou hodnotu za účelem převedení výstupu z LINQ dotazu na potřebný datový typ
         ObservableCollection<Zaznam> Kolekce = new ObservableCollection<Zaznam>();
         foreach (var prvek in Prijmy)
         {
            Kolekce.Add((Zaznam)prvek);
         }

         // Návratová hodnota vrací vybraná data jako kolekci záznamů
         return Kolekce;
      }

      /// <summary>
      /// Výběr všech záznamů v kategorii výdaje.
      /// </summary>
      /// <param name="KolekceZazanamu">Celková kolekce záznamů</param>
      /// <returns>Kolekce vybraných záznamů</returns>
      public ObservableCollection<Zaznam> VratVydaje(ObservableCollection<Zaznam> KolekceZazanamu)
      {
         // LINQ dotaz pro výběr všech záznamů spadajících do kategorie Výdaje
         var Vydaje = from zaznam in KolekceZazanamu
                      where (zaznam.PrijemNeboVydaj == KategoriePrijemVydaj.Vydaj)
                      orderby zaznam.Datum descending
                      select zaznam;

         // Vytvoření kolekce pro návratovou hodnotu za účelem převedení výstupu z LINQ dotazu na potřebný datový typ
         ObservableCollection<Zaznam> Kolekce = new ObservableCollection<Zaznam>();
         foreach (var prvek in Vydaje)
         {
            Kolekce.Add((Zaznam)prvek);
         }

         // Návratová hodnota vrací vybraná data jako kolekci záznamů
         return Kolekce;
      }

      /// <summary>
      /// Výběr záznamů podle názvu
      /// </summary>
      /// <param name="Zaznamy">Celková kolekce záznamů</param>
      /// <param name="Nazev">Textový řetězec pro porovnání jednotlivých záznamů</param>
      /// <returns>Kolekce vybraných záznamů</returns>
      public ObservableCollection<Zaznam> VratZaznamyDleNazvu(ObservableCollection<Zaznam> Zaznamy, string Nazev)
      {
         // LINQ dotaz pro výběr záznamů se shodným jménem
         var ZaznamyNazev = from zaznam in Zaznamy
                            where zaznam.Nazev == Nazev
                            select zaznam;

         // Vytvoření kolekce pro návratovou hodnotu za účelem převedení výstupu z LINQ dotazu na potřebný datový typ
         ObservableCollection<Zaznam> Kolekce = new ObservableCollection<Zaznam>();
         foreach (var prvek in ZaznamyNazev)
         {
            Kolekce.Add((Zaznam)prvek);
         }

         // Návratová hodnota vrací vybraná data jako kolekci záznamů
         return Kolekce;
      }

      /// <summary>
      /// Výběr záznamů spadajících do kategorie v parametru
      /// </summary>
      /// <param name="Zaznamy">Celková kolekce záznamů</param>
      /// <param name="kategorie">Kategorie záznamu pro vyhledání</param>
      /// <returns>Kolekce vybraných záznamů</returns>
      public ObservableCollection<Zaznam> VratZaznamyDleKategorie(ObservableCollection<Zaznam> Zaznamy, Kategorie kategorie)
      {
         // LINQ dotaz pro výběr záznamů spadajících do konkrétní kategorie
         var ZaznamyKategorie = from zaznam in Zaznamy
                                where zaznam.Kategorie == kategorie
                                select zaznam;

         // Vytvoření kolekce pro návratovou hodnotu za účelem převedení výstupu z LINQ dotazu na potřebný datový typ
         ObservableCollection<Zaznam> Kolekce = new ObservableCollection<Zaznam>();
         foreach (var prvek in ZaznamyKategorie)
         {
            Kolekce.Add((Zaznam)prvek);
         }

         // Návratová hodnota vrací vybraná data jako kolekci záznamů
         return Kolekce;
      }

      /// <summary>
      /// Výběr záznamů s datem v zadaném časovém období
      /// </summary>
      /// <param name="Zaznamy">Celková kolekce záznamů</param>
      /// <param name="PocatecniDatum">Spodní hranice pro vyhledání</param>
      /// <param name="KoncoveDatum">Horní hranice pro vyhledání</param>
      /// <returns>Kolekce vybraných záznamů</returns>
      public ObservableCollection<Zaznam> VratZaznamyVCasovemRozmezi(ObservableCollection<Zaznam> Zaznamy, DateTime PocatecniDatum, DateTime KoncoveDatum)
      {
         // LINQ dotaz pro výběr záznamů s datem v zadaném časovém období
         var ZaznamyVCase = from zaznam in Zaznamy
                            where ((zaznam.Datum >= PocatecniDatum) && (zaznam.Datum <= KoncoveDatum))
                            select zaznam;

         // Vytvoření kolekce pro návratovou hodnotu za účelem převedení výstupu z LINQ dotazu na potřebný datový typ
         ObservableCollection<Zaznam> Kolekce = new ObservableCollection<Zaznam>();
         foreach (var prvek in ZaznamyVCase) 
         {
            Kolekce.Add((Zaznam)prvek);
         }

         // Návratová hodnota vrací vybraná data jako kolekci záznamů
         return Kolekce;
      }

      /// <summary>
      /// Výběr záznamů s hodnotou v zadaném rozmezí
      /// </summary>
      /// <param name="Zaznamy">Celková kolekce záznamů</param>
      /// <param name="MIN">Minimální hodnota pro vyhledání</param>
      /// <param name="MAX">Maximální hodnota pro vyhledání</param>
      /// <returns>Kolekce vybraných záznamů</returns>
      public ObservableCollection<Zaznam> VratZaznamySHodnotouVRozmezi(ObservableCollection<Zaznam> Zaznamy, double MIN, double MAX)
      {
         // LINQ dotaz pro výběr záznamů s datem v zadaném časovém období
         var ZaznamyVHodnote = from zaznam in Zaznamy
                               where (zaznam.Hodnota_PrijemVydaj >= MIN && zaznam.Hodnota_PrijemVydaj <= MAX)
                               select zaznam;

         // Vytvoření kolekce pro návratovou hodnotu za účelem převedení výstupu z LINQ dotazu na potřebný datový typ
         ObservableCollection<Zaznam> Kolekce = new ObservableCollection<Zaznam>();
         foreach (var prvek in ZaznamyVHodnote)
         {
             Kolekce.Add((Zaznam)prvek);
         }

         // Návratová hodnota vrací vybraná data jako kolekci záznamů
         return Kolekce;
      }

      /// <summary>
      /// Výběr záznamů s počtem položek dle zadaného rozmezí
      /// </summary>
      /// <param name="Zaznamy">Celková kolekce záznamů</param>
      /// <param name="PocetMIN">Minimální počet položek</param>
      /// <param name="PocetMAX">Maximální počet položek</param>
      /// <returns></returns>
      public ObservableCollection<Zaznam> VratZaznamyDlePoctuPolozek(ObservableCollection<Zaznam> Zaznamy, int PocetMIN, int PocetMAX)
      {
         // LINQ dotaz pro výběr záznamu dle zadaného rozpětí počtu položek
         var ZaznamySPoctemPolozek = from zaznam in Zaznamy
                                     where (PocetMIN <= zaznam.SeznamPolozek.Count && PocetMAX >= zaznam.SeznamPolozek.Count)
                                     select zaznam;

         // Vytvoření kolekce pro návratovou hodnotu za účelem převedení výstupu z LINQ dotazu na potřebný datový typ
         ObservableCollection<Zaznam> Kolekce = new ObservableCollection<Zaznam>();
         foreach (var prvek in ZaznamySPoctemPolozek)
         {
            Kolekce.Add((Zaznam)prvek);
         }

         // Návratová hodnota vrací vybraná data jako kolekci záznamů
         return Kolekce;
      }

      /// <summary>
      /// Metoda pro sestupné seřazení kolekce záznamů.
      /// </summary>
      /// <param name="Zaznamy">Kolekce záznamů</param>
      /// <returns>Seřazená kolekce záznamů</returns>
      public ObservableCollection<Zaznam> SeradZaznamy(ObservableCollection<Zaznam> Zaznamy)
      {
         // LINQ dotaz pro seřazení záznamů 
         var Razeni = from zaznam in Zaznamy
                      orderby zaznam.Datum descending
                      select zaznam;

         // Vytvoření kolekce pro návratovou hodnotu za účelem převedení výstupu z LINQ dotazu na potřebný datový typ
         ObservableCollection<Zaznam> Kolekce = new ObservableCollection<Zaznam>();
         foreach (var prvek in Razeni)
         {
            Kolekce.Add((Zaznam)prvek);
         }

         // Návratová hodnota vrací vybraná data jako kolekci záznamů
         return Kolekce;
      }

    }
}
