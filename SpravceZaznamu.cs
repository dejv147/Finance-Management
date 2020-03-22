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
   /// Kategorie podle kterých se záznamy rozdělí do různých kolkecí.
   /// </summary>
   public enum Kategorie
   {
      Rodina, Jidlo, Auto, Cestovani, Sport, Kino, Divadlo, Dar, Vypalta, Brigada, Obleceni, Restaurace,
      Alkohol, Kultura, Inkaso, Najem, DomaciMazlicek, Partner, Zdravi, Kosmetika, Domacnost, Domov, PC, 
      Nevybrano
   };

   /// <summary>
   /// Výčtový typ pro určení zda záznam představuje výdaj, nebo příjem
   /// </summary>
   public enum KategoriePrijemVydaj { Prijem, Vydaj };


   /// <summary>
   /// Třída zprostředkovávající přidělování jednotlivých záznamů konkrétnímu uživately. 
   /// Třída pouze zprostředkovává předávání záznamů uživateli a uchovává výčtové typy ve jmeném prostoru pro možnost rozdělení záznamů.
   /// </summary>
   public class SpravceZaznamu: INotifyPropertyChanged
   {
      /// <summary>
      /// Událost z rozhraní INotifyPropertyChanged
      /// </summary>
      public event PropertyChangedEventHandler PropertyChanged;

      /// <summary>
      /// Seznam všech transakcí (záznamů)
      /// </summary>
      public ObservableCollection<Zaznam> SeznamTransakci { get; set; }



      /// <summary>
      /// Konstruktor třídy. 
      /// Vytvoří se nová kolekce záznamů (seznam transakcí).
      /// </summary>
      public SpravceZaznamu()
      {
         SeznamTransakci = new ObservableCollection<Zaznam>();
      }

   }
}
