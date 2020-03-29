using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


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
   /// Třída obsahující funkce a metody pro správu a řízení celé aplikace. Tato třída je brána jako hlavní, proto v sobě uchovává a spravuje veškerá data aplikace.
   /// Instance této třídy je vytvořena pouze jednou, vždy je třeba předávat instanci této třídy pro zamezení vytvoření více instancí spravujících chod aplikace.
   /// </summary>
   public class Spravce_Aplikace: INotifyPropertyChanged
   {
      /// <summary>
      /// Událost z rozhraní INotifyPropertyChanged
      /// </summary>
      public event PropertyChangedEventHandler PropertyChanged;

      /// <summary>
      /// Cestak souboru pro uložení kolekce obsahující seznam položek v daném záznamu
      /// </summary>
      private string cesta = "SpravaFinanciData.xml";

      /// <summary>
      /// Instance přihlášeného uživatele
      /// </summary>
      private Uzivatel PrihlasenyUzivatel;

      /// <summary>
      /// Jméno přihlášeného uživatele
      /// </summary>
      public string JmenoPrihlasenehoUzivatele { get; private set; }

      /// <summary>
      /// Pomocná kolekce pro ukládání dat a vyvolání události při změně obsahu.
      /// </summary>
      private ObservableCollection<Uzivatel> KolekceUzivatelu;

      /// <summary>
      /// Kolekce uživatelů, která v sobě uchovává veškerá data všech uživatelů.
      /// Každý uživatel má vlastní kolkeci všech svých záznamů.
      /// </summary>
      public ObservableCollection<Uzivatel> KolekceUzivateluSeZaznamy
      {
         get
         {
            return KolekceUzivatelu;
         }
         set
         {
            KolekceUzivatelu = value;
            VyvolejZmenu(nameof(KolekceUzivateluSeZaznamy));
         }
      }

      /// <summary>
      /// Kolekce dat, která se zpracovává při běhu programu. 
      /// Kolekce obsahuje pouze data, která patří přihlášenému uživateli.
      /// </summary>
      public ObservableCollection<Zaznam> KolekceDatPrihlasenehoUzivatele
      {
         get
         {
            return PrihlasenyUzivatel.SeznamZaznamuUzivatele;
         }  
         set
         {
            PrihlasenyUzivatel.SeznamZaznamuUzivatele = value;
            VyvolejZmenu(nameof(KolekceDatPrihlasenehoUzivatele));
         }
      }




      /// <summary>
      /// Konstruktor třídy pro správu aplikace. 
      /// Obstará úvodní nastavení včetně načtení dat ze souboru XML.
      /// </summary>
      public Spravce_Aplikace()
      {
         // Inicializace proměnných při úvodním nastavení (po spuštění aplikace)
         JmenoPrihlasenehoUzivatele = "";
         PrihlasenyUzivatel = new Uzivatel();
         KolekceDatPrihlasenehoUzivatele = new ObservableCollection<Zaznam>();
         KolekceUzivateluSeZaznamy = new ObservableCollection<Uzivatel>();

         // Načtení dat ze souboru
         Nacti();
      }



      /// <summary>
      /// Metoda pro vytvoření nového uživatele a přidání do kolekce uživatelů.
      /// </summary>
      /// <param name="Jmeno">Jméno uživatele</param>
      /// <param name="Heslo">Heslo uživatele</param>
      public void VytvoreniNovehoUzivatele(string Jmeno, string Heslo)
      {
         // Kontrola zda již neexistuje uživatel se stejným jménem
         if (KontrolaExistujicihoJmenaUzivatele(Jmeno))
            throw new ArgumentException("Uživatel se zadaným jménem již existuje!");

         // Vytvoření nového uživatele
         Uzivatel NovyUzivatel = new Uzivatel(Jmeno, Heslo);

         // Přidání nového uživatele do kolekce uživatelů (data aplikace)
         KolekceUzivateluSeZaznamy.Add(NovyUzivatel);
         Uloz();
      }

      /// <summary>
      /// Metoda zkontroluje zda již neexistuje uživatel se stejným jménem.
      /// Pokud uživatel se stejným jménem již existuje vrátí TRUE a pokud je uživatelské jméno k dispozici vrátí FALSE.
      /// </summary>
      /// <param name="JmenoNovehoUzivatele">Zadané jméno</param>
      /// <returns>TRUE - uživatel již existuje, FALSE - uživatel ještě neexistuje</returns>
      public bool KontrolaExistujicihoJmenaUzivatele(string JmenoNovehoUzivatele)
      {
         // Cyklus postupně projde všechny existující uživatele a při první shodě jmen vrátí TRUE. 
         foreach (Uzivatel uzivatel in KolekceUzivateluSeZaznamy)
         {
            if (uzivatel.Jmeno == JmenoNovehoUzivatele)
               return true;
         }
         // Pokud ke shodě nedojde a cyklus se ukončí vrátí se FALSE
         return false;
      }

      /// <summary>
      /// Metoda pro kontrolu přihlášeného uživatele.
      /// Pokud je jméno předané v parametru shodné se jménem přihlášeného uživatele vrátí se TRUE.
      /// </summary>
      /// <param name="JmenoUzivatele">Jméno kontrolovaného uživatele</param>
      /// <returns>TRUE - je přihlášen kontrolovaný uživatel, FALSE - kontrolovaný uživatel není přihlášen</returns>
      public bool KontrolaPrihlaseniUzivatele(string JmenoUzivatele)
      {
         if (!(PrihlasenyUzivatel.Jmeno == JmenoUzivatele) || (PrihlasenyUzivatel == null))
            return false;
         else
            return true;
      }

      /// <summary>
      /// Metoda pro přihlášení uživatele do aplikace.
      /// V případě úspěšného přihlášení načte vybraného uživatele do interní proměnné pro zpracování jeho dat.
      /// Metoda hledá v kolekci jméno uživatele a v přpaděnalezení provede kontrolu hesla.
      /// </summary>
      /// <param name="JmenoUzivatele">Zadané jméno uživatele</param>
      /// <param name="HesloUzivatele">Zadané heslo uživatele</param>
      public void PrihlaseniUzivatele(string JmenoUzivatele, string HesloUzivatele)
      {
         // Cyklus projde všechny uživatele v kolekci a v případě nalezené shody jmen zkontroluje zadané heslo
         foreach(Uzivatel uzivatel in KolekceUzivateluSeZaznamy)
         {
            // Nalezení jména uživatele
            if (uzivatel.Jmeno == JmenoUzivatele)
            {
               // Kontrola hesla 
               if (uzivatel.Heslo == HesloUzivatele)
               {
                  // Přihlášení uživatele -> uložení nalezeného uživatele do interní proměnné pro práci s jeho daty
                  PrihlasenyUzivatel = uzivatel;
                  return;
               }
               // Zadané heslo není správně
               else
               {
                  throw new ArgumentException("Zadali jste špatné heslo!");
               }

            }

         }

         // Uživatel nebyl nalezen
         throw new ArgumentException("Uživatel se zadaným jménem nebyl nalezen.");
      }

      /// <summary>
      /// Metoda pro odhlášení uživatele.
      /// Nejprve uloží data do souboru a poté zpracovávaná data odstraní z interních proměnných.
      /// </summary>
      public void OdhlaseniUzivatele()
      {
         // Uložení dat přihlášeného uživatele do souboru
         UlozeniDatDoSouboru();

         // Zrušení přihlášeného uživatele (smazání jeho dat z aplikace)
         PrihlasenyUzivatel = null;
         JmenoPrihlasenehoUzivatele = "";
      }

      /// <summary>
      /// Nastavení úvodního vykreslení poznámkového bloku.
      /// </summary>
      /// <param name="PoznamkovyBlok">Textový blok pro vložení poznámky</param>
      /// <param name="Zobrazit">Příznakový bit: 0 - nevykreslovat, 1 - vykreslit</param>
      public void NastavPoznamkuUzivatele(TextBox PoznamkovyBlok, byte Zobrazit)
      {
         PoznamkovyBlok.Text = PrihlasenyUzivatel.Poznamka;
         PoznamkovyBlok.TextChanged += PoznamkovyBlok_TextChanged;
         PrihlasenyUzivatel.PoznamkaZobrazena = Zobrazit;
      }

      /// <summary>
      /// Obsluha události při změně obsahu poznámkového bloku uživatele.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PoznamkovyBlok_TextChanged(object sender, TextChangedEventArgs e)
      {
         // Převední objektu na původní datový typ
         TextBox PoznamkovyBlok = sender as TextBox;

         // Uložení textu do vlastnosti přihlášeného uživatele
         PrihlasenyUzivatel.Poznamka = PoznamkovyBlok.Text;
      }

      /// <summary>
      /// Metoda pro zjištění, zda má být poznámkový blok uživatele zobrazen.
      /// </summary>
      /// <returns>Příznakový bit: 0 - nezobrazovat, 1 - zobrazit</returns>
      public byte VratZobrazeniPoznamky()
      {
         return PrihlasenyUzivatel.PoznamkaZobrazena;
      }

      /// <summary>
      /// Přidání nového záznamu do kolekce dat uživatele
      /// </summary>
      /// <param name="zaznam">Nový záznam, který bude přidán do kolekce přihlášeného uživatele</param>
      public void PridaniNovehoZaznamu(Zaznam zaznam)
      {
         KolekceDatPrihlasenehoUzivatele.Add(zaznam);
         UlozeniDatDoSouboru();
      }

      /// <summary>
      /// Uložení provedených změn do souboru dat.
      /// Kolekci záznamů přihlášeného uživatele uloží do celkové kolekce dat a provede uložení dat do souboru XML.
      /// </summary>
      public void UlozeniDatDoSouboru()
      {
         Uloz();
      }

      /// <summary>
      /// Metoda pro kontrolu zda v kolekci uživatele již existuje stejný záznam jako záznam předaný v parametru.
      /// </summary>
      /// <param name="zaznam">Záznam ke kontrole</param>
      /// <returns>True - záznam již existuje, False - záznam neexistuje</returns>
      public bool KontrolaExistujicihoZaznamu(Zaznam zaznam)
      {
         // Čítač podobností při porovnání záznamů
         int citac = 0;

         // Postupné projití všech záznamů v kolekci přihlášeného uživatele pro kontrolu existujícího záznamu
         foreach (Zaznam InterniZaznam in KolekceDatPrihlasenehoUzivatele)
         {
            // Kontrola názvu
            if (zaznam.Nazev == InterniZaznam.Nazev)
               citac++;

            // Kontrola data
            if (zaznam.Datum == InterniZaznam.Datum)
               citac++;

            // Kontrola hodnoty
            if (zaznam.Hodnota_PrijemVydaj == InterniZaznam.Hodnota_PrijemVydaj)
               citac++;

            // Kontrola poznámky
            if (zaznam.Poznamka == InterniZaznam.Poznamka)
               citac++;

            // Kontrola kategorie
            if (zaznam.Kategorie == InterniZaznam.Kategorie)
               citac++;

            // Kontrola zda se jedná o příjem či výdaj
            if (zaznam.PrijemNeboVydaj == InterniZaznam.PrijemNeboVydaj)
               citac++;

            // Pokud jsou všechny parametry stejné, nastaví se návratová hodnota na TRUE a ukončí se porovnávání. 
            // Pokud se nějaká hodnota liší, čítač se vynuluje a pokračuje se v porovnávání dalšího záznamu.
            if (citac > 5)
               return true;
            else
               citac = 0;
         }

         // Pokud předaný záznam v kolekci záznamů uživatele neexistuje nastaví se návratová hodnota na false
         return false;
      }


      /// <summary>
      /// Metoda pro uložení záznamů do textového souboru ve formátu TXT.
      /// </summary>
      /// <param name="CestaSouboru">Cesta k souboru pro uložení</param>
      /// <param name="KolekceDat">Kolekce záznamů určených k uložení</param>
      public void UlozData_TXT(string CestaSouboru, ObservableCollection<Zaznam> KolekceDat)
      {
         // Vytvoření nového textového souboru pro uložení dat v textové formě
         using (StreamWriter sw = new StreamWriter(CestaSouboru))
         {
            // Cyklus pro postupné zapisování jednotlivých záznamů do textového souboru
            foreach (Zaznam zaznam in KolekceDat)
            {
               // Vypsání údajů záznamu
               sw.WriteLine(zaznam.Nazev + " (" + zaznam.PrijemNeboVydaj.ToString() + "): " + zaznam.Hodnota_PrijemVydaj + " Kč (" + zaznam.Kategorie + ")");
               sw.WriteLine(zaznam.Datum.ToShortDateString() + "\t Pozn.: " + zaznam.Poznamka + ".");

               // Oddělení položek od záznamu
               sw.WriteLine("...");

               // Vypsání položek daného záznamu
               foreach (Polozka polozka in zaznam.SeznamPolozek)
               {
                  sw.WriteLine(polozka.Nazev + " (" + polozka.Popis + "): " + polozka.Cena + " Kč (" + polozka.KategoriePolozky + ");");
               }

               // Oddělení jednotlivých záznamů
               sw.WriteLine("\n\n\n");
            }

            // Vyprázdnění paměti po zápisu do souboru
            sw.Flush();
         }
      }

      /// <summary>
      /// Metoda pro uložení záznamů do separovaného textového dokumentu ve formátu CSV.
      /// </summary>
      /// <param name="CestaSouboru">Cesta k souboru pro uložení</param>
      /// <param name="KolekceDat">Kolekce záznamů určených k uložení</param>
      public void UlozData_CSV(string CestaSouboru, ObservableCollection<Zaznam> KolekceDat)
      {
         // Vytvoření nového souboru ve formátu CSV pro zápis dat
         using (StreamWriter sw = new StreamWriter(CestaSouboru))
         {
            // Cyklus pro postupné zapisování jednotlivých záznamů do souboru
            foreach (Zaznam zaznam in KolekceDat)
            {
               // Vytvoření pole textových řetězců reprezentující jednotlivé parametry a hodnoty daného záznamu
               string[] parametry = { zaznam.Nazev.Replace(';', ' '), zaznam.PrijemNeboVydaj.ToString(), zaznam.Hodnota_PrijemVydaj.ToString(),
                                      zaznam.Kategorie.ToString(), zaznam.Datum.ToShortDateString(), zaznam.Poznamka.Replace(';', ' ') };

               // Seskupení pole parametrů do jednoho textového řetězce se středníkem jako separátor pro oddělení jednotlivých parametrů
               string radek = String.Join(";", parametry);

               // Zapsání řádku reprezentující daný záznam do souboru
               sw.WriteLine(radek);

               // Vytvoření řádku pro výpis položek
               string radekPolozky = "";

               // Postupné vypsání jednotlivých položek do souboru pod daný záznam
               foreach (Polozka polozka in zaznam.SeznamPolozek)
               {
                  // Vytvoření pole textových řetězců reprezentující jednotlivé parametry a hodnoty daného záznamu
                  string[] parametryPolozky = { polozka.Nazev.Replace(';', ' '), polozka.Popis.Replace(';', ' '), polozka.Cena.ToString(), polozka.KategoriePolozky.ToString() };

                  // Seskupení pole parametrů do jednoho textového řetězce se středníkem jako separátor pro oddělení jednotlivých parametrů
                  string radekJednePolozky = String.Join(";", parametryPolozky) + ";";

                  // Přidání položky do řádku položek
                  radekPolozky += radekJednePolozky;
               }

               // Zapsání řádku reprezentující daný záznam do souboru
               sw.WriteLine(radekPolozky);
            }

            // Vyprázdnění paměti po zápisu do souboru
            sw.Flush();
         }
      }

      /// <summary>
      /// Metoda pro uložení vybraných záznamů do souboru ve formátu XML.
      /// </summary>
      /// <param name="CestaSouboru">Cesta k souboru pro uložení</param>
      /// <param name="KolekceDat">Kolekce záznamů určených k uložení</param>
      public void UlozData_XML(string CestaSouboru, ObservableCollection<Zaznam> KolekceDat)
      {
         // Vytvoření serializátoru s předáním typu kolekce pro uložení
         XmlSerializer serializer = new XmlSerializer(KolekceDat.GetType());

         // Funkce pro zápis dat do souboru. Blok using zajistí automatické zavření souboru. 
         using (StreamWriter sw = new StreamWriter(CestaSouboru))
         {
            // Volání metody pro serializaci kolekce k uložení
            serializer.Serialize(sw, KolekceDat);
         }
      }

      /// <summary>
      /// Metoda pro načtení záznamů ze souboru ve formátu XML.
      /// Metoda načte záznamy ze souboru a jednotlivé záznamy porovná s již existujícími. Záznamy, které ještě neexistují se přidají do kolekce přihlášeného uživatele.
      /// </summary>
      /// <param name="cesta">Cesta k souboru pro načtení záznamů ze souboru</param>
      /// <param name="Prepsat">0 - záznamy budou přidány do stávající kolekce, 1 - původní záznamy budou smazány a přidají se nové záznamy</param>
      public void NactiData_XML(string cesta, byte Prepsat)
      {
         // Vytvoření dočasné kolekce pro načtené jednotlivých záznamů ze souboru
         ObservableCollection<Zaznam> NacteneZaznamy = new ObservableCollection<Zaznam>();

         // Vytvoření serializátoru s předáním typu kolekce pro uložení
         XmlSerializer serializer = new XmlSerializer(NacteneZaznamy.GetType());

         // Pokud soubor existuje do kolekce se načtou data ze souboru
         if (File.Exists(cesta))
         {
            // Funkce pro čtení dat ze souboru. Blok using zajistí automatické zavření souboru. 
            using (StreamReader sr = new StreamReader(cesta))
            {
               // Provedení deserializace -> načtení objektů ze souboru do vnitřní kolekce (včetně přetypování)
               NacteneZaznamy = (ObservableCollection<Zaznam>)serializer.Deserialize(sr);
            }
         }

         // Smazání původních záznamů pokud je nastavena volba pro přepsání kolekce záznamů
         if (Prepsat == 1)
            KolekceDatPrihlasenehoUzivatele.Clear();

         // Interní proměnná pro zjištění počtu úspěšně přidaných záznamů
         int CitacNahranychZaznamu = 0;

         // Přidání nových záznamů do kolekce přihlášeného uživatele
         foreach (Zaznam zaznam in NacteneZaznamy)
         {
            // Pokud daný záznam ještě neexistuje, přidá se do kolekce záznamů jako nový záznam
            if (!KontrolaExistujicihoZaznamu(zaznam))
            {
               PridaniNovehoZaznamu(zaznam);
               CitacNahranychZaznamu++;
            }
         }

         // Zobrazení informačního okna o výsledku přidání nových záznamů do kolekce včetně zobrazení počtu úspěšně přidaných záznamů
         MessageBox.Show(String.Format("Bylo přidáno {0} záznamů z celkových {1}. \nZáznamy, které již existují byly přeskočeny.", CitacNahranychZaznamu, NacteneZaznamy.Count), 
                         "Import dat dokončen", MessageBoxButton.OK, MessageBoxImage.Information);
      }




      /// <summary>
      /// Metoda pro uložení dat aplikace do souboru XML uložený ve složce AppData.
      /// K uložení dat je využita třída XmlSerializer, která uloží obsah do formátu XML automaticky. Tato třída ukládá pouze veřejné vlastnosti položek v kolekci.
      /// </summary>
      private void Uloz()
      {
         try
         {
            // Uložení cesty ke složce v AppData do textového řetězce
            cesta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sprava_Financi");

            // Pokud složka neexistuje, vytvoří se nová složka
            if (!Directory.Exists(cesta))
               Directory.CreateDirectory(cesta);

            // Sloučení cesty do složky a samotného souboru
            cesta = Path.Combine(cesta, "DataUzivatelu.xml");

            // Vytvoření serializátoru s předáním typu kolekce pro uložení
            XmlSerializer serializer = new XmlSerializer(KolekceUzivateluSeZaznamy.GetType());

            // Funkce pro zápis dat do souboru. Blok using zajistí automatické zavření souboru. 
            using (StreamWriter sw = new StreamWriter(cesta))
            {
               // Volání metody pro serializaci kolekce k uložení
               serializer.Serialize(sw, KolekceUzivateluSeZaznamy);
            }
         }
         catch (Exception ex)
         {  
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      /// <summary>
      /// Metoda pro načtení uložených dat ze souboru XML uložený ve složce AppData.
      /// Využívá se třída XmlSerializer, která je využita i pro uložení těchto dat. Tato třída ukládá pouze veřejné vlastnosti položek v kolekci.
      /// </summary>
      private void Nacti()
      {
         try
         {
            // Uložení cesty ke složce v AppData do textového řetězce
            cesta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sprava_Financi");

            // Pokud složka neexistuje, vytvoří se nová složka
            if (!Directory.Exists(cesta))
               Directory.CreateDirectory(cesta);

            // Sloučení cesty do složky a samotného souboru
            cesta = Path.Combine(cesta, "DataUzivatelu.xml");

            // Vytvoření serializátoru s předáním typu kolekce pro uložení
            XmlSerializer serializer = new XmlSerializer(KolekceUzivateluSeZaznamy.GetType());

            // Pokud soubor existuje do kolekce aplikace se načtou data ze souboru
            if (File.Exists(cesta))
            {
               // Funkce pro čtení dat ze souboru. Blok using zajistí automatické zavření souboru. 
               using (StreamReader sr = new StreamReader(cesta))
               {
                  // Provedení deserializace -> načtení objektů ze souboru do vnitřní kolekce (včetně přetypování)
                  KolekceUzivateluSeZaznamy = (ObservableCollection<Uzivatel>)serializer.Deserialize(sr);
               }
            }

            else  // Pokud soubor neexistuje, vytvoří se prázdná kolekce
            {
               KolekceUzivateluSeZaznamy = new ObservableCollection<Uzivatel>();
            }
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }

      /// <summary>
      /// Pomocná metoda pro vyvolání události, které předá jméno změněné vlastnosti (převzato v parametru).
      /// Metoda je volána vždy s názvem vlastnosti tam, kde je potřeba reagovat na změnu. 
      /// </summary>
      /// <param name="vlastnost">Vlastnost, která vyvolá změnu na základě změny vlastního obsahu</param>
      protected void VyvolejZmenu(string vlastnost)
      {
         if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(vlastnost));
      }

   }
}
