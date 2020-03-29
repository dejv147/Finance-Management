using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
   /// Třída pro statistické zpracování a vyhodnocení zadaných dat. 
   /// Třída slouží pro zpracování dat a grafické zobrazení zpracovaných dat. 
   /// Dále slouží k obsluze okenního forluláře (plátna pro vykreslení statistických grafů a ovládacích prvků).
   /// </summary>
   public class Statistika
   {
      /// <summary>
      /// Blok uchovávající informace o záznamech pro konkrétní kategorii.
      /// </summary>
      public struct BlokKategorie
      {
         public DateTime DatumPocatecni;
         public DateTime DatumKoncove;
         public string Nazev;
         public int PocetZaznamu;
         public double CelkovaHodnota;
         public KategoriePrijemVydaj kategorie;
         public Brush Barva;
      }

      /// <summary>
      /// Blok uchovávající informace o záznamech v určitém časovém období.
      /// </summary>
      public struct BlokPrehledu
      {
         public DateTime DatumPocatecni;
         public DateTime DatumKoncove;
         public string Nazev;
         public string Popisek;
         public int PocetZaznamu;
         public double CelkovaHodnota;
         public KategoriePrijemVydaj kategorie;
         public Brush Barva;
      }

      /// <summary>
      /// Instance třídy pro validaci vstupních dat
      /// </summary>
      private Validace validator;

      /// <summary>
      /// Instance třídy pro získání názvu dnů a měsíce do grafu Prehled
      /// </summary>
      private Hodiny hodiny;

      /// <summary>
      /// Uchování dat pro možnost zobrazení
      /// </summary>
      public ObservableCollection<Zaznam> KolekceZaznamu { get; private set; }

      /// <summary>
      /// Pomocná proměnná pro nastavení barvy
      /// </summary>
      private Brush BarvaModra = Brushes.Blue;

      /// <summary>
      /// Pomocná proměnná pro nastavení barvy
      /// </summary>
      private Brush BarvaCervena = Brushes.Red;

      /// <summary>
      /// Pomocná proměnná pro nastavení barvy
      /// </summary>
      private Brush BarvaCerna = Brushes.Black;

      /// <summary>
      /// Pomocná proměnná pro nastavení barvy
      /// </summary>
      private Brush BarvaZluta = Brushes.Yellow;

      /// <summary>
      /// Pomocná proměnná pro nastavení barvy
      /// </summary>
      private Brush BarvaZelena = Brushes.Green;

      /// <summary>
      /// Plátno pro vykreslení samotného grafu
      /// </summary>
      public Canvas PlatnoGrafuCanvas { get; private set; }

      /// <summary>
      /// Plátno pro vykreslení ovládacích prvků pro ovládání grafu
      /// </summary>
      public Canvas PlatnoOvladacichPrvkuCanvas { get; private set; }

      /// <summary>
      /// Plátno pro vykreslení informačního okna, nebo oblasti pod grafem
      /// </summary>
      public Canvas PlatnoInfoOblastiCanvas { get; private set; }

      /// <summary>
      /// Textová reprezentace názvu grafu
      /// </summary>
      public string Nazev { get; private set; }

      /// <summary>
      /// Grafická reprezentace názvu grafu
      /// </summary>
      public TextBlock GrafickyNazev { get; private set; }

      /// <summary>
      /// Graficky reprezentovaná legenda grafu
      /// </summary>
      public StackPanel Legenda { get; private set; }

      /// <summary>
      /// Datum, od kterého se zpracovávají data pro vykreslení grafu kategorie
      /// </summary>
      private DateTime PocatecniDatum;

      /// <summary>
      /// Datum, do kterého se zpracovávají data pro vykreslení grafu kategorie
      /// </summary>
      private DateTime KoncoveDatum;

      /// <summary>
      /// Počet řádků (hodnot na ose Y) pro 1 graf
      /// </summary>
      private const int PocetHodnot = 5;

      /// <summary>
      /// Pomocná proměnná pro uložení maximální hodnoty prvků v grafu pro určení hodnot na ose Y
      /// </summary>
      private double MaximalniHodnota;

      /// <summary>
      /// Kolekce uchovávající hodnoty na ose Y
      /// </summary>
      private List<int> HodnotyOsyY;

      /// <summary>
      /// Interní proměnná pro identifikaci vykreslované stránky grafu
      /// </summary>
      private int CisloStrankyGrafu;

      /// <summary>
      /// Maximální počet stránek grafu v závislosti na počtu bloků určených k vykreslení
      /// </summary>
      private int MaximalniPocetStranGrafu;

      /// <summary>
      /// Příznakový bit pro indikaci zda je vykresleno informační okno
      /// </summary>
      private byte InfoVykresleno;


      /// <summary>
      /// Počet sloupců (prvků na ose X) na 1 stránce grafu
      /// </summary>
      private const int PocetPrvkuKategorie = 6;

      /// <summary>
      /// Pomocná proměnná pro výchozí souřadnici vykreslování popisu osy X grafu Kategorie
      /// </summary>
      private int SouradniceX_PrvniSloupecKategorie;

      /// <summary>
      /// Pomocná proměnná pro uložení mezery mezi bloky v grafu Kategorie
      /// </summary>
      private int OdstupHodnotX_Kategorie;

      /// <summary>
      /// Kolekce bloků reprezentujících všechny záznamy v dané kategorii pro příjmy
      /// </summary>
      public ObservableCollection<BlokKategorie> KolekceBlokuPrijjmuKategorie { get; private set; }

      /// <summary>
      /// Kolekce bloků reprezentujících všechny záznamy v dané kategorii pro výdaje
      /// </summary>
      public ObservableCollection<BlokKategorie> KolekceBlokuVydajuKategorie { get; private set; }

      /// <summary>
      /// Kolekce bloků zobrazovaných na 1 stránce grafu Kategorie pro vykreslení výdajů
      /// </summary>
      public ObservableCollection<BlokKategorie> ZobrazeneBlokyVydajuKategorie { get; private set; }

      /// <summary>
      /// Kolekce bloků zobrazovaných na 1 stránce grafu Kategorie pro vykreslení příjmů
      /// </summary>
      public ObservableCollection<BlokKategorie> ZobrazeneBlokyPrijmuKategorie { get; private set; }

      /// <summary>
      /// Popisky osy X (názvy kategorií) při vykreslení grafu Kategorie
      /// </summary>
      public ObservableCollection<string> ZobrazeneKategorie { get; private set; }


      /// <summary>
      /// Počet prvků na ose X na 1 stránce grafu Prehled
      /// </summary>
      private int PocetPrvkuPrehled;

      /// <summary>
      /// Pomocná proměnná pro výchozí souřadnici vykreslování popisu osy X grafu Přehled
      /// </summary>
      private int SouradniceX_PrvniPrvekPrehledu;

      /// <summary>
      /// Pomocná proměnná pro uložení mezery mezi bloky v grafu Přehled
      /// </summary>
      private int OdstupHodnotX_Prehled;

      /// <summary>
      /// Uložení vybraného měsíce pro výběr dat k zobrazení do grafu Prehled
      /// </summary>
      private int VybranyMesic;

      /// <summary>
      /// Uložení vybraného roku pro výběr dat k zobrazení do grafu Prehled
      /// </summary>
      private int VybranyRok;

      /// <summary>
      /// Uložení vybraného koncového roku pro výběr dat k zobrazení do grafu Prehled v případě zobrazení roků
      /// </summary>
      private int VybranyKoncovyRok;

      /// <summary>
      /// Výčtový typ pro výběr různých druhů zobrazení dat
      /// </summary>
      public enum ZvoleneZobrazeniPrehledu { Dny, Tydny, Mesice, Roky }

      /// <summary>
      /// Proměnná pro práci s výčtovým typem
      /// </summary>
      private ZvoleneZobrazeniPrehledu ZobrazeniPrehledu;

      /// <summary>
      /// Kolekce bloků reprezentujících všechny záznamy v daném období pro příjmy
      /// </summary>
      public ObservableCollection<BlokPrehledu> KolekceBlokuPrijjmuPrehled { get; private set; }

      /// <summary>
      /// Kolekce bloků reprezentujících všechny záznamy v daném období pro výdaje
      /// </summary>
      public ObservableCollection<BlokPrehledu> KolekceBlokuVydajuPrehled { get; private set; }

      /// <summary>
      /// Kolekce bloků zobrazovaných na 1 stránce grafu Prehled pro vykreslení výdajů
      /// </summary>
      public ObservableCollection<BlokPrehledu> ZobrazeneBlokyVydajuPrehled { get; private set; }

      /// <summary>
      /// Kolekce bloků zobrazovaných na 1 stránce grafu Prehled pro vykreslení příjmů
      /// </summary>
      public ObservableCollection<BlokPrehledu> ZobrazeneBlokyPrijmuPrehled { get; private set; }

      /// <summary>
      /// Popisky osy X (časové období) při vykreslování grafu Přehled
      /// </summary>
      public ObservableCollection<string> PopiskyGrafuPrehledu { get; private set; }

      /// <summary>
      /// Příznakový bit pro možnost vykreslení příjmů do grafu
      /// </summary>
      private byte VykreslitPrijmy;

      /// <summary>
      /// Příznakový bit pro možnost vykreslení výdajů do grafu
      /// </summary>
      private byte VykreslitVydaje;

      /// <summary>
      /// Příznakový bit informující zda se bude vykreslovat graf Prehled (0) nebo graf Kategorie (1)
      /// </summary>
      private byte VykreslenyGraf;



      /// <summary>
      /// Konstruktor třídy pro inicializaci vnitřních proměnných a úvodní nastavení.
      /// </summary>
      /// <param name="KolekceZaznamu">Kolekce dat pro zpracování a vykreslení</param>
      /// <param name="Graf">Plátno pro vykreslování grafu</param>
      /// <param name="OvladaciPrvky">Plátno pro vykreslení ovládacích prvků grafu</param>
      /// <param name="SpodniOblast">Plátno pro vykreslení oblasti pod grafem a informačního okna</param>
      public Statistika(ObservableCollection<Zaznam> KolekceZaznamu, Canvas Graf, Canvas OvladaciPrvky, Canvas SpodniOblast)
      {
         // Inicializace interních kolekcí a proměnných
         this.KolekceZaznamu = KolekceZaznamu;
         this.PlatnoGrafuCanvas = Graf;
         this.PlatnoInfoOblastiCanvas = SpodniOblast;
         this.PlatnoOvladacichPrvkuCanvas = OvladaciPrvky;
         Nazev = "";
         MaximalniHodnota = 0;
         CisloStrankyGrafu = 0;
         MaximalniPocetStranGrafu = 0;
         InfoVykresleno = 0;
         SouradniceX_PrvniSloupecKategorie = 0;
         OdstupHodnotX_Kategorie = 0;
         SouradniceX_PrvniPrvekPrehledu = 0;
         OdstupHodnotX_Prehled = 0;
         VykreslitPrijmy = 1;
         VykreslitVydaje = 1;
         validator = new Validace();
         hodiny = new Hodiny();
         HodnotyOsyY = new List<int>();
         KolekceBlokuPrijjmuKategorie = new ObservableCollection<BlokKategorie>();
         KolekceBlokuVydajuKategorie = new ObservableCollection<BlokKategorie>();
         ZobrazeneBlokyPrijmuKategorie = new ObservableCollection<BlokKategorie>();
         ZobrazeneBlokyVydajuKategorie = new ObservableCollection<BlokKategorie>();
         ZobrazeneKategorie = new ObservableCollection<string>();
         KolekceBlokuPrijjmuPrehled = new ObservableCollection<BlokPrehledu>();
         KolekceBlokuVydajuPrehled = new ObservableCollection<BlokPrehledu>();
         ZobrazeneBlokyPrijmuPrehled = new ObservableCollection<BlokPrehledu>();
         ZobrazeneBlokyVydajuPrehled = new ObservableCollection<BlokPrehledu>();
         PopiskyGrafuPrehledu = new ObservableCollection<string>();

         // Úvodní vykreslení grafu 
         UvodniNastaveniGrafuPrehledu();
      }



      /// <summary>
      /// Určení hodnot zobrazených na ose Y na základě maximální hodnoty prvků v grafu
      /// </summary>
      /// <param name="MaximalniHodnotaPrvku">Maximální hodnota prvků v grafu</param>
      public void NastavHodnotyNaOseY(double MaximalniHodnotaPrvku)
      {
         // Smazání kolekce hodnot pro možnost vytvoření nových hodnot na ose Y
         HodnotyOsyY.Clear();

         // Ochranná podmínka pro případ že na stránce v grafu nejsou žádná data, maximální hodnota je tedy 0
         if (MaximalniHodnotaPrvku <= PocetHodnot)
         {
            for (int i = 1; i <= PocetHodnot; i++)
               HodnotyOsyY.Add(i);

            return;
         }
         
         // Pomocná proměnná pro zaokrouhlení na vyšší číslo s určitým počtem desetinných míst
         int Zaokrouhleni = MaximalniHodnotaPrvku.ToString().Length - 1;

         // Určení nejvyššího čísla na ose Y, které je vyšší než maximální hodnota prvků v grafu
         double HorniHranice = Math.Ceiling(MaximalniHodnotaPrvku / Zaokrouhleni) * Zaokrouhleni;
        
         // Výpočet hodnot zobrazovaných na ose Y dle zjištěné maximální zobrazené hodnoty a počtu hodnot na ose
         for (int i = 1; i <= PocetHodnot; i++)
         {
            HodnotyOsyY.Add((int)Math.Round(HorniHranice / (double)PocetHodnot * i));
         }
      }

      /// <summary>
      /// Metoda pro vytvoření grafického popisku s textem předaným v parametru.
      /// </summary>
      /// <param name="Nazev">Textová reprezentace názvu grafu</param>
      public void VytvorNazevGrafu(string Nazev)
      {
         // Uložení textového názve do interní proměnné
         this.Nazev = Nazev;

         // Vytvoření popisku s textem názvu grafu
         TextBlock nazev = new TextBlock
         {
            FontSize = 24,
            Foreground = Brushes.DarkBlue,
            Text = Nazev,
            TextDecorations = TextDecorations.Underline
         };
         GrafickyNazev = nazev;
      }

      /// <summary>
      /// Metoda pro vykreslení legendy grafu
      /// </summary>
      public void VytvorLegenduGrafu()
      {
         // Vytvoření StackPanelů pro uložení jednotlivých prvků na potřebné pozice
         StackPanel legenda = new StackPanel { Orientation = Orientation.Vertical };
         StackPanel Prijem = new StackPanel { Orientation = Orientation.Horizontal };
         StackPanel Vydaj = new StackPanel { Orientation = Orientation.Horizontal };

         // Vytvoření konkrétních prvků legendy
         Ellipse OznaceniPrijmu = new Ellipse { Height = 20, Width = 20, Fill = BarvaZelena };
         Ellipse OznaceniVydaju = new Ellipse { Height = 20, Width = 20, Fill = BarvaCervena };
         Label PopisPrijmu = new Label { FontSize = 22, Content = " Příjmy" };
         Label PopisVydaju = new Label { FontSize = 22, Content = " Výdaje" };

         // Vytvoření řádků legendy
         Prijem.Children.Add(OznaceniPrijmu);
         Prijem.Children.Add(PopisPrijmu);
         Vydaj.Children.Add(OznaceniVydaju);
         Vydaj.Children.Add(PopisVydaju);

         // Přidání řádků do bloku legendy
         legenda.Children.Add(Prijem);
         legenda.Children.Add(Vydaj);

         // Uložení vytvořené legendy do interní proměnné
         Legenda = legenda;
      }



      /// <summary>
      /// Úvodní nastavení grafu Prehled pro možnost 1. vykreslení
      /// </summary>
      public void UvodniNastaveniGrafuPrehledu()
      {
         // Nastavení vykreslování grafu na první stránku
         CisloStrankyGrafu = 0;

         // Výchozí nastavení zvoleného zobrazení
         VykreslenyGraf = 0;
         ZobrazeniPrehledu = ZvoleneZobrazeniPrehledu.Dny;
         PocetPrvkuPrehled = 11;
         VykreslitPrijmy = 1;
         VykreslitVydaje = 1;

         // Nastavení počátečního výběru pro zobrazení
         VybranyMesic = DateTime.Now.Month;
         VybranyRok = DateTime.Now.Year;

         // Volání pomocných metod pro vytvoření úvodního zobrazení grafu
         VytvorNazevGrafu("Přehled financí");
         VytvorLegenduGrafu();
         VykresliOvladaciPrvkyGrafuPrehledu(PlatnoOvladacichPrvkuCanvas, ZobrazeniPrehledu);
         VytvorBlokyProGrafPrehled(ZobrazeniPrehledu);
         VyberBlokyKZobrazeniPrehledu();
         VykresliGrafPrehledu(PlatnoGrafuCanvas);
         VykresliOblastPodGrafemPrehledu(PlatnoInfoOblastiCanvas);
      }

      /// <summary>
      /// Aktualizace vykreslení při změně dat nebo nastavení
      /// </summary>
      public void AktualizujGrafPrehledu()
      {
         VyberBlokyKZobrazeniPrehledu();
         VykresliGrafPrehledu(PlatnoGrafuCanvas);
         VykresliOblastPodGrafemPrehledu(PlatnoInfoOblastiCanvas);
      }

      /// <summary>
      /// Metoda pro nalezení maximální hodnoty mezi zobrazovanými daty za účelem upravení hodnot na ose Y
      /// </summary>
      public void NajdiMaximalniHodnotuPrehledu()
      {
         // Smazání maximální hodnoty pro možnost opětovného nastavení
         MaximalniHodnota = 0;

         // Kolekce příjmů se projde pokud je určena k vykreslení
         if (VykreslitPrijmy == 1)
         {
            // Nalezení nejvyšší hodnoty mezi příjmy
            foreach (BlokPrehledu blok in ZobrazeneBlokyPrijmuPrehled)
            {
               // Aktualizace maximální hodnoty v případě nalezení vyšší hodnoty než poslední nalezená maximální hodnota
               if (blok.CelkovaHodnota > MaximalniHodnota)
                  MaximalniHodnota = blok.CelkovaHodnota;
            }
         }

         // Kolekce výdajů se projde pokud je určena k vykreslení
         if (VykreslitVydaje == 1)
         {
            // Nalezení nejvyšší hodnoty mezi výdaji
            foreach (BlokPrehledu blok in ZobrazeneBlokyVydajuPrehled)
            {
               // Aktualizace maximální hodnoty v případě nalezení vyšší hodnoty než poslední nalezená maximální hodnota
               if (blok.CelkovaHodnota > MaximalniHodnota)
                  MaximalniHodnota = blok.CelkovaHodnota;
            }
         }

         // Pokud je maximální hodnota menší než počet hodnot na ose Y nastaví se maximální hodnota právě na tento počet
         if (MaximalniHodnota < PocetHodnot)
            MaximalniHodnota = PocetHodnot;

         // Nastavení hodnot na ose Y dle nejvyšší nalezené hodnoty prvků
         NastavHodnotyNaOseY(MaximalniHodnota);
      }

      /// <summary>
      /// Metoda pro vytvoření bloků reprezentující data záznamů ve zvoleném časovém období dle vybraného zobrazení
      /// </summary>
      /// <param name="Zobrazeni">Vybraná možnost zobrazení</param>
      public void VytvorBlokyProGrafPrehled(ZvoleneZobrazeniPrehledu Zobrazeni)
      {
         // Smazání obsahu kolekce pro nové načtení dat
         KolekceBlokuPrijjmuPrehled.Clear();
         KolekceBlokuVydajuPrehled.Clear();

         // Vytvoření bloků na základě zvoleného zobrazení
         switch(Zobrazeni)
         {
            case ZvoleneZobrazeniPrehledu.Dny:     VytvorBlokyProZobrazeniDnu(VybranyMesic, VybranyRok);       break;
            case ZvoleneZobrazeniPrehledu.Tydny:   VytvorBlokyProZobrazeniTydnu(VybranyMesic, VybranyRok);     break;
            case ZvoleneZobrazeniPrehledu.Mesice:  VytvorBlokyProZobrazeniMesicu(VybranyRok);                  break;
            case ZvoleneZobrazeniPrehledu.Roky:    VytvorBlokyProZobrazeniRoku(VybranyRok, VybranyKoncovyRok); break;
            default: break;
         }

         // Určení počtu stran pro vykreslení na zázkladě počtu prvků v kolekci pro vykreslení
         MaximalniPocetStranGrafu = (int)Math.Ceiling((double)KolekceBlokuPrijjmuPrehled.Count / (double)PocetPrvkuPrehled);

         // Nastavení vykreslování na první stránku
         CisloStrankyGrafu = 0;
      }

      /// <summary>
      /// Vytvoření bloků reprezentující záznamy v 1 den
      /// </summary>
      /// <param name="Mesic">Vybraný měsíc pro vytvoření bloků dnů</param>
      /// <param name="Rok">Vybraný rok pro vytvoření bloků dnů v měsíci</param>
      private void VytvorBlokyProZobrazeniDnu(int Mesic, int Rok)
      {
         // Určení počtu prvků na ose X
         PocetPrvkuPrehled = 11;

         // Vytvoření datumů dle zvoleného měsíce a roku
         DateTime Zacatek = new DateTime(Rok, Mesic, 1);
         DateTime Konec = new DateTime(Rok, Mesic, DateTime.DaysInMonth(Rok, Mesic));

         // Pokud je vybrán aktuální měsíc, vytvoří se bloky pouze od začátku měsíce do aktuálního dne
         if (DateTime.Now < Konec)
            Konec = DateTime.Now;
         
         // Cyklus pro vytvoření bloků reprezentující jednotlivé dny v zadaném období
         for (DateTime Datum = Zacatek; Datum <= Konec; Datum = Datum.AddDays(1))
         {
            // Vytvoření nových bloků
            BlokPrehledu BlokPrijmu = new BlokPrehledu();
            BlokPrehledu BlokVydaju = new BlokPrehledu();

            // Inicializace hodnot do úvodního nastavení
            BlokPrijmu.PocetZaznamu = 0;
            BlokPrijmu.CelkovaHodnota = 0;
            BlokPrijmu.Barva = BarvaZelena;
            BlokVydaju.PocetZaznamu = 0;
            BlokVydaju.CelkovaHodnota = 0;
            BlokVydaju.Barva = BarvaCervena;

            // Přidání názvu bloku dne
            BlokPrijmu.Nazev = hodiny.VratDenVTydnu(Datum.DayOfWeek);
            BlokVydaju.Nazev = hodiny.VratDenVTydnu(Datum.DayOfWeek);

            // Přidání popisku dne
            BlokPrijmu.Popisek = hodiny.VratDenVTydnu(Datum.DayOfWeek) + " " + Datum.Date.ToString("dd.MM.");
            BlokVydaju.Popisek = hodiny.VratDenVTydnu(Datum.DayOfWeek) + " " + Datum.Date.ToString("dd.MM.");

            // Uložení kategorie bloku
            BlokPrijmu.kategorie = KategoriePrijemVydaj.Prijem;
            BlokVydaju.kategorie = KategoriePrijemVydaj.Vydaj;

            // Uložení data
            BlokPrijmu.DatumPocatecni = Datum;
            BlokPrijmu.DatumKoncove = Datum;
            BlokVydaju.DatumPocatecni = Datum;
            BlokVydaju.DatumKoncove = Datum;
            
            // Cyklus projde všechny záznamy pro nalezení záznamů z daného dne
            foreach (Zaznam zaznam in KolekceZaznamu)
            {
               // Byl nalezen záznam v daném dni
               if (zaznam.Datum == Datum)
               {
                  // Záznam spadá mezi příjmy
                  if (zaznam.PrijemNeboVydaj == KategoriePrijemVydaj.Prijem)
                  {
                     BlokPrijmu.PocetZaznamu++;
                     BlokPrijmu.CelkovaHodnota += zaznam.Hodnota_PrijemVydaj;
                  }

                  // Záznam spadá mezi výdaje
                  else
                  {
                     BlokVydaju.PocetZaznamu++;
                     BlokVydaju.CelkovaHodnota += zaznam.Hodnota_PrijemVydaj;
                  }
               }
            }

            // Přidání vytvořeného bloku do kolekce
            KolekceBlokuPrijjmuPrehled.Add(BlokPrijmu);
            KolekceBlokuVydajuPrehled.Add(BlokVydaju);
         }
      }

      /// <summary>
      /// Vytvoření bloků reprezentující záznamy v celém týdnu
      /// </summary>
      /// <param name="Mesic">Vybraný měsíc pro vytvoření bloků jednotlivých týdnů</param>
      /// <param name="Rok">Vybraný rok pro vytvoření bloků jednotlivých týdnů konkrétního měsíce/param>
      private void VytvorBlokyProZobrazeniTydnu(int Mesic, int Rok)
      {
         // Získání datumů pro první a poslední den v týdnu
         List<DateTime> DatumyTydnu = Tydny.GetWeek(Mesic, Rok);

         // Zjištění počtu týdnů v daném měsíci
         int PocetTydnu = DatumyTydnu.Count / 2;

         // Určení počtu prvků na ose X (odpovídá počtu týdnu v zadaném měsíci)
         PocetPrvkuPrehled = PocetTydnu;

         // Postupné vytvoření potřebného počtu týdnů z závislosti na zadaném měsíci
         for (int tyden = 1; tyden <= PocetTydnu; tyden++)
         {
            // Vytvoření nových bloků
            BlokPrehledu BlokPrijmu = new BlokPrehledu();
            BlokPrehledu BlokVydaju = new BlokPrehledu();

            // Inicializace hodnot do úvodního nastavení
            BlokPrijmu.PocetZaznamu = 0;
            BlokPrijmu.CelkovaHodnota = 0;
            BlokPrijmu.Barva = BarvaZelena;
            BlokVydaju.PocetZaznamu = 0;
            BlokVydaju.CelkovaHodnota = 0;
            BlokVydaju.Barva = BarvaCervena;

            // Uložení názvu daného týdne
            BlokPrijmu.Nazev = hodiny.VratMesicTextove(Mesic) + ": " + tyden.ToString() + ". týden";
            BlokVydaju.Nazev = hodiny.VratMesicTextove(Mesic) + ": " + tyden.ToString() + ". týden";

            // Určení počátečního a koncového data týdne
            DateTime ZacatekTydne = DatumyTydnu[2 * (tyden - 1)];
            DateTime KonecTydne = DatumyTydnu[2 * (tyden - 1) + 1];

            // Uložení data
            BlokPrijmu.DatumPocatecni = ZacatekTydne;
            BlokPrijmu.DatumKoncove = KonecTydne;
            BlokVydaju.DatumPocatecni = ZacatekTydne;
            BlokVydaju.DatumKoncove = KonecTydne;

            // Uložení popisků týdne
            BlokPrijmu.Popisek = String.Format("{0}.{1}. - {2}.{3}.", ZacatekTydne.Day, ZacatekTydne.Month, KonecTydne.Day, KonecTydne.Month);
            BlokVydaju.Popisek = String.Format("{0}.{1}. - {2}.{3}.", ZacatekTydne.Day, ZacatekTydne.Month, KonecTydne.Day, KonecTydne.Month);

            // Uložení kategorie bloku
            BlokPrijmu.kategorie = KategoriePrijemVydaj.Prijem;
            BlokVydaju.kategorie = KategoriePrijemVydaj.Vydaj;

            // Postupné procházení jednotlivých dní v daném týdnu
            for (DateTime den = ZacatekTydne; den <= KonecTydne; den = den.AddDays(1))
            {
               // Cyklus projde všechny záznamy pro nalezení záznamů z daného dne
               foreach (Zaznam zaznam in KolekceZaznamu)
               {
                  // Byl nalezen záznam v daném dni
                  if (zaznam.Datum == den)
                  {
                     // Záznam spadá mezi příjmy
                     if (zaznam.PrijemNeboVydaj == KategoriePrijemVydaj.Prijem)
                     {
                        BlokPrijmu.PocetZaznamu++;
                        BlokPrijmu.CelkovaHodnota += zaznam.Hodnota_PrijemVydaj;
                     }

                     // Záznam spadá mezi výdaje
                     else
                     {
                        BlokVydaju.PocetZaznamu++;
                        BlokVydaju.CelkovaHodnota += zaznam.Hodnota_PrijemVydaj;
                     }
                  }
               }
            }

            // Přidání vytvořeného bloku do kolekce
            KolekceBlokuPrijjmuPrehled.Add(BlokPrijmu);
            KolekceBlokuVydajuPrehled.Add(BlokVydaju);
         }
      }

      /// <summary>
      /// Vytvoření bloků v zadaném roce, kde každý blok obsahuje informace o záznamech celého měsíce.
      /// </summary>
      /// <param name="Rok">Rok, pro který jsou vytvářeny bloky měsíce</param>
      private void VytvorBlokyProZobrazeniMesicu(int Rok)
      {
         // Určení počtu prvků na ose X
         PocetPrvkuPrehled = 12;

         // Určení posledního měsíce v roce (pokud se vykresluje aktuální rok, poslední vykreslovaný měsíc bude aktuální měsíc)
         int KoncovyMesic = DateTime.Now.Year > Rok ? 12 : DateTime.Now.Month;

         // Zůžení osy X pro případ, že je vykreslován malý počet prvků (sloupců)
         if (KoncovyMesic < 10)
            PocetPrvkuPrehled = KoncovyMesic + 1;

         // Cyklus pro vytvoření bloků reprezentující jednotlivé měsíce v zadaném roce
         for (int i = 1; i <= KoncovyMesic; i++)
         {
            // Vytvoření nových bloků
            BlokPrehledu BlokPrijmu = new BlokPrehledu();
            BlokPrehledu BlokVydaju = new BlokPrehledu();

            // Inicializace hodnot do úvodního nastavení
            BlokPrijmu.PocetZaznamu = 0;
            BlokPrijmu.CelkovaHodnota = 0;
            BlokPrijmu.Barva = BarvaZelena;
            BlokVydaju.PocetZaznamu = 0;
            BlokVydaju.CelkovaHodnota = 0;
            BlokVydaju.Barva = BarvaCervena;

            // Přidání názvů měsíce
            BlokPrijmu.Nazev = hodiny.VratMesicTextove(i);
            BlokVydaju.Nazev = hodiny.VratMesicTextove(i);

            // Přidání popisku měíce
            BlokPrijmu.Popisek = hodiny.VratMesicTextove(i);
            BlokVydaju.Popisek = hodiny.VratMesicTextove(i);

            // Uložení kategorie bloku
            BlokPrijmu.kategorie = KategoriePrijemVydaj.Prijem;
            BlokVydaju.kategorie = KategoriePrijemVydaj.Vydaj;

            // Uložení data
            BlokPrijmu.DatumPocatecni = new DateTime(Rok, i, 1);
            BlokVydaju.DatumPocatecni = new DateTime(Rok, i, 1);
            BlokPrijmu.DatumKoncove = new DateTime(Rok, i, DateTime.DaysInMonth(Rok, i));
            BlokVydaju.DatumKoncove = new DateTime(Rok, i, DateTime.DaysInMonth(Rok, i));

            // Postupné procházení všech dní v měsíci
            for (DateTime Den = BlokPrijmu.DatumPocatecni; Den <= BlokPrijmu.DatumKoncove; Den = Den.AddDays(1))
            {
               // Cyklus projde všechny záznamy pro nalezení záznamů z daného dne
               foreach (Zaznam zaznam in KolekceZaznamu)
               {
                  // Byl nalezen záznam v daném dni
                  if (zaznam.Datum == Den)
                  {
                     // Záznam spadá mezi příjmy
                     if (zaznam.PrijemNeboVydaj == KategoriePrijemVydaj.Prijem)
                     {
                        BlokPrijmu.PocetZaznamu++;
                        BlokPrijmu.CelkovaHodnota += zaznam.Hodnota_PrijemVydaj;
                     }

                     // Záznam spadá mezi výdaje
                     else
                     {
                        BlokVydaju.PocetZaznamu++;
                        BlokVydaju.CelkovaHodnota += zaznam.Hodnota_PrijemVydaj;
                     }
                  }
               }
            }

            // Přidání vytvořeného bloku do kolekce
            KolekceBlokuPrijjmuPrehled.Add(BlokPrijmu);
            KolekceBlokuVydajuPrehled.Add(BlokVydaju);
         }
      }

      /// <summary>
      /// Vytvoření bloků uchovávající v sobě veškeré záznamy daného roku v zadánem intervalu let.
      /// </summary>
      /// <param name="RokPocatecni">Počáteční rok</param>
      /// <param name="RokKoncovy">Koncový rok</param>
      private void VytvorBlokyProZobrazeniRoku(int RokPocatecni, int RokKoncovy)
      {
         // Určení počtu prvků na ose X
         PocetPrvkuPrehled = (RokKoncovy - RokPocatecni + 1) > 7 ? 5 : (RokKoncovy - RokPocatecni + 1) + 1;


         for (int rok = RokPocatecni; rok <= RokKoncovy; rok++)
         {
            // Vytvoření nových bloků
            BlokPrehledu BlokPrijmu = new BlokPrehledu();
            BlokPrehledu BlokVydaju = new BlokPrehledu();

            // Inicializace hodnot do úvodního nastavení
            BlokPrijmu.PocetZaznamu = 0;
            BlokPrijmu.CelkovaHodnota = 0;
            BlokPrijmu.Barva = BarvaZelena;
            BlokVydaju.PocetZaznamu = 0;
            BlokVydaju.CelkovaHodnota = 0;
            BlokVydaju.Barva = BarvaCervena;

            // Uložení názvu roku
            BlokPrijmu.Nazev = rok.ToString();
            BlokVydaju.Nazev = rok.ToString();

            // Uložení popisku roků
            BlokPrijmu.Popisek = rok.ToString();
            BlokVydaju.Popisek = rok.ToString();

            // Uložení kategorie bloku
            BlokPrijmu.kategorie = KategoriePrijemVydaj.Prijem;
            BlokVydaju.kategorie = KategoriePrijemVydaj.Vydaj;

            // Uložení data
            BlokPrijmu.DatumPocatecni = new DateTime(rok, 1, 1);
            BlokPrijmu.DatumKoncove = new DateTime(rok, 12, DateTime.DaysInMonth(rok, 12));
            BlokVydaju.DatumPocatecni = new DateTime(rok, 1, 1);
            BlokVydaju.DatumKoncove = new DateTime(rok, 12, DateTime.DaysInMonth(rok, 12));


            // Cyklus projde všechny záznamy pro nalezení záznamů z daného roku
            foreach (Zaznam zaznam in KolekceZaznamu)
            {
               // Byl nalezen záznam v daném roce
               if (zaznam.Datum.Year == rok)
               {
                  // Záznam spadá mezi příjmy
                  if (zaznam.PrijemNeboVydaj == KategoriePrijemVydaj.Prijem)
                  {
                     BlokPrijmu.PocetZaznamu++;
                     BlokPrijmu.CelkovaHodnota += zaznam.Hodnota_PrijemVydaj;
                  }

                  // Záznam spadá mezi výdaje
                  else
                  {
                     BlokVydaju.PocetZaznamu++;
                     BlokVydaju.CelkovaHodnota += zaznam.Hodnota_PrijemVydaj;
                  }
               }
            }

            // Přidání vytvořeného bloku do kolekce
            KolekceBlokuPrijjmuPrehled.Add(BlokPrijmu);
            KolekceBlokuVydajuPrehled.Add(BlokVydaju);
         }
      }


      /// <summary>
      /// Výběr potřebného počtu bloků pro zobrazení 1 stránky grafu
      /// </summary>
      public void VyberBlokyKZobrazeniPrehledu()
      {
         // Určení indexu bloku pro výběr bloku z kolekce na základě vykreslované stránky grafu
         int PrvniBlok = CisloStrankyGrafu * PocetPrvkuPrehled;

         // Smazání kolekcí zobrazovaných dat pro možnost přidání nových bloků pro zobrazení
         ZobrazeneBlokyPrijmuPrehled.Clear();
         ZobrazeneBlokyVydajuPrehled.Clear();

         // Pokud je v kolekci více bloků, vybere se pouze požadovaný počet bloků k vykreslení
         if ((PrvniBlok + PocetPrvkuPrehled) <= KolekceBlokuVydajuPrehled.Count)
         {
            // Postupné přidání bloků z celkové kolekce do kolekce bloků určených k zobrazení
            for (int index = PrvniBlok; index < (PrvniBlok + PocetPrvkuPrehled); index++)
            {
               ZobrazeneBlokyPrijmuPrehled.Add(KolekceBlokuPrijjmuPrehled[index]);
               ZobrazeneBlokyVydajuPrehled.Add(KolekceBlokuVydajuPrehled[index]);
            }
         }

         // Pokud v kolekci zbývá jen několik bloků, vyberou se všechny zbývající bloky pro vykreslení na poslední stránku grafu
         else
         {
            // Postupné přidání bloků z celkové kolekce do kolekce bloků určených k zobrazení
            for (int index = PrvniBlok; index < KolekceBlokuVydajuPrehled.Count; index++)
            {
               ZobrazeneBlokyPrijmuPrehled.Add(KolekceBlokuPrijjmuPrehled[index]);
               ZobrazeneBlokyVydajuPrehled.Add(KolekceBlokuVydajuPrehled[index]);
            }
         }

         // Vytvoření popisků vybraných bloků pro možnost vykreslení popisků pod grafem
         VytvorPopiskyPrehledu();
      }

      /// <summary>
      /// Uložení textového popisku vybraných bloků do kolekce.
      /// </summary>
      public void VytvorPopiskyPrehledu()
      {
         // Smazání obsahu kolekce pro opětovné vytvoření nových popisků při změně dat
         PopiskyGrafuPrehledu.Clear();

         // Pomocné proměnné pro rozhodnutí, která z kolekcí obsahuje více prvků
         int PocetPrijmu = ZobrazeneBlokyPrijmuPrehled.Count;
         int PocetVydaju = ZobrazeneBlokyVydajuPrehled.Count;
         ObservableCollection<BlokPrehledu> Prehled = new ObservableCollection<BlokPrehledu>();

         // Načtení kolekce s více prvky
         if (PocetPrijmu > PocetVydaju)
            Prehled = ZobrazeneBlokyPrijmuPrehled;
         else
            Prehled = ZobrazeneBlokyVydajuPrehled;

         // Uložení jednotlivých popisků do kolekce
         foreach (BlokPrehledu blok in Prehled)
         {
            PopiskyGrafuPrehledu.Add(blok.Popisek);
         }
      }

      /// <summary>
      /// Vykreslení sloupcového grafu. 
      /// Na ose Y jsou hodnoty příjmů a výdajů v součtu za zvolené časové období. 
      /// Na ose X jsou časové úseky v rámci zvoleného časového intervalu. 
      /// </summary>
      /// <param name="MyCanvas">Plátno pro vykreslení oblasti grafu</param>
      public void VykresliGrafPrehledu(Canvas MyCanvas)
      {
         // Pomocné proměnné pro určení pozic prvků na plátně
         int Vyska = (int)MyCanvas.Height - 10;
         int Sirka = (int)MyCanvas.Width;

         // Smazání obsahu pro možnost nového vykreslení
         MyCanvas.Children.Clear();

         // Výběr bloků určených k zobrazení v grafu
         VyberBlokyKZobrazeniPrehledu();

         // Kontrolní podmínka že pro danou stránku nejsou vybrány žádné bloky
         if (ZobrazeneBlokyPrijmuPrehled.Count == 0 && ZobrazeneBlokyVydajuPrehled.Count == 0)
         {
            // Změna čísla stránky a opětvoný výběr bloků
            CisloStrankyGrafu--;
            VyberBlokyKZobrazeniPrehledu();
         }

         // Výpočet maximální hodnoty zobrazených bloků pro určení hodnot na ose Y
         NajdiMaximalniHodnotuPrehledu();

         // Pomocná proměnná pro uchování šířky bloku obsahující popisky osy Y (začátek vykreslování samotného grafu)
         int Odsazeni = HodnotyOsyY[HodnotyOsyY.Count - 1].ToString().Length * 10;

         // Přičtení velikosti označení hodnoty (Kč za číslem)
         Odsazeni += 30;

         // Pomocná proměnná pro snadnější vykreslení hodnot na osu Y
         int mezeraY = (int)Math.Floor((double)Vyska / (double)HodnotyOsyY.Count);

         // Pomocná proměnná pro snadnější vykreslování prvků na osu X
         int mezeraX = (int)Math.Floor((double)(Sirka - Odsazeni) / (double)(PocetPrvkuPrehled + 0.5));

         // Určení šířky sloupce reprezentující 1 blok dat
         int SirkaSloupce = (mezeraX - 10) / 3;

         // Uložení souřadnice prvního bloku pro možnost vykreslení popisu osy X na správné souřadnice
         SouradniceX_PrvniPrvekPrehledu = Odsazeni + mezeraX;
         OdstupHodnotX_Prehled = mezeraX;

         // Levé ohraničené oblasti grafu (osa Y)
         Rectangle LeveOhraniceni = new Rectangle
         {
            Width = 2,
            Height = Vyska + 10,
            Fill = BarvaCerna
         };

         // Spodní ohraničené oblasti grafu (osa X)
         Rectangle SpodniOhraniceni = new Rectangle
         {
            Width = Sirka - Odsazeni,
            Height = 2,
            Fill = BarvaCerna
         };

         // Přidání ohraničení na plátno
         MyCanvas.Children.Add(LeveOhraniceni);
         MyCanvas.Children.Add(SpodniOhraniceni);
         Canvas.SetLeft(LeveOhraniceni, Odsazeni - 2);
         Canvas.SetBottom(LeveOhraniceni, 0);
         Canvas.SetLeft(SpodniOhraniceni, Odsazeni);
         Canvas.SetBottom(SpodniOhraniceni, 0);

         // Přidání popisku osy Y včetně pomocných čar značících úroveň na ose Y pro lepší orientaci v grafu
         for (int i = 1; i <= HodnotyOsyY.Count; i++)
         {
            // Číselná hodnota na ose Y
            Label popisek = new Label
            {
               FontSize = 14,
               Content = HodnotyOsyY[i - 1].ToString() + " Kč"
            };

            // Pomocná čára
            Rectangle cara = new Rectangle
            {
               Width = Sirka - Odsazeni,
               Height = 1,
               Fill = Brushes.Gray
            };

            // Vyznačení hodnoty na ose
            Rectangle oznaceni = new Rectangle
            {
               Width = 10,
               Height = 1,
               Fill = BarvaCerna
            };

            // Přidání prvků a vykreslení na plátno
            MyCanvas.Children.Add(popisek);
            MyCanvas.Children.Add(cara);
            MyCanvas.Children.Add(oznaceni);
            Canvas.SetLeft(popisek, 0);
            Canvas.SetBottom(popisek, (i * mezeraY) - 12);
            Canvas.SetLeft(cara, Odsazeni);
            Canvas.SetBottom(cara, (i * mezeraY));
            Canvas.SetLeft(oznaceni, Odsazeni - oznaceni.Width / 2 - 1);
            Canvas.SetBottom(oznaceni, (i * mezeraY));
         }

         // Přidání prvků na osu X včetně vykreslení grafických bloků reprezentující data 
         for (int i = 1; i <= PocetPrvkuPrehled; i++)
         {
            // Vyznačení hodnoty na ose
            Rectangle oznaceni = new Rectangle
            {
               Width = 1,
               Height = 7,
               Fill = BarvaCerna
            };

            // Přidání prvků a vykreslení na plátno
            MyCanvas.Children.Add(oznaceni);
            Canvas.SetLeft(oznaceni, Odsazeni + (i * mezeraX));
            Canvas.SetBottom(oznaceni, 0);

            // Pokud je blok v kolekci příjmů pro vykreslení, vytvoří se sloupec reprezentující jeho data -> vykreslení sloupců příjmů
            if (i <= ZobrazeneBlokyPrijmuPrehled.Count && VykreslitPrijmy == 1) 
            {
               // Výpočet výšky sloupce na základě jeho hodnoty
               int VyskaSloupce = (int)Math.Ceiling((ZobrazeneBlokyPrijmuPrehled[i - 1].CelkovaHodnota * Vyska) / MaximalniHodnota);

               // Pokud na vykreslované stránce nejsou žádné bloky k vykreslení, nastaví se výška sloupce na 0 pro možnost vykreslení prázdného grafu
               if (VyskaSloupce < 0)
                  VyskaSloupce = 0;

               // Vytvoření sloupce reprezentující blok příjmů
               Rectangle prijem = new Rectangle
               {
                  Width = SirkaSloupce,
                  Height = VyskaSloupce,
                  Fill = BarvaZelena
               };

               // Uložení indexu bloku do názvu sloupce pro možnost pozdější identifikace bloku ze sloupce
               prijem.Name = "sloupec" + (i - 1).ToString();

               // Přidání událoti pro možnost reagovat na pohyb myší na sloupci
               prijem.MouseMove += PrijemPrehled_MouseMove;
               prijem.MouseLeave += PrijemPrehled_MouseLeave;

               // Vykreslení sloupce na plátno
               MyCanvas.Children.Add(prijem);
               Canvas.SetLeft(prijem, Odsazeni + (i * mezeraX) - 5 - SirkaSloupce);
               Canvas.SetBottom(prijem, 2);
            }

            // Pokud je blok v kolekci výdajů pro vykreslení, vytvoří se sloupec reprezentující jeho data -> vykreslení sloupců výdajů
            if (i <= ZobrazeneBlokyVydajuPrehled.Count && VykreslitVydaje == 1) 
            {
               // Výpočet výšky sloupce na základě jeho hodnoty
               int VyskaSloupce = (int)Math.Ceiling((ZobrazeneBlokyVydajuPrehled[i - 1].CelkovaHodnota * Vyska) / MaximalniHodnota);

               // Pokud na vykreslované stránce nejsou žádné bloky k vykreslení, nastaví se výška sloupce na 0 pro možnost vykreslení prázdného grafu
               if (VyskaSloupce < 0)
                  VyskaSloupce = 0;

               // Vytvoření sloupce reprezentující blok příjmů
               Rectangle vydaj = new Rectangle
               {
                  Width = SirkaSloupce,
                  Height = VyskaSloupce,
                  Fill = BarvaCervena
               };

               // Uložení indexu bloku do názvu sloupce pro možnost pozdější identifikace bloku ze sloupce
               vydaj.Name = "sloupec" + (i - 1).ToString();

               // Přidání událoti pro možnost reagovat na pohyb myší na sloupci
               vydaj.MouseMove += VydajPrehled_MouseMove;
               vydaj.MouseLeave += VydajPrehled_MouseLeave;
               
               // Vykreslení sloupce na plátno
               MyCanvas.Children.Add(vydaj);
               Canvas.SetLeft(vydaj, Odsazeni + (i * mezeraX) + 5);
               Canvas.SetBottom(vydaj, 2);
            }
         }
      }

      /// <summary>
      /// Metoda pro vykreslení ovládacích prvků pro graf Prehled.
      /// </summary>
      /// <param name="MyCanvas">Plátno pro vykreslení</param>
      /// <param name="Zobrazeni">Vybraná možnost zobrazení</param>
      public void VykresliOvladaciPrvkyGrafuPrehledu(Canvas MyCanvas, ZvoleneZobrazeniPrehledu Zobrazeni)
      {
         // Pomocné proměnné pro určení pozic prvků na plátně
         int VyskaCanvas = (int)MyCanvas.Height;
         int SirkaCanvas = (int)MyCanvas.Width;

         // Smazání obsahu pro možnost nového vykreslení
         MyCanvas.Children.Clear();

         // Vykrelení názvu grafu a legendy
         MyCanvas.Children.Add(GrafickyNazev);
         MyCanvas.Children.Add(Legenda);
         Canvas.SetLeft(GrafickyNazev, 0);
         Canvas.SetTop(GrafickyNazev, 100);
         Canvas.SetLeft(Legenda, 0);
         Canvas.SetTop(Legenda, 0);

         // Levá šipka pro možnost vykreslit jiný graf
         Path LevaSipka = new Path
         {
            Stroke = BarvaCervena,
            Fill = BarvaModra,
            StrokeThickness = 3,
            Data = Geometry.Parse("m0,20 l20,-20 l0,10 l25,0 l0,20 l-25,0 l0,10 l-20,-20")
         };

         // Pravá šipka pro možnost vykreslit jiný graf
         Path PravaSipka = new Path
         {
            Stroke = BarvaCervena,
            Fill = BarvaModra,
            StrokeThickness = 3,
            Data = Geometry.Parse("m0,20 l0,10 l25,0 l0,10 l20,-20 l-20,-20 l0,10 l-25,0 l0,10")
         };

         // Přidání události pro možnost kliknutí na šipku
         LevaSipka.MouseDown += LevaSipkaPrepinaniGrafu_MouseDown;
         PravaSipka.MouseDown += PravaSipkaPrepinaniGrafu_MouseDown;

         // Vykreslení šipek na plátno
         MyCanvas.Children.Add(LevaSipka);
         MyCanvas.Children.Add(PravaSipka);
         Canvas.SetRight(LevaSipka, 70);
         Canvas.SetTop(LevaSipka, 10);
         Canvas.SetRight(PravaSipka, 10);
         Canvas.SetTop(PravaSipka, 10);


         // Popisek pro výběr zobrazení
         Label popisekZobrazeni = new Label
         {
            FontSize = 16,
            Content = "Zobrazit: "
         };

         // Vytvoření výběrových polí pro možnost volby zobrazení
         RadioButton dny = new RadioButton { Content = " dny", FontSize = 24 };
         RadioButton tydny = new RadioButton { Content = " týdny", FontSize = 24 };
         RadioButton mesice = new RadioButton { Content = " měsíce", FontSize = 24 };
         RadioButton roky = new RadioButton { Content = " roky", FontSize = 24 };

         // Nastavení zaškrtnutí RadioButton při vykreslování dle zvolené možnosti zobrazení
         switch(Zobrazeni)
         {
            case ZvoleneZobrazeniPrehledu.Dny:     dny.IsChecked = true;      break;
            case ZvoleneZobrazeniPrehledu.Tydny:   tydny.IsChecked = true;    break;
            case ZvoleneZobrazeniPrehledu.Mesice:  mesice.IsChecked = true;   break;
            case ZvoleneZobrazeniPrehledu.Roky:    roky.IsChecked = true;     break;
            default: break;
         }

         // Přidání událostí pro možnost reagovat na změnu volby 
         dny.Checked += Dny_Checked;
         tydny.Checked += Tydny_Checked;
         mesice.Checked += Mesice_Checked;
         roky.Checked += Roky_Checked;

         // Umístění výběru zobrazení na plátno
         MyCanvas.Children.Add(popisekZobrazeni);
         MyCanvas.Children.Add(dny);
         MyCanvas.Children.Add(tydny);
         MyCanvas.Children.Add(mesice);
         MyCanvas.Children.Add(roky);
         Canvas.SetLeft(popisekZobrazeni, 10);
         Canvas.SetTop(popisekZobrazeni, 150);
         Canvas.SetLeft(dny, 10);
         Canvas.SetTop(dny, 180);
         Canvas.SetLeft(tydny, 130);
         Canvas.SetTop(tydny, 180);
         Canvas.SetLeft(mesice, 10);
         Canvas.SetTop(mesice, 220);
         Canvas.SetLeft(roky, 130);
         Canvas.SetTop(roky, 220);


         // Popisek pro výběr měsíce
         Label MesicPopisek = new Label { FontSize = 14, Content = "Vyberte měsíc:" };

         // Popisek pro výběr roku
         Label RokPopisek = new Label { FontSize = 14, Content = "Vyberte rok:" };

         // Rozbalovací okno pro výběr měsíce
         ComboBox MesicVyber = new ComboBox { FontSize = 16, Width = 120, };

         // Rozbalovací okno pro výběr roku
         ComboBox RokVyber = new ComboBox { FontSize = 16, Width = 120, };

         // Přidání možností do rozbalovacího okna pro výběr měsíce
         for (int i = 1; i <= 12; i++)
         {
            ComboBoxItem item = new ComboBoxItem { Content = hodiny.VratMesicTextove(i) };
            MesicVyber.Items.Add(item);
         }

         // Přidání možností do rozbalovacího okna pro výběr roku
         for (int i = 2019; i <= DateTime.Now.Year; i++)
         {
            ComboBoxItem item = new ComboBoxItem { Content = i.ToString() };
            RokVyber.Items.Add(item);
         }

         // Nastavení defaultního výběru rozbalovacích oken
         MesicVyber.SelectedIndex = VybranyMesic - 1;
         RokVyber.SelectedIndex = VybranyRok - 2019;

         // Přidání událostí pro možnost reagovat na změnu výběru
         MesicVyber.SelectionChanged += MesicVyber_SelectionChanged;
         RokVyber.SelectionChanged += RokVyber_SelectionChanged;


         // Zobrazení zadávacích polí na základě vybraného zobrazení
         switch (Zobrazeni)
         {
            // Vykreslení prvků pro výběr vstupních dat v případě vykreslování ovládání pro zobrazení dnů
            case ZvoleneZobrazeniPrehledu.Dny:
               MyCanvas.Children.Add(MesicPopisek);
               MyCanvas.Children.Add(RokPopisek);
               MyCanvas.Children.Add(MesicVyber);
               MyCanvas.Children.Add(RokVyber);
               Canvas.SetLeft(MesicPopisek, 10);
               Canvas.SetTop(MesicPopisek, 270);
               Canvas.SetLeft(MesicVyber, 20);
               Canvas.SetTop(MesicVyber, 300);
               Canvas.SetLeft(RokPopisek, 10);
               Canvas.SetTop(RokPopisek, 350);
               Canvas.SetLeft(RokVyber, 20);
               Canvas.SetTop(RokVyber, 380);
               break;

            // Vykreslení prvků pro výběr vstupních dat v případě vykreslování ovládání pro zobrazení týdnů
            case ZvoleneZobrazeniPrehledu.Tydny:
               MyCanvas.Children.Add(MesicPopisek);
               MyCanvas.Children.Add(RokPopisek);
               MyCanvas.Children.Add(MesicVyber);
               MyCanvas.Children.Add(RokVyber);
               Canvas.SetLeft(MesicPopisek, 10);
               Canvas.SetTop(MesicPopisek, 270);
               Canvas.SetLeft(MesicVyber, 20);
               Canvas.SetTop(MesicVyber, 300);
               Canvas.SetLeft(RokPopisek, 10);
               Canvas.SetTop(RokPopisek, 350);
               Canvas.SetLeft(RokVyber, 20);
               Canvas.SetTop(RokVyber, 380);
               break;

            // Vykreslení prvků pro výběr vstupních dat v případě vykreslování ovládání pro zobrazení měsíců
            case ZvoleneZobrazeniPrehledu.Mesice:
               MyCanvas.Children.Add(RokPopisek);
               MyCanvas.Children.Add(RokVyber);
               Canvas.SetLeft(RokPopisek, 10);
               Canvas.SetTop(RokPopisek, 300);
               Canvas.SetLeft(RokVyber, 20);
               Canvas.SetTop(RokVyber, 350);
               break;

            // Vykreslení prvků pro výběr vstupních dat v případě vykreslování ovládání pro zobrazení roků
            case ZvoleneZobrazeniPrehledu.Roky:

               // Popisky pro výběr období pro zobrazení grafu
               Label popisekRokPocatecni = new Label { FontSize = 14, Content = "Vyberte počáteční rok:" };
               Label popisekRokKoncovy = new Label { FontSize = 14, Content = "Vyberte koncový rok:" };

               // Rozbalovací okno pro výběr roku
               ComboBox RokKoncovyVyber = new ComboBox { FontSize = 16, Width = 120, };

               // Přidání možností do rozbalovacího okna pro výběr koncového roku
               for (int i = 2019; i <= DateTime.Now.Year; i++)
               {
                  ComboBoxItem item = new ComboBoxItem { Content = i.ToString() };
                  RokKoncovyVyber.Items.Add(item);
               }

               // Nastavení defaultního výběru
               VybranyKoncovyRok = VybranyRok;
               RokKoncovyVyber.SelectedIndex = VybranyKoncovyRok - 2019;
               
               // Přidání události pro možnot reagovat na výběr roku
               RokKoncovyVyber.SelectionChanged += RokKoncovyVyber_SelectionChanged;

               // Vykreslení prvků na plátno
               MyCanvas.Children.Add(popisekRokPocatecni);
               MyCanvas.Children.Add(popisekRokKoncovy);
               MyCanvas.Children.Add(RokVyber);
               MyCanvas.Children.Add(RokKoncovyVyber);
               Canvas.SetLeft(popisekRokPocatecni, 10);
               Canvas.SetTop(popisekRokPocatecni, 270);
               Canvas.SetLeft(RokVyber, 20);
               Canvas.SetTop(RokVyber, 300);
               Canvas.SetLeft(popisekRokKoncovy, 10);
               Canvas.SetTop(popisekRokKoncovy, 350);
               Canvas.SetLeft(RokKoncovyVyber, 20);
               Canvas.SetTop(RokKoncovyVyber, 380);
               break;

            default: break;
         }

         // Zaškrtávací pole pro možnost výběru zobrazení příjmů
         CheckBox VykresleniPrijmuCheck = new CheckBox
         {
            Height = 20,
            Width = 20,
            IsChecked = true
         };

         // Zaškrtávací pole pro možnost výběru zobrazení výdajů
         CheckBox VykresleniVydajuCheck = new CheckBox
         {
            Height = 20,
            Width = 20,
            IsChecked = true
         };

         // Popisek pro zaškrtávací pole
         Label vykreslitPrijmy = new Label
         {
            FontSize = 18,
            Foreground = BarvaZelena,
            Content = "Zobrazit příjmy"
         };

         // Popisek pro zaškrtávací pole
         Label vykreslitVydaje = new Label
         {
            FontSize = 18,
            Foreground = BarvaCervena,
            Content = "Zobrazit výdaje"
         };

         // Přidání události pro možnost reagovat na výběr dat pro zobrazení od uživatele
         VykresleniVydajuCheck.Click += VykresleniVydajuCheck_Click;
         VykresleniPrijmuCheck.Click += VykresleniPrijmuCheck_Click;

         // Vykreslení zaškrtávacích polí včetně popisků na plátno
         MyCanvas.Children.Add(VykresleniPrijmuCheck);
         MyCanvas.Children.Add(VykresleniVydajuCheck);
         MyCanvas.Children.Add(vykreslitPrijmy);
         MyCanvas.Children.Add(vykreslitVydaje);
         Canvas.SetLeft(VykresleniPrijmuCheck, 30);
         Canvas.SetTop(VykresleniPrijmuCheck, 430);
         Canvas.SetLeft(VykresleniVydajuCheck, 30);
         Canvas.SetTop(VykresleniVydajuCheck, 470);
         Canvas.SetLeft(vykreslitPrijmy, 50);
         Canvas.SetTop(vykreslitPrijmy, 420);
         Canvas.SetLeft(vykreslitVydaje, 50);
         Canvas.SetTop(vykreslitVydaje, 460);
      }

      /// <summary>
      /// Vykreslení informační bubliny pro graf Prehled při výběru konkrétního bloku.
      /// </summary>
      /// <param name="MyCanvas">Plátno pro vykreslení informační bubliny</param>
      /// <param name="Blok">Vybraný blok, jehož data jsou vykreslena</param>
      public void VykresliInfoBublinuGrafuPrehledu(Canvas MyCanvas, BlokPrehledu Blok)
      {
         // Pomocné proměnné pro určení pozic prvků na plátně
         int VyskaCanvas = (int)MyCanvas.Height;
         int SirkaCanvas = (int)MyCanvas.Width;
         int SirkaNazev = Blok.Nazev.Length * 10;

         // Určení barvy pro vykreslení informační tabulky dle toho, zda se jedná o příjmy či výdaje
         Brush Barva;
         if (Blok.kategorie == KategoriePrijemVydaj.Prijem)
            Barva = BarvaZelena;
         else
            Barva = BarvaCervena;

         // Smazání obsahu pro možnost nového vykreslení
         MyCanvas.Children.Clear();

         // Pomocná proměnná
         string slovo = "";
         if (Blok.kategorie == KategoriePrijemVydaj.Prijem)
            slovo = "příjmů";
         else
            slovo = "výdajů";


         // Okraje
         Rectangle okraje = new Rectangle
         {
            Fill = Barva,
            Width = SirkaCanvas,
            Height = VyskaCanvas
         };

         // Pozadí informačního okna
         Rectangle pozadi = new Rectangle
         {
            Fill = Brushes.LightBlue,
            Width = SirkaCanvas - 2,
            Height = VyskaCanvas - 2
         };

         // Oddělení názvu
         Rectangle deliciCara = new Rectangle
         {
            Fill = Barva,
            Width = SirkaCanvas,
            Height = 1
         };

         // Přepážka
         Rectangle prepazka = new Rectangle
         {
            Fill = Barva,
            Width = 1,
            Height = VyskaCanvas / 3
         };

         // Přepážka
         Rectangle prepazka2 = new Rectangle
         {
            Fill = Barva,
            Width = 1,
            Height = VyskaCanvas / 3
         };

         // Název kategorie
         Label nazev = new Label
         {
            Content = Blok.Nazev,
            FontSize = 24
         };

         // Popisek informující zda se jedná o příjem nebo o výdaj
         Label PrijemVydaj = new Label
         {
            FontSize = 24
         };

         // Rozmezí datumů pro který je graf vykreslen
         Label datum = new Label
         {
            Content = String.Format("\t{0}.{1}.{2}\t\t->\t{3}.{4}.{5}", Blok.DatumPocatecni.Day, Blok.DatumPocatecni.Month,
            Blok.DatumPocatecni.Year, Blok.DatumKoncove.Day, Blok.DatumKoncove.Month, Blok.DatumKoncove.Year),
            FontSize = 20
         };

         // Informace o celkové hodnotě
         Label hodnota = new Label
         {
            Content = "Celková hodnota " + slovo + " je " + Blok.CelkovaHodnota.ToString() + " Kč.",
            FontSize = 22
         };

         // Počet záznamů v bloku kategorie
         Label pocet = new Label
         {
            Content = "Počet záznamů: " + Blok.PocetZaznamu.ToString(),
            FontSize = 18
         };


         // Určení popisku a barvy na základě zda sejedná o příjem nebo o výdaj
         if (Blok.kategorie == KategoriePrijemVydaj.Prijem)
         {
            PrijemVydaj.Content = "PŘÍJMY";
            PrijemVydaj.Foreground = BarvaZelena;
         }
         else
         {
            PrijemVydaj.Content = "VÝDAJE";
            PrijemVydaj.Foreground = BarvaCervena;
         }


         // Přidání okrajů na plátno
         MyCanvas.Children.Add(okraje);
         Canvas.SetLeft(okraje, 0);
         Canvas.SetTop(okraje, 0);

         // Přidání pozadí na plátno
         MyCanvas.Children.Add(pozadi);
         Canvas.SetLeft(pozadi, 1);
         Canvas.SetTop(pozadi, 1);

         // Přidání dělící čáry na plátno
         MyCanvas.Children.Add(deliciCara);
         Canvas.SetLeft(deliciCara, 0);
         Canvas.SetTop(deliciCara, VyskaCanvas / 3 - 1);

         // Přidání názvu na plátno
         MyCanvas.Children.Add(nazev);
         Canvas.SetLeft(nazev, 4);
         Canvas.SetTop(nazev, 6);

         // Přidání data na plátno
         MyCanvas.Children.Add(datum);
         Canvas.SetLeft(datum, SirkaNazev + 135);
         Canvas.SetTop(datum, 6);

         // Přidání popisku na plátno
         MyCanvas.Children.Add(PrijemVydaj);
         Canvas.SetRight(PrijemVydaj, 11);
         Canvas.SetTop(PrijemVydaj, 6);

         // Přidání přepážky na plátno
         MyCanvas.Children.Add(prepazka);
         MyCanvas.Children.Add(prepazka2);
         Canvas.SetLeft(prepazka, SirkaNazev + 34);
         Canvas.SetTop(prepazka, 1);
         Canvas.SetRight(prepazka2, 200);
         Canvas.SetTop(prepazka2, 1);

         // Přidání celkové hodnoty na plátno
         MyCanvas.Children.Add(hodnota);
         Canvas.SetLeft(hodnota, 24);
         Canvas.SetTop(hodnota, VyskaCanvas / 3 + 20);

         // Přidání počtu záznamů na plátno
         MyCanvas.Children.Add(pocet);
         Canvas.SetRight(pocet, 16);
         Canvas.SetBottom(pocet, 6);
      }

      /// <summary>
      /// Metoda pro vykreslení oblasti pod grafem, tedy popisky osy X a ovládací šipky pro pohyb mezi stránkami grafu.
      /// </summary>
      /// <param name="MyCanvas">Plátno pro vykreslení</param>
      public void VykresliOblastPodGrafemPrehledu(Canvas MyCanvas)
      {
         // Pomocné proměnné pro určení pozic prvků na plátně
         int VyskaCanvas = (int)MyCanvas.Height;
         int SirkaCanvas = (int)MyCanvas.Width;

         // Smazání obsahu pro možnost nového vykreslení
         MyCanvas.Children.Clear();

         // Levá šipka pro možnost měnit číslo stránky grafu (pohyb v grafu)
         Path LevaSipka = new Path
         {
            Stroke = BarvaZluta,
            Fill = BarvaCervena,
            StrokeThickness = 5,
            Data = Geometry.Parse("m0,40 l40,-40 l0,20 l50,0 l0,40 l-50,0 l0,20 l-40,-40")
         };

         // Pravá šipka pro možnost měnit číslo stránky grafu (pohyb v grafu)
         Path PravaSipka = new Path
         {
            Stroke = BarvaZluta,
            Fill = BarvaCervena,
            StrokeThickness = 5,
            Data = Geometry.Parse("m0,40 l0,20 l50,0 l0,20 l40,-40 l-40,-40 l0,20 l-50,0 l0,20")
         };

         // Přidání události pro možnost kliknutí na šipku
         LevaSipka.MouseDown += LevaSipkaProStrankyGrafuPrehled_MouseDown;
         PravaSipka.MouseDown += PravaSipkaProStrankyGrafuPrehled_MouseDown;

         // Vykreslení šipek na plátno pokud je k dispozici více stránek grafu pro vykreslení
         if (MaximalniPocetStranGrafu > 1)
         {
            MyCanvas.Children.Add(LevaSipka);
            MyCanvas.Children.Add(PravaSipka);
            Canvas.SetRight(LevaSipka, 140);
            Canvas.SetBottom(LevaSipka, 20);
            Canvas.SetRight(PravaSipka, 20);
            Canvas.SetBottom(PravaSipka, 20);
         }

         // Vypsání popisků osy X (zvolené období 1 bloku)
         for (int i = 1; i <= PopiskyGrafuPrehledu.Count; i++)
         {
            // Vytvoření popisku
            Label popisek = new Label
            {
               FontSize = 18,
               Content = PopiskyGrafuPrehledu[i - 1],
               Foreground = Brushes.DarkRed
            };

            // Naklonění textu
            RotateTransform rotace = new RotateTransform(45);
            popisek.RenderTransform = rotace;

            // Vykreslení popisku na plátno
            MyCanvas.Children.Add(popisek);
            Canvas.SetLeft(popisek, SouradniceX_PrvniPrvekPrehledu + ((i - 1) * OdstupHodnotX_Prehled));
            Canvas.SetTop(popisek, -20);
         }
      }



      /// <summary>
      /// Úvodní nastavení grafu Kategorie pro možnost 1. vykreslení
      /// </summary>
      public void UvodniNastaveniGrafuKategorie()
      {
         // Nastavení vykreslování grafu na první stránku
         CisloStrankyGrafu = 0;

         // Výchozí nastavení zobrazení
         VykreslenyGraf = 1;
         VykreslitPrijmy = 1;
         VykreslitVydaje = 1;

         // Nastavení počátečního časového rozmezí pro výběr dat
         PocatecniDatum = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1);
         KoncoveDatum = DateTime.Now;

         // Volání pomocných metod pro vytvoření úvodního zobrazení grafu
         VytvorBlokyProGrafKategorii(PocatecniDatum, KoncoveDatum);
         VytvorNazevGrafu("Zobrazení kategorií");
         VytvorLegenduGrafu();
         VykresliGrafKategorie(PlatnoGrafuCanvas, ZobrazeneBlokyPrijmuKategorie, ZobrazeneBlokyVydajuKategorie);
         VykresliOblastPodGrafemKategorie(PlatnoInfoOblastiCanvas);
         VykresliOvladaciPrvkyGrafuKategorie(PlatnoOvladacichPrvkuCanvas);
      }

      /// <summary>
      /// Aktualizace vykreslení při změně dat nebo nastavení
      /// </summary>
      public void AktualizujGrafKategorii()
      {
         VytvorBlokyProGrafKategorii(PocatecniDatum, KoncoveDatum);
         VykresliGrafKategorie(PlatnoGrafuCanvas, ZobrazeneBlokyPrijmuKategorie, ZobrazeneBlokyVydajuKategorie);
         VykresliOblastPodGrafemKategorie(PlatnoInfoOblastiCanvas);
      }

      /// <summary>
      /// Nalezení maximální hodnoty prvků v grafu pro určení hodnot na ose Y
      /// </summary>
      private void NajdiMaximalniHodnotuPrvkuKategorie()
      {
         // Smazání maximální hodnoty pro možnost opětovného nastavení
         MaximalniHodnota = 0;

         // Kolekce příjmů se projde pokud je určena k vykreslování
         if (VykreslitPrijmy == 1)
         {
            // Nalezení nejvyšší hodnoty mezi příjmy
            foreach (BlokKategorie blok in ZobrazeneBlokyPrijmuKategorie)
            {
               // Aktualizace maximální hodnoty v případě nalezení vyšší hodnoty než poslední nalezená maximální hodnota
               if (blok.CelkovaHodnota > MaximalniHodnota)
                  MaximalniHodnota = blok.CelkovaHodnota;
            }
         }

         // Kolekce výdajů se projde pokud je určena k vykreslování
         if (VykreslitVydaje == 1)
         {
            // Nalezení nejvyšší hodnoty mezi výdaji
            foreach (BlokKategorie blok in ZobrazeneBlokyVydajuKategorie)
            {
               // Aktualizace maximální hodnoty v případě nalezení vyšší hodnoty než poslední nalezená maximální hodnota
               if (blok.CelkovaHodnota > MaximalniHodnota)
                  MaximalniHodnota = blok.CelkovaHodnota;
            }
         }
         
         // Pokud je maximální hodnota menší než počet hodnot na ose Y nastaví se maximální hodnota právě na tento počet
         if (MaximalniHodnota < PocetHodnot)
            MaximalniHodnota = PocetHodnot;

         // Nastavení hodnot na ose Y dle nejvyšší nalezené hodnoty prvků
         NastavHodnotyNaOseY(MaximalniHodnota);
      }

      /// <summary>
      /// Vytvoření bloků obsahujících data potřebná pro vykreslení grafu Kategorie. 
      /// Metoda statisticky vyhodnotí všechny záznamy v zadaném časovém období a roztřídí je dle kategorií do jednotlivých bloků.
      /// </summary>
      /// <param name="PocatecniDatum">Počáteční datum pro statistické vyhodnocení</param>
      /// <param name="KoncoveDatum">Koncové datum pro statistické vyhodnocení</param>
      public void VytvorBlokyProGrafKategorii(DateTime PocatecniDatum, DateTime KoncoveDatum)
      {
         // Smazání obsahu kolekce pro nové načtení dat
         KolekceBlokuPrijjmuKategorie.Clear();
         KolekceBlokuVydajuKategorie.Clear();

         // Cyklus postupně projde všechny kategorie záznamů a vybere všechny záznamy z dané kategorie
         for (int i = 0; i < 23; i++)
         {
            // Vytvoření nových bloku
            BlokKategorie BlokPrijmu = new BlokKategorie();
            BlokKategorie BlokVydaju = new BlokKategorie();

            // Inicializace hodnot do úvodního nastavení
            BlokPrijmu.PocetZaznamu = 0;
            BlokPrijmu.CelkovaHodnota = 0;
            BlokVydaju.PocetZaznamu = 0;
            BlokVydaju.CelkovaHodnota = 0;

            // Přidání názvu kategorie
            BlokPrijmu.Nazev = VratNazevKategorie((Kategorie)i);
            BlokVydaju.Nazev = VratNazevKategorie((Kategorie)i);

            // Uložení kategorie bloku
            BlokPrijmu.kategorie = KategoriePrijemVydaj.Prijem;
            BlokVydaju.kategorie = KategoriePrijemVydaj.Vydaj;

            // Uložení počátečního a koncového data do bloku
            BlokPrijmu.DatumPocatecni = PocatecniDatum;
            BlokPrijmu.DatumKoncove = KoncoveDatum;
            BlokVydaju.DatumPocatecni = PocatecniDatum;
            BlokVydaju.DatumKoncove = KoncoveDatum;

            // Uložení barvy bloku
            BlokPrijmu.Barva = BarvaZelena;
            BlokVydaju.Barva = BarvaCervena;


            // Cyklus postupně projde všechny záznamy pro nalezení všech záznamů z dané kategorie
            foreach (Zaznam zaznam in KolekceZaznamu)
            {
               // Záznam spadá do vybrané kategorie
               if (zaznam.Kategorie == (Kategorie)i)
               {
                  // Kontrola zda záznam splňuje zadané časové období
                  if (zaznam.Datum <= KoncoveDatum && zaznam.Datum >= PocatecniDatum)
                  {
                     // Záznam spadá mezi příjmy
                     if (zaznam.PrijemNeboVydaj == KategoriePrijemVydaj.Prijem)
                     {
                        BlokPrijmu.PocetZaznamu++;
                        BlokPrijmu.CelkovaHodnota += zaznam.Hodnota_PrijemVydaj;
                     }

                     // Záznam spadá mezi výdaje
                     else
                     {
                        BlokVydaju.PocetZaznamu++;
                        BlokVydaju.CelkovaHodnota += zaznam.Hodnota_PrijemVydaj;
                     }

                  }
               }

            }

            // Přidání bloku do kolekce
            KolekceBlokuPrijjmuKategorie.Add(BlokPrijmu);
            KolekceBlokuVydajuKategorie.Add(BlokVydaju);
         }

         // Určení počtu stran pro vykreslení na zázkladě počtu prvků v kolekci pro vykreslení
         MaximalniPocetStranGrafu = (int)Math.Ceiling((double)KolekceBlokuPrijjmuKategorie.Count / (double)PocetPrvkuKategorie);
      }

      /// <summary>
      /// Výběr potřebného počtu bloků pro zobrazení 1 stránky grafu
      /// </summary>
      public void VyberBlokyKategorieKZobrazeni()
      {
         // Určení indexu bloku pro výběr bloku z kolekce na základě vykreslované stránky grafu
         int PrvniBlok = CisloStrankyGrafu * PocetPrvkuKategorie;

         // Smazání kolekcí zobrazovaných dat pro možnost přidání nových bloků pro zobrazení
         ZobrazeneBlokyPrijmuKategorie.Clear();
         ZobrazeneBlokyVydajuKategorie.Clear();

         // Pokud je v kolekci více bloků, vybere se pouze požadovaný počet bloků k vykreslení
         if ((PrvniBlok + PocetPrvkuKategorie) <= KolekceBlokuPrijjmuKategorie.Count)
         {
            // Postupné přidání bloků z celkové kolekce do kolekce bloků určených k zobrazení
            for (int index = PrvniBlok; index < (PrvniBlok + PocetPrvkuKategorie); index++)
            {
               ZobrazeneBlokyPrijmuKategorie.Add(KolekceBlokuPrijjmuKategorie[index]);
               ZobrazeneBlokyVydajuKategorie.Add(KolekceBlokuVydajuKategorie[index]);
            }
         }

         // Pokud v kolekci zbývá jen několik bloků, vyberou se všechny zbývající bloky pro vykreslení na poslední stránku grafu
         else
         {
            // Postupné přidání bloků z celkové kolekce do kolekce bloků určených k zobrazení
            for (int index = PrvniBlok; index < KolekceBlokuPrijjmuKategorie.Count; index++)
            {
               ZobrazeneBlokyPrijmuKategorie.Add(KolekceBlokuPrijjmuKategorie[index]);
               ZobrazeneBlokyVydajuKategorie.Add(KolekceBlokuVydajuKategorie[index]);
            }
         }

         // Uložení popisků kategorií vybraných bloků 
         VytvorPopiskyKategorii();
      }

      /// <summary>
      /// Uložení textové reprezentace vybraných bloků.
      /// </summary>
      public void VytvorPopiskyKategorii()
      {
         // Smazání obsahu kolekce pro opětovné vytvoření nových popisků při změně dat
         ZobrazeneKategorie.Clear();

         // Pomocné proměnné pro rozhodnutí, která z kolekcí obsahuje více prvků
         int PocetPrijmu = ZobrazeneBlokyPrijmuKategorie.Count;
         int PocetVydaju = ZobrazeneBlokyVydajuKategorie.Count;
         ObservableCollection<BlokKategorie> KategorieVGrafu = new ObservableCollection<BlokKategorie>();

         // Načtení kolekce s více prvky
         if (PocetPrijmu <= PocetVydaju)
            KategorieVGrafu = ZobrazeneBlokyVydajuKategorie;
         else
            KategorieVGrafu = ZobrazeneBlokyPrijmuKategorie;


         // Uložení jednotlivých názvů (kategorií) do kolekce
         foreach (BlokKategorie blok in KategorieVGrafu)
         {
            ZobrazeneKategorie.Add(blok.Nazev);
         }
      }

      /// <summary>
      /// Převedení kategorie na název do kolekce pro možnost vypsání názvu kategorie s diakritikou
      /// </summary>
      /// <param name="kategorie">Kategorie pro převod na text</param>
      /// <returns>Název kategorie</returns>
      public string VratNazevKategorie(Kategorie kategorie)
      {
         // Návratová proměnná
         string NazevKategorie = "";

         // Přiřazení názvu kategorie s diakritikou do návratové proměnné
         switch(kategorie)
         {
            case Kategorie.Alkohol:          NazevKategorie = "Alkohol";               break;
            case Kategorie.Auto:             NazevKategorie = "Auto";                  break;
            case Kategorie.Brigada:          NazevKategorie = "Brigáda";               break;
            case Kategorie.Cestovani:        NazevKategorie = "Cestování";             break;
            case Kategorie.Dar:              NazevKategorie = "Dar";                   break;
            case Kategorie.Divadlo:          NazevKategorie = "Divadlo";               break;
            case Kategorie.DomaciMazlicek:   NazevKategorie = "Domácí mazlíček";       break;
            case Kategorie.Domacnost:        NazevKategorie = "Domáctnost";            break;
            case Kategorie.Domov:            NazevKategorie = "Domov / Dům";           break;
            case Kategorie.Inkaso:           NazevKategorie = "Inkaso";                break;
            case Kategorie.Jidlo:            NazevKategorie = "Potraviny";             break;
            case Kategorie.Kino:             NazevKategorie = "Kino";                  break;
            case Kategorie.Kosmetika:        NazevKategorie = "Kosmetika";             break;
            case Kategorie.Kultura:          NazevKategorie = "Kultura";               break;
            case Kategorie.Najem:            NazevKategorie = "Nájem";                 break;
            case Kategorie.Obleceni:         NazevKategorie = "Oblečení";              break;
            case Kategorie.Partner:          NazevKategorie = "Partner / Partnerka";   break;
            case Kategorie.PC:               NazevKategorie = "Pc";                    break;
            case Kategorie.Restaurace:       NazevKategorie = "Restaurace";            break;
            case Kategorie.Rodina:           NazevKategorie = "Rodina";                break;
            case Kategorie.Sport:            NazevKategorie = "Sport";                 break;
            case Kategorie.Vypalta:          NazevKategorie = "Výplata";               break;
            case Kategorie.Zdravi:           NazevKategorie = "Zdraví";                break;
            default: break;
         }
         return NazevKategorie;
      }

      /// <summary>
      /// Vykreslení sloupcového grafu.
      /// Na ose Y jsou hodnoty příjmů a výdajů v součtu za zvolené časové období.
      /// Na ose X jsou jednotlivé kategorie.
      /// </summary>
      /// <param name="MyCanvas">Plátno pro vykreslení</param>
      /// <param name="KolekcePrijmuKZobrazeni">Kolekce bloků reprezentující všechny příjmy v určitém časovém období</param>
      /// <param name="KolekceVydajuKZobrazeni">Kolekce bloků reprezentující všechny výdaje v určitém časovém období</param>
      public void VykresliGrafKategorie(Canvas MyCanvas, ObservableCollection<BlokKategorie> KolekcePrijmuKZobrazeni, ObservableCollection<BlokKategorie> KolekceVydajuKZobrazeni)
      {
         // Načtení rozměrů plátna pro vykreslení grafu
         int Sirka = (int)MyCanvas.Width;
         int Vyska = (int)MyCanvas.Height - 10;

         // Smazání obsahu plátno pro možnost nového vykreslení
         MyCanvas.Children.Clear();

         // Výběr bloků určených k zobrazení v grafu
         VyberBlokyKategorieKZobrazeni();

         // Kontrolní podmínka pro možnost že pro danou stránku nejsou vybrány žádné bloky
         if (ZobrazeneBlokyPrijmuKategorie.Count == 0 && ZobrazeneBlokyVydajuKategorie.Count == 0)
         {
            // Změna čísla stránky a opětovný výběr bloků
            CisloStrankyGrafu--;
            VyberBlokyKategorieKZobrazeni();
         }

         // Výpočet maximální hodnoty zobrazených bloků pro určení hodnot na ose Y
         NajdiMaximalniHodnotuPrvkuKategorie();

         // Pomocná proměnná pro uchování šířky bloku obsahující popisky osy Y (začátek vykreslování samotného grafu)
         int Odsazeni = HodnotyOsyY[HodnotyOsyY.Count - 1].ToString().Length * 10;

         // Přičtení velikosti označení hodnoty (Kč za číslem)
         Odsazeni += 30;

         // Pomocná proměnná pro snadnější vykreslení hodnot na osu Y
         int mezeraY = (int)Math.Floor((double)Vyska / (double)HodnotyOsyY.Count);

         // Pomocná proměnná pro snadnější vykreslování prvků na osu X
         int mezeraX = (int)Math.Floor((double)(Sirka - Odsazeni) / (double)(PocetPrvkuKategorie + 0.5));

         // Určení šířky sloupce reprezentující 1 blok dat
         int SirkaSloupce = (mezeraX - 10) / 3;

         // Uložení souřadnice prvního bloku pro možnost vykreslení popisu osy X na správné souřdnice
         SouradniceX_PrvniSloupecKategorie = Odsazeni + mezeraX;
         OdstupHodnotX_Kategorie = mezeraX;

         // Levé ohraničené oblasti grafu (osa Y)
         Rectangle LeveOhraniceni = new Rectangle
         {
            Width = 2,
            Height = Vyska + 10,
            Fill = BarvaCerna
         };

         // Spodní ohraničené oblasti grafu (osa X)
         Rectangle SpodniOhraniceni = new Rectangle
         {
            Width = Sirka - Odsazeni,
            Height = 2,
            Fill = BarvaCerna
         };

         // Přidání ohraničení na plátno
         MyCanvas.Children.Add(LeveOhraniceni);
         MyCanvas.Children.Add(SpodniOhraniceni);
         Canvas.SetLeft(LeveOhraniceni, Odsazeni - 2);
         Canvas.SetBottom(LeveOhraniceni, 0);
         Canvas.SetLeft(SpodniOhraniceni, Odsazeni);
         Canvas.SetBottom(SpodniOhraniceni, 0);


         // Přidání popisku osy Y včetně pomocných čar značících úroveň na ose Y pro lepší orientaci v grafu
         for (int i = 1; i <= HodnotyOsyY.Count; i++) 
         {
            // Číselná hodnota na ose Y
            Label popisek = new Label
            {
               FontSize = 14,
               Content = HodnotyOsyY[i - 1].ToString() + " Kč"
            };

            // Pomocná čára
            Rectangle cara = new Rectangle
            {
               Width = Sirka - Odsazeni,
               Height = 1,
               Fill = Brushes.Gray
            };

            // Vyznačení hodnoty na ose
            Rectangle oznaceni = new Rectangle
            {
               Width = 10,
               Height = 1,
               Fill = BarvaCerna
            };

            // Přidání prvků a vykreslení na plátno
            MyCanvas.Children.Add(popisek);
            MyCanvas.Children.Add(cara);
            MyCanvas.Children.Add(oznaceni);
            Canvas.SetLeft(popisek, 0);
            Canvas.SetBottom(popisek, (i * mezeraY) - 12);
            Canvas.SetLeft(cara, Odsazeni);
            Canvas.SetBottom(cara, (i * mezeraY));
            Canvas.SetLeft(oznaceni, Odsazeni - oznaceni.Width / 2 - 1);
            Canvas.SetBottom(oznaceni, (i * mezeraY));
         }


         // Přidání prvků na osu X včetně vykreslení grafických bloků reprezentující data
         for (int i = 1; i <= PocetPrvkuKategorie; i++)
         {
            // Vyznačení hodnoty na ose
            Rectangle oznaceni = new Rectangle
            {
               Width = 1,
               Height = 7,
               Fill = BarvaCerna
            };

            // Přidání prvků a vykreslení na plátno
            MyCanvas.Children.Add(oznaceni);
            Canvas.SetLeft(oznaceni, Odsazeni + (i * mezeraX));
            Canvas.SetBottom(oznaceni, 0);


            // Pokud je blok v kolekci příjmů pro vykreslení, vytvoří se sloupec reprezentující jeho data -> vykreslení sloupců příjmů
            if (i <= ZobrazeneBlokyPrijmuKategorie.Count && VykreslitPrijmy == 1) 
            {
               // Výpočet výšky sloupce na základě jeho hodnoty
               int VyskaSloupce = (int)Math.Ceiling((KolekcePrijmuKZobrazeni[i - 1].CelkovaHodnota * Vyska) / MaximalniHodnota);

               // Pokud na vykreslované stránce nejsou žádné bloky k vykreslení, nastaví se výška sloupce na 0 pro možnost vykreslení prázdného grafu
               if (VyskaSloupce < 0)
                  VyskaSloupce = 0;

               // Vytvoření sloupce reprezentující blok příjmů
               Rectangle prijem = new Rectangle
               {
                  Width = SirkaSloupce,
                  Height = VyskaSloupce,
                  Fill = BarvaZelena
               };

               // Uložení indexu bloku do názvu sloupce pro možnost pozdější identifikace bloku ze sloupce
               prijem.Name = "sloupec" + (i - 1).ToString();

               // Přidání událoti pro možnost reagovat na pohyb myší na sloupci
               prijem.MouseMove += PrijemKategorie_MouseMove;
               prijem.MouseLeave += PrijemKategorie_MouseLeave;

               // Vykreslení sloupce na plátno
               MyCanvas.Children.Add(prijem);
               Canvas.SetLeft(prijem, Odsazeni + (i * mezeraX) - 5 - SirkaSloupce);
               Canvas.SetBottom(prijem, 2);
            }

            // Pokud je blok v kolekci výdajů pro vykreslení, vytvoří se sloupec reprezentující jeho data -> vykreslení sloupců výdajů
            if (i <= ZobrazeneBlokyVydajuKategorie.Count && VykreslitVydaje == 1) 
            {
               // Výpočet výšky sloupce na základě jeho hodnoty
               int VyskaSloupce = (int)Math.Ceiling((KolekceVydajuKZobrazeni[i - 1].CelkovaHodnota * Vyska) / MaximalniHodnota);

               // Pokud na vykreslované stránce nejsou žádné bloky k vykreslení, nastaví se výška sloupce na 0 pro možnost vykreslení prázdného grafu
               if (VyskaSloupce < 0)
                  VyskaSloupce = 0;

               // Vytvoření sloupce reprezentující blok příjmů
               Rectangle vydaj = new Rectangle
               {
                  Width = SirkaSloupce,
                  Height = VyskaSloupce,
                  Fill = BarvaCervena
               };

               // Uložení indexu bloku do názvu sloupce pro možnost pozdější identifikace bloku ze sloupce
               vydaj.Name = "sloupec" + (i - 1).ToString();

               // Přidání událoti pro možnost reagovat na pohyb myší na sloupci
               vydaj.MouseMove += VydajKategorie_MouseMove;
               vydaj.MouseLeave += VydajKategorie_MouseLeave;

               // Vykreslení sloupce na plátno
               MyCanvas.Children.Add(vydaj);
               Canvas.SetLeft(vydaj, Odsazeni + (i * mezeraX) + 5);
               Canvas.SetBottom(vydaj, 2);
            }

         }

      }

      /// <summary>
      /// Vykreslení panelu s ovládacími prvky grafu včetně legendy ke grafu.
      /// </summary>
      /// <param name="MyCanvas">Plátno pro vykreslení</param>
      public void VykresliOvladaciPrvkyGrafuKategorie(Canvas MyCanvas)
      {
         // Smazání plátna pro možnost nového vykreslení
         MyCanvas.Children.Clear();

         // Vykrelení názvu grafu a legendy
         MyCanvas.Children.Add(GrafickyNazev);
         MyCanvas.Children.Add(Legenda);
         Canvas.SetLeft(GrafickyNazev, 0);
         Canvas.SetTop(GrafickyNazev, 140);
         Canvas.SetLeft(Legenda, 0);
         Canvas.SetTop(Legenda, 0);

         // Levá šipka pro možnost vykreslit jiný graf
         Path LevaSipka = new Path
         {
            Stroke = BarvaCervena,
            Fill = BarvaModra,
            StrokeThickness = 3,
            Data = Geometry.Parse("m0,20 l20,-20 l0,10 l25,0 l0,20 l-25,0 l0,10 l-20,-20")
         };

         // Pravá šipka pro možnost vykreslit jiný graf
         Path PravaSipka = new Path
         {
            Stroke = BarvaCervena,
            Fill = BarvaModra,
            StrokeThickness = 3,
            Data = Geometry.Parse("m0,20 l0,10 l25,0 l0,10 l20,-20 l-20,-20 l0,10 l-25,0 l0,10")
         };

         // Přidání události pro možnost kliknutí na šipku
         LevaSipka.MouseDown += LevaSipkaPrepinaniGrafu_MouseDown;
         PravaSipka.MouseDown += PravaSipkaPrepinaniGrafu_MouseDown;

         // Vykreslení šipek na plátno
         MyCanvas.Children.Add(LevaSipka);
         MyCanvas.Children.Add(PravaSipka);
         Canvas.SetRight(LevaSipka, 70);
         Canvas.SetTop(LevaSipka, 10);
         Canvas.SetRight(PravaSipka, 10);
         Canvas.SetTop(PravaSipka, 10);


         // Popisek pro výběr počátečního data
         Label DatumPocatecniPopis = new Label
         {
            FontSize = 14,
            Content = "Vyberte počáteční datum:"
         };

         // Popisek pro výběr koncového data
         Label DatumKoncovePopis = new Label
         {
            FontSize = 14,
            Content = "Vyberte koncové datum:"
         };

         // Blok pro zadání počátečního data
         DatePicker DatumPocatecni = new DatePicker
         {
            FontSize = 16,
            SelectedDate = PocatecniDatum,
            Height = 30,
            Width = 150,
            IsTodayHighlighted = true
         };

         // Blok pro zadání koncového data
         DatePicker DatumKoncove = new DatePicker
         {
            FontSize = 16,
            SelectedDate = KoncoveDatum,
            Height = 30,
            Width = 150,
            IsTodayHighlighted = true
         };

         // Přidání událostí pro možnost reagovat na změnu hodnoty v zadávacím poli
         DatumPocatecni.SelectedDateChanged += DatumPocatecniKategorie_SelectedDateChanged;
         DatumKoncove.SelectedDateChanged += DatumKoncoveKategorie_SelectedDateChanged;

         // Vykreslení zadávacích polí na plátno
         MyCanvas.Children.Add(DatumPocatecniPopis);
         MyCanvas.Children.Add(DatumKoncovePopis);
         MyCanvas.Children.Add(DatumPocatecni);
         MyCanvas.Children.Add(DatumKoncove);
         Canvas.SetLeft(DatumPocatecniPopis, 20);
         Canvas.SetTop(DatumPocatecniPopis, 200);
         Canvas.SetLeft(DatumPocatecni, 30);
         Canvas.SetTop(DatumPocatecni, 230);
         Canvas.SetLeft(DatumKoncovePopis, 20);
         Canvas.SetTop(DatumKoncovePopis, 300);
         Canvas.SetLeft(DatumKoncove, 30);
         Canvas.SetTop(DatumKoncove, 330);

         // Zaškrtávací pole pro možnost výběru zobrazení příjmů
         CheckBox VykresleniPrijmuCheck = new CheckBox
         {
            Height = 20,
            Width = 20,
            IsChecked = true
         };

         // Zaškrtávací pole pro možnost výběru zobrazení výdajů
         CheckBox VykresleniVydajuCheck = new CheckBox
         {
            Height = 20,
            Width = 20,
            IsChecked = true
         };

         // Popisek pro zaškrtávací pole
         Label vykreslitPrijmy = new Label
         {
            FontSize = 18,
            Foreground = BarvaZelena,
            Content = "Zobrazit příjmy"
         };

         // Popisek pro zaškrtávací pole
         Label vykreslitVydaje = new Label
         {
            FontSize = 18,
            Foreground = BarvaCervena,
            Content = "Zobrazit výdaje"
         };

         // Přidání události pro možnost reagovat na výběr dat pro zobrazení od uživatele
         VykresleniVydajuCheck.Click += VykresleniVydajuCheck_Click;
         VykresleniPrijmuCheck.Click += VykresleniPrijmuCheck_Click;

         // Vykreslení zaškrtávacích polí včetně popisků na plátno
         MyCanvas.Children.Add(VykresleniPrijmuCheck);
         MyCanvas.Children.Add(VykresleniVydajuCheck);
         MyCanvas.Children.Add(vykreslitPrijmy);
         MyCanvas.Children.Add(vykreslitVydaje);
         Canvas.SetLeft(VykresleniPrijmuCheck, 10);
         Canvas.SetTop(VykresleniPrijmuCheck, 410);
         Canvas.SetLeft(VykresleniVydajuCheck, 10);
         Canvas.SetTop(VykresleniVydajuCheck, 450);
         Canvas.SetLeft(vykreslitPrijmy, 30);
         Canvas.SetTop(vykreslitPrijmy, 400);
         Canvas.SetLeft(vykreslitVydaje, 30);
         Canvas.SetTop(vykreslitVydaje, 440);
      }

      /// <summary>
      /// Vykreslení informační bubliny pro graf Kategorie při výběru konkrétního bloku.
      /// </summary>
      /// <param name="MyCanvas">Plátno pro vykreslení informační bubliny</param>
      /// <param name="Blok">Vybraný blok, jehož data jsou vykreslena</param>
      public void VykresliInfoBublinuGrafuKategorie(Canvas MyCanvas, BlokKategorie Blok)
      {
         // Pomocné proměnné pro určení pozic prvků na plátně
         int VyskaCanvas = (int)MyCanvas.Height;
         int SirkaCanvas = (int)MyCanvas.Width;
         int SirkaNazev = Blok.Nazev.Length * 10;

         // Určení barvy pro vykreslení informační tabulky dle toho, zda se jedná o příjmy či výdaje
         Brush Barva;
         if (Blok.kategorie == KategoriePrijemVydaj.Prijem)
            Barva = BarvaZelena;
         else
            Barva = BarvaCervena;

         // Smazání obsahu pro možnost nového vykreslení
         MyCanvas.Children.Clear();

         // Pomocná proměnná
         string slovo = "";
         if (Blok.kategorie == KategoriePrijemVydaj.Prijem)
            slovo = "příjmů";
         else
            slovo = "výdajů";


         // Okraje
         Rectangle okraje = new Rectangle
         {
            Fill = Barva,
            Width = SirkaCanvas,
            Height = VyskaCanvas
         };

         // Pozadí informačního okna
         Rectangle pozadi = new Rectangle
         {
            Fill = Brushes.LightBlue,
            Width = SirkaCanvas - 2,
            Height = VyskaCanvas - 2
         };

         // Oddělení názvu
         Rectangle deliciCara = new Rectangle
         {
            Fill = Barva,
            Width = SirkaCanvas,
            Height = 1
         };

         // Přepážka
         Rectangle prepazka = new Rectangle
         {
            Fill = Barva,
            Width = 1,
            Height = VyskaCanvas / 3
         };

         // Přepážka
         Rectangle prepazka2 = new Rectangle
         {
            Fill = Barva,
            Width = 1,
            Height = VyskaCanvas / 3
         };

         // Název kategorie
         Label nazev = new Label
         {
            Content = Blok.Nazev,
            FontSize = 24
         };

         // Popisek informující zda se jedná o příjem nebo o výdaj
         Label PrijemVydaj = new Label
         {
            FontSize = 24
         };

         // Rozmezí datumů pro který je graf vykreslen
         Label datum = new Label
         {
            Content = String.Format("\t{0}.{1}.{2}\t\t->\t{3}.{4}.{5}", Blok.DatumPocatecni.Day, Blok.DatumPocatecni.Month, 
            Blok.DatumPocatecni.Year, Blok.DatumKoncove.Day, Blok.DatumKoncove.Month, Blok.DatumKoncove.Year),
            FontSize = 20
         };

         // Informace o celkové hodnotě
         Label hodnota = new Label
         {
            Content = "Celková hodnota " + slovo + " je " + Blok.CelkovaHodnota.ToString() + " Kč.",
            FontSize = 22
         };

         // Počet záznamů v bloku kategorie
         Label pocet = new Label
         {
            Content = "Počet záznamů: " + Blok.PocetZaznamu.ToString(),
            FontSize = 18
         };


         // Určení popisku a barvy na základě zda sejedná o příjem nebo o výdaj
         if (Blok.kategorie == KategoriePrijemVydaj.Prijem)
         {
            PrijemVydaj.Content = "PŘÍJMY";
            PrijemVydaj.Foreground = BarvaZelena;
         }
         else
         {
            PrijemVydaj.Content = "VÝDAJE";
            PrijemVydaj.Foreground = BarvaCervena;
         }


         // Přidání okrajů na plátno
         MyCanvas.Children.Add(okraje);
         Canvas.SetLeft(okraje, 0);
         Canvas.SetTop(okraje, 0);

         // Přidání pozadí na plátno
         MyCanvas.Children.Add(pozadi);
         Canvas.SetLeft(pozadi, 1);
         Canvas.SetTop(pozadi, 1);

         // Přidání dělící čáry na plátno
         MyCanvas.Children.Add(deliciCara);
         Canvas.SetLeft(deliciCara, 0);
         Canvas.SetTop(deliciCara, VyskaCanvas / 3 - 1);

         // Přidání názvu na plátno
         MyCanvas.Children.Add(nazev);
         Canvas.SetLeft(nazev, 4);
         Canvas.SetTop(nazev, 6);

         // Přidání data na plátno
         MyCanvas.Children.Add(datum);
         Canvas.SetLeft(datum, SirkaNazev + 135);
         Canvas.SetTop(datum, 6);

         // Přidání popisku na plátno
         MyCanvas.Children.Add(PrijemVydaj);
         Canvas.SetRight(PrijemVydaj, 11);
         Canvas.SetTop(PrijemVydaj, 6);

         // Přidání přepážky na plátno
         MyCanvas.Children.Add(prepazka);
         MyCanvas.Children.Add(prepazka2);
         Canvas.SetLeft(prepazka, SirkaNazev + 34);
         Canvas.SetTop(prepazka, 1);
         Canvas.SetRight(prepazka2, 200);
         Canvas.SetTop(prepazka2, 1);

         // Přidání celkové hodnoty na plátno
         MyCanvas.Children.Add(hodnota);
         Canvas.SetLeft(hodnota, 24);
         Canvas.SetTop(hodnota, VyskaCanvas / 3 + 20);

         // Přidání počtu záznamů na plátno
         MyCanvas.Children.Add(pocet);
         Canvas.SetRight(pocet, 16);
         Canvas.SetBottom(pocet, 6);
      }

      /// <summary>
      /// Metoda pro vykreslení oblasti pod grafem, tedy popisky osy X a ovládací šipky pro pohyb mezi stránkami grafu.
      /// </summary>
      /// <param name="MyCanvas">Plátno pro vykreslení</param>
      public void VykresliOblastPodGrafemKategorie(Canvas MyCanvas)
      {
         // Pomocné proměnné pro určení pozic prvků na plátně
         int VyskaCanvas = (int)MyCanvas.Height;
         int SirkaCanvas = (int)MyCanvas.Width;

         // Smazání obsahu pro možnost nového vykreslení
         MyCanvas.Children.Clear();

         // Levá šipka pro možnost měnit číslo stránky grafu (pohyb v grafu)
         Path LevaSipka = new Path
         {
            Stroke = BarvaZluta,
            Fill = BarvaCervena,
            StrokeThickness = 5,
            Data = Geometry.Parse("m0,40 l40,-40 l0,20 l50,0 l0,40 l-50,0 l0,20 l-40,-40")
         };

         // Pravá šipka pro možnost měnit číslo stránky grafu (pohyb v grafu)
         Path PravaSipka = new Path
         {
            Stroke = BarvaZluta,
            Fill = BarvaCervena,
            StrokeThickness = 5,
            Data = Geometry.Parse("m0,40 l0,20 l50,0 l0,20 l40,-40 l-40,-40 l0,20 l-50,0 l0,20")
         };

         // Přidání události pro možnost kliknutí na šipku
         LevaSipka.MouseDown += LevaSipkaProStrankyGrafuKategorie_MouseDown;
         PravaSipka.MouseDown += PravaSipkaProStrankyGrafuKategorie_MouseDown;

         // Vykreslení šipek na plátno
         MyCanvas.Children.Add(LevaSipka);
         MyCanvas.Children.Add(PravaSipka);
         Canvas.SetRight(LevaSipka, 140);
         Canvas.SetBottom(LevaSipka, 20);
         Canvas.SetRight(PravaSipka, 20);
         Canvas.SetBottom(PravaSipka, 20);


         // Vypsání popisků osy X (názvy kategorií)
         for (int i = 1; i <= ZobrazeneKategorie.Count; i++) 
         {
            // Vytvoření popisku
            Label popisek = new Label
            {
               FontSize = 18,
               Content = ZobrazeneKategorie[i - 1],
               Foreground = Brushes.DarkRed
            };

            // Naklonění textu
            RotateTransform rotace = new RotateTransform(45);
            popisek.RenderTransform = rotace;

            // Vykreslení popisku na plátno
            MyCanvas.Children.Add(popisek);
            Canvas.SetLeft(popisek, SouradniceX_PrvniSloupecKategorie + ((i - 1) * OdstupHodnotX_Kategorie));
            Canvas.SetTop(popisek, -20);
         }
      }



      /// <summary>
      /// Obsluha události při kliknutí na šipku. Přepne se zobrazení na jiný graf, tedy vykreslí se nový Kategorie.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PravaSipkaPrepinaniGrafu_MouseDown(object sender, MouseButtonEventArgs e)
      {
         // Vykreslení grafu Kategorie
         UvodniNastaveniGrafuKategorie();
      }

      /// <summary>
      /// Obsluha události při kliknutí na šipku. Přepne se zobrazení na jiný graf, tedy vykreslí se nový graf Prehled.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void LevaSipkaPrepinaniGrafu_MouseDown(object sender, MouseButtonEventArgs e)
      {
         // Vykreslení grafu Přehled
         UvodniNastaveniGrafuPrehledu();
      }

      /// <summary>
      /// Obsluha události při změně zaškrtávacího pole pro možnost nastavení nebo zrušení zobrazení příjmů v grafu
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void VykresleniPrijmuCheck_Click(object sender, RoutedEventArgs e)
      {
         // Převedení zpět na původní datový typ
         CheckBox checkBox = sender as CheckBox;

         // Nastavení příznakového bitu dle toho, zda je zaškrtávací políčko zatrhnuto
         if (checkBox.IsChecked == true)
            VykreslitPrijmy = 1;
         else
            VykreslitPrijmy = 0;

         // Aktualizace vykreslení grafu
         if (VykreslenyGraf == 0)
            AktualizujGrafPrehledu();
         else
            AktualizujGrafKategorii();
      }

      /// <summary>
      /// Obsluha události při změně zaškrtávacího pole pro možnost nastavení nebo zrušení zobrazení výdajů v grafu
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void VykresleniVydajuCheck_Click(object sender, RoutedEventArgs e)
      {
         // Převedení zpět na původní datový typ
         CheckBox checkBox = sender as CheckBox;

         // Nastavení příznakového bitu dle toho, zda je zaškrtávací políčko zatrhnuto
         if (checkBox.IsChecked == true)
            VykreslitVydaje = 1;
         else
            VykreslitVydaje = 0;

         // Aktualizace vykreslení grafu
         if (VykreslenyGraf == 0)
            AktualizujGrafPrehledu();
         else
            AktualizujGrafKategorii();
      }


      /// <summary>
      /// Obsluha události při kliknutí na pravou šipku. 
      /// Zvýší se číslo stránky a aktualizuje se vykreslení grafu.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PravaSipkaProStrankyGrafuPrehled_MouseDown(object sender, MouseButtonEventArgs e)
      {
         // Pokud je vykreslena poslední stránka grafu, událot se neobslouží
         if (CisloStrankyGrafu == MaximalniPocetStranGrafu - 1)
            return;

         // Změna čísla stránky s aktualizací vykreslení grafu
         CisloStrankyGrafu++;
         AktualizujGrafPrehledu();
         return;
      }

      /// <summary>
      /// Obsluha události při kliknutí na levou šipku. 
      /// Sníží se číslo stránky a aktualizuje se vykreslení grafu.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void LevaSipkaProStrankyGrafuPrehled_MouseDown(object sender, MouseButtonEventArgs e)
      {
         // Pokud je vykreslena první stránka grafu, událost se neobslouží
         if (CisloStrankyGrafu == 0)
            return;

         // Změna čísla stránky s aktualizací vykreslení grafu
         CisloStrankyGrafu--;
         AktualizujGrafPrehledu();
         return;
      }


      /// <summary>
      /// Obsluha události při odjetí kurzoru myši ze sloupce
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void VydajPrehled_MouseLeave(object sender, MouseEventArgs e)
      {
         if (InfoVykresleno == 1)
         {
            // Vykreslení spodní oblasti grafu na plátno
            PlatnoInfoOblastiCanvas.Children.Clear();
            VykresliOblastPodGrafemPrehledu(PlatnoInfoOblastiCanvas);

            // Nastavení příznakového bitu pro možnost dalšího vykreslení
            InfoVykresleno = 0;
         }
      }

      /// <summary>
      /// Obsluha události při najetí kurzoru myši na vybraný sloupec
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void VydajPrehled_MouseMove(object sender, MouseEventArgs e)
      {
         if (InfoVykresleno == 0)
         {
            // Převedení objektu na obdélník
            Rectangle sloupec = sender as Rectangle;

            // Zjištění indexu vybraného sloupce odpovídajícího bloku v kolekci bloků
            int index = int.Parse(sloupec.Name.Substring(7));

            // Zobrazení informačního okna pro vybraný objekt
            VykresliInfoBublinuGrafuPrehledu(PlatnoInfoOblastiCanvas, ZobrazeneBlokyVydajuPrehled[index]);

            // Nastavení příznakového bitu pro možnost dalšího vykreslení
            InfoVykresleno = 1;
         }
      }

      /// <summary>
      /// Obsluha události při odjetí kurzoru myši ze sloupce
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PrijemPrehled_MouseLeave(object sender, MouseEventArgs e)
      {
         if (InfoVykresleno == 1)
         {
            // Vykreslení spodní oblasti grafu na plátno
            PlatnoInfoOblastiCanvas.Children.Clear();
            VykresliOblastPodGrafemPrehledu(PlatnoInfoOblastiCanvas);

            // Nastavení příznakového bitu pro možnost dalšího vykreslení
            InfoVykresleno = 0;
         }
      }

      /// <summary>
      /// Obsluha události při najetí kurzoru myši na vybraný sloupec
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PrijemPrehled_MouseMove(object sender, MouseEventArgs e)
      {
         if (InfoVykresleno == 0)
         {
            // Převedení objektu na obdélník
            Rectangle sloupec = sender as Rectangle;

            // Zjištění indexu vybraného sloupce odpovídajícího bloku v kolekci bloků
            int index = int.Parse(sloupec.Name.Substring(7));

            // Zobrazení informačního okna pro vybraný objekt
            VykresliInfoBublinuGrafuPrehledu(PlatnoInfoOblastiCanvas, ZobrazeneBlokyPrijmuPrehled[index]);

            // Nastavení příznakového bitu pro možnost dalšího vykreslení
            InfoVykresleno = 1;
         }
      }

      /// <summary>
      /// Aktualizace vykreslení grafu pro zvolené zobrazení při výběru zobrazení roků
      /// </summary>
      /// <param name="sender">Zvolená objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void Roky_Checked(object sender, RoutedEventArgs e)
      {
         ZobrazeniPrehledu = ZvoleneZobrazeniPrehledu.Roky;
         VykresliOvladaciPrvkyGrafuPrehledu(PlatnoOvladacichPrvkuCanvas, ZobrazeniPrehledu);
         VytvorBlokyProGrafPrehled(ZobrazeniPrehledu);
         AktualizujGrafPrehledu();
      }

      /// <summary>
      /// Aktualizace vykreslení grafu pro zvolené zobrazení při výběru zobrazení měsíců
      /// </summary>
      /// <param name="sender">Zvolená objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void Mesice_Checked(object sender, RoutedEventArgs e)
      {
         ZobrazeniPrehledu = ZvoleneZobrazeniPrehledu.Mesice;
         VykresliOvladaciPrvkyGrafuPrehledu(PlatnoOvladacichPrvkuCanvas, ZobrazeniPrehledu);
         VytvorBlokyProGrafPrehled(ZobrazeniPrehledu);
         AktualizujGrafPrehledu();
      }

      /// <summary>
      /// Aktualizace vykreslení grafu pro zvolené zobrazení při výběru zobrazení týdnů
      /// </summary>
      /// <param name="sender">Zvolená objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void Tydny_Checked(object sender, RoutedEventArgs e)
      {
         ZobrazeniPrehledu = ZvoleneZobrazeniPrehledu.Tydny;
         VykresliOvladaciPrvkyGrafuPrehledu(PlatnoOvladacichPrvkuCanvas, ZobrazeniPrehledu);
         VytvorBlokyProGrafPrehled(ZobrazeniPrehledu);
         AktualizujGrafPrehledu();
      }

      /// <summary>
      /// Aktualizace vykreslení grafu pro zvolené zobrazení při výběru zobrazení dnů
      /// </summary>
      /// <param name="sender">Zvolená objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void Dny_Checked(object sender, RoutedEventArgs e)
      {
         ZobrazeniPrehledu = ZvoleneZobrazeniPrehledu.Dny;
         VykresliOvladaciPrvkyGrafuPrehledu(PlatnoOvladacichPrvkuCanvas, ZobrazeniPrehledu);
         VytvorBlokyProGrafPrehled(ZobrazeniPrehledu);
         AktualizujGrafPrehledu();
      }

      /// <summary>
      /// Obsluha události při změně vybraného roku
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void RokKoncovyVyber_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         try
         {
            // Převedení zvoleného objektu na původní datový typ
            ComboBox Box = sender as ComboBox;

            // Kontrola zda počáteční rok je menší než koncový
            if ((Box.SelectedIndex + 2019) < VybranyRok)
            {
               Box.SelectedIndex = VybranyKoncovyRok - 2019;
               throw new ArgumentException("Koncový rok musí být větší než počáteční");
            }
               
            // Načtení roku dle zvoleného indexu v rozbalovacím okně
            VybranyKoncovyRok = Box.SelectedIndex + 2019;

            // Aktualizace vykreslení grafu s novými daty dle výběru
            VytvorBlokyProGrafPrehled(ZobrazeniPrehledu);
            AktualizujGrafPrehledu();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Exclamation);
         }
      }

      /// <summary>
      /// Obsluha události při změně vybraného roku
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void RokVyber_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         // Převedení zvoleného objektu na původní datový typ
         ComboBox Box = sender as ComboBox;

         // Načtení roku dle zvoleného indexu v rozbalovacím okně
         VybranyRok = Box.SelectedIndex + 2019;

         // Aktualizace vykreslení grafu s novými daty dle výběru
         VytvorBlokyProGrafPrehled(ZobrazeniPrehledu);
         AktualizujGrafPrehledu();
      }

      /// <summary>
      /// Obsluha události při změně vybraného měsíce
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void MesicVyber_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         try
         {
            // Převedení zvoleného objektu na původní datový typ
            ComboBox Box = sender as ComboBox;

            // Kontrolní podmínka pro případ, že uživatel zadá budoucí měsíc
            if (VybranyRok == DateTime.Now.Year && (Box.SelectedIndex + 1) > DateTime.Now.Month)
            {
               Box.SelectedIndex = VybranyMesic - 1;
               throw new ArgumentException("Nelze zobrazit data z budoucnosti!");
            }
               
            // Načtení měsíce dle zvoleného indexu v rozbalovacím okně
            VybranyMesic = Box.SelectedIndex + 1;

            // Aktualizace vykreslení grafu s novými daty dle výběru
            VytvorBlokyProGrafPrehled(ZobrazeniPrehledu);
            AktualizujGrafPrehledu();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Exclamation);
         }
      }


      /// <summary>
      /// Obsluha události při odjetí kurzoru myši ze sloupce
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void VydajKategorie_MouseLeave(object sender, MouseEventArgs e)
      {
         if (InfoVykresleno == 1)
         {
            // Vykreslení spodní oblasti grafu na plátno
            PlatnoInfoOblastiCanvas.Children.Clear();
            VykresliOblastPodGrafemKategorie(PlatnoInfoOblastiCanvas);

            // Nastavení příznakového bitu pro možnost dalšího vykreslení
            InfoVykresleno = 0;
         }
      }

      /// <summary>
      /// Obsluha události při odjetí kurzoru myši ze sloupce
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PrijemKategorie_MouseLeave(object sender, MouseEventArgs e)
      {
         if (InfoVykresleno == 1)
         {
            // Vykreslení spodní oblasti grafu na plátno
            PlatnoInfoOblastiCanvas.Children.Clear();
            VykresliOblastPodGrafemKategorie(PlatnoInfoOblastiCanvas);

            // Nastavení příznakového bitu pro možnost dalšího vykreslení
            InfoVykresleno = 0;
         }
      }

      /// <summary>
      /// Obsluha události při najetí kurzoru myši na vybraný sloupec
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void VydajKategorie_MouseMove(object sender, MouseEventArgs e)
      {
         if (InfoVykresleno == 0)
         {
            // Převedení objektu na obdélník
            Rectangle sloupec = sender as Rectangle;

            // Zjištění indexu vybraného sloupce odpovídajícího bloku v kolekci bloků
            int index = int.Parse(sloupec.Name.Substring(7));

            // Zobrazení informačního okna pro vybraný objekt
            VykresliInfoBublinuGrafuKategorie(PlatnoInfoOblastiCanvas, ZobrazeneBlokyVydajuKategorie[index]);

            // Nastavení příznakového bitu pro možnost dalšího vykreslení
            InfoVykresleno = 1;
         }
      }

      /// <summary>
      /// Obsluha události při najetí kurzoru myši na vybraný sloupec
      /// </summary>
      /// <param name="sender">Vybraný objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PrijemKategorie_MouseMove(object sender, MouseEventArgs e)
      {
         if (InfoVykresleno == 0)
         {
            // Převedení objektu na obdélník
            Rectangle sloupec = sender as Rectangle;

            // Zjištění indexu vybraného sloupce odpovídajícího bloku v kolekci bloků
            int index = int.Parse(sloupec.Name.Substring(7));

            // Zobrazení informačního okna pro vybraný objekt
            VykresliInfoBublinuGrafuKategorie(PlatnoInfoOblastiCanvas, ZobrazeneBlokyPrijmuKategorie[index]);

            // Nastavení příznakového bitu pro možnost dalšího vykreslení
            InfoVykresleno = 1;
         }
      }

      /// <summary>
      /// Obsluha události při kliknutí na pravou šipku. 
      /// Zvýší se číslo stránky a aktualizuje se vykreslení grafu.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void PravaSipkaProStrankyGrafuKategorie_MouseDown(object sender, MouseButtonEventArgs e)
      {
         // Pokud je vykreslena poslední stránka grafu, událot se neobslouží
         if (CisloStrankyGrafu == MaximalniPocetStranGrafu - 1)
            return;

         // Změna čísla stránky s aktualizací vykreslení grafu
         CisloStrankyGrafu++;
         AktualizujGrafKategorii();
         return;
      }

      /// <summary>
      /// Obsluha události při kliknutí na levou šipku. 
      /// Sníží se číslo stránky a aktualizuje se vykreslení grafu.
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void LevaSipkaProStrankyGrafuKategorie_MouseDown(object sender, MouseButtonEventArgs e)
      {
         // Pokud je vykreslena první stránka grafu, událost se neobslouží
         if (CisloStrankyGrafu == 0)
            return;

         // Změna čísla stránky s aktualizací vykreslení grafu
         CisloStrankyGrafu--;
         AktualizujGrafKategorii();
         return;
      }

      /// <summary>
      /// Obsluha události při změně zadaného koncového data pro výběr dat k zobrazení grafu Kategorie
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void DatumKoncoveKategorie_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
      {
         try
         {
            // Převedení vybraného objektu na původní datový typ
            DatePicker Datum = sender as DatePicker;

            // Načtení zadaného do pomocné proměnné
            DateTime NacteneDatum = validator.NactiDatum(Datum.SelectedDate);

            // Ošetření pro případ že uživatel zadá koncové datum menší než je počáteční
            if (NacteneDatum < PocatecniDatum)
            {
               Datum.SelectedDate = KoncoveDatum;
               throw new ArgumentException("Koncové datum je menší než počáteční!");
            }
            else
            {
               // Načtení zadaného data do interní proměnné
               KoncoveDatum = NacteneDatum;
            }  
            
            // Nastavení grafu na první stránku
            CisloStrankyGrafu = 0;

            // Aktualizace vykreslení grafu s novým koncovým datem pro výběr dat k zobrazení
            AktualizujGrafKategorii();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
         }
      }

      /// <summary>
      /// Obsluha události při změně zadaného počátečního data pro výběr dat k zobrazení grafu Kategorie
      /// </summary>
      /// <param name="sender">Zvolený objekt</param>
      /// <param name="e">Vyvolaná událost</param>
      private void DatumPocatecniKategorie_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
      {
         try
         {
            // Převedení vybraného objektu na původní datový typ
            DatePicker Datum = sender as DatePicker;

            // Načtení zadaného do pomocné proměnné
            DateTime NacteneDatum = validator.NactiDatum(Datum.SelectedDate);

            // Ošetření pro případ že uživatel zadá počáteční datum větší než je koncové
            if (NacteneDatum > KoncoveDatum)
            {
               Datum.SelectedDate = PocatecniDatum;
               throw new ArgumentException("Počáteční datum je větší než koncové!");
            }
            else
            {
               // Načtení zadaného data do interní proměnné
               PocatecniDatum = NacteneDatum;
            }
            
            // Nastavení grafu na první stránku
            CisloStrankyGrafu = 0;

            // Aktualizace vykreslení grafu s novým počátečním datem pro výběr dat k zobrazení
            AktualizujGrafKategorii();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
         }
      }


   }
}
